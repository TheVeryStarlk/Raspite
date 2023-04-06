namespace Raspite.Serializer.Tags;

public sealed class StringTag : Tag<string>
{
    internal override byte Type => 8;
}