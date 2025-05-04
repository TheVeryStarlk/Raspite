using System.Runtime.InteropServices;
using Raspite.Tags;

namespace Raspite;

internal ref struct BinaryTagWriter(Span<byte> span, bool littleEndian, int maximumDepth)
{
    private SpanWriter writer = new(span, littleEndian);
    private int maximumDepth = maximumDepth;
    private bool nameless;

    public void Write(Tag tag)
    {
        if (!nameless)
        {
            writer.Write(tag.Identifier);
            writer.Write(tag.Name);
        }

        switch (tag)
        {
            case ByteTag current:
                writer.Write(current.Value);
                break;

            case ShortTag current:
                writer.Write(current.Value);
                break;

            case IntegerTag current:
                writer.Write(current.Value);
                break;

            case LongTag current:
                writer.Write(current.Value);
                break;

            case FloatTag current:
                writer.Write(current.Value);
                break;

            case DoubleTag current:
                writer.Write(current.Value);
                break;

            case ByteCollectionTag current:
                writer.Write(current.Value.Length);
                writer.Write(current.Value);
                break;

            case StringTag current:
                writer.Write(current.Value);
                break;

            case ListTag current:
                var identifier = (byte) (current.Value.Length > 0 ? current.Value[0].Identifier : 0);

                writer.Write(identifier);
                writer.Write(current.Value.Length);

                foreach (var child in current.Value)
                {
                    if (child.Identifier != identifier)
                    {
                        throw new BinaryTagSerializerException("List tag can only hold one type.");
                    }

                    nameless = true;
                    Write(child);
                }

                break;

            case CompoundTag current:
                foreach (var child in current.Value)
                {
                    nameless = false;
                    Write(child);
                }

                writer.Write((byte) 0);
                break;

            case IntegerCollectionTag current:
                writer.Write(current.Value.Length);

                if (BitConverter.IsLittleEndian == littleEndian)
                {
                    writer.Write(MemoryMarshal.AsBytes(current.Value.AsSpan()));
                    break;
                }

                foreach (var child in current.Value)
                {
                    writer.Write(child);
                }

                break;

            case LongCollectionTag current:
                writer.Write(current.Value.Length);

                if (BitConverter.IsLittleEndian == littleEndian)
                {
                    writer.Write(MemoryMarshal.AsBytes(current.Value.AsSpan()));
                    break;
                }

                foreach (var child in current.Value)
                {
                    writer.Write(child);
                }

                break;

            default:
                throw new BinaryTagSerializerException("Unknown tag.");
        }

        if (maximumDepth < 0)
        {
            throw new BinaryTagSerializerException("Maximum depth reached.");
        }

        maximumDepth--;
    }
}