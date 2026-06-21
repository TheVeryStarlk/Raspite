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

    public CompoundTagBuilder AddList(ListTag? value, string name = "")
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

    public CompoundTagBuilder AddByteList(ReadOnlySpan<byte?> values, string name = "")
    {
        tags.Add(ListTagBuilder<ByteTag>.Create(name).AddByteRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddByteList(ReadOnlySpan<byte> values, string name = "")
    {
        tags.Add(ListTagBuilder<ByteTag>.Create(name).AddByteRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddBooleanList(ReadOnlySpan<bool?> values, string name = "")
    {
        tags.Add(ListTagBuilder<ByteTag>.Create(name).AddBooleanRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddBooleanList(ReadOnlySpan<bool> values, string name = "")
    {
        tags.Add(ListTagBuilder<ByteTag>.Create(name).AddBooleanRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddShortList(ReadOnlySpan<short?> values, string name = "")
    {
        tags.Add(ListTagBuilder<ShortTag>.Create(name).AddShortRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddShortList(ReadOnlySpan<short> values, string name = "")
    {
        tags.Add(ListTagBuilder<ShortTag>.Create(name).AddShortRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddIntegerList(ReadOnlySpan<int?> values, string name = "")
    {
        tags.Add(ListTagBuilder<IntegerTag>.Create(name).AddIntegerRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddIntegerList(ReadOnlySpan<int> values, string name = "")
    {
        tags.Add(ListTagBuilder<IntegerTag>.Create(name).AddIntegerRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddLongList(ReadOnlySpan<long?> values, string name = "")
    {
        tags.Add(ListTagBuilder<LongTag>.Create(name).AddLongRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddLongList(ReadOnlySpan<long> values, string name = "")
    {
        tags.Add(ListTagBuilder<LongTag>.Create(name).AddLongRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddFloatList(ReadOnlySpan<float?> values, string name = "")
    {
        tags.Add(ListTagBuilder<FloatTag>.Create(name).AddFloatRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddFloatList(ReadOnlySpan<float> values, string name = "")
    {
        tags.Add(ListTagBuilder<FloatTag>.Create(name).AddFloatRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddDoubleList(ReadOnlySpan<double> values, string name = "")
    {
        tags.Add(ListTagBuilder<DoubleTag>.Create(name).AddDoubleRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddDoubleList(ReadOnlySpan<double?> values, string name = "")
    {
        tags.Add(ListTagBuilder<DoubleTag>.Create(name).AddDoubleRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddStringList(ReadOnlySpan<string?> values, string name = "")
    {
        tags.Add(ListTagBuilder<StringTag>.Create(name).AddStringRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddCompoundList(ReadOnlySpan<CompoundTag?> values, string name = "")
    {
        tags.Add(ListTagBuilder<CompoundTag>.Create(name).AddCompoundRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddListList(ReadOnlySpan<ListTag?> values, string name = "")
    {
        tags.Add(ListTagBuilder<ListTag>.Create(name).AddListRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddBytesList(ReadOnlySpan<byte[]?> values, string name = "")
    {
        tags.Add(ListTagBuilder<BytesTag>.Create(name).AddBytesRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddIntegersList(ReadOnlySpan<int[]?> values, string name = "")
    {
        tags.Add(ListTagBuilder<IntegersTag>.Create(name).AddIntegersRange(values).Build());
        return this;
    }

    public CompoundTagBuilder AddLongsList(ReadOnlySpan<long[]?> values, string name = "")
    {
        tags.Add(ListTagBuilder<LongsTag>.Create(name).AddLongsRange(values).Build());
        return this;
    }

    public CompoundTag Build()
    {
        return new CompoundTag([.. tags], parentName);
    }
}