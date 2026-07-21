// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/12/2008 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Drawing.Fonts;
using Aspose.Fonts.EmbeddedOpenType;
using Aspose.Fonts.EmbeddedOpenType.LzComp;
using Aspose.Fonts.TrueType;
using Aspose.IO;
using Aspose.JavaAttributes;

namespace Aspose.Fonts
{
    /// <summary>
    /// Basic font utility methods.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    public static class FontUtil
    {
        /// <summary>
        /// Default FontSize for Rtf format, measured in half points.
        /// </summary>
        public const int RtfDefaultFontSize = 24;

        /// <summary>
        /// Default character set of a font.
        /// </summary>
        public const int DefaultCharset = 1;

        /// <summary>
        /// ANSI character set of a font.
        /// </summary>
        public const int AnsiCharset = 0;

        /// <summary>
        /// Undefined character set of a font.
        /// </summary>
        public const int UndefinedCharset = -1;

        /// <summary>
        /// Returns True if specified font name belongs to known symbolic fonts.
        /// It is of course better to look at the font char set, but it is not always accurate.
        /// </summary>
        public static bool IsSymbolic(string fontName)
        {
            // WORDSNET-20983 Despite 'Segoe UI Symbol' font contains 'Symbol' in its name,
            // Word does not consider it as a symbolic font.
            // WORDSNET-26687 Similar issue with 'Noto Sans Symbols' font.
            if (fontName.Equals("Segoe UI Symbol", StringComparison.OrdinalIgnoreCase))
                return false;
            if (fontName.Equals("Noto Sans Symbols", StringComparison.OrdinalIgnoreCase))
                return false;

            return fontName.IndexOf("Symbol", 0, StringComparison.Ordinal) >= 0 ||
                   fontName.IndexOf("Webdings", 0, StringComparison.Ordinal) >= 0 ||
                   fontName.IndexOf("Wingdings", 0, StringComparison.Ordinal) >= 0 ||
                   fontName.IndexOf("Dingbats", 0, StringComparison.Ordinal) >= 0;
        }

        public static string ValidatePostscriptName(string postscriptName)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in postscriptName)
                if (IsPostscriptCharAllowed(c))
                    builder.Append(c);
            return builder.ToString();
        }

        private static bool IsPostscriptCharAllowed(char c)
        {
            return
                c >= 33 && c <= 126 &&
                c != '[' && c != ']' && c != '(' && c != ')' && c != '{' &&
                c != '}' && c != '<' && c != '>' && c != '/' && c != '%';
        }

        /// <summary>
        /// If the Unicode character is in the Private Use Area 0xF020-0xF0FF, converts it into 0x0020 - 0x00FF.
        /// This is needed when converting from Unicode characters formatted using the Symbol font into Symbol encoding.
        /// There is no code page for Symbol encoding in .NET, therefore one needs to use this method.
        /// </summary>
        public static char UnicodeToSymbol(char c)
        {
            return IsPrivateUnicode(c) ? (char)(c - 0xF000) : c;
        }

        /// <summary>
        /// Converts a character encoded using the Symbol encoding into a Unicode character.
        /// See comments above.
        /// </summary>
        public static char SymbolToUnicode(char c)
        {
            return IsSymbol(c) ? (char)(c + 0xF000) : c;
        }

        /// <summary>
        /// Converts a character encoded using Wingding font in Private Use Area
        /// to analogue Unicode character from some other Unicode area.
        /// </summary>
        public static char WingdingToUnicode(char c)
        {
            switch (c)
            {
                case '':
                    return '✓';

                default:
                    return c;
            }
        }

        /// <summary>
        /// Returns True if specified character is in the Private Use Area (0xF020-0xF0FF).
        /// </summary>
        public static bool IsPrivateUnicode(char c)
        {
            return ((c >= 0xF020) && (c <= 0xF0FF));
        }

        public static bool IsSymbol(char c)
        {
            return ((c >= 0x0020) && (c <= 0x00FF));
        }


        /// <summary>
        /// Gets charset from fontName for some specific font names.
        /// </summary>
        /// <param name="fontName">Font Name</param>
        /// <returns>Charset value. Zero if fontName is not on a list.</returns>
        public static int CharsetFromFontName(string fontName)
        {
            // Charset suffixes comparison is case sensitive. For example, MSW doesn't consider
            // CYR or TUR suffixes as a charset suffixes.
            string suffix = fontName.Substring(fontName.LastIndexOf(" ", StringComparison.Ordinal) + 1);

            if (gSuffixCharsetMap.ContainsKey(suffix))
            {
                return gSuffixCharsetMap[suffix];
            }
            else
            {
                switch (suffix.ToUpper())
                {
                    case "BATANG":
                        return 129;
                    case "明朝":
                        return 128;
                    default:
                        return UndefinedCharset;
                }
            }
        }

        /// <summary>
        /// Gets character set resolved by either IANA or Font name.
        /// </summary>
        /// <returns><see cref="UndefinedCharset"/> if cannot be resolved.</returns>
        public static int ResolveCharset(string ianaName, string fontName)
        {
            // Try to resolve by IANA.
            int charset = CharsetByIanaName(ianaName);
            if (charset != UndefinedCharset)
                return charset;

            // Try to resolve by font name.
            return CharsetFromFontName(fontName);
        }

        /// <summary>
        /// Gets character set by IANA name/aliases.
        /// </summary>
        /// <remarks>
        /// The Internet Assigned Numbers Authority (IANA)
        /// is responsible for the global coordination of the DNS Root, IP addressing,
        /// and other Internet protocol resources.
        /// http://www.iana.org/assignments/character-sets/character-sets.xml
        /// </remarks>
        private static int CharsetByIanaName(string ianaName)
        {
            if (StringUtil.HasChars(ianaName))
            {
                int charset = gCharsetIanaNameMap.TryGetValue(ianaName);
                if (!StringToIntBidirectionalMap.IsNullSubstitute(charset))
                    return charset;

                // Leave this just in case. For example, Hangul character in MS Word is ks_c-5601-1987, but
                // in the Encoding class and at the site above it is ks_c_5601-1987. MS Word can read the both values.
                // Maybe other office applications use different values.
                // Also, it may be needed when IANA name will be public.
                Encoding encoding = TryGetEncoding(ianaName);
                if (encoding != null)
                    return CharsetFromCodePage(encoding.WindowsCodePage);
            }

            return UndefinedCharset;
        }

        /// <summary>
        /// Gets IANA name by charset code.
        /// </summary>
        public static string IanaNameByCharset(int charset)
        {
            return gCharsetIanaNameMap.GetValue(charset, "");
        }

        /// <summary>
        /// Get charset decimal value by code page.
        /// </summary>
        /// <returns>Charset value. Zero if code page is not on a list.</returns>
        public static int CharsetFromCodePage(int codePage)
        {
            int val = gCharsetFromCodePage[codePage];
            if (!IntToIntDictionary.IsNullSubstitute(val))
                return val;

            // Zero it is ANSI charset. MS Word 2013 writes it by default upon writing strict OOXML.
            return 0;
        }

        /// <summary>
        /// Detects if a fontName is an Asian font name or not.
        /// </summary>
        public static bool IsAsianFont(IFontProvider fontProvider, string fontName)
        {
            // WORDSNET-26956 First check if this is well-known EastAsian font.
            if (ArrayUtil.Contains(gEastAsianFonts, fontName))
                return true;

            TTFont font = fontProvider.FetchTTFont(fontName, FontStyle.Regular);
            return font.IsCjkMetrics;
        }

        /// <summary>
        /// Detects if a fontName is an Asian font name or not.
        /// </summary>
        public static bool IsBoldSimulationAllowedOnSemibold(string familyName)
        {
            // WORDSNET-28920 Experiments shows that MW allows bold simulation for some specific well-known SemiBold fonts
            // and disallow for all others.
            return ArrayUtil.Contains(gBoldSimulationAllowedOnSemibold, familyName);
        }

        /// <summary>
        /// Detects if a fontName is a Common System font name or not.
        /// </summary>
        public static bool IsCommonSystemFont(string fontName)
        {
            return (Array.IndexOf(gCommonSystemFonts, fontName) != -1);
        }

        /// <summary>
        /// Detects if the specified fontName is a monospaced font's name.
        /// </summary>
        public static bool IsMonospacedFont(string fontName)
        {
            return (Array.IndexOf(gMonospacedFonts, fontName) != -1);
        }

        /// <summary>
        /// Reads the last resort font 'Fanwood' that is embedded into assembly resources.
        /// </summary>
        [JavaConvertCheckedExceptions]
        public static TTFont LoadLastResortFontFromResources()
        {
            using (Stream stream = SystemPal.FetchResourceStream("Aspose.Resources.Fanwood.ttf"))
            {
                byte[] data = StreamUtil.CopyStreamToByteArray(stream);
                return TTFontBuilder.Read(new PhysicalFontData(new MemoryFontData(data)));
            }
        }

        private static Encoding TryGetEncoding(string ianaName)
        {
            try
            {
                return Encoding.GetEncoding(ianaName);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        static FontUtil()
        {
            FillCharsetFromCodePageMap();
            FillCharsetIanaNameMap();
            FillSuffixCharsetMap();
            FillFontSubstitutesCache();
        }

        /// <summary>
        /// Fills dictionary that allows getting charset code from code page.
        /// </summary>
        private static void FillCharsetFromCodePageMap()
        {
            //ANSI charset.
            gCharsetFromCodePage.Add(1252, 0);

            //SHIFTJIS charset.
            gCharsetFromCodePage.Add(932, 128);

            //HANGUL charset.
            gCharsetFromCodePage.Add(949, 129);

            //GB2312 charset.
            gCharsetFromCodePage.Add(936, 134);

            //CHINESEBIG5 charset.
            gCharsetFromCodePage.Add(950, 136);

            //GREEK charset.
            gCharsetFromCodePage.Add(1253, 161);

            //TURKISH charset.
            gCharsetFromCodePage.Add(1254, 162);

            //Vietnamese charset.
            gCharsetFromCodePage.Add(1258, 163);

            //HEBREW charset.
            gCharsetFromCodePage.Add(1255, 177);

            //ARABIC charset.
            gCharsetFromCodePage.Add(1256, 178);

            //BALTIC charset.
            gCharsetFromCodePage.Add(1257, 186);

            //RUSSIAN charset.
            gCharsetFromCodePage.Add(1251, 204);

            //THAI charset.
            gCharsetFromCodePage.Add(874, 222);

            //EE charset.
            gCharsetFromCodePage.Add(1250, 238);
        }

        /// <summary>
        /// Fills map between IANA name and charset code.
        /// </summary>
        private static void FillCharsetIanaNameMap()
        {
            //ANSI charset.
            gCharsetIanaNameMap.AddEntry(0, "iso-8859-1");

            //Default charset (1) is skipped now. MS Word converts it to system default charset.

            //Symbol charset (2) is skipped. MS Word does not write charset for such font.

            //Macintosh (Standard Roman) charset.
            gCharsetIanaNameMap.AddEntry(0x4D, "macintosh");

            //SHIFTJIS charset.
            gCharsetIanaNameMap.AddEntry(0x80, "shift_jis");

            //HANGUL charset.
            gCharsetIanaNameMap.AddEntry(0x81, "ks_c-5601-1987");

            //Johab charset. IANA name is taken from §21.1.2.3.1 of ISO/IEC 29500:1.
            gCharsetIanaNameMap.AddEntry(0x82, "KS C-5601-1992");

            //GB2312 charset.
            gCharsetIanaNameMap.AddEntry(0x86, "GBK");

            //CHINESEBIG5 charset.
            gCharsetIanaNameMap.AddEntry(0x88, "Big5");

            //GREEK charset.
            gCharsetIanaNameMap.AddEntry(0xA1, "windows-1253");

            //TURKISH charset.
            gCharsetIanaNameMap.AddEntry(0xA2, "iso-8859-9");

            //Vietnamese charset.
            gCharsetIanaNameMap.AddEntry(0xA3, "windows-1258");

            //HEBREW charset.
            gCharsetIanaNameMap.AddEntry(0xB1, "windows-1255");

            //ARABIC charset.
            gCharsetIanaNameMap.AddEntry(0xB2, "windows-1256");

            //BALTIC charset.
            gCharsetIanaNameMap.AddEntry(0xBA, "windows-1257");

            //RUSSIAN charset.
            gCharsetIanaNameMap.AddEntry(0xCC, "windows-1251");

            //THAI charset.
            gCharsetIanaNameMap.AddEntry(0xDE, "windows-874");

            //EE charset.
            gCharsetIanaNameMap.AddEntry(0xEE, "windows-1250");

            //OEM charset (0xFF) is skipped.
        }

        /// <summary>
        /// Fills map between font name script suffix and charset code.
        /// </summary>
        private static void FillSuffixCharsetMap()
        {
            gSuffixCharsetMap.Add("(Arabic)", 178);
            gSuffixCharsetMap.Add("CE", 238);
            gSuffixCharsetMap.Add("Cyr", 204);
            gSuffixCharsetMap.Add("(Vietnamese)", 163);
            gSuffixCharsetMap.Add("Western",0);
            gSuffixCharsetMap.Add("(Hebrew)", 177);
            gSuffixCharsetMap.Add("Tur", 162);
            gSuffixCharsetMap.Add("Greek", 161);
            gSuffixCharsetMap.Add("Baltic", 186);
            gSuffixCharsetMap.Add("(Thai)", 222);
            // WORDSNET-28233 Added suffix for Japanese fonts.
            gSuffixCharsetMap.Add("JP", 128);
        }

        /// <summary>
        /// Minimal allowed by MSWord Font Size measured in half points.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int MinAllowedFontSize = 2;

        /// <summary>
        /// MS Word treats these fonts as common system fonts.
        /// </summary>
        /// <remarks>
        /// For example, MS Word does not embed these fonts if corresponded option is specified in its GUI.
        /// </remarks>
        private static readonly string[] gCommonSystemFonts = new string[] { "Arial", "Times New Roman", "Courier New", "Symbol", "Wingdings" };

        /// <summary>
        /// List of monospaced fonts' names.
        /// </summary>
        private static readonly string[] gMonospacedFonts = new string[] { "Consolas", "Courier New", "Lucida Console", "DotumChe" };

        /// <summary>
        /// The list of fonts that allow kerning to disable the Allow Hanging Punctuation rule.
        /// </summary>
        private static readonly string[] gFontsAllowingKerningToDisableHangingPunctuation = new string[] { "Times New Roman",
            "Verdana", "Calibri", "Calibri Light", "Arial", "Arabic Typesetting", "Cambria" };

        /// <summary>
        /// The fonts' offset factors used for position of subscript glyphs.
        /// </summary>
        private static Dictionary<string, float> gFontSubscriptOffsetFactors;

        /// <summary>
        /// Default offset factor used to position subscript glyphs.
        /// </summary>
        private const float DefaultFontSubscriptOffsetFactor = 0.15f;

        /// <summary>
        /// Default offset factor used to position superscript glyphs.
        /// </summary>
        private const float DefaultFontSuperscriptOffsetFactor = -0.303f;

        private static readonly object gLockSubscriptFactor = new object();

        private static readonly IntToIntDictionary gCharsetFromCodePage = new IntToIntDictionary();
        private static readonly StringToIntBidirectionalMap gCharsetIanaNameMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntDictionary gSuffixCharsetMap = new StringToIntDictionary();


        /// <summary>
        /// Returns trimmed family name which suits GDI+ family name parsing behavior.
        /// </summary>
        public static string TrimFamilyNameForGdi(string familyName)
        {
            const int maxGdiFamilyNameLength = 31;
            return familyName.Length <= maxGdiFamilyNameLength
                ? familyName
                : familyName.Substring(0, maxGdiFamilyNameLength);
        }

        /// <summary>
        /// Initializes a dictionary that contains the offset factors for positioning subscript glyphs.
        /// </summary>
        private static void InitializeSubscriptFactors()
        {
            if (gFontSubscriptOffsetFactors == null)
            {
                lock (gLockSubscriptFactor)
                {
                    if (gFontSubscriptOffsetFactors == null)
                    {
                        gFontSubscriptOffsetFactors = new Dictionary<string, float>();
                        gFontSubscriptOffsetFactors.Add("Arial", 0.07f);
                        gFontSubscriptOffsetFactors.Add("Courier New", 0.105f);
                        gFontSubscriptOffsetFactors.Add("SimSun", 0.065f);
                        gFontSubscriptOffsetFactors.Add("Calibri", 0.1f);
                    }
                }
            }
        }

        public static StringToStringDictionary FontSubstitutesCache
        {
            get { return gFontSubstitutesCache; }
        }

        private static void FillFontSubstitutesCache()
        {
            gFontSubstitutesCache = new StringToStringDictionary(false);

            // Check the well known font substitutes here. Actually MW uses registry keys for this purpose but
            // predefined table seems to be OK.
            // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\FontSubstitutes
            // Also some of the substitutes from registry are set by default in TableSubstitutionRule.
            // The difference is that substitutes from here are checked before embedded font and AltName
            // and substitute from TableSubstitutionRule are checked after.
            gFontSubstitutesCache.Add("Courier", "Courier New");
            gFontSubstitutesCache.Add("David Transparent", "David");
            gFontSubstitutesCache.Add("Fixed Miriam Transparent", "Miriam Fixed");
            gFontSubstitutesCache.Add("Miriam Transparent", "Miriam");
            gFontSubstitutesCache.Add("Rod Transparent", "Rod");
            gFontSubstitutesCache.Add("Times", "Times New Roman");
        }

        private static StringToStringDictionary gFontSubstitutesCache;

        /// <summary>
        /// Limits size of data that should be encoded with MTX algorithm (<see cref="MtxCoder.Encode"/>).
        /// </summary>
        /// <remarks>
        /// IN. MTX encoding algorithm is quite slow due to the LZCOMP encoding (<see cref="LzCompEncoder"/>).
        /// And it looks like MS Word limits the size of data that is encoded with MTX algorithm to 4 Mb (found experimentally).
        /// If size is greater than 4 Mb, MS Word performs XOR encoding (<see cref="EotFontFiler.XorFontData"/>) only.
        /// </remarks>
        public const int MtxEncodeSizeLimit = 4 * (1024 * 1024); // 4 Mb.

        /// <summary>
        /// Limits size of font that can be collected without subsetting.
        /// </summary>
        /// <remarks>
        /// IN. It seems, MS Word limits the font size for full embedding. If font size exceeds this value,
        /// it embeds only used characters, despite a save option is set to 'FullEmbedding'.
        /// This value is found experimentally, and it is something like <see cref="MtxEncodeSizeLimit"/>.
        /// </remarks>
        public const int FullEmbedSizeLimit = 4 * (1024 * 1024); // 4 Mb.

        /// <summary>
        /// The collection of well-known EastAsian fonts.
        /// </summary>
        private static readonly string[] gEastAsianFonts = new string[]
            { "ＭＳ ゴシック", "맑은 고딕", "宋体", "新細明體", "ＭＳ 明朝", "SimSun" };

        /// <summary>
        /// Seems that MW has predefined list of Windows font families for which bold simulation is allowed on Semibold style (weight = 600).
        /// </summary>
        private static readonly string[] gBoldSimulationAllowedOnSemibold = new string[]
            { "Georgia Pro Cond Semibold", "Georgia Pro Semibold", "Segoe UI Semibold", "Verdana Pro Cond Semibold", "Verdana Pro Semibold" };
    }
}
