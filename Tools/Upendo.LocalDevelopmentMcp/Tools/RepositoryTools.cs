using System.ComponentModel;
using ModelContextProtocol.Server;
using Upendo.LocalDevelopmentMcp.Infrastructure;
using Upendo.LocalDevelopmentMcp.Models;

namespace Upendo.LocalDevelopmentMcp.Tools;

[McpServerToolType]
public sealed class RepositoryTools(RepositoryScanner scanner, ApprovedProcessRunner processes)
{
    [McpServerTool(Name = "repository_search", ReadOnly = true)]
    [Description("Safely searches text within configured repository roots. It cannot read excluded or out-of-repository paths.")]
    public IReadOnlyList<FileMatch> Search(
        [Description("Case-insensitive literal text to locate.")] string query,
        [Description("Optional extension filter such as .cs or .sql.")] string? extension = null) => scanner.Search(query, extension);

    [McpServerTool(Name = "repository_read_file", ReadOnly = true)]
    [Description("Reads one permitted repository text file subject to configured path and size limits.")]
    public string ReadFile([Description("Repository-relative path.")] string path) => scanner.ReadText(path);

    [McpServerTool(Name = "repository_projects", ReadOnly = true)]
    [Description("Lists discovered .NET and npm projects under approved roots.")]
    public IReadOnlyList<ProjectDescriptor> Projects() => scanner.GetProjects();

    [McpServerTool(Name = "git_status", ReadOnly = true)]
    [Description("Returns porcelain Git status without modifying the working tree.")]
    public Task<ProcessResult> GitStatus(CancellationToken cancellationToken) =>
        processes.RunGitAsync(["status", "--short", "--branch"], cancellationToken);

    [McpServerTool(Name = "git_diff", ReadOnly = true)]
    [Description("Returns the current unstaged or staged Git diff without modifying the working tree.")]
    public Task<ProcessResult> GitDiff(bool staged, CancellationToken cancellationToken) =>
        processes.RunGitAsync(staged ? ["diff", "--cached", "--no-ext-diff"] : ["diff", "--no-ext-diff"], cancellationToken);

    [McpServerTool(Name = "create_patch", ReadOnly = true)]
    [Description("Creates patch text from current working-tree changes. This does not commit, stage, push, or write files.")]
    public Task<ProcessResult> CreatePatch(CancellationToken cancellationToken) =>
        processes.RunGitAsync(["diff", "--binary", "--no-ext-diff", "HEAD"], cancellationToken);
}
