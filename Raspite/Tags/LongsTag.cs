using System.Collections.Immutable;

namespace Raspite.Tags;

public sealed class LongsTag(ImmutableArray<long> value, string name = "") : Tag<ImmutableArray<long>>(value, name)
{
    public override byte Identifier => Longs;
}