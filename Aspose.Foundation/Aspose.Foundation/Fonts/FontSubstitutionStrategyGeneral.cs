// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2016 by Victor Sotnikov

using System.Collections.Generic;
using Aspose.Fonts.TrueType;

namespace Aspose.Fonts
{
    /// <summary>
    /// Perform AW own font substitution algorithm, based on Panose and UnicodeRanges.
    /// </summary>
    /// <remarks>
    /// To find best substitution, the distance to requested fontInfo is calculated for all available fonts.
    /// The closest font is considered the best fit.
    ///
    /// Following considerations are used when calculating distance:
    /// 1. The more Unicode ranges/codepages are missing, the farther the font.
    /// 2. Missing range is more important then PANOSE match. I.e. font with one missing range and full PANOSE
    /// match is farther then font with all supported ranges and full PANOSE mismatch.
    /// 3. PANOSE values meaning depend on FamilyKind value. There is no point to match other PANOSE values
    /// on FamilyKind mismatch. Thus font with FamilyKind mismatch is farther then font with FamilyKind match
    /// and all other values mismatch.
    /// 4. PANOSE value match with Any value is better then mismatch but worser then exact match.
    ///
    /// Possible improvements:
    /// 1. Some of the PANOSE values could be considered more important then others.
    /// 2. Some PANOSE values could be compared with each other rather then simply checking the exact match
    /// (like Weight, Contrast). Though some values can't be compared (like Decorative Class).
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal class FontSubstitutionStrategyGeneral : IFontSubstitutionStrategy
    {
        /// <summary>
        /// Performs font substitution.
        /// </summary>
        public string GetSubstitution(FontSubstitutionInfo fontInfo, IEnumerable<FontSearchInfo> availableFonts)
        {
            FontPanose panose = PreparePanose(fontInfo);
            FontCodepageRanges codepages = PrepareCodepages(fontInfo);

            FontSearchInfo currentMatch = null;
            int currentMatchDistance = int.MaxValue;
            foreach (FontSearchInfo searchInfo in availableFonts)
            {
                int distance = CalculateDistance(panose, fontInfo.UnicodeRanges, codepages, searchInfo);

                if (distance < currentMatchDistance)
                {
                    currentMatch = searchInfo;
                    currentMatchDistance = distance;

                    if (distance == 0)
                        break;
                }
            }

            return currentMatch != null
                ? currentMatch.FontFamilyName
                : null;
        }

        private static FontPanose PreparePanose(FontSubstitutionInfo fontInfo)
        {
            if (fontInfo.Panose.Values[0] != FontPanose.ValueAny)
                return fontInfo.Panose;

            // In this case MW uses standard fonts. See FontSubstitutionStrategyMsWordSpecific.
            // This code will run when standard fonts are not available. Just use panose of these fonts.
            // Note: I've used full PANOSE to get closer substitution to MW.
            switch (fontInfo.Family)
            {
                case FontFamilyCore.Swiss:
                case FontFamilyCore.Modern:
                    if(fontInfo.Pitch == FontPitchCore.Fixed)
                        return new FontPanose(new byte[] { 2, 11, 6, 9, 7, 2, 5, 8, 2, 4 }); // MS Mincho
                    return new FontPanose(new byte[] { 2, 11, 6, 4, 2, 2, 2, 2, 2, 4 }); // Times New Roman
                case FontFamilyCore.Auto:
                case FontFamilyCore.Roman:
                    if (fontInfo.Pitch == FontPitchCore.Fixed)
                        return new FontPanose(new byte[] { 2, 2, 6, 9, 4, 2, 5, 8, 3, 4 }); // MS Gothic
                    return new FontPanose(new byte[] { 2, 2, 6, 3, 5, 4, 5, 2, 3, 4 }); // Arial
                case FontFamilyCore.Decorative:
                    return new FontPanose(new byte[] { 4, 4, 6, 5, 5, 16, 2, 2, 13, 2 }); // Gabriola
                case FontFamilyCore.Script:
                    return new FontPanose(new byte[] { 3, 7, 4, 2, 5, 3, 2, 3, 2, 3 }); // Bradley Hand ITC
                default:
                    return fontInfo.Panose;
            }
        }

        private static FontCodepageRanges PrepareCodepages(FontSubstitutionInfo fontInfo)
        {
            // Charset in fontInfo is int. Check the value before using for codepage.
            if (fontInfo.Charset < 0 || fontInfo.Charset > 0xFF)
                return fontInfo.CodepageRanges;

            return fontInfo.CodepageRanges.AddCharset((byte)fontInfo.Charset);
        }

        private static int CalculateDistance(FontPanose panose, FontUnicodeRanges unicodeRanges, FontCodepageRanges codepages,
            FontSearchInfo searchInfo)
        {
            return GetNumberOfMissingRanges(unicodeRanges, codepages, searchInfo) * MissingRangeWeight +
                   GetPanoseDistanse(panose, searchInfo.Panose) + GetSymbolicDistance(panose, codepages, searchInfo);
        }

        private static int GetSymbolicDistance(FontPanose panose, FontCodepageRanges codepages, FontSearchInfo searchInfo)
        {
            // Regular fonts should not be substituted with symbolic fonts and vise versa. So use the distance high enough.
            // Note: Panose may be inaccurate/invalid so to be sure check the other fields AW uses to determine symbolic font.
            // Currently AW uses symbolic codepage and 'cmap' symbolic encoding record. Use only codepage for now.
            bool isSymbolic = panose.Values[0] == 5 || codepages.IsSymbolCharsetUsed;
            bool isSymbolicRequested = searchInfo.Panose.Values[0] == 5 || searchInfo.CodepageRanges.IsSymbolCharsetUsed;
            return isSymbolic ^ isSymbolicRequested
                ? SymbolFamilyMismatch
                : 0;
        }

        private static int GetNumberOfMissingRanges(FontUnicodeRanges unicodeRanges, FontCodepageRanges codepages,
            FontSearchInfo searchInfo)
        {
            FontUnicodeRanges actualRanges = searchInfo.UnicodeRanges;
            FontCodepageRanges actualCodepages = searchInfo.CodepageRanges;
            return GetMissingBits(unicodeRanges.Range1, actualRanges.Range1) +
                   GetMissingBits(unicodeRanges.Range2, actualRanges.Range2) +
                   GetMissingBits(unicodeRanges.Range3, actualRanges.Range3) +
                   GetMissingBits(unicodeRanges.Range4, actualRanges.Range4) +
                   GetMissingBits(codepages.Range1, actualCodepages.Range1) +
                   GetMissingBits(codepages.Range2, actualCodepages.Range2);
        }

        private static int GetMissingBits(uint expectedRanges, uint actualRanges)
        {
            // Return the number of ranges supported in expectedRanges and not supported in actualRanges.
            return BitUtil.CountBitsInt32((int)(expectedRanges & ~actualRanges));
        }

        private static int GetPanoseDistanse(FontPanose expected, FontPanose actual)
        {
            byte expectedFamilyKind = expected.Values[0];
            byte actualFamilyKind = actual.Values[0];

            // Values meaning depends on FamilyKind. Moreover for Any/NoFit FamilyKind values are not defined at all.
            // So do not match values on FamilyKind mismatch.
            if (expectedFamilyKind == FontPanose.ValueNoFit || actualFamilyKind == FontPanose.ValueNoFit)
                return PanoseFamilyKindMismatchWeigth;

            if (expectedFamilyKind == FontPanose.ValueAny || actualFamilyKind == FontPanose.ValueAny)
                return PanoseFamilyKindAnyMatchWeigth;

            if (expectedFamilyKind != actualFamilyKind)
                return PanoseFamilyKindMismatchWeigth;

            int distance = 0;
            for (int i = 1; i < 10; i++)
                distance += MatchPanoseValue(expected.Values[i], actual.Values[i]);

            return distance;
        }

        private static int MatchPanoseValue(byte expected, byte actual)
        {
            if (expected == FontPanose.ValueNoFit || actual == FontPanose.ValueNoFit)
                return PanoseValueMismatchWeigth;

            if (expected == FontPanose.ValueAny || actual == FontPanose.ValueAny)
                return PanoseValueAnyMatchWeigth;

            return expected == actual ? 0 : PanoseValueMismatchWeigth;
        }

        private const int PanoseValueAnyMatchWeigth = 1;
        private const int PanoseValueMismatchWeigth = 2;
        private const int PanoseFamilyKindAnyMatchWeigth = 20;
        private const int PanoseFamilyKindMismatchWeigth = 30;
        private const int MissingRangeWeight = 100;
        private const int SymbolFamilyMismatch = 100000;
    }
}
