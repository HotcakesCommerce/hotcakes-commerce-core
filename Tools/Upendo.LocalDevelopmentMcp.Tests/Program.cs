using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Upendo.LocalDevelopmentMcp.Configuration;
using Upendo.LocalDevelopmentMcp.Infrastructure;
using Upendo.LocalDevelopmentMcp.Validation;

if (args.Length > 0)
{
    return await RunHelperAsync(args);
}

return await RunRegressionTestsAsync();

static async Task<int> RunRegressionTestsAsync()
{
    var tests = new (string Name, Func<Task> Run)[]
    {
        ("Git status completes promptly", GitStatusCompletesPromptlyAsync),
        ("Concurrent stdout and stderr are captured", ConcurrentOutputIsCapturedAsync),
        ("Large output is drained and truncated", LargeOutputIsDrainedAsync),
        ("Cancellation terminates the child process", CancellationTerminatesProcessAsync),
        ("Timeout terminates the complete process tree", TimeoutTerminatesProcessTreeAsync),
        ("Repository scanning prunes exclusions and duplicates", RepositoryScanningIsScopedAsync),
        ("Explicit files remain narrowly scoped", ExplicitFilesAreScopedAsync),
        ("Build catalog validates supported definitions", BuildCatalogValidatesDefinitionsAsync),
        ("Build catalog rejects unsafe definitions", BuildCatalogRejectsUnsafeDefinitionsAsync),
        ("Packaged build catalog is internally valid", PackagedBuildCatalogIsValidAsync)
    };

    var failures = 0;
    foreach (var test in tests)
    {
        try
        {
            await test.Run();
            Console.WriteLine($"PASS: {test.Name}");
        }
        catch (Exception exception)
        {
            failures++;
            Console.Error.WriteLine($"FAIL: {test.Name}{Environment.NewLine}{exception}");
        }
    }

    Console.WriteLine($"{tests.Length - failures}/{tests.Length} regression tests passed.");
    return failures == 0 ? 0 : 1;
}

static async Task GitStatusCompletesPromptlyAsync()
{
    var root = FindRepositoryRoot();
    var runner = CreateRunner(root, [], gitTimeoutSeconds: 10, buildTimeoutSeconds: 10);
    var stopwatch = Stopwatch.StartNew();

    var result = await runner.RunGitAsync(["status", "--short", "--branch"], CancellationToken.None);

    Assert(result.Success, $"Git status failed: {result.StandardError}");
    Assert(stopwatch.Elapsed < TimeSpan.FromSeconds(5), $"Git status took {stopwatch.Elapsed}.");
}

static async Task ConcurrentOutputIsCapturedAsync()
{
    var runner = CreateHelperRunner("dual-output", 10);
    var result = await runner.RunBuildAsync("test-helper", CancellationToken.None);

    Assert(result.Success, result.StandardError);
    Assert(result.StandardOutput.Contains("stdout-complete", StringComparison.Ordinal), "stdout was not captured.");
    Assert(result.StandardError.Contains("stderr-complete", StringComparison.Ordinal), "stderr was not captured.");
}

static async Task LargeOutputIsDrainedAsync()
{
    var runner = CreateHelperRunner("large-output", 10);
    var result = await runner.RunBuildAsync("test-helper", CancellationToken.None);

    Assert(result.Success, result.StandardError);
    Assert(result.StandardOutput.EndsWith("[output truncated]", StringComparison.Ordinal), "stdout was not marked as truncated.");
    Assert(result.StandardError.EndsWith("[output truncated]", StringComparison.Ordinal), "stderr was not marked as truncated.");
}

static async Task CancellationTerminatesProcessAsync()
{
    var runner = CreateHelperRunner("sleep", 30);
    using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(250));
    var stopwatch = Stopwatch.StartNew();

    await AssertThrowsAsync<OperationCanceledException>(
        () => runner.RunBuildAsync("test-helper", cancellation.Token));

    Assert(stopwatch.Elapsed < TimeSpan.FromSeconds(5), $"Cancellation took {stopwatch.Elapsed}.");
}

