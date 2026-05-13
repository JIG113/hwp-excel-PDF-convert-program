using System.Threading;
using System.Threading.Tasks;
using Converter.Core.Conversion;

namespace Converter.Adapters.Hwp;

public sealed class HwpConverter : IConverter
{
    public bool CanHandle(string extension) => extension is ".hwp" or ".hwpx";

    public Task<ConversionResult> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken)
    {
        // TODO: HWP -> ODT(or text) -> PDF 파이프라인 연동
        return Task.FromResult(new ConversionResult(
            request.InputPath,
            request.OutputPath,
            ConversionStatus.Failed,
            ErrorCode: "NOT_IMPLEMENTED",
            ErrorMessage: "HWP converter is scaffolded but not implemented yet."));
    }
}
