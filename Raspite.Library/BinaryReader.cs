using System.Buffers.Binary;
using System.Text;

namespace Raspite.Library;

internal sealed class BinaryReader
{
    private int current;

    private readonly byte[] source;
    private readonly bool bigEndian;

    public BinaryReader(byte[] source, NbtSerializerOptions options)
    {
        this.source = source;
        bigEndian = options.Endianness is Endianness.Big;
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
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown tag type.")
        };
    }

    private string ReadHeader()
    {
        return Tag.Resolve(source[current++]);
    }

    private ReadOnlySpan<byte> ReadPayload(int size)
    {
        var buffer = source.AsSpan(current, size);

        current += size;
        return buffer;
    }

    private byte HandleByte()
    {
        return ReadPayload(Tag.Byte.Size)[0];
    }

    private short HandleShort()
    {
        var payload = ReadPayload(Tag.Short.Size);

        return bigEndian
            ? BinaryPrimitives.ReadInt16BigEndian(payload)
            : BinaryPrimitives.ReadInt16LittleEndian(payload);
    }

    private int HandleInt()
    {
        var payload = ReadPayload(Tag.Int.Size);

        return bigEndian
            ? BinaryPrimitives.ReadInt32BigEndian(payload)
            : BinaryPrimitives.ReadInt32LittleEndian(payload);
    }

    private long HandleLong()
    {
        var payload = ReadPayload(Tag.Long.Size);

        return bigEndian
            ? BinaryPrimitives.ReadInt64BigEndian(payload)
            : BinaryPrimitives.ReadInt64LittleEndian(payload);
    }

    private float HandleFloat()
    {
        var payload = ReadPayload(Tag.Float.Size);

        return bigEndian
            ? BinaryPrimitives.ReadSingleBigEndian(payload)
            : BinaryPrimitives.ReadSingleLittleEndian(payload);
    }

    private double HandleDouble()
    {
        var payload = ReadPayload(Tag.Double.Size);

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
        var bytes = source.AsSpan().Slice(current, size);

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
        Span<int> ints = stackalloc int[HandleInt()];

        for (var index = 0; index < ints.Length; index++)
        {
            ints[index] = HandleInt();
        }

        return ints.ToArray();
    }

    private long[] HandleLongArray()
    {
        Span<long> longs = stackalloc long[HandleInt()];

        for (var index = 0; index < longs.Length; index++)
        {
            longs[index] = HandleLong();
        }

        return longs.ToArray();
    }
}