namespace Raspite.Serializer.Tags;

public sealed class IntegerArrayTag : Tag<int[]>
{
    internal override byte Type => 11;
}