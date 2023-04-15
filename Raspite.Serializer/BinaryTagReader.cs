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
            8 => await ReadStringTagAsync(name),
            10 => await ReadCompoundTagAsync(name),
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
}