namespace Raspite.Serializer.Tags;

/// <inheritdoc cref="Tag{T}"/>
public sealed class IntegerTag : Tag<int>
{
    internal override byte Type => 3;
}