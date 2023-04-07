namespace Raspite.Serializer.Tags;

public sealed class ShortTag : Tag<short>
{
    internal override byte Type => 2;
}