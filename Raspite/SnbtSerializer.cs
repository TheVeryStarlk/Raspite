using System.Buffers;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Raspite.Tags;

namespace Raspite;

/// <summary>
/// Represents a stringified named binary <see cref="Tag"/> (SNBT) serializer.
/// </summary>
public static class SnbtSerializer
{
    private const int InitialCapacity = 16;

    /// <summary>
    /// Attempts to parse the <see cref="ReadOnlySpan{T}"/> as a <see cref="TTag"/>.
    /// </summary>
    /// <param name="source">The <see cref="ReadOnlySpan{T}"/> to parse.</param>
    /// <param name="tag">The parsed <see cref="TTag"/>.</param>
    /// <param name="options">The <see cref="SnbtSerializerOptions"/> to use.</param>
    /// <typeparam name="TTag">The expected <see cref="TTag"/> type.</typeparam>
    /// <returns><c>true</c> if the <see cref="ReadOnlySpan{T}"/> was parsed successfully; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentException">The expected <see cref="TTag"/> was incorrect.</exception>
    public static bool TryParse<TTag>(ReadOnlySpan<char> source, [NotNullWhen(true)] out TTag? tag, SnbtSerializerOptions? options = null) where TTag : class, ITag
    {
        if (!TryParse(source, out var rawTag, options))
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
    /// Attempts to parse the <see cref="ReadOnlySpan{T}"/> as a <see cref="ITag"/>.
    /// </summary>
    /// <param name="source">The <see cref="ReadOnlySpan{T}"/> to parse.</param>
    /// <param name="tag">The parsed <see cref="ITag"/>.</param>
    /// <param name="options">The <see cref="SnbtSerializerOptions"/> to use.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="ReadOnlySpan{T}"/> was parsed successfully; otherwise, <c>false</c>,
    /// including when the <see cref="SnbtSerializerOptions"/> limits are exceeded.
    /// </returns>
    public static bool TryParse(ReadOnlySpan<char> source, out ITag tag, SnbtSerializerOptions? options = null)
    {
        options ??= new SnbtSerializerOptions();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.Value.MaximumDepth);
        ArgumentOutOfRangeException.ThrowIfNegative(options.Value.MaximumChildren);

        var reader = new SnbtReader(source);

        if (!TryInstantiate(ref reader, string.Empty, options.Value.MaximumDepth, options.Value.MaximumChildren, out tag) || reader.TryPeek(out _))
        {
            tag = EndTag.Instance;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Serializes the <see cref="ITag"/> to a <see cref="IBufferWriter{T}"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="IBufferWriter{T}"/> to serialize to.</param>
    /// <param name="tag">The <see cref="ITag"/> to serialize.</param>
    /// <param name="options">The <see cref="SnbtSerializerOptions"/> to use.</param>
    /// <remarks>The root <see cref="ITag"/>'s name is not written, as SNBT has no representation for it.</remarks>
    public static void Serialize(IBufferWriter<char> buffer, ITag tag, SnbtSerializerOptions? options = null)
    {
        options ??= new SnbtSerializerOptions();

        ArgumentNullException.ThrowIfNull(options.Value.IndentString);

        var writer = new SnbtWriter(buffer, options.Value);

        Write(ref writer, tag, options.Value.MaximumDepth, options.Value.MaximumChildren);
    }

    /// <summary>
    /// Serializes the <see cref="ITag"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="tag">The <see cref="ITag"/> to serialize.</param>
    /// <param name="options">The <see cref="SnbtSerializerOptions"/> to use.</param>
    /// <returns>The serialized <see cref="string"/>.</returns>
    /// <remarks>The root <see cref="ITag"/>'s name is not written, as SNBT has no representation for it.</remarks>
    public static string Serialize(ITag tag, SnbtSerializerOptions? options = null)
    {
        using var buffer = PooledBufferWriter.Rent();

        Serialize(buffer, tag, options);

        return new string(buffer.WrittenSpan);
    }

    private static bool TryInstantiate(ref SnbtReader reader, string name, int maximumDepth, int maximumChildren, out ITag tag)
    {
        tag = EndTag.Instance;

        if (maximumDepth < 1 || !reader.TryPeek(out var first))
        {
            return false;
        }

        return first switch
        {
            '{' => TryReadCompound(ref reader, name, maximumDepth - 1, maximumChildren, out tag),
            '[' => TryReadList(ref reader, name, maximumDepth - 1, maximumChildren, out tag),
            _ => reader.TryReadPrimitiveTag(out tag, name)
        };
    }

    private static bool TryReadCompound(ref SnbtReader reader, string name, int maximumDepth, int maximumChildren, out ITag tag)
    {
        tag = EndTag.Instance;

        if (!reader.TryRead('{'))
        {
            return false;
        }

        var items = ArrayPool<ITag>.Shared.Rent(InitialCapacity);
        var count = 0;

        try
        {
            var done = reader.TryRead('}');

            while (!done)
            {
                if (count >= maximumChildren || !reader.TryReadName(out var key)
                                             || !TryInstantiate(ref reader, key, maximumDepth, maximumChildren, out var item))
                {
                    return false;
                }

                Add(ref items, ref count, item);

                if (!TryReadNext(ref reader, '}', out done))
                {
                    return false;
                }
            }

            try
            {
                tag = new CompoundTag([.. items.AsSpan(0, count)], name);
            }
            catch (ArgumentException)
            {
                // Duplicate names.
                return false;
            }
        }
        finally
        {
            Return(items, count);
        }

        return true;
    }

    private static bool TryReadList(ref SnbtReader reader, string name, int maximumDepth, int maximumChildren, out ITag tag)
    {
        tag = EndTag.Instance;

        if (!reader.TryReadListStart(out var identifier))
        {
            return false;
        }

        switch (identifier)
        {
            case Tag.Bytes when TryReadBytes(ref reader, out var value):
                tag = new BytesTag(value, name);
                return true;

            case Tag.Integers when TryReadIntegers(ref reader, out var value):
                tag = new IntegersTag(value, name);
                return true;

            case Tag.Longs when TryReadLongs(ref reader, out var value):
                tag = new LongsTag(value, name);
                return true;

            case Tag.List:
                break;

            default:
                return false;
        }

        var items = ArrayPool<ITag>.Shared.Rent(InitialCapacity);
        var count = 0;

        try
        {
            var done = reader.TryRead(']');

            while (!done)
            {
                if (count >= maximumChildren || !TryInstantiate(ref reader, string.Empty, maximumDepth, maximumChildren, out var item))
                {
                    return false;
                }

                // Lists are homogeneous.
                if (count > 0 && item.Identifier != items[0].Identifier)
                {
                    return false;
                }

                Add(ref items, ref count, item);

                if (!TryReadNext(ref reader, ']', out done))
                {
                    return false;
                }
            }

            var span = items.AsSpan(0, count);

            tag = span.IsEmpty ? new ListTag<EndTag>([], name) : span[0].Identifier switch
            {
                Tag.Byte => CreateList<ByteTag>(span, name),
                Tag.Short => CreateList<ShortTag>(span, name),
                Tag.Integer => CreateList<IntegerTag>(span, name),
                Tag.Long => CreateList<LongTag>(span, name),
                Tag.Float => CreateList<FloatTag>(span, name),
                Tag.Double => CreateList<DoubleTag>(span, name),
                Tag.Bytes => CreateList<BytesTag>(span, name),
                Tag.String => CreateList<StringTag>(span, name),
                Tag.List => CreateList<IListTag>(span, name),
                Tag.Compound => CreateList<CompoundTag>(span, name),
                Tag.Integers => CreateList<IntegersTag>(span, name),
                Tag.Longs => CreateList<LongsTag>(span, name),
                _ => throw new UnreachableException()
            };
        }
        finally
        {
            Return(items, count);
        }

        return true;
    }

    private static bool TryReadBytes(ref SnbtReader reader, out ImmutableArray<byte> value)
    {
        value = [];

        var items = ArrayPool<byte>.Shared.Rent(InitialCapacity);
        var count = 0;

        try
        {
            var done = reader.TryRead(']');

            while (!done)
            {
                if (!reader.TryReadByteArrayItem(out var item))
                {
                    return false;
                }

                Add(ref items, ref count, item);

                if (!TryReadNext(ref reader, ']', out done))
                {
                    return false;
                }
            }

            value = [.. items.AsSpan(0, count)];
        }
        finally
        {
            Return(items, count);
        }

        return true;
    }

    private static bool TryReadIntegers(ref SnbtReader reader, out ImmutableArray<int> value)
    {
        value = [];

        var items = ArrayPool<int>.Shared.Rent(InitialCapacity);
        var count = 0;

        try
        {
            var done = reader.TryRead(']');

            while (!done)
            {
                if (!reader.TryReadIntegerArrayItem(out var item))
                {
                    return false;
                }

                Add(ref items, ref count, item);

                if (!TryReadNext(ref reader, ']', out done))
                {
                    return false;
                }
            }

            value = [.. items.AsSpan(0, count)];
        }
        finally
        {
            Return(items, count);
        }

        return true;
    }

    private static bool TryReadLongs(ref SnbtReader reader, out ImmutableArray<long> value)
    {
        value = [];

        var items = ArrayPool<long>.Shared.Rent(InitialCapacity);
        var count = 0;

        try
        {
            var done = reader.TryRead(']');

            while (!done)
            {
                if (!reader.TryReadLongArrayItem(out var item))
                {
                    return false;
                }

                Add(ref items, ref count, item);

                if (!TryReadNext(ref reader, ']', out done))
                {
                    return false;
                }
            }

            value = [.. items.AsSpan(0, count)];
        }
        finally
        {
            Return(items, count);
        }

        return true;
    }

    /// <summary>
    /// Reads the separator after an item, allowing a trailing one before the closing character.
    /// </summary>
    private static bool TryReadNext(ref SnbtReader reader, char close, out bool done)
    {
        if (reader.TryRead(','))
        {
            done = reader.TryRead(close);
            return true;
        }

        done = true;
        return reader.TryRead(close);
    }

    private static ListTag<TTag> CreateList<TTag>(ReadOnlySpan<ITag> items, string name) where TTag : class, ITag
    {
        var array = new TTag[items.Length];

        for (var index = 0; index < items.Length; index++)
        {
            array[index] = (TTag) items[index];
        }

        return new ListTag<TTag>(ImmutableCollectionsMarshal.AsImmutableArray(array), name);
    }

    private static void Add<T>(ref T[] items, ref int count, T item)
    {
        if (count == items.Length)
        {
            var larger = ArrayPool<T>.Shared.Rent(items.Length * 2);

            items.AsSpan(0, count).CopyTo(larger);
            Return(items, count);

            items = larger;
        }

        items[count++] = item;
    }

    private static void Return<T>(T[] items, int count)
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            items.AsSpan(0, count).Clear();
        }

        ArrayPool<T>.Shared.Return(items);
    }

