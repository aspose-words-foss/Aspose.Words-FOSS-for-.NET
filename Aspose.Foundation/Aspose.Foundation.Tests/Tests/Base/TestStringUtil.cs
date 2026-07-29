// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/10/2017 by Konstantin Sidorenko

using NUnit.Framework;

namespace Aspose.Tests.Base
{
    [TestFixture]
    public class TestStringUtil
    {
        /// <summary>
        /// Tests <see cref="StringUtil.Ellipsisize"/> method.
        /// </summary>
        [Test]
        public void TestEllipsisize()
        {
            const string str = "Very long string with large number of characters";
            string output = StringUtil.Ellipsisize(str, 10);
            Assert.That(output, Is.EqualTo("Very lo..."));

            Assert.That(StringUtil.Ellipsisize("1234", 1), Is.EqualTo("1"));
            Assert.That(StringUtil.Ellipsisize("1234", 2), Is.EqualTo("12"));
            Assert.That(StringUtil.Ellipsisize("1234", 3), Is.EqualTo("123"));
            Assert.That(StringUtil.Ellipsisize("1234", 4), Is.EqualTo("1234"));
            Assert.That(StringUtil.Ellipsisize("1234", 5), Is.EqualTo("1234"));
            Assert.That(StringUtil.Ellipsisize("1234567", 5), Is.EqualTo("12..."));
            Assert.That(StringUtil.Ellipsisize("1234567", 6), Is.EqualTo("123..."));
            Assert.That(StringUtil.Ellipsisize("1234567", 7), Is.EqualTo("1234567"));
        }
    }
}