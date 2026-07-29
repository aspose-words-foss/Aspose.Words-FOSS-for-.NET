// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/03/2020 by Edward Voronov

using System.Collections.Generic;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    [TestFixture]
    public class TestRichStrings
    {
        static TestRichStrings()
        {
            gBlueFont.Bold = AttrBoolEx.True;
            gBlueFont.Color = DrColor.Blue;

            gYellowFont.Italic = AttrBoolEx.True;
            gYellowFont.Color = DrColor.Yellow;

            gGreenFont.Color = DrColor.Green;

            gLoremIpsumRichFonts = PopulateFonts(
                new[] {gBlueFont, gYellowFont, gEmptyFont, gBlueFont, gYellowFont},
                new[] {6, 6, 5, 4, 5});
        }

        [Test]
        [TestCase('a')]
        [TestCase('Z')]
        public void TestRichCharToSystemChar(char value)
        {
            RichChar @char = new RichChar(value, gEmptyFont);

            Assert.That(@char.ToSystemChar(), Is.EqualTo(value));
        }

        [Test]
        [TestCase('a', 'A')]
        [TestCase('Z', 'Z')]
        public void TestRichCharToUppper(char value, char expectedResult)
        {
            RichChar @char = new RichChar(value, gBlueFont);

            RichChar result = @char.ToUpperInternal();

            AssertRichChar(new RichChar(value, gBlueFont), @char);
            AssertRichChar(new RichChar(expectedResult, gBlueFont), result);
        }

        [Test]
        [TestCase('a', 'a')]
        [TestCase('Z', 'z')]
        public void TestRichCharToLower(char value, char expectedResult)
        {
            RichChar @char = new RichChar(value, gBlueFont);

            RichChar result = @char.ToLowerInternal();

            AssertRichChar(new RichChar(value, gBlueFont), @char);
            AssertRichChar(new RichChar(expectedResult, gBlueFont), result);
        }

        [Test]
        public void TestRichStringToSystemString()
        {
            RichString @string = BuildLoremIpsumRichString();

            Assert.That(@string.ToSystemString(), Is.EqualTo(LoremIpsum));
        }

        [Test]
        public void TestRichStringLength()
        {
            RichString @string = BuildLoremIpsumRichString();

            Assert.That(@string.Length, Is.EqualTo(26));
        }

        [Test]
        [TestCase(2, 'r')]
        [TestCase(22, 'a')]
        public void TestRichStringIndexer(int index, char expectedResult)
        {
            RichString @string = BuildLoremIpsumRichString();

            Assert.That(@string.GetInternal(index).Character, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("dol", 12)]
        [TestCase("blah", -1)]
        public void TestRichStringIndexOf(string subString, int expectedResult)
        {
            RichString @string = BuildLoremIpsumRichString();

            Assert.That(@string.IndexOf(subString), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("or", 0, 1)]
        [TestCase("or", 10, 15)]
        [TestCase("or", 20, -1)]
        public void TestRichStringIndexOfWithStartIndex(string subString, int startIndex, int expectedResult)
        {
            RichString @string = BuildLoremIpsumRichString();

            Assert.That(@string.IndexOf(subString, startIndex), Is.EqualTo(expectedResult));
        }

        [Test]
        public void TestRichStringSubstring()
        {
            RichString @string = BuildLoremIpsumRichString();

            RichString result = @string.Substring(7, 9);

            AssertRichString(BuildLoremIpsumRichString(), @string);
            RunPr[] fonts = PopulateFonts(
                new[] {gYellowFont, gEmptyFont},
                new[] {5 ,4});
            AssertRichString(BuildRichString("psum dolo", fonts), result);
        }

        [Test]
        public void TestRichStringReplace()
        {
            RichString @string = BuildLoremIpsumRichString();

            RichString result = @string.ReplaceInternal("or", "**");

            AssertRichString(BuildLoremIpsumRichString(), @string);
            AssertRichString(BuildRichString("L**em ipsum dol** sit amet", gLoremIpsumRichFonts), result);
        }

        [Test]
        public void TestRichStringReplaceRecursive()
        {
            RichString @string = BuildLoremIpsumRichString();

            RichString result = @string.ReplaceInternal("or", "'or'");

            AssertRichString(BuildLoremIpsumRichString(), @string);
            RunPr[] fonts = PopulateFonts(
                new[] {gBlueFont, gYellowFont, gEmptyFont, gBlueFont, gYellowFont},
                new[] {8, 6, 7, 4, 5});
            AssertRichString(BuildRichString("L'or'em ipsum dol'or' sit amet", fonts), result);
        }

        [Test]
        public void TestRichStringRemove()
        {
            RichString @string = BuildLoremIpsumRichString();

            RichString result = @string.RemoveInternal(7, 9);

            AssertRichString(BuildLoremIpsumRichString(), @string);

            RunPr[] fonts = PopulateFonts(
                new[] {gBlueFont, gYellowFont, gEmptyFont, gBlueFont, gYellowFont},
                new[] {6, 1, 1, 4, 5});
            AssertRichString(BuildRichString("Lorem ir sit amet", fonts), result);
        }

        [Test]
        public void TestRichStringToUpper()
        {
            RichString @string = BuildLoremIpsumRichString();

            RichString result = @string.ToUpperInternal();

            AssertRichString(BuildLoremIpsumRichString(), @string);
            AssertRichString(BuildRichString("LOREM IPSUM DOLOR SIT AMET", gLoremIpsumRichFonts), result);
        }

        [Test]
        public void TestRichStringToLower()
        {
            RichString @string = BuildLoremIpsumRichString();

            RichString result = @string.ToLowerInternal();

            AssertRichString(BuildLoremIpsumRichString(), @string);
            AssertRichString(BuildRichString("lorem ipsum dolor sit amet", gLoremIpsumRichFonts), result);
        }

        [Test]
        public void TestRichStringToChunks()
        {
            RichString @string = BuildLoremIpsumRichString();

            RichStringChunk[] chunks = @string.ToChunks();

            Assert.That(chunks.Length, Is.EqualTo(5));

            Assert.That(chunks[0].Value, Is.EqualTo("Lorem "));
            Assert.That(chunks[1].Value, Is.EqualTo("ipsum "));
            Assert.That(chunks[2].Value, Is.EqualTo("dolor"));
            Assert.That(chunks[3].Value, Is.EqualTo(" sit"));
            Assert.That(chunks[4].Value, Is.EqualTo(" amet"));

            AssertFont(gBlueFont, chunks[0].RunPr);
            AssertFont(gYellowFont, chunks[1].RunPr);
            AssertFont(gEmptyFont, chunks[2].RunPr);
            AssertFont(gBlueFont, chunks[3].RunPr);
            AssertFont(gYellowFont, chunks[4].RunPr);
        }

        [Test]
        public void TestRichStringBuilderGetLength()
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            Assert.That(builder.Length, Is.EqualTo(26));
        }

        [Test]
        public void TestRichStringBuilderDecreaseLength()
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            builder.Length = 10;

            List<RunPr> fonts = new List<RunPr>(gLoremIpsumRichFonts);
            fonts.RemoveRange(10, fonts.Count - 10);
            AssertRichStringBuilder(BuildRichString("Lorem ipsu", fonts.ToArray()), builder);
        }

        [Test]
        public void TestRichStringBuilderIncreaseLength()
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            builder.Length = 30;

            List<RunPr> fonts = new List<RunPr>(gLoremIpsumRichFonts);
            fonts.AddRange(new[] {gEmptyFont, gEmptyFont, gEmptyFont, gEmptyFont});
            AssertRichStringBuilder(BuildRichString("Lorem ipsum dolor sit amet\0\0\0\0", fonts.ToArray()), builder);
        }

        [Test]
        [TestCase(2, 'r')]
        [TestCase(22, 'a')]
        public void TestRichStringBuilderGetIndexer(int index, char expectedResult)
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            Assert.That(builder.GetInternal(index).Character, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(2, '*', "Lo*em ipsum dolor sit amet")]
        public void TestRichStringBuilderSetIndexer(int index, char value, string expectedResult)
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            builder.SetInternal(index, new RichChar(value, gGreenFont));

            List<RunPr> fonts = new List<RunPr>(gLoremIpsumRichFonts);
            fonts[index] = gGreenFont;
            AssertRichStringBuilder(BuildRichString(expectedResult, fonts.ToArray()), builder);
        }

        [Test]
        public void TestRichStringBuilderAppendRichChar()
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            builder.AppendInternal(new RichChar('z', gGreenFont));

            List<RunPr> fonts = new List<RunPr>(gLoremIpsumRichFonts);
            fonts.Add(gGreenFont);
            AssertRichStringBuilder(BuildRichString("Lorem ipsum dolor sit ametz", fonts.ToArray()), builder);
        }

        [Test]
        public void TestRichStringBuilderAppendChar()
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            builder.AppendInternal('z', new RichChar('\0', gGreenFont));

            List<RunPr> fonts = new List<RunPr>(gLoremIpsumRichFonts);
            fonts.Add(gGreenFont);
            AssertRichStringBuilder(BuildRichString("Lorem ipsum dolor sit ametz", fonts.ToArray()), builder);
        }

        [Test]
        public void TestRichStringBuilderAppendNumberChars()
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            builder.AppendInternal('z', 4, new RichChar('\0', gGreenFont));

            List<RunPr> fonts = new List<RunPr>(gLoremIpsumRichFonts);
            fonts.AddRange(new[] {gGreenFont, gGreenFont, gGreenFont, gGreenFont});
            AssertRichStringBuilder(BuildRichString("Lorem ipsum dolor sit ametzzzz", fonts.ToArray()), builder);
        }

        [Test]
        public void TestRichStringBuilderAppendRichString()
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            builder.AppendInternal(BuildRichString(
                "blah",
                new[] {gEmptyFont, gBlueFont, gYellowFont, gGreenFont}));

            List<RunPr> fonts = new List<RunPr>(gLoremIpsumRichFonts);
            fonts.AddRange(new[] {gEmptyFont, gBlueFont, gYellowFont, gGreenFont});
            AssertRichStringBuilder(BuildRichString("Lorem ipsum dolor sit ametblah", fonts.ToArray()), builder);
        }

        [Test]
        public void TestRichStringBuilderAppendString()
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            builder.AppendInternal("blah", new RichChar('\0', gGreenFont));

            List<RunPr> fonts = new List<RunPr>(gLoremIpsumRichFonts);
            fonts.AddRange(new[] {gGreenFont, gGreenFont, gGreenFont, gGreenFont});
            AssertRichStringBuilder(BuildRichString("Lorem ipsum dolor sit ametblah", fonts.ToArray()), builder);
        }

        [Test]
        [TestCase(0, 'a', "aLorem ipsum dolor sit amet")]
        [TestCase(5, 'Z', "LoremZ ipsum dolor sit amet")]
        public void TestRichStringBuilderInsertChar(int index, char value, string expectedResult)
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            builder.InsertInternal(5, '*', new RichChar('\0', gGreenFont));

            List<RunPr> fonts = new List<RunPr>(gLoremIpsumRichFonts);
            fonts.Insert(5, gGreenFont);
            AssertRichStringBuilder(BuildRichString("Lorem* ipsum dolor sit amet", fonts.ToArray()), builder);
        }

        [Test]
        public void TestRichStringBuilderInsertString()
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            builder.InsertInternal(5, "Wow", new RichChar('\0', gGreenFont));

            List<RunPr> fonts = new List<RunPr>(gLoremIpsumRichFonts);
            fonts.InsertRange(5, new []{gGreenFont, gGreenFont, gGreenFont});
            AssertRichStringBuilder(BuildRichString("LoremWow ipsum dolor sit amet", fonts.ToArray()), builder);
        }

        [Test]
        public void TestRichStringBuilderInsertNumberStrings()
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            List<IChar> charSource = new List<IChar> { new RichChar('\0', gGreenFont), new RichChar('\0', gBlueFont), new RichChar('\0', gYellowFont) };
            builder.InsertInternal(
                5,
                "Wow",
                3,
                charSource);

            List<RunPr> fonts = new List<RunPr>(gLoremIpsumRichFonts);
            for (int i = 0; i < 3; i++)
                fonts.Insert(5, gYellowFont);
            for (int i = 0; i < 3; i++)
                fonts.Insert(5, gBlueFont);
            for (int i = 0; i < 3; i++)
                fonts.Insert(5, gGreenFont);

            AssertRichStringBuilder(BuildRichString("LoremWowWowWow ipsum dolor sit amet", fonts.ToArray()), builder);
        }

        [Test]
        public void TestRichStringBuilderReplaceByValue()
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            builder.ReplaceInternal("or", "**");

            AssertRichStringBuilder(BuildRichString("L**em ipsum dol** sit amet", gLoremIpsumRichFonts), builder);
        }

        [Test]
        public void TestRichStringBuilderReplaceAtIndex()
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            builder.ReplaceInternal(2, '*', new RichChar('\0', gGreenFont));

            List<RunPr> fonts = new List<RunPr>(gLoremIpsumRichFonts);
            fonts[2] = gGreenFont;
            AssertRichStringBuilder(BuildRichString("Lo*em ipsum dolor sit amet", fonts.ToArray()), builder);
        }

        [Test]
        public void TestRichStringBuilderRemove()
        {
            RichStringBuilder builder = BuildLoremIpsumRichStringBuilder();

            builder.RemoveInternal(7, 9);

            RunPr[] fonts = PopulateFonts(
                new[] {gBlueFont, gYellowFont, gEmptyFont, gBlueFont, gYellowFont},
                new[] {6, 1, 1, 4, 5});
            AssertRichStringBuilder(BuildRichString("Lorem ir sit amet", fonts), builder);
        }

        private static RichString BuildLoremIpsumRichString()
        {
            return new RichString(BuildRichChars(LoremIpsum, gLoremIpsumRichFonts));
        }

        private static RichStringBuilder BuildLoremIpsumRichStringBuilder()
        {
            return new RichStringBuilder(BuildLoremIpsumRichString());
        }

        private static RichString BuildRichString(string value, RunPr[] fonts)
        {
            return new RichString(BuildRichChars(value, fonts));
        }

        private static List<IChar> BuildRichChars(string value, params RunPr[] fonts)
        {
            Assert.That(fonts.Length, Is.EqualTo(value.Length));

            List<IChar> chars = new List<IChar>(value.Length);
            for (int i = 0; i < value.Length; i++)
                chars.Add(new RichChar(value[i], fonts[i]));

            return chars;
        }

        private static RunPr[] PopulateFonts(RunPr[] parts, int[] lengths)
        {
            Assert.That(lengths.Length, Is.EqualTo(parts.Length));

            List<RunPr> result = new List<RunPr>();
            for (int i = 0; i < parts.Length; i++)
            {
                for (int j = 0; j < lengths[i]; j++)
                    result.Add(parts[i]);
            }

            return result.ToArray();
        }

        private static void AssertFont(RunPr expected, RunPr actual)
        {
            AssertFont(expected, actual, null);
        }

        private static void AssertFont(RunPr expected, RunPr actual, string context)
        {
            Assert.That(AttrCollection.Equals(expected, actual, null), Is.True, string.Format("{0}expected: {1}; actual:{2}", context, expected, actual));
        }

        private static void AssertRichChar(RichChar expected, RichChar actual)
        {
            AssertRichChar(expected, actual, null);
        }

        private static void AssertRichChar(RichChar expected, RichChar actual, string context)
        {
            Assert.That(actual.Character, Is.EqualTo(expected.Character));
            AssertFont(expected.RunPr, actual.RunPr, string.Format("{0}Char: {1}; ", context, expected.Character));
        }

        internal static void AssertRichString(RichString expected, RichString actual)
        {
            Assert.That(actual.ToSystemString(), Is.EqualTo(expected.ToSystemString()));

            for (int i = 0; i < actual.Length; i++)
                AssertRichChar(expected.GetInternal(i), actual.GetInternal(i), string.Format("{0}[{1}]; ", expected.ToSystemString(), i));
        }

        private static void AssertRichStringBuilder(RichString expected, RichStringBuilder actual)
        {
            AssertRichString(expected, actual.ToRichString());
        }

        private const string LoremIpsum = "Lorem ipsum dolor sit amet";
        private static readonly RunPr[] gLoremIpsumRichFonts;
        private static readonly RunPr gBlueFont = new RunPr();
        private static readonly RunPr gYellowFont = new RunPr();
        private static readonly RunPr gGreenFont = new RunPr();
        private static readonly RunPr gEmptyFont = new RunPr();
    }
}
