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
                ConversionErrorCode.InputFileMissing,
                ConversionErrorMessages.Resolve(ConversionErrorCode.InputFileMissing));
        }

        var outputDir = Path.GetDirectoryName(request.OutputPath);
        if (string.IsNullOrWhiteSpace(outputDir) || !Directory.Exists(outputDir))
        {
            return new ConversionResult(
                request.InputPath,
                request.OutputPath,
                ConversionStatus.Failed,
                ConversionErrorCode.OutputDirectoryMissing,
                ConversionErrorMessages.Resolve(ConversionErrorCode.OutputDirectoryMissing));
        }

        return null;
    }
}
