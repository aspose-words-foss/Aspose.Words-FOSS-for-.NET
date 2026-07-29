// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/09/2008 by Roman Korchagin

using System.Collections.Generic;
using System.Text;
using Aspose.Collections.Generic;

namespace Aspose.Charset
{
    /// <summary>
    /// Constants and utility methods for code pages and font character sets.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    public static class CodePage
    { 

        public const int SymbolCharSet = 2;

        public const int HebrewCharSet = 177;
        public const int ArabicCharSet = 178;

        /// <summary>
        /// 437 MS-DOS Latin US
        /// </summary>
        public const int DosLatinUSCodePage = 437;
        /// <summary>
        /// 850 MS-DOS Latin 1
        /// </summary>
        public const int DosLatin1CodePage = 850;
        /// <summary>
        /// 1200
        /// </summary>
        public const int CodePageUtf16 = 1200;
        /// <summary>
        /// 1201
        /// </summary>
        public const int CodePageUtf16BE = 1201;
        /// <summary>
        /// 1252 Windows ANSI code page.
        /// </summary>
        public const int WindowsLatin1CodePage = 1252;
        /// <summary>
        /// 1255 Windows Hebrew code page.
        /// </summary>
        public const int HebrewCodePage = 1255;
        /// <summary>
        /// 1256 Windows Arabic code page. 
        /// </summary>
        public const int ArabicCodePage = 1256;
        /// <summary>
        /// 10000
        /// </summary>
        public const int MacintoshRomanCodePage = 10000;
        /// <summary>
        /// 12000
        /// </summary>
        public const int CodePageUtf32 = 12000;
        /// <summary>
        /// 12001
        /// </summary>
        public const int CodePageUtf32BE = 12001;
        /// <summary>
        /// 65000
        /// </summary>
        public const int CodePageUtf7 = 65000;
        /// <summary>
        /// 65001
        /// </summary>
        public const int CodePageUtf8 = 65001;

        // Few national encodings used in auto detection of files in unknown encoding.
        // Japanese
        public const int WindowsJapaneseShiftJis = 932;
        // Chinese
        public const int OfficialChineseGb18030 = 54936;
        public const int WindowsChineseGb2312 = 936;
        // Korean
        public const int WindowsKoreanKs_c_5601_1987 = 949;
        // Russian
        public const int WindowsCyrillic1251 = 1251;
        public const int OldCyrillicKOI8R = 20866;

        public const int Unknown = int.MinValue;

        /// <summary>
        /// Returns the default value if cannot find a mapping.
        /// </summary>
        public static int CharSetToCodePage(int charSet, int defaultCodePage)
        {
            return (gCharSetMap.ContainsKey(charSet))
                ? gCharSetMap[charSet]
                : defaultCodePage;
        }

        /// <summary>
        /// Returns true if specified encoding is a known single-byte encoding.
        /// </summary>
        public static bool IsKnownSingleByteEncoding(Encoding encoding)
        {
            return gSingleByteEncodings.Contains(encoding.CodePage);
        }

        /// <summary>
        /// Returns <c>true</c> if the specified code page is a code page of a right-to-left language.
        /// </summary>
        public static bool IsRightToLeft(int codePage)
        {
            return (codePage == HebrewCodePage) || (codePage == ArabicCodePage);
        }

        /// <summary>
        /// Returns <c>true</c> if the specified charset is a charset of a right-to-left language.
        /// </summary>
        public static bool IsRightToLeftCharset(int charset)
        {
            return (charset == HebrewCharSet) || (charset == ArabicCharSet);
        }

        /// <summary>
        /// Maps Language to ANSI code page.
        /// </summary>
        /// <remarks>
        /// See https://www.microsoft.com/resources/msdn/goglobal/default.mspx?OS=Windows%20Vista
        /// </remarks>
        public static int LidToCodePage(int lid)
        {
            switch ((Language) lid)
            {
                case Language.Hebrew:
                case Language.HebrewIsrael:
                    return 1255;

                case Language.RussianRussia:
                    return WindowsCyrillic1251;

                case Language.GreekGreece:
                    return 1253;

                default:
                    // Default code page we used to read non Unicode text.
                    return 1252;
            }
        }

        /// <summary>
        /// .NET doesn't define UTF-7 BOM (signature) and doesn't cut it from stream.
        /// We have to skip it manually.
        /// Seen here: http://blogs.msdn.com/oldnewthing/archive/2004/03/24/95235.aspx
        /// See also: http://en.wikipedia.org/wiki/Byte-order_mark
        /// UTF-7 can have 5 different BOMs. And 4 of them depend on the first data (non-BOM) byte.
        /// I've never seen samples of 4-byte BOMS so supported just this one.
        /// UTF-7 itself is pretty rare and archaic.
        /// </summary>
        public static readonly byte[] Utf7Bom = new byte[] { 0x2B, 0x2F, 0x76, 0x38, 0x2D };
        /// <summary>
        /// UTF-8
        /// </summary>
        public static readonly byte[] Utf8Bom = new byte[] { 0xEF, 0xBB, 0xBF };
        /// <summary>
        /// UTF-16, Little Endian
        /// </summary>
        public static readonly byte[] Utf16LeBom = new byte[] { 0xFF, 0xFE };
        /// <summary>
        /// UTF-16, Big Endian
        /// </summary>
        public static readonly byte[] Utf16BeBom = new byte[] { 0xFE, 0xFF };

        /// <summary>
        /// UTF-32, Little Endian
        /// </summary>
        public static readonly byte[] Utf32LeBom = new byte[] { 0xFF, 0xFE, 0x00, 0x00 };
        /// <summary>
        /// UTF-32, Big Endian
        /// </summary>
        public static readonly byte[] Utf32BeBom = new byte[] { 0x00, 0x00, 0xFE, 0xFF };

        /// <summary>
        /// Returns CodePage by BOM, or <see cref="Unknown"/> code page if a specified BOM is unknown. 
        /// </summary>
        public static int GetByBom(byte[] bom)
        {
            if (ArrayUtil.HasData(bom))
            {
                foreach (KeyValuePair<byte[], int> bomToCodePage in gBomToCodePage)
                {
                    byte[] knownBom = bomToCodePage.Key;
                    if (ArrayUtil.CompareBytes(bom, knownBom, knownBom.Length))
                        return bomToCodePage.Value;
                }
            }

            return Unknown;
        }

        /// <summary>
        /// Returns BOM from a specified buffer.
        /// </summary>
        public static byte[] GetBom(byte[] buffer)
        {
            Debug.Assert(buffer != null);

            foreach (byte[] knownBom in gBomToCodePage.Keys)
            {
                if (ArrayUtil.CompareBytes(knownBom, buffer, knownBom.Length))
                    return knownBom;
            }

            return ArrayUtil.EmptyByteArray;
        }

        
        static CodePage()
        {
            // Source: Word2007RTFSpec1.9.1.docx
            gCharSetMap.Add(0, WindowsLatin1CodePage);   // ANSI
            // There is no such code page in .NET. Use UnicodeToSymbol and SymbolToUnicode instead.
            // gCharsetMap.Add(SymbolCharSet, 42);     // Symbol
            gCharSetMap.Add(77, MacintoshRomanCodePage); // Mac Roman
            gCharSetMap.Add(78, 10001); // Mac Shift Jis
            gCharSetMap.Add(79, 10003); // Mac Hangul
            gCharSetMap.Add(80, 10008); // Mac GB2312
            gCharSetMap.Add(81, 10002); // Mac Big5
            gCharSetMap.Add(83, 10005); // Mac Hebrew
            gCharSetMap.Add(84, 10004); // Mac Arabic
            gCharSetMap.Add(85, 10006); // Mac Greek
            gCharSetMap.Add(86, 10081); // Mac Turkish
            gCharSetMap.Add(87, 10021); // Mac Thai
            gCharSetMap.Add(88, 10029); // Mac East Europe
            gCharSetMap.Add(89, 10007); // Mac Russian
            gCharSetMap.Add(128, WindowsJapaneseShiftJis);  // Shift JIS
            gCharSetMap.Add(129, 949);  // Hangul
            gCharSetMap.Add(130, 1361); // Johab
            gCharSetMap.Add(134, 936);  // GB2312
            gCharSetMap.Add(136, 950);  // Big5
            gCharSetMap.Add(161, 1253); // Greek
            gCharSetMap.Add(162, 1254); // Turkish
            gCharSetMap.Add(163, 1258); // Vietnamese
            gCharSetMap.Add(177, HebrewCodePage); // Hebrew
            gCharSetMap.Add(178, ArabicCodePage); // Arabic 
            gCharSetMap.Add(186, 1257); // Baltic
            gCharSetMap.Add(204, 1251); // Russian
            gCharSetMap.Add(222, 874);  // Thai
            gCharSetMap.Add(238, 1250); // Eastern European
            gCharSetMap.Add(254, DosLatinUSCodePage);  // PC 437
            gCharSetMap.Add(255, DosLatin1CodePage);  // OEM
            gCharSetMap.Add(65535, CodePageUtf16); 

            // The following list is obtained using Encoding.GetEncodings() method.
            // Also the list of encodings with IsSinleByte flag available on MSDN 
            // http://msdn.microsoft.com/en-us/library/system.text.encoding.issinglebyte.aspx 
            gSingleByteEncodings.Add(37);
            gSingleByteEncodings.Add(437);
            gSingleByteEncodings.Add(500);
            gSingleByteEncodings.Add(708);
            gSingleByteEncodings.Add(720);
            gSingleByteEncodings.Add(737);
            gSingleByteEncodings.Add(775);
            gSingleByteEncodings.Add(850);
            gSingleByteEncodings.Add(852);
            gSingleByteEncodings.Add(855);
            gSingleByteEncodings.Add(857);
            gSingleByteEncodings.Add(858);
            gSingleByteEncodings.Add(860);
            gSingleByteEncodings.Add(861);
            gSingleByteEncodings.Add(862);
            gSingleByteEncodings.Add(863);
            gSingleByteEncodings.Add(864);
            gSingleByteEncodings.Add(865);
            gSingleByteEncodings.Add(866);
            gSingleByteEncodings.Add(869);
            gSingleByteEncodings.Add(870);
            gSingleByteEncodings.Add(874);
            gSingleByteEncodings.Add(875);
            gSingleByteEncodings.Add(1026);
            gSingleByteEncodings.Add(1047);
            gSingleByteEncodings.Add(1140);
            gSingleByteEncodings.Add(1141);
            gSingleByteEncodings.Add(1142);
            gSingleByteEncodings.Add(1143);
            gSingleByteEncodings.Add(1144);
            gSingleByteEncodings.Add(1145);
            gSingleByteEncodings.Add(1146);
            gSingleByteEncodings.Add(1147);
            gSingleByteEncodings.Add(1148);
            gSingleByteEncodings.Add(1149);
            gSingleByteEncodings.Add(1250);
            gSingleByteEncodings.Add(1251);
            gSingleByteEncodings.Add(1252);
            gSingleByteEncodings.Add(1253);
            gSingleByteEncodings.Add(1254);
            gSingleByteEncodings.Add(1255);
            gSingleByteEncodings.Add(1256);
            gSingleByteEncodings.Add(1257);
            gSingleByteEncodings.Add(1258);
            gSingleByteEncodings.Add(10000);
            gSingleByteEncodings.Add(10004);
            gSingleByteEncodings.Add(10005);
            gSingleByteEncodings.Add(10006);
            gSingleByteEncodings.Add(10007);
            gSingleByteEncodings.Add(10010);
            gSingleByteEncodings.Add(10017);
            gSingleByteEncodings.Add(10021);
            gSingleByteEncodings.Add(10029);
            gSingleByteEncodings.Add(10079);
            gSingleByteEncodings.Add(10081);
            gSingleByteEncodings.Add(10082);
            gSingleByteEncodings.Add(20105);
            gSingleByteEncodings.Add(20106);
            gSingleByteEncodings.Add(20107);
            gSingleByteEncodings.Add(20108);
            gSingleByteEncodings.Add(20127);
            gSingleByteEncodings.Add(20269);
            gSingleByteEncodings.Add(20273);
            gSingleByteEncodings.Add(20277);
            gSingleByteEncodings.Add(20278);
            gSingleByteEncodings.Add(20280);
            gSingleByteEncodings.Add(20284);
            gSingleByteEncodings.Add(20285);
            gSingleByteEncodings.Add(20290);
            gSingleByteEncodings.Add(20297);
            gSingleByteEncodings.Add(20420);
            gSingleByteEncodings.Add(20423);
            gSingleByteEncodings.Add(20424);
            gSingleByteEncodings.Add(20833);
            gSingleByteEncodings.Add(20838);
            gSingleByteEncodings.Add(20866);
            gSingleByteEncodings.Add(20871);
            gSingleByteEncodings.Add(20880);
            gSingleByteEncodings.Add(20905);
            gSingleByteEncodings.Add(20924);
            gSingleByteEncodings.Add(21025);
            gSingleByteEncodings.Add(21866);
            gSingleByteEncodings.Add(28591);
            gSingleByteEncodings.Add(28592);
            gSingleByteEncodings.Add(28593);
            gSingleByteEncodings.Add(28594);
            gSingleByteEncodings.Add(28595);
            gSingleByteEncodings.Add(28596);
            gSingleByteEncodings.Add(28597);
            gSingleByteEncodings.Add(28598);
            gSingleByteEncodings.Add(28599);
            gSingleByteEncodings.Add(28603);
            gSingleByteEncodings.Add(28605);
            gSingleByteEncodings.Add(29001);
            gSingleByteEncodings.Add(38598);

            // The order is important for correct BOM detection.
            gBomToCodePage.Add(Utf8Bom, CodePageUtf8);
            gBomToCodePage.Add(Utf7Bom, CodePageUtf7);
            gBomToCodePage.Add(Utf32LeBom, CodePageUtf32);
            gBomToCodePage.Add(Utf32BeBom, CodePageUtf32BE);
            gBomToCodePage.Add(Utf16LeBom, CodePageUtf16);
            gBomToCodePage.Add(Utf16BeBom, CodePageUtf16BE);
        }

        /// <summary>
        /// A map of font character sets to the corresponding code pages.
        /// Key is integer font character set. Value is integer code page.
        /// </summary>
        private static readonly SortedIntegerListGeneric<int> gCharSetMap = new SortedIntegerListGeneric<int>();

        /// <summary>
        /// List of known single-byte encodings.
        /// </summary>
        private static readonly HashSetGeneric<int> gSingleByteEncodings = new HashSetGeneric<int>();

        /// <summary>
        /// Maps known BOMs to CodePages.
        /// </summary>
        private static readonly Dictionary<byte[], int> gBomToCodePage = new Dictionary<byte[], int>(6);
    }
}
