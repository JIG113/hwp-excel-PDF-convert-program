using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Converter.Core.Conversion;

public sealed class ConverterRouter
{
    private readonly IReadOnlyList<IConverter> _converters;

    public ConverterRouter(IEnumerable<IConverter> converters)
    {
        _converters = converters.ToList();
    }

    public IConverter Resolve(string inputPath)
    {
        var extension = Path.GetExtension(inputPath).ToLowerInvariant();
        var converter = _converters.FirstOrDefault(c => c.CanHandle(extension));

        if (converter is null)
        {
            throw new NotSupportedException($"Unsupported file extension: {extension}");
        }

        return converter;
    }
}
