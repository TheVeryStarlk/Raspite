using Raspite.Tags;

namespace Raspite;

public readonly record struct SnbtSerializerOptions()
{
    /// <summary>
    /// Whether to format output with newlines and indentation. Defaults to <c>false</c> (compact).
    /// </summary>
    public bool Indented { get; init; } = false;

    /// <summary>
    /// The string used for indentation when <see cref="Indented"/> is <c>true</c>. Defaults to two spaces.
    /// </summary>
    public string IndentString { get; init; } = "  ";

    /// <summary>
    /// Whether to prefer single quotes over double quotes for strings. Defaults to <c>false</c>.
    /// </summary>
    /// <remarks>
    /// The other quote is used instead when it avoids escaping.
    /// </remarks>
    public bool SingleQuoteStrings { get; init; } = false;

    /// <summary>
    /// The maximum allowed nest depth.
    /// </summary>
    public int MaximumDepth { get; init; } = 512;

    /// <summary>
    /// The maximum allowed number of children in a <see cref="ListTag"/> or <see cref="CompoundTag"/>.
    /// </summary>
    public int MaximumChildren { get; init; } = 512;
}
