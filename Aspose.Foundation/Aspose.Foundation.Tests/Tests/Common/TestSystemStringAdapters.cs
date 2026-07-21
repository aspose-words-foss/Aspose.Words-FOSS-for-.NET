// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2020 by Edward Voronov

using System.Text;
using Aspose.Common;
using NUnit.Framework;

namespace Aspose.Foundation.Tests.Tests.Common
{
    [TestFixture]
    public class TestSystemStringAdapters
    {
        [Test]
        [TestCase('a')]
        [TestCase('Z')]
        public void TestSystemCharAdapterToSystemChar(char value)
        {
            SystemCharAdapter @char = SystemCharAdapter.Create(value);

            Assert.That(@char.ToSystemChar(), Is.EqualTo(value));
        }

        [Test]
        [TestCase('a', 'A')]
        [TestCase('Z', 'Z')]
        public void TestSystemCharAdapterToUppper(char value, char expectedResult)
        {
            SystemCharAdapter @char = SystemCharAdapter.Create(value);

            SystemCharAdapter result = @char.ToUpperInternal();

            Assert.That(@char.Value, Is.EqualTo(value));
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase('a', 'a')]
        [TestCase('Z', 'z')]
        public void TestSystemCharAdapterToLower(char value, char expectedResult)
        {
            SystemCharAdapter @char = SystemCharAdapter.Create(value);

            SystemCharAdapter result = @char.ToLowerInternal();

            Assert.That(@char.Value, Is.EqualTo(value));
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(LoremIpsum)]
        public void TestSystemStringAdapterToSystemString(string value)
        {
            SystemStringAdapter @string = SystemStringAdapter.Create(value);

            Assert.That(@string.ToSystemString(), Is.EqualTo(value));
        }

