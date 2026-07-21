// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/12/2006 by Konstantin Sidorenko
// 2016/04/18 by Anatoliy Sidorenko


using System;
using System.Text;
using Aspose.Common;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Text
{
    [TestFixture]
    public class TestEncoding
    {
        //>>>>>>>>>>>>>>>>>>>>>>>> Encoder
        
        /// <summary>
        /// "Convenience method" java.nio.charset.CharsetDecoder.decode(ByteBuffer in) can't properly decode
        /// single one-byte symbol of multibyte encodings(like Shift-Jis) - it just returns an empty char array.
        /// FIXED.Hand made:) "convenience method" used instead for workaround.
        /// </summary>
        [Test]
        public void TestOneByteSymbolOfMultiByteEncoding()
        {
            Encoding encoding = Encoding.GetEncoding("shift_jis");

            byte[] bytes = { 58 };
            char[] chars = encoding.GetChars(bytes);
            Assert.That(chars[0], Is.EqualTo(':'));
        }

        [Test]
        public void TestChinese()
        {
            //the string contains both simplified and traditional chinese signs.
            string inp = "华东哗 鸃 ";

            //JAVA: windows wrongly calls 936 codepage as GB2312 - really it is extended GB2312 - GBK
            //with minor MS addition (euro sign).
            //GB2312 accepts only simplified chinese, GBK supersets GB2312 with traditional chinese.
            Encoding encoding = Encoding.GetEncoding("gb2312");
            byte[] bytes = encoding.GetBytes(inp);
            string outp = encoding.GetString(bytes);
            Assert.That(outp, Is.EqualTo(inp));

            //GB18030 is the last official chinese encoding. support both simplified and traditional signs.
            encoding = Encoding.GetEncoding("GB18030");
            bytes = encoding.GetBytes(inp);
            outp = encoding.GetString(bytes);
            Assert.That(outp, Is.EqualTo(inp));

            //Unicode can handle chinese as well.
            encoding = Encoding.Unicode;
            bytes = encoding.GetBytes(inp);
            outp = encoding.GetString(bytes);
            Assert.That(outp, Is.EqualTo(inp));
        }

        /// <summary>
        /// IDEA 8.0 changes the mean of Default File Encoding configuration param.
        ///
        /// In earler versions we get IDEA Default encoding (utf-8) by Encoding.getDefault() (uses Charset.defaultCharset() internally).
        /// But since IDEA 8.0 we get the system encoding (windows-1252) in the same case.
        ///
        /// As a result some tests can't roundtrip text strings since them encoded and decoded by different encodings.
        /// For instance: TestCustomers.TestJatApostrophe() (old version, changed now) - doc.save(txt) encodes strings
        /// using utf-8, but stream.toString() under IDEA 8 decodes bytes to string using system encoding windows-1252.
        /// </summary>
        [Test]
        public void TestApostrophe()
        {
            string inp = "\u2019";
            Encoding encoding = Encoding.GetEncoding("utf-8");

            byte[] bytes = encoding.GetBytes(inp);
            string outp = encoding.GetString(bytes);
            Assert.That(outp, Is.EqualTo(inp));

            //IDEA 8: default encoding is windows-1252 (system default).
            //IDEA 7 and earler: default encoding is IDEA default encoding (utf-8).
            Encoding defaultEncoging = Encoding.Default;
            Debug.WriteLine("defaultEncoging = " + defaultEncoging);

            //under IDEA 8 some old tests can't roundtrip text strings since them encoded and decoded by different encodings.
            encoding = Encoding.GetEncoding("Windows-1252");
            outp = encoding.GetString(bytes);
            Assert.That(inp.Equals(outp), Is.False);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestDefaultEncoding()
        {
            Assert.That(Encoding.Unicode.CodePage, Is.EqualTo(Encoding.GetEncoding(-1).CodePage));
        }

        [Test]
        public void TestDefaultEncoding2()
        {
#if JAVA
            Assert.That(Encoding.Unicode.CodePage, Is.EqualTo(Encoding.GetEncoding(0).CodePage));
#else
            Assert.That(Encoding.Default.CodePage, Is.EqualTo(Encoding.GetEncoding(0).CodePage));
#endif
        }

        [Test]
        public void TestSomeEncodings() 
        {
            Assert.That(Encoding.Unicode.HeaderName, Is.EqualTo(Encoding.GetEncoding(1200).HeaderName));

            Assert.That("EUC-JP", Is.EqualTo(Encoding.GetEncoding(20932).HeaderName));
            Assert.That("euc-jp", Is.EqualTo(Encoding.GetEncoding(51932).HeaderName));

            Assert.That("euc-jp", Is.EqualTo(Encoding.GetEncoding("euc-jp").HeaderName));
            Assert.That("euc-jp", Is.EqualTo(Encoding.GetEncoding("EUC-JP").HeaderName));

            Assert.That(Encoding.GetEncoding("utf-8").HeaderName, Is.EqualTo(Encoding.GetEncoding(65001).HeaderName));
            Assert.That(Encoding.GetEncoding("iso-8859-5").HeaderName, Is.EqualTo(Encoding.GetEncoding(28595).HeaderName));
            Assert.That(Encoding.GetEncoding("koi8-r").HeaderName, Is.EqualTo(Encoding.GetEncoding(20866).HeaderName));
            Assert.That(Encoding.GetEncoding("Windows-1251").HeaderName, Is.EqualTo(Encoding.GetEncoding(1251).HeaderName));
            Assert.That(Encoding.GetEncoding("big5").HeaderName, Is.EqualTo(Encoding.GetEncoding(950).HeaderName));
            Assert.That(Encoding.GetEncoding("windows-874").HeaderName, Is.EqualTo(Encoding.GetEncoding(874).HeaderName));
        }

        [Test, Ignore("Used to generate TestCases for TestAllEncodings()")]
        [JavaDelete]
        public void GenerateTestCasesForEncodings()
        {
            EncodingInfo[] encodings = Encoding.GetEncodings();
            foreach (EncodingInfo enc in encodings)
                if (enc.Name != "EUC-JP")
                    Console.WriteLine("[TestCase(" + enc.CodePage + ", \"" + enc.Name + "\")]");
        }

        [TestCase(37, "IBM037")]
        [TestCase(437, "IBM437")]
        [TestCase(500, "IBM500")]
        [TestCase(708, "ASMO-708")]
        [TestCase(720, "DOS-720")]
        [TestCase(737, "ibm737")]
        [TestCase(775, "ibm775")]
        [TestCase(850, "ibm850")]
        [TestCase(852, "ibm852")]
        [TestCase(855, "IBM855")]
        [TestCase(857, "ibm857")]
        [TestCase(858, "IBM00858")]
        [TestCase(860, "IBM860")]
        [TestCase(861, "ibm861")]
        [TestCase(862, "DOS-862")]
        [TestCase(863, "IBM863")]
        [TestCase(864, "IBM864")]
        [TestCase(865, "IBM865")]
        [TestCase(866, "cp866")]
        [TestCase(869, "ibm869")]
        [TestCase(870, "IBM870")]
        [TestCase(874, "windows-874")]
        [TestCase(875, "cp875")]
        [TestCase(932, "shift_jis")]
        [TestCase(936, "gb2312")]
        [TestCase(949, "ks_c_5601-1987")]
        [TestCase(950, "big5")]
        [TestCase(1026, "IBM1026")]
        [TestCase(1047, "IBM01047")]
        [TestCase(1140, "IBM01140")]
        [TestCase(1141, "IBM01141")]
        [TestCase(1142, "IBM01142")]
        [TestCase(1143, "IBM01143")]
        [TestCase(1144, "IBM01144")]
        [TestCase(1145, "IBM01145")]
        [TestCase(1146, "IBM01146")]
        [TestCase(1147, "IBM01147")]
        [TestCase(1148, "IBM01148")]
        [TestCase(1149, "IBM01149")]
        [TestCase(1200, "utf-16")]
        [TestCase(1201, "utf-16BE")]
        [TestCase(1250, "windows-1250")]
        [TestCase(1251, "windows-1251")]
        [TestCase(1252, "Windows-1252")]
        [TestCase(1253, "windows-1253")]
        [TestCase(1254, "windows-1254")]
        [TestCase(1255, "windows-1255")]
        [TestCase(1256, "windows-1256")]
        [TestCase(1257, "windows-1257")]
        [TestCase(1258, "windows-1258")]
        [TestCase(1361, "Johab")]
        [TestCase(10000, "macintosh")]
        [TestCase(10001, "x-mac-japanese")]
        [TestCase(10002, "x-mac-chinesetrad")]
        [TestCase(10003, "x-mac-korean")]
        [TestCase(10004, "x-mac-arabic")]
        [TestCase(10005, "x-mac-hebrew")]
        [TestCase(10006, "x-mac-greek")]
        [TestCase(10007, "x-mac-cyrillic")]
        [TestCase(10008, "x-mac-chinesesimp")]
        [TestCase(10010, "x-mac-romanian")]
        [TestCase(10017, "x-mac-ukrainian")]
        [TestCase(10021, "x-mac-thai")]
        [TestCase(10029, "x-mac-ce")]
        [TestCase(10079, "x-mac-icelandic")]
        [TestCase(10081, "x-mac-turkish")]
        [TestCase(10082, "x-mac-croatian")]
        [TestCase(12000, "utf-32")]
        [TestCase(12001, "utf-32BE")]
        [TestCase(20000, "x-Chinese-CNS")]
        [TestCase(20001, "x-cp20001")]
        [TestCase(20002, "x-Chinese-Eten")]
        [TestCase(20003, "x-cp20003")]
        [TestCase(20004, "x-cp20004")]
        [TestCase(20005, "x-cp20005")]
        [TestCase(20105, "x-IA5")]
        [TestCase(20106, "x-IA5-German")]
        [TestCase(20107, "x-IA5-Swedish")]
        [TestCase(20108, "x-IA5-Norwegian")]
        [TestCase(20127, "us-ascii")]
        [TestCase(20261, "x-cp20261")]
        [TestCase(20269, "x-cp20269")]
        [TestCase(20273, "IBM273")]
        [TestCase(20277, "IBM277")]
        [TestCase(20278, "IBM278")]
        [TestCase(20280, "IBM280")]
        [TestCase(20284, "IBM284")]
        [TestCase(20285, "IBM285")]
        [TestCase(20290, "IBM290")]
        [TestCase(20297, "IBM297")]
        [TestCase(20420, "IBM420")]
        [TestCase(20423, "IBM423")]
        [TestCase(20424, "IBM424")]
        [TestCase(20833, "x-EBCDIC-KoreanExtended")]
        [TestCase(20838, "IBM-Thai")]
        [TestCase(20866, "koi8-r")]
        [TestCase(20871, "IBM871")]
        [TestCase(20880, "IBM880")]
        [TestCase(20905, "IBM905")]
        [TestCase(20924, "IBM00924")]
        [TestCase(20936, "x-cp20936")]
        [TestCase(20949, "x-cp20949")]
        [TestCase(21025, "cp1025")]
        [TestCase(21866, "koi8-u")]
        [TestCase(28591, "iso-8859-1")]
        [TestCase(28592, "iso-8859-2")]
        [TestCase(28593, "iso-8859-3")]
        [TestCase(28594, "iso-8859-4")]
        [TestCase(28595, "iso-8859-5")]
        [TestCase(28596, "iso-8859-6")]
        [TestCase(28597, "iso-8859-7")]
        [TestCase(28598, "iso-8859-8")]
        [TestCase(28599, "iso-8859-9")]
        [TestCase(28603, "iso-8859-13")]
        [TestCase(28605, "iso-8859-15")]
        [TestCase(29001, "x-Europa")]
        [TestCase(38598, "iso-8859-8-i")]
        [TestCase(50220, "iso-2022-jp")]
        [TestCase(50221, "csISO2022JP")]
        [TestCase(50222, "iso-2022-jp")]
        [TestCase(50225, "iso-2022-kr")]
        [TestCase(50227, "x-cp50227")]
        [TestCase(51932, "euc-jp")]
        [TestCase(51936, "EUC-CN")]
        [TestCase(51949, "euc-kr")]
        [TestCase(52936, "hz-gb-2312")]
        [TestCase(54936, "GB18030")]
        [TestCase(57002, "x-iscii-de")]
        [TestCase(57003, "x-iscii-be")]
        [TestCase(57004, "x-iscii-ta")]
        [TestCase(57005, "x-iscii-te")]
        [TestCase(57006, "x-iscii-as")]
        [TestCase(57007, "x-iscii-or")]
        [TestCase(57008, "x-iscii-ka")]
        [TestCase(57009, "x-iscii-ma")]
        [TestCase(57010, "x-iscii-gu")]
        [TestCase(57011, "x-iscii-pa")]
        [TestCase(65000, "utf-7")]
        [TestCase(65001, "utf-8")]
        public void TestAllEncodings(int codePage, string name)
        {
            Assert.That(Encoding.GetEncoding(name).HeaderName, Is.EqualTo(Encoding.GetEncoding(codePage).HeaderName));
        }

        [Test]
        public void TestUtf7()
        {
            Encoding utf7 = Encoding.GetEncoding("utf-7");

            // This just throws if can't get the charset. This can happen if our utf-7 is not properly registered.
            utf7.GetDecoder();

            Assert.That(Encoding.GetEncoding("utf-7").HeaderName, Is.EqualTo(Encoding.GetEncoding(65000).HeaderName));
        }

        [Test]
        public void TestGetBytes()
        {
            string testStr = "Test String Тестовая строчка 123 /n.";

            byte[] unicodeBytes =
            {
                84, 0, 101, 0, 115, 0, 116, 0, 32, 0, 83, 0, 116, 0, 114, 0, 105, 0, 110, 0, 103, 0,
                32, 0, 34, 4, 53, 4, 65, 4, 66, 4, 62, 4, 50, 4, 48, 4, 79, 4, 32, 0, 65, 4, 66, 4, 64, 4, 62, 4, 71, 4, 58, 4,
                48, 4, 32, 0, 49, 0, 50, 0, 51, 0, 32, 0, 47, 0, 110, 0, 46, 0
            };

            byte[] win1252Bytes =
            {
                84, 101, 115, 116, 32, 83, 116, 114, 105, 110, 103, 32, 63, 63, 63, 63, 63, 63, 63,
                63, 32, 63, 63, 63, 63, 63, 63, 63, 32, 49, 50, 51, 32, 47, 110, 46
            };

            byte[] bytes = Encoding.Unicode.GetBytes(testStr);
            Assert.That(unicodeBytes, Is.EqualTo(bytes));

            bytes = Encoding.GetEncoding("Windows-1252").GetBytes(testStr);
            Assert.That(win1252Bytes, Is.EqualTo(bytes));
        }

        [Test]
        public void TestGetBytesFromCharsIndexed()
        {
            string testStr = "Test string Тестовая строчка 123 /n.";
            char[] chars = testStr.ToCharArray();

            byte[] unicodeBytes =
            {
                84, 0, 101, 0, 115, 0, 116, 0, 32, 0, 83, 0, 116, 0, 114, 0, 105, 0, 110, 0, 103, 0,
                32, 0, 34, 4, 53, 4, 65, 4, 66, 4, 62, 4, 50, 4, 48, 4, 79, 4, 32, 0, 65, 4, 66, 4, 64, 4, 62, 4, 71, 4, 58, 4,
                48, 4, 32, 0, 49, 0, 50, 0, 51, 0, 32, 0, 47, 0, 110, 0, 46, 0
            };

            byte[] bytes = Encoding.Unicode.GetBytes(chars, 0, 1);
            bytes = Encoding.Unicode.GetBytes(chars, chars.Length - 1, 1);
            bytes = Encoding.Unicode.GetBytes(chars, 10, 1);
            bytes = Encoding.Unicode.GetBytes(chars, 10, 2);
            bytes = Encoding.Unicode.GetBytes(chars, 10, 3);
        }

        [Test]
        public void TestGetString()
        {
            string testStrUnicode = "Test String Тестовая строчка 123 /n.";
            string testStrWin1252 = "Test String ???????? ??????? 123 /n.";

            byte[] unicodeBytes =
            {
                84, 0, 101, 0, 115, 0, 116, 0, 32, 0, 83, 0, 116, 0, 114, 0, 105, 0, 110, 0, 103, 0,
                32, 0, 34, 4, 53, 4, 65, 4, 66, 4, 62, 4, 50, 4, 48, 4, 79, 4, 32, 0, 65, 4, 66, 4, 64, 4, 62, 4, 71, 4, 58, 4,
                48, 4, 32, 0, 49, 0, 50, 0, 51, 0, 32, 0, 47, 0, 110, 0, 46, 0
            };

            byte[] win1252Bytes =
            {
                84, 101, 115, 116, 32, 83, 116, 114, 105, 110, 103, 32, 63, 63, 63, 63, 63, 63, 63,
                63, 32, 63, 63, 63, 63, 63, 63, 63, 32, 49, 50, 51, 32, 47, 110, 46
            };

            string gotten = Encoding.Unicode.GetString(unicodeBytes);
            Assert.That(testStrUnicode, Is.EqualTo(gotten));

            gotten = Encoding.GetEncoding("Windows-1252").
            GetString(win1252Bytes);
            Assert.That(testStrWin1252, Is.EqualTo(gotten));
        }

        [Test]
        public void TestGetChars()
        {
            string testStrUnicode = "Test String Тестовая строчка 123 /n.";
            string testStrWin1252 = "Test String ???????? ??????? 123 /n.";

            byte[] unicodeBytes =
            {
                84, 0, 101, 0, 115, 0, 116, 0, 32, 0, 83, 0, 116, 0, 114, 0, 105, 0, 110, 0, 103, 0,
                32, 0, 34, 4, 53, 4, 65, 4, 66, 4, 62, 4, 50, 4, 48, 4, 79, 4, 32, 0, 65, 4, 66, 4, 64, 4, 62, 4, 71, 4, 58, 4,
                48, 4, 32, 0, 49, 0, 50, 0, 51, 0, 32, 0, 47, 0, 110, 0, 46, 0
            };

            byte[] win1252Bytes =
            {
                84, 101, 115, 116, 32, 83, 116, 114, 105, 110, 103, 32, 63, 63, 63, 63, 63, 63, 63,
                63, 32, 63, 63, 63, 63, 63, 63, 63, 32, 49, 50, 51, 32, 47, 110, 46
            };
#if JAVA
            int[] count = { 10, 20, 72, 10, 10 };
#else
            int[] count = { 6, 22, 37, 10, 11 };
#endif
            //UNICODE
            Encoding encoding = Encoding.Unicode;
            Assert.That(count[0], Is.EqualTo(encoding.GetMaxCharCount(10)));
            Assert.That(count[1], Is.EqualTo(encoding.GetMaxByteCount(10)));

            int bytesConsumed = encoding.GetMaxCharCount(unicodeBytes.Length);
            Assert.That(count[2], Is.EqualTo(bytesConsumed));
            char[] chars = new char[bytesConsumed];
            chars = encoding.GetChars(unicodeBytes);
            string strUnicode = new string(chars);
            Assert.That(testStrUnicode, Is.EqualTo(strUnicode));

            //WINDOWS_1252
            encoding = Encoding.GetEncoding("Windows-1252");
            Assert.That(count[3], Is.EqualTo(encoding.GetMaxCharCount(10)));
            Assert.That(count[4], Is.EqualTo(encoding.GetMaxByteCount(10)));

            bytesConsumed = encoding.GetMaxCharCount(win1252Bytes.Length);
            Assert.That(win1252Bytes.Length, Is.EqualTo(bytesConsumed));
            chars = encoding.GetChars(win1252Bytes);
            string strWin1252 = new string(chars);
            Assert.That(testStrWin1252, Is.EqualTo(strWin1252));
        }

        // Check that only one Encoding instance is created like in .NET.
        [Test]
        public void TestSingleInstance()
        {
            Encoding utf7 = Encoding.GetEncoding("utf-7");
            Assert.That(Encoding.GetEncoding("utf-7"), Is.SameAs(utf7));
            Assert.That(Encoding.GetEncoding(65000), Is.SameAs(utf7));

            Encoding unicode = Encoding.Unicode;
            Assert.That(Encoding.Unicode, Is.SameAs(unicode));
            Assert.That(Encoding.GetEncoding(1200), Is.SameAs(unicode));
        }

        [Test]
        public void TestGetPreamble()
        {
            // UTF 8
            Assert.That(Encoding.GetEncoding("utf-8").GetPreamble().Length, Is.EqualTo(3));
            Assert.That(new UTF8Encoding(true).GetPreamble().Length, Is.EqualTo(3));
            Assert.That(new UTF8Encoding(false).GetPreamble().Length, Is.EqualTo(0));

            // Unicode
            Assert.That(Encoding.Unicode.GetPreamble().Length, Is.EqualTo(2));
            Assert.That(Encoding.BigEndianUnicode.GetPreamble().Length, Is.EqualTo(2));
            Assert.That(Encoding.UTF32.GetPreamble().Length, Is.EqualTo(4));
            Assert.That(Encoding.GetEncoding("utf-32be").GetPreamble().Length, Is.EqualTo(4));

            // Other
            Assert.That(Encoding.GetEncoding(1252).GetPreamble().Length, Is.EqualTo(0));
        }

        [Test]
        public void TestJiraJ1459()
        {
            char[] chars = { (char)128, (char)129, (char)141, (char)142, (char)143, (char)144, (char)157, (char)158 };
            Encoding dstEncoding = Encoding.GetEncoding(1252);
            byte[] actualBytes = dstEncoding.GetBytes(chars);
            byte[] expectedBytes = { (byte)63, (byte)129, (byte)141, (byte)63, (byte)143, (byte)144, (byte)157, (byte)63 };
            // check that 128, 142 and 158 get replaced by '?', the rest stay the same
            Assert.That(actualBytes, Is.EqualTo(expectedBytes));
        }

        //>>>>>>>>>>>>>>>>>>>>>>>>>> Decoder

        [Test][JavaThrows(true)]
        public void TestDecoderGetChars()
        {
            string testStrUnicode = "Test String Тестовая строчка 123 /n.";
            string testStrWin1252 = "Test String ???????? ??????? 123 /n.";

            byte[] unicodeBytes =
            {
                84, 0, 101, 0, 115, 0, 116, 0, 32, 0, 83, 0, 116, 0, 114, 0, 105, 0, 110, 0, 103, 0,
                32, 0, 34, 4, 53, 4, 65, 4, 66, 4, 62, 4, 50, 4, 48, 4, 79, 4, 32, 0, 65, 4, 66, 4, 64, 4, 62, 4, 71, 4, 58, 4,
                48, 4, 32, 0, 49, 0, 50, 0, 51, 0, 32, 0, 47, 0, 110, 0, 46, 0
            };

            byte[] win1252Bytes =
            {
                84, 101, 115, 116, 32, 83, 116, 114, 105, 110, 103, 32, 63, 63, 63, 63, 63, 63, 63,
                63, 32, 63, 63, 63, 63, 63, 63, 63, 32, 49, 50, 51, 32, 47, 110, 46
            };

            Decoder decoder = Encoding.Unicode.GetDecoder();
            char[] chars = new char[Encoding.Unicode.GetMaxCharCount(unicodeBytes.Length)];
            decoder.GetChars(unicodeBytes, 0, unicodeBytes.Length, chars, 0);
            string unicodeStr = new string(chars).Trim('\0');
            Assert.That(testStrUnicode, Is.EqualTo(unicodeStr));

            decoder = Encoding.GetEncoding("Windows-1252").GetDecoder();
            chars = new char[Encoding.Default.GetMaxCharCount(win1252Bytes.Length)];
            decoder.GetChars(win1252Bytes, 0, win1252Bytes.Length, chars, 0);
            string wingStr = new string(chars).Trim();
            Assert.That(testStrWin1252, Is.EqualTo(wingStr));
        }

        [Test]
        public void TestJiraJ1859()
        {
            byte [] bytes = Encoding.GetEncoding(1252).GetBytes("14â€ Encouraged");
            string s = new string(Encoding.UTF8.GetChars(bytes));

            Assert.That("14” Encouraged", Is.EqualTo(s));
        }

        [Test]
        public void TestJiraJ1967()
        {
            Encoding encoding = Encoding.GetEncoding(1201);
            Assert.That(encoding.HeaderName, Is.EqualTo("utf-16BE"));
            Assert.That(encoding.CodePage, Is.EqualTo(1201));
            Assert.That(encoding.WebName, Is.EqualTo("utf-16BE"));
        }

        [Test]
        public void TestJava2326()
        {
            SystemPal.SaveCulture();
            SystemPal.SaveUICulture();
            try
            {
                SystemPal.SetStandardCulture();
                SystemPal.SetStandardUICulture();

                const string encodingName = "IBM290";
#if JAVA
            Encoding e = Encoding.fromJava(java.nio.charset.Charset.forName(encodingName));
#else
                Encoding e = Encoding.GetEncoding(encodingName);
                Assert.That(e.EncodingName, Is.EqualTo("IBM EBCDIC (Japanese katakana)"));
#endif
                Assert.That(e.HeaderName, Is.EqualTo("IBM290"));
                Assert.That(e.WebName, Is.EqualTo("IBM290"));
                Assert.That(e.GetPreamble().Length, Is.EqualTo(0));

            }
            finally
            {
                SystemPal.RestoreCulture();
                SystemPal.RestoreUICulture();
            }
        }
    }
}
