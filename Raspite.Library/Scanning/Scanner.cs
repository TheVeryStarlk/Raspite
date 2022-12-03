using System.Text;

namespace Raspite.Library.Scanning;

/// <summary>
/// Converts a single dimensional sequence of bytes into a hierarchical representation in <see cref="Token"/>. 
/// </summary>
public sealed class Scanner
{
    private int current;

    private readonly byte[] source;
    private readonly bool swap;

    public Scanner(byte[] source, Endian endian = Endian.Big)
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
        var name = tag?.Type;

        if (tag is null)
        {
            tag = ReadHeader();

            if (tag.Type is nameof(Tag.End))
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
        // Get the tag's ID byte.
        var tag = Tag.Resolve(source[current]);

        // We finished eating the tag's ID byte.
        current++;

        return tag;
    }

    private byte[] ReadPayload(int size, bool numeric = true)
    {
        // Start eating the payload's bytes.
        var bytes = source[current..(current + size)];

        // Sometimes we need to override swapping because endianness is not present in payloads like strings.
        if (swap && numeric)
        {
            Array.Reverse(bytes);
        }

        // We finished eating the payload's bytes.
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
        // Get the byte array's length, then eat its payload.
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

        // Eat the list's length.
        var length = HandleInt();

        // Start eating the list's payload.
        var tokens = new Token[length];

        for (var index = 0; index < length; index++)
        {
            tokens[index] = Scan(tag);
        }

        return tokens;
    }

    private Token[] HandleCompound()
    {
        var tokens = new List<Token>();

        // Eat until we hit the ending tag.
        while (Tag.Resolve(source[current]).Type is not nameof(Tag.End))
        {
            tokens.Add(Scan());
        }

        // Eat the ending tag.
        current++;

        return tokens.ToArray();
    }

    private int[] HandleIntArray()
    {
        // Eat the int array's length.
        var length = HandleInt();

        // Start eating the int array's payload.
        var ints = new int[length];

        for (var index = 0; index < length; index++)
        {
            ints[index] = HandleInt();
        }

        return ints;
    }

    private long[] HandleLongArray()
    {
        // Eat the long array's length.
        var length = HandleInt();

        // Start eating the long array's payload.
        var longs = new long[length];

        for (var index = 0; index < length; index++)
        {
            longs[index] = HandleLong();
        }

        return longs;
    }
}