namespace Raspite.Serializer;

public sealed class BinaryTagSerializerException(string message) : Exception(message)
{
	internal static void ThrowIfGreaterThan(int value, uint other, string message)
	{
		if (value > other)
		{
			throw new BinaryTagSerializerException(message);
		}
	}

	internal static void ThrowIfGreaterThan(int value, int other, string message)
	{
		if (value > other)
		{
			throw new BinaryTagSerializerException(message);
		}
	}
}