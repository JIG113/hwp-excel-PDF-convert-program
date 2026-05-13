using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Converter.Core.Conversion;

public sealed class ProcessCommandRunner : ICommandRunner
{
    public async Task<CommandResult> RunAsync(string fileName, string arguments, int timeoutSeconds, CancellationToken cancellationToken)
    {
        using var process = new Process();
        process.StartInfo.FileName = fileName;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();

        var stdOutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stdErrTask = process.StandardError.ReadToEndAsync(cancellationToken);

        var waitTask = process.WaitForExitAsync(cancellationToken);
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeoutSeconds), cancellationToken);

        var completed = await Task.WhenAny(waitTask, timeoutTask);
        if (completed == timeoutTask)
        {
            try { process.Kill(true); } catch { }
            return new CommandResult(-1, string.Empty, $"Process timed out after {timeoutSeconds}s.");
        }

        await waitTask;
        return new CommandResult(process.ExitCode, await stdOutTask, await stdErrTask);
    }
}
