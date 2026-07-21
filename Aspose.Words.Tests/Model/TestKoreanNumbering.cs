// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2024 by Ilya Navrotskiy

using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests Korean numbering.
    /// </summary>
    [TestFixture]
    public class TestKoreanNumbering
    {
        [SetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests Korean Chosung numbering.
        /// </summary>
        [TestCase(0, "0")]
        [TestCase(1, "ㄱ")]
        [TestCase(10, "ㅊ")]
        [TestCase(11, "ㅋ")]
        [TestCase(14, "ㅎ")]
        [TestCase(15, "ㄱ")]
        [TestCase(20, "ㅂ")]
        [TestCase(28, "ㅎ")]
        [TestCase(100, "ㄴ")]
        [TestCase(1000, "ㅂ")]
        public void TestChosungNumbering(long value, string expectedString)
        {
            string s = NumberConverter.NumberToLocalizedString(value, NumberStyle.Chosung, false);
            Assert.That(s, Is.EqualTo(expectedString));
        }

        /// <summary>
        /// Tests Hangul (koreanLegal) numbering.
        /// </summary>
        [TestCase(0, "0")]
        [TestCase(1, "하나")]
        [TestCase(10, "열")]
        [TestCase(11, "열하나")]
        [TestCase(20, "스물")]
        [TestCase(21, "스물하나")]
        [TestCase(100, "백")]
        [TestCase(101, "백일")]
        [TestCase(110, "백십")]
        [TestCase(111, "백십일")]
        [TestCase(120, "백이십")]
        [TestCase(121, "백이십일")]
        [TestCase(200, "이백")]
        [TestCase(999, "구백구십구")]
        [TestCase(1000, "천")]
        [TestCase(1001, "천일")]
        [TestCase(2000, "이천")]
        [TestCase(10000, "만")]
        [TestCase(10001, "만일")]
        [TestCase(11111, "만일천백십일")]
        [TestCase(32768, "삼만이천칠백육십팔")]
        [TestCase(100000, "십만")]
        [TestCase(1000000, "백만")]
        [TestCase(3400000, "삼백사십만")]
        [TestCase(3410000, "삼백사십만")]
        [TestCase(1001011, "백만일천십일")]
        [TestCase(5050505, "오백오만오백오")]
        [TestCase(9123456, "구백일십이만삼천사백오십육")]
        [TestCase(10000000, "")]
        public void TestHangulNumbering(long value, string expectedString)
        {
            string s = NumberConverter.NumberToLocalizedString(value, NumberStyle.Hangul, false);
            Assert.That(s, Is.EqualTo(expectedString));
        }
    }
}
