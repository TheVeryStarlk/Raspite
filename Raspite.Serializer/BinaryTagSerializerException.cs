using System.Numerics;

namespace Raspite.Serializer;

/// <summary>
/// Represents an <see cref="Exception"/> that is thrown when invalid NBT is encountered, or when the defined <see cref="BinaryTagSerializerOptions.MaximumDepth"/> is passed.
/// </summary>
/// <param name="message">The context specific <see cref="BinaryTagSerializerException"/> message.</param>
public sealed class BinaryTagSerializerException(string message) : Exception(message)
{
    /// <summary>
    /// Throws a <see cref="BinaryTagSerializerException"/> if <paramref name="value"/> is greater than <paramref name="other"/>.
    /// </summary>
    /// <param name="value">The argument to validate as less or equal than <paramref name="other"/>.</param>
    /// <param name="other">The value to compare with <paramref name="value"/>.</param>
    /// <param name="message">The context specific <see cref="BinaryTagSerializerException"/> message.</param>
    /// <typeparam name="T">An <see cref="INumber{TSelf}"/> for comparison.</typeparam>
    /// <exception cref="BinaryTagSerializerException">The thrown <see cref="BinaryTagSerializerException"/>.</exception>
    public static void ThrowIfGreaterThan<T>(T value, T other, string message) where T : INumber<T>
    {
        if (value > other)
        {
            throw new BinaryTagSerializerException(message);
        }
    }
}