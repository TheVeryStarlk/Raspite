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
/// Provides options to be used by the <see cref="BinaryReader"/> and the <see cref="BinaryWriter"/>.
/// </summary>
public sealed class NbtSerializerOptions
{
    public Endianness Endianness { get; set; }

    public Compression Compression { get; set; }
}