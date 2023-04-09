﻿using Raspite.Serializer.Streams;
using Raspite.Serializer.Tags;

namespace Raspite.Serializer;

public sealed class BinaryTagSerializerOptions
{
    public bool LittleEndian { get; set; }
}

public static class BinaryTagSerializer
{
    public static async Task SerializeAsync(Tag tag, Stream stream, BinaryTagSerializerOptions? options = null)
    {
        options ??= new BinaryTagSerializerOptions();

        var binaryStream = new BinaryStream(stream, options.LittleEndian);
        var writer = new BinaryTagWriter(binaryStream);

        await writer.EvaluateAsync(tag);
    }

    public static async Task<T> DeserializeAsync<T>(Stream stream, BinaryTagSerializerOptions? options = null)
        where T : Tag
    {
        options ??= new BinaryTagSerializerOptions();

        var binaryStream = new BinaryStream(stream, options.LittleEndian);
        var reader = new BinaryTagReader(binaryStream);

        return await reader.EvaluateAsync<T>();
    }
}