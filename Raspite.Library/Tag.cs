namespace Raspite.Library;

public abstract record Tag(string? Name = null)
{
    public sealed record End : Tag;

    public sealed record Byte(byte Value, string? Name = null) : Tag(Name)
    {
        public static int Size => sizeof(byte);
    }

    public sealed record Short(short Value, string? Name = null) : Tag(Name)
    {
        public static int Size => sizeof(short);
    }

    public sealed record Int(int Value, string? Name = null) : Tag(Name)
    {
        public static int Size => sizeof(int);
    }

    public sealed record Long(long Value, string? Name = null) : Tag(Name)
    {
        public static int Size => sizeof(long);
    }

    public sealed record Float(float Value, string? Name = null) : Tag(Name)
    {
        public static int Size => sizeof(float);
    }

    public sealed record Double(double Value, string? Name = null) : Tag(Name)
    {
        public static int Size => sizeof(double);
    }

    public sealed record String(string Value, string? Name = null) : Tag(Name);

    public sealed record ByteArray(byte[] Values, string? Name = null) : Tag(Name);

    public sealed record IntArray(int[] Values, string? Name = null) : Tag(Name);

    public sealed record LongArray(long[] Values, string? Name = null) : Tag(Name);

    public sealed record Compound(Tag[] Children, string? Name = null) : Tag(Name);

    public sealed record List(Tag[] Children, string? Name = null) : Tag(Name);

    public static string Resolve(int tag)
    {
        return tag switch
        {
            0 => nameof(End),
            1 => nameof(Byte),
            2 => nameof(Short),
            3 => nameof(Int),
            4 => nameof(Long),
            5 => nameof(Float),
            6 => nameof(Double),
            7 => nameof(ByteArray),
            8 => nameof(String),
            9 => nameof(List),
            10 => nameof(Compound),
            11 => nameof(IntArray),
            12 => nameof(LongArray),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}