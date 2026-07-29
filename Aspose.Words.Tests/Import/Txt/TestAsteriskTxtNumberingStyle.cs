// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2012 by Alexey Butalov
using Aspose.Words.RW.Txt.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Txt
{
    /// <summary>
    /// Unit tests for AsteriskTxtNumberingStyle
    /// </summary>
    [TestFixture]
    public class TestAsteriskTxtNumberingStyle
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            mNumberingStyle = new AsteriskTxtNumberingStyle();
        }

        [Test]
        public void TestIsBullet()
        {
            Assert.That(mNumberingStyle.IsBullet, Is.True);
        }

        [Test]
        public void TestIsSet()
        {
            Assert.That(mNumberingStyle.IsSet, Is.False);
        }

        [Test]
        public void TestIsLevelsSupported()
        {
            Assert.That(mNumberingStyle.IsLevelsSupported, Is.False);
        }

        [Test]
        public void TestGetNumberFormatLevel1()
        {
            string numberFormat = mNumberingStyle.GetNumberFormat(0);
            Assert.That(numberFormat, Is.EqualTo("*"));
        }
        
        [Test]
        public void TestDetectNumberingTrue1()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyle.DetectNumbering("* Some text");
            Assert.That(numberingInfo, IsNot.Null());
            Assert.That(numberingInfo.Numbers.Length, Is.EqualTo(1));
            Assert.That(numberingInfo.Numbers[0], Is.EqualTo("*"));
            Assert.That(numberingInfo.Text, Is.EqualTo("*"));
        }

        [Test]
        public void TestDetectNumberingTrue2()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyle.DetectNumbering("*ötext");
            Assert.That(numberingInfo, IsNot.Null());
            Assert.That(numberingInfo.Numbers.Length, Is.EqualTo(1));
            Assert.That(numberingInfo.Numbers[0], Is.EqualTo("*"));
            Assert.That(numberingInfo.Text, Is.EqualTo("*"));
        }

        [Test]
        public void TestDetectNumberingTrue3()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyle.DetectNumbering("*1945");
            Assert.That(numberingInfo, IsNot.Null());
            Assert.That(numberingInfo.Numbers.Length, Is.EqualTo(1));
            Assert.That(numberingInfo.Numbers[0], Is.EqualTo("*"));
            Assert.That(numberingInfo.Text, Is.EqualTo("*"));
        }

        [Test]
        public void TestDetectNumberingFalse1()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyle.DetectNumbering("1 Some text");
            Assert.That(numberingInfo, Is.Null);
        }

        [Test]
        public void TestDetectNumberingFalse2()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyle.DetectNumbering("** Some text");
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
