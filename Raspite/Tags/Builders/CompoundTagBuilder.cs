namespace Raspite.Tags.Builders;

public sealed class CompoundTagBuilder
{
    private readonly string parent;

    // Arbitrary length.
    private readonly Tag[] children = new Tag[byte.MaxValue];

    private int index;

    private CompoundTagBuilder(string parent)
    {
        this.parent = parent;
    }

    public static CompoundTagBuilder Create(string parent = "")
    {
        return new CompoundTagBuilder(parent);
    }

    public CompoundTagBuilder AddByteTag(byte value, string name = "")
    {
        children[index++] = new ByteTag(value, name);
        return this;
    }

    public CompoundTagBuilder AddShortTag(short value, string name = "")
    {
        children[index++] = new ShortTag(value, name);
        return this;
    }

    public CompoundTagBuilder AddIntegerTag(int value, string name = "")
    {
        children[index++] = new IntegerTag(value, name);
        return this;
    }

    public CompoundTagBuilder AddLongTag(long value, string name = "")
    {
        children[index++] = new LongTag(value, name);
        return this;
    }

    public CompoundTagBuilder AddFloatTag(float value, string name = "")
    {
        children[index++] = new FloatTag(value, name);
        return this;
    }

    public CompoundTagBuilder AddDoubleTag(double value, string name = "")
    {
        children[index++] = new DoubleTag(value, name);
        return this;
    }

    public CompoundTagBuilder AddStringTag(string value, string name = "")
    {
        children[index++] = new StringTag(value, name);
        return this;
    }

    public CompoundTagBuilder AddListTag(ListTag value, string name = "")
    {
        children[index++] = value;
        return this;
    }

    public CompoundTagBuilder AddCompoundTag(CompoundTag value, string name = "")
    {
        children[index++] = value;
        return this;
    }

    public CompoundTagBuilder AddByteCollectionTag(byte[] value, string name = "")
    {
        children[index++] = new ByteCollectionTag(value, name);
        return this;
    }

    public CompoundTagBuilder AddIntegerCollectionTag(int[] value, string name = "")
    {
        children[index++] = new IntegerCollectionTag(value, name);
        return this;
    }

    public CompoundTagBuilder AddLongCollectionTag(long[] value, string name = "")
    {
        children[index++] = new LongCollectionTag(value, name);
        return this;
    }

    public CompoundTag Build()
    {
        return new CompoundTag(children[..index], parent);
    }
}