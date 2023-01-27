namespace Raspite.Library;

public sealed class BinaryTagSerializerOptions
{
    public bool LittleEndian { get; set; }
}

public static class BinaryTagSerializer
{
    public static TagBase Deserialize(byte[] source, BinaryTagSerializerOptions? options = null)
    {
        options ??= new BinaryTagSerializerOptions();
        return new BinaryTagReader(source.AsSpan(), options.LittleEndian).Read();
    }

    public static T Deserialize<T>(byte[] source, BinaryTagSerializerOptions? options = null) where T : TagBase
    {
        options ??= new BinaryTagSerializerOptions();
        return (T) new BinaryTagReader(source.AsSpan(), options.LittleEndian).Read();
    }

    public static byte[] Serialize(TagBase source, BinaryTagSerializerOptions? options = null)
    {
        options ??= new BinaryTagSerializerOptions();
        return new BinaryTagWriter(options.LittleEndian).Write(source).ToArray();
    }

    public static byte[] Serialize<T>(T source, BinaryTagSerializerOptions? options = null) where T : TagBase
    {
        options ??= new BinaryTagSerializerOptions();
        return new BinaryTagWriter(options.LittleEndian).Write(source).ToArray();
    }
}