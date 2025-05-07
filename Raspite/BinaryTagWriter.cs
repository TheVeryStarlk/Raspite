using System.Buffers;

namespace Raspite;

internal ref struct BinaryTagWriter(IBufferWriter<byte> writer)
{
}