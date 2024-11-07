using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Raspite.Serializer;

internal ref struct SpanReader(ReadOnlySpan<byte> span, bool littleEndian)
{
	public int Position { get; private set; }

	private ReadOnlySpan<byte> span = span;

	public bool TryRead(int length, out ReadOnlySpan<byte> value)
	{
		value = default;

		if (Position + length > span.Length)
		{
			return false;
		}

		value = span.Slice(Position, length);
		Position += length;

		return true;
	}

	public bool TryRead(out byte value)
	{
		value = default;

		if (Position + sizeof(byte) > span.Length)
		{
			return false;
		}

		value = span[Position++];
		return true;
	}

	public bool TryRead(out short value)
	{
		value = default;

		if (Position + sizeof(short) > span.Length)
		{
			return false;
		}

		var slice = span[Position..(Position += sizeof(short))];

		value = littleEndian
			? BinaryPrimitives.ReadInt16LittleEndian(slice)
			: BinaryPrimitives.ReadInt16BigEndian(slice);

		return true;
	}

	public bool TryRead(out ushort value)
	{
		value = default;

		if (Position + sizeof(ushort) > span.Length)
		{
			return false;
		}

		var slice = span[Position..(Position += sizeof(ushort))];

		value = littleEndian
			? BinaryPrimitives.ReadUInt16LittleEndian(slice)
			: BinaryPrimitives.ReadUInt16BigEndian(slice);

		return true;
	}

	public bool TryRead(out int value)
	{
		value = default;

		if (Position + sizeof(int) > span.Length)
		{
			return false;
		}

		var slice = span[Position..(Position += sizeof(int))];

		value = littleEndian
			? BinaryPrimitives.ReadInt32LittleEndian(slice)
			: BinaryPrimitives.ReadInt32BigEndian(slice);

		return true;
	}

	public bool TryRead(out long value)
	{
		value = default;

		if (Position + sizeof(long) > span.Length)
		{
			return false;
		}

		var slice = span[Position..(Position += sizeof(long))];

		value = littleEndian
			? BinaryPrimitives.ReadInt64LittleEndian(slice)
			: BinaryPrimitives.ReadInt64BigEndian(slice);

		return true;
	}

	public bool TryRead(out float value)
	{
		value = default;

		if (Position + sizeof(float) > span.Length)
		{
			return false;
		}

		var slice = span[Position..(Position += sizeof(float))];

		value = littleEndian
			? BinaryPrimitives.ReadSingleLittleEndian(slice)
			: BinaryPrimitives.ReadSingleBigEndian(slice);

		return true;
	}

	public bool TryRead(out double value)
	{
		value = default;

		if (Position + sizeof(double) > span.Length)
		{
			return false;
		}

		var slice = span[Position..(Position += sizeof(double))];

		value = littleEndian
			? BinaryPrimitives.ReadDoubleLittleEndian(slice)
			: BinaryPrimitives.ReadDoubleBigEndian(slice);

		return true;
	}

	public bool TryRead([NotNullWhen(true)] out string? value)
	{
		value = string.Empty;

		if (!TryRead(out ushort length) || !TryRead(length, out var buffer))
		{
			return false;
		}

		value = Encoding.UTF8.GetString(buffer);
		return true;
	}
}