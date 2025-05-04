namespace Raspite.Tags;

public sealed class DoubleTag(double value, string name = "") : Tag<double>(value, name)
{
    public override byte Identifier => 6;
}