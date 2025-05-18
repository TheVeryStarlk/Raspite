namespace Raspite;

public readonly struct TagSerializerOptions()
{
    public static TagSerializerOptions Default => new();

    public int MaximumDepth { get; init; } = 512;

    public bool LittleEndian { get; init; } = false;
}