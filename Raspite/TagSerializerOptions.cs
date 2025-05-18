namespace Raspite;

public readonly struct TagSerializerOptions()
{
    public int MaximumDepth { get; init; } = 512;

    public bool LittleEndian { get; init; } = false;
}