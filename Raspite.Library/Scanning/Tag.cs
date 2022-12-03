namespace Raspite.Library.Scanning;

public sealed class Tag
{
    /// <summary>
    /// Signifies the end of a <see cref="Compound"/>.
    /// It is only ever used inside a <see cref="Compound"/>, and is not named despite being in a <see cref="Compound"/>.
    /// </summary>
    public static Tag End => new Tag(nameof(End));

    /// <summary>
    /// A single signed byte.
    /// </summary>
    public static Tag Byte => new Tag(nameof(Byte), 1);

    /// <summary>
    /// A single signed, big endian 16 bit integer.
    /// </summary>
    public static Tag Short => new Tag(nameof(Short), 2);

    /// <summary>
    /// A single signed, big endian 32 bit integer.
    /// </summary>
    public static Tag Int => new Tag(nameof(Int), 4);

    /// <summary>
    /// A single signed, big endian 64 bit integer.
    /// </summary>
    public static Tag Long => new Tag(nameof(Long), 8);

    /// <summary>
    /// A single, big endian IEEE-754 single-precision floating point number (<see cref="double.NaN"/> possible).
    /// </summary>
    public static Tag Float => new Tag(nameof(Float), 4);

    /// <summary>
    /// A single, big endian IEEE-754 double-precision floating point number (<see cref="double.NaN"/> possible).
    /// </summary>
    public static Tag Double => new Tag(nameof(Double), 8);

    /// <summary>
    /// A length-prefixed array of signed bytes. The prefix is a signed integer (thus 4 bytes).
    /// </summary>
    public static Tag ByteArray => new Tag(nameof(ByteArray));

    /// <summary>
    /// A length-prefixed modified UTF-8 string.
    /// The prefix is an unsigned short (thus 2 bytes) signifying the length of the string in bytes.
    /// </summary>
    public static Tag String => new Tag(nameof(String));

    /// <summary>
    /// A list of nameless tags, all of the same type.
    /// The list is prefixed with the <see cref="Tag"/> ID of the items it contains (thus 1 byte), and the length of the list as a signed integer (a further 4 bytes).
    /// If the length of the list is 0 or negative, the type may be <see cref="End"/> but otherwise it must be any other type.
    /// (The Notchian implementation uses <see cref="End"/> in that situation, but another reference implementation by Mojang uses 1 instead; parsers should accept any type if the length is less than or equal to 0).
    /// </summary>
    public static Tag List => new Tag(nameof(List));

    /// <summary>
    /// Effectively a list of a named tags. Order is not guaranteed.
    /// </summary>
    public static Tag Compound => new Tag(nameof(Compound));

    /// <summary>
    /// A length-prefixed array of signed integers.
    /// The prefix is a signed integer (thus 4 bytes) and indicates the number of 4 byte integers.
    /// </summary>
    public static Tag IntArray => new Tag(nameof(IntArray));

    /// <summary>
    /// A length-prefixed array of signed longs.
    /// The prefix is a signed integer (thus 4 bytes) and indicates the number of 8 byte longs.
    /// </summary>
    public static Tag LongArray => new Tag(nameof(LongArray));

    /// <summary>
    /// Represents the type of the tag.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Represents the number of bytes that hold the tag's payload. 0 Means undefined.
    /// </summary>
    public int Size { get; }

    // This class works as a "smart" enum.
    private Tag(string type, int size = 0)
    {
        Type = type;
        Size = size;
    }

    public static Tag Resolve(int id)
    {
        return id switch
        {
            0 => End,
            1 => Byte,
            2 => Short,
            3 => Int,
            4 => Long,
            5 => Float,
            6 => Double,
            7 => ByteArray,
            8 => String,
            9 => List,
            10 => Compound,
            11 => IntArray,
            12 => LongArray,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}