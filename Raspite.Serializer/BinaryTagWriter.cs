using System.Runtime.InteropServices;
using Raspite.Serializer.Tags;

namespace Raspite.Serializer;

public ref struct BinaryTagWriter(Span<byte> span, bool littleEndian, int maximumDepth)
{
    public int Position => writer.Position;

    private SpanWriter writer = new(span, littleEndian);
    private int maximumDepth = maximumDepth;

    private bool nameless;

    public void Write(Tag tag)
    {
        if (!nameless)
        {
            writer.WriteByte(tag.Identifier);
            writer.WriteString(tag.Name);
        }

        switch (tag)
        {
            case ByteTag current:
                writer.WriteByte(current.Value);
                break;

            case ShortTag current:
                writer.WriteShort(current.Value);
                break;

            case IntegerTag current:
                writer.WriteInteger(current.Value);
                break;

            case LongTag current:
                writer.WriteLong(current.Value);
                break;

            case FloatTag current:
                writer.WriteFloat(current.Value);
                break;

            case DoubleTag current:
                writer.WriteDouble(current.Value);
                break;

            case ByteCollectionTag current:
                writer.WriteInteger(current.Children.Length);
                writer.Write(current.Children);
                break;

            case StringTag current:
                writer.WriteString(current.Value);
                break;

            case ListTag current:
                var identifier = current.Children.Length > 0 ? current.Children.First().Identifier : byte.MinValue;

                writer.WriteByte(identifier);
                writer.WriteInteger(current.Children.Length);

                foreach (var child in current.Children)
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
                foreach (var child in current.Children)
                {
                    nameless = false;
                    Write(child);
                }

                writer.WriteByte(0);
                break;

            case IntegerCollectionTag current:
                writer.WriteInteger(current.Children.Length);

                if (BitConverter.IsLittleEndian == littleEndian)
                {
                    writer.Write(MemoryMarshal.AsBytes(current.Children.AsSpan()));
                    break;
                }

                foreach (var child in current.Children)
                {
                    writer.WriteInteger(child);
                }

                break;

            case LongCollectionTag current:
                writer.WriteInteger(current.Children.Length);

                if (BitConverter.IsLittleEndian == littleEndian)
                {
                    writer.Write(MemoryMarshal.AsBytes(current.Children.AsSpan()));
                    break;
                }

                foreach (var child in current.Children)
                {
                    writer.WriteLong(child);
                }

                break;

            default:
                throw new BinaryTagSerializerException($"Unknown tag: '{tag}'.");
        }

        if (maximumDepth < 0)
        {
            throw new BinaryTagSerializerException("Maximum depth reached.");
        }

        maximumDepth--;
    }
}