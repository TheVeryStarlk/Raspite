using System.Text;

namespace Raspite.Library.Reading;

public sealed class Reader
{
    private int current;

    private readonly byte[] source;
    private readonly bool swap;

    public Reader(byte[] source, Endian endian)
    {
        this.source = source;
        swap = BitConverter.IsLittleEndian == endian is Endian.Big;
    }

    public Token Run()
    {
        return Scan();
    }

    private Token Scan(Tag? tag = null)
    {
        // Take into account for nameless tags.
        string? name = null;

        if (tag is null)
        {
            tag = ReadHeader();

            if (tag.Matches(Tag.End))
            {
                return new Token(tag);
            }

            name = HandleString();
        }

        return tag.Type switch
        {
            nameof(Tag.Byte) => new Token.Value(tag, name, HandleByte()),
            nameof(Tag.Short) => new Token.Value(tag, name, HandleShort()),
            nameof(Tag.Int) => new Token.Value(tag, name, HandleInt()),
            nameof(Tag.Long) => new Token.Value(tag, name, HandleLong()),
            nameof(Tag.Float) => new Token.Value(tag, name, HandleFloat()),
            nameof(Tag.Double) => new Token.Value(tag, name, HandleDouble()),
            nameof(Tag.ByteArray) => new Token.Value(tag, name, HandleByteArray()),
            nameof(Tag.String) => new Token.Value(tag, name, HandleString()),
            nameof(Tag.List) => new Token.Parent(tag, name, HandleList()),
            nameof(Tag.Compound) => new Token.Parent(tag, name, HandleCompound()),
            nameof(Tag.IntArray) => new Token.Value(tag, name, HandleIntArray()),
            nameof(Tag.LongArray) => new Token.Value(tag, name, HandleLongArray()),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private Tag ReadHeader()
    {
        var tag = Tag.Resolve(source[current]);

        current++;
        return tag;
    }

    private byte[] ReadPayload(int size, bool numeric = true)
    {
        var bytes = source[current..(current + size)];

        if (swap && numeric)
        {
            Array.Reverse(bytes);
        }

        current += bytes.Length;
        return bytes;
    }

    private byte HandleByte()
    {
        return ReadPayload(Tag.Byte.Size).First();
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
        var bytes = ReadPayload(BitConverter.ToUInt16(ReadPayload(Tag.Short.Size)), false);
        return Encoding.UTF8.GetString(bytes);
    }

    private Token[] HandleList()
    {
        var tag = ReadHeader();
        var tokens = new Token[HandleInt()];

        for (var index = 0; index < tokens.Length; index++)
        {
            tokens[index] = Scan(tag);
        }

        return tokens;
    }

    private Token[] HandleCompound()
    {
        var tokens = new List<Token>();

        // Eat until we hit the ending tag.
        while (!Tag.Resolve(source[current]).Matches(Tag.End))
        {
            tokens.Add(Scan());
        }

        // Eat the ending tag.
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