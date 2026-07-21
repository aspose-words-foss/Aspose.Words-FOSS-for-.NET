// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2016 by Konstantin Kornilov

using System.Collections.Generic;
using Aspose.Fonts.TrueType;

namespace Aspose.Fonts
{
    /// <summary>
    /// Strategy implementation that handles specific cases of MW substitution which don't fit to the general substitution algorithm.
    /// </summary>
    /// <remarks>
    /// Font substitution differs in MW13 and MW16. This class simulates MW16 logic.
    /// </remarks>
    public class FontSubstitutionStrategyMsWordSpecific : IFontSubstitutionStrategy
    {
        public string GetSubstitution(FontSubstitutionInfo fontInfo, IEnumerable<FontSearchInfo> availableFonts)
        {
            // Cases with valid PANOSE should be handled by general algorithm.
            if (IsPanoseValid(fontInfo.Panose))
                return null;

            // Seems that MW returns "Courier New" font if PANOSE is invalid but not exactly empty.
            if (!IsPanoseEmpty(fontInfo.Panose))
                return "Courier New";

            if (fontInfo.Charset == 2)
                return GetSymbolSubstitution(fontInfo);

            if (fontInfo.CodepageRanges.Range1 == FontCodepageRanges.Range1Symbol)
                return "Symbol";

            if (fontInfo.CodepageRanges.Range1 == FontCodepageRanges.Range1Ansi)
                return GetLatinSubstitution(fontInfo);

            return GetRegularSubstitution(fontInfo);
        }

        private static bool IsPanoseValid(FontPanose panose)
        {
            return !(panose.Values[0] == FontPanose.ValueAny || panose.Values[0] == FontPanose.ValueNoFit);
        }

        private static bool IsPanoseEmpty(FontPanose panose)
        {
            foreach (byte b in panose.Values)
                if (b != 0x00)
                    return false;

            return true;
        }

        private static string GetRegularSubstitution(FontSubstitutionInfo info)
        {
            // Note: It seems that MW also handles Codepages/Unicode ranges in this case.
            // AW currently do not.
            switch (info.Family)
            {
                case FontFamilyCore.Auto:
                case FontFamilyCore.Roman:
                    return info.Pitch == FontPitchCore.Fixed
                        ? "MS Mincho"
                        : "Times New Roman";
                case FontFamilyCore.Swiss:
                case FontFamilyCore.Modern:
                    return info.Pitch == FontPitchCore.Fixed
                        ? "MS Gothic"
                        : "Arial";
                case FontFamilyCore.Decorative:
                    return "Gabriola";
                case FontFamilyCore.Script:
                    return "Bradley Hand ITC";
                default:
                    Debug.Fail("Unexpected font family.");
                    return null;
            }
        }

        private static string GetLatinSubstitution(FontSubstitutionInfo info)
        {
            switch (info.Family)
            {
                case FontFamilyCore.Auto:
                case FontFamilyCore.Decorative:
                case FontFamilyCore.Script:
                case FontFamilyCore.Swiss:
                    return "Arial";
                case FontFamilyCore.Roman:
                    return "Times New Roman";
                case FontFamilyCore.Modern:
                    return "Courier New";
                default:
                    Debug.Fail("Unexpected font family.");
                    return null;
            }
        }

        private static string GetSymbolSubstitution(FontSubstitutionInfo info)
        {
            // Obtained from experiments.
            return info.Family == FontFamilyCore.Roman
                ? "Symbol"
                : "Wingdings";
        }
    }
}
