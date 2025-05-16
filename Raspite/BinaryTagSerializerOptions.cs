namespace Raspite;

public readonly struct BinaryTagSerializerOptions()
{
    public int MaximumDepth { get; init; } = 512;

    public bool LittleEndian { get; init; } = false;
}