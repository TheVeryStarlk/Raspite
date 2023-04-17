using Raspite.Serializer.Streams;
using Raspite.Serializer.Tags;

namespace Raspite.Serializer;

/// <summary>
/// Represents options that control the way the serializer behaves.
/// </summary>
public sealed class BinaryTagSerializerOptions
{
    /// <summary>
    /// Represents the endianness of the data.
    /// </summary>
    /// <remarks>
    /// False by default.
    /// </remarks>
    public bool LittleEndian { get; set; }
}

/// <summary>
/// Provides functionality to serialize and deserialize Minecraft NBT tags.
/// </summary>
public static class BinaryTagSerializer
{
    /// <summary>
    /// Serializes the provided <see cref="Tag"/> into a <see cref="Stream"/>.
    /// </summary>
    /// <param name="tag">The tag to serialize.</param>
    /// <param name="stream">The destination stream.</param>
    /// <param name="options">Options to control the way serializing behaves.</param>
    public static async Task SerializeAsync(Tag tag, Stream stream, BinaryTagSerializerOptions? options = null)
    {
        options ??= new BinaryTagSerializerOptions();

        var binaryStream = new WriteableBinaryStream(stream, options.LittleEndian);
        var writer = new BinaryTagWriter(binaryStream);

        await writer.EvaluateAsync(tag);
    }

    /// <summary>
    /// Deserializes the provided <see cref="byte"/> array to a <see cref="Tag{T}"/>.
    /// </summary>
    /// <param name="stream">The origin stream.</param>
    /// <param name="options">Options to control the way deserializing behaves.</param>
    /// <typeparam name="T">The tag's value type.</typeparam>
    /// <returns>The deserialized tag.</returns>
    public static async Task<T> DeserializeAsync<T>(Stream stream, BinaryTagSerializerOptions? options = null)
        where T : Tag
    {
        options ??= new BinaryTagSerializerOptions();

        var binaryStream = new ReadableBinaryStream(stream, options.LittleEndian);
        var reader = new BinaryTagReader(binaryStream);

        return (T) await reader.EvaluateAsync();
    }
}