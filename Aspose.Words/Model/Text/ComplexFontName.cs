// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2013 by Alexey Morozov

using System;
using Aspose.Fonts;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Fonts;
using Aspose.Words.Themes;

namespace Aspose.Words
{
    /// <summary>
    /// Implements union of font represented as string and font represented as ThemeFont.
    /// </summary>
    internal class ComplexFontName : InternableComplexAttr, IComplexAttr
    {
        private ComplexFontName(string name)
        {
            if(!StringUtil.HasChars(name))
               throw new InvalidOperationException("Empty name.") ;

            mName = name;
        }

        private ComplexFontName(ThemeFontCore themeFontCore)
        {
            mThemeFontCore = themeFontCore;
        }

        /// <summary>
        /// Creates instance from its name.
        /// </summary>
        /// <param name="name">Font name. Can not be null.</param>
        internal static ComplexFontName FromName(string name)
        {
            // WORDSNET-23252 Seems Word always truncates too long font name.
            return new ComplexFontName(
                (name.Length > FontInfo.MaxFontNameLength)
                    ? name.Substring(0, FontInfo.MaxFontNameLength)
                    : name);
        }

        /// <summary>
        /// Creates instance from theme font name.
        /// </summary>
        internal static ComplexFontName FromTheme(ThemeFontCore themeFont)
        {
            return new ComplexFontName(themeFont);
        }

        /// <summary>
        /// Creates instance from theme font and region mask.
        /// </summary>
        internal static ComplexFontName FromTheme(ThemeFont themeFont, ThemeFontCore regionMask)
        {
            return new ComplexFontName(ToThemeFontCore(themeFont) | regionMask);
        }

        /// <summary>
        /// Returns resolved font name.
        /// </summary>
        internal string Resolve(Theme theme)
        {
            return Resolve(this, theme);
        }

        /// <summary>
        /// Returns resolved font name.
        /// </summary>
        internal static string Resolve(object fontNameObject, Theme theme)
        {
            return Resolve(fontNameObject, theme, RunPr.DefaultNameAscii);
        }

        /// <summary>
        /// Returns resolved font name.
        /// </summary>
        internal static string Resolve(object fontNameObject, Theme theme, string defaultFontName)
        {
            return Resolve(fontNameObject, theme, defaultFontName, RunPr.DefaultNameAscii);
        }

        /// <summary>
        /// Returns resolved font name.
        /// </summary>
        internal static string Resolve(object fontNameObject, Theme theme, string defaultFontName, string themeDefaultFontName)
        {
            if (fontNameObject == null)
                return null;

            ComplexFontName fontName = (ComplexFontName)fontNameObject;

            // Simple font name, return as is.
            if (!fontName.IsThemeFont)
                return fontName.Name;

            // Theme font but theme is null, return default "Times New Roman".
            if (theme == null)
                return themeDefaultFontName;

            string resolvedName = ((IThemeProvider)theme).GetFontName(fontName.ThemeFontCore);
            return StringUtil.HasChars(resolvedName) ? resolvedName : defaultFontName;
        }

        public override bool Equals(object obj)
        {
            return (obj is ComplexFontName) && (Equals(this, (ComplexFontName)obj));
        }

        public static bool Equals(ComplexFontName a, ComplexFontName b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (ReferenceEquals(null, a))
                return false;

            if (ReferenceEquals(null, b))
                return false;

            return string.Equals(a.mName, b.mName, StringComparison.OrdinalIgnoreCase) &&
                   (a.mThemeFontCore == b.mThemeFontCore);
        }

        public override int GetHashCode()
        {
            string name = mName != null
                ? mName.ToLowerInvariant()
                : null;
            return string.Format("{0}{1}", name, (int)mThemeFontCore).GetHashCode();
        }

        public override string ToString()
        {
            return IsThemeFont ? ThemeFontConverter.ToString(mThemeFontCore) : mName;
        }

        public IComplexAttr DeepCloneComplexAttr()
        {
            return (ComplexFontName)MemberwiseClone();
        }

        /// <summary>
        /// Converts specified <see cref="ThemeFont"/> value to a corresponding <see cref="ThemeFontCore"/> group mask.
        /// </summary>
        private static ThemeFontCore ToThemeFontCore(ThemeFont themeFont)
        {
            switch (themeFont)
            {
                case ThemeFont.Major:
                    return ThemeFontCore.Major;

                case ThemeFont.Minor:
                    return ThemeFontCore.Minor;

                default:
                    throw new ArgumentOutOfRangeException("themeFont");
            }
        }

        /// <summary>
        /// Converts specified <see cref="ThemeFontCore"/> value to a corresponding <see cref="ThemeFont"/>.
        /// </summary>
        private static ThemeFont ToThemeFont(ThemeFontCore themeFontCore)
        {
            switch (themeFontCore)
            {
                case ThemeFontCore.MajorAscii:
                case ThemeFontCore.MajorBidi:
                case ThemeFontCore.MajorEastAsia:
                case ThemeFontCore.MajorHAnsi:
                    return ThemeFont.Major;

                case ThemeFontCore.MinorAscii:
                case ThemeFontCore.MinorBidi:
                case ThemeFontCore.MinorEastAsia:
                case ThemeFontCore.MinorHAnsi:
                    return ThemeFont.Minor;

                case ThemeFontCore.None:
                    return ThemeFont.None;

                default:
                    throw new ArgumentOutOfRangeException("themeFontCore");
            }
        }

        internal string Name
        {
            get
            {
                if(IsThemeFont)
                    throw new InvalidOperationException("Inaccessible because font is defined by theme font.");
                return mName;
            }
        }

        /// <summary>
        /// Gets Theme font name.
        /// </summary>
        internal ThemeFontCore ThemeFontCore
        {
            get
            {
                if(!IsThemeFont)
                    throw new InvalidOperationException("Inaccessible because font is defined by font name.");
                return mThemeFontCore;
            }
        }

        /// <summary>
        /// Gets Theme font name.
        /// </summary>
        internal ThemeFont ThemeFont
        {
            get
            {
                if (IsThemeFont)
                    return ToThemeFont(ThemeFontCore);

                return ThemeFont.None;
            }
        }

        internal bool IsThemeFont
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return (mThemeFontCore != ThemeFontCore.None) && !StringUtil.HasChars(mName); }
        }

        /// <summary>
        /// Detects if a font is Asian or not.
        /// </summary>
        internal static bool IsAsianFont(IFontProvider fontProvider, ComplexFontName fontName)
        {
            bool result = false;

            if (fontName != null)
            {
                if (fontName.IsThemeFont)
                {
                    result = ((fontName.ThemeFontCore & ThemeFontCore.RegionMask) == ThemeFontCore.EastAsia);
                }
                else
                {
                    result = FontUtil.IsAsianFont(fontProvider, fontName.Name);
                }
            }

            return result;
        }

        public bool IsInheritedComplexAttr
        {
            get { return false; }
        }

        private readonly string mName;
        private readonly ThemeFontCore mThemeFontCore = ThemeFontCore.None;
    }
}
