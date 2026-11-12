namespace Upendo.LocalDevelopmentMcp.Models;

public sealed record FileMatch(string Path, int Line, string Text);
public sealed record ProcessResult(bool Success, int ExitCode, string StandardOutput, string StandardError, long DurationMilliseconds);
public sealed record ValidationFinding(string Severity, string Code, string Path, int? Line, string Message);
public sealed record ValidationReport(string Name, bool Success, IReadOnlyList<ValidationFinding> Findings);
public sealed record ProjectDescriptor(string Path, string Type, string? TargetFramework);
public sealed record RepositoryMetadata(string Root, string? Branch, string? Commit, IReadOnlyList<ProjectDescriptor> Projects);
public sealed record ReleaseReadinessReport(bool Ready, IReadOnlyList<ValidationReport> Checks);
