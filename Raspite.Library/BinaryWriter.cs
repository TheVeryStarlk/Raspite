using System.Text;

namespace Raspite.Library;

internal sealed class BinaryWriter
{
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

    private List<byte> ReadPayload(int tag, string? name)
    {
        var bytes = new List<byte>()
        {
            (byte) tag, 2
        };

        var length = BitConverter.GetBytes((ushort) (name?.Length ?? 0));

        if (needSwap)
        {
            Array.Reverse(length);
        }

        bytes.AddRange(length);
        bytes.AddRange(Encoding.UTF8.GetBytes(name ?? string.Empty));

        return bytes;
    }

    private byte[] HandleByte(Tag.Byte tag)
    {
        var payload = ReadPayload(1, tag.Name);
        payload.Add(tag.Value);

        return payload.ToArray();
    }

    private byte[] HandleShort(Tag.Short tag)
    {
        var bytes = ReadPayload(2, tag.Name);
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
        var bytes = ReadPayload(3, tag.Name);
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
        var bytes = ReadPayload(4, tag.Name);
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
        var bytes = ReadPayload(5, tag.Name);
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
        var bytes = ReadPayload(6, tag.Name);
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
        var bytes = ReadPayload(7, tag.Name);
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
        var bytes = ReadPayload(8, tag.Name);
        var length = BitConverter.GetBytes((short) tag.Value.Length);

        if (needSwap)
        {
            Array.Reverse(length);
        }

        bytes.AddRange(length);
        bytes.AddRange(Encoding.UTF8.GetBytes(tag.Value));

        return bytes.ToArray();
    }

    private byte[] HandleIntArray(Tag.IntArray tag)
    {
        var bytes = ReadPayload(11, tag.Name);
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

    private byte[] HandleCompound(Tag.Compound tag)
    {
        var bytes = ReadPayload(10, tag.Name);

        bytes.AddRange(tag.Children.SelectMany(Scan));
        bytes.Add(0);

        return bytes.ToArray();
    }

    private byte[] HandleLongArray(Tag.LongArray tag)
    {
        var bytes = ReadPayload(12, tag.Name);
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

    private byte[] HandleList(Tag.List tag)
    {
        throw new NotImplementedException();
    }
}