namespace Raspite.Tags.Building;

public sealed class CompoundTagBuilder
{
    private readonly List<Tag> tags = [];
    private readonly string tag;

    private CompoundTagBuilder(string tag)
    {
        this.tag = tag;
    }

    public static CompoundTagBuilder Create(string name)
    {
        return new CompoundTagBuilder(name);
    }

    public CompoundTagBuilder AddByte(byte value, string name = "")
    {
        tags.Add(new ByteTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddShort(short value, string name = "")
    {
        tags.Add(new ShortTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddInteger(int value, string name = "")
    {
        tags.Add(new IntegerTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddLong(long value, string name = "")
    {
        tags.Add(new LongTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddFloat(float value, string name = "")
    {
        tags.Add(new FloatTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddDouble(double value, string name = "")
    {
        tags.Add(new DoubleTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddString(string value, string name = "")
    {
        tags.Add(new StringTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddBytes(byte[] value, string name = "")
    {
        tags.Add(new BytesTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddIntegers(int[] value, string name = "")
    {
        tags.Add(new IntegersTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddLongs(long[] value, string name = "")
    {
        tags.Add(new LongsTag(value, name));
        return this;
    }

    public CompoundTagBuilder AddCompound(CompoundTag value, string name = "")
    {
        tags.Add(value);
        return this;
    }

    public CompoundTagBuilder AddList(ListTag value, string name = "")
    {
        tags.Add(value);
        return this;
    }

    public CompoundTag Build()
    {
        return new CompoundTag([.. tags], tag);
    }
}