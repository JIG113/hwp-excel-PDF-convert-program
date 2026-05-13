using System.IO;
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
        var preflightError = ConversionPreflight.ValidatePaths(request);
        if (preflightError is not null)
        {
            return Task.FromResult(preflightError);
        }

        // NOTE: 다음 단계에서 실제 PDF 렌더러(PDFium/PDFSharp 등)로 대체 예정.
        File.Copy(request.InputPath, request.OutputPath, overwrite: true);
        return Task.FromResult(new ConversionResult(
            request.InputPath,
            request.OutputPath,
            ConversionStatus.Succeeded));
    }
}
