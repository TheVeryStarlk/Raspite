﻿using Raspite.Serializer.Streams;
using Raspite.Serializer.Tags;

namespace Raspite.Serializer;

/// <summary>
/// Represents an error that occurred while writing.
/// </summary>
public sealed class BinaryTagWriterException : BinaryTagSerializationException
{
    public BinaryTagWriterException(string message) : base(message)
    {
    }
}

internal sealed class BinaryTagWriter
{
    private bool isNameless;

    private readonly WriteableBinaryStream stream;

    public BinaryTagWriter(WriteableBinaryStream stream)
    {
        this.stream = stream;
    }

    public async Task EvaluateAsync(Tag tag)
    {
        // Do not write the headers if we're inside a list.
        if (!isNameless)
        {
            stream.WriteByte(tag.Type);
            await stream.WriteStringAsync(tag.Name);
        }

        switch (tag)
        {
            case SignedByteTag signedByteTag:
                WriteSignedByteTag(signedByteTag);
                break;

            case ShortTag shortTag:
                await WriteShortTagAsync(shortTag);
                break;

            case IntegerTag integerTag:
                await WriteIntegerTagAsync(integerTag);
                break;

            case LongTag longTag:
                await WriteLongTagAsync(longTag);
                break;

            case FloatTag floatTag:
                await WriteFloatTagAsync(floatTag);
                break;

            case DoubleTag doubleTag:
                await WriteDoubleTagAsync(doubleTag);
                break;

            case SignedByteCollectionTag signedByteCollectionTag:
                await WriteSignedByteCollectionTagAsync(signedByteCollectionTag);
                break;

            case StringTag stringTag:
                await WriteStringTagAsync(stringTag);
                break;

            case ListTag listTag:
                await WriteListTagAsync(listTag);
                break;

            case CompoundTag compoundTag:
                await WriteCompoundTagAsync(compoundTag);
                break;

            case IntegerCollectionTag integerCollectionTag:
                await WriteIntegerCollectionTagAsync(integerCollectionTag);
                break;

            case LongCollectionTag longCollectionTag:
                await WriteLongCollectionTagAsync(longCollectionTag);
                break;

            default:
                throw new BinaryTagWriterException($"Unknown tag type '{tag}'.");
        }
    }

    private void WriteSignedByteTag(SignedByteTag tag)
    {
        stream.WriteSignedByte(tag.Value);
    }

    private async Task WriteShortTagAsync(ShortTag tag)
    {
        await stream.WriteShortAsync(tag.Value);
    }

    private async Task WriteIntegerTagAsync(IntegerTag tag)
    {
        await stream.WriteIntegerAsync(tag.Value);
    }

    private async Task WriteLongTagAsync(LongTag tag)
    {
        await stream.WriteLongAsync(tag.Value);
    }

    private async Task WriteFloatTagAsync(FloatTag tag)
    {
        await stream.WriteFloatAsync(tag.Value);
    }

    private async Task WriteDoubleTagAsync(DoubleTag tag)
    {
        await stream.WriteDoubleAsync(tag.Value);
    }

    private async Task WriteSignedByteCollectionTagAsync(SignedByteCollectionTag tag)
    {
        await stream.WriteIntegerAsync(tag.Children.Length);
        await stream.WriteSignedBytesAsync(tag.Children);
    }

    private async Task WriteStringTagAsync(StringTag tag)
    {
        await stream.WriteStringAsync(tag.Value);
    }

    private async Task WriteListTagAsync(ListTag tag)
    {
        var predefinedType = tag.Children.FirstOrDefault()?.Type ?? 0;

        stream.WriteByte(predefinedType);
        await stream.WriteIntegerAsync(tag.Children.Length);

        foreach (var child in tag.Children)
        {
            if (child.Type != predefinedType)
            {
                throw new BinaryTagWriterException("List tag cannot contain multiple tag types.");
            }

            isNameless = true;
            await EvaluateAsync(child);
        }

        isNameless = false;
    }

    private async Task WriteCompoundTagAsync(CompoundTag tag)
    {
        var wasNameless = isNameless;
        isNameless = false;

        foreach (var child in tag.Children)
        {
            await EvaluateAsync(child);
        }

        stream.WriteByte(0);
        isNameless = wasNameless;
    }

    private async Task WriteIntegerCollectionTagAsync(IntegerCollectionTag tag)
    {
        await stream.WriteIntegerAsync(tag.Children.Length);

        foreach (var child in tag.Children)
        {
            await stream.WriteIntegerAsync(child);
        }
    }

    private async Task WriteLongCollectionTagAsync(LongCollectionTag tag)
    {
        await stream.WriteIntegerAsync(tag.Children.Length);

        foreach (var child in tag.Children)
        {
            await stream.WriteLongAsync(child);
        }
    }
}