using System.Buffers;
using Raspite.Tags;

namespace Raspite;

public static class BinaryTagSerializer
{
    public static void Serialize(IBufferWriter<byte> buffer, Tag tag, BinaryTagSerializerOptions? options = null)
    {
        options ??= new BinaryTagSerializerOptions();

        try
        {
            var writer = new BinaryTagWriter(buffer, options.LittleEndian);
            Write(writer, tag, options.MaximumDepth);
        }
        catch (ArgumentException exception)
        {
            // To make it easier to just catch one exception.
            throw new BinaryTagSerializerException(exception);
        }

        return;

        static void Write(BinaryTagWriter writer, Tag tag, int maximumDepth)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maximumDepth, 0);

            switch (tag)
            {
                case ByteTag current:
                    writer.WriteByteTag(current.Value, current.Name);
                    break;

                case ShortTag current:
                    writer.WriteShortTag(current.Value, current.Name);
                    break;

                case IntegerTag current:
                    writer.WriteIntegerTag(current.Value, current.Name);
                    break;

                case LongTag current:
                    writer.WriteLongTag(current.Value, current.Name);
                    break;

                case FloatTag current:
                    writer.WriteFloatTag(current.Value, current.Name);
                    break;

                case DoubleTag current:
                    writer.WriteDoubleTag(current.Value, current.Name);
                    break;

                case StringTag current:
                    writer.WriteStringTag(current.Value, current.Name);
                    break;

                case ListTag current:
                    writer.WriteListTag(current.Value.First().Identifier, current.Value.Length, current.Name);

                    foreach (var item in current.Value)
                    {
                        Write(writer, item, maximumDepth--);
                    }

                    break;

                case CompoundTag current:
                    writer.WriteCompoundTag(current.Name);

                    foreach (var item in current.Value)
                    {
                        Write(writer, item, maximumDepth--);
                    }

                    writer.WriteEndTag();
                    break;

                case ByteCollectionTag current:
                    writer.WriteByteCollectionTag(current.Value, current.Name);
                    break;

                case IntegerCollectionTag current:
                    writer.WriteIntegerCollectionTag(current.Value, current.Name);
                    break;

                case LongCollectionTag current:
                    writer.WriteLongCollectionTag(current.Value, current.Name);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(tag));
            }
        }
    }

    public static T Deserialize<T>(ReadOnlySpan<byte> span, BinaryTagSerializerOptions? options = null) where T : Tag
    {
        options ??= new BinaryTagSerializerOptions();

        try
        {
            var reader = new BinaryTagReader(span, options.LittleEndian);

            if (!reader.TryPeek(out var identifier))
            {
                throw new ArgumentException("Failed to start deserialize the tag.");
            }

            return (T) Read(ref reader, identifier, options.MaximumDepth);
        }
        catch (ArgumentException exception)
        {
            // To make it easier to just catch one exception.
            throw new BinaryTagSerializerException(exception);
        }

        static Tag Read(ref BinaryTagReader reader, byte current, int maximumDepth)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maximumDepth, 0);

            switch (current)
            {
                case Tag.Byte when reader.TryReadByteTag(out var value, out var name):
                    return new ByteTag(value, name);

                case Tag.Short when reader.TryReadShortTag(out var value, out var name):
                    return new ShortTag(value, name);

                case Tag.Integer when reader.TryReadIntegerTag(out var value, out var name):
                    return new IntegerTag(value, name);

                case Tag.Long when reader.TryReadLongTag(out var value, out var name):
                    return new LongTag(value, name);

                case Tag.Float when reader.TryReadFloatTag(out var value, out var name):
                    return new FloatTag(value, name);

                case Tag.Double when reader.TryReadDoubleTag(out var value, out var name):
                    return new DoubleTag(value, name);

                case Tag.String when reader.TryReadStringTag(out var value, out var name):
                    return new StringTag(value, name);

                case Tag.List when reader.TryReadListTag(out var identifier, out var length, out var name):
                {
                    if (identifier is Tag.End || length < 1)
                    {
                        return new ListTag([], name);
                    }

                    ArgumentOutOfRangeException.ThrowIfGreaterThan(length, maximumDepth);

                    var items = new Tag[length];

                    for (var index = 0; index < length; index++)
                    {
                        reader.Nameless = true;
                        items[index] = Read(ref reader, identifier, maximumDepth--);
                    }

                    return new ListTag(items, name);
                }

                case Tag.Compound when reader.TryReadCompoundTag(out var name):
                {
                    var items = new Tag[maximumDepth];
                    var index = 0;

                    while (true)
                    {
                        if (!reader.TryPeek(out var identifier))
                        {
                            throw new ArgumentException("Failed to create a compound tag.");
                        }

                        if (identifier is Tag.End)
                        {
                            break;
                        }

                        reader.Nameless = false;
                        items[index++] = Read(ref reader, identifier, maximumDepth--);
                    }

                    if (!reader.TryReadEndTag())
                    {
                        throw new ArgumentException("Failed to close a compound tag.");
                    }

                    return new CompoundTag(items[..index], name);
                }

                case Tag.ByteCollection when reader.TryReadByteCollectionTag(out var value, out var name):
                    return new ByteCollectionTag(value.ToArray(), name);

                case Tag.IntegerCollection when reader.TryReadIntegerCollectionTag(out var value, out var name):
                    return new IntegerCollectionTag(value.ToArray(), name);

                case Tag.LongCollection when reader.TryReadLongCollectionTag(out var value, out var name):
                    return new LongCollectionTag(value.ToArray(), name);

                default:
                    throw new ArgumentOutOfRangeException(nameof(current));
            }
        }
    }
}

public sealed class BinaryTagSerializerOptions
{
    public int MaximumDepth { get; init; } = byte.MaxValue;

    public bool LittleEndian { get; init; } = false;
}