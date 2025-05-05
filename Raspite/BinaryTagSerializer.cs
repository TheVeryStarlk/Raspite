using System.IO.Pipelines;
using Raspite.Tags;

namespace Raspite;

public static class BinaryTagSerializer
{
    public static async Task SerializeAsync(Stream stream, Tag tag, BinaryTagSerializerOptions? options = null)
    {
        options ??= new BinaryTagSerializerOptions();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MaximumDepth, nameof(options.MaximumDepth));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MinimumLength, nameof(options.MinimumLength));

        var writer = PipeWriter.Create(stream);
        var self = new BinaryTagWriter(writer, options.LittleEndian, options.MaximumDepth);

        self.Write(tag);

        var result = await writer.FlushAsync();

        if (result.IsCanceled)
        {
            throw new BinaryTagSerializerException("Operation was cancelled.");
        }
    }

    public static T Deserialize<T>(ReadOnlySpan<byte> span, BinaryTagSerializerOptions? options = null) where T : Tag
    {
        options ??= new BinaryTagSerializerOptions();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MaximumDepth, nameof(options.MaximumDepth));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MinimumLength, nameof(options.MinimumLength));

        var reader = new BinaryTagReader(span, options.LittleEndian, options.MaximumDepth);

        if (reader.TryRead(out var tag))
        {
            return (T) tag;
        }

        throw new BinaryTagSerializerException("Failed to deserialize tag.");
    }
}

public sealed class BinaryTagSerializerOptions
{
    public int MaximumDepth { get; init; } = 256;

    public int MinimumLength { get; init; } = 2048;

    public bool LittleEndian { get; init; }
}