        [Test]
        [TestCase("", 0)]
        [TestCase(LoremIpsum, 26)]
        public void TestSystemStringAdapterLength(string value, int expectedResult)
        {
            SystemStringAdapter @string = SystemStringAdapter.Create(value);

            Assert.That(@string.Length, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(LoremIpsum, 2, 'r')]
        [TestCase(LoremIpsum, 22, 'a')]
        public void TestSystemStringAdapterIndexer(string value, int index, char expectedResult)
        {
            SystemStringAdapter @string = SystemStringAdapter.Create(value);

            Assert.That(@string.GetInternal(index).Value, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(LoremIpsum, "dol", 12)]
        [TestCase(LoremIpsum, "blah", -1)]
        public void TestSystemStringAdapterIndexOf(string value, string subString, int expectedResult)
        {
            SystemStringAdapter @string = SystemStringAdapter.Create(value);

            Assert.That(@string.IndexOf(subString), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(LoremIpsum, "or", "**", "L**em ipsum dol** sit amet")]
        [TestCase(LoremIpsum, "blah", "**", LoremIpsum)]
        public void TestSystemStringAdapterReplace(string value, string oldValue, string newValue, string expectedResult)
        {
            SystemStringAdapter @string = SystemStringAdapter.Create(value);

            SystemStringAdapter result = @string.ReplaceInternal(oldValue, newValue);

            Assert.That(@string.Value, Is.EqualTo(value));
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(LoremIpsum, 7, 9, "Lorem ir sit amet")]
        public void TestSystemStringAdapterRemove(string value, int startIndex, int count, string expectedResult)
        {
            SystemStringAdapter @string = SystemStringAdapter.Create(value);

            SystemStringAdapter result = @string.RemoveInternal(startIndex, count);

            Assert.That(@string.Value, Is.EqualTo(value));
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(LoremIpsum, "LOREM IPSUM DOLOR SIT AMET")]
        public void TestSystemStringAdapterToUpper(string value, string expectedResult)
        {
            SystemStringAdapter @string = SystemStringAdapter.Create(value);

            SystemStringAdapter result = @string.ToUpperInternal();

            Assert.That(@string.Value, Is.EqualTo(value));
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(LoremIpsum, "lorem ipsum dolor sit amet")]
        public void TestSystemStringAdapterToLower(string value, string expectedResult)
        {
            SystemStringAdapter @string = SystemStringAdapter.Create(value);

            SystemStringAdapter result = @string.ToLowerInternal();

            Assert.That(@string.Value, Is.EqualTo(value));
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        public void TestSystemStringBuilderAdapterGetLength()
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            Assert.That(builder.Length, Is.EqualTo(26));
        }

        [Test]
        public void TestSystemStringBuilderAdapterSetLength1()
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            builder.Length = 10;

            Assert.That(builder.Value.ToString(), Is.EqualTo("Lorem ipsu"));
        }

        [Test]
        public void TestSystemStringBuilderAdapterSetLength2()
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            builder.Length = 30;

            Assert.That(builder.Value.ToString(), Is.EqualTo("Lorem ipsum dolor sit amet\0\0\0\0"));
        }

        [Test]
        [TestCase(2, 'r')]
        [TestCase(22, 'a')]
        public void TestSystemStringBuilderAdapterGetIndexer(int index, char expectedResult)
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            Assert.That(builder.GetChar(index).Value, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(2, '*', "Lo*em ipsum dolor sit amet")]
        public void TestSystemStringBuilderAdapterSetIndexer(int index, char value, string expectedResult)
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            builder.SetChar(index, SystemCharAdapter.Create(value));

            Assert.That(builder.Value.ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase('a', "Lorem ipsum dolor sit ameta")]
        [TestCase('Z', "Lorem ipsum dolor sit ametZ")]
        public void TestSystemStringBuilderAdapterAppendSystemCharAdapter(char value, string expectedResult)
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            builder.Append(SystemCharAdapter.Create(value));

            Assert.That(builder.Value.ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase('a', "Lorem ipsum dolor sit ameta")]
        [TestCase('Z', "Lorem ipsum dolor sit ametZ")]
        public void TestSystemStringBuilderAdapterAppendChar(char value, string expectedResult)
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            builder.Append(value);

            Assert.That(builder.Value.ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase('a', 1, "Lorem ipsum dolor sit ameta")]
        [TestCase('Z', 5, "Lorem ipsum dolor sit ametZZZZZ")]
        public void TestSystemStringBuilderAdapterAppendNumberChars(char value, int count, string expectedResult)
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            builder.Append(value, count);

            Assert.That(builder.Value.ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("blah", "Lorem ipsum dolor sit ametblah")]
        [TestCase("Wow", "Lorem ipsum dolor sit ametWow")]
        public void TestSystemStringBuilderAdapterAppendSystemStringAdapter(string value, string expectedResult)
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            builder.Append(SystemStringAdapter.Create(value));

            Assert.That(builder.Value.ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("blah", "Lorem ipsum dolor sit ametblah")]
        [TestCase("Wow", "Lorem ipsum dolor sit ametWow")]
        public void TestSystemStringBuilderAdapterAppendString(string value, string expectedResult)
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            builder.Append(value);

            Assert.That(builder.Value.ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(0, 'a', "aLorem ipsum dolor sit amet")]
        [TestCase(5, 'Z', "LoremZ ipsum dolor sit amet")]
        public void TestSystemStringBuilderAdapterInsertChar(int index, char value, string expectedResult)
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            builder.Insert(index, value);

            Assert.That(builder.Value.ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(0, "blah", "blahLorem ipsum dolor sit amet")]
        [TestCase(5, "Wow", "LoremWow ipsum dolor sit amet")]
        public void TestSystemStringBuilderAdapterInsertString(int index, string value, string expectedResult)
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            builder.Insert(index, value);

            Assert.That(builder.Value.ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(0, "blah", 1, "blahLorem ipsum dolor sit amet")]
        [TestCase(5, "Wow", 3, "LoremWowWowWow ipsum dolor sit amet")]
        public void TestSystemStringBuilderAdapterInsertNumberStrings(int index, string value, int count, string expectedResult)
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            builder.Insert(index, value, count);

            Assert.That(builder.Value.ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("or", "**", "L**em ipsum dol** sit amet")]
        [TestCase("blah", "**", LoremIpsum)]
        public void TestSystemStringBuilderAdapterReplaceByValue(string oldValue, string newValue, string expectedResult)
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            builder.ReplaceInternal(oldValue, newValue);

            Assert.That(builder.Value.ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(2, '*', "Lo*em ipsum dolor sit amet")]
        public void TestSystemStringBuilderAdapterReplaceAtIndex(int index, char value, string expectedResult)
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            builder.Replace(index, value);

            Assert.That(builder.Value.ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(6, 6, "Lorem dolor sit amet")]
        public void TestSystemStringBuilderAdapterRemove(int startIndex, int length, string expectedResult)
        {
            SystemStringBuilderAdapter builder = new SystemStringBuilderAdapter(new StringBuilder(LoremIpsum));

            builder.RemoveInternal(startIndex, length);

            Assert.That(builder.Value.ToString(), Is.EqualTo(expectedResult));
        }

        public const string LoremIpsum = "Lorem ipsum dolor sit amet";
    }
}
