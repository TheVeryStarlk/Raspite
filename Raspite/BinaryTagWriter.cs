using System.Buffers;

namespace Raspite;

internal readonly ref struct BinaryTagWriter(IBufferWriter<byte> writer)
{
    public void WriteString(string name, string value)
    {
        writer.WriteString(name);
        writer.WriteString(value);
    }
}