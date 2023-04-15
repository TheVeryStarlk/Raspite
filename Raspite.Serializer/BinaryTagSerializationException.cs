namespace Raspite.Serializer;

/// <summary>
/// Represents a faulty state for the binary tag serializer.
/// </summary>
public class BinaryTagSerializationException : Exception
{
    public BinaryTagSerializationException(string message) : base(message)
    {
    }
}