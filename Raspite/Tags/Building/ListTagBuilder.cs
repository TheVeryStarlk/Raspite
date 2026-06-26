namespace Raspite.Tags.Building;

public sealed class ListTagBuilder<TTag> where TTag : class, ITag
{
    private readonly List<TTag> tags;
    private readonly string parentName;

    internal ListTagBuilder(List<TTag> initalTags, string name)
    {
        tags = initalTags;
        parentName = name;
    }

    public static ListTagBuilder<TTag> Create(string name = "")
    {
        return new ListTagBuilder<TTag>([], name);
    }

    public ListTagBuilder<TTag> Add(TTag tag)
    {
        tags.Add(tag);
        return this;
    }

    public ListTagBuilder<TTag> RemoveAt(int index)
    {
        tags.RemoveAt(index);
        return this;
    }

    public ListTagBuilder<TTag> RemoveAll(Predicate<TTag> predicate)
    {
        tags.RemoveAll(predicate);
        return this;
    }

    public ListTag<TTag> Build()
    {
        return new ListTag<TTag>([.. tags], parentName);
    }
}

public static class ListTagBuilderExtensions
{
    public static ListTagBuilder<ByteTag> AddByte(this ListTagBuilder<ByteTag> builder, byte? value)
    {
        return value.HasValue
            ? builder.Add(new ByteTag(value.Value))
            : builder;
    }

    public static ListTagBuilder<ByteTag> AddBoolean(this ListTagBuilder<ByteTag> builder, bool? value)
    {
        return value.HasValue
            ? builder.Add(new ByteTag(value.Value))
            : builder;
    }

    public static ListTagBuilder<ShortTag> AddShort(this ListTagBuilder<ShortTag> builder, short? value)
    {
        return value.HasValue
            ? builder.Add(new ShortTag(value.Value))
            : builder;
    }

    public static ListTagBuilder<IntegerTag> AddInteger(this ListTagBuilder<IntegerTag> builder, int? value)
    {
        return value.HasValue
            ? builder.Add(new IntegerTag(value.Value))
            : builder;
    }

    public static ListTagBuilder<LongTag> AddLong(this ListTagBuilder<LongTag> builder, long? value)
    {
        return value.HasValue
            ? builder.Add(new LongTag(value.Value))
            : builder;
    }

    public static ListTagBuilder<FloatTag> AddFloat(this ListTagBuilder<FloatTag> builder, float? value)
    {
        return value.HasValue
            ? builder.Add(new FloatTag(value.Value))
            : builder;
    }

    public static ListTagBuilder<DoubleTag> AddDouble(this ListTagBuilder<DoubleTag> builder, double? value)
    {
        return value.HasValue
            ? builder.Add(new DoubleTag(value.Value))
            : builder;
    }

    public static ListTagBuilder<StringTag> AddString(this ListTagBuilder<StringTag> builder, string? value)
    {
        return value is not null
            ? builder.Add(new StringTag(value))
            : builder;
    }

    public static ListTagBuilder<CompoundTag> AddCompound(this ListTagBuilder<CompoundTag> builder, CompoundTag? value)
    {
        return value is not null
            ? builder.Add(value)
            : builder;
    }

    public static ListTagBuilder<ListTag<TTag>> AddList<TTag>(this ListTagBuilder<ListTag<TTag>> builder, ListTag<TTag>? value) where TTag : class, ITag
    {
        return value is not null
            ? builder.Add(value)
            : builder;
    }

    public static ListTagBuilder<BytesTag> AddBytes(this ListTagBuilder<BytesTag> builder, byte[]? value)
    {
        return value is not null
            ? builder.Add(new BytesTag([.. value]))
            : builder;
    }

    public static ListTagBuilder<IntegersTag> AddIntegers(this ListTagBuilder<IntegersTag> builder, int[]? value)
    {
        return value is not null
            ? builder.Add(new IntegersTag([.. value]))
            : builder;
    }

    public static ListTagBuilder<LongsTag> AddLongs(this ListTagBuilder<LongsTag> builder, long[]? value)
    {
        return value is not null
            ? builder.Add(new LongsTag([.. value]))
            : builder;
    }
}

public static class ListTagBuilderRangeExtensions
{
    public static ListTagBuilder<ByteTag> AddByteRange(this ListTagBuilder<ByteTag> builder, params ReadOnlySpan<byte?> range)
    {
        foreach (byte? value in range)
        {
            builder.AddByte(value);
        }

        return builder;
    }

    public static ListTagBuilder<ByteTag> AddByteRange(this ListTagBuilder<ByteTag> builder, params ReadOnlySpan<byte> range)
    {
        foreach (byte value in range)
        {
            builder.AddByte(value);
        }

        return builder;
    }

