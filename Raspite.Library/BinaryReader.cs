using System.Text;

namespace Raspite.Library;

internal sealed class BinaryReader
{
    private int current;

    private readonly byte[] source;
    private readonly bool needSwap;

    public BinaryReader(byte[] source, BinaryOptions options)
    {
        this.source = source;
        needSwap = BitConverter.IsLittleEndian == options.Endianness is Endianness.Big;
    }

    public Tag Run()
    {
        return Scan();
    }

    private Tag Scan(string? type = null)
    {
        // Take into account for unnamed tags.
        string? name = null;

        if (type is null)
        {
            type = ReadHeader();

            if (type is nameof(Tag.End))
            {
                return new Tag.End();
            }

            name = HandleString();
        }

        return type switch
        {
            nameof(Tag.Byte) => new Tag.Byte(HandleByte(), name),
            nameof(Tag.Short) => new Tag.Short(HandleShort(), name),
            nameof(Tag.Int) => new Tag.Int(HandleInt(), name),
            nameof(Tag.Long) => new Tag.Long(HandleLong(), name),
            nameof(Tag.Float) => new Tag.Float(HandleFloat(), name),
            nameof(Tag.Double) => new Tag.Double(HandleDouble(), name),
            nameof(Tag.ByteArray) => new Tag.ByteArray(HandleByteArray(), name),
            nameof(Tag.String) => new Tag.String(HandleString(), name),
            nameof(Tag.List) => new Tag.List(HandleList(), name),
            nameof(Tag.Compound) => new Tag.Compound(HandleCompound(), name),
            nameof(Tag.IntArray) => new Tag.IntArray(HandleIntArray(), name),
            nameof(Tag.LongArray) => new Tag.LongArray(HandleLongArray(), name),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private string ReadHeader()
    {
        return Tag.Resolve(source[current++]);
    }

    private byte[] ReadPayload(int size)
    {
        var bytes = source[current..(current + size)];

        if (needSwap)
        {
            Array.Reverse(bytes);
        }

        current += bytes.Length;
        return bytes;
    }

    private byte HandleByte()
    {
        return ReadPayload(Tag.Byte.Size)[0];
    }

    private short HandleShort()
    {
        return BitConverter.ToInt16(ReadPayload(Tag.Short.Size));
    }

    private int HandleInt()
    {
        return BitConverter.ToInt32(ReadPayload(Tag.Int.Size));
    }

    private long HandleLong()
    {
        return BitConverter.ToInt64(ReadPayload(Tag.Long.Size));
    }

    private float HandleFloat()
    {
        return BitConverter.ToSingle(ReadPayload(Tag.Float.Size));
    }

    private double HandleDouble()
    {
        return BitConverter.ToDouble(ReadPayload(Tag.Double.Size));
    }

    private byte[] HandleByteArray()
    {
        return ReadPayload(HandleInt());
    }

    private string HandleString()
    {
        var size = BitConverter.ToUInt16(ReadPayload(Tag.Short.Size));
        var bytes = source[current..(current + size)];

        current += bytes.Length;
        return Encoding.UTF8.GetString(bytes);
    }

    private Tag[] HandleList()
    {
        var tag = ReadHeader();
        var children = new Tag[HandleInt()];

        for (var index = 0; index < children.Length; index++)
        {
            children[index] = Scan(tag);
        }

        return children;
    }

    private Tag[] HandleCompound()
    {
        var tokens = new List<Tag>();

        // Eat until we hit the ending tag.
        while (Tag.Resolve(source[current]) is not nameof(Tag.End))
        {
            tokens.Add(Scan());
        }

        current++;
        return tokens.ToArray();
    }

    private int[] HandleIntArray()
    {
        var ints = new int[HandleInt()];

        for (var index = 0; index < ints.Length; index++)
        {
            ints[index] = HandleInt();
        }

        return ints;
    }

    private long[] HandleLongArray()
    {
        var longs = new long[HandleInt()];

        for (var index = 0; index < longs.Length; index++)
        {
            longs[index] = HandleLong();
        }

        return longs;
    }
}