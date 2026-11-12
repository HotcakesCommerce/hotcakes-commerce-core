using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Upendo.LocalDevelopmentMcp.Configuration;
using Upendo.LocalDevelopmentMcp.Infrastructure;
using Upendo.LocalDevelopmentMcp.Validation;

var builder = Host.CreateApplicationBuilder(args);

// stdout is reserved for MCP JSON-RPC. All diagnostics must go to stderr.
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
builder.Configuration.AddJsonFile(".upendo-mcp.json", optional: true, reloadOnChange: false);
builder.Configuration.AddEnvironmentVariables("UPENDO_MCP_");

builder.Services
    .AddOptions<RepositoryOptions>()
    .Bind(builder.Configuration.GetSection(RepositoryOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<RepositoryContext>();
builder.Services.AddSingleton<SafePathPolicy>();
builder.Services.AddSingleton<ApprovedProcessRunner>();
builder.Services.AddSingleton<RepositoryScanner>();
builder.Services.AddSingleton<ManifestValidator>();
builder.Services.AddSingleton<SqlProviderValidator>();
builder.Services.AddSingleton<SecretScanner>();
builder.Services.AddSingleton<BuildCatalogValidator>();
builder.Services.AddSingleton<ReleaseReadinessService>();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithResourcesFromAssembly();

await builder.Build().RunAsync();
