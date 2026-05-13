namespace Converter.Core.Conversion;

public enum ConversionErrorCode
{
    None = 0,
    EngineNotFound,
    UnsupportedFormat,
    InputFileMissing,
    OutputDirectoryMissing,
    Timeout,
    ProcessCrash,
    OutputMissing,
    PermissionDenied,
    ConversionFailed,
}
