// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2012 by Konstantin Kornilov

using System.Collections.Generic;
using System.Drawing;
using Aspose.Fonts.TrueType;

namespace Aspose.Fonts
{
    /// <summary>
    /// Represents information about a font that is used in font searching.
    /// </summary>
    /// <remarks>
    /// Instances of this class are immutable and therefore thread-safe.
    /// </remarks>
    /// <dev>
    /// This class is serialized in <see cref="FontSearchInfoSerializer"/>. Changes in this class should be reflected there.
    /// </dev>
    public class FontSearchInfo
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public FontSearchInfo(PhysicalFontData fontData, string fontFullName, string fontFamilyName,
            ICollection<string> fontFullNamesAllLanguages,
            ICollection<string> fontFamilyNamesAllLanguages,
            FontStyle style, bool isCff, int fontSourcePriority, string version,
            FontPanose panose, FontUnicodeRanges unicodeRanges, FontCodepageRanges codepageRanges,
            FontFsType fsType)
        {
            FontData = fontData;
            FontFullName = fontFullName;
            FontFamilyName = fontFamilyName;
            FontFullNamesAllLanguages = fontFullNamesAllLanguages;
            FontFamilyNamesAllLanguages = fontFamilyNamesAllLanguages;
            Style = style;
            IsCff = isCff;
            FontSourcePriority = fontSourcePriority;
            Version = version;
            Panose = panose;
            UnicodeRanges = unicodeRanges;
            CodepageRanges = codepageRanges;
            FsType = fsType;
        }

        /// <summary>
        /// Link to font binary data.
        /// </summary>
        public PhysicalFontData FontData { get; }

        /// <summary>
        /// Sorted list of font full names in all languages.
        /// </summary>
        public ICollection<string> FontFullNamesAllLanguages { get; }

        /// <summary>
        /// Sorted list of font family names in all languages.
        /// </summary>
        public ICollection<string> FontFamilyNamesAllLanguages { get; }

        /// <summary>
        /// True if font data is a TTC file.
        /// </summary>
        public bool IsTtc
        {
            get { return FontData.IsTtc; }
        }

        /// <summary>
        /// True if font data contains CFF outlines.
        /// </summary>
        public bool IsCff { get; }

        /// <summary>
        /// Font style.
        /// </summary>
        public FontStyle Style { get; }

        /// <summary>
        /// Full font name.
        /// </summary>
        public string FontFullName { get; }

        /// <summary>
        /// Font family name.
        /// </summary>
        public string FontFamilyName { get; }

        /// <summary>
        /// Font source priority.
        /// </summary>
        public int FontSourcePriority { get; }

        /// <summary>
        /// Font version string.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Font PANOSE classification number.
        /// </summary>
        public FontPanose Panose { get; }

        /// <summary>
        /// Supported Unicode ranges.
        /// </summary>
        public FontUnicodeRanges UnicodeRanges { get; }

        /// <summary>
        /// Supported codepages.
        /// </summary>
        public FontCodepageRanges CodepageRanges { get; }

        /// <summary>
        /// Embedding licensing rights for the font.
        /// </summary>
        public FontFsType FsType { get; }
    }
}
