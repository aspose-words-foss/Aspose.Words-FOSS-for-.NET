// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/07/2010 by Roman Korchagin
#if !NETSTANDARD

using System.Drawing;
using System.Globalization;
using Aspose.Collections;
using Aspose.Fonts;
using Aspose.Fonts.TrueType;
using Aspose.JavaAttributes;

namespace Aspose.Drawing.Fonts
{
    /// <summary>
    /// Port this class manually to Java.
    /// </summary>
    [JavaManual("Manual porting by design.")]
    public static class FontPal
    {
        /// <summary>
        /// Provides access to information about fonts installed on the system.
        /// </summary>
        internal static FontFamily GetNativeFamily(string fontName)
        {
            string key = GetFontFamiliesKey(fontName);
            return gFontFamilies.ContainsKey(key)
                ? gFontFamilies[key]
                : null;
        }

        internal static bool IsWindowsFont(TTFont font)
        {
            // WORDSJAVA-1634 This method needs to be implemented in Java
            return true;
        }

        /// <summary>
        /// This is used to find if specified font is installed on the system.
        /// Key is a font name. Value is a FontFamily object.
        /// </summary>
        private static readonly StringToObjDictionary<FontFamily> gFontFamilies = new StringToObjDictionary<FontFamily>();

        /// <summary>
        /// Default family which will be used if particular family is not found.
        /// </summary>
        private static readonly FontFamily gDefaultFamily;

        static FontPal()
        {
            FontFamily[] families = FontFamily.Families;
            foreach (FontFamily family in families)
                gFontFamilies[GetFontFamiliesKey(family.GetName(FamilyNameLanguageId))] = family;

            if (families.Length > 0)
                gDefaultFamily = families[0];
        }

        private static string GetFontFamiliesKey(string familyName)
        {
            return FontUtil.TrimFamilyNameForGdi(familyName).ToLowerInvariant();
        }

        /// <summary>
        /// Language Id to use with <see cref="FontFamily.GetName"/> method.
        /// </summary>
        /// <remarks>
        /// <see cref="FontFamily.Name"/> may return localized name for certain Thread.CurrentThread.CurrentUICulture values (see WORDSNET-17583).
        /// On the other hand, AW always used default English font family name. So we should use <see cref="FontFamily.GetName"/> method with this
        /// language id to get the proper result.
        /// </remarks>
        internal static int FamilyNameLanguageId = new CultureInfo("en-US").LCID;
    }
}
#endif
