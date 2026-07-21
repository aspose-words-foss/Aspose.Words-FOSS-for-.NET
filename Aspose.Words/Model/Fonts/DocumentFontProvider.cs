// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2012 by Konstantin Kornilov

using System;
using System.Drawing;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Drawing.Fonts;
using Aspose.Fonts.TrueType;

namespace Aspose.Words.Fonts
{
    /// <summary>
    /// Helps to resolve fonts. Uses fonts embedded into a document plus fonts from specified font sources.
    /// </summary>
    internal class DocumentFontProvider : IDrFontProvider
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal DocumentFontProvider(Document document)
        {
            mDocument = document;
        }

        /// <summary>
        /// Fetches the drawing font and tries to perform a simple font substitution.
        /// </summary>
        public DrFont FetchDrFont(string familyName, float sizePoints, FontStyle style)
        {
            return FetchDrFont(familyName, sizePoints, style, style, false, false);
        }

        /// <summary>
        /// Fetches the drawing font and tries to perform a simple font substitution.
        /// </summary>
        public DrFont FetchDrFont(string familyName, float sizePoints, FontStyle actualStyle, FontStyle fontFaceStyle, bool isVertical, bool useWord97FontMetrics)
        {
            return FetchDrFont(familyName, sizePoints, actualStyle, fontFaceStyle, isVertical, true, useWord97FontMetrics, false, false, false);
        }

        /// <summary>
        /// Fetches the drawing font and tries to perform a simple font substitution.
        /// </summary>
        public DrFont FetchDrFont(string familyName, float sizePoints, FontStyle actualStyle, FontStyle fontFaceStyle, bool isVertical, bool useWord97FontMetrics, bool isSmallCaps, bool isSuperscript, bool isSubscript)
        {
            return FetchDrFont(familyName, sizePoints, actualStyle, fontFaceStyle, isVertical, true, useWord97FontMetrics, isSmallCaps, isSuperscript, isSubscript);
        }

        /// <summary>
        /// Fetches the drawing font and tries to perform a simple font substitution.
        /// </summary>
        public DrFont FetchDrFont(string familyName,
            float sizePoints,
            FontStyle actualStyle,
            FontStyle fontFaceStyle,
            bool isVertical,
            bool adjustCjkFontMetrics,
            bool useWord97FontMetrics)
        {
            return new DrFont(sizePoints, actualStyle, FetchTTFont(familyName, fontFaceStyle), isVertical, adjustCjkFontMetrics, useWord97FontMetrics);
        }

        /// <summary>
        /// Fetches the drawing font and tries to perform a simple font substitution.
        /// </summary>
        public DrFont FetchDrFont(string familyName,
            float sizePoints,
            FontStyle actualStyle,
            FontStyle fontFaceStyle,
            bool isVertical,
            bool adjustCjkFontMetrics,
            bool useWord97FontMetrics,
            bool isSmallCaps,
            bool isSuperscript,
            bool isSubscript)
        {
            return new DrFont(sizePoints, actualStyle, FetchTTFont(familyName, fontFaceStyle), isVertical, adjustCjkFontMetrics, useWord97FontMetrics, isSmallCaps, isSuperscript, isSubscript);
        }

        /// <summary>
        /// Gets the specified font. Returns null if font is not available.
        /// </summary>
        public TTFont GetTTFont(string familyName, FontStyle style)
        {
            // FOSS: font substitution and external/system font resolution were removed.
            // Only fonts embedded into the document are resolved to real font data; every other
            // font name is left unresolved (callers fall back to the last-resort font via FetchTTFont).
            if (!StringUtil.HasChars(familyName))
                return null;

            RegisterRequestedFont(familyName, style);

            return GetEmbeddedFont(GetFontInfo(familyName), style);
        }

        /// <summary>
        /// Fetches the TrueType font and tries to perform a simple font substitution.
        /// </summary>
        public TTFont FetchTTFont(string familyName, FontStyle style)
        {
            // FOSS: substitution rules were removed. Use the embedded font if present, otherwise
            // the last-resort font so callers always get non-null font data.
            return GetTTFont(familyName, style) ?? Settings.GetAnyFont();
        }

