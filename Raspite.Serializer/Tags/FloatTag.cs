namespace Raspite.Serializer.Tags;

public sealed record FloatTag(float Value, string Name = "") : Tag<float>(Value, Name)
{
    public override byte Identifier => 5;
}