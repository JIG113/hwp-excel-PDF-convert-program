using System.Threading;
using System.Threading.Tasks;
using Converter.Core.Conversion;

namespace Converter.Adapters.Hwp;

public sealed class HwpConverter : IConverter
{
    public bool CanHandle(string extension) => extension is ".hwp" or ".hwpx";

    public Task<ConversionResult> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken)
    {
        var preflightError = ConversionPreflight.ValidatePaths(request);
        if (preflightError is not null)
        {
            return Task.FromResult(preflightError);
        }

        return Task.FromResult(new ConversionResult(
            request.InputPath,
            request.OutputPath,
            ConversionStatus.Failed,
            ConversionErrorCode.ConversionFailed,
            "HWP converter strategy is documented but not implemented yet."));
    }
}
