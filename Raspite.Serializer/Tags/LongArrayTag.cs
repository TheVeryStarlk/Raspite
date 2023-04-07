namespace Raspite.Serializer.Tags;

public sealed class LongArrayTag : Tag<long[]>
{
    internal override byte Type => 12;
}