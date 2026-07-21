// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Themes;

namespace Aspose.Words.ApsBuilder.Dml.Text.Layout
{
    /// <summary>
    /// Represents the repository of text fonts.
    /// </summary>
    internal class DmlTextFontRepository
    {
        /// <summary>
        /// WORDSNET-10615 In run properties font typeface is specified as "+mn-lt" (See TestJira10614).
        /// It seems it is abbreviation of MinorLatin theme font. So try to get actual font name from document theme.
        /// Actually, it would be more correct to get font name from ThemeOverride in case of Chart,
        /// but it is not very convenient with current structure, so use main document's theme.
        /// In most cases this gives correct result, will change this if customers complain.
        /// </summary>
        internal static string GetTypeFace(string typeFace, Theme theme)
        {
            if (theme == null)
            {
                // WORDSNET-19779 If theme is not specified, and latin typeface is "+mn-lt", MS Word uses "Calibri"
                // as default typeface.
                typeFace = (typeFace == TypeFaceMinorAscii) ? DefaultTypeFace : typeFace;
                return typeFace;
            }

            IThemeProvider themeProvider = theme;

            switch (typeFace)
            {
                case TypeFaceMinorAscii:
                    return themeProvider.GetFontName(ThemeFontCore.MinorAscii);
                case TypeFaceMinorEastAsia:
                    return themeProvider.GetFontName(ThemeFontCore.MinorEastAsia);
                case TypeFaceMinorBidi:
                    return themeProvider.GetFontName(ThemeFontCore.MinorBidi);
                case TypeFaceMajorAscii:
                    return themeProvider.GetFontName(ThemeFontCore.MajorAscii);
                case TypeFaceMajorEastAsia:
                    return themeProvider.GetFontName(ThemeFontCore.MajorEastAsia);
                case TypeFaceMajorBidi:
                    return themeProvider.GetFontName(ThemeFontCore.MajorBidi);
                default:
                    return typeFace;
            }
        }

        private const string TypeFaceMinorAscii = "+mn-lt";
        private const string TypeFaceMinorEastAsia = "+mn-ea";
        private const string TypeFaceMinorBidi = "+mn-cs";
        private const string TypeFaceMajorAscii = "+mj-lt";
        private const string TypeFaceMajorEastAsia = "+mj-ea";
        private const string TypeFaceMajorBidi = "+mj-cs";
        private const string DefaultTypeFace = "Calibri";
    }
}
