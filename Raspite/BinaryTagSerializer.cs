using System.IO.Pipelines;
using Raspite.Tags;

namespace Raspite;

public static class BinaryTagSerializer
{
    public static void Serialize(Stream stream, Tag tag, BinaryTagSerializerOptions? options = null)
    {
        options ??= new BinaryTagSerializerOptions();

        var writer = new BinaryTagWriter(PipeWriter.Create(stream), options.LittleEndian, options.MaximumDepth);
        writer.Write(tag);
    }
}

public sealed class BinaryTagSerializerOptions
{
    public bool LittleEndian { get; init; }

    public int MaximumDepth { get; init; } = 256;

    public int MinimumLength { get; init; } = 2048;
}