namespace Raspite;

public sealed class BinaryTagSerializerException(Exception exception) : Exception(exception.Message, exception);