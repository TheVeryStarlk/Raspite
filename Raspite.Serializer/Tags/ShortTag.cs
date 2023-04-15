namespace Raspite.Serializer.Tags;

/// <inheritdoc cref="Tag{T}"/>
public sealed class ShortTag : Tag<short>
{
    internal override byte Type => 2;
}