    private static void Write(ref SnbtWriter writer, ITag tag, int maximumDepth, int maximumChildren)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumDepth);

        switch (tag)
        {
            case ByteTag current:
                writer.WriteByteTag(current.Value);
                break;

            case ShortTag current:
                writer.WriteShortTag(current.Value);
                break;

            case IntegerTag current:
                writer.WriteIntegerTag(current.Value);
                break;

            case LongTag current:
                writer.WriteLongTag(current.Value);
                break;

            case FloatTag current:
                writer.WriteFloatTag(current.Value);
                break;

            case DoubleTag current:
                writer.WriteDoubleTag(current.Value);
                break;

            case StringTag current:
                writer.WriteStringTag(current.Value);
                break;

            case IListTag current:
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(current.Length, maximumChildren);

                maximumDepth--;

                writer.WriteListStart();

                foreach (var item in current.RawTags)
                {
                    Write(ref writer, item, maximumDepth, maximumChildren);
                }

                writer.WriteListEnd();
                break;
            }

            case CompoundTag current:
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(current.Value.Length, maximumChildren);

                maximumDepth--;

                writer.WriteCompoundStart();

                foreach (var item in current.Value)
                {
                    writer.WriteName(item.Name);
                    Write(ref writer, item, maximumDepth, maximumChildren);
                }

                writer.WriteCompoundEnd();
                break;
            }

            case BytesTag current:
                writer.WriteBytesTag(current.Value.AsSpan());
                break;

            case IntegersTag current:
                writer.WriteIntegersTag(current.Value.AsSpan());
                break;

            case LongsTag current:
                writer.WriteLongsTag(current.Value.AsSpan());
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(tag));
        }
    }
}

