using System.Text;
using Raspite.Serializer.Tags;

namespace Raspite.Serializer;

public sealed class BinaryTagReaderException : Exception
{
    public BinaryTagReaderException(string message) : base(message)
    {
    }
}

internal sealed class BinaryTagReader
{
    private readonly BinaryStream stream;

    public BinaryTagReader(BinaryStream stream)
    {
        this.stream = stream;
    }

    public async Task<Tag> EvaluateAsync(byte? type = null)
    {
        type ??= stream.ReadByte();

        var size = await stream.ReadShortAsync();
        var name = Encoding.UTF8.GetString(await stream.ReadBytesAsync(size));

        Tag result = type switch
        {
            1 => ReadSignedByteTag(name),
            10 => await ReadCompoundTagAsync(name),
            _ => throw new BinaryTagReaderException("Unknown tag type.")
        };

        return result;
    }

    private SignedByteTag ReadSignedByteTag(string name)
    {
        var payload = stream.ReadSignedByte();

        return new SignedByteTag()
        {
            Name = name,
            Value = payload
        };
    }

    private async Task<CompoundTag> ReadCompoundTagAsync(string name)
    {
        var children = new List<Tag>();

        while (stream.CanRead)
        {
            var current = stream.ReadByte();

            if (current == 0)
            {
                return new CompoundTag()
                {
                    Name = name,
                    Children = children.ToArray()
                };
            }

            children.Add(await EvaluateAsync(current));
        }

        throw new BinaryTagReaderException("Compound tag did not end with an ending tag.");
    }
}