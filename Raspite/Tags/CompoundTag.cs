using System.Collections.Frozen;
using System.Collections.Immutable;

using Raspite.Tags.Building;

namespace Raspite.Tags;

public sealed class CompoundTag : Tag<ImmutableArray<ITag>>
{
    public override byte Identifier => Compound;

    public ITag this[string name] => cache[name];

    public int Length => Value.Length;

    public ImmutableArray<string> Keys => cache.Keys;

    private readonly FrozenDictionary<string, ITag> cache;

    public CompoundTag(ImmutableArray<ITag> value, string name = "") : this(value, value.ToFrozenDictionary(tag => tag.Name), name)
    {
    }

    internal CompoundTag(ImmutableArray<ITag> value, FrozenDictionary<string, ITag> cache, string name = "") : base(value, name)
    {
        this.cache = cache;
    }

    public static CompoundTag Create(IEnumerable<ITag> tags, string name = "")
    {
        ImmutableArray<ITag> immutableTags = [.. tags];
        return new CompoundTag(immutableTags, immutableTags.ToFrozenDictionary(tag => tag.Name), name);
    }

    public bool ContainsKey(string name)
    {
        return cache.ContainsKey(name);
    }

    /// <summary>
    /// Converts this tag to a builder containing all the values this tag contained.
    /// It is the inverse operation of <code>CompoundTagBuilder.Build()</code>
    /// </summary>
    /// <param name="name">
    /// The name of the tag about to be built.
    /// Can be used for renaming it.
    /// <code>null</code> to leave the name as it was.
    /// </param>
    /// <returns></returns>
    public CompoundTagBuilder ToBuilder(string? name = null)
    {
        return new CompoundTagBuilder(cache.ToDictionary(), name ?? Name);
    }
}

public static class CompoundTagExtensions
{
    extension(CompoundTag compoundTag)
    {
        public byte? GetByte(string name)
        {
            return (compoundTag[name] as ByteTag)?.Value; 
        }

        public bool? GetBoolean(string name)
        {
            var value = (compoundTag[name] as ByteTag)?.Value;
            return value is null ? null : value != 0; 
        }

        public short? GetShort(string name)
        {
            return (compoundTag[name] as ShortTag)?.Value; 
        }

        public int? GetInteger(string name)
        {
            return (compoundTag[name] as IntegerTag)?.Value; 
        }

        public long? GetLong(string name)
        {
            return (compoundTag[name] as LongTag)?.Value; 
        }

        public float? GetFloat(string name)
        {
            return (compoundTag[name] as FloatTag)?.Value; 
        }

        public double? GetDouble(string name)
        {
            return (compoundTag[name] as DoubleTag)?.Value; 
        }

        public string? GetString(string name)
        {
            return (compoundTag[name] as StringTag)?.Value; 
        }

        public ListTag<TTag>? GetList<TTag>(string name) where TTag : class, ITag
        {
            var tag = compoundTag[name];

            return tag switch
            {
                ListTag<TTag> typedListTag => typedListTag,
                ListTag<EndTag> emptyListTag => new ListTag<TTag>([], emptyListTag.Name),
                _ => null,
            };
        }

        public CompoundTag? GetCompound(string name)
        {
            return compoundTag[name] as CompoundTag; 
        }

        public ImmutableArray<byte>? GetBytes(string name)
        {
            return (compoundTag[name] as BytesTag)?.Value; 
        }

        public ImmutableArray<int>? GetIntegers(string name)
        {
            return (compoundTag[name] as IntegersTag)?.Value; 
        }

        public ImmutableArray<long>? GetLongs(string name)
        {
            return (compoundTag[name] as LongsTag)?.Value; 
        }
    }
}