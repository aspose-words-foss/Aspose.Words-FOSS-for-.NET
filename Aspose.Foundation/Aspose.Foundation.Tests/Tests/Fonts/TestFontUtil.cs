// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/02/2016 by Alexander Zhiltsov

using Aspose.Fonts;
using NUnit.Framework;

namespace Aspose.Tests.Fonts
{
    /// <summary>
    /// Class for testing methods of <see cref="FontUtil"/>.
    /// </summary>
    [TestFixture]
    public class TestFontUtil
    {
        /// <summary>
        /// Tests the <see cref="FontUtil.ResolveCharset"/> method.
        /// </summary>
        [TestCase(null, "", -1)]
        [TestCase("", "", -1)]
        [TestCase("unknown charset", "", -1)]
        [TestCase("iso-8859-1", "", 0)]
        [TestCase("macintosh", "", 0x4D)]
        [TestCase("shift_jis", "", 0x80)]
        [TestCase("ks_c-5601-1987", "", 0x81)]
        [TestCase("KS C-5601-1992", "", 0x82)] // IANA name is taken from §21.1.2.3.1 of ISO/IEC 29500:1
        [TestCase("GBK", "", 0x86)]
        [TestCase("Big5", "", 0x88)]
        [TestCase("windows-1253", "", 0xA1)]   // IANA name is taken from §21.1.2.3.1 of ISO/IEC 29500:1
        [TestCase("iso-8859-9", "", 0xA2)]
        [TestCase("windows-1258", "", 0xA3)]   // IANA name is taken from §21.1.2.3.1 of ISO/IEC 29500:1
        [TestCase("windows-1255", "", 0xB1)]
        [TestCase("windows-1256", "", 0xB2)]
        [TestCase("windows-1257", "", 0xBA)]   // IANA name is taken from §21.1.2.3.1 of ISO/IEC 29500:1
        [TestCase("windows-1251", "", 0xCC)]
        [TestCase("windows-874", "", 0xDE)]    // IANA name is taken from §21.1.2.3.1 of ISO/IEC 29500:1
        [TestCase("windows-1250", "", 0xEE)]
        public void TestCharsetByIanaName(string ianaName, string fontName, int expectedResult)
        {
            Assert.That(FontUtil.ResolveCharset(ianaName, fontName), Is.EqualTo(expectedResult));
        }

        /// <summary>
        /// Tests the <see cref="FontUtil.IanaNameByCharset"/> method.
        /// </summary>
        [TestCase(-1, "")]
        [TestCase(0, "iso-8859-1")]
        [TestCase(1, "")]
        [TestCase(2, "")]
        [TestCase(0x4D, "macintosh")]
        [TestCase(0x80, "shift_jis")]
        [TestCase(0x81, "ks_c-5601-1987")]
        [TestCase(0x82, "KS C-5601-1992")] // IANA name is taken from §21.1.2.3.1 of ISO/IEC 29500:1
        [TestCase(0x86, "GBK")]
        [TestCase(0x88, "Big5")]
        [TestCase(0xA1, "windows-1253")]   // IANA name is taken from §21.1.2.3.1 of ISO/IEC 29500:1
        [TestCase(0xA2, "iso-8859-9")]
        [TestCase(0xA3, "windows-1258")]   // IANA name is taken from §21.1.2.3.1 of ISO/IEC 29500:1
        [TestCase(0xB1, "windows-1255")]
        [TestCase(0xB2, "windows-1256")]
        [TestCase(0xBA, "windows-1257")]   // IANA name is taken from §21.1.2.3.1 of ISO/IEC 29500:1
        [TestCase(0xCC, "windows-1251")]
        [TestCase(0xDE, "windows-874")]    // IANA name is taken from §21.1.2.3.1 of ISO/IEC 29500:1
        [TestCase(0xEE, "windows-1250")]
        [TestCase(0xFF, "")]
        public void TestIanaNameByCharset(int charset, string expectedIanaName)
        {
            Assert.That(FontUtil.IanaNameByCharset(charset), Is.EqualTo(expectedIanaName));
        }

    }
}
