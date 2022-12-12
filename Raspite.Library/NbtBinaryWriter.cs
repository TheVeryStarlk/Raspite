using System.Text;

namespace Raspite.Library;

internal struct NbtBinaryWriter
{
    private bool nameless;

    private readonly NbtTag source;
    private readonly bool needSwap;

    public NbtBinaryWriter(NbtTag source, NbtSerializerOptions options)
    {
        this.source = source;
        needSwap = BitConverter.IsLittleEndian == options.Endianness is Endianness.Big;
    }

    public byte[] Run()
    {
        return Scan(source);
    }

    private byte[] Scan(NbtTag parent)
    {
        return parent switch
        {
            NbtTag.Byte tag => HandleByte(tag),
            NbtTag.Short tag => HandleShort(tag),
            NbtTag.Int tag => HandleInt(tag),
            NbtTag.Long tag => HandleLong(tag),
            NbtTag.Float tag => HandleFloat(tag),
            NbtTag.Double tag => HandleDouble(tag),
            NbtTag.ByteArray tag => HandleByteArray(tag),
            NbtTag.String tag => HandleString(tag),
            NbtTag.List tag => HandleList(tag),
            NbtTag.Compound tag => HandleCompound(tag),
            NbtTag.IntArray tag => HandleIntArray(tag),
            NbtTag.LongArray tag => HandleLongArray(tag),
            _ => throw new ArgumentOutOfRangeException(nameof(parent), parent, "Unexpected tag.")
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
        var length = BitConverter.GetBytes((short) bytes.Length);

        if (needSwap)
        {
            Array.Reverse(length);
        }

        payload.AddRange(length);
        payload.AddRange(bytes);

        return payload;
    }

    private byte[] HandleByte(NbtTag.Byte tag)
    {
        var payload = WritePayload(1, tag.Name);
        payload.Add(tag.Value);

        return payload.ToArray();
    }

    private byte[] HandleShort(NbtTag.Short tag)
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

    private byte[] HandleInt(NbtTag.Int tag)
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

    private byte[] HandleLong(NbtTag.Long tag)
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

    private byte[] HandleFloat(NbtTag.Float tag)
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

    private byte[] HandleDouble(NbtTag.Double tag)
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

    private byte[] HandleByteArray(NbtTag.ByteArray tag)
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

    private byte[] HandleString(NbtTag.String tag)
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

    private byte[] HandleList(NbtTag.List tag)
    {
        var bytes = WritePayload(9, tag.Name);

        var children = tag.Children.Count();

        var type = children > 0 ? Scan(tag.Children.First())[0] : 0;
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

    private byte[] HandleCompound(NbtTag.Compound tag)
    {
        var bytes = WritePayload(10, tag.Name);

        nameless = false;

        bytes.AddRange(tag.Children.SelectMany(Scan));
        bytes.Add(0);

        return bytes.ToArray();
    }

    private byte[] HandleIntArray(NbtTag.IntArray tag)
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

    private byte[] HandleLongArray(NbtTag.LongArray tag)
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