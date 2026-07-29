// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2012 by Alexey Butalov
using Aspose.Words.RW.Txt.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Txt
{
    /// <summary>
    /// Unit tests for ArabicDotTxtNumberingStyle 
    /// </summary>
    [TestFixture]
    public class TestArabicDotTxtNumberingStyle
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            mNumberingStyle = new ArabicDotTxtNumberingStyle();
        }

        [Test]
        public void TestIsBullet()
        {
            Assert.That(mNumberingStyle.IsBullet, Is.False);
        }

        [Test]
        public void TestIsSet()
        {
            Assert.That(mNumberingStyle.IsSet, Is.True);
        }

        [Test]
        public void TestIsLevelsSupported()
        {
            Assert.That(mNumberingStyle.IsLevelsSupported, Is.True);
        }

        [Test]
        public void TestGetNextNumber()
        {
            string nextNumber = mNumberingStyle.GetNextNumber("1");
            Assert.That(nextNumber, Is.EqualTo("2"));

            nextNumber = mNumberingStyle.GetNextNumber("134");
            Assert.That(nextNumber, Is.EqualTo("135"));

            nextNumber = mNumberingStyle.GetNextNumber("a");
            Assert.That(nextNumber, Is.EqualTo(""));
        }

        [Test]
        public void TestGetNumberFormat()
        {
            string numberFormat = mNumberingStyle.GetNumberFormat(0);
            Assert.That(numberFormat, Is.EqualTo("\x0000."));

            numberFormat = mNumberingStyle.GetNumberFormat(1);
            Assert.That(numberFormat, Is.EqualTo("\x0000.\x0001."));
        }

        [Test]
        public void TestDetectNumberingLevel1()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyle.DetectNumbering("1. Some text");
            Assert.That(numberingInfo, IsNot.Null());
            Assert.That(numberingInfo.Numbers.Length, Is.EqualTo(1));
            Assert.That(numberingInfo.Numbers[0], Is.EqualTo("1"));
            Assert.That(numberingInfo.Text, Is.EqualTo("1."));
        }

        [Test]
        public void TestDetectNumberingLevel2()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyle.DetectNumbering("4.12.9. Some text");
            Assert.That(numberingInfo, IsNot.Null());
            Assert.That(numberingInfo.Numbers.Length, Is.EqualTo(3));
            Assert.That(numberingInfo.Numbers[0], Is.EqualTo("4"));
            Assert.That(numberingInfo.Numbers[1], Is.EqualTo("12"));
            Assert.That(numberingInfo.Numbers[2], Is.EqualTo("9"));
            Assert.That(numberingInfo.Text, Is.EqualTo("4.12.9."));
        }

        [Test]
        public void TestDetectNumberingLevel2WoTrailingDotLevel1()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyle.DetectNumbering("4.12 Some text");
            Assert.That(numberingInfo, IsNot.Null());
            Assert.That(numberingInfo.Numbers.Length, Is.EqualTo(2));
            Assert.That(numberingInfo.Numbers[0], Is.EqualTo("4"));
            Assert.That(numberingInfo.Numbers[1], Is.EqualTo("12"));
            Assert.That(numberingInfo.Text, Is.EqualTo("4.12"));
        }

        [Test]
        public void TestDetectNumberingLevel2WoTrailingDotLevel0()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyle.DetectNumbering("1 Some text");
            Assert.That(numberingInfo, IsNot.Null());
        }

        [Test]
        public void TestDetectNumberingEmptyText()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyle.DetectNumbering("");
            Assert.That(numberingInfo, Is.Null);
        }

#if ANDROID
        static
#endif
        private TxtNumberingStyle mNumberingStyle;
    }
}
