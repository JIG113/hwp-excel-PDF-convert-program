using Converter.Core.Conversion;
using FluentAssertions;

namespace Core.Tests;

public class ConverterRouterTests
{
    [Fact]
    public void Resolve_ShouldReturnMatchingConverter()
    {
        var expected = new FakeConverter(".txt");
        var router = new ConverterRouter(new IConverter[] { expected });

        var resolved = router.Resolve("a.txt");
        resolved.Should().Be(expected);
    }

    [Fact]
    public void Resolve_ShouldThrow_ForUnsupportedExtension()
    {
        var router = new ConverterRouter(new IConverter[] { new FakeConverter(".txt") });
        var act = () => router.Resolve("a.unknown");
        act.Should().Throw<NotSupportedException>();
    }

    private sealed class FakeConverter(string ext) : IConverter
    {
        public bool CanHandle(string extension) => extension == ext;
        public Task<ConversionResult> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken)
            => Task.FromResult(new ConversionResult(request.InputPath, request.OutputPath, ConversionStatus.Succeeded));
    }
}
