using System.IO.Pipelines;
using Raspite.Tags;

namespace Raspite;

public static class BinaryTagSerializer
{
    public static async Task SerializeAsync(Stream stream, Tag tag, BinaryTagSerializerOptions? options = null)
    {
        options ??= new BinaryTagSerializerOptions();

        var writer = PipeWriter.Create(stream);
        BinaryTagWriter.Write(writer, tag, options.LittleEndian, options.MaximumDepth);

        var result = await writer.FlushAsync();

        if (result.IsCanceled)
        {
            throw new BinaryTagSerializerException("Operation was cancelled.");
        }
    }
}

public sealed class BinaryTagSerializerOptions
{
    public bool LittleEndian { get; init; }

    public int MaximumDepth { get; init; } = 256;

    public int MinimumLength { get; init; } = 2048;
}