static async Task TimeoutTerminatesProcessTreeAsync()
{
    var temporaryDirectory = CreateTemporaryDirectory();
    try
    {
        var markerPath = Path.Combine(temporaryDirectory, "orphan-marker.txt");
        var runner = CreateHelperRunner("spawn-marker-child", 1, markerPath);
        var stopwatch = Stopwatch.StartNew();

        await AssertThrowsAsync<TimeoutException>(
            () => runner.RunBuildAsync("test-helper", CancellationToken.None));

        Assert(stopwatch.Elapsed < TimeSpan.FromSeconds(5), $"Timeout cleanup took {stopwatch.Elapsed}.");
        await Task.Delay(TimeSpan.FromSeconds(3));
        Assert(!File.Exists(markerPath), "A descendant process survived timeout cleanup.");
    }
    finally
    {
        DeleteTemporaryDirectory(temporaryDirectory);
    }
}

static Task RepositoryScanningIsScopedAsync()
{
    var temporaryDirectory = CreateTemporaryDirectory();
    try
    {
        var sourceDirectory = Directory.CreateDirectory(Path.Combine(temporaryDirectory, "src")).FullName;
        var excludedDirectory = Directory.CreateDirectory(Path.Combine(sourceDirectory, "node_modules", "package")).FullName;
        File.WriteAllText(Path.Combine(sourceDirectory, "included.txt"), "needle");
        File.WriteAllText(Path.Combine(excludedDirectory, "excluded.txt"), "needle");

        var options = new RepositoryOptions
        {
            Root = temporaryDirectory,
            AllowedRoots = [".", "src"],
            ExcludedGlobs = ["**/node_modules/**"]
        };
        var context = new RepositoryContext(Options.Create(options));
        var paths = new SafePathPolicy(context);
        var scanner = new RepositoryScanner(context, paths);

        var files = scanner.EnumerateFiles().Select(paths.ToRelativePath).ToArray();
        Assert(files.SequenceEqual(["src/included.txt"]), $"Unexpected scan results: {string.Join(", ", files)}");
    }
    finally
    {
        DeleteTemporaryDirectory(temporaryDirectory);
    }

    return Task.CompletedTask;
}

static Task ExplicitFilesAreScopedAsync()
{
    var temporaryDirectory = CreateTemporaryDirectory();
    try
    {
        File.WriteAllText(Path.Combine(temporaryDirectory, "AGENTS.md"), "standards");
        File.WriteAllText(Path.Combine(temporaryDirectory, "private.txt"), "private");
        var excludedDirectory = Directory.CreateDirectory(Path.Combine(temporaryDirectory, "excluded")).FullName;
        File.WriteAllText(Path.Combine(excludedDirectory, "AGENTS.md"), "excluded standards");
        var sourceDirectory = Directory.CreateDirectory(Path.Combine(temporaryDirectory, "src")).FullName;
        File.WriteAllText(Path.Combine(sourceDirectory, "included.txt"), "included");

        var options = new RepositoryOptions
        {
            Root = temporaryDirectory,
            AllowedRoots = ["src"],
            AllowedFiles = ["AGENTS.md", "excluded/AGENTS.md"],
            ExcludedGlobs = ["excluded/**"]
        };
        var context = new RepositoryContext(Options.Create(options));
        var paths = new SafePathPolicy(context);
        var scanner = new RepositoryScanner(context, paths);

        Assert(scanner.ReadText("AGENTS.md") == "standards", "The explicitly allowed file was not readable.");
        AssertThrows<UnauthorizedAccessException>(() => scanner.ReadText("private.txt"));
        AssertThrows<UnauthorizedAccessException>(() => scanner.ReadText("excluded/AGENTS.md"));
        Assert(scanner.ReadText("src/included.txt") == "included", "The allowed directory was not readable.");
    }
    finally
    {
        DeleteTemporaryDirectory(temporaryDirectory);
    }

    return Task.CompletedTask;
}

