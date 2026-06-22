namespace Raspite.Tags.Building;

public sealed class CompoundTagBuilder
{
    private readonly List<ITag> tags;
    private readonly string parentName;

    internal CompoundTagBuilder(List<ITag> initialTags, string name)
    {
        tags = initialTags;
        parentName = name;
    }

    public static CompoundTagBuilder Create(string name = "")
    {
        return new CompoundTagBuilder([], name);
    }

    internal void Add(ITag tag)
    {
        tags.Add(tag);
    }

    public CompoundTagBuilder Remove(string name)
    {
        tags.RemoveAll(tag => tag.Name == name);
        return this;
    }

    public CompoundTagBuilder RemoveAll(Predicate<ITag> predicate)
    {
        tags.RemoveAll(predicate);
        return this;
    }

    public CompoundTag Build()
    {
        return new CompoundTag([.. tags], parentName);
    }
}

public static class CompoundTagBuilderExtensions
{
    extension(CompoundTagBuilder builder)
    {
        public CompoundTagBuilder AddByte(byte? value, string name = "")
        {
            if (value.HasValue)
            {
                builder.Add(new ByteTag(value.Value, name));
            }

            return builder;
        }

        public CompoundTagBuilder AddBoolean(bool? value, string name = "")
        {
            if (value.HasValue)
            {
                builder.Add(new ByteTag(value.Value, name));
            }

            return builder;
        }

        public CompoundTagBuilder AddShort(short? value, string name = "")
        {
            if (value.HasValue)
            {
                builder.Add(new ShortTag(value.Value, name));
            }

            return builder;
        }

        public CompoundTagBuilder AddInteger(int? value, string name = "")
        {
            if (value.HasValue)
            {
                builder.Add(new IntegerTag(value.Value, name));
            }

            return builder;
        }

        public CompoundTagBuilder AddLong(long? value, string name = "")
        {
            if (value.HasValue)
            {
                builder.Add(new LongTag(value.Value, name));
            }

            return builder;
        }

        public CompoundTagBuilder AddFloat(float? value, string name = "")
        {
            if (value.HasValue)
            {
                builder.Add(new FloatTag(value.Value, name));
            }

            return builder;
        }

        public CompoundTagBuilder AddDouble(double? value, string name = "")
        {
            if (value.HasValue)
            {
                builder.Add(new DoubleTag(value.Value, name));
            }

            return builder;
        }

        public CompoundTagBuilder AddString(string? value, string name = "")
        {
            if (value is not null)
            {
                builder.Add(new StringTag(value, name));
            }

            return builder;
        }

        public CompoundTagBuilder AddList<TTag>(ListTag<TTag>? value, string name = "") where TTag : ITag
        {
            if (value is not null)
            {
                builder.Add(value);
            }

            return builder;
        }

        public CompoundTagBuilder AddCompound(CompoundTag? value, string name = "")
        {
            if (value is not null)
            {
                builder.Add(value);
            }

            return builder;
        }

        public CompoundTagBuilder AddBytes(byte[]? value, string name = "")
        {
            if (value is not null)
            {
                builder.Add(new BytesTag([.. value], name));
            }

            return builder;
        }

        public CompoundTagBuilder AddIntegers(int[]? value, string name = "")
        {
            if (value is not null)
            {
                builder.Add(new IntegersTag([.. value], name));
            }

            return builder;
        }

        public CompoundTagBuilder AddLongs(long[]? value, string name = "")
        {
            if (value is not null)
            {
                builder.Add(new LongsTag([.. value], name));
            }

            return builder;
        }
    }
}

public static class CompoundTagBuilderListExtensions
{
    extension(CompoundTagBuilder builder)
    {
        public CompoundTagBuilder AddByteList(ReadOnlySpan<byte?> values, string name = "")
        {
            builder.Add(ListTagBuilder<ByteTag>.Create(name).AddByteRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddByteList(ReadOnlySpan<byte> values, string name = "")
        {
            builder.Add(ListTagBuilder<ByteTag>.Create(name).AddByteRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddBooleanList(ReadOnlySpan<bool?> values, string name = "")
        {
            builder.Add(ListTagBuilder<ByteTag>.Create(name).AddBooleanRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddBooleanList(ReadOnlySpan<bool> values, string name = "")
        {
            builder.Add(ListTagBuilder<ByteTag>.Create(name).AddBooleanRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddShortList(ReadOnlySpan<short?> values, string name = "")
        {
            builder.Add(ListTagBuilder<ShortTag>.Create(name).AddShortRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddShortList(ReadOnlySpan<short> values, string name = "")
        {
            builder.Add(ListTagBuilder<ShortTag>.Create(name).AddShortRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddIntegerList(ReadOnlySpan<int?> values, string name = "")
        {
            builder.Add(ListTagBuilder<IntegerTag>.Create(name).AddIntegerRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddIntegerList(ReadOnlySpan<int> values, string name = "")
        {
            builder.Add(ListTagBuilder<IntegerTag>.Create(name).AddIntegerRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddLongList(ReadOnlySpan<long?> values, string name = "")
        {
            builder.Add(ListTagBuilder<LongTag>.Create(name).AddLongRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddLongList(ReadOnlySpan<long> values, string name = "")
        {
            builder.Add(ListTagBuilder<LongTag>.Create(name).AddLongRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddFloatList(ReadOnlySpan<float?> values, string name = "")
        {
            builder.Add(ListTagBuilder<FloatTag>.Create(name).AddFloatRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddFloatList(ReadOnlySpan<float> values, string name = "")
        {
            builder.Add(ListTagBuilder<FloatTag>.Create(name).AddFloatRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddDoubleList(ReadOnlySpan<double> values, string name = "")
        {
            builder.Add(ListTagBuilder<DoubleTag>.Create(name).AddDoubleRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddDoubleList(ReadOnlySpan<double?> values, string name = "")
        {
            builder.Add(ListTagBuilder<DoubleTag>.Create(name).AddDoubleRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddStringList(ReadOnlySpan<string?> values, string name = "")
        {
            builder.Add(ListTagBuilder<StringTag>.Create(name).AddStringRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddCompoundList(ReadOnlySpan<CompoundTag?> values, string name = "")
        {
            builder.Add(ListTagBuilder<CompoundTag>.Create(name).AddCompoundRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddListList<TTag>(ReadOnlySpan<ListTag<TTag>?> values, string name = "") where TTag : ITag
        {
            builder.Add(ListTagBuilder<ListTag<TTag>>.Create(name).AddListRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddBytesList(ReadOnlySpan<byte[]?> values, string name = "")
        {
            builder.Add(ListTagBuilder<BytesTag>.Create(name).AddBytesRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddIntegersList(ReadOnlySpan<int[]?> values, string name = "")
        {
            builder.Add(ListTagBuilder<IntegersTag>.Create(name).AddIntegersRange(values).Build());
            return builder;
        }

        public CompoundTagBuilder AddLongsList(ReadOnlySpan<long[]?> values, string name = "")
        {
            builder.Add(ListTagBuilder<LongsTag>.Create(name).AddLongsRange(values).Build());
            return builder;
        }
    }
}