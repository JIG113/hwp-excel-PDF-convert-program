using Converter.Core.Conversion;
using FluentAssertions;

namespace Core.Tests;

public class ConversionPreflightTests
{
    [Fact]
    public void ValidatePaths_ShouldFail_WhenInputMissing()
    {
        var req = new ConversionRequest("missing.txt", Path.Combine(Path.GetTempPath(), "a.pdf"));
        var result = ConversionPreflight.ValidatePaths(req);
        result.Should().NotBeNull();
        result!.ErrorCode.Should().Be(ConversionErrorCode.InputFileMissing);
    }

    [Fact]
    public void ValidatePaths_ShouldFail_WhenOutputDirectoryMissing()
    {
        var input = Path.GetTempFileName();
        var req = new ConversionRequest(input, Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "a.pdf"));
        var result = ConversionPreflight.ValidatePaths(req);
        result.Should().NotBeNull();
        result!.ErrorCode.Should().Be(ConversionErrorCode.OutputDirectoryMissing);
        File.Delete(input);
    }

    [Fact]
    public void ValidatePaths_ShouldPass_ForValidPaths()
    {
        var input = Path.GetTempFileName();
        var output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".pdf");
        var result = ConversionPreflight.ValidatePaths(new ConversionRequest(input, output));
        result.Should().BeNull();
        File.Delete(input);
    }
}
