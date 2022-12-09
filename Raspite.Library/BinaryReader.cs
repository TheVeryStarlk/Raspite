using System.Buffers.Binary;
using System.Text;

namespace Raspite.Library;

internal ref struct BinaryReader
{
    private int current;

    private readonly Span<byte> source;
    private readonly bool bigEndian;

    public BinaryReader(Span<byte> source, NbtSerializerOptions options)
    {
        this.source = source;
        bigEndian = options.Endianness is Endianness.Big;
    }

    public TagBase Run()
    {
        return Scan();
    }

    private TagBase Scan(string? type = null)
    {
        // Take into account for unnamed tags.
        string? name = null;

        if (type is null)
        {
            type = ReadHeader();

            if (type is nameof(TagBase.End))
            {
                return new TagBase.End();
            }

            name = HandleString();
        }

        return type switch
        {
            nameof(TagBase.Byte) => new TagBase.Byte(HandleByte(), name),
            nameof(TagBase.Short) => new TagBase.Short(HandleShort(), name),
            nameof(TagBase.Int) => new TagBase.Int(HandleInt(), name),
            nameof(TagBase.Long) => new TagBase.Long(HandleLong(), name),
            nameof(TagBase.Float) => new TagBase.Float(HandleFloat(), name),
            nameof(TagBase.Double) => new TagBase.Double(HandleDouble(), name),
            nameof(TagBase.ByteArray) => new TagBase.ByteArray(HandleByteArray(), name),
            nameof(TagBase.String) => new TagBase.String(HandleString(), name),
            nameof(TagBase.List) => new TagBase.List(HandleList(), name),
            nameof(TagBase.Compound) => new TagBase.Compound(HandleCompound(), name),
            nameof(TagBase.IntArray) => new TagBase.IntArray(HandleIntArray(), name),
            nameof(TagBase.LongArray) => new TagBase.LongArray(HandleLongArray(), name),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown tag.")
        };
    }

    private string Resolve(int tag)
    {
        return tag switch
        {
            0 => nameof(TagBase.End),
            1 => nameof(TagBase.Byte),
            2 => nameof(TagBase.Short),
            3 => nameof(TagBase.Int),
            4 => nameof(TagBase.Long),
            5 => nameof(TagBase.Float),
            6 => nameof(TagBase.Double),
            7 => nameof(TagBase.ByteArray),
            8 => nameof(TagBase.String),
            9 => nameof(TagBase.List),
            10 => nameof(TagBase.Compound),
            11 => nameof(TagBase.IntArray),
            12 => nameof(TagBase.LongArray),
            _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Unknown tag.")
        };
    }

    private string ReadHeader()
    {
        return Resolve(source[current++]);
    }

    private ReadOnlySpan<byte> ReadPayload(int size)
    {
        var buffer = source.Slice(current, size);

        current += size;
        return buffer;
    }

    private byte HandleByte()
    {
        return ReadPayload(sizeof(byte))[0];
    }

    private short HandleShort()
    {
        var payload = ReadPayload(sizeof(short));

        return bigEndian
            ? BinaryPrimitives.ReadInt16BigEndian(payload)
            : BinaryPrimitives.ReadInt16LittleEndian(payload);
    }

    private int HandleInt()
    {
        var payload = ReadPayload(sizeof(int));

        return bigEndian
            ? BinaryPrimitives.ReadInt32BigEndian(payload)
            : BinaryPrimitives.ReadInt32LittleEndian(payload);
    }

    private long HandleLong()
    {
        var payload = ReadPayload(sizeof(long));

        return bigEndian
            ? BinaryPrimitives.ReadInt64BigEndian(payload)
            : BinaryPrimitives.ReadInt64LittleEndian(payload);
    }

    private float HandleFloat()
    {
        var payload = ReadPayload(sizeof(float));

        return bigEndian
            ? BinaryPrimitives.ReadSingleBigEndian(payload)
            : BinaryPrimitives.ReadSingleLittleEndian(payload);
    }

    private double HandleDouble()
    {
        var payload = ReadPayload(sizeof(double));

        return bigEndian
            ? BinaryPrimitives.ReadDoubleBigEndian(payload)
            : BinaryPrimitives.ReadDoubleLittleEndian(payload);
    }

    private byte[] HandleByteArray()
    {
        return ReadPayload(HandleInt()).ToArray();
    }

    private string HandleString()
    {
        var size = HandleShort();
        var bytes = source.Slice(current, size);

        current += bytes.Length;
        return Encoding.UTF8.GetString(bytes);
    }

    private TagBase[] HandleList()
    {
        var tag = ReadHeader();
        var children = new TagBase[HandleInt()];

        for (var index = 0; index < children.Length; index++)
        {
            children[index] = Scan(tag);
        }

        return children;
    }

    private TagBase[] HandleCompound()
    {
        var tokens = new List<TagBase>();

        // Eat until we hit the ending tag.
        while (Resolve(source[current]) is not nameof(TagBase.End))
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