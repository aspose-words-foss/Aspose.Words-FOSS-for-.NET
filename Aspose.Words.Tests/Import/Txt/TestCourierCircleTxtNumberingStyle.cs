// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/06/2012 by Alexey Butalov
using Aspose.Words.RW.Txt.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Txt
{
    /// <summary>
    /// Unit tests for CourierCircleTxtNumberingStyle
    /// </summary>
    public class TestCourierCircleTxtNumberingStyle
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            mNumberingStyle = new CourierCircleTxtNumberingStyle();
        }

        [Test]
        public void TestDetectNumberingTrue1()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyle.DetectNumbering("o Some text");
            Assert.That(numberingInfo, IsNot.Null());
            Assert.That(numberingInfo.Numbers.Length, Is.EqualTo(1));
            Assert.That(numberingInfo.Numbers[0], Is.EqualTo("o"));
            Assert.That(numberingInfo.Text, Is.EqualTo("o"));
        }

        [Test]
        public void TestDetectNumberingFalse1()
        {
            TxtNumberingInfo numberingInfo = mNumberingStyle.DetectNumbering("overall");
            Assert.That(numberingInfo, Is.Null);
        }

#if ANDROID
        static
#endif
        private TxtNumberingStyle mNumberingStyle;
    }
}
