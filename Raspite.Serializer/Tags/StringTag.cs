using System.Text;

namespace Raspite.Serializer.Tags;

public sealed record StringTag : Tag<string>
{
	public override byte Identifier => 8;

	private StringTag()
	{
	}

	public static StringTag Create(string value, string name = "")
	{
		return new StringTag
		{
			Name = name,
			Value = value
		};
	}

	internal override int CalculateLength(bool nameless)
	{
		var name = nameless ? 0 : sizeof(byte) + sizeof(ushort) + Encoding.UTF8.GetByteCount(Name);
		return name + sizeof(ushort) + Encoding.UTF8.GetByteCount(Value);
	}
}