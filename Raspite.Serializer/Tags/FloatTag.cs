namespace Raspite.Serializer.Tags;

public sealed record FloatTag(float Value, string Name = "") : Tag<float>
{
    public override byte Identifier => 5;
}