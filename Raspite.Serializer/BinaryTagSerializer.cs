using Raspite.Serializer.Tags;

namespace Raspite.Serializer;

public sealed class BinaryTagSerializerOptions
{
    public bool LittleEndian { get; set; }
}

public static class BinaryTagSerializer
{
    public static TagBase Deserialize(byte[] source, BinaryTagSerializerOptions? options = null)
    {
        options ??= new BinaryTagSerializerOptions();
        return new BinaryTagReader(new MemoryStream(source), options.LittleEndian).Read();
    }

    public static T Deserialize<T>(byte[] source, BinaryTagSerializerOptions? options = null) where T : TagBase
    {
        return (T) Deserialize(source, options);
    }

    public static TagBase Deserialize(Stream source, BinaryTagSerializerOptions? options = null)
    {
        options ??= new BinaryTagSerializerOptions();
        return new BinaryTagReader(source, options.LittleEndian).Read();
    }

    public static T Deserialize<T>(Stream source, BinaryTagSerializerOptions? options = null) where T : TagBase
    {
        return (T) Deserialize(source, options);
    }

    public static byte[] Serialize(TagBase source, BinaryTagSerializerOptions? options = null)
    {
        options ??= new BinaryTagSerializerOptions();
        return new BinaryTagWriter(options.LittleEndian).Write(source).ToArray();
    }
}