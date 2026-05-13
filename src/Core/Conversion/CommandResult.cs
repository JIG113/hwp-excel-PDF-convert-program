namespace Converter.Core.Conversion;

public sealed record CommandResult(int ExitCode, string StdOut, string StdErr);
