namespace Raspite.Serializer;

/// <summary>
/// Represents an error from the binary tag serializer.
/// </summary>
public class BinaryTagSerializationException : Exception
{
    public BinaryTagSerializationException(string message) : base(message)
    {
    }
}