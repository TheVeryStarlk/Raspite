namespace Raspite.Library;

public enum Endianness
{
    Big,
    Little
}

public enum Compression
{
    None,
    GZip
}

/// <summary>
/// Provides options that control the behavior of the <see cref="NbtSerializer"/>.
/// </summary>
public sealed class NbtSerializerOptions
{
    public Endianness Endianness { get; set; }

    public Compression Compression { get; set; }
}