namespace Raspite.Serializer.Tags;

public sealed record IntegerTag(int Value, string Name = "") : Tag<int>(Value, Name)
{
    public override byte Identifier => 3;
}