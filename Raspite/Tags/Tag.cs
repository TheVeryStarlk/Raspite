using System.Diagnostics;

namespace Raspite.Tags;

[DebuggerDisplay("{Name}")]
public abstract class Tag(string name = "")
{
    public abstract byte Identifier { get; }

    public string Name { get; set; } = name;

    public const byte End = 0;
    public const byte Byte = 1;
    public const byte Short = 2;
    public const byte Integer = 3;
    public const byte Long = 4;
    public const byte Float = 5;
    public const byte Double = 6;
    public const byte ByteCollection = 7;
    public const byte String = 8;
    public const byte List = 9;
    public const byte Compound = 10;
    public const byte IntegerCollection = 11;
    public const byte LongCollection = 12;
}

[DebuggerDisplay("{Name} = {Value}")]
public abstract class Tag<T>(T value, string name = "") : Tag(name)
{
    public T Value => value;
}