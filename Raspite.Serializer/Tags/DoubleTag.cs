namespace Raspite.Serializer.Tags;

public sealed record DoubleTag(double Value, string Name = "") : Tag<double>(Value, Name)
{
    public override byte Identifier => 6;
}