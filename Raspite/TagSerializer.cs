using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Raspite.Tags;

namespace Raspite;

/// <summary>
/// Represents a named binary <see cref="Tag"/> (NBT) serializer.
/// </summary>
public static class TagSerializer
{
    /// <summary>
    /// Attempts to parse the <see cref="ReadOnlySpan{T}"/> as a <see cref="TTag"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="ReadOnlySpan{T}"/> to parse.</param>
    /// <param name="tag">The parsed <see cref="Tag"/>.</param>
    /// <param name="options">The <see cref="TagSerializerOptions"/> to use.</param>
    /// <returns><c>true</c> if the <see cref="ReadOnlySpan{T}"/> was parsed successfully; otherwise, <c>false</c>.</returns>
    /// <typeparam name="TTag">The expected <see cref="TTag"/> type.</typeparam>
    /// <returns><c>true</c> if the <see cref="ReadOnlySpan{T}"/> was parsed successfully; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentException">The expected <see cref="TTag"/> was incorrect.</exception>
    public static bool TryParse<TTag>(ReadOnlySpan<byte> buffer, [NotNullWhen(true)] out TTag? tag, TagSerializerOptions? options = null) where TTag : Tag
    {
        if (!TryParse(buffer, out var rawTag, options))
        {
            tag = null;
            return false;
        }

        if (rawTag is not TTag typedTag)
        {
            throw new ArgumentException($"Expected {typeof(TTag).Name} but got {rawTag.GetType().Name}.");
        }

        tag = typedTag;
        return true;
    }

    /// <summary>
    /// Attempts to parse the <see cref="ReadOnlySpan{T}"/> as a <see cref="Tag"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="ReadOnlySpan{T}"/> to parse.</param>
    /// <param name="tag">The parsed <see cref="Tag"/>.</param>
    /// <param name="options">The <see cref="TagSerializerOptions"/> to use.</param>
    /// <returns><c>true</c> if the <see cref="ReadOnlySpan{T}"/> was parsed successfully; otherwise, <c>false</c>.</returns>
    public static bool TryParse(ReadOnlySpan<byte> buffer, out Tag tag, TagSerializerOptions? options = null)
    {
        options ??= new TagSerializerOptions();

        tag = EndTag.Instance;

        var reader = new TagReader(buffer, options.Value.LittleEndian, options.Value.Network);
        var success = reader.TryPeek(out var parent) && TryInstantiate(ref reader, out tag, parent, options.Value.MaximumDepth, options.Value.MaximumChildren);

        return success;

        static bool TryInstantiate(ref TagReader reader, out Tag tag, byte parent, int maximumDepth, int maximumChildren)
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
                    ArgumentOutOfRangeException.ThrowIfGreaterThan(length, maximumChildren);
                    maximumDepth--;

                    if (identifier is Tag.End || length < 1)
                    {
                        tag = new ListTag([], name);
                        return true;
                    }

                    var items = ArrayPool<Tag>.Shared.Rent(length);

                    try
                    {
                        for (var index = 0; index < length; index++)
                        {
                            reader.Nameless = true;

                            if (!TryInstantiate(ref reader, out var temporary, identifier, maximumDepth, maximumChildren))
                            {
                                return false;
                            }

                            items[index] = temporary;
                        }

                        tag = new ListTag([.. items.AsSpan(0, length)], name);
                    }
                    finally
                    {
                        ArrayPool<Tag>.Shared.Return(items, clearArray: true);
                    }

                    return true;
                }

                case Tag.Compound when reader.TryReadCompoundTag(out var name):
                {
                    maximumDepth--;

                    var items = ArrayPool<Tag>.Shared.Rent(maximumChildren);
                    var index = 0;

                    try
                    {
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

                            if (!TryInstantiate(ref reader, out var temporary, identifier, maximumDepth, maximumChildren))
                            {
                                return false;
                            }

                            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, maximumChildren);
                            items[index++] = temporary;
                        }

                        if (!reader.TryReadEndTag())
                        {
                            return false;
                        }

                        tag = new CompoundTag([.. items.AsSpan(0, index)], name);
                    }
                    finally
                    {
                        ArrayPool<Tag>.Shared.Return(items, clearArray: true);
                    }

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
    /// <param name="options">The <see cref="TagSerializerOptions"/> to use.</param>
    public static void Serialize(IBufferWriter<byte> buffer, Tag tag, TagSerializerOptions? options = null)
    {
        options ??= new TagSerializerOptions();

        var writer = new TagWriter(buffer, options.Value.LittleEndian, options.Value.Network);

        Write(ref writer, tag, options.Value.MaximumDepth, options.Value.MaximumChildren);

        return;

        static void Write(ref TagWriter writer, Tag tag, int maximumDepth, int maximumChildren)
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
                {
                    ArgumentOutOfRangeException.ThrowIfGreaterThan(current.Value.Length, maximumChildren);
                    maximumDepth--;

                    if (current.Value.Length < 1)
                    {
                        writer.WriteListTag(Tag.End, 0, current.Name);
                        return;
                    }

                    writer.WriteListTag(current.Value[0].Identifier, current.Value.Length, current.Name);

                    foreach (var item in current.Value)
                    {
                        writer.Nameless = true;
                        Write(ref writer, item, maximumDepth, maximumChildren);
                    }

                    break;
                }

                case CompoundTag current:
                {
                    ArgumentOutOfRangeException.ThrowIfGreaterThan(current.Value.Length, maximumChildren);
                    maximumDepth--;

                    writer.WriteCompoundTag(current.Name);

                    foreach (var item in current.Value)
                    {
                        writer.Nameless = false;
                        Write(ref writer, item, maximumDepth, maximumChildren);
                    }

                    writer.WriteEndTag();
                    break;
                }

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