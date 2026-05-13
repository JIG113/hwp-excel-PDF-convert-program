using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Converter.Core.Conversion;

namespace Converter.Core.Jobs;

public sealed class JobManager
{
    private readonly ConverterRouter _router;
    private readonly SemaphoreSlim _semaphore;

    public JobManager(ConverterRouter router, int maxConcurrency = 2)
    {
        _router = router;
        _semaphore = new SemaphoreSlim(Math.Max(1, maxConcurrency));
    }

    public async Task<IReadOnlyCollection<ConversionResult>> RunBatchAsync(
        IEnumerable<ConversionRequest> requests,
        CancellationToken cancellationToken = default)
    {
        var results = new ConcurrentBag<ConversionResult>();
        var tasks = new List<Task>();

        foreach (var request in requests)
        {
            tasks.Add(RunOneAsync(request, results, cancellationToken));
        }

        await Task.WhenAll(tasks);
        return results.ToArray();
    }

    private async Task RunOneAsync(
        ConversionRequest request,
        ConcurrentBag<ConversionResult> results,
        CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        var startedAt = DateTime.UtcNow;

        try
        {
            var converter = _router.Resolve(request.InputPath);
            var result = await converter.ConvertAsync(request, cancellationToken);
            results.Add(result with { Duration = DateTime.UtcNow - startedAt });
        }
        catch (OperationCanceledException)
        {
            results.Add(new ConversionResult(
                request.InputPath,
                request.OutputPath,
                ConversionStatus.Canceled,
                ErrorMessage: "Operation was canceled.",
                Duration: DateTime.UtcNow - startedAt));
        }
        catch (Exception ex)
        {
            results.Add(new ConversionResult(
                request.InputPath,
                request.OutputPath,
                ConversionStatus.Failed,
                ConversionErrorCode.ConversionFailed,
                ex.Message,
                Duration: DateTime.UtcNow - startedAt));
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
