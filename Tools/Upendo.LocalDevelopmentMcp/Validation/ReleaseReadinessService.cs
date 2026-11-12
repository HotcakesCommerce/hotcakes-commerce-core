using Upendo.LocalDevelopmentMcp.Models;

namespace Upendo.LocalDevelopmentMcp.Validation;

public sealed class ReleaseReadinessService(SecretScanner secrets, ManifestValidator manifests, SqlProviderValidator sql)
{
    public ReleaseReadinessReport Evaluate(string? manifestPath, string? installSqlPath, string? uninstallSqlPath)
    {
        var checks = new List<ValidationReport> { secrets.Scan() };
        if (!string.IsNullOrWhiteSpace(manifestPath)) checks.Add(manifests.Validate(manifestPath));
        if (!string.IsNullOrWhiteSpace(installSqlPath) && !string.IsNullOrWhiteSpace(uninstallSqlPath))
            checks.Add(sql.Compare(installSqlPath, uninstallSqlPath));
        return new ReleaseReadinessReport(checks.All(x => x.Success), checks);
    }
}
