namespace Raspite.Tags;

public sealed class IntegerCollectionTag(int[] value, string name = "") : Tag<int[]>(value, name)
{
    public override byte Identifier => 11;
}