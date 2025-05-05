using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Raspite.Tags;

namespace Raspite;

internal ref struct BinaryTagReader(ReadOnlySpan<byte> span, bool littleEndian, int maximumDepth)
{
    public readonly int Consumed => reader.Consumed;

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
                tag = new ByteTag(value);
                break;

            case 2 when reader.TryRead(out short value):
                tag = new ShortTag(value);
                break;

            case 3 when reader.TryRead(out int value):
                tag = new IntegerTag(value);
                break;

            case 4 when reader.TryRead(out long value):
                tag = new LongTag(value);
                break;

            case 5 when reader.TryRead(out float value):
                tag = new FloatTag(value);
                break;

            case 6 when reader.TryRead(out double value):
                tag = new DoubleTag(value);
                break;

            case 7 when reader.TryRead(out int length) && reader.TryRead(length, out var span):
                tag = new ByteCollectionTag(span.ToArray());
                break;

            case 8 when reader.TryRead(out string? value):
                tag = new StringTag(value);
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
            tag = new ListTag([]);
            return true;
        }

        if (length > maximumDepth)
        {
            throw new BinaryTagSerializerException("Maximum depth reached.");
        }

        var children = new Tag[length];
        var index = 0;

        while (length > index)
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

            children[index++] = child;
        }

        current = 0;

        tag = new ListTag(children[..index]);
        return true;
    }

    private bool TryReadCompoundTag([NotNullWhen(true)] out CompoundTag? tag)
    {
        tag = null;

        var children = new Tag[maximumDepth];
        var index = 0;

        while (true)
        {
            if (index > maximumDepth)
            {
                throw new BinaryTagSerializerException("Maximum depth reached.");
            }

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

            child.Name = name;
            children[index++] = child;
        }

        tag = new CompoundTag(children[..index]);
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
            tag = new IntegerCollectionTag(MemoryMarshal.Cast<byte, int>(buffer).ToArray());
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

        tag = new IntegerCollectionTag(children);
        return true;
    }

    private bool TryReadLongCollectionTag([NotNullWhen(true)] out LongCollectionTag? tag)
    {
        tag = null;

        if (!reader.TryRead(out int length))
        {
            return false;
        }

        if (BitConverter.IsLittleEndian == littleEndian && reader.TryRead(length * sizeof(long), out var buffer))
        {
            tag = new LongCollectionTag(MemoryMarshal.Cast<byte, long>(buffer).ToArray());
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

        tag = new LongCollectionTag(children);
        return true;
    }
}