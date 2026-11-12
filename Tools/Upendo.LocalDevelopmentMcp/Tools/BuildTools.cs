using System.ComponentModel;
using ModelContextProtocol.Server;
using Upendo.LocalDevelopmentMcp.Infrastructure;
using Upendo.LocalDevelopmentMcp.Models;

namespace Upendo.LocalDevelopmentMcp.Tools;

[McpServerToolType]
public sealed class BuildTools(ApprovedProcessRunner processes)
{
    [McpServerTool(Name = "run_approved_build", Destructive = false)]
    [Description("Runs one explicitly configured MSBuild, dotnet, or npm build. Arbitrary commands and arguments are not accepted.")]
    public Task<ProcessResult> RunApprovedBuild(
        [Description("Exact build name from .upendo-mcp.json.")] string buildName,
        CancellationToken cancellationToken) => processes.RunBuildAsync(buildName, cancellationToken);
}