    public static ListTagBuilder<ByteTag> AddBooleanRange(this ListTagBuilder<ByteTag> builder, params ReadOnlySpan<bool?> range)
    {
        foreach (bool? value in range)
        {
            builder.AddBoolean(value);
        }

        return builder;
    }

    public static ListTagBuilder<ByteTag> AddBooleanRange(this ListTagBuilder<ByteTag> builder, params ReadOnlySpan<bool> range)
    {
        foreach (bool value in range)
        {
            builder.AddBoolean(value);
        }

        return builder;
    }

    public static ListTagBuilder<ShortTag> AddShortRange(this ListTagBuilder<ShortTag> builder, params ReadOnlySpan<short?> range)
    {
        foreach (short? value in range)
        {
            builder.AddShort(value);
        }

        return builder;
    }

    public static ListTagBuilder<ShortTag> AddShortRange(this ListTagBuilder<ShortTag> builder, params ReadOnlySpan<short> range)
    {
        foreach (short value in range)
        {
            builder.AddShort(value);
        }

        return builder;
    }

    public static ListTagBuilder<IntegerTag> AddIntegerRange(this ListTagBuilder<IntegerTag> builder, params ReadOnlySpan<int?> range)
    {
        foreach (int? value in range)
        {
            builder.AddInteger(value);
        }

        return builder;
    }

    public static ListTagBuilder<IntegerTag> AddIntegerRange(this ListTagBuilder<IntegerTag> builder, params ReadOnlySpan<int> range)
    {
        foreach (int value in range)
        {
            builder.AddInteger(value);
        }

        return builder;
    }

    public static ListTagBuilder<LongTag> AddLongRange(this ListTagBuilder<LongTag> builder, params ReadOnlySpan<long?> range)
    {
        foreach (long? value in range)
        {
            builder.AddLong(value);
        }

        return builder;
    }

    public static ListTagBuilder<LongTag> AddLongRange(this ListTagBuilder<LongTag> builder, params ReadOnlySpan<long> range)
    {
        foreach (long value in range)
        {
            builder.AddLong(value);
        }

        return builder;
    }

    public static ListTagBuilder<FloatTag> AddFloatRange(this ListTagBuilder<FloatTag> builder, params ReadOnlySpan<float?> range)
    {
        foreach (float? value in range)
        {
            builder.AddFloat(value);
        }

        return builder;
    }

    public static ListTagBuilder<FloatTag> AddFloatRange(this ListTagBuilder<FloatTag> builder, params ReadOnlySpan<float> range)
    {
        foreach (float value in range)
        {
            builder.AddFloat(value);
        }

        return builder;
    }

    public static ListTagBuilder<DoubleTag> AddDoubleRange(this ListTagBuilder<DoubleTag> builder, params ReadOnlySpan<double?> range)
    {
        foreach (double? value in range)
        {
            builder.AddDouble(value);
        }

        return builder;
    }

    public static ListTagBuilder<DoubleTag> AddDoubleRange(this ListTagBuilder<DoubleTag> builder, params ReadOnlySpan<double> range)
    {
        foreach (double value in range)
        {
            builder.AddDouble(value);
        }

        return builder;
    }

    public static ListTagBuilder<StringTag> AddStringRange(this ListTagBuilder<StringTag> builder, params ReadOnlySpan<string?> range)
    {
        foreach (string? value in range)
        {
            builder.AddString(value);
        }

        return builder;
    }

    public static ListTagBuilder<CompoundTag> AddCompoundRange(this ListTagBuilder<CompoundTag> builder, params ReadOnlySpan<CompoundTag?> range)
    {
        foreach (CompoundTag? value in range)
        {
            builder.AddCompound(value);
        }

        return builder;
    }

    public static ListTagBuilder<ListTag<TTag>> AddListRange<TTag>(this ListTagBuilder<ListTag<TTag>> builder, params ReadOnlySpan<ListTag<TTag>?> range) where TTag : class, ITag
    {
        foreach (ListTag<TTag>? value in range)
        {
            builder.AddList(value);
        }

        return builder;
    }

    public static ListTagBuilder<BytesTag> AddBytesRange(this ListTagBuilder<BytesTag> builder, params ReadOnlySpan<byte[]?> range)
    {
        foreach (byte[]? value in range)
        {
            builder.AddBytes(value);
        }

        return builder;
    }

    public static ListTagBuilder<IntegersTag> AddIntegersRange(this ListTagBuilder<IntegersTag> builder, params ReadOnlySpan<int[]?> range)
    {
        foreach (int[]? value in range)
        {
            builder.AddIntegers(value);
        }

        return builder;
    }

    public static ListTagBuilder<LongsTag> AddLongsRange(this ListTagBuilder<LongsTag> builder, params ReadOnlySpan<long[]?> range)
    {
        foreach (long[]? value in range)
        {
            builder.AddLongs(value);
        }

        return builder;
    }
}