using Microsoft.Extensions.Options;
using Upendo.LocalDevelopmentMcp.Configuration;

namespace Upendo.LocalDevelopmentMcp.Infrastructure;

public sealed class RepositoryContext
{
    public RepositoryContext(IOptions<RepositoryOptions> options)
    {
        Options = options.Value;
        Root = Path.GetFullPath(Options.Root, Directory.GetCurrentDirectory());
        if (!Directory.Exists(Root))
        {
            throw new DirectoryNotFoundException($"Repository root does not exist: {Root}");
        }
    }

    public string Root { get; }
    public RepositoryOptions Options { get; }
}
