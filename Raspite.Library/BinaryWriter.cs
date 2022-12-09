using System.Text;

namespace Raspite.Library;

internal struct BinaryWriter
{
    private bool nameless;

    private readonly TagBase source;
    private readonly bool needSwap;

    public BinaryWriter(TagBase source, NbtSerializerOptions options)
    {
        this.source = source;
        needSwap = BitConverter.IsLittleEndian == options.Endianness is Endianness.Big;
    }

    public byte[] Run()
    {
        return Scan(source);
    }

    private byte[] Scan(TagBase parent)
    {
        return parent switch
        {
            TagBase.Byte tag => HandleByte(tag),
            TagBase.Short tag => HandleShort(tag),
            TagBase.Int tag => HandleInt(tag),
            TagBase.Long tag => HandleLong(tag),
            TagBase.Float tag => HandleFloat(tag),
            TagBase.Double tag => HandleDouble(tag),
            TagBase.ByteArray tag => HandleByteArray(tag),
            TagBase.String tag => HandleString(tag),
            TagBase.List tag => HandleList(tag),
            TagBase.Compound tag => HandleCompound(tag),
            TagBase.IntArray tag => HandleIntArray(tag),
            TagBase.LongArray tag => HandleLongArray(tag),
            _ => throw new ArgumentOutOfRangeException(nameof(parent), parent, "Unknown tag.")
        };
    }

    private List<byte> WritePayload(int tag, string? name)
    {
        if (nameless)
        {
            return new List<byte>();
        }

        name ??= string.Empty;

        var payload = new List<byte>()
        {
            (byte) tag
        };

        var bytes = Encoding.UTF8.GetBytes(name);
        var length = BitConverter.GetBytes((ushort) bytes.Length);

        if (needSwap)
        {
            Array.Reverse(length);
        }

        payload.AddRange(length);
        payload.AddRange(bytes);

        return payload;
    }

    private byte[] HandleByte(TagBase.Byte tag)
    {
        var payload = WritePayload(1, tag.Name);
        payload.Add(tag.Value);

        return payload.ToArray();
    }

    private byte[] HandleShort(TagBase.Short tag)
    {
        var bytes = WritePayload(2, tag.Name);
        var value = BitConverter.GetBytes(tag.Value);

        if (needSwap)
        {
            Array.Reverse(value);
        }

        bytes.AddRange(value);
        return bytes.ToArray();
    }

    private byte[] HandleInt(TagBase.Int tag)
    {
        var bytes = WritePayload(3, tag.Name);
        var value = BitConverter.GetBytes(tag.Value);

        if (needSwap)
        {
            Array.Reverse(value);
        }

        bytes.AddRange(value);
        return bytes.ToArray();
    }

    private byte[] HandleLong(TagBase.Long tag)
    {
        var bytes = WritePayload(4, tag.Name);
        var value = BitConverter.GetBytes(tag.Value);

        if (needSwap)
        {
            Array.Reverse(value);
        }

        bytes.AddRange(value);
        return bytes.ToArray();
    }

    private byte[] HandleFloat(TagBase.Float tag)
    {
        var bytes = WritePayload(5, tag.Name);
        var value = BitConverter.GetBytes(tag.Value);

        if (needSwap)
        {
            Array.Reverse(value);
        }

        bytes.AddRange(value);
        return bytes.ToArray();
    }

    private byte[] HandleDouble(TagBase.Double tag)
    {
        var bytes = WritePayload(6, tag.Name);
        var value = BitConverter.GetBytes(tag.Value);

        if (needSwap)
        {
            Array.Reverse(value);
        }

        bytes.AddRange(value);
        return bytes.ToArray();
    }

    private byte[] HandleByteArray(TagBase.ByteArray tag)
    {
        var bytes = WritePayload(7, tag.Name);
        var length = BitConverter.GetBytes(tag.Values.Length);

        if (needSwap)
        {
            Array.Reverse(length);
        }

        bytes.AddRange(length);
        bytes.AddRange(tag.Values);

        return bytes.ToArray();
    }

    private byte[] HandleString(TagBase.String tag)
    {
        var bytes = WritePayload(8, tag.Name);

        var value = Encoding.UTF8.GetBytes(tag.Value);
        var length = BitConverter.GetBytes((short) value.Length);

        if (needSwap)
        {
            Array.Reverse(length);
        }

        bytes.AddRange(length);
        bytes.AddRange(value);

        return bytes.ToArray();
    }

    private byte[] HandleList(TagBase.List tag)
    {
        var bytes = WritePayload(9, tag.Name);

        var children = tag.Children.Length;

        var type = children > 0 ? Scan(tag.Children[0])[0] : 0;
        bytes.Add((byte) type);

        var length = BitConverter.GetBytes(children);

        if (needSwap)
        {
            Array.Reverse(length);
        }

        bytes.AddRange(length);

        foreach (var child in tag.Children)
        {
            nameless = true;
            bytes.AddRange(Scan(child));
        }

        nameless = false;

        return bytes.ToArray();
    }

    private byte[] HandleCompound(TagBase.Compound tag)
    {
        var bytes = WritePayload(10, tag.Name);

        nameless = false;

        bytes.AddRange(tag.Children.SelectMany(Scan));
        bytes.Add(0);

        return bytes.ToArray();
    }

    private byte[] HandleIntArray(TagBase.IntArray tag)
    {
        var bytes = WritePayload(11, tag.Name);
        var length = BitConverter.GetBytes(tag.Values.Length);

        if (needSwap)
        {
            Array.Reverse(length);
        }

        bytes.AddRange(length);

        foreach (var value in tag.Values)
        {
            var integer = BitConverter.GetBytes(value);

            if (needSwap)
            {
                Array.Reverse(integer);
            }

            bytes.AddRange(integer);
        }

        return bytes.ToArray();
    }

    private byte[] HandleLongArray(TagBase.LongArray tag)
    {
        var bytes = WritePayload(12, tag.Name);
        var length = BitConverter.GetBytes(tag.Values.Length);

        if (needSwap)
        {
            Array.Reverse(length);
        }

        bytes.AddRange(length);

        foreach (var value in tag.Values)
        {
            var @long = BitConverter.GetBytes(value);

            if (needSwap)
            {
                Array.Reverse(@long);
            }

            bytes.AddRange(@long);
        }

        return bytes.ToArray();
    }
}