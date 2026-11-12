using System.Text.RegularExpressions;
using Upendo.LocalDevelopmentMcp.Infrastructure;
using Upendo.LocalDevelopmentMcp.Models;

namespace Upendo.LocalDevelopmentMcp.Validation;

public sealed partial class SecretScanner(RepositoryScanner scanner, SafePathPolicy paths)
{
    public ValidationReport Scan()
    {
        var findings = new List<ValidationFinding>();
        foreach (var file in scanner.EnumerateFiles())
        {
            var relative = paths.ToRelativePath(file);
            var name = Path.GetFileName(file);
            if (name.EndsWith(".pfx", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".snk", StringComparison.OrdinalIgnoreCase) || name.Equals(".env", StringComparison.OrdinalIgnoreCase))
                findings.Add(new("error", "UNSAFE_FILE", relative, null, "Potentially sensitive file is present in the repository scope."));

            var info = new FileInfo(file);
            if (info.Length > 512_000 || IsLikelyBinary(file)) continue;
            var line = 0;
            foreach (var text in File.ReadLines(file))
            {
                line++;
                if (SecretRegex().IsMatch(text))
                    findings.Add(new("error", "POSSIBLE_SECRET", relative, line, "Possible embedded credential or private key marker."));
            }
        }
        return new ValidationReport("secrets", findings.All(x => x.Severity != "error"), findings);
    }

    private static bool IsLikelyBinary(string path)
    {
        string[] binaryExtensions = [".dll", ".exe", ".zip", ".png", ".jpg", ".jpeg", ".gif", ".ico", ".pdf"];
        return binaryExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
    }

    [GeneratedRegex(
        """(BEGIN (RSA|EC|OPENSSH )?PRIVATE KEY|password\s*[:=]\s*["'][^"']+|api[_-]?key\s*[:=]\s*["'][^"']+|client[_-]?secret\s*[:=])""",
        RegexOptions.IgnoreCase)]
    private static partial Regex SecretRegex();
}
