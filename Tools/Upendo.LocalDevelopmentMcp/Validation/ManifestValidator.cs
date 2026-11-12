using System.Xml.Linq;
using Upendo.LocalDevelopmentMcp.Infrastructure;
using Upendo.LocalDevelopmentMcp.Models;

namespace Upendo.LocalDevelopmentMcp.Validation;

public sealed class ManifestValidator(SafePathPolicy paths)
{
    public ValidationReport Validate(string relativePath)
    {
        var findings = new List<ValidationFinding>();
        var fullPath = paths.ResolveReadablePath(relativePath);
        try
        {
            var document = XDocument.Load(fullPath, LoadOptions.SetLineInfo);
            if (!document.Descendants().Any(x => x.Name.LocalName == "components"))
                findings.Add(new("error", "MANIFEST_COMPONENTS", relativePath, null, "Manifest does not contain a components element."));
            if (!document.Descendants().Any(x => x.Name.LocalName == "package"))
                findings.Add(new("error", "MANIFEST_PACKAGE", relativePath, null, "Manifest does not contain a package element."));
            if (!document.Descendants().Any(x => x.Name.LocalName == "component" && string.Equals((string?)x.Attribute("type"), "Script", StringComparison.OrdinalIgnoreCase)))
                findings.Add(new("warning", "MANIFEST_SCRIPT", relativePath, null, "No Script component was found. Confirm install and uninstall SQL are intentionally absent."));
        }
        catch (Exception exception) when (exception is IOException or System.Xml.XmlException)
        {
            findings.Add(new("error", "MANIFEST_XML", relativePath, null, exception.Message));
        }
        return new ValidationReport("manifest", findings.All(x => x.Severity != "error"), findings);
    }
}
