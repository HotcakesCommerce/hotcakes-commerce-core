using System.Text.Json;
using Upendo.LocalDevelopmentMcp.Configuration;
using Upendo.LocalDevelopmentMcp.Infrastructure;
using Upendo.LocalDevelopmentMcp.Models;

namespace Upendo.LocalDevelopmentMcp.Validation;

public sealed class BuildCatalogValidator(RepositoryContext context, SafePathPolicy paths)
{
    private static readonly HashSet<string> SupportedKinds = new(StringComparer.OrdinalIgnoreCase)
    {
        "msbuild",
        "dotnet",
        "npm"
    };

    public ValidationReport Validate()
    {
        var findings = new List<ValidationFinding>();
        var builds = context.Options.Builds;

        if (builds.Length == 0)
        {
            findings.Add(new("warning", "BUILD_CATALOG_EMPTY", ".upendo-mcp.json", null,
                "The effective build catalog is empty. An explicit Builds array in .upendo-mcp.json replaces the packaged defaults."));
        }

        var duplicateNames = builds
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var build in builds)
        {
            ValidateDefinition(build, duplicateNames, findings);
        }

        return new ValidationReport("build-catalog", findings.All(x => x.Severity != "error"), findings);
    }

    private void ValidateDefinition(
        BuildDefinition build,
        ISet<string> duplicateNames,
        ICollection<ValidationFinding> findings)
    {
        var path = string.IsNullOrWhiteSpace(build.Name) ? "<unnamed build>" : $"builds/{build.Name}";

        if (string.IsNullOrWhiteSpace(build.Name))
        {
            AddError(findings, "BUILD_NAME_REQUIRED", path, "Build name is required.");
        }
        else if (duplicateNames.Contains(build.Name))
        {
            AddError(findings, "BUILD_NAME_DUPLICATE", path, $"Build name '{build.Name}' is duplicated case-insensitively.");
        }

        var kind = build.Kind?.Trim() ?? string.Empty;
        if (!SupportedKinds.Contains(kind))
        {
            AddError(findings, "BUILD_KIND_UNSUPPORTED", path,
                $"Build kind '{build.Kind}' is unsupported. Supported kinds are: msbuild, dotnet, npm.");
            return;
        }

        ValidateWorkingDirectory(build, path, findings, out var workingDirectory);
        ValidateExecutable(build, kind, path, findings);
        ValidateArguments(build, kind, path, findings, workingDirectory);
    }

    private void ValidateWorkingDirectory(
        BuildDefinition build,
        string path,
        ICollection<ValidationFinding> findings,
        out string? workingDirectory)
    {
        workingDirectory = null;
        try
        {
            workingDirectory = paths.ResolveWorkingDirectory(build.WorkingDirectory);
        }
        catch (Exception exception) when (exception is ArgumentException or DirectoryNotFoundException or UnauthorizedAccessException)
        {
            AddError(findings, "BUILD_WORKING_DIRECTORY", path,
                $"Working directory '{build.WorkingDirectory}' is invalid: {exception.Message}");
        }
    }

    private static void ValidateExecutable(
        BuildDefinition build,
        string kind,
        string path,
        ICollection<ValidationFinding> findings)
    {
        if (string.IsNullOrWhiteSpace(build.Executable))
        {
            AddError(findings, "BUILD_EXECUTABLE_REQUIRED", path, "Executable is required.");
            return;
        }

        var executableName = Path.GetFileName(build.Executable.Trim());
        var expected = kind switch
        {
            "msbuild" => executableName.Equals("MSBuild.exe", StringComparison.OrdinalIgnoreCase) ||
                         executableName.Equals("msbuild", StringComparison.OrdinalIgnoreCase),
            "dotnet" => executableName.Equals("dotnet.exe", StringComparison.OrdinalIgnoreCase) ||
                        executableName.Equals("dotnet", StringComparison.OrdinalIgnoreCase),
            "npm" => executableName.Equals("npm.cmd", StringComparison.OrdinalIgnoreCase) ||
                     executableName.Equals("npm", StringComparison.OrdinalIgnoreCase),
            _ => false
        };

        if (!expected)
        {
            AddError(findings, "BUILD_EXECUTABLE_KIND", path,
                $"Executable '{build.Executable}' does not match build kind '{kind}'.");
        }

        if (Path.IsPathFullyQualified(build.Executable))
        {
            AddWarning(findings, "BUILD_EXECUTABLE_PORTABILITY", path,
                $"Executable '{build.Executable}' is an absolute machine-specific path; prefer an environment-independent executable name or a documented local override.");
        }
    }

    private void ValidateArguments(
        BuildDefinition build,
        string kind,
        string path,
        ICollection<ValidationFinding> findings,
        string? workingDirectory)
    {
        if (build.Arguments.Any(ContainsShellSyntax))
        {
            AddError(findings, "BUILD_ARGUMENT_SHELL", path,
                "Build arguments contain shell chaining or metacharacters, which are not allowed.");
        }

        var arguments = build.Arguments.ToArray();
        if (kind.Equals("npm", StringComparison.OrdinalIgnoreCase) &&
            arguments.Any(x => string.Equals(x, "install", StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(x, "ci", StringComparison.OrdinalIgnoreCase)))
        {
            AddError(findings, "BUILD_ARGUMENT_INSTALL", path,
                "Package installation commands are not approved build definitions. Install dependencies as a separate human-run prerequisite.");
        }

        if (arguments.Any(x => x.Contains("deploy", StringComparison.OrdinalIgnoreCase) ||
                               x.Contains("publish", StringComparison.OrdinalIgnoreCase)))
        {
            AddError(findings, "BUILD_ARGUMENT_DELIVERY", path,
                "Deployment and publication commands are not approved build definitions.");
        }

        switch (kind.ToLowerInvariant())
        {
            case "msbuild":
                ValidateProjectTarget(build, path, workingDirectory, findings, ".sln", ".slnx", ".csproj");
                break;
            case "dotnet":
                if (arguments.Length == 0 || !string.Equals(arguments[0], "build", StringComparison.OrdinalIgnoreCase))
                {
                    AddError(findings, "BUILD_DOTNET_VERB", path, "Dotnet build definitions must start with the 'build' verb.");
                }
                ValidateProjectTarget(build, path, workingDirectory, findings, ".sln", ".slnx", ".csproj");
                break;
            case "npm":
                ValidateNpmTarget(build, path, workingDirectory, findings);
                break;
        }
    }

    private void ValidateProjectTarget(
        BuildDefinition build,
        string path,
        string? workingDirectory,
        ICollection<ValidationFinding> findings,
        params string[] extensions)
    {
        var target = build.Arguments.FirstOrDefault(x => extensions.Any(extension => x.EndsWith(extension, StringComparison.OrdinalIgnoreCase)));
        if (target is null)
        {
            AddError(findings, "BUILD_PROJECT_TARGET", path,
                $"Build definition must reference a {string.Join(", ", extensions)} file.");
            return;
        }

        if (workingDirectory is null) return;
        try
        {
            var targetPath = Path.GetFullPath(target, workingDirectory);
            var relativeTarget = Path.GetRelativePath(context.Root, targetPath);
            if (relativeTarget == ".." || relativeTarget.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            {
                AddError(findings, "BUILD_PROJECT_SCOPE", path,
                    $"Referenced project or solution file is outside the repository: {target}");
            }
            else if (paths.IsExcluded(targetPath))
            {
                AddError(findings, "BUILD_PROJECT_EXCLUDED", path,
                    $"Referenced project or solution file is excluded by repository policy: {target}");
            }
            else if (!File.Exists(targetPath))
            {
                AddError(findings, "BUILD_PROJECT_MISSING", path,
                    $"Referenced project or solution file does not exist: {target}");
            }
        }
        catch (Exception exception) when (exception is ArgumentException or UnauthorizedAccessException)
        {
            AddError(findings, "BUILD_PROJECT_SCOPE", path,
                $"Referenced project or solution file is outside the approved repository scope: {target} ({exception.Message})");
        }
    }

    private void ValidateNpmTarget(
        BuildDefinition build,
        string path,
        string? workingDirectory,
        ICollection<ValidationFinding> findings)
    {
        var arguments = build.Arguments.ToArray();
        if (arguments.Length < 2 || !string.Equals(arguments[0], "run", StringComparison.OrdinalIgnoreCase))
        {
            AddError(findings, "BUILD_NPM_SCRIPT", path, "Npm build definitions must use 'run <script>'.");
            return;
        }

        if (workingDirectory is null) return;
        var packagePath = Path.Combine(workingDirectory, "package.json");
        if (!File.Exists(packagePath))
        {
            AddError(findings, "BUILD_PACKAGE_MISSING", path, "The npm working directory does not contain package.json.");
            return;
        }

        try
        {
            using var document = JsonDocument.Parse(File.ReadAllText(packagePath));
            if (!document.RootElement.TryGetProperty("scripts", out var scripts) ||
                scripts.ValueKind != JsonValueKind.Object ||
                !scripts.TryGetProperty(arguments[1], out _))
            {
                AddError(findings, "BUILD_NPM_SCRIPT_MISSING", path,
                    $"package.json does not define the npm script '{arguments[1]}'.");
            }
        }
        catch (JsonException exception)
        {
            AddError(findings, "BUILD_PACKAGE_JSON", path, $"package.json is not valid JSON: {exception.Message}");
        }
        catch (IOException exception)
        {
            AddError(findings, "BUILD_PACKAGE_READ", path, $"package.json could not be read: {exception.Message}");
        }
    }

    private static bool ContainsShellSyntax(string argument) =>
        argument.Contains("&&", StringComparison.Ordinal) ||
        argument.Contains("||", StringComparison.Ordinal) ||
        argument.Contains(';') ||
        argument.Contains('|') ||
        argument.Contains('&');

    private static void AddError(ICollection<ValidationFinding> findings, string code, string path, string message) =>
        findings.Add(new("error", code, path, null, message));

    private static void AddWarning(ICollection<ValidationFinding> findings, string code, string path, string message) =>
        findings.Add(new("warning", code, path, null, message));
}
