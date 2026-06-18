namespace Raspite.Tags.Building;

public sealed class CompoundTagBuilder
{
    private readonly List<Tag> tags = [];
    private readonly string parentName;

    private CompoundTagBuilder(string tag)
    {
        parentName = tag;
    }

    public static CompoundTagBuilder Create(string name = "")
    {
        return new CompoundTagBuilder(name);
    }

    public CompoundTagBuilder AddByte(byte? value, string name = "")
    {
        if (value.HasValue)
        {
            tags.Add(new ByteTag(value.Value, name));
        }

        return this;
    }

    public CompoundTagBuilder AddBoolean(bool? value, string name = "")
    {
        if (value.HasValue)
        {
            tags.Add(new ByteTag(value.Value, name));
        }

        return this;
    }

    public CompoundTagBuilder AddShort(short? value, string name = "")
    {
        if (value.HasValue)
        {
            tags.Add(new ShortTag(value.Value, name));
        }

        return this;
    }

    public CompoundTagBuilder AddInteger(int? value, string name = "")
    {
        if (value.HasValue)
        {
            tags.Add(new IntegerTag(value.Value, name));
        }

        return this;
    }

    public CompoundTagBuilder AddLong(long? value, string name = "")
    {
        if (value.HasValue)
        {
            tags.Add(new LongTag(value.Value, name));
        }

        return this;
    }

    public CompoundTagBuilder AddFloat(float? value, string name = "")
    {
        if (value.HasValue)
        {
            tags.Add(new FloatTag(value.Value, name));
        }

        return this;
    }

    public CompoundTagBuilder AddDouble(double? value, string name = "")
    {
        if (value.HasValue)
        {
            tags.Add(new DoubleTag(value.Value, name));
        }

        return this;
    }

    public CompoundTagBuilder AddString(string? value, string name = "")
    {
        if (value is not null)
        {
            tags.Add(new StringTag(value, name));
        }

        return this;
    }

    public CompoundTagBuilder AddList<TTag>(ListTag<TTag>? value, string name = "") where TTag : Tag
    {
        if (value is not null)
        {
            tags.Add(value);
        }

        return this;
    }

    public CompoundTagBuilder AddCompound(CompoundTag? value, string name = "")
    {
        if (value is not null)
        {
            tags.Add(value);
        }

        return this;
    }

    public CompoundTagBuilder AddBytes(byte[]? value, string name = "")
    {
        if (value is not null)
        {
            tags.Add(new BytesTag([.. value], name));
        }

        return this;
    }

    public CompoundTagBuilder AddIntegers(int[]? value, string name = "")
    {
        if (value is not null)
        {
            tags.Add(new IntegersTag([.. value], name));
        }

        return this;
    }

    public CompoundTagBuilder AddLongs(long[]? value, string name = "")
    {
        if (value is not null)
        {
            tags.Add(new LongsTag([.. value], name));
        }

        return this;
    }

    public CompoundTag Build()
    {
        return new CompoundTag([.. tags], parentName);
    }
}