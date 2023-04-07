namespace Raspite.Serializer.Tags;

public sealed class LongTag : Tag<long>
{
    internal override byte Type => 4;
}