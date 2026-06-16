namespace Raspite.Tags.Building;

public sealed class CompoundTagBuilder
{
    private readonly List<Tag> tags = [];
    private readonly string name = "";

    private CompoundTagBuilder(string name)
    {
        this.name = name;
    }

    public static CompoundTagBuilder Create(string name)
    {
        return new(name);
    }

    public CompoundTagBuilder AddByte(string name, byte value)
    {
        tags.Add(new ByteTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddShort(string name, short value)
    {
        tags.Add(new ShortTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddInt(string name, int value)
    {
        tags.Add(new IntegerTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddLong(string name, long value)
    {
        tags.Add(new LongTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddFloat(string name, float value)
    {
        tags.Add(new FloatTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddDouble(string name, double value)
    {
        tags.Add(new DoubleTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddString(string name, string value)
    {
        tags.Add(new StringTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddBytes(string name, byte[] value)
    {
        tags.Add(new BytesTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddIntegers(string name, int[] value)
    {
        tags.Add(new IntegersTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddLongs(string name, long[] value)
    {
        tags.Add(new LongsTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddCompound(string name, CompoundTag value)
    {
        tags.Add(value);
        return this;
    }

    public CompoundTagBuilder AddList(string name, ListTag value)
    {
        tags.Add(value);
        return this;
    }

    public CompoundTag Build()
    {
        return new CompoundTag([.. tags], name);
    }
}