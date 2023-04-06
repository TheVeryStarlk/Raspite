using System.Text;
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