        /// <summary>
        /// Returns fallback font for the specified font and character code.
        /// Returns null if fallback font is not found.
        /// </summary>
        /// <param name="font">Font for which fallback font is searched.</param>
        /// <param name="charCode">Character code.</param>
        /// <param name="useCharacterReplacements">
        /// Specifies if character replacements should be used when checking fallback font for glyph availability.
        /// </param>
        public TTFont GetFallbackFont(TTFont font, int charCode, bool useCharacterReplacements)
        {
            // FOSS: font fallback resolution was removed along with the substitution engine.
            return null;
        }

        /// <summary>
        /// Returns <see cref="FontInfo"/> from the document's font info
        /// collection by its name <see cref="FontInfo.Name"/>.
        /// </summary>
        internal FontInfo GetFontInfo(string familyName)
        {
            foreach (FontInfo info in mDocument.FontInfos)
                if (StringUtil.EqualsOrdinalIgnoreCase(info.Name, familyName))
                    return info;

            return null;
        }

        /// <summary>
        /// Returns resolved font name with checking part of font substitution rules required for HTML reader/writer.
        /// </summary>
        /// <remarks>
        /// If font name can be resolved as a font from document font infos or from external cache or embedded font
        /// then return resolved font family name.
        /// If font name can be resolved via substitution rules then return original family name.
        /// Else return null.
        /// </remarks>
        internal string GetFontNameForHtml(string fontFamily)
        {
            // FOSS: substitution was removed. Only report a resolved name for fonts that are actually
            // embedded into the document; otherwise return null so the caller keeps the originally
            // requested font family name.
            const FontStyle familyFontStyle = FontStyle.Regular;

            TTFont resolvedFont = GetTTFont(fontFamily, familyFontStyle);
            if (resolvedFont == null)
                return null;

            // We should use name with which the font was imported from @font-face rule.
            if (resolvedFont.Data.IsEmbedded)
                return fontFamily;

            // Note that by returning "FamilyName" instead of "fontFamily" we "normalize" the font name.
            // For example, both "arial" and "ARIAL" will be resolved to "Arial".
            return resolvedFont.FamilyName;
        }

        /// <summary>
        /// Returns embedded <see cref="TTFont"/> from the specified <paramref name="info"/>
        /// with the specified <paramref name="style"/>. If there is no exactly font match found,
        /// it tries to get font with other styles (not italic, then not bold and regular at last).
        /// </summary>
        internal static TTFont GetEmbeddedFont(FontInfo info, FontStyle style)
        {
            // There may be no font info at all.
            if (info == null)
                return null;

            // First try exact style.
            TTFont font = info.GetEmbeddedFontParsed(style);
            if (font != null)
                return font;

            // Then try non-italic style.
            FontStyle tryStyle = (style & (~FontStyle.Italic));
            font = info.GetEmbeddedFontParsed(tryStyle);
            if (font != null)
                return font;

            // Then try non-bold style.
            tryStyle = (style & (~FontStyle.Bold));
            font = info.GetEmbeddedFontParsed(tryStyle);
            if (font != null)
                return font;

            // Then try regular style.
            font = info.GetEmbeddedFontParsed(FontStyle.Regular);
            if (font != null)
                return font;

            // At last try to get any style.
            return info.GetEmbeddedFontParsedAnyStyle();
        }

        /// <summary>
        /// Stores all requested fonts (font familyName and style) into the requestedFontsCache collection.
        /// </summary>
        private void RegisterRequestedFont(string familyName, FontStyle style)
        {
            // Only for debug purposes.
#if DEBUG
            string requestedFontKey = TTFont.BuildFontKey(familyName, FormatterPal.IntToStr((int)style));

            if (!RequestedFontsCache.ContainsKey(requestedFontKey))
                RequestedFontsCache.Add(requestedFontKey, (int)style);
#endif
        }

        internal DocumentFontProvider Clone()
        {
            return (DocumentFontProvider)MemberwiseClone();
        }

        /// <summary>
        /// This collection contains requested fonts family names and styles.
        /// </summary>
        internal StringToIntDictionary RequestedFontsCache
        {
            get { return mRequestedFontsCache; }
            set { mRequestedFontsCache = value; }
        }

        private FontSettings Settings
        {
            get { return mDocument.EffectiveFontSettings; }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Document mDocument;
        private StringToIntDictionary mRequestedFontsCache = new StringToIntDictionary();
    }
}
