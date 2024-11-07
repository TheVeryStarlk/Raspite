using System.Runtime.CompilerServices;
using System.Text;

namespace Raspite.Serializer.Tags;

public abstract record Tag
{
	public abstract byte Identifier { get; }

	public string Name { get; set; } = string.Empty;

	internal virtual int CalculateLength(bool nameless)
	{
		return nameless ? 0 : sizeof(byte) + sizeof(ushort) + Encoding.UTF8.GetByteCount(Name);
	}
}

public abstract record Tag<T> : Tag
{
	public required T Value { get; set; }

	internal override int CalculateLength(bool nameless)
	{
		return base.CalculateLength(nameless) + Unsafe.SizeOf<T>();
	}
}

public abstract record CollectionTag<T> : Tag
{
	public required T[] Children { get; set; }

	public T this[int index]
	{
		get => Children[index];
		set => Children[index] = value;
	}
}