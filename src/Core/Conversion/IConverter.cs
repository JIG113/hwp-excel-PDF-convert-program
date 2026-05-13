using System.Threading;
using System.Threading.Tasks;

namespace Converter.Core.Conversion;

public interface IConverter
{
    bool CanHandle(string extension);
    Task<ConversionResult> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken);
}
