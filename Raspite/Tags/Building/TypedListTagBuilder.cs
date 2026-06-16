namespace Raspite.Tags.Building;

public sealed class ListTagBuilder<T> where T : Tag
{
    private readonly List<T> tags = [];
    private readonly string name;

    private ListTagBuilder(string name)
    {
        this.name = name;
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
        return new ListTag([.. tags], name);
    }
}

public static class ListTagBuilderExtensions
{
    public static ListTagBuilder<StringTag> AddString(this ListTagBuilder<StringTag> builder, string value)
    {
        builder.Add(new StringTag(value));
        return builder;
    }

    public static ListTagBuilder<ByteTag> AddByte(this ListTagBuilder<ByteTag> builder, byte value)
    {
        builder.Add(new ByteTag(value));
        return builder;
    }

    public static ListTagBuilder<ShortTag> AddShort(this ListTagBuilder<ShortTag> builder, short value)
    {
        builder.Add(new ShortTag(value));
        return builder;
    }

    public static ListTagBuilder<IntegerTag> AddInt(this ListTagBuilder<IntegerTag> builder, int value)
    {
        builder.Add(new IntegerTag(value));
        return builder;
    }

    public static ListTagBuilder<LongTag> AddLong(this ListTagBuilder<LongTag> builder, long value)
    {
        builder.Add(new LongTag(value));
        return builder;
    }

    public static ListTagBuilder<FloatTag> AddFloat(this ListTagBuilder<FloatTag> builder, float value)
    {
        builder.Add(new FloatTag(value));
        return builder;
    }

    public static ListTagBuilder<DoubleTag> AddDouble(this ListTagBuilder<DoubleTag> builder, double value)
    {
        builder.Add(new DoubleTag(value));
        return builder;
    }

    public static ListTagBuilder<CompoundTag> AddCompound(this ListTagBuilder<CompoundTag> builder, CompoundTag value)
    {
        builder.Add(value);
        return builder;
    }

    public static ListTagBuilder<ListTag> AddList(this ListTagBuilder<ListTag> builder, ListTag value)
    {
        builder.Add(value);
        return builder;
    }
}