using System.Buffers;
using Raspite.Tags;
using Raspite.Tags.Building;

namespace Raspite.Tests;

internal sealed class SnbtSerializerTests
{
    [TestCase("1b", (byte) 1)]
    [TestCase("0B", (byte) 0)]
    [TestCase("127b", (byte) 127)]
    [TestCase("-128B", (byte) 128)]
    [TestCase("-1b", (byte) 255)]
    [TestCase("true", (byte) 1)]
    [TestCase("false", (byte) 0)]
    [TestCase("TRUE", (byte) 1)]
    public void Parsing_ByteTag_IsCorrect(string source, byte expected)
    {
        AssertParsed<ByteTag, byte>(source, expected);
    }

    [TestCase("12s", (short) 12)]
    [TestCase("-32768S", short.MinValue)]
    [TestCase("32767s", short.MaxValue)]
    public void Parsing_ShortTag_IsCorrect(string source, short expected)
    {
        AssertParsed<ShortTag, short>(source, expected);
    }

    [TestCase("0", 0)]
    [TestCase("123", 123)]
    [TestCase("-456", -456)]
    [TestCase("+7", 7)]
    [TestCase("-0", 0)]
    [TestCase("2147483647", int.MaxValue)]
    public void Parsing_IntegerTag_IsCorrect(string source, int expected)
    {
        AssertParsed<IntegerTag, int>(source, expected);
    }

    [TestCase("1234567890123L", 1234567890123L)]
    [TestCase("0l", 0L)]
    [TestCase("-9223372036854775808L", long.MinValue)]
    public void Parsing_LongTag_IsCorrect(string source, long expected)
    {
        AssertParsed<LongTag, long>(source, expected);
    }

    [TestCase("1.5f", 1.5f)]
    [TestCase("-0.25F", -0.25f)]
    [TestCase(".5f", 0.5f)]
    [TestCase("1f", 1f)]
    [TestCase("2.f", 2f)]
    [TestCase("1e3f", 1000f)]
    [TestCase("1.5E-2f", 0.015f)]
    public void Parsing_FloatTag_IsCorrect(string source, float expected)
    {
        AssertParsed<FloatTag, float>(source, expected);
    }

    [TestCase("1.5d", 1.5)]
    [TestCase("0.0D", 0.0)]
    [TestCase("3d", 3.0)]
    [TestCase("1.5", 1.5)]
    [TestCase("-0.25", -0.25)]
    [TestCase("1.", 1.0)]
    [TestCase(".5", 0.5)]
    [TestCase("2.5e2", 250.0)]
    [TestCase("1e-3d", 0.001)]
    public void Parsing_DoubleTag_IsCorrect(string source, double expected)
    {
        AssertParsed<DoubleTag, double>(source, expected);
    }

    [TestCase("hello", "hello")]
    [TestCase("foo.bar_baz+qux-1", "foo.bar_baz+qux-1")]
    [TestCase("  padded  ", "padded")]
    [TestCase("\"hello world\"", "hello world")]
    [TestCase("'hello world'", "hello world")]
    [TestCase("\"\"", "")]
    [TestCase("'say \"hi\"'", "say \"hi\"")]
    [TestCase("\"it's\"", "it's")]
    [TestCase("\"{not: [a, compound]}\"", "{not: [a, compound]}")]
    [TestCase("\"ÅÄÖ 🙂\"", "ÅÄÖ 🙂")]
    public void Parsing_StringTag_IsCorrect(string source, string expected)
    {
        AssertParsed<StringTag, string>(source, expected);
    }

