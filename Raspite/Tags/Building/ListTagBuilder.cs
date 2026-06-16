namespace Raspite.Tags.Building;

public sealed class ListTagBuilder
{
    private readonly string name = "";

    private ListTagBuilder(string name)
    {
        this.name = name;
    }

    public static ListTagBuilder Create(string name = "")
    {
        return new(name);
    }

    public TypedListTagBuilder<StringTag> AddString(string value)
    {
        return new TypedListTagBuilder<StringTag>(name).AddString(value);
    }

    public TypedListTagBuilder<ByteTag> AddByte(byte value)
    {
        return new TypedListTagBuilder<ByteTag>(name).AddByte(value);
    }

    public TypedListTagBuilder<ShortTag> AddShort(short value)
    {
        return new TypedListTagBuilder<ShortTag>(name).AddShort(value);
    }

    public TypedListTagBuilder<IntegerTag> AddInt(int value)
    {
        return new TypedListTagBuilder<IntegerTag>(name).AddInt(value);
    }

    public TypedListTagBuilder<LongTag> AddLong(long value)
    {
        return new TypedListTagBuilder<LongTag>(name).AddLong(value);
    }

    public TypedListTagBuilder<FloatTag> AddFloat(float value)
    {
        return new TypedListTagBuilder<FloatTag>(name).AddFloat(value);
    }

    public TypedListTagBuilder<DoubleTag> AddDouble(double value)
    {
        return new TypedListTagBuilder<DoubleTag>(name).AddDouble(value);
    }

    public TypedListTagBuilder<CompoundTag> AddCompound(CompoundTag value)
    {
        return new TypedListTagBuilder<CompoundTag>(name).AddCompound(value);
    }

    public TypedListTagBuilder<ListTag> AddList(ListTag value)
    {
        return new TypedListTagBuilder<ListTag>(name).AddList(value);
    }
}