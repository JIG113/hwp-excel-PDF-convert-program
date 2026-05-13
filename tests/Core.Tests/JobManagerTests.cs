using Converter.Core.Conversion;
using Converter.Core.Jobs;
using FluentAssertions;

namespace Core.Tests;

public class JobManagerTests
{
    [Fact]
    public async Task RunBatch_ShouldCaptureFailures()
    {
        var router = new ConverterRouter(new IConverter[] { new FailingConverter() });
        var manager = new JobManager(router, 1);
        var requests = new[] { new ConversionRequest("a.txt", "b.pdf") };
        var results = await manager.RunBatchAsync(requests);
        results.Single().Status.Should().Be(ConversionStatus.Failed);
    }

    [Fact]
    public async Task RunBatch_ShouldSupportCancellation()
    {
        var router = new ConverterRouter(new IConverter[] { new SlowConverter() });
        var manager = new JobManager(router, 1);
        using var cts = new CancellationTokenSource(50);
        var requests = new[] { new ConversionRequest("a.txt", "b.pdf") };
        var results = await manager.RunBatchAsync(requests, cts.Token);
        results.Single().Status.Should().Be(ConversionStatus.Canceled);
    }

    private sealed class FailingConverter : IConverter
    {
        public bool CanHandle(string extension) => true;
        public Task<ConversionResult> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken)
            => throw new InvalidOperationException("fail");
    }

    private sealed class SlowConverter : IConverter
    {
        public bool CanHandle(string extension) => true;
        public async Task<ConversionResult> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken)
        {
            await Task.Delay(5000, cancellationToken);
            return new ConversionResult(request.InputPath, request.OutputPath, ConversionStatus.Succeeded);
        }
    }
}
