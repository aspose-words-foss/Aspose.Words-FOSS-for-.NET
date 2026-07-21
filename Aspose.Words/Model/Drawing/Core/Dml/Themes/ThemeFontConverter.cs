// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2015 by Andrey Noskov

using System;

namespace Aspose.Words.Drawing.Core.Dml.Themes
{
    /// <summary>
    /// Converts ThemeFont enumeration values to string.
    /// </summary>
    internal static class ThemeFontConverter
    {
        internal static string ToString(ThemeFontCore enumValue)
        {
            switch (enumValue)
            {
                case ThemeFontCore.None:
                    return "None";

                case ThemeFontCore.MajorAscii:
                    return "MajorAscii";
                case ThemeFontCore.MajorBidi:
                    return "MajorBidi";
                case ThemeFontCore.MajorEastAsia:
                    return "MajorEastAsia";
                case ThemeFontCore.MajorHAnsi:
                    return "MajorHAnsi";

                case ThemeFontCore.MinorAscii:
                    return "MinorAscii";
                case ThemeFontCore.MinorBidi:
                    return "MinorBidi";
                case ThemeFontCore.MinorEastAsia:
                    return "MinorEastAsia";
                case ThemeFontCore.MinorHAnsi:
                    return "MinorHAnsi";


                case ThemeFontCore.GroupMask:
                    return "GroupMask";
                case ThemeFontCore.Major:
                    return "Major";
                case ThemeFontCore.Minor:
                    return "Minor";

                case ThemeFontCore.RegionMask:
                    return "RegionMask";
                case ThemeFontCore.Ascii:
                    return "Ascii";
                case ThemeFontCore.Bidi:
                    return "Bidi";
                case ThemeFontCore.EastAsia:
                    return "EastAsia";
                case ThemeFontCore.HAnsi:
                    return "HAnsi";

                default:
                    throw new ArgumentOutOfRangeException("Unknown ThemeFont value, please update ThemeFontHelper.ToString().");
            }
        }
    }
}
