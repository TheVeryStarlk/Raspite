namespace Raspite.Serializer.Tags;

public sealed class SignedByteTag : Tag<sbyte>
{
    internal override byte Type => 1;
}