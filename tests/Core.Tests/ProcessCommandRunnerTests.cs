using Converter.Core.Conversion;
using FluentAssertions;

namespace Core.Tests;

public class ProcessCommandRunnerTests
{
    [Fact]
    public async Task RunAsync_ShouldReturnExitCode()
    {
        var runner = new ProcessCommandRunner();
        var result = await runner.RunAsync("bash", "-lc \"echo hi\"", 5, CancellationToken.None);
        result.ExitCode.Should().Be(0);
    }

    [Fact]
    public async Task RunAsync_ShouldTimeout()
    {
        var runner = new ProcessCommandRunner();
        var result = await runner.RunAsync("bash", "-lc \"sleep 2\"", 1, CancellationToken.None);
        result.ExitCode.Should().Be(-1);
    }

    [Fact]
    public async Task RunAsync_ShouldCaptureNonZeroExit()
    {
        var runner = new ProcessCommandRunner();
        var result = await runner.RunAsync("bash", "-lc \"exit 7\"", 5, CancellationToken.None);
        result.ExitCode.Should().Be(7);
    }
}
