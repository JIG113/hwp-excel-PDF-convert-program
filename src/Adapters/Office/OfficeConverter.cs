using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Converter.Core.Conversion;
using Microsoft.Extensions.Logging;

namespace Converter.Adapters.Office;

public sealed class OfficeConverter : IConverter
{
    private readonly ICommandRunner _commandRunner;
    private readonly ILogger<OfficeConverter> _logger;

    public OfficeConverter(ICommandRunner commandRunner, ILogger<OfficeConverter> logger)
    {
        _commandRunner = commandRunner;
        _logger = logger;
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

        var soffice = ResolveLibreOfficeBinary(request);
        if (string.IsNullOrWhiteSpace(soffice))
        {
            return Failed(request, ConversionErrorCode.EngineNotFound);
        }

        var timeoutSeconds = ResolveTimeout(request, 180);
        var outputDir = Path.GetDirectoryName(request.OutputPath)!;
        var expectedOutput = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(request.InputPath) + ".pdf");
        var tempDir = CreateTempWorkingDirectory();

        try
        {
            var args = BuildArgs(request.InputPath, outputDir, tempDir);
            _logger.LogInformation("Office conversion start: {Input} -> {Output}. command={Command} args={Args}", request.InputPath, request.OutputPath, soffice, args);

            var cmdResult = await _commandRunner.RunAsync(soffice, args, timeoutSeconds, cancellationToken);
            _logger.LogInformation("Office conversion process done: exitCode={ExitCode}", cmdResult.ExitCode);
            _logger.LogDebug("Office conversion stdout: {StdOut}", cmdResult.StdOut);
            _logger.LogDebug("Office conversion stderr: {StdErr}", cmdResult.StdErr);

            if (cmdResult.ExitCode == -1)
            {
                return new ConversionResult(request.InputPath, request.OutputPath, ConversionStatus.Failed,
                    ConversionErrorCode.Timeout, ConversionErrorMessages.Resolve(ConversionErrorCode.Timeout), EngineStdErr: cmdResult.StdErr, EngineExitCode: cmdResult.ExitCode);
            }

            if (cmdResult.ExitCode != 0)
            {
                return new ConversionResult(request.InputPath, request.OutputPath, ConversionStatus.Failed,
                    ConversionErrorCode.ProcessCrash, ConversionErrorMessages.Resolve(ConversionErrorCode.ProcessCrash), EngineStdErr: cmdResult.StdErr, EngineExitCode: cmdResult.ExitCode);
            }

            if (!File.Exists(expectedOutput))
            {
                return Failed(request, ConversionErrorCode.OutputMissing, cmdResult.StdErr, cmdResult.ExitCode);
            }

            if (!string.Equals(expectedOutput, request.OutputPath, StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(expectedOutput, request.OutputPath, overwrite: true);
            }

            return new ConversionResult(request.InputPath, request.OutputPath, ConversionStatus.Succeeded, EngineExitCode: cmdResult.ExitCode);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Failed(request, ConversionErrorCode.PermissionDenied, ex.Message);
        }
        catch (Exception ex)
        {
            return Failed(request, ConversionErrorCode.ConversionFailed, ex.Message);
        }
        finally
        {
            TryCleanup(tempDir);
        }
    }

    private static string BuildArgs(string inputPath, string outputDir, string tempDir)
    {
        var builder = new StringBuilder();
        builder.Append("--headless ");
        builder.Append($"-env:UserInstallation=file:///{tempDir.Replace('\\', '/')} ");
        builder.Append("--convert-to pdf ");
        builder.Append($"--outdir \"{outputDir}\" \"{inputPath}\"");
        return builder.ToString();
    }

    private static string? ResolveLibreOfficeBinary(ConversionRequest request)
    {
        if (request.Options is not null && request.Options.TryGetValue(ConversionOptions.LibreOfficePath, out var configuredPath) && File.Exists(configuredPath))
        {
            return configuredPath;
        }

        var commonWindows = new[]
        {
            @"C:\Program Files\LibreOffice\program\soffice.exe",
            @"C:\Program Files (x86)\LibreOffice\program\soffice.exe",
        };

        var installed = commonWindows.FirstOrDefault(File.Exists);
        if (!string.IsNullOrWhiteSpace(installed))
        {
            return installed;
        }

        return "soffice";
    }

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

    private static string CreateTempWorkingDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "doc-to-pdf", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void TryCleanup(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch
        {
            // best effort
        }
    }

    private static ConversionResult Failed(ConversionRequest request, ConversionErrorCode code, string? stderr = null, int? exitCode = null)
        => new(request.InputPath, request.OutputPath, ConversionStatus.Failed, code, ConversionErrorMessages.Resolve(code), EngineStdErr: stderr, EngineExitCode: exitCode);
}
