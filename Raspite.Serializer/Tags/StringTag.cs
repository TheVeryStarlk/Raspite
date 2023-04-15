namespace Raspite.Serializer.Tags;

/// <inheritdoc cref="Tag{T}"/>
public sealed class StringTag : Tag<string>
{
    internal override byte Type => 8;
}