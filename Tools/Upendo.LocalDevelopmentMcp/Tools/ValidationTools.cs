using System.ComponentModel;
using ModelContextProtocol.Server;
using Upendo.LocalDevelopmentMcp.Models;
using Upendo.LocalDevelopmentMcp.Validation;

namespace Upendo.LocalDevelopmentMcp.Tools;

[McpServerToolType]
public sealed class ValidationTools(
    ManifestValidator manifests,
    SqlProviderValidator sql,
    SecretScanner secrets,
    BuildCatalogValidator builds,
    ReleaseReadinessService release)
{
    [McpServerTool(Name = "validate_manifest", ReadOnly = true)]
    [Description("Performs deterministic structural checks against a module manifest.")]
    public ValidationReport ValidateManifest(string path) => manifests.Validate(path);

    [McpServerTool(Name = "compare_sql_install_uninstall", ReadOnly = true)]
    [Description("Checks provider tokens and compares visibly created SQL objects with uninstall DROP statements.")]
    public ValidationReport CompareSql(string installPath, string uninstallPath) => sql.Compare(installPath, uninstallPath);

    [McpServerTool(Name = "scan_repository_secrets", ReadOnly = true)]
    [Description("Scans approved repository files for unsafe file types and common embedded-secret patterns.")]
    public ValidationReport ScanSecrets() => secrets.Scan();

    [McpServerTool(Name = "validate_build_catalog", ReadOnly = true)]
    [Description("Checks the effective approved build catalog without executing builds, including names, kinds, paths, project references, npm scripts, executable portability, and unsafe arguments.")]
    public ValidationReport ValidateBuildCatalog() => builds.Validate();

    [McpServerTool(Name = "release_readiness", ReadOnly = true)]
    [Description("Combines secret, manifest, and SQL checks into a release-readiness report.")]
    public ReleaseReadinessReport ReleaseReadiness(string? manifestPath = null, string? installSqlPath = null, string? uninstallSqlPath = null) =>
        release.Evaluate(manifestPath, installSqlPath, uninstallSqlPath);
}
