using System.Collections.Frozen;

namespace Raspite.Tags.Building;

public sealed class CompoundTagBuilder
{
    public ITag this[string name] => tags[name];

    private readonly Dictionary<string, ITag> tags;
    private readonly string parentName;

    internal CompoundTagBuilder(Dictionary<string, ITag> initialTags, string name)
    {
        tags = initialTags;
        parentName = name;
    }

    public static CompoundTagBuilder Create(string name = "")
    {
        return new CompoundTagBuilder([], name);
    }

    public CompoundTagBuilder Add(ITag tag, bool overwriteExisting = false)
    {
        if (overwriteExisting)
        {
            tags[tag.Name] = tag;
        }
        else
        {
            tags.Add(tag.Name, tag);
        }

        return this;
    }

    public CompoundTagBuilder Remove(string name)
    {
        tags.Remove(name);
        return this;
    }

    public CompoundTag Build()
    {
        return new CompoundTag([.. tags.Values], tags.ToFrozenDictionary(), parentName);
    }
}

public static class CompoundTagBuilderExtensions
{
    extension(CompoundTagBuilder builder)
    {
        public CompoundTagBuilder AddByte(byte? value, string name = "")
        {
            return value.HasValue
                ? builder.Add(new ByteTag(value.Value, name))
                : builder;
        }

        public CompoundTagBuilder AddBoolean(bool? value, string name = "")
        {
            return value.HasValue
                ? builder.Add(new ByteTag(value.Value, name))
                : builder;
        }

        public CompoundTagBuilder AddShort(short? value, string name = "")
        {
            return value.HasValue
                ? builder.Add(new ShortTag(value.Value, name))
                : builder;
        }

        public CompoundTagBuilder AddInteger(int? value, string name = "")
        {
            return value.HasValue
                ? builder.Add(new IntegerTag(value.Value, name))
                : builder;
        }

        public CompoundTagBuilder AddLong(long? value, string name = "")
        {
            return value.HasValue
                ? builder.Add(new LongTag(value.Value, name))
                : builder;
        }

        public CompoundTagBuilder AddFloat(float? value, string name = "")
        {
            return value.HasValue
                ? builder.Add(new FloatTag(value.Value, name))
                : builder;
        }

        public CompoundTagBuilder AddDouble(double? value, string name = "")
        {
            return value.HasValue
                ? builder.Add(new DoubleTag(value.Value, name))
                : builder;
        }

        public CompoundTagBuilder AddString(string? value, string name = "")
        {
            return value is not null
                ? builder.Add(new StringTag(value, name))
                : builder;
        }

        public CompoundTagBuilder AddList<TTag>(ListTag<TTag>? value, string name = "") where TTag : class, ITag
        {
            return value is not null
                ? builder.Add(value)
                : builder;
        }

        public CompoundTagBuilder AddCompound(CompoundTag? value, string name = "")
        {
            return value is not null
                ? builder.Add(value)
                : builder;
        }

        public CompoundTagBuilder AddBytes(byte[]? value, string name = "")
        {
            return value is not null
                ? builder.Add(new BytesTag([.. value], name))
                : builder;
        }

        public CompoundTagBuilder AddIntegers(int[]? value, string name = "")
        {
            return value is not null
                ? builder.Add(new IntegersTag([.. value], name))
                : builder;
        }

        public CompoundTagBuilder AddLongs(long[]? value, string name = "")
        {
            return value is not null
                ? builder.Add(new LongsTag([.. value], name))
                : builder;
        }
    }
}

public static class CompoundTagBuilderListExtensions
{
    extension(CompoundTagBuilder builder)
    {
        public CompoundTagBuilder AddByteList(ReadOnlySpan<byte?> values, string name = "")
        {
            return builder.Add(ListTagBuilder<ByteTag>.Create(name).AddByteRange(values).Build());
        }

        public CompoundTagBuilder AddByteList(ReadOnlySpan<byte> values, string name = "")
        {
            return builder.Add(ListTagBuilder<ByteTag>.Create(name).AddByteRange(values).Build());
        }