    [TestCase(@"""a\nb""", "a\nb")]
    [TestCase(@"""a\r\tb""", "a\r\tb")]
    [TestCase(@"""\b\f""", "\b\f")]
    [TestCase(@"""\""quoted\""""", "\"quoted\"")]
    [TestCase(@"'it\'s'", "it's")]
    [TestCase(@"""back\\slash""", @"back\slash")]
    [TestCase(@"'\""'", "\"")]
    [TestCase(@"""\s""", " ")]
    [TestCase(@"""\x41\u00e9\U0001F600""", "Aé😀")]
    [TestCase(@"""prefix \n suffix""", "prefix \n suffix")]
    public void Parsing_EscapedStringTag_IsCorrect(string source, string expected)
    {
        AssertParsed<StringTag, string>(source, expected);
    }

    [TestCase("123abc")]
    [TestCase("300b")]
    [TestCase("40000s")]
    [TestCase("2147483648")]
    [TestCase("01")]
    [TestCase("1e5")]
    [TestCase("1.2.3")]
    [TestCase("-")]
    [TestCase(".")]
    [TestCase("b")]
    [TestCase("Infinity")]
    [TestCase("NaN")]
    public void Parsing_NumericFallback_IsStringTag(string source)
    {
        AssertParsed<StringTag, string>(source, source);
    }

    [Test]
    public void Parsing_EmptyCompoundTag_IsCorrect()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(SnbtSerializer.TryParse<CompoundTag>("{}", out var tag), Is.True);
            Assert.That(tag!.Length, Is.Zero);
            Assert.That(tag.Name, Is.Empty);

            Assert.That(SnbtSerializer.TryParse<CompoundTag>("  {  }  ", out tag), Is.True);
            Assert.That(tag!.Length, Is.Zero);
        }
    }

