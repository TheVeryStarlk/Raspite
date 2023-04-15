namespace Raspite.Serializer.Tags;

/// <inheritdoc cref="Tag{T}"/>
public sealed class DoubleTag : Tag<double>
{
    internal override byte Type => 6;
}