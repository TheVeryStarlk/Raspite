using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Raspite.Tags;

public sealed class CompoundTag : Tag<Tag[]>
{
    public override byte Identifier => Compound;
    
    private readonly FrozenDictionary<string, Tag> cache;

    public CompoundTag(Tag[] value, string name = "") : base(value, name)
    {
        cache = value.ToFrozenDictionary(tag => tag.Name);
    }

    public Tag this[string name] => cache[name];

    public T Get<T>(string name) where T : Tag => (T) cache[name];

    public T GetValue<T>(string name) => ((Tag<T>) cache[name]).Value;

    public T? GetOrDefault<T>(string name) where T : Tag => cache.GetValueOrDefault(name) as T;

    public bool TryGet<T>(string name, [NotNullWhen(true)] out T? tag) where T : Tag 
    {
        if (cache.TryGetValue(name, out var rawTag) && rawTag is T typedTag)
        {
            tag = typedTag;
            return true;
        }

        tag = null;
        return false;
    }
    
    public bool Contains(string name) => cache.ContainsKey(name);

    public int Count => cache.Count;

    public ImmutableArray<string> Keys => cache.Keys;
}