using System.Buffers;
using Raspite.Tags;

namespace Raspite;

/// <summary>
/// Represents a named binary <see cref="Tag"/> (NBT) serializer.
/// </summary>
public static class TagSerializer
{
    /// <summary>
    /// Attempts to parse the <see cref="ReadOnlySpan{T}"/> as a <see cref="Tag"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="ReadOnlySpan{T}"/> to parse.</param>
    /// <param name="tag">The parsed <see cref="Tag"/>.</param>
    /// <param name="littleEndian">Whether to parse as little-endian (<c>true</c>) or big-endian (<c>false</c>).</param>
    /// <param name="maximumDepth">The maximum allowed nest depth.</param>
    /// <returns><c>true</c> if the <see cref="ReadOnlySpan{T}"/> was parsed successfully; otherwise, <c>false</c>.</returns>
    public static bool TryParse(ReadOnlySpan<byte> buffer, out Tag tag, bool littleEndian = false, int maximumDepth = 512)
    {
        tag = EndTag.Instance;

        var reader = new TagReader(buffer, littleEndian);
        var success = reader.TryPeek(out var parent) && TryInstantiate(ref reader, out tag, parent, maximumDepth);

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

                    var items = new Tag[512];
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

    /// <summary>
    /// Serializes the <see cref="Tag"/> to a <see cref="IBufferWriter{T}"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="IBufferWriter{T}"/> to serialize to.</param>
    /// <param name="tag">The <see cref="Tag"/> to serialize.</param>
    /// <param name="littleEndian">Whether to serialize as little-endian (<c>true</c>) or big-endian (<c>false</c>).</param>
    /// <param name="maximumDepth">The maximum allowed nest depth.</param>
    public static void Serialize(IBufferWriter<byte> buffer, Tag tag, bool littleEndian = false, int maximumDepth = 512)
    {
        var writer = new TagWriter(buffer, littleEndian);
        Write(writer, tag, maximumDepth);

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
                    maximumDepth--;

                    if (current.Value.Length < 1)
                    {
                        writer.WriteListTag(Tag.End, 0, current.Name);
                        return;
                    }

                    writer.WriteListTag(current.Value[0].Identifier, current.Value.Length, current.Name);

                    foreach (var item in current.Value)
                    {
                        Write(writer, item, maximumDepth);
                    }

                    break;

                case CompoundTag current:
                    maximumDepth--;

                    writer.WriteCompoundTag(current.Name);

                    foreach (var item in current.Value)
                    {
                        Write(writer, item, maximumDepth);
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