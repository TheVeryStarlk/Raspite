using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Raspite.Tags;

public sealed class CompoundTag : Tag<Tag[]>
{
    public override byte Identifier => Compound;

    public Tag this[string name] => cache[name];

    public int Count => cache.Count;

    public ImmutableArray<string> Keys => cache.Keys;

    private readonly FrozenDictionary<string, Tag> cache;

    public CompoundTag(Tag[] value, string name = "") : base(value, name)
    {
        cache = value.ToFrozenDictionary(tag => tag.Name);
    }

    public TTag Get<TTag>(string name) where TTag : Tag
    {
        return (TTag) cache[name];
    }

    public T GetValue<T>(string name)
    {
        return ((Tag<T>) cache[name]).Value;
    }

    public TTag? GetOrDefault<TTag>(string name) where TTag : Tag
    {
        return cache.GetValueOrDefault(name) as TTag;
    }

    public bool TryGet<TTag>(string name, [NotNullWhen(true)] out TTag? tag) where TTag : Tag
    {
        if (cache.TryGetValue(name, out var rawTag) && rawTag is TTag typedTag)
        {
            tag = typedTag;
            return true;
        }

        tag = null;
        return false;
    }

    public bool Contains(string name)
    {
        return cache.ContainsKey(name);
    }
}