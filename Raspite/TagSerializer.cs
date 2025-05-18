using System.Buffers;
using Raspite.Tags;

namespace Raspite;

public static class TagSerializer
{
    public static bool TryParse(ReadOnlySpan<byte> buffer, out Tag tag)
    {
        var options = new TagSerializerOptions();
        return TryParse(buffer, out tag, options);
    }

    public static void Serialize(IBufferWriter<byte> buffer, Tag tag)
    {
        var options = new TagSerializerOptions();
        Serialize(buffer, tag, options);
    }

    public static bool TryParse(ReadOnlySpan<byte> buffer, out Tag tag, TagSerializerOptions options)
    {
        tag = EndTag.Instance;

        var reader = new TagReader(buffer, options.LittleEndian);
        var success = reader.TryPeek(out var parent) && TryInstantiate(ref reader, out tag, parent, options.MaximumDepth);

        return success;

        static bool TryInstantiate(ref TagReader reader, out Tag tag, byte parent, int maximumDepth)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maximumDepth, 0);

            tag = EndTag.Instance;

            switch (parent)
            {
                case Tag.Byte when reader.TryReadByteTag(out var value, out var name):
                    tag = new ByteTag(value, name);
                    return true;

                case Tag.Short when reader.TryReadShortTag(out var value, out var name):
                    tag = new ShortTag(value, name);
                    return true;

                case Tag.Integer when reader.TryReadIntegerTag(out var value, out var name):
                    tag = new IntegerTag(value, name);
                    return true;

                case Tag.Long when reader.TryReadLongTag(out var value, out var name):
                    tag = new LongTag(value, name);
                    return true;

                case Tag.Float when reader.TryReadFloatTag(out var value, out var name):
                    tag = new FloatTag(value, name);
                    return true;

                case Tag.Double when reader.TryReadDoubleTag(out var value, out var name):
                    tag = new DoubleTag(value, name);
                    return true;

                case Tag.String when reader.TryReadStringTag(out var value, out var name):
                    tag = new StringTag(value, name);
                    return true;

                case Tag.List when reader.TryReadListTag(out var identifier, out var length, out var name):
                {
                    maximumDepth--;

                    if (identifier is Tag.End || length < 1)
                    {
                        tag = new ListTag([], name);
                        return true;
                    }

                    ArgumentOutOfRangeException.ThrowIfGreaterThan(length, maximumDepth);

                    var items = new Tag[length];

                    for (var index = 0; index < length; index++)
                    {
                        reader.Nameless = true;

                        if (!TryInstantiate(ref reader, out var temporary, identifier, maximumDepth))
                        {
                            return false;
                        }

                        items[index] = temporary;
                    }

                    tag = new ListTag(items, name);

                    return true;
                }

                case Tag.Compound when reader.TryReadCompoundTag(out var name):
                {
                    maximumDepth--;

                    var items = new Tag[maximumDepth];
                    var index = 0;

                    while (true)
                    {
                        if (!reader.TryPeek(out var identifier))
                        {
                            return false;
                        }

                        if (identifier is Tag.End)
                        {
                            break;
                        }

                        reader.Nameless = false;

                        if (!TryInstantiate(ref reader, out var temporary, identifier, maximumDepth))
                        {
                            return false;
                        }

                        items[index++] = temporary;
                    }

                    if (!reader.TryReadEndTag())
                    {
                        return false;
                    }

                    tag = new CompoundTag(items[..index], name);

                    return true;
                }

                case Tag.Bytes when reader.TryReadBytesTag(out var value, out var name):
                    tag = new BytesTag(value.ToArray(), name);
                    return true;

                case Tag.Integers when reader.TryReadIntegersTag(out var value, out var name):
                    tag = new IntegersTag(value.ToArray(), name);
                    return true;

                case Tag.Longs when reader.TryReadLongsTag(out var value, out var name):
                    tag = new LongsTag(value.ToArray(), name);
                    return true;

                default:
                    return false;
            }
        }
    }

    public static void Serialize(IBufferWriter<byte> buffer, Tag tag, TagSerializerOptions options)
    {
        var writer = new TagWriter(buffer, options.LittleEndian);
        Write(writer, tag, options.MaximumDepth);

        return;

        static void Write(TagWriter writer, Tag tag, int maximumDepth)
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

                case BytesTag current:
                    writer.WriteBytesTag(current.Value, current.Name);
                    break;

                case IntegersTag current:
                    writer.WriteIntegersTag(current.Value, current.Name);
                    break;

                case LongsTag current:
                    writer.WriteLongsTag(current.Value, current.Name);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(tag));
            }
        }
    }
}