    [Test]
    public void Parsing_CompoundTag_IsCorrect()
    {
        const string source = """
                              {
                                  byte: 1b,
                                  "spaced key": 'value',
                                  'quoted\'key': "x",
                                  123: 4,
                                  nested: { deep: { deeper: {} } },
                                  list: [1, 2, 3],
                                  array: [I; 1, 2],
                              }
                              """;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(SnbtSerializer.TryParse<CompoundTag>(source, out var tag), Is.True);
            Assert.That(tag!.Keys, Is.EquivalentTo(new[] { "byte", "spaced key", "quoted'key", "123", "nested", "list", "array" }));
            Assert.That(tag.GetByte("byte"), Is.EqualTo(1));
            Assert.That(tag.GetString("spaced key"), Is.EqualTo("value"));
            Assert.That(tag.GetString("quoted'key"), Is.EqualTo("x"));
            Assert.That(tag.GetInteger("123"), Is.EqualTo(4));
            Assert.That(tag.GetCompound("nested")!.GetCompound("deep")!.GetCompound("deeper")!.Length, Is.Zero);
            Assert.That(tag.GetList<IntegerTag>("list")!.Value.Select(item => item.Value), Is.EqualTo(new[] { 1, 2, 3 }));
            Assert.That(tag.GetIntegers("array"), Is.EqualTo(new[] { 1, 2 }));

            // The key becomes the child tag's name.
            Assert.That(tag["byte"].Name, Is.EqualTo("byte"));
            Assert.That(tag["spaced key"].Name, Is.EqualTo("spaced key"));
        }
    }

    [TestCase("[1b, 2b]", Tag.Byte, typeof(ListTag<ByteTag>))]
    [TestCase("[true, false]", Tag.Byte, typeof(ListTag<ByteTag>))]
    [TestCase("[1s, 2s]", Tag.Short, typeof(ListTag<ShortTag>))]
    [TestCase("[1, 2, 3]", Tag.Integer, typeof(ListTag<IntegerTag>))]
    [TestCase("[1L, 2L]", Tag.Long, typeof(ListTag<LongTag>))]
    [TestCase("[1f, 2.5f]", Tag.Float, typeof(ListTag<FloatTag>))]
    [TestCase("[1.5, 2d]", Tag.Double, typeof(ListTag<DoubleTag>))]
    [TestCase("[a, 'b c', \"d\"]", Tag.String, typeof(ListTag<StringTag>))]
    [TestCase("[{a: 1}, {}]", Tag.Compound, typeof(ListTag<CompoundTag>))]
    [TestCase("[[1], [a], []]", Tag.List, typeof(ListTag<IListTag>))]
    [TestCase("[[B; 1b], [B;]]", Tag.Bytes, typeof(ListTag<BytesTag>))]
    [TestCase("[[I; 1], [I;]]", Tag.Integers, typeof(ListTag<IntegersTag>))]
    [TestCase("[[L; 1L], [L;]]", Tag.Longs, typeof(ListTag<LongsTag>))]
    public void Parsing_HomogeneousListTag_IsCorrect(string source, byte identifier, Type type)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(SnbtSerializer.TryParse<IListTag>(source, out var tag), Is.True);
            Assert.That(tag, Is.TypeOf(type));
            Assert.That(tag!.Length, Is.GreaterThan(1));
            Assert.That(tag.RawTags.Select(item => item.Identifier), Is.All.EqualTo(identifier));
            Assert.That(tag.RawTags.Select(item => item.Name), Is.All.Empty);
        }
    }

    [TestCase("[]")]
    [TestCase("[ ]")]
    public void Parsing_EmptyListTag_IsEndTagList(string source)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(SnbtSerializer.TryParse<ListTag<EndTag>>(source, out var tag), Is.True);
            Assert.That(tag!.Length, Is.Zero);
        }
    }

    [TestCase("[B;]", new byte[0])]
    [TestCase("[B; 1b, 2b]", new byte[] { 1, 2 })]
    [TestCase("[B;1,-1,127B]", new byte[] { 1, 255, 127 })]
    [TestCase("[ B; 1b, ]", new byte[] { 1 })]
    public void Parsing_BytesTag_IsCorrect(string source, byte[] expected)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(SnbtSerializer.TryParse<BytesTag>(source, out var tag), Is.True);
            Assert.That(tag!.Value, Is.EqualTo(expected).AsCollection);
        }
    }

    [TestCase("[I;]", new int[0])]
    [TestCase("[I; 1, 2]", new[] { 1, 2 })]
    [TestCase("[I;-2147483648,2147483647]", new[] { int.MinValue, int.MaxValue })]
    public void Parsing_IntegersTag_IsCorrect(string source, int[] expected)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(SnbtSerializer.TryParse<IntegersTag>(source, out var tag), Is.True);
            Assert.That(tag!.Value, Is.EqualTo(expected).AsCollection);
        }
    }

    [TestCase("[L;]", new long[0])]
    [TestCase("[L; 1L, 2L]", new long[] { 1, 2 })]
    [TestCase("[L; 1, 9999999999, -3l]", new[] { 1, 9999999999, -3 })]
    public void Parsing_LongsTag_IsCorrect(string source, long[] expected)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(SnbtSerializer.TryParse<LongsTag>(source, out var tag), Is.True);
            Assert.That(tag!.Value, Is.EqualTo(expected).AsCollection);
        }
    }

    [Test]
    public void Parsing_LargeArray_IgnoresMaximumChildren()
    {
        var source = $"[I; {string.Join(", ", Enumerable.Range(0, 1000))}]";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(SnbtSerializer.TryParse<IntegersTag>(source, out var tag), Is.True);
            Assert.That(tag!.Value, Is.EqualTo(Enumerable.Range(0, 1000)).AsCollection);
        }
    }

    [TestCase("[1, 1b]")]
    [TestCase("[1, 1L]")]
    [TestCase("[1.5, 1.5f]")]
    [TestCase("[1, \"a\"]")]
    [TestCase("[a, 1]")]
    [TestCase("[{}, []]")]
    [TestCase("[[I;], [L;]]")]
    [TestCase("[[], {}]")]
    public void Parsing_MixedListTag_Fails(string source)
    {
        AssertFails(source);
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase("{")]
    [TestCase("}")]
    [TestCase("[")]
    [TestCase("]")]
    [TestCase("{a:1")]
    [TestCase("{a:1}}")]
    [TestCase("[1,2")]
    [TestCase("[1 2]")]
    [TestCase("[,]")]
    [TestCase("[1,,]")]
    [TestCase("{,}")]
    [TestCase("{a 1}")]
    [TestCase("{a:}")]
    [TestCase("{:1}")]
    [TestCase("{a:1 b:2}")]
    [TestCase("{a:1,,}")]
    [TestCase("{a:1,a:2}")]
    [TestCase("{\"unterminated:1}")]
    [TestCase("\"unterminated")]
    [TestCase("'mismatched\"")]
    [TestCase("\"bad escape \\q\"")]
    [TestCase("\"short escape \\x4\"")]
    [TestCase("\"invalid scalar \\UFFFFFFFF\"")]
    [TestCase("\"trailing backslash\\")]
    [TestCase("1 2")]
    [TestCase("{a:1} trailing")]
    [TestCase("@")]
    [TestCase("hello world")]
    [TestCase("minecraft:stone")]
    [TestCase("[X; 1]")]
    [TestCase("[B; 1b")]
    [TestCase("[B; 128]")]
    [TestCase("[B; a]")]
    [TestCase("[B; 1s]")]
    [TestCase("[I; 1b]")]
    [TestCase("[I; 1L]")]
    [TestCase("[I; 2147483648]")]
    [TestCase("[L; 1.5]")]
    [TestCase("[1;2]")]
    public void Parsing_Malformed_Fails(string source)
    {
        AssertFails(source);
    }

    [Test]
    public void Parsing_MaximumDepth_IsEnforced()
    {
        const int depth = 8;

        var nested = new string('[', depth) + new string(']', depth);
        var compound = "{a:{b:{c:1}}}";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(SnbtSerializer.TryParse(nested, out _, new SnbtSerializerOptions { MaximumDepth = depth }), Is.True);
            Assert.That(SnbtSerializer.TryParse(nested, out _, new SnbtSerializerOptions { MaximumDepth = depth - 1 }), Is.False);

            // Leaves count towards the depth as well, matching TagSerializer.
            Assert.That(SnbtSerializer.TryParse(compound, out _, new SnbtSerializerOptions { MaximumDepth = 4 }), Is.True);
            Assert.That(SnbtSerializer.TryParse(compound, out _, new SnbtSerializerOptions { MaximumDepth = 3 }), Is.False);

            // Deeply nested input fails cleanly with the default limit.
            Assert.That(SnbtSerializer.TryParse(new string('[', 100_000), out _), Is.False);
            Assert.That(SnbtSerializer.TryParse(string.Concat(Enumerable.Repeat("{a:", 1000)), out _), Is.False);
        }
    }

    [Test]
    public void Parsing_MaximumChildren_IsEnforced()
    {
        var options = new SnbtSerializerOptions { MaximumChildren = 3 };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(SnbtSerializer.TryParse("{a:1,b:2,c:3}", out _, options), Is.True);
            Assert.That(SnbtSerializer.TryParse("{a:1,b:2,c:3,d:4}", out _, options), Is.False);
            Assert.That(SnbtSerializer.TryParse("[1,2,3]", out _, options), Is.True);
            Assert.That(SnbtSerializer.TryParse("[1,2,3,4]", out _, options), Is.False);
            Assert.That(SnbtSerializer.TryParse("[{a:1,b:2,c:3,d:4}]", out _, options), Is.False);

            // Arrays are not limited, matching TagSerializer.
            Assert.That(SnbtSerializer.TryParse("[I;1,2,3,4]", out _, options), Is.True);
        }
    }

    [Test]
    public void Parsing_LargeListTag_IsCorrect()
    {
        var source = $"[{string.Join(',', Enumerable.Range(0, 500).Select(index => $"{{i:{index}}}"))}]";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(SnbtSerializer.TryParse<ListTag<CompoundTag>>(source, out var tag), Is.True);
            Assert.That(tag!.Value.Select(item => item.GetInteger("i")), Is.EqualTo(Enumerable.Range(0, 500)));
        }
    }

    [Test]
    public void Parsing_WrongExpectedType_Throws()
    {
        Assert.Throws<ArgumentException>(() => SnbtSerializer.TryParse<CompoundTag>("1b", out _));
    }

    [Test]
    public void Parsing_InvalidOptions_Throws()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SnbtSerializer.TryParse("{}", out _, new SnbtSerializerOptions { MaximumDepth = 0 }));
            Assert.Throws<ArgumentOutOfRangeException>(() => SnbtSerializer.TryParse("{}", out _, new SnbtSerializerOptions { MaximumChildren = -1 }));
        }
    }

    [Test]
    public void Serializing_Primitives_IsCorrect()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(SnbtSerializer.Serialize(new ByteTag(1)), Is.EqualTo("1b"));
            Assert.That(SnbtSerializer.Serialize(new ByteTag(255)), Is.EqualTo("-1b"));
            Assert.That(SnbtSerializer.Serialize(new ByteTag(true)), Is.EqualTo("1b"));
            Assert.That(SnbtSerializer.Serialize(new ShortTag(-12)), Is.EqualTo("-12s"));
            Assert.That(SnbtSerializer.Serialize(new IntegerTag(int.MinValue)), Is.EqualTo("-2147483648"));
            Assert.That(SnbtSerializer.Serialize(new LongTag(long.MaxValue)), Is.EqualTo("9223372036854775807L"));
            Assert.That(SnbtSerializer.Serialize(new FloatTag(1.5f)), Is.EqualTo("1.5f"));
            Assert.That(SnbtSerializer.Serialize(new FloatTag(0.1f)), Is.EqualTo("0.1f"));
            Assert.That(SnbtSerializer.Serialize(new DoubleTag(0.5)), Is.EqualTo("0.5d"));
            Assert.That(SnbtSerializer.Serialize(new DoubleTag(3)), Is.EqualTo("3d"));
            Assert.That(SnbtSerializer.Serialize(new DoubleTag(1e300)), Is.EqualTo("1E+300d"));
            Assert.That(SnbtSerializer.Serialize(new DoubleTag(double.PositiveInfinity)), Is.EqualTo("1e309d"));
            Assert.That(SnbtSerializer.Serialize(new FloatTag(float.NegativeInfinity)), Is.EqualTo("-1e39f"));
        }
    }

    [TestCase("hello", "hello")]
    [TestCase("foo.bar_baz+qux-1", "foo.bar_baz+qux-1")]
    [TestCase("", "\"\"")]
    [TestCase("hello world", "\"hello world\"")]
    [TestCase("minecraft:stone", "\"minecraft:stone\"")]
    [TestCase("123", "\"123\"")]
    [TestCase("123abc", "\"123abc\"")]
    [TestCase("-1", "\"-1\"")]
    [TestCase(".5", "\".5\"")]
    [TestCase("true", "\"true\"")]
    [TestCase("False", "\"False\"")]
    [TestCase("ÅÄÖ", "\"ÅÄÖ\"")]
    [TestCase("say \"hi\"", "'say \"hi\"'")]
    [TestCase("it's", "\"it's\"")]
    [TestCase("\"it's\"", "\"\\\"it's\\\"\"")]
    [TestCase("a\\b", "\"a\\\\b\"")]
    [TestCase("a\nb\tc\rd\be\ff", "\"a\\nb\\tc\\rd\\be\\ff\"")]
    [TestCase("\u0001\u001f", "\"\\x01\\x1F\"")]
    public void Serializing_StringTag_IsCorrect(string value, string expected)
    {
        Assert.That(SnbtSerializer.Serialize(new StringTag(value)), Is.EqualTo(expected));
    }

    [TestCase("hello world", "'hello world'")]
    [TestCase("it's", "\"it's\"")]
    [TestCase("\"it's\"", "'\"it\\'s\"'")]
    public void Serializing_SingleQuoteStrings_IsCorrect(string value, string expected)
    {
        Assert.That(SnbtSerializer.Serialize(new StringTag(value), new SnbtSerializerOptions { SingleQuoteStrings = true }), Is.EqualTo(expected));
    }

    [Test]
    public void Serializing_Compact_IsCorrect()
    {
        var tag = CompoundTagBuilder
            .Create("Ignored")
            .AddByte(1, "key")
            .AddIntegerList([1, 2, 3], "list")
            .AddIntegers([1, 2], "arr")
            .Build();

        Assert.That(SnbtSerializer.Serialize(tag), Is.EqualTo("{key:1b,list:[1,2,3],arr:[I;1,2]}"));
    }

    [Test]
    public void Serializing_CompactContainers_IsCorrect()
    {
        var tag = CompoundTagBuilder
            .Create()
            .AddCompound(CompoundTagBuilder.Create("empty").Build())
            .AddList(new ListTag<EndTag>([], "none"))
            .AddBytes([], "bytes")
            .AddBytes([1, 255], "signed")
            .AddLongs([1, -2], "longs")
            .AddCompoundList([CompoundTagBuilder.Create().AddString("x", "1 2").Build()], "compounds")
            .AddString("v", "")
            .Build();

        Assert.That(
            SnbtSerializer.Serialize(tag),
            Is.EqualTo("""{empty:{},none:[],bytes:[B;],signed:[B;1b,-1b],longs:[L;1L,-2L],compounds:[{"1 2":x}],"":v}"""));
    }

    [Test]
    public void Serializing_Indented_IsCorrect()
    {
        var tag = CompoundTagBuilder
            .Create()
            .AddByte(1, "key")
            .AddIntegerList([1, 2], "list")
            .AddIntegers([1, 2], "arr")
            .AddIntegers([], "none")
            .AddCompound(CompoundTagBuilder.Create("empty").Build())
            .AddList(new ListTag<EndTag>([], "nothing"))
            .AddCompoundList([CompoundTagBuilder.Create().AddString("value", "spaced key").Build()], "compounds")
            .Build();

        const string expected = """
                                {
                                  key: 1b,
                                  list: [
                                    1,
                                    2
                                  ],
                                  arr: [I; 1, 2],
                                  none: [I;],
                                  empty: {},
                                  nothing: [],
                                  compounds: [
                                    {
                                      "spaced key": value
                                    }
                                  ]
                                }
                                """;

        Assert.That(SnbtSerializer.Serialize(tag, new SnbtSerializerOptions { Indented = true }), Is.EqualTo(expected.ReplaceLineEndings("\n")));
    }

    [Test]
    public void Serializing_IndentString_IsUsed()
    {
        var tag = CompoundTagBuilder
            .Create()
            .AddCompound(CompoundTagBuilder.Create("a").AddByte(1, "b").Build())
            .Build();

        var options = new SnbtSerializerOptions { Indented = true, IndentString = "\t" };

        Assert.That(SnbtSerializer.Serialize(tag, options), Is.EqualTo("{\n\ta: {\n\t\tb: 1b\n\t}\n}"));
    }

    [Test]
    public void Serializing_BufferWriter_MatchesString()
    {
        var tag = CompoundTagBuilder.Create().AddString(new string('x', 10_000), "long").AddIntegerList([1, 2], "list").Build();
        var buffer = new ArrayBufferWriter<char>(1);

        SnbtSerializer.Serialize(buffer, tag);

        Assert.That(buffer.WrittenSpan.ToString(), Is.EqualTo(SnbtSerializer.Serialize(tag)));
    }

    [Test]
    public void Serializing_Limits_Throw()
    {
        var tag = CompoundTagBuilder.Create().AddIntegerList([1, 2, 3, 4], "list").Build();

        using (Assert.EnterMultipleScope())
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SnbtSerializer.Serialize(tag, new SnbtSerializerOptions { MaximumChildren = 3 }));
            Assert.Throws<ArgumentOutOfRangeException>(() => SnbtSerializer.Serialize(tag, new SnbtSerializerOptions { MaximumDepth = 2 }));
            Assert.That(SnbtSerializer.Serialize(tag, new SnbtSerializerOptions { MaximumDepth = 3 }), Is.EqualTo("{list:[1,2,3,4]}"));
        }
    }

    [Test]
    public void Serializing_NonFinite_RoundTrips()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(SnbtSerializer.TryParse<DoubleTag>(SnbtSerializer.Serialize(new DoubleTag(double.NegativeInfinity)), out var @double), Is.True);
            Assert.That(@double!.Value, Is.EqualTo(double.NegativeInfinity));

            Assert.That(SnbtSerializer.TryParse<FloatTag>(SnbtSerializer.Serialize(new FloatTag(float.PositiveInfinity)), out var @float), Is.True);
            Assert.That(@float!.Value, Is.EqualTo(float.PositiveInfinity));
        }
    }

    [TestCase("{}")]
    [TestCase("[]")]
    [TestCase("1b")]
    [TestCase("\"\"")]
    [TestCase("{a:-128b,b:32767s,c:-1,d:1L,e:0.1f,f:-0.0d,g:1.7976931348623157E+308d,h:4.9E-324d,i:1.4E-45f}")]
    [TestCase("{\"a b\":{'c:d':[[1],[[]],[a]]},e:[{},{f:[B;1b,-2b]}],g:[I;],h:[L;-9223372036854775808L]}")]
    [TestCase("{s:\"line\\nbreak \\\"quote\\\" 'single' \\\\ tab\\t ctrl\\x07 é 🙂\"}")]
    [TestCase("[[I;1,2],[I;],[I;3]]")]
    [TestCase("{list:[\"123\",\"true\",\"false\",abc,\"\"],bools:[true,false],keys:{123:1,true:2,\"\":3}}")]
    public void RoundTrip_IsCorrect(string source)
    {
        foreach (var options in new[] { new SnbtSerializerOptions(), new SnbtSerializerOptions { Indented = true }, new SnbtSerializerOptions { SingleQuoteStrings = true } })
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(SnbtSerializer.TryParse(source, out var first), Is.True);

                var serialized = SnbtSerializer.Serialize(first, options);

                Assert.That(SnbtSerializer.TryParse(serialized, out var second), Is.True, serialized);

                AssertEquivalent(first, second);

                // Serialization is stable.
                Assert.That(SnbtSerializer.Serialize(second, options), Is.EqualTo(serialized));
            }
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public void RoundTrip_BigTest_IsCorrect(bool indented)
    {
        using (Assert.EnterMultipleScope())
        {
            var binary = File.ReadAllBytes("bigtest.nbt");

            Assert.That(TagSerializer.TryParse<CompoundTag>(binary, out var expected), Is.True);

            var options = new SnbtSerializerOptions { Indented = indented, MaximumChildren = 1024 };
            var serialized = SnbtSerializer.Serialize(expected!, options);

            Assert.That(SnbtSerializer.TryParse<CompoundTag>(serialized, out var actual, options), Is.True, serialized);

            // SNBT has no root name.
            AssertEquivalent(expected!, actual!, ignoreName: true);

            // Writing the parsed tag back to binary reproduces the original file.
            var renamed = new CompoundTag(actual!.Value, expected!.Name);
            var writer = new ArrayBufferWriter<byte>();

            TagSerializer.Serialize(writer, renamed);

            Assert.That(writer.WrittenSpan.ToArray(), Is.EqualTo(binary).AsCollection);
        }
    }

    private static void AssertParsed<TTag, T>(string source, T expected) where TTag : Tag<T>
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(SnbtSerializer.TryParse<TTag>(source, out var tag), Is.True);
            Assert.That(tag!.Value, Is.EqualTo(expected));
            Assert.That(tag.Name, Is.Empty);
        }
    }

    private static void AssertFails(string source)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(SnbtSerializer.TryParse(source, out var tag), Is.False);
            Assert.That(tag, Is.SameAs(EndTag.Instance));
        }
    }

    private static void AssertEquivalent(ITag expected, ITag actual, bool ignoreName = false)
    {
        Assert.That(actual.Identifier, Is.EqualTo(expected.Identifier));

        if (!ignoreName)
        {
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
        }

        switch (expected)
        {
            case ByteTag current:
                Assert.That(((ByteTag) actual).Value, Is.EqualTo(current.Value));
                break;

            case ShortTag current:
                Assert.That(((ShortTag) actual).Value, Is.EqualTo(current.Value));
                break;

            case IntegerTag current:
                Assert.That(((IntegerTag) actual).Value, Is.EqualTo(current.Value));
                break;

            case LongTag current:
                Assert.That(((LongTag) actual).Value, Is.EqualTo(current.Value));
                break;

            case FloatTag current:
                Assert.That(BitConverter.SingleToInt32Bits(((FloatTag) actual).Value), Is.EqualTo(BitConverter.SingleToInt32Bits(current.Value)));
                break;

            case DoubleTag current:
                Assert.That(BitConverter.DoubleToInt64Bits(((DoubleTag) actual).Value), Is.EqualTo(BitConverter.DoubleToInt64Bits(current.Value)));
                break;

            case StringTag current:
                Assert.That(((StringTag) actual).Value, Is.EqualTo(current.Value));
                break;

            case IListTag current:
            {
                var list = (IListTag) actual;

                Assert.That(list.Length, Is.EqualTo(current.Length));

                for (var index = 0; index < current.Length; index++)
                {
                    AssertEquivalent(current.RawTags[index], list.RawTags[index]);
                }

                break;
            }

            case CompoundTag current:
            {
                var compound = (CompoundTag) actual;

                Assert.That(compound.Value.Select(item => item.Name), Is.EqualTo(current.Value.Select(item => item.Name)));

                for (var index = 0; index < current.Length; index++)
                {
                    AssertEquivalent(current.Value[index], compound.Value[index]);
                }

                break;
            }

            case BytesTag current:
                Assert.That(((BytesTag) actual).Value, Is.EqualTo(current.Value).AsCollection);
                break;

            case IntegersTag current:
                Assert.That(((IntegersTag) actual).Value, Is.EqualTo(current.Value).AsCollection);
                break;

            case LongsTag current:
                Assert.That(((LongsTag) actual).Value, Is.EqualTo(current.Value).AsCollection);
                break;

            default:
                Assert.Fail($"Unexpected {expected.GetType().Name}.");
                break;
        }
    }
}
