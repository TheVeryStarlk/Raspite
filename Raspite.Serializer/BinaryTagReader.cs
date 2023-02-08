using System.Buffers.Binary;
using System.Numerics;
using System.Text;
using Raspite.Serializer.Tags;

namespace Raspite.Serializer;

public sealed class BinaryTagReaderException : InvalidOperationException
{
    public BinaryTagReaderException(object value, string message) : base($"{message} At '{value}'.")
    {
    }
}

internal ref struct BinaryTagReader
{
    private int current;

    private readonly ReadOnlySpan<byte> source;
    private readonly bool littleEndian;

    public BinaryTagReader(ReadOnlySpan<byte> source, bool littleEndian)
    {
        this.source = source;
        this.littleEndian = littleEndian;
    }

    public TagBase Read(TagBase.Type? type = null)
    {
        string? name = null;

        if (type is null)
        {
            type = (TagBase.Type) HandleByte();

            if (type is TagBase.Type.End)
            {
                throw new BinaryTagWriterException("Unexpected ending tag.");
            }

            name = HandleString();
        }

        TagBase tag = type switch
        {
            TagBase.Type.Byte => new ByteTag()
            {
                Value = HandleByte()
            },
            TagBase.Type.Short => new ShortTag()
            {
                Value = HandleShort()
            },
            TagBase.Type.Int => new IntTag()
            {
                Value = HandleInt()
            },
            TagBase.Type.Long => new LongTag()
            {
                Value = HandleLong()
            },
            TagBase.Type.Float => new FloatTag()
            {
                Value = HandleFloat()
            },
            TagBase.Type.Double => new DoubleTag()
            {
                Value = HandleDouble()
            },
            TagBase.Type.ByteArray => new ByteArrayTag()
            {
                Value = HandleByteArray()
            },
            TagBase.Type.String => new StringTag()
            {
                Value = HandleString()
            },
            TagBase.Type.List => new ListTag()
            {
                Value = HandleList()
            },
            TagBase.Type.Compound => new CompoundTag()
            {
                Value = HandleCompound()
            },
            TagBase.Type.IntArray => new IntArrayTag()
            {
                Value = HandleIntArray()
            },
            TagBase.Type.LongArray => new LongArrayTag()
            {
                Value = HandleLongArray()
            },
            _ => throw new BinaryTagReaderException(type, "Unexpected tag type.")
        };

        tag.Name = name;
        return tag;
    }

    private ReadOnlySpan<byte> ReadPayload(int length)
    {
        if (current >= source.Length)
        {
            throw new BinaryTagReaderException(current, "Index cannot be bigger than the source's length.");
        }

        if (length > source.Length)
        {
            throw new BinaryTagReaderException(length, "Length cannot be bigger than the source's length.");
        }

        var buffer = source.Slice(current, Validate(length));

        current += length;
        return buffer;
    }

    private byte HandleByte()
    {
        return ReadPayload(sizeof(byte))[0];
    }

    private short HandleShort()
    {
        var payload = ReadPayload(sizeof(short));

        return littleEndian
            ? BinaryPrimitives.ReadInt16LittleEndian(payload)
            : BinaryPrimitives.ReadInt16BigEndian(payload);
    }

    private int HandleInt()
    {
        var payload = ReadPayload(sizeof(int));

        return littleEndian
            ? BinaryPrimitives.ReadInt32LittleEndian(payload)
            : BinaryPrimitives.ReadInt32BigEndian(payload);
    }

    private long HandleLong()
    {
        var payload = ReadPayload(sizeof(long));

        return littleEndian
            ? BinaryPrimitives.ReadInt64LittleEndian(payload)
            : BinaryPrimitives.ReadInt64BigEndian(payload);
    }

    private float HandleFloat()
    {
        var payload = ReadPayload(sizeof(float));

        return littleEndian
            ? BinaryPrimitives.ReadSingleLittleEndian(payload)
            : BinaryPrimitives.ReadSingleBigEndian(payload);
    }

    private double HandleDouble()
    {
        var payload = ReadPayload(sizeof(double));

        return littleEndian
            ? BinaryPrimitives.ReadDoubleLittleEndian(payload)
            : BinaryPrimitives.ReadDoubleBigEndian(payload);
    }

    private IEnumerable<byte> HandleByteArray()
    {
        var length = Validate(HandleInt());
        return ReadPayload(length).ToArray();
    }

    private string HandleString()
    {
        var length = Validate(HandleShort());
        return Encoding.UTF8.GetString(ReadPayload(length));
    }

    private IEnumerable<TagBase> HandleList()
    {
        var type = (TagBase.Type) HandleByte();
        var length = Validate(HandleInt());
        var children = new TagBase[length];

        for (var index = 0; index < children.Length; index++)
        {
            children[index] = Read(type);
        }

        return children;
    }

    private IEnumerable<TagBase> HandleCompound()
    {
        var children = new List<TagBase>();

        // Eat until we hit the ending tag.
        while (true)
        {
            if (current >= source.Length)
            {
                if (source[^1] is not (byte) TagBase.Type.End)
                {
                    throw new BinaryTagReaderException(current, "Compound tag did not have an ending tag.");
                }

                break;
            }

            if (source[current] is (byte) TagBase.Type.End)
            {
                break;
            }

            children.Add(Read());
        }

        current++;
        return children;
    }

    private IEnumerable<int> HandleIntArray()
    {
        var length = Validate(HandleInt());
        var ints = new int[length];

        for (var index = 0; index < ints.Length; index++)
        {
            ints[index] = HandleInt();
        }

        return ints;
    }

    private IEnumerable<long> HandleLongArray()
    {
        var length = Validate(HandleInt());
        var longs = new long[length];

        for (var index = 0; index < longs.Length; index++)
        {
            longs[index] = HandleLong();
        }

        return longs;
    }

    private T Validate<T>(T value) where T : INumber<T>
    {
        if (T.IsNegative(value))
        {
            throw new BinaryTagReaderException(value, "Length cannot be negative.");
        }

        return value;
    }
}