static Task BuildCatalogValidatesDefinitionsAsync()
{
    var temporaryDirectory = CreateTemporaryDirectory();
    try
    {
        Directory.CreateDirectory(Path.Combine(temporaryDirectory, "src"));
        File.WriteAllText(Path.Combine(temporaryDirectory, "src", "Example.csproj"), "<Project />");
        File.WriteAllText(Path.Combine(temporaryDirectory, "package.json"), "{\"scripts\":{\"build\":\"echo build\"}}");

        var options = new RepositoryOptions
        {
            Root = temporaryDirectory,
            AllowedRoots = ["."],
            Builds =
            [
                new BuildDefinition { Name = "msbuild-example", Kind = "msbuild", WorkingDirectory = ".", Executable = "msbuild", Arguments = ["src/Example.csproj"] },
                new BuildDefinition { Name = "dotnet-example", Kind = "dotnet", WorkingDirectory = ".", Executable = "dotnet", Arguments = ["build", "src/Example.csproj"] },
                new BuildDefinition { Name = "npm-example", Kind = "npm", WorkingDirectory = ".", Executable = "npm", Arguments = ["run", "build"] }
            ]
        };
        var report = new BuildCatalogValidator(
            new RepositoryContext(Options.Create(options)),
            new SafePathPolicy(new RepositoryContext(Options.Create(options)))).Validate();

        Assert(report.Success, string.Join(Environment.NewLine, report.Findings));
        Assert(report.Findings.Count == 0, "Valid definitions should not produce findings.");
    }
    finally
    {
        DeleteTemporaryDirectory(temporaryDirectory);
    }

    return Task.CompletedTask;
}

static Task PackagedBuildCatalogIsValidAsync()
{
    var root = FindRepositoryRoot();
    var json = File.ReadAllText(Path.Combine(root, "Tools", "Upendo.LocalDevelopmentMcp", "appsettings.json"));
    var configuration = JsonSerializer.Deserialize<ConfigurationFile>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        ?? throw new InvalidOperationException("The packaged MCP configuration could not be deserialized.");
    var context = new RepositoryContext(Options.Create(configuration.Repository));
    var report = new BuildCatalogValidator(context, new SafePathPolicy(context)).Validate();

    Assert(report.Success, string.Join(Environment.NewLine, report.Findings.Select(x => $"{x.Code}: {x.Message}")));
    return Task.CompletedTask;
}

static Task BuildCatalogRejectsUnsafeDefinitionsAsync()
{
    var temporaryDirectory = CreateTemporaryDirectory();
    try
    {
        var options = new RepositoryOptions
        {
            Root = temporaryDirectory,
            AllowedRoots = ["."],
            Builds =
            [
                new BuildDefinition { Name = "duplicate", Kind = "dotnet", WorkingDirectory = ".", Executable = "dotnet", Arguments = ["build", "missing.csproj"] },
                new BuildDefinition { Name = "DUPLICATE", Kind = "shell", WorkingDirectory = ".", Executable = "cmd.exe", Arguments = ["/c", "echo", "ok"] },
                new BuildDefinition { Name = "unsafe-npm", Kind = "npm", WorkingDirectory = ".", Executable = "npm", Arguments = ["install"] }
            ]
        };
        var context = new RepositoryContext(Options.Create(options));
        var report = new BuildCatalogValidator(context, new SafePathPolicy(context)).Validate();
        var codes = report.Findings.Select(x => x.Code).ToHashSet(StringComparer.Ordinal);

        Assert(!report.Success, "Unsafe definitions should fail validation.");
        Assert(codes.Contains("BUILD_NAME_DUPLICATE"), "Duplicate names were not rejected.");
        Assert(codes.Contains("BUILD_KIND_UNSUPPORTED"), "Unsupported kinds were not rejected.");
        Assert(codes.Contains("BUILD_PROJECT_MISSING"), "Missing project targets were not rejected.");
        Assert(codes.Contains("BUILD_ARGUMENT_INSTALL"), "Package installation was not rejected.");
    }
    finally
    {
        DeleteTemporaryDirectory(temporaryDirectory);
    }

    return Task.CompletedTask;
}

