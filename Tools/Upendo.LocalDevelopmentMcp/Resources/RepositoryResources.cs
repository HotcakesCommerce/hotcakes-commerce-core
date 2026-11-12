using System.ComponentModel;
using System.Text;
using ModelContextProtocol.Server;
using Upendo.LocalDevelopmentMcp.Infrastructure;

namespace Upendo.LocalDevelopmentMcp.Resources;

[McpServerResourceType]
public sealed class RepositoryResources(RepositoryContext context, RepositoryScanner scanner)
{
    [McpServerResource(UriTemplate = "upendo://repository/metadata", Name = "Repository metadata")]
    [Description("Returns the repository root and discovered .NET and npm projects.")]
    public string Metadata()
    {
        var projects = scanner.GetProjects();
        var lines = projects.Select(x => $"- {x.Type}: {x.Path}{(x.TargetFramework is null ? string.Empty : $" ({x.TargetFramework})")}");
        return $"Repository root: {context.Root}{Environment.NewLine}{string.Join(Environment.NewLine, lines)}";
    }

    [McpServerResource(UriTemplate = "upendo://repository/development-standards", Name = "Development standards")]
    [Description("Combines the configured repository instruction and standards files.")]
    public string DevelopmentStandards()
    {
        var output = new StringBuilder();
        foreach (var file in context.Options.StandardsFiles)
        {
            try
            {
                output.AppendLine($"# {file}").AppendLine(scanner.ReadText(file)).AppendLine();
            }
            catch (FileNotFoundException)
            {
                output.AppendLine($"# {file}").AppendLine("[not present]").AppendLine();
            }
            catch (UnauthorizedAccessException)
            {
                output.AppendLine($"# {file}").AppendLine("[outside configured read scope]").AppendLine();
            }
        }
        return output.ToString();
    }
}
