// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/05/2021 by Alexey Maslov
using Aspose.Words.RW.Txt.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Txt
{
    /// <summary>
    /// Unit tests for different RomanRightBracketTxtNumberingStyle (Lowercase, Uppercase)
    /// </summary>
    [TestFixture]
    public class TestRomanRightBracketTxtNumberingStyle
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            mNumberingStyleLower = new LowercaseRomanRightBracketTxtNumberingStyle();
            mNumberingStyleUpper = new UppercaseRomanRightBracketTxtNumberingStyle();
        }

        [Test]
        public void TestIsBullet()
        {
            Assert.That(mNumberingStyleLower.IsBullet, Is.False);
            Assert.That(mNumberingStyleUpper.IsBullet, Is.False);
        }

        [Test]
        public void TestIsSet()
        {
            Assert.That(mNumberingStyleLower.IsSet, Is.True);
            Assert.That(mNumberingStyleUpper.IsSet, Is.True);
        }

        [Test]
        public void TestIsLevelsSupported()
        {
            Assert.That(mNumberingStyleLower.IsLevelsSupported, Is.False);
            Assert.That(mNumberingStyleUpper.IsLevelsSupported, Is.False);
        }

        [Test]
        public void TestGetNextNumberLower()
        {
            string nextNumber = mNumberingStyleLower.GetNextNumber("i");
            Assert.That(nextNumber, Is.EqualTo("ii"));

            nextNumber = mNumberingStyleLower.GetNextNumber("mdlxiii");
            Assert.That(nextNumber, Is.EqualTo("mdlxiv"));

            nextNumber = mNumberingStyleLower.GetNextNumber("a");
            Assert.That(nextNumber, Is.EqualTo(""));
        }

        [Test]
        public void TestGetNextNumberUpper()
        {
            string nextNumber = mNumberingStyleUpper.GetNextNumber("V");
            Assert.That(nextNumber, Is.EqualTo("VI"));

            nextNumber = mNumberingStyleUpper.GetNextNumber("MMDCCLXXXIX");
            Assert.That(nextNumber, Is.EqualTo("MMDCCXC"));

            nextNumber = mNumberingStyleUpper.GetNextNumber("5");
            Assert.That(nextNumber, Is.EqualTo(""));
        }

        [Test]
        public void TestGetNumberFormat()
        {
            string numberFormat = mNumberingStyleLower.GetNumberFormat(0);
            Assert.That(numberFormat, Is.EqualTo("\x0000)"));

            numberFormat = mNumberingStyleUpper.GetNumberFormat(0);
            Assert.That(numberFormat, Is.EqualTo("\x0000)"));
        }

        [Test]
        public void TestDetectNumberingLeveLower()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyleLower.DetectNumbering("i) Some text");
            Assert.That(numberingInfo, IsNot.Null());
            Assert.That(numberingInfo.Numbers.Length, Is.EqualTo(1));
            Assert.That(numberingInfo.Numbers[0], Is.EqualTo("i"));
            Assert.That(numberingInfo.Text, Is.EqualTo("i)"));
        }

        [Test]
        public void TestDetectNumberingLeveUpper()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyleUpper.DetectNumbering("XI) Some text");
            Assert.That(numberingInfo, IsNot.Null());
            Assert.That(numberingInfo.Numbers.Length, Is.EqualTo(1));
            Assert.That(numberingInfo.Numbers[0], Is.EqualTo("XI"));
            Assert.That(numberingInfo.Text, Is.EqualTo("XI)"));
        }

        [Test]
        public void TestDetectNumberingNoDelimiter()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyleLower.DetectNumbering("i Some text");
            Assert.That(numberingInfo, Is.Null);

            numberingInfo = mNumberingStyleUpper.DetectNumbering("M Some text");
            Assert.That(numberingInfo, Is.Null);
        }

        [Test]
        public void TestDetectNumberingEmptyText()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyleLower.DetectNumbering("");
            Assert.That(numberingInfo, Is.Null);

            numberingInfo = mNumberingStyleUpper.DetectNumbering("");
            Assert.That(numberingInfo, Is.Null);
        }

#if ANDROID
        static
#endif
        private TxtNumberingStyle mNumberingStyleLower;
#if ANDROID
        static
#endif
        private TxtNumberingStyle mNumberingStyleUpper;
    }
}
