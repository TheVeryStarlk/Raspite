namespace Raspite.Tags;

public sealed class FloatTag(float value, string name = "") : Tag<float>(value, name)
{
    public override byte Identifier => Float;
}