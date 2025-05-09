namespace Raspite.Tags;

public sealed class IntegerTag(int value, string name = "") : Tag<int>(value, name)
{
    public override byte Identifier => Integer;
}