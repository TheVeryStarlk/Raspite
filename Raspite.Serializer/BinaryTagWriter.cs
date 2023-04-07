﻿using System.Text;
using Raspite.Serializer.Tags;

namespace Raspite.Serializer;

internal sealed class BinaryTagWriter
{
    private bool isNameless;

    private readonly BinaryStream stream;

    public BinaryTagWriter(BinaryStream stream)
    {
        this.stream = stream;
    }

    public async Task EvaluateAsync(Tag tag)
    {
        if (!isNameless)
        {
            await stream.WriteBytesAsync(tag.Type);
            await stream.WriteShortAsync((short) tag.Name.Length);
            await stream.WriteBytesAsync(Encoding.UTF8.GetBytes(tag.Name));
        }

        switch (tag)
        {
            case ByteTag byteTag:
                await WriteByteTagAsync(byteTag);
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

            case StringTag stringTag:
                await WriteStringTagAsync(stringTag);
                break;

            case CompoundTag compoundTag:
                await WriteCompoundTagAsync(compoundTag);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(tag), tag, "Unknown tag type.");
        }
    }

    private async Task WriteByteTagAsync(ByteTag tag)
    {
        await stream.WriteBytesAsync(tag.Value);
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

    private async Task WriteStringTagAsync(StringTag tag)
    {
        await stream.WriteUnsignedShortAsync((ushort) tag.Value.Length);
        await stream.WriteBytesAsync(Encoding.UTF8.GetBytes(tag.Value));
    }

    private async Task WriteCompoundTagAsync(CompoundTag tag)
    {
        var wasNameless = isNameless;
        isNameless = false;

        foreach (var child in tag.Children)
        {
            await EvaluateAsync(child);
        }

        await stream.WriteBytesAsync(0);
        isNameless = wasNameless;
    }
}