using System.IO.Compression;

namespace Raspite.Library;

/// <summary>
/// Provides functionality to serialize bytes to a <see cref="NbtTag"/> and vice-versa.
/// </summary>
public static class NbtSerializer
{
    /// <summary>
    /// Converts a sequence of bytes into a <see cref="NbtTag"/>.
    /// </summary>
    /// <param name="source">The bytes to convert from.</param>
    /// <param name="options">Options to control the behavior of serializing.</param>
    /// <returns>A representation of the bytes into a <see cref="NbtTag"/>.</returns>
    public static async Task<NbtTag> SerializeAsync(byte[] source, NbtSerializerOptions? options = null)
    {
        if (options?.Compression is Compression.GZip)
        {
            using var destination = new MemoryStream();
            await using var stream = new GZipStream(new MemoryStream(source), CompressionMode.Decompress);

            await stream.CopyToAsync(destination);
            await stream.FlushAsync();

            source = destination.ToArray();
        }

        return new BinaryReader(source, options ?? new NbtSerializerOptions()).Run();
    }

    /// <summary>
    /// Converts a <see cref="NbtTag"/> into sequence a of bytes.
    /// </summary>
    /// <param name="source">The tag to convert from.</param>
    /// <param name="options">Options to control the behavior of deserializing.</param>
    /// <returns>A representation of the <see cref="NbtTag"/> into bytes.</returns>
    public static async Task<byte[]> DeserializeAsync(NbtTag source, NbtSerializerOptions? options = null)
    {
        var bytes = new BinaryWriter(source, options ?? new NbtSerializerOptions()).Run();

        if (options?.Compression is Compression.GZip)
        {
            using var destination = new MemoryStream();
            await using var stream = new GZipStream(destination, CompressionMode.Compress);

            await stream.WriteAsync(bytes);
            await stream.FlushAsync();

            bytes = destination.ToArray();
        }

        return bytes;
    }
}