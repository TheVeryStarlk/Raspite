using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Raspite.Tags;

public sealed class CompoundTag : Tag<ImmutableArray<Tag>>
{
    public override byte Identifier => Compound;

    public Tag this[string name] => cache[name];

    public int Length => Value.Length;

    public ImmutableArray<string> Keys => cache.Keys;

    private readonly FrozenDictionary<string, Tag> cache;

    public CompoundTag(ImmutableArray<Tag> value, string name = "") : base(value, name)
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

    public T? GetValueOrDefault<T>(string name)
    {
        if (cache.GetValueOrDefault(name) is Tag<T> tag)
        {
            return tag.Value;
        }

        return default;
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

    public bool TryGetValue<T>(string name, [NotNullWhen(true)] out T? value)
    {
        if (cache.TryGetValue(name, out var rawTag) && rawTag is Tag<T> typedTag)
        {
            value = typedTag.Value;
            return value is not null;
        }

        value = default;
        return false;
    }

    public bool ContainsKey(string name)
    {
        return cache.ContainsKey(name);
    }
}