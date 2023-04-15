namespace Raspite.Serializer.Tags;

/// <inheritdoc cref="Tag{T}"/>
public sealed class SignedByteTag : Tag<sbyte>
{
    internal override byte Type => 1;
}