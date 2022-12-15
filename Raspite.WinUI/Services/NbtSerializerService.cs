using Raspite.Library;
using System;
using System.Threading.Tasks;

namespace Raspite.WinUI.Services;

internal sealed class NbtSerializerService
{
    public async Task<NbtTag?> SerializeAsync(byte[] source)
    {
        var possibilities = new NbtSerializerOptions[]
        {
            new NbtSerializerOptions()
            {
                Endianness = Endianness.Big,
                Compression = Compression.None
            },
            new NbtSerializerOptions()
            {
                Endianness = Endianness.Little,
                Compression = Compression.None
            },
            new NbtSerializerOptions()
            {
                Endianness = Endianness.Big,
                Compression = Compression.GZip
            },
            new NbtSerializerOptions()
            {
                Endianness = Endianness.Little,
                Compression = Compression.GZip
            }
        };

        foreach (var possibility in possibilities)
        {
            try
            {
                return await NbtSerializer.SerializeAsync(source, possibility);
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        return null;
    }
}
