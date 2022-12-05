using System.Text;

namespace Raspite.Library;

public abstract record Tag(string? Name = null)
{
    public sealed record End : Tag
    {
        public override byte[] Deserialize(bool swap)
        {
            throw new InvalidOperationException();
        }
    }

    public sealed record Byte(byte Value, string? Name = null) : Tag(Name)
    {
        public static int Size => sizeof(byte);

        public override byte[] Deserialize(bool swap)
        {
            var current = 0;
            var name = Encoding.UTF8.GetBytes(Name ?? string.Empty);

            var payload = new byte[3 + name.Length + Size];
            payload[current++] = 1;

            var length = BitConverter.GetBytes((short) name.Length);

            if (swap)
            {
                Array.Reverse(length);
            }

            payload[current++] = length[0];
            payload[current] = length[1];

            for (var index = 0; index < name.Length; index++)
            {
                payload[index + 3] = name[index];
            }

            payload[^1] = Value;
            return payload;
        }
    }

    public sealed record Short(short Value, string? Name = null) : Tag(Name)
    {
        public static int Size => sizeof(short);

        public override byte[] Deserialize(bool swap)
        {
            var current = 0;
            var name = Encoding.UTF8.GetBytes(Name ?? string.Empty);

            var payload = new byte[3 + name.Length + Size];
            payload[current++] = 2;

            var length = BitConverter.GetBytes((short) name.Length);

            if (swap)
            {
                Array.Reverse(length);
            }

            payload[current++] = length[0];
            payload[current++] = length[1];

            foreach (var @byte in name)
            {
                payload[current++] = @byte;
            }

            var value = BitConverter.GetBytes(Value);

            if (swap)
            {
                Array.Reverse(value);
            }

            payload[current++] = value[0];
            payload[current] = value[1];

            return payload;
        }
    }

    public sealed record Int(int Value, string? Name = null) : Tag(Name)
    {
        public static int Size => sizeof(int);

        public override byte[] Deserialize(bool swap)
        {
            var current = 0;
            var name = Encoding.UTF8.GetBytes(Name ?? string.Empty);

            var payload = new byte[3 + name.Length + Size];
            payload[current++] = 3;

            var length = BitConverter.GetBytes((short) name.Length);

            if (swap)
            {
                Array.Reverse(length);
            }

            payload[current++] = length[0];
            payload[current++] = length[1];

            foreach (var @byte in name)
            {
                payload[current++] = @byte;
            }

            var value = BitConverter.GetBytes(Value);

            if (swap)
            {
                Array.Reverse(value);
            }

            foreach (var @byte in value)
            {
                payload[current++] = @byte;
            }

            return payload;
        }
    }

    public sealed record Long(long Value, string? Name = null) : Tag(Name)
    {
        public static int Size => sizeof(long);

        public override byte[] Deserialize(bool swap)
        {
            var current = 0;
            var name = Encoding.UTF8.GetBytes(Name ?? string.Empty);

            var payload = new byte[3 + name.Length + Size];
            payload[current++] = 4;

            var length = BitConverter.GetBytes((short) name.Length);

            if (swap)
            {
                Array.Reverse(length);
            }

            payload[current++] = length[0];
            payload[current++] = length[1];

            foreach (var @byte in name)
            {
                payload[current++] = @byte;
            }

            var value = BitConverter.GetBytes(Value);

            if (swap)
            {
                Array.Reverse(value);
            }

            foreach (var @byte in value)
            {
                payload[current++] = @byte;
            }

            return payload;
        }
    }

    public sealed record Float(float Value, string? Name = null) : Tag(Name)
    {
        public static int Size => sizeof(float);

        public override byte[] Deserialize(bool swap)
        {
            var current = 0;
            var name = Encoding.UTF8.GetBytes(Name ?? string.Empty);

            var payload = new byte[3 + name.Length + Size];
            payload[current++] = 5;

            var length = BitConverter.GetBytes((short) name.Length);

            if (swap)
            {
                Array.Reverse(length);
            }

            payload[current++] = length[0];
            payload[current++] = length[1];

            foreach (var @byte in name)
            {
                payload[current++] = @byte;
            }

            var value = BitConverter.GetBytes(Value);

            if (swap)
            {
                Array.Reverse(value);
            }

            foreach (var @byte in value)
            {
                payload[current++] = @byte;
            }

            return payload;
        }
    }

    public sealed record Double(double Value, string? Name = null) : Tag(Name)
    {
        public static int Size => sizeof(double);

        public override byte[] Deserialize(bool swap)
        {
            var current = 0;
            var name = Encoding.UTF8.GetBytes(Name ?? string.Empty);

            var payload = new byte[3 + name.Length + Size];
            payload[current++] = 6;

            var length = BitConverter.GetBytes((short) name.Length);

            if (swap)
            {
                Array.Reverse(length);
            }

            payload[current++] = length[0];
            payload[current++] = length[1];

            foreach (var @byte in name)
            {
                payload[current++] = @byte;
            }

            var value = BitConverter.GetBytes(Value);

            if (swap)
            {
                Array.Reverse(value);
            }

            foreach (var @byte in value)
            {
                payload[current++] = @byte;
            }

            return payload;
        }
    }

