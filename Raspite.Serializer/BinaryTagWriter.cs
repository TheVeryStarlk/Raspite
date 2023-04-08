using System.Text;
using Raspite.Serializer.Tags;

namespace Raspite.Serializer;

public sealed class BinaryTagWriterException : Exception
{
    public BinaryTagWriterException(string message) : base(message)
    {
    }
}

internal sealed class BinaryTagWriter
{
    private bool isNameless;

    private readonly BinaryStream stream;

    public BinaryTagWriter(BinaryStream stream)
    {
        this.stream = stream;
    }

    public async Task EvaluateAsync(Tag tag)
    {
        // Do not write the headers if we're inside a list.
        if (!isNameless)
        {
            stream.WriteByte(tag.Type);

            var buffer = Encoding.UTF8.GetBytes(tag.Name);

            await stream.WriteShortAsync((short) buffer.Length);
            await stream.WriteBytesAsync(buffer);
        }

        switch (tag)
        {
            case SignedByteTag signedByteTag:
                WriteSignedByteTag(signedByteTag);
                break;

            case ShortTag shortTag:
                await WriteShortTagAsync(shortTag);
                break;

            case IntegerTag integerTag:
                await WriteIntegerTagAsync(integerTag);
                break;

            case LongTag longTag:
                await WriteLongTagAsync(longTag);
                break;

            case FloatTag floatTag:
                await WriteFloatTagAsync(floatTag);
                break;

            case DoubleTag doubleTag:
                await WriteDoubleTagAsync(doubleTag);
                break;

            case SignedByteCollectionTag signedByteCollectionTag:
                await WriteSignedByteCollectionTagAsync(signedByteCollectionTag);
                break;

            case StringTag stringTag:
                await WriteStringTagAsync(stringTag);
                break;

            case CompoundTag compoundTag:
                await WriteCompoundTagAsync(compoundTag);
                break;

            case IntegerCollectionTag integerCollectionTag:
                await WriteIntegerCollectionTagAsync(integerCollectionTag);
                break;

            case LongCollectionTag longCollectionTag:
                await WriteLongCollectionTagAsync(longCollectionTag);
                break;

            default:
                // This is a walk-around for not being able to know the generic type of
                // a list tag in compile-time thus making it impossible* to add a case for it.
                if (tag.GetType().Name == typeof(ListTag<>).Name)
                {
                    var children = (Tag[]?) tag
                        .GetType()
                        .GetProperty("Children")?
                        .GetValue(tag);

                    await WriteListTagAsync(new CompoundTag()
                    {
                        Name = tag.Name,
                        Children = children ?? Array.Empty<Tag>()
                    });

                    break;
                }

                throw new BinaryTagWriterException("Unknown tag type.");
        }
    }

    private void WriteSignedByteTag(SignedByteTag tag)
    {
        stream.WriteSignedByte(tag.Value);
    }

    private async Task WriteShortTagAsync(ShortTag tag)
    {
        await stream.WriteShortAsync(tag.Value);
    }

    private async Task WriteIntegerTagAsync(IntegerTag tag)
    {
        await stream.WriteIntegerAsync(tag.Value);
    }

    private async Task WriteLongTagAsync(LongTag tag)
    {
        await stream.WriteLongAsync(tag.Value);
    }

    private async Task WriteFloatTagAsync(FloatTag tag)
    {
        await stream.WriteFloatAsync(tag.Value);
    }

    private async Task WriteDoubleTagAsync(DoubleTag tag)
    {
        await stream.WriteDoubleAsync(tag.Value);
    }

    private async Task WriteSignedByteCollectionTagAsync(SignedByteCollectionTag tag)
    {
        await stream.WriteIntegerAsync(tag.Children.Length);
        await stream.WriteSignedBytesAsync(tag.Children);
    }

    private async Task WriteStringTagAsync(StringTag tag)
    {
        var buffer = Encoding.UTF8.GetBytes(tag.Value);

        await stream.WriteUnsignedShortAsync((ushort) buffer.Length);
        await stream.WriteBytesAsync(buffer);
    }

    private async Task WriteListTagAsync(CollectionTag<Tag> tag)
    {
        var predefinedType = tag.Children.FirstOrDefault()?.Type ?? 0;

        stream.WriteByte(predefinedType);
        await stream.WriteIntegerAsync(tag.Children.Length);

        foreach (var child in tag.Children)
        {
            if (child.Type != predefinedType)
            {
                throw new BinaryTagWriterException("List tag cannot contain multiple tag types.");
            }

            isNameless = true;
            await EvaluateAsync(child);
        }

        isNameless = false;
    }

    private async Task WriteCompoundTagAsync(CompoundTag tag)
    {
        var wasNameless = isNameless;
        isNameless = false;

        foreach (var child in tag.Children)
        {
            await EvaluateAsync(child);
        }

        stream.WriteByte(0);
        isNameless = wasNameless;
    }

    private async Task WriteIntegerCollectionTagAsync(IntegerCollectionTag tag)
    {
        await stream.WriteIntegerAsync(tag.Children.Length);

        foreach (var child in tag.Children)
        {
            await stream.WriteIntegerAsync(child);
        }
    }

    private async Task WriteLongCollectionTagAsync(LongCollectionTag tag)
    {
        await stream.WriteIntegerAsync(tag.Children.Length);

        foreach (var child in tag.Children)
        {
            await stream.WriteLongAsync(child);
        }
    }
}