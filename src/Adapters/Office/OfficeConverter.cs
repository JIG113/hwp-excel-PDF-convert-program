using System.Threading;
using System.Threading.Tasks;
using Converter.Core.Conversion;

namespace Converter.Adapters.Office;

public sealed class OfficeConverter : IConverter
{
    public bool CanHandle(string extension)
        => extension is ".doc" or ".docx" or ".xls" or ".xlsx" or ".ppt" or ".pptx";

    public Task<ConversionResult> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken)
    {
        // TODO: LibreOffice headless 연동
        return Task.FromResult(new ConversionResult(
            request.InputPath,
            request.OutputPath,
            ConversionStatus.Failed,
            ErrorCode: "NOT_IMPLEMENTED",
            ErrorMessage: "Office converter is scaffolded but not implemented yet."));
    }
}
