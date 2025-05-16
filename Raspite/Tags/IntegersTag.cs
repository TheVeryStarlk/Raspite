namespace Raspite.Tags;

public sealed class IntegersTag(int[] value, string name = "") : Tag<int[]>(value, name)
{
    public override byte Identifier => Integers;
}