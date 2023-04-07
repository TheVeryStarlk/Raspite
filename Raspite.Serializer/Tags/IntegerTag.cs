namespace Raspite.Serializer.Tags;

public sealed class IntegerTag : Tag<int>
{
    internal override byte Type => 3;
}