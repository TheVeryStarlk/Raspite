using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Raspite.Serializer.Tags;

namespace Raspite.Serializer;

internal ref struct BinaryTagReader(ReadOnlySpan<byte> span, bool littleEndian, int maximumDepth)
{
	public int Position => reader.Position;

	private SpanReader reader = new(span, littleEndian);
	private byte current;

	public bool TryRead([NotNullWhen(true)] out Tag? tag)
	{
		tag = null;

		var name = string.Empty;

		if (current is 0 && (!reader.TryRead(out current) || !reader.TryRead(out name)))
		{
			return false;
		}

		switch (current)
		{
			case 1 when reader.TryRead(out byte value):
				tag = ByteTag.Create(value);
				break;

			case 2 when reader.TryRead(out short value):
				tag = ShortTag.Create(value);
				break;

			case 3 when reader.TryRead(out int value):
				tag = IntegerTag.Create(value);
				break;

			case 4 when reader.TryRead(out long value):
				tag = LongTag.Create(value);
				break;

			case 5 when reader.TryRead(out float value):
				tag = FloatTag.Create(value);
				break;

			case 6 when reader.TryRead(out double value):
				tag = DoubleTag.Create(value);
				break;

			case 7 when reader.TryRead(out int length) && reader.TryRead(length, out var children):
				tag = ByteCollectionTag.Create(children.ToArray());
				break;

			case 8 when reader.TryRead(out string? value):
				tag = StringTag.Create(value);
				break;

			case 9 when TryReadListTag(out var result):
				tag = result;
				break;

			case 10 when TryReadCompoundTag(out var result):
				tag = result;
				break;

			case 11 when TryReadIntegerCollectionTag(out var result):
				tag = result;
				break;

			case 12 when TryReadLongCollectionTag(out var result):
				tag = result;
				break;

			default:
				return false;
		}

		tag.Name = name;
		return true;
	}

	private bool TryReadListTag([NotNullWhen(true)] out ListTag? tag)
	{
		tag = null;

		if (!reader.TryRead(out byte type) || !reader.TryRead(out int length))
		{
			return false;
		}

		if (type is 0)
		{
			tag = ListTag.Create([]);
			return true;
		}

		BinaryTagSerializerException.ThrowIfGreaterThan(
			length,
			maximumDepth,
			"Children length can not be bigger than the maximum depth.");

		var builder = ListTag.Create<Tag>();
		var count = 0;

		while (length > count)
		{
			current = type;

			if (!TryRead(out var child))
			{
				return false;
			}

			if (child.Identifier != type)
			{
				throw new BinaryTagSerializerException("List tags can not hold different tag types.");
			}

			builder.Add(child);
			count++;
		}

		current = 0;

		tag = builder.Build();
		return true;
	}

	private bool TryReadCompoundTag([NotNullWhen(true)] out CompoundTag? tag)
	{
		tag = null;

		var builder = CompoundTag.Create();
		var count = 0;

		while (true)
		{
			BinaryTagSerializerException.ThrowIfGreaterThan(
				count,
				maximumDepth,
				"Maximum depth reached.");

			count++;

			if (!reader.TryRead(out current))
			{
				return false;
			}

			if (current is 0)
			{
				break;
			}

			if (!reader.TryRead(out string? name) || !TryRead(out var child))
			{
				return false;
			}

			builder.Add(child with { Name = name });
		}

		tag = builder.Build();
		return true;
	}

	private bool TryReadIntegerCollectionTag([NotNullWhen(true)] out IntegerCollectionTag? tag)
	{
		tag = null;

		if (!reader.TryRead(out int length))
		{
			return false;
		}

		if (BitConverter.IsLittleEndian == littleEndian && reader.TryRead(length * sizeof(int), out var buffer))
		{
			tag = IntegerCollectionTag.Create(MemoryMarshal.Cast<byte, int>(buffer).ToArray());
			return true;
		}

		var children = new int[length];

		for (var index = 0; index < children.Length; index++)
		{
			if (!reader.TryRead(out int value))
			{
				return false;
			}

			children[index] = value;
		}

		tag = IntegerCollectionTag.Create(children);
		return true;
	}

	private bool TryReadLongCollectionTag([NotNullWhen(true)] out LongCollectionTag? tag)
	{
		tag = null;

		if (!reader.TryRead(out int length))
		{
			return false;
		}

		if (BitConverter.IsLittleEndian == littleEndian
		    && reader.TryRead(length * sizeof(long), out var buffer))
		{
			tag = LongCollectionTag.Create(MemoryMarshal.Cast<byte, long>(buffer).ToArray());
			return true;
		}

		var children = new long[length];

		for (var index = 0; index < children.Length; index++)
		{
			if (!reader.TryRead(out long value))
			{
				return false;
			}

			children[index] = value;
		}

		tag = LongCollectionTag.Create(children);
		return true;
	}
}