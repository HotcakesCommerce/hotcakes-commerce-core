using System.Text.RegularExpressions;
using Upendo.LocalDevelopmentMcp.Infrastructure;
using Upendo.LocalDevelopmentMcp.Models;

namespace Upendo.LocalDevelopmentMcp.Validation;

public sealed partial class SqlProviderValidator(RepositoryScanner scanner)
{
    public ValidationReport Compare(string installPath, string uninstallPath)
    {
        var findings = new List<ValidationFinding>();
        var install = scanner.ReadText(installPath);
        var uninstall = scanner.ReadText(uninstallPath);

        RequireToken(install, installPath, "{databaseOwner}", findings);
        RequireToken(install, installPath, "{objectQualifier}", findings);
        RequireToken(uninstall, uninstallPath, "{databaseOwner}", findings);
        RequireToken(uninstall, uninstallPath, "{objectQualifier}", findings);

        var created = CreatedObjectRegex().Matches(install).Select(x => Normalize(x.Groups[2].Value)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var dropped = DroppedObjectRegex().Matches(uninstall).Select(x => Normalize(x.Groups[2].Value)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var name in created.Except(dropped, StringComparer.OrdinalIgnoreCase))
            findings.Add(new("error", "SQL_UNINSTALL_MISSING", uninstallPath, null, $"Created object is not visibly removed by uninstall: {name}"));

        if (Regex.IsMatch(install, @"\b(RowCount)\b", RegexOptions.IgnoreCase))
            findings.Add(new("warning", "SQL_ALIAS", installPath, null, "Avoid the unescaped RowCount alias. Prefer [RowsAffected] or another bracketed alias."));

        return new ValidationReport("sql-provider", findings.All(x => x.Severity != "error"), findings);
    }

    private static void RequireToken(string text, string path, string token, List<ValidationFinding> findings)
    {
        if (!text.Contains(token, StringComparison.Ordinal))
            findings.Add(new("error", "SQL_TOKEN", path, null, $"Required provider token is missing: {token}"));
    }

    private static string Normalize(string value) => value.Replace("[", string.Empty).Replace("]", string.Empty).Trim();

    [GeneratedRegex(@"\bCREATE\s+(TABLE|PROCEDURE|FUNCTION|VIEW|TRIGGER)\s+([^\s(;]+)", RegexOptions.IgnoreCase)]
    private static partial Regex CreatedObjectRegex();

    [GeneratedRegex(@"\bDROP\s+(TABLE|PROCEDURE|FUNCTION|VIEW|TRIGGER)\s+([^\s(;]+)", RegexOptions.IgnoreCase)]
    private static partial Regex DroppedObjectRegex();
}
