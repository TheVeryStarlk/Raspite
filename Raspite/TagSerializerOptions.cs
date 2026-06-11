namespace Raspite;

public readonly record struct TagSerializerOptions()
{
    /// <summary>
    /// Whether to skip writing/reading the root tag's name and length prefix thereof
    /// </summary>
    /// <remarks>
    /// Necessary for networking from 1.20.2 onwards
    /// </remarks>
    public bool Network { get; init; } = false;

    /// <summary>
    /// Whether to serialize/parse as little-endian (<c>true</c>) or big-endian (<c>false</c>).
    /// </summary>
    public bool LittleEndian { get; init; } = false;

    /// <summary>
    /// The maximum allowed nest depth.
    /// </summary>
    public int MaximumDepth { get; init; } = 512;
}