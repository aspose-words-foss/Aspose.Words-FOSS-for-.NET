// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2012 by Alexey Butalov
using Aspose.Words.RW.Txt.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Txt
{
    /// <summary>
    /// Unit tests for ArabicRightBracketTxtNumberingStyle
    /// </summary>
    [TestFixture]
    public class TestArabicRightBracketTxtNumberingStyle
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            mNumberingStyle = new ArabicRightBracketTxtNumberingStyle();
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
            Assert.That(mNumberingStyle.IsLevelsSupported, Is.False);
        }

        [Test]
        public void TestGetNextNumber()
        {
            string nextNumber = mNumberingStyle.GetNextNumber("2");
            Assert.That(nextNumber, Is.EqualTo("3"));

            nextNumber = mNumberingStyle.GetNextNumber("134");
            Assert.That(nextNumber, Is.EqualTo("135"));

            nextNumber = mNumberingStyle.GetNextNumber("a");
            Assert.That(nextNumber, Is.EqualTo(""));
        }

        [Test]
        public void TestGetNumberFormatLevel1()
        {
            string numberFormat = mNumberingStyle.GetNumberFormat(0);
            Assert.That(numberFormat, Is.EqualTo("\x0000)"));
        }

        [Test]
        public void TestDetectNumbering()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyle.DetectNumbering("4) Some text");
            Assert.That(numberingInfo, IsNot.Null());
            Assert.That(numberingInfo.Numbers.Length, Is.EqualTo(1));
            Assert.That(numberingInfo.Numbers[0], Is.EqualTo("4"));
            Assert.That(numberingInfo.Text, Is.EqualTo("4)"));
        }

        [Test]
        public void TestDetectNumberingWrongText()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyle.DetectNumbering("1 Some text");
            Assert.That(numberingInfo, Is.Null);
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
