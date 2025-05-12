using System.Buffers;
using Raspite.Tags;

namespace Raspite;

/// <summary>
/// Provides functionality to serialize or deserialize <see cref="Tag"/>s.
/// </summary>
public static class BinaryTagSerializer
{
    /// <summary>
    /// Serializes the provided <see cref="Tag"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="IBufferWriter{T}"/> to serialize to.</param>
    /// <param name="tag">The <see cref="Tag"/> to serialize.</param>
    /// <param name="littleEndian">Whether to serialize as little-endian (<c>true</c>) or big-endian (<c>false</c>).</param>
    /// <exception cref="ArgumentOutOfRangeException">Unknown <see cref="Tag"/>.</exception>
    public static void Serialize(IBufferWriter<byte> buffer, Tag tag, bool littleEndian)
    {
        var writer = new BinaryTagWriter(buffer, littleEndian);
        Write(writer, tag);

        return;

        static void Write(BinaryTagWriter writer, Tag tag)
        {
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
                    if (current.Value.Length is 0)
                    {
                        writer.WriteListTag(Tag.End, 0, current.Name);
                        return;
                    }

                    writer.WriteListTag(current.Value.First().Identifier, current.Value.Length, current.Name);

                    foreach (var item in current.Value)
                    {
                        Write(writer, item);
                    }

                    break;

                case CompoundTag current:
                    writer.WriteCompoundTag(current.Name);

                    foreach (var item in current.Value)
                    {
                        Write(writer, item);
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

    /// <summary>
    /// Deserializes the provided <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to deserialize.</param>
    /// <param name="littleEndian"></param>
    /// <typeparam name="T">Whether to deserialize as little-endian (<c>true</c>) or big-endian (<c>false</c>).</typeparam>
    /// <returns>
    /// The deserialized <see cref="Tag"/>.
    /// </returns>
    /// <exception cref="ArgumentException">Failed to start deserialize the tag.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Unknown <see cref="Tag"/>.</exception>
    public static T Deserialize<T>(ReadOnlySpan<byte> span, bool littleEndian) where T : Tag
    {
        var reader = new BinaryTagReader(span, littleEndian);

        if (!reader.TryPeek(out var identifier))
        {
            throw new ArgumentException("Failed to start deserialize the tag.");
        }

        return (T) Read(ref reader, identifier);

        static Tag Read(ref BinaryTagReader reader, byte current)
        {
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

                    var items = new Tag[length];

                    for (var index = 0; index < length; index++)
                    {
                        reader.Nameless = true;
                        items[index] = Read(ref reader, identifier);
                    }

                    return new ListTag(items, name);
                }

                case Tag.Compound when reader.TryReadCompoundTag(out var name):
                {
                    // Arbitrary length.
                    var items = new Tag[byte.MaxValue];
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

                        items[index++] = Read(ref reader, identifier);
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