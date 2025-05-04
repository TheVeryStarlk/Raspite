using System.Numerics;

namespace Raspite;

public sealed class BinaryTagSerializerException(string message) : Exception(message)
{
    public static void ThrowIfGreaterThan<T>(T value, T other, string message) where T : INumber<T>
    {
        if (value > other)
        {
            throw new BinaryTagSerializerException(message);
        }
    }
}