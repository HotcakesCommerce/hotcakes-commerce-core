using System.IO.Enumeration;

namespace Upendo.LocalDevelopmentMcp.Infrastructure;

public sealed class SafePathPolicy(RepositoryContext context)
{
    public string ResolveReadablePath(string repositoryRelativePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(repositoryRelativePath);
        var fullPath = Path.GetFullPath(repositoryRelativePath, context.Root);
        EnsureInsideRepository(fullPath);

        if (!IsAllowed(fullPath) || IsExcluded(fullPath))
        {
            throw new UnauthorizedAccessException($"Path is outside the configured MCP read scope: {repositoryRelativePath}");
        }

        return fullPath;
    }

    public string ResolveWorkingDirectory(string repositoryRelativePath)
    {
        var fullPath = ResolveReadablePath(repositoryRelativePath);
        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException(fullPath);
        }
        return fullPath;
    }

    public string ToRelativePath(string fullPath) => Path.GetRelativePath(context.Root, fullPath).Replace('\\', '/');

    public bool IsExcluded(string fullPath)
    {
        var relative = ToRelativePath(fullPath);
        return context.Options.ExcludedGlobs.Any(pattern => FileSystemName.MatchesSimpleExpression(pattern, relative, ignoreCase: true));
    }

    private bool IsAllowed(string fullPath)
    {
        if (context.Options.AllowedFiles.Any(file =>
                string.Equals(Path.GetFullPath(file, context.Root), fullPath, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return context.Options.AllowedRoots.Any(root =>
        {
            var allowed = Path.GetFullPath(root, context.Root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var candidate = fullPath.TrimEnd(Path.DirectorySeparatorChar) + (Directory.Exists(fullPath) ? Path.DirectorySeparatorChar : string.Empty);
            return candidate.StartsWith(allowed, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(fullPath, allowed.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase);
        });
    }

    private void EnsureInsideRepository(string fullPath)
    {
        var rootPrefix = context.Root.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        if (!fullPath.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(fullPath, context.Root, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Path traversal outside the repository is not allowed.");
        }
    }
}
