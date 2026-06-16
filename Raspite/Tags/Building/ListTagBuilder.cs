namespace Raspite.Tags.Building;

public sealed class ListTagBuilder<T> where T : Tag
{
    private readonly List<T> tags = [];
    private readonly string parentName;

    private ListTagBuilder(string name)
    {
        parentName = name;
    }

    public static ListTagBuilder<T> Create(string name = "")
    {
        return new ListTagBuilder<T>(name);
    }

    internal void Add(T tag)
    {
        tags.Add(tag);
    }

    public ListTag Build()
    {
        return new ListTag([.. tags], parentName);
    }
}

public static class ListTagBuilderExtensions
{
    public static ListTagBuilder<ByteTag> AddByte(this ListTagBuilder<ByteTag> builder, byte? value)
    {
        if (value.HasValue)
        {
            builder.Add(new ByteTag(value.Value));
        }

        return builder;
    }

    public static ListTagBuilder<ByteTag> AddBoolean(this ListTagBuilder<ByteTag> builder, bool? value)
    {
        if (value.HasValue)
        {
            builder.Add(new ByteTag(value.Value));
        }

        return builder;
    }

    public static ListTagBuilder<ShortTag> AddShort(this ListTagBuilder<ShortTag> builder, short? value)
    {
        if (value.HasValue)
        {
            builder.Add(new ShortTag(value.Value));
        }

        return builder;
    }

    public static ListTagBuilder<IntegerTag> AddInteger(this ListTagBuilder<IntegerTag> builder, int? value)
    {
        if (value.HasValue)
        {
            builder.Add(new IntegerTag(value.Value));
        }

        return builder;
    }

    public static ListTagBuilder<LongTag> AddLong(this ListTagBuilder<LongTag> builder, long? value)
    {
        if (value.HasValue)
        {
            builder.Add(new LongTag(value.Value));
        }

        return builder;
    }

    public static ListTagBuilder<FloatTag> AddFloat(this ListTagBuilder<FloatTag> builder, float? value)
    {
        if (value.HasValue)
        {
            builder.Add(new FloatTag(value.Value));
        }

        return builder;
    }

    public static ListTagBuilder<DoubleTag> AddDouble(this ListTagBuilder<DoubleTag> builder, double? value)
    {
        if (value.HasValue)
        {
            builder.Add(new DoubleTag(value.Value));
        }

        return builder;
    }

    public static ListTagBuilder<StringTag> AddString(this ListTagBuilder<StringTag> builder, string? value)
    {
        if (value is not null)
        {
            builder.Add(new StringTag(value));
        }

        return builder;
    }

    public static ListTagBuilder<CompoundTag> AddCompound(this ListTagBuilder<CompoundTag> builder, CompoundTag? value)
    {
        if (value is not null)
        {
            builder.Add(value);
        }

        return builder;
    }

    public static ListTagBuilder<ListTag> AddList(this ListTagBuilder<ListTag> builder, ListTag? value)
    {
        if (value is not null)
        {
            builder.Add(value);
        }

        return builder;
    }

    public static ListTagBuilder<BytesTag> AddBytes(this ListTagBuilder<BytesTag> builder, byte[]? value)
    {
        if (value is not null)
        {
            builder.Add(new BytesTag([.. value]));
        }

        return builder;
    }

    public static ListTagBuilder<IntegersTag> AddIntegers(this ListTagBuilder<IntegersTag> builder, int[]? value)
    {
        if (value is not null)
        {
            builder.Add(new IntegersTag([.. value]));
        }

        return builder;
    }

    public static ListTagBuilder<LongsTag> AddLongs(this ListTagBuilder<LongsTag> builder, long[]? value)
    {
        if (value is not null)
        {
            builder.Add(new LongsTag([.. value]));
        }

        return builder;
    }
}