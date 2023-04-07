namespace Raspite.Serializer.Tags;

public sealed class ByteTag : Tag<byte>
{
    internal override byte Type => 1;
}