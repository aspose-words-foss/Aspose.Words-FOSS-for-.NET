// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/03/2012 by Andrey Soldatov

using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests for <see cref="NumberConverter.ToExcelColumnName"/> and <see cref="NumberConverter.ParseExcelColumnName"/>.
    /// </summary>
    [TestFixture]
    public class TestExcelColumnNumbering
    {
        [Test]
        public void TestLowerCase()
        {
            Assert.That(NumberConverter.ParseExcelColumnName("a"), Is.EqualTo(0));
            Assert.That(NumberConverter.ParseExcelColumnName("z"), Is.EqualTo(25));
            Assert.That(NumberConverter.ParseExcelColumnName("Aa"), Is.EqualTo(26));
            Assert.That(NumberConverter.ParseExcelColumnName("aB"), Is.EqualTo(27));
        }

        [Test]
        public void TestNextValue()
        {
            CheckNextValue("ZZ", "AAA");
            CheckNextValue("ZZZ", "AAAA");
            CheckNextValue("ZZZZ", "AAAAA");
            CheckNextValue("ZZZZZ", "AAAAAA");
        }

        [Test]
        public void TestPair()
        {
            CheckPair(0, "A");
            CheckPair(25, "Z");
            CheckPair(26, "AA");
            CheckPair(27, "AB");
            CheckPair(701, "ZZ");
            CheckPair(702, "AAA");
        }

        [Test]
        public void TestRoundTrip()
        {
            CheckRoundTrip(123456789);
            CheckRoundTrip(987654321);
        }

        [Test]
        public void TestBigNumbers()
        {
            Assert.That(NumberConverter.ToExcelColumnName(int.MaxValue - 1), Is.EqualTo("FXSHRXW"));
            Assert.That(NumberConverter.ToExcelColumnName(int.MaxValue), Is.EqualTo("FXSHRXX"));

            Assert.That(NumberConverter.ParseExcelColumnName("FXSHRXW"), Is.EqualTo(int.MaxValue - 1));
            Assert.That(NumberConverter.ParseExcelColumnName("FXSHRXX"), Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void TestBigNumbersError()
        {
             Assert.That(NumberConverter.ParseExcelColumnName("FXSHRXY"), Is.EqualTo(-1));
        }

        [Test]
        public void TestInvalidColumnIndex()
        {
            Assert.That(NumberConverter.ToExcelColumnName(-1), Is.EqualTo(null));
        }

        private static void CheckNextValue(string prevValue, string nextValue)
        {
            Assert.That(NumberConverter.ParseExcelColumnName(nextValue) - NumberConverter.ParseExcelColumnName(prevValue), Is.EqualTo(1));
        }

        private static void CheckPair(int intValue, string stringValue)
        {
            Assert.That(NumberConverter.ParseExcelColumnName(stringValue), Is.EqualTo(intValue));
            Assert.That(NumberConverter.ToExcelColumnName(intValue), Is.EqualTo(stringValue));
        }

        private static void CheckRoundTrip(int intValue)
        {
            string stringValue = NumberConverter.ToExcelColumnName(intValue);
            int intValue2 = NumberConverter.ParseExcelColumnName(stringValue);

            Assert.That(intValue2, Is.EqualTo(intValue));
        }
    }
}
