using System.Collections.Immutable;

namespace Raspite.Tags;

public sealed class IntegersTag(ImmutableArray<int> value, string name = "") : Tag<ImmutableArray<int>>(value, name)
{
    public override byte Identifier => Integers;

    public static IntegersTag Create(IEnumerable<int> ints, string name = "")
    {
        return new IntegersTag([.. ints], name);
    }
}