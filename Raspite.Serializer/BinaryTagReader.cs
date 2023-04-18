using Raspite.Serializer.Streams;
using Raspite.Serializer.Tags;

namespace Raspite.Serializer;

/// <summary>
/// Represents an error that occurred while reading.
/// </summary>
public sealed class BinaryTagReaderException : Exception
{
    public BinaryTagReaderException(string message) : base(message)
    {
    }
}

internal sealed class BinaryTagReader
{
    private bool isNameless;

    private readonly ReadableBinaryStream stream;

    public BinaryTagReader(ReadableBinaryStream stream)
    {
        this.stream = stream;
    }

    public async Task<Tag> EvaluateAsync(byte? type = null)
    {
        type ??= (byte) stream.ReadByte();

        var name = string.Empty;

        // Do not read the name if we're inside a list.
        if (!isNameless)
        {
            name = await stream.ReadStringAsync();
        }

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
            9 => await ReadListTagAsync(name),
            10 => await ReadCompoundTagAsync(name),
            11 => await ReadIntegerCollectionTagAsync(name),
            12 => await ReadLongCollectionTagAsync(name),
            _ => throw new BinaryTagReaderException($"Unknown tag type {type}.")
        };

        return result;
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

    private async Task<ListTag> ReadListTagAsync(string name)
    {
        var predefinedType = (byte) stream.ReadByte();

        var size = await stream.ReadIntegerAsync();
        var children = new Tag[size];

        for (var index = 0; index < size; index++)
        {
            isNameless = true;
            children[index] = await EvaluateAsync(predefinedType);
        }

        isNameless = false;

        return new ListTag()
        {
            Name = name,
            Children = children
        };
    }

    private async Task<CompoundTag> ReadCompoundTagAsync(string name)
    {
        var children = new List<Tag>();
        var wasNameless = isNameless;

        isNameless = false;

        while (true)
        {
            var current = stream.ReadByte();

            switch (current)
            {
                case -1:
                    throw new BinaryTagReaderException("Compound tag did not end with an ending tag.");

                case 0:
                    isNameless = wasNameless;

                    return new CompoundTag()
                    {
                        Name = name,
                        Children = children.ToArray()
                    };

                default:
                    children.Add(await EvaluateAsync((byte) current));
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