/// <summary>
/// A per-thread reusable <see cref="IBufferWriter{T}"/> backed by <see cref="ArrayPool{T}"/>.
/// </summary>
file sealed class PooledBufferWriter : IBufferWriter<char>, IDisposable
{
    private const int MinimumLength = 256;

    [ThreadStatic]
    private static PooledBufferWriter? cached;

    public ReadOnlySpan<char> WrittenSpan => buffer.AsSpan(0, index);

    private char[] buffer = [];

    private int index;

    public static PooledBufferWriter Rent()
    {
        var writer = cached ?? new PooledBufferWriter();
        cached = null;

        return writer;
    }

    public void Advance(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, buffer.Length - index);

        index += count;
    }

    public Memory<char> GetMemory(int sizeHint = 0)
    {
        Ensure(sizeHint);
        return buffer.AsMemory(index);
    }

    public Span<char> GetSpan(int sizeHint = 0)
    {
        Ensure(sizeHint);
        return buffer.AsSpan(index);
    }

    public void Dispose()
    {
        if (buffer.Length > 0)
        {
            ArrayPool<char>.Shared.Return(buffer);
        }

        buffer = [];
        index = 0;
        cached = this;
    }

    private void Ensure(int sizeHint)
    {
        sizeHint = Math.Max(sizeHint, 1);

        if (buffer.Length - index >= sizeHint)
        {
            return;
        }

        var larger = ArrayPool<char>.Shared.Rent(Math.Max(MinimumLength, Math.Max(buffer.Length * 2, index + sizeHint)));

        buffer.AsSpan(0, index).CopyTo(larger);

        if (buffer.Length > 0)
        {
            ArrayPool<char>.Shared.Return(buffer);
        }

        buffer = larger;
    }
}