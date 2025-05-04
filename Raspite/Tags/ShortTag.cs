namespace Raspite.Tags;

public sealed class ShortTag(short value, string name = "") : Tag<short>(value, name)
{
    public override byte Identifier => 2;
}