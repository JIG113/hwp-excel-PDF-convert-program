using System;
using System.Collections.Generic;

namespace Converter.Core.Conversion;

public enum ConversionStatus
{
    Pending,
    Running,
    Succeeded,
    Failed,
    Canceled
}

public sealed record ConversionRequest(
    string InputPath,
    string OutputPath,
    IReadOnlyDictionary<string, string>? Options = null,
    string? Password = null
);

public sealed record ConversionResult(
    string InputPath,
    string OutputPath,
    ConversionStatus Status,
    ConversionErrorCode ErrorCode = ConversionErrorCode.None,
    string? ErrorMessage = null,
    TimeSpan? Duration = null,
    string? EngineStdErr = null,
    int? EngineExitCode = null
);
