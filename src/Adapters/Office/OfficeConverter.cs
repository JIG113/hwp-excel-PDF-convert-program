using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Converter.Core.Conversion;

namespace Converter.Adapters.Office;

public sealed class OfficeConverter : IConverter
{
    private readonly ICommandRunner _commandRunner;

    public OfficeConverter(ICommandRunner commandRunner)
    {
        _commandRunner = commandRunner;
    }

    public bool CanHandle(string extension)
        => extension is ".doc" or ".docx" or ".xls" or ".xlsx" or ".ppt" or ".pptx";

    public async Task<ConversionResult> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken)
    {
        var preflightError = ConversionPreflight.ValidatePaths(request);
        if (preflightError is not null)
        {
            return preflightError;
        }

        var libreOfficeBin = ResolveLibreOfficeBinary(request);
        if (string.IsNullOrWhiteSpace(libreOfficeBin))
        {
            return new ConversionResult(request.InputPath, request.OutputPath, ConversionStatus.Failed,
                ConversionErrors.EngineUnavailable, "LibreOffice executable path is not configured.");
        }

        var timeoutSeconds = ResolveTimeout(request, defaultSeconds: 120);
        var outputDir = Path.GetDirectoryName(request.OutputPath)!;
        var args = $"--headless --convert-to pdf --outdir \"{outputDir}\" \"{request.InputPath}\"";

        var cmdResult = await _commandRunner.RunAsync(libreOfficeBin, args, timeoutSeconds, cancellationToken);
        if (cmdResult.ExitCode != 0)
        {
            return new ConversionResult(request.InputPath, request.OutputPath, ConversionStatus.Failed,
                ConversionErrors.EngineFailed, $"LibreOffice failed: {cmdResult.StdErr}");
        }

        return new ConversionResult(request.InputPath, request.OutputPath, ConversionStatus.Succeeded);
    }

    private static string? ResolveLibreOfficeBinary(ConversionRequest request)
        => request.Options is not null && request.Options.TryGetValue(ConversionOptions.LibreOfficePath, out var bin)
            ? bin
            : "soffice";

    private static int ResolveTimeout(ConversionRequest request, int defaultSeconds)
    {
        if (request.Options is not null
            && request.Options.TryGetValue(ConversionOptions.TimeoutSeconds, out var value)
            && int.TryParse(value, out var parsed)
            && parsed > 0)
        {
            return parsed;
        }

        return defaultSeconds;
    }
}
