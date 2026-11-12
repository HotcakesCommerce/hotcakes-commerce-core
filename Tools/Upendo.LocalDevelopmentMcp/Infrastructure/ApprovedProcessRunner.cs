using System.Diagnostics;
using System.Text;
using Upendo.LocalDevelopmentMcp.Models;

namespace Upendo.LocalDevelopmentMcp.Infrastructure;

public sealed class ApprovedProcessRunner(RepositoryContext context, SafePathPolicy paths)
{
    private const int MaximumCapturedCharacters = 2_000_000;

    public async Task<ProcessResult> RunBuildAsync(string buildName, CancellationToken cancellationToken)
    {
        var definition = context.Options.Builds.SingleOrDefault(x => string.Equals(x.Name, buildName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Unknown approved build '{buildName}'. Configure it in .upendo-mcp.json.");

        return await RunExactAsync(definition.Executable, definition.Arguments, definition.WorkingDirectory, context.Options.BuildProcessTimeoutSeconds, setGitEnvironment: false, cancellationToken);
    }

    public Task<ProcessResult> RunGitAsync(IReadOnlyList<string> arguments, CancellationToken cancellationToken)
    {
        string[] allowedPrefixes = ["status", "diff", "rev-parse", "ls-files"];
        if (arguments.Count == 0 || !allowedPrefixes.Contains(arguments[0], StringComparer.Ordinal))
        {
            throw new UnauthorizedAccessException("Only approved read-only Git operations are available.");
        }
        return RunExactAsync("git", arguments, ".", context.Options.GitProcessTimeoutSeconds, setGitEnvironment: true, cancellationToken);
    }

    private async Task<ProcessResult> RunExactAsync(
        string executable,
        IReadOnlyList<string> arguments,
        string workingDirectory,
        int timeoutSeconds,
        bool setGitEnvironment,
        CancellationToken cancellationToken)
    {
        // No shell is involved. The executable and every argument come from configuration or a fixed allowlist.
        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            WorkingDirectory = paths.ResolveWorkingDirectory(workingDirectory),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        if (setGitEnvironment)
        {
            startInfo.Environment["GIT_TERMINAL_PROMPT"] = "0";
            startInfo.Environment["GIT_ASKPASS"] = string.Empty;
            startInfo.Environment["GCM_INTERACTIVE"] = "Never";
        }
        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);
        using var process = new Process { StartInfo = startInfo };
        var stopwatch = Stopwatch.StartNew();
        if (!process.Start())
        {
            throw new InvalidOperationException($"Unable to start approved executable: {executable}");
        }

        // Start both readers before waiting so a chatty child cannot deadlock on either pipe.
        var stdoutTask = ReadLimitedAsync(process.StandardOutput);
        var stderrTask = ReadLimitedAsync(process.StandardError);
        try
        {
            await process.WaitForExitAsync(linkedCancellation.Token);
            var stdout = await stdoutTask;
            var stderr = await stderrTask;
            stopwatch.Stop();
            return new ProcessResult(process.ExitCode == 0, process.ExitCode, stdout, stderr, stopwatch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException) when (linkedCancellation.IsCancellationRequested)
        {
            KillProcessTree(process);
            await process.WaitForExitAsync(CancellationToken.None);
            await Task.WhenAll(stdoutTask, stderrTask);
            if (timeout.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException($"Approved process exceeded its {timeoutSeconds}-second timeout: {executable}");
            }
            throw;
        }
    }

    private static void KillProcessTree(Process process)
    {
        if (!process.HasExited)
        {
            process.Kill(entireProcessTree: true);
        }
    }

    private static async Task<string> ReadLimitedAsync(StreamReader reader)
    {
        var builder = new StringBuilder();
        var buffer = new char[8192];
        var truncated = false;
        while (true)
        {
            var count = await reader.ReadAsync(buffer.AsMemory(0, buffer.Length));
            if (count == 0) break;
            if (builder.Length < MaximumCapturedCharacters)
            {
                var retained = Math.Min(count, MaximumCapturedCharacters - builder.Length);
                builder.Append(buffer, 0, retained);
                truncated |= retained < count;
            }
            else
            {
                truncated = true;
            }
        }
        if (truncated) builder.AppendLine().Append("[output truncated]");
        return builder.ToString();
    }
}