        public CompoundTagBuilder AddBooleanList(ReadOnlySpan<bool?> values, string name = "")
        {
            return builder.Add(ListTagBuilder<ByteTag>.Create(name).AddBooleanRange(values).Build());
        }

        public CompoundTagBuilder AddBooleanList(ReadOnlySpan<bool> values, string name = "")
        {
            return builder.Add(ListTagBuilder<ByteTag>.Create(name).AddBooleanRange(values).Build());
        }

        public CompoundTagBuilder AddShortList(ReadOnlySpan<short?> values, string name = "")
        {
            return builder.Add(ListTagBuilder<ShortTag>.Create(name).AddShortRange(values).Build());
        }

        public CompoundTagBuilder AddShortList(ReadOnlySpan<short> values, string name = "")
        {
            return builder.Add(ListTagBuilder<ShortTag>.Create(name).AddShortRange(values).Build());
        }

        public CompoundTagBuilder AddIntegerList(ReadOnlySpan<int?> values, string name = "")
        {
            return builder.Add(ListTagBuilder<IntegerTag>.Create(name).AddIntegerRange(values).Build());
        }

        public CompoundTagBuilder AddIntegerList(ReadOnlySpan<int> values, string name = "")
        {
            return builder.Add(ListTagBuilder<IntegerTag>.Create(name).AddIntegerRange(values).Build());
        }

        public CompoundTagBuilder AddLongList(ReadOnlySpan<long?> values, string name = "")
        {
            return builder.Add(ListTagBuilder<LongTag>.Create(name).AddLongRange(values).Build());
        }

        public CompoundTagBuilder AddLongList(ReadOnlySpan<long> values, string name = "")
        {
            return builder.Add(ListTagBuilder<LongTag>.Create(name).AddLongRange(values).Build());
        }

        public CompoundTagBuilder AddFloatList(ReadOnlySpan<float?> values, string name = "")
        {
            return builder.Add(ListTagBuilder<FloatTag>.Create(name).AddFloatRange(values).Build());
        }

        public CompoundTagBuilder AddFloatList(ReadOnlySpan<float> values, string name = "")
        {
            return builder.Add(ListTagBuilder<FloatTag>.Create(name).AddFloatRange(values).Build());
        }

        public CompoundTagBuilder AddDoubleList(ReadOnlySpan<double> values, string name = "")
        {
            return builder.Add(ListTagBuilder<DoubleTag>.Create(name).AddDoubleRange(values).Build());
        }

        public CompoundTagBuilder AddDoubleList(ReadOnlySpan<double?> values, string name = "")
        {
            return builder.Add(ListTagBuilder<DoubleTag>.Create(name).AddDoubleRange(values).Build());
        }

        public CompoundTagBuilder AddStringList(ReadOnlySpan<string?> values, string name = "")
        {
            return builder.Add(ListTagBuilder<StringTag>.Create(name).AddStringRange(values).Build());
        }

        public CompoundTagBuilder AddCompoundList(ReadOnlySpan<CompoundTag?> values, string name = "")
        {
            return builder.Add(ListTagBuilder<CompoundTag>.Create(name).AddCompoundRange(values).Build());
        }

        public CompoundTagBuilder AddListList<TTag>(ReadOnlySpan<ListTag<TTag>?> values, string name = "") where TTag : class, ITag
        {
            return builder.Add(ListTagBuilder<ListTag<TTag>>.Create(name).AddListRange(values).Build());
        }

        public CompoundTagBuilder AddBytesList(ReadOnlySpan<byte[]?> values, string name = "")
        {
            return builder.Add(ListTagBuilder<BytesTag>.Create(name).AddBytesRange(values).Build());
        }

        public CompoundTagBuilder AddIntegersList(ReadOnlySpan<int[]?> values, string name = "")
        {
            return builder.Add(ListTagBuilder<IntegersTag>.Create(name).AddIntegersRange(values).Build());
        }

        public CompoundTagBuilder AddLongsList(ReadOnlySpan<long[]?> values, string name = "")
        {
            return builder.Add(ListTagBuilder<LongsTag>.Create(name).AddLongsRange(values).Build());
        }
    }
}