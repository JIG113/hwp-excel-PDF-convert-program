using System.Threading;
using System.Threading.Tasks;
using Converter.Core.Conversion;

namespace Converter.Adapters.TextImage;

public sealed class TextImageConverter : IConverter
{
    public bool CanHandle(string extension)
        => extension is ".txt" or ".csv" or ".jpg" or ".jpeg" or ".png";

    public Task<ConversionResult> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken)
    {
        // TODO: 텍스트/이미지 PDF 생성 엔진 연동
        return Task.FromResult(new ConversionResult(
            request.InputPath,
            request.OutputPath,
            ConversionStatus.Failed,
            ErrorCode: "NOT_IMPLEMENTED",
            ErrorMessage: "Text/Image converter is scaffolded but not implemented yet."));
    }
}
