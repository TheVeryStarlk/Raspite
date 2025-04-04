namespace Raspite.Serializer.Tags;

public sealed record DoubleTag(double Value, string Name = "") : Tag<double>
{
    public override byte Identifier => 6;
}