    public sealed record String(string Value, string? Name = null) : Tag(Name)
    {
        public override byte[] Deserialize(bool swap)
        {
            var current = 0;

            var name = Encoding.UTF8.GetBytes(Name ?? string.Empty);
            var value = Encoding.UTF8.GetBytes(Value);

            var payload = new byte[3 + name.Length + 2 + value.Length];
            payload[current++] = 8;

            var nameLength = BitConverter.GetBytes((short) name.Length);

            if (swap)
            {
                Array.Reverse(nameLength);
            }

            payload[current++] = nameLength[0];
            payload[current++] = nameLength[1];

            foreach (var @byte in name)
            {
                payload[current++] = @byte;
            }

            var valueLength = BitConverter.GetBytes((short) value.Length);

            if (swap)
            {
                Array.Reverse(valueLength);
            }

            payload[current++] = valueLength[0];
            payload[current++] = valueLength[1];

            foreach (var @byte in value)
            {
                payload[current++] = @byte;
            }

            return payload;
        }
    }

    public sealed record ByteArray(byte[] Values, string? Name = null) : Tag(Name)
    {
        public override byte[] Deserialize(bool swap)
        {
            var current = 0;

            var name = Encoding.UTF8.GetBytes(Name ?? string.Empty);

            var payload = new byte[3 + name.Length + 4 + Values.Length];
            payload[current++] = 7;

            var nameLength = BitConverter.GetBytes((short) name.Length);

            if (swap)
            {
                Array.Reverse(nameLength);
            }

            payload[current++] = nameLength[0];
            payload[current++] = nameLength[1];

            foreach (var @byte in name)
            {
                payload[current++] = @byte;
            }

            var valueLength = BitConverter.GetBytes(Values.Length);

            if (swap)
            {
                Array.Reverse(valueLength);
            }

            foreach (var @byte in valueLength)
            {
                payload[current++] = @byte;
            }

            foreach (var @byte in Values)
            {
                payload[current++] = @byte;
            }

            return payload;
        }
    }

    public sealed record IntArray(int[] Values, string? Name = null) : Tag(Name)
    {
        public override byte[] Deserialize(bool swap)
        {
            var current = 0;

            var name = Encoding.UTF8.GetBytes(Name ?? string.Empty);

            var size = 0;

            for (var number = 0; number < Values.Length; number++)
            {
                size += Int.Size;
            }

            var payload = new byte[3 + name.Length + 4 + size];
            payload[current++] = 11;

            var nameLength = BitConverter.GetBytes((short) name.Length);

            if (swap)
            {
                Array.Reverse(nameLength);
            }

            payload[current++] = nameLength[0];
            payload[current++] = nameLength[1];

            foreach (var @byte in name)
            {
                payload[current++] = @byte;
            }

            var valueLength = BitConverter.GetBytes(Values.Length);

            if (swap)
            {
                Array.Reverse(valueLength);
            }

            foreach (var @byte in valueLength)
            {
                payload[current++] = @byte;
            }

            foreach (var value in Values)
            {
                var integer = BitConverter.GetBytes(value);

                if (swap)
                {
                    Array.Reverse(integer);
                }

                foreach (var @byte in integer)
                {
                    payload[current++] = @byte;
                }
            }

            return payload;
        }
    }

    public sealed record LongArray(long[] Values, string? Name = null) : Tag(Name)
    {
        public override byte[] Deserialize(bool swap)
        {
            var current = 0;

            var name = Encoding.UTF8.GetBytes(Name ?? string.Empty);

            var size = 0;

            for (var number = 0; number < Values.Length; number++)
            {
                size += Long.Size;
            }

            var payload = new byte[3 + name.Length + 4 + size];
            payload[current++] = 12;

            var nameLength = BitConverter.GetBytes((short) name.Length);

            if (swap)
            {
                Array.Reverse(nameLength);
            }

            payload[current++] = nameLength[0];
            payload[current++] = nameLength[1];

            foreach (var @byte in name)
            {
                payload[current++] = @byte;
            }

            var valueLength = BitConverter.GetBytes(Values.Length);

            if (swap)
            {
                Array.Reverse(valueLength);
            }

            foreach (var @byte in valueLength)
            {
                payload[current++] = @byte;
            }

            foreach (var value in Values)
            {
                var @long = BitConverter.GetBytes(value);

                if (swap)
                {
                    Array.Reverse(@long);
                }

                foreach (var @byte in @long)
                {
                    payload[current++] = @byte;
                }
            }

            return payload;
        }
    }

    public sealed record Compound(Tag[] Children, string? Name = null) : Tag(Name)
    {
        public override byte[] Deserialize(bool swap)
        {
            var name = Encoding.UTF8.GetBytes(Name ?? string.Empty);

            var payload = new List<byte>
            {
                10
            };

            var nameLength = BitConverter.GetBytes((short) name.Length);

            if (swap)
            {
                Array.Reverse(nameLength);
            }

            payload.Add(nameLength[0]);
            payload.Add(nameLength[1]);

            payload.AddRange(name);

            payload.AddRange(Children.SelectMany(child => child.Deserialize(swap)));

            payload.Add(0);
            return payload.ToArray();
        }
    }

    public sealed record List(Tag[] Children, string? Name = null) : Tag(Name)
    {
        public override byte[] Deserialize(bool swap)
        {
            throw new NotImplementedException();
        }
    }

    public static string Resolve(int tag)
    {
        return tag switch
        {
            0 => nameof(End),
            1 => nameof(Byte),
            2 => nameof(Short),
            3 => nameof(Int),
            4 => nameof(Long),
            5 => nameof(Float),
            6 => nameof(Double),
            7 => nameof(ByteArray),
            8 => nameof(String),
            9 => nameof(List),
            10 => nameof(Compound),
            11 => nameof(IntArray),
            12 => nameof(LongArray),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public abstract byte[] Deserialize(bool swap);
}