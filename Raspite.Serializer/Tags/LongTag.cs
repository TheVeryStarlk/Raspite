namespace Raspite.Serializer.Tags;

/// <inheritdoc cref="Tag{T}"/>
public sealed class LongTag : Tag<long>
{
    internal override byte Type => 4;
}