using System.Text;

namespace Raspite.Library;

internal sealed class BinaryWriter
{
    private bool nameless;

    private readonly Tag source;
    private readonly bool needSwap;

    public BinaryWriter(Tag source, BinaryOptions options)
    {
        this.source = source;
        needSwap = BitConverter.IsLittleEndian == options.Endianness is Endianness.Big;
    }

    public byte[] Run()
    {
        return Scan(source);
    }

    private byte[] Scan(Tag parent)
    {
        return parent switch
        {
            Tag.Byte tag => HandleByte(tag),
            Tag.Short tag => HandleShort(tag),
            Tag.Int tag => HandleInt(tag),
            Tag.Long tag => HandleLong(tag),
            Tag.Float tag => HandleFloat(tag),
            Tag.Double tag => HandleDouble(tag),
            Tag.ByteArray tag => HandleByteArray(tag),
            Tag.String tag => HandleString(tag),
            Tag.List tag => HandleList(tag),
            Tag.Compound tag => HandleCompound(tag),
            Tag.IntArray tag => HandleIntArray(tag),
            Tag.LongArray tag => HandleLongArray(tag),
            _ => throw new ArgumentOutOfRangeException()
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

    private byte[] HandleByte(Tag.Byte tag)
    {
        var payload = WritePayload(1, tag.Name);
        payload.Add(tag.Value);

        return payload.ToArray();
    }

    private byte[] HandleShort(Tag.Short tag)
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

    private byte[] HandleInt(Tag.Int tag)
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

    private byte[] HandleLong(Tag.Long tag)
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

    private byte[] HandleFloat(Tag.Float tag)
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

    private byte[] HandleDouble(Tag.Double tag)
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

    private byte[] HandleByteArray(Tag.ByteArray tag)
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

    private byte[] HandleString(Tag.String tag)
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

    private byte[] HandleList(Tag.List tag)
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

    private byte[] HandleCompound(Tag.Compound tag)
    {
        var bytes = WritePayload(10, tag.Name);

        nameless = false;

        bytes.AddRange(tag.Children.SelectMany(Scan));
        bytes.Add(0);

        return bytes.ToArray();
    }

    private byte[] HandleIntArray(Tag.IntArray tag)
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

    private byte[] HandleLongArray(Tag.LongArray tag)
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