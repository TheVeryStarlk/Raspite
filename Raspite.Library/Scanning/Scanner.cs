using System.Text;

namespace Raspite.Library.Scanning;

public sealed class Scanner
{
    private int current;

    private readonly byte[] source;
    private readonly bool swap;

    public Scanner(byte[] source, Endian endian)
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
            // Get the tag's ID byte.
            tag = Tag.Resolve(source[current]);

            // We finished eating the tag's ID byte.
            current++;

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
        // Get the byte array's length, then eat its payload.
        return ReadPayload(HandleInt());
    }

    private string HandleString()
    {
        // Get the string's length.
        var length = BitConverter.ToUInt16(ReadPayload(Tag.Short.Size));

        // Start eating the string's bytes.
        var bytes = source[current..(current + length)];

        // We finished eating the string's bytes.
        current += length;

        return Encoding.UTF8.GetString(bytes);
    }

    private Token[] HandleList()
    {
        // Get the tag's ID byte.
        var tag = Tag.Resolve(source[current]);

        // We finished eating the tag's ID byte.
        current++;

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
            var token = Scan();
            tokens.Add(token);
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

    private byte[] ReadPayload(int size)
    {
        // Start eating the payload's bytes.
        var bytes = source[current..(current + size)];

        if (swap)
        {
            Array.Reverse(bytes);
        }

        // We finished eating the payload's bytes.
        current += bytes.Length;

        return bytes;
    }
}