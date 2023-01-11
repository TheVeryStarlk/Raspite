using System.Runtime.InteropServices;
using System.Text;

namespace Raspite.Library;

public sealed class BinaryTagWriterException : InvalidOperationException
{
    public BinaryTagWriterException(string message) : base(message)
    {
    }

    public BinaryTagWriterException(object value, string message) : base($"{message} At '{value}'.")
    {
    }
}

internal sealed class BinaryTagWriter
{
    // Store state whether we're in a list or not.
    private bool nameless;

    private readonly bool needSwap;

    public BinaryTagWriter(bool littleEndian)
    {
        needSwap = BitConverter.IsLittleEndian != littleEndian;
    }

    public ReadOnlySpan<byte> Write(TagBase parent)
    {
        return parent switch
        {
            EndTag => throw new BinaryTagWriterException("Unexpected ending tag."),
            ByteTag tag => HandleByte(tag),
            ShortTag tag => HandleShort(tag),
            IntTag tag => HandleInt(tag),
            LongTag tag => HandleLong(tag),
            FloatTag tag => HandleFloat(tag),
            DoubleTag tag => HandleDouble(tag),
            ByteArrayTag tag => HandleByteArray(tag),
            StringTag tag => HandleString(tag),
            ListTag tag => HandleList(tag),
            CompoundTag tag => HandleCompound(tag),
            IntArrayTag tag => HandleIntArray(tag),
            LongArrayTag tag => HandleLongArray(tag),
            _ => throw new BinaryTagWriterException(parent, "Unknown tag.")
        };
    }

    private ReadOnlySpan<byte> WritePayload(TagBase.Type type, byte[] body, string? name)
    {
        if (nameless)
        {
            return body;
        }

        name ??= string.Empty;

        var payload = new List<byte>()
        {
            (byte) type
        };

        var nameBytes = Encoding.UTF8.GetBytes(name);
        var nameLength = BitConverter.GetBytes((short) nameBytes.Length);

        payload.AddRange(Reverse(nameLength));
        payload.AddRange(nameBytes);
        payload.AddRange(body);

        return CollectionsMarshal.AsSpan(payload);
    }

    private ReadOnlySpan<byte> HandleByte(ByteTag tag)
    {
        var value = new byte[]
        {
            tag.Value
        };

        var payload = WritePayload(TagBase.Type.Byte, value, tag.Name);
        return payload;
    }

    private ReadOnlySpan<byte> HandleShort(ShortTag tag)
    {
        var value = Reverse(BitConverter.GetBytes(tag.Value));
        var payload = WritePayload(TagBase.Type.Short, value, tag.Name);

        return payload;
    }

    private ReadOnlySpan<byte> HandleInt(IntTag tag)
    {
        var value = Reverse(BitConverter.GetBytes(tag.Value));
        var payload = WritePayload(TagBase.Type.Int, value, tag.Name);

        return payload;
    }

    private ReadOnlySpan<byte> HandleLong(LongTag tag)
    {
        var value = Reverse(BitConverter.GetBytes(tag.Value));
        var payload = WritePayload(TagBase.Type.Long, value, tag.Name);

        return payload;
    }

    private ReadOnlySpan<byte> HandleFloat(FloatTag tag)
    {
        var value = Reverse(BitConverter.GetBytes(tag.Value));
        var payload = WritePayload(TagBase.Type.Float, value, tag.Name);

        return payload;
    }

    private ReadOnlySpan<byte> HandleDouble(DoubleTag tag)
    {
        var value = Reverse(BitConverter.GetBytes(tag.Value));
        var payload = WritePayload(TagBase.Type.Double, value, tag.Name);

        return payload;
    }

    private ReadOnlySpan<byte> HandleByteArray(ByteArrayTag tag)
    {
        var value = tag.Value.ToArray();
        var length = Reverse(BitConverter.GetBytes(value.Length));

        return WritePayload(TagBase.Type.ByteArray, length.Concat(value).ToArray(), tag.Name);
    }

    private ReadOnlySpan<byte> HandleString(StringTag tag)
    {
        var value = Encoding.UTF8.GetBytes(tag.Value);
        var length = Reverse(BitConverter.GetBytes((short) value.Length));

        return WritePayload(TagBase.Type.String, length.Concat(value).ToArray(), tag.Name);
    }

    private ReadOnlySpan<byte> HandleList(ListTag tag)
    {
        var type = (byte) (tag.Value.Any() ? Write(tag.Value.First())[0] : 0);
        var length = Reverse(BitConverter.GetBytes(tag.Value.Count()));

        var buffer = new List<byte>()
        {
            type
        };

        buffer.AddRange(length);

        foreach (var value in tag.Value)
        {
            nameless = true;

            var currentType = value switch
            {
                ByteTag => TagBase.Type.Byte,
                ShortTag => TagBase.Type.Short,
                IntTag => TagBase.Type.Int,
                LongTag => TagBase.Type.Long,
                FloatTag => TagBase.Type.Float,
                DoubleTag => TagBase.Type.Double,
                ByteArrayTag => TagBase.Type.ByteArray,
                StringTag => TagBase.Type.String,
                ListTag => TagBase.Type.List,
                CompoundTag => TagBase.Type.Compound,
                IntArrayTag => TagBase.Type.IntArray,
                LongArrayTag => TagBase.Type.LongArray,
                _ => throw new BinaryTagWriterException(value, "Unexpected tag.")
            };

            if (currentType != (TagBase.Type) type)
            {
                throw new BinaryTagWriterException(
                    type,
                    $"List contained a type different than the predefined one ('{currentType}').");
            }

            buffer.AddRange(Write(value).ToArray());
        }

        nameless = false;
        return WritePayload(TagBase.Type.List, buffer.ToArray(), tag.Name);
    }

    private ReadOnlySpan<byte> HandleCompound(CompoundTag tag)
    {
        var wasNameless = nameless;
        var buffer = new List<byte>();

        nameless = false;
        buffer.AddRange(tag.Value.SelectMany(value => Write(value).ToArray()));
        buffer.Add((byte) TagBase.Type.End);

        nameless = wasNameless;
        return WritePayload(TagBase.Type.Compound, buffer.ToArray(), tag.Name);
    }

    private ReadOnlySpan<byte> HandleIntArray(IntArrayTag tag)
    {
        var length = Reverse(BitConverter.GetBytes(tag.Value.Count()));

        var buffer = new List<byte>();
        buffer.AddRange(length);

        foreach (var value in tag.Value)
        {
            buffer.AddRange(Reverse(BitConverter.GetBytes(value)));
        }

        return WritePayload(TagBase.Type.IntArray, buffer.ToArray(), tag.Name);
    }

    private ReadOnlySpan<byte> HandleLongArray(LongArrayTag tag)
    {
        var length = Reverse(BitConverter.GetBytes(tag.Value.Count()));

        var buffer = new List<byte>();
        buffer.AddRange(length);

        foreach (var value in tag.Value)
        {
            buffer.AddRange(Reverse(BitConverter.GetBytes(value)));
        }

        return WritePayload(TagBase.Type.LongArray, buffer.ToArray(), tag.Name);
    }

    private byte[] Reverse(byte[] value)
    {
        if (needSwap)
        {
            Array.Reverse(value);
        }

        return value;
    }
}