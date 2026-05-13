using System.IO;

namespace Converter.Core.Conversion;

public static class ConversionPreflight
{
    public static ConversionResult? ValidatePaths(ConversionRequest request)
    {
        if (!File.Exists(request.InputPath))
        {
            return new ConversionResult(
                request.InputPath,
                request.OutputPath,
                ConversionStatus.Failed,
                ConversionErrors.InputNotFound,
                $"Input file does not exist: {request.InputPath}");
        }

        var outputDir = Path.GetDirectoryName(request.OutputPath);
        if (string.IsNullOrWhiteSpace(outputDir) || !Directory.Exists(outputDir))
        {
            return new ConversionResult(
                request.InputPath,
                request.OutputPath,
                ConversionStatus.Failed,
                ConversionErrors.OutputDirectoryNotFound,
                $"Output directory does not exist: {outputDir}");
        }

        return null;
    }
}
