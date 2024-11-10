using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using Raspite.Serializer.Tags;

namespace Raspite.Serializer;

public static class BinaryTagSerializer
{
	public static async Task<T> DeserializeAsync<T>(
		Stream stream,
		BinaryTagSerializerOptions options = default,
		CancellationToken cancellationToken = default) where T : Tag
	{
		if (!stream.CanRead)
		{
			throw new BinaryTagSerializerException("Stream is not readable.");
		}

		options = options.Validate();

		var reader = PipeReader.Create(
			stream,
			new StreamPipeReaderOptions(bufferSize: options.MinimumLength));

		while (true)
		{
			var result = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
			var buffer = result.Buffer;

			// In the event that no tag is parsed successfully, mark consumed
			// as nothing and examined as the entire buffer.
			var consumed = buffer.Start;
			var examined = buffer.End;

			try
			{
				if (TryRead(options.LittleEndian, options.MaximumDepth, ref buffer, out var tag))
				{
					// A single tag was successfully parsed so mark the start of the
					// parsed buffer as consumed. TryRead trims the buffer to
					// point to the data after the tag was parsed.
					consumed = buffer.Start;

					// Examined is marked the same as consumed here, so the next call
					// to DeserializeAsync<T> will process the next tag if there's
					// one.
					examined = consumed;

					return (T) tag;
				}

				// There's no more data to be processed.
				if (result.IsCompleted)
				{
					if (buffer.Length > 0)
					{
						// The tag is incomplete and there's no more data to process.
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

		throw new BinaryTagSerializerException("Could not deserialize the stream.");

		static bool TryRead(
			bool littleEndian,
			uint maximumDepth,
			ref ReadOnlySequence<byte> buffer,
			[NotNullWhen(true)] out Tag? tag)
		{
			tag = null;

			var reader = new BinaryTagReader(
				buffer.IsSingleSegment ? buffer.FirstSpan : buffer.ToArray(),
				littleEndian,
				maximumDepth);

			if (!reader.TryRead(out tag))
			{
				return false;
			}

			buffer = buffer.Slice(reader.Position);
			return true;
		}
	}

	public static async Task SerializeAsync<T>(
		T tag,
		Stream stream,
		BinaryTagSerializerOptions options = default,
		CancellationToken cancellationToken = default) where T : Tag
	{
		if (!stream.CanWrite)
		{
			throw new BinaryTagSerializerException("Stream is not writable.");
		}

		options = options.Validate();

		var writer = PipeWriter.Create(stream);

		var buffer = writer.GetMemory(tag.CalculateLength(tag is ListTag));

		var written = Write(options.LittleEndian, options.MaximumDepth, tag, buffer.Span);

		await stream.WriteAsync(buffer[..written], cancellationToken).ConfigureAwait(false);

		return;

		static int Write(bool littleEndian, uint maximumDepth, Tag tag, Span<byte> buffer)
		{
			var writer = new BinaryTagWriter(buffer, littleEndian, maximumDepth);
			writer.Write(tag);

			return writer.Position;
		}
	}
}

public readonly struct BinaryTagSerializerOptions
{
	public bool LittleEndian { get; init; }

	public uint MaximumDepth { get; init; }

	public int MinimumLength { get; init; }

	public BinaryTagSerializerOptions Validate()
	{
		return new BinaryTagSerializerOptions
		{
			LittleEndian = LittleEndian,
			MaximumDepth = MaximumDepth is 0 ? 256 : MaximumDepth,
			MinimumLength = MinimumLength <= 0 ? short.MaxValue : MinimumLength
		};
	}
}