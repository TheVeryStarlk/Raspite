using System.Runtime.InteropServices;
using Raspite.Serializer.Tags;

namespace Raspite.Serializer;

internal ref struct BinaryTagWriter(Span<byte> span, bool littleEndian, uint maximumDepth)
{
	public int Position => writer.Position;

	private SpanWriter writer = new(span, littleEndian);
	private uint maximumDepth = maximumDepth;
	private bool nameless;

	public void WriteTag(Tag tag)
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
				var identifier = (byte) (current.Children.Length > 0 ? current.Children[0].Identifier : 0);

				writer.WriteByte(identifier);
				writer.WriteInteger(current.Children.Length);

				foreach (var child in current.Children)
				{
					if (child.Identifier != identifier)
					{
						throw new BinaryTagSerializerException("List tag can only hold one type.");
					}

					nameless = true;
					WriteTag(child);
				}

				break;

			case CompoundTag current:
				foreach (var child in current.Children)
				{
					nameless = false;
					WriteTag(child);
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

		if (maximumDepth <= 0)
		{
			throw new BinaryTagSerializerException("Maximum depth reached.");
		}

		maximumDepth--;
	}
}