using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using Raspite.Tags;

namespace Raspite;

public static class BinaryTagSerializer
{
    public static async Task SerializeAsync(Stream stream, Tag tag, BinaryTagSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= new BinaryTagSerializerOptions();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MaximumDepth, nameof(options.MaximumDepth));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MinimumLength, nameof(options.MinimumLength));

        var writer = PipeWriter.Create(
            stream,
            new StreamPipeWriterOptions(minimumBufferSize: options.MinimumLength));

        var self = new BinaryTagWriter(writer, options.LittleEndian, options.MaximumDepth);

        self.Write(tag);

        var result = await writer.FlushAsync(cancellationToken);

        if (result.IsCanceled)
        {
            throw new OperationCanceledException();
        }
    }

    public static async Task<T> DeserializeAsync<T>(Stream stream, BinaryTagSerializerOptions? options = null, CancellationToken cancellationToken = default) where T : Tag
    {
        options ??= new BinaryTagSerializerOptions();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MaximumDepth, nameof(options.MaximumDepth));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MinimumLength, nameof(options.MinimumLength));

        var reader = PipeReader.Create(
            stream,
            new StreamPipeReaderOptions(minimumReadSize: options.MinimumLength));

        while (true)
        {
            var result = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            var buffer = result.Buffer;

            var consumed = buffer.Start;
            var examined = buffer.End;

            try
            {
                if (TryRead(options.LittleEndian, options.MaximumDepth, ref buffer, out var tag))
                {
                    consumed = buffer.Start;
                    examined = consumed;

                    return (T) tag;
                }

                if (result.IsCompleted)
                {
                    if (buffer.Length > 0)
                    {
                        throw new BinaryTagSerializerException("Incomplete stream.");
                    }

                    break;
                }
            }
            finally
            {
                reader.AdvanceTo(consumed, examined);
            }
        }

        throw new BinaryTagSerializerException("Failed to deserialize the stream.");

        static bool TryRead(bool littleEndian, int maximumDepth, ref ReadOnlySequence<byte> buffer, [NotNullWhen(true)] out Tag? tag)
        {
            tag = null;

            // Should somehow continue where it left of from.
            var reader = new BinaryTagReader(
                buffer.IsSingleSegment ? buffer.FirstSpan : buffer.ToArray(),
                littleEndian,
                maximumDepth);

            if (!reader.TryRead(out tag))
            {
                return false;
            }

            buffer = buffer.Slice(reader.Consumed);
            return true;
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