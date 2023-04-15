using Raspite.Serializer.Streams;
using Raspite.Serializer.Tags;

namespace Raspite.Serializer;

/// <summary>
/// Represents an error that occured while reading.
/// </summary>
public sealed class BinaryTagReaderException : Exception
{
    public BinaryTagReaderException(string message) : base(message)
    {
    }
}

internal sealed class BinaryTagReader
{
    private readonly ReadableBinaryStream stream;

    public BinaryTagReader(ReadableBinaryStream stream)
    {
        this.stream = stream;
    }

    public async Task<T> EvaluateAsync<T>(byte? type = null) where T : Tag
    {
        type ??= (byte) stream.ReadByte();

        var name = await stream.ReadStringAsync();

        Tag result = type switch
        {
            1 => ReadSignedByteTag(name),
            2 => await ReadShortTagAsync(name),
            3 => await ReadIntegerTagAsync(name),
            4 => await ReadLongTagAsync(name),
            5 => await ReadFloatTagAsync(name),
            6 => await ReadDoubleTagAsync(name),
            7 => await ReadSignedByteCollectionTagAsync(name),
            8 => await ReadStringTagAsync(name),
            10 => await ReadCompoundTagAsync(name),
            11 => await ReadIntegerCollectionTagAsync(name),
            12 => await ReadLongCollectionTagAsync(name),
            _ => throw new BinaryTagReaderException("Unknown tag type.")
        };

        return (T) result;
    }

    private SignedByteTag ReadSignedByteTag(string name)
    {
        var value = stream.ReadSignedByte();

        return new SignedByteTag()
        {
            Name = name,
            Value = value
        };
    }

    private async Task<ShortTag> ReadShortTagAsync(string name)
    {
        var value = await stream.ReadShortAsync();

        return new ShortTag()
        {
            Name = name,
            Value = value
        };
    }

    private async Task<IntegerTag> ReadIntegerTagAsync(string name)
    {
        var value = await stream.ReadIntegerAsync();

        return new IntegerTag()
        {
            Name = name,
            Value = value
        };
    }

    private async Task<LongTag> ReadLongTagAsync(string name)
    {
        var value = await stream.ReadLongAsync();

        return new LongTag()
        {
            Name = name,
            Value = value
        };
    }

    private async Task<FloatTag> ReadFloatTagAsync(string name)
    {
        var value = await stream.ReadFloatAsync();

        return new FloatTag()
        {
            Name = name,
            Value = value
        };
    }

    private async Task<DoubleTag> ReadDoubleTagAsync(string name)
    {
        var value = await stream.ReadDoubleAsync();

        return new DoubleTag()
        {
            Name = name,
            Value = value
        };
    }

    private async Task<SignedByteCollectionTag> ReadSignedByteCollectionTagAsync(string name)
    {
        var size = await stream.ReadIntegerAsync();
        var children = await stream.ReadSignedBytesAsync(size);

        return new SignedByteCollectionTag()
        {
            Name = name,
            Children = children
        };
    }

    private async Task<StringTag> ReadStringTagAsync(string name)
    {
        var value = await stream.ReadStringAsync();

        return new StringTag()
        {
            Name = name,
            Value = value
        };
    }

    private async Task<CompoundTag> ReadCompoundTagAsync(string name)
    {
        var children = new List<Tag>();

        while (true)
        {
            var current = stream.ReadByte();

            switch (current)
            {
                case -1:
                    throw new BinaryTagReaderException("Compound tag did not end with an ending tag.");

                case 0:
                    return new CompoundTag()
                    {
                        Name = name,
                        Children = children.ToArray()
                    };

                default:
                    children.Add(await EvaluateAsync<Tag>((byte) current));
                    break;
            }
        }
    }

    private async Task<IntegerCollectionTag> ReadIntegerCollectionTagAsync(string name)
    {
        var size = await stream.ReadIntegerAsync();
        var children = new int[size];

        for (var index = 0; index < size; index++)
        {
            children[index] = await stream.ReadIntegerAsync();
        }

        return new IntegerCollectionTag()
        {
            Name = name,
            Children = children
        };
    }

    private async Task<LongCollectionTag> ReadLongCollectionTagAsync(string name)
    {
        var size = await stream.ReadIntegerAsync();
        var children = new long[size];

        for (var index = 0; index < size; index++)
        {
            children[index] = await stream.ReadLongAsync();
        }

        return new LongCollectionTag()
        {
            Name = name,
            Children = children
        };
    }
}