static ApprovedProcessRunner CreateHelperRunner(string helperMode, int timeoutSeconds, params string[] helperArguments)
{
    var executable = Environment.ProcessPath ?? throw new InvalidOperationException("The test executable path is unavailable.");
    var arguments = new[] { helperMode }.Concat(helperArguments).ToArray();
    var build = new BuildDefinition
    {
        Name = "test-helper",
        Kind = "test",
        WorkingDirectory = ".",
        Executable = executable,
        Arguments = arguments
    };

    return CreateRunner(FindRepositoryRoot(), [build], gitTimeoutSeconds: 10, buildTimeoutSeconds: timeoutSeconds);
}

static ApprovedProcessRunner CreateRunner(
    string root,
    BuildDefinition[] builds,
    int gitTimeoutSeconds,
    int buildTimeoutSeconds)
{
    var options = new RepositoryOptions
    {
        Root = root,
        AllowedRoots = ["."],
        GitProcessTimeoutSeconds = gitTimeoutSeconds,
        BuildProcessTimeoutSeconds = buildTimeoutSeconds,
        Builds = builds
    };
    var context = new RepositoryContext(Options.Create(options));
    return new ApprovedProcessRunner(context, new SafePathPolicy(context));
}

static string FindRepositoryRoot()
{
    var directory = new DirectoryInfo(AppContext.BaseDirectory);
    while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, ".git")))
    {
        directory = directory.Parent;
    }

    return directory?.FullName ?? throw new DirectoryNotFoundException("Unable to locate the repository root.");
}

static string CreateTemporaryDirectory()
{
    var root = Path.Combine(Path.GetTempPath(), "Upendo.LocalDevelopmentMcp.Tests");
    var path = Path.Combine(root, Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(path);
    return path;
}

static void DeleteTemporaryDirectory(string path)
{
    var expectedRoot = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "Upendo.LocalDevelopmentMcp.Tests"));
    var resolvedPath = Path.GetFullPath(path);
    if (!resolvedPath.StartsWith(expectedRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException($"Refusing to delete unexpected test path: {resolvedPath}");
    }

    if (Directory.Exists(resolvedPath))
    {
        Directory.Delete(resolvedPath, recursive: true);
    }
}

static async Task AssertThrowsAsync<TException>(Func<Task> action) where TException : Exception
{
    try
    {
        await action();
    }
    catch (TException)
    {
        return;
    }

    throw new InvalidOperationException($"Expected {typeof(TException).Name}.");
}

static void AssertThrows<TException>(Action action) where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }

    throw new InvalidOperationException($"Expected {typeof(TException).Name}.");
}

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

static async Task<int> RunHelperAsync(string[] arguments)
{
    switch (arguments[0])
    {
        case "dual-output":
            await Task.WhenAll(
                Console.Out.WriteLineAsync("stdout-complete"),
                Console.Error.WriteLineAsync("stderr-complete"));
            return 0;
        case "large-output":
            await Task.WhenAll(
                Console.Out.WriteAsync(new string('O', 2_100_000)),
                Console.Error.WriteAsync(new string('E', 2_100_000)));
            return 0;
        case "sleep":
            await Task.Delay(TimeSpan.FromSeconds(30));
            return 0;
        case "spawn-marker-child":
            StartMarkerChild(arguments[1]);
            await Task.Delay(TimeSpan.FromSeconds(30));
            return 0;
        case "write-marker":
            await Task.Delay(TimeSpan.FromSeconds(2));
            await File.WriteAllTextAsync(arguments[1], "descendant survived");
            return 0;
        default:
            throw new InvalidOperationException($"Unknown helper mode: {arguments[0]}");
    }
}

static void StartMarkerChild(string markerPath)
{
    var executable = Environment.ProcessPath ?? throw new InvalidOperationException("The test executable path is unavailable.");
    var startInfo = new ProcessStartInfo
    {
        FileName = executable,
        UseShellExecute = false,
        CreateNoWindow = true
    };
    startInfo.ArgumentList.Add("write-marker");
    startInfo.ArgumentList.Add(markerPath);
    Process.Start(startInfo)?.Dispose();
}

sealed class ConfigurationFile
{
    public RepositoryOptions Repository { get; set; } = new();
}
