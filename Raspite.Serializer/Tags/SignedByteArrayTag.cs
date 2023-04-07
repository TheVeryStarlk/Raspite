namespace Raspite.Serializer.Tags;

public sealed class SignedByteArrayTag : Tag<sbyte[]>
{
    internal override byte Type => 7;
}