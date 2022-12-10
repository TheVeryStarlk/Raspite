using System.Buffers.Binary;
using System.Text;

namespace Raspite.Library;

internal ref struct BinaryReader
{
    private int current;

    private readonly ReadOnlySpan<byte> source;
    private readonly bool bigEndian;

    public BinaryReader(ReadOnlySpan<byte> source, NbtSerializerOptions options)
    {
        this.source = source;
        bigEndian = options.Endianness is Endianness.Big;
    }

    public NbtTag Run()
    {
        return Scan();
    }

    private NbtTag Scan(string? type = null)
    {
        // Take into account for unnamed tags.
        string? name = null;

        if (type is null)
        {
            type = ReadHeader();

            if (type is nameof(NbtTag.End))
            {
                return new NbtTag.End();
            }

            name = HandleString();
        }

        return type switch
        {
            nameof(NbtTag.Byte) => new NbtTag.Byte(HandleByte(), name),
            nameof(NbtTag.Short) => new NbtTag.Short(HandleShort(), name),
            nameof(NbtTag.Int) => new NbtTag.Int(HandleInt(), name),
            nameof(NbtTag.Long) => new NbtTag.Long(HandleLong(), name),
            nameof(NbtTag.Float) => new NbtTag.Float(HandleFloat(), name),
            nameof(NbtTag.Double) => new NbtTag.Double(HandleDouble(), name),
            nameof(NbtTag.ByteArray) => new NbtTag.ByteArray(HandleByteArray(), name),
            nameof(NbtTag.String) => new NbtTag.String(HandleString(), name),
            nameof(NbtTag.List) => new NbtTag.List(HandleList(), name),
            nameof(NbtTag.Compound) => new NbtTag.Compound(HandleCompound(), name),
            nameof(NbtTag.IntArray) => new NbtTag.IntArray(HandleIntArray(), name),
            nameof(NbtTag.LongArray) => new NbtTag.LongArray(HandleLongArray(), name),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown tag.")
        };
    }

    private string Resolve(int tag)
    {
        return tag switch
        {
            0 => nameof(NbtTag.End),
            1 => nameof(NbtTag.Byte),
            2 => nameof(NbtTag.Short),
            3 => nameof(NbtTag.Int),
            4 => nameof(NbtTag.Long),
            5 => nameof(NbtTag.Float),
            6 => nameof(NbtTag.Double),
            7 => nameof(NbtTag.ByteArray),
            8 => nameof(NbtTag.String),
            9 => nameof(NbtTag.List),
            10 => nameof(NbtTag.Compound),
            11 => nameof(NbtTag.IntArray),
            12 => nameof(NbtTag.LongArray),
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

    private NbtTag[] HandleList()
    {
        var tag = ReadHeader();
        var children = new NbtTag[HandleInt()];

        for (var index = 0; index < children.Length; index++)
        {
            children[index] = Scan(tag);
        }

        return children;
    }

    private NbtTag[] HandleCompound()
    {
        var tokens = new List<NbtTag>();

        // Eat until we hit the ending tag.
        while (Resolve(source[current]) is not nameof(NbtTag.End))
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