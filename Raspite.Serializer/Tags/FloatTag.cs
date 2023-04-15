namespace Raspite.Serializer.Tags;

/// <inheritdoc cref="Tag{T}"/>
public sealed class FloatTag : Tag<float>
{
    internal override byte Type => 5;
}