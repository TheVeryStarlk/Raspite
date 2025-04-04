namespace Raspite.Serializer.Tags;

public sealed record IntegerTag(int Value, string Name = "") : Tag<int>
{
    public override byte Identifier => 3;
}