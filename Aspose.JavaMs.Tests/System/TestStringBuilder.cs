// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/01/2007 by Konstantin Sidorenko
// 2016/01/18 by Anatoliy Sidorenko

using System;
using System.Text;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestStringBuilder
    {
        [Test]
        public void TestStringBuilderReplace()
        {
            String result;

            result = sb.Replace("dog", "elefant").ToString();
            Assert.That("The quick brown elefant jumps over the lazy cat.", Is.EqualTo(result));

            result = sb.Replace("e", "E").ToString();
            Assert.That("ThE quick brown ElEfant jumps ovEr thE lazy cat.", Is.EqualTo(result));

            result = sb.Replace("E", "").ToString();
            Assert.That("Th quick brown lfant jumps ovr th lazy cat.", Is.EqualTo(result));

            result = sb.Replace("o", null).ToString();
            Assert.That("Th quick brwn lfant jumps vr th lazy cat.", Is.EqualTo(result));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestStringBuilderReplaceException1()
        {
            sb.Replace(null, "XXX").ToString();
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestStringBuilderReplaceException2()
        {
            sb.Replace("", "XXX").ToString();
        }

        [Test]
        public void TestInsertCount()
        {
            String insert = "Insert";
            String original = "original";

            // Empty initial StringBuilder
            StringBuilder builder = new StringBuilder();
            Assert.That("", Is.EqualTo(builder.Insert(0, insert, 0).ToString()));
            Assert.That(insert, Is.EqualTo(builder.Insert(0, insert, 1).ToString()));

            builder = new StringBuilder();
            Assert.That("InsertInsertInsertInsertInsert", Is.EqualTo(builder.Insert(0, insert, 5).ToString()));

            // Empty insert string
            builder = new StringBuilder();
            Assert.That("", Is.EqualTo(builder.Insert(0, null, 0).ToString()));
            Assert.That("", Is.EqualTo(builder.Insert(0, "", 0).ToString()));

            builder = new StringBuilder(original);
            Assert.That(original, Is.EqualTo(builder.Insert(0, null, 0).ToString()));
            Assert.That(original, Is.EqualTo(builder.Insert(0, "", 0).ToString()));

            builder = new StringBuilder(original);
            Assert.That(original, Is.EqualTo(builder.Insert(3, null, 0).ToString()));
            Assert.That(original, Is.EqualTo(builder.Insert(3, "", 0).ToString()));

            // Insert in the middle
            builder = new StringBuilder(original);
            Assert.That("oriInsertginal", Is.EqualTo(builder.Insert(3, insert, 1).ToString()));
            Assert.That("oriInsertInsertInsertInsertginal", Is.EqualTo(builder.Insert(3, insert, 3).ToString()));

            // Insert into the start
            builder = new StringBuilder(original);
            Assert.That("Insertoriginal", Is.EqualTo(builder.Insert(0, insert, 1).ToString()));
            Assert.That("InsertInsertInsertInsertoriginal", Is.EqualTo(builder.Insert(0, insert, 3).ToString()));

            builder = new StringBuilder(original);
            Assert.That("oInsertriginal", Is.EqualTo(builder.Insert(1, insert, 1).ToString()));
            Assert.That("oInsertInsertInsertInsertriginal", Is.EqualTo(builder.Insert(1, insert, 3).ToString()));

            // Insert to the end
            builder = new StringBuilder(original);
            Assert.That("originalInsert", Is.EqualTo(builder.Insert(original.Length, insert, 1).ToString()));
            Assert.That("originalInsertInsertInsertInsert", Is.EqualTo(builder.Insert(original.Length, insert, 3).ToString()));

            builder = new StringBuilder(original);
            Assert.That("originaInsertl", Is.EqualTo(builder.Insert(original.Length - 1, insert, 1).ToString()));
            Assert.That("originaInsertInsertInsertInsertl", Is.EqualTo(builder.Insert(original.Length - 1, insert, 3).ToString()));
        }

        [Test]
        public void TestUnsignedDigits()
        {
            byte b = 0x1B;
            ushort ush = 0x32FF;
            uint ui = 0xf2374322U;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0:X2},{1:X4},{2:X8}", b, ush, ui);
            Assert.That(builder.ToString(), Is.EqualTo("1B,32FF,F2374322"));

            builder.Length = 0;
            builder.AppendFormat("{0:X2},{1:X4},{2:X8}", 0x1B, 0x32FF, 0xf2374322U);
            Assert.That(builder.ToString(), Is.EqualTo("1B,32FF,F2374322"));
        }

        [TestCase("H ", "H \u0007", "\u0007", "", 2, 1)]
        [TestCase("tea", "test", "st", "a", 2, 2)]
        [TestCase("test", "test", "te", "a", 0, 1)]
        [TestCase("atest", "testtest", "test", "a", 0, 6)]
        [TestCase("atatat", "testtesttest", "tes", "a", 0, 11)]
        public void TestStringReplacedWithinRange(string expected, string data, string oldVal, string newVal, int startIndex, int count)
        {
            StringBuilder builder = new StringBuilder(data);
            builder.Replace(oldVal, newVal, startIndex, count);
            Assert.That(builder.ToString(), Is.EqualTo(expected));
        }

        private StringBuilder sb = new StringBuilder("The quick brown dog jumps over the lazy cat.");
    }
}
