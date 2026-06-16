namespace Raspite.Tags.Building;

public sealed class TypedListTagBuilder<T> where T : Tag
{
    private readonly List<T> tags = [];
    private readonly string name = "";

    internal TypedListTagBuilder(string name)
    {
        this.name = name;
    }

    internal TypedListTagBuilder<T> Add(T tag)
    {
        tags.Add(tag);
        return this;
    }

    public ListTag Build()
    {
        return new ListTag([.. tags.Cast<Tag>()], name);
    }
}

public static class TypedListTagBuilderExtensions
{
    public static TypedListTagBuilder<StringTag> AddString(this TypedListTagBuilder<StringTag> builder, string value)
    {
        builder.Add(new StringTag(value));
        return builder;
    }

    public static TypedListTagBuilder<ByteTag> AddByte(this TypedListTagBuilder<ByteTag> builder, byte value)
    {
        builder.Add(new ByteTag(value));
        return builder;
    }

    public static TypedListTagBuilder<ShortTag> AddShort(this TypedListTagBuilder<ShortTag> builder, short value)
    {
        builder.Add(new ShortTag(value));
        return builder;
    }

    public static TypedListTagBuilder<IntegerTag> AddInt(this TypedListTagBuilder<IntegerTag> builder, int value)
    {
        builder.Add(new IntegerTag(value));
        return builder;
    }

    public static TypedListTagBuilder<LongTag> AddLong(this TypedListTagBuilder<LongTag> builder, long value)
    {
        builder.Add(new LongTag(value));
        return builder;
    }

    public static TypedListTagBuilder<FloatTag> AddFloat(this TypedListTagBuilder<FloatTag> builder, float value)
    {
        builder.Add(new FloatTag(value));
        return builder;
    }

    public static TypedListTagBuilder<DoubleTag> AddDouble(this TypedListTagBuilder<DoubleTag> builder, double value)
    {
        builder.Add(new DoubleTag(value));
        return builder;
    }

    public static TypedListTagBuilder<CompoundTag> AddCompound(this TypedListTagBuilder<CompoundTag> builder, CompoundTag value)
    {
        builder.Add(value);
        return builder;
    }

    public static TypedListTagBuilder<ListTag> AddList(this TypedListTagBuilder<ListTag> builder, ListTag value)
    {
        builder.Add(value);
        return builder;
    }
}