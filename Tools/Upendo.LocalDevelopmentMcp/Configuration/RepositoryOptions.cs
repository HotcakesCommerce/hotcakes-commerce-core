using System.ComponentModel.DataAnnotations;

namespace Upendo.LocalDevelopmentMcp.Configuration;

public sealed class RepositoryOptions
{
    public const string SectionName = "Repository";

    [Required]
    public string Root { get; init; } = ".";

    public string[] StandardsFiles { get; init; } = [];
    public string[] AllowedRoots { get; init; } = ["."];
    public string[] AllowedFiles { get; init; } = [];
    public string[] ExcludedGlobs { get; init; } = [];

    [Range(1, 16 * 1024 * 1024)]
    public int MaximumReadBytes { get; init; } = 1024 * 1024;

    [Range(1, 2000)]
    public int MaximumSearchResults { get; init; } = 200;

    [Range(1, 3600)]
    public int GitProcessTimeoutSeconds { get; init; } = 30;

    [Range(1, 3600)]
    public int BuildProcessTimeoutSeconds { get; init; } = 900;

    public BuildDefinition[] Builds { get; init; } = [];
}

public sealed class BuildDefinition
{
    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string Kind { get; init; } = string.Empty;

    [Required]
    public string WorkingDirectory { get; init; } = ".";

    [Required]
    public string Executable { get; init; } = string.Empty;

    public string[] Arguments { get; init; } = [];
}
