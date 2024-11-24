using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using Raspite.Serializer.Tags;

namespace Raspite.Serializer;

/// <summary>
/// Provides functionality to serialize <see cref="Tag{T}"/> to NBT and to deserialize NBT into <see cref="Tag{T}"/>.
/// </summary>
public static class BinaryTagSerializer
{
	/// <summary>
	/// Reads the buffer as a <see cref="Tag{T}"/>.
	/// </summary>
	/// <param name="buffer">The buffer to read from.</param>
	/// <param name="options">Options to control the deserializer.</param>
	/// <typeparam name="T">The <see cref="Tag{T}"/> type to deserialize to.</typeparam>
	/// <returns>The deserialized <see cref="Tag{T}"/>.</returns>
	/// <exception cref="BinaryTagSerializerException">An exception has occured during deserializing.</exception>
	public static T Deserialize<T>(
		ReadOnlySpan<byte> buffer,
		BinaryTagSerializerOptions? options = null) where T : Tag
	{
		options ??= new BinaryTagSerializerOptions();

		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MaximumDepth, nameof(options.MaximumDepth));
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MinimumLength, nameof(options.MinimumLength));

		var reader = new BinaryTagReader(buffer, options.LittleEndian, options.MaximumDepth);

		if (reader.TryRead(out var tag))
		{
			return (T) tag;
		}

		throw new BinaryTagSerializerException("Could not deserialize tag.");
	}

	/// <summary>
	/// Writes the <see cref="Tag{T}"/> to the buffer.
	/// </summary>
	/// <param name="buffer">The buffer to write the <see cref="Tag{T}"/> to.</param>
	/// <param name="tag">The <see cref="Tag{T}"/> to write to the buffer.</param>
	/// <param name="options">Options to control the serializer.</param>
	/// <typeparam name="T">The <see cref="Tag{T}"/>'s type.</typeparam>
	/// <returns>The end position of the serialized <see cref="Tag{T}"/>.</returns>
	/// <exception cref="BinaryTagSerializerException">An exception has occured during serializing.</exception>
	public static int Serialize<T>(
		Span<byte> buffer,
		T tag,
		BinaryTagSerializerOptions? options = null) where T : Tag
	{
		options ??= new BinaryTagSerializerOptions();

		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MaximumDepth, nameof(options.MaximumDepth));
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MinimumLength, nameof(options.MinimumLength));

		if (tag.CalculateLength(tag is ListTag) > buffer.Length)
		{
			throw new BinaryTagSerializerException("Buffer is too small.");
		}

		var writer = new BinaryTagWriter(buffer, options.LittleEndian, options.MaximumDepth);

		writer.Write(tag);

		return writer.Position;
	}

	/// <summary>
	/// Reads the <see cref="Stream"/> as a <see cref="Tag{T}"/>.
	/// </summary>
	/// <param name="stream">The <see cref="Stream"/> to read from.</param>
	/// <param name="options">Options to control the deserializer.</param>
	/// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> that can be used to cancel the deserialize operation.</param>
	/// <typeparam name="T">The <see cref="Tag{T}"/> type to deserialize to.</typeparam>
	/// <returns>A task that represents the asynchronous deserialize operation.</returns>
	/// <exception cref="BinaryTagSerializerException">An exception has occured during deserializing.</exception>
	public static async Task<T> DeserializeAsync<T>(
		Stream stream,
		BinaryTagSerializerOptions? options = null,
		CancellationToken cancellationToken = default) where T : Tag
	{
		if (!stream.CanRead)
		{
			throw new BinaryTagSerializerException("Stream is not readable.");
		}

		options ??= new BinaryTagSerializerOptions();

		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MaximumDepth, nameof(options.MaximumDepth));
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MinimumLength, nameof(options.MinimumLength));

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
			int maximumDepth,
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

	/// <summary>
	/// Writes the <see cref="Tag{T}"/> to the <see cref="Stream"/>.
	/// </summary>
	/// <param name="stream">The <see cref="Stream"/> to write to.</param>
	/// <param name="tag">The <see cref="Tag{T}"/> to write to the <see cref="Stream"/>.</param>
	/// <param name="options">Options to control the serializer.</param>
	/// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> that can be used to cancel the serialize operation.</param>
	/// <typeparam name="T">The <see cref="Tag{T}"/>'s type.</typeparam>
	/// <exception cref="BinaryTagSerializerException">An exception has occured during serializing.</exception>
	public static async Task SerializeAsync<T>(
		Stream stream,
		T tag,
		BinaryTagSerializerOptions? options = null,
		CancellationToken cancellationToken = default) where T : Tag
	{
		if (!stream.CanWrite)
		{
			throw new BinaryTagSerializerException("Stream is not writable.");
		}

		options ??= new BinaryTagSerializerOptions();

		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MaximumDepth, nameof(options.MaximumDepth));
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.MinimumLength, nameof(options.MinimumLength));

		var writer = PipeWriter.Create(stream);

		var buffer = writer.GetMemory(tag.CalculateLength(tag is ListTag));
		var written = Write(options.LittleEndian, options.MaximumDepth, tag, buffer.Span);

		await stream.WriteAsync(buffer[..written], cancellationToken).ConfigureAwait(false);

		return;

		static int Write(bool littleEndian, int maximumDepth, Tag tag, Span<byte> buffer)
		{
			var writer = new BinaryTagWriter(buffer, littleEndian, maximumDepth);
			writer.Write(tag);

			return writer.Position;
		}
	}
}

/// <summary>
/// Provides options to be used with <see cref="BinaryTagReader"/>.
/// </summary>
public sealed class BinaryTagSerializerOptions
{
	/// <summary>
	/// Represents if the buffer is big endian or little endian.
	/// </summary>
	/// <remarks>
	/// Defaults to false.
	/// </remarks>
	public bool LittleEndian { get; init; }

	/// <summary>
	/// The maximum depth of the buffer.
	/// This ensures to not read or write more children than the set <see cref="MaximumDepth"/>.
	/// </summary>
	/// <remarks>
	/// Defaults to 256.
	/// </remarks>
	public int MaximumDepth { get; init; } = 256;

	/// <summary>
	/// The minimum length of which to read from the buffer.
	/// </summary>
	/// <remarks>
	/// Defaults to 2048.
	/// </remarks>
	public int MinimumLength { get; init; } = 2048;
}