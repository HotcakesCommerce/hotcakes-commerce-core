using System.Text.RegularExpressions;
using System.Xml.Linq;
using Upendo.LocalDevelopmentMcp.Models;

namespace Upendo.LocalDevelopmentMcp.Infrastructure;

public sealed partial class RepositoryScanner(RepositoryContext context, SafePathPolicy paths)
{
    public IEnumerable<string> EnumerateFiles()
    {
        var visitedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var visitedDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var allowedRoot in context.Options.AllowedRoots)
        {
            var fullRoot = paths.ResolveReadablePath(allowedRoot);
            foreach (var file in EnumerateFiles(fullRoot, visitedFiles, visitedDirectories))
            {
                yield return file;
            }
        }
    }

    private IEnumerable<string> EnumerateFiles(
        string directory,
        ISet<string> visitedFiles,
        ISet<string> visitedDirectories)
    {
        if (!visitedDirectories.Add(directory) || paths.IsExcluded(directory)) yield break;

        var options = new EnumerationOptions
        {
            RecurseSubdirectories = false,
            IgnoreInaccessible = true,
            AttributesToSkip = FileAttributes.ReparsePoint
        };
        foreach (var file in Directory.EnumerateFiles(directory, "*", options))
        {
            if (!visitedFiles.Add(file) || paths.IsExcluded(file)) continue;
            yield return file;
        }

        foreach (var childDirectory in Directory.EnumerateDirectories(directory, "*", options))
        {
            foreach (var file in EnumerateFiles(childDirectory, visitedFiles, visitedDirectories))
            {
                yield return file;
            }
        }
    }

    public IReadOnlyList<FileMatch> Search(string query, string? extension)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        var results = new List<FileMatch>();
        foreach (var file in EnumerateFiles())
        {
            if (!string.IsNullOrWhiteSpace(extension) && !file.EndsWith(extension, StringComparison.OrdinalIgnoreCase)) continue;
            if (new FileInfo(file).Length > context.Options.MaximumReadBytes) continue;

            var lineNumber = 0;
            foreach (var line in File.ReadLines(file))
            {
                lineNumber++;
                if (!line.Contains(query, StringComparison.OrdinalIgnoreCase)) continue;
                results.Add(new FileMatch(paths.ToRelativePath(file), lineNumber, line.Trim()));
                if (results.Count >= context.Options.MaximumSearchResults) return results;
            }
        }
        return results;
    }

    public string ReadText(string relativePath)
    {
        var fullPath = paths.ResolveReadablePath(relativePath);
        var info = new FileInfo(fullPath);
        if (!info.Exists) throw new FileNotFoundException("Repository file was not found.", relativePath);
        if (info.Length > context.Options.MaximumReadBytes) throw new InvalidOperationException("File exceeds the configured MCP read limit.");
        return File.ReadAllText(fullPath);
    }

    public IReadOnlyList<ProjectDescriptor> GetProjects()
    {
        return EnumerateFiles()
            .Where(x => x.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) || x.EndsWith("package.json", StringComparison.OrdinalIgnoreCase))
            .Select(file => DescribeProject(file))
            .OrderBy(x => x.Path, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private ProjectDescriptor DescribeProject(string file)
    {
        if (file.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var document = XDocument.Load(file);
                var framework = document.Descendants().FirstOrDefault(x => x.Name.LocalName is "TargetFramework" or "TargetFrameworkVersion")?.Value;
                return new ProjectDescriptor(paths.ToRelativePath(file), "dotnet", framework);
            }
            catch
            {
                return new ProjectDescriptor(paths.ToRelativePath(file), "dotnet", null);
            }
        }
        return new ProjectDescriptor(paths.ToRelativePath(file), "npm", null);
    }
}
