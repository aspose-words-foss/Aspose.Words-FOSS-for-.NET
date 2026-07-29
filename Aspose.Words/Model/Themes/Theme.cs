// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/06/2009 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Collections;
using Aspose.JavaAttributes;
using Aspose.OpcPackaging;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Fonts;
using Aspose.Words.Settings;
using Aspose.Xml;

namespace Aspose.Words.Themes
{
    /// <summary>
    /// Represents document Theme, and provides access to main theme parts including <see cref="MajorFonts"/>, <see cref="MinorFonts"/> and <see cref="Colors"/>
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-styles-and-themes/">Working with Styles and Themes</a> documentation article.</para>
    /// </summary>
    /// <dev>
    /// At the moment this is just a crude hack to preserve themes during DOCX to DOCX roundtrip,
    /// but later should be worked into a more appropriate object.
    ///
    /// WORDSNET-10513 Limited support for theme handling was implemented.
    /// If customer changes one of theme part then theme is rewritten with new values.
    /// If theme is unchanged during roundtrip then it is written raw as before.
    /// </dev>
    public class Theme : IThemeProvider, IDmlExtensionListSource
    {
        /// <summary>
        /// Allows to specify the set of major fonts for different languages.
        /// </summary>
        public ThemeFonts MajorFonts
        {
            get { return mMajorFonts; }
        }

        /// <summary>
        /// Allows to specify the set of minor fonts for different languages.
        /// </summary>
        public ThemeFonts MinorFonts
        {
            get { return mMinorFonts; }
        }

        /// <summary>
        /// Allows to specify the set of theme colors for the document.
        /// </summary>
        public ThemeColors Colors
        {
            get { return mColorScheme; }
        }

        /// <summary>
        /// Returns the default built-in theme from embedded resource.
        /// </summary>
        internal static Theme BuiltInTheme
        {
            get
            {
                // Implementing Singleton in C# http://msdn.microsoft.com/en-us/library/ms998558.aspx
                if (gDefaultTheme == null)
                {
                    lock (gDefaultThemeSyncRoot)
                    {
                        if (gDefaultTheme == null)
                        {
                            gDefaultTheme = ReadBuiltInTheme("Aspose.Words.Resources.AllStyles2007.docx");
                        }
                    }
                }

                return gDefaultTheme;
            }
        }

        #region IThemeProvider

        /// <summary>
        /// Safely gets a font name according to the algorithm described in 17.15.1.88 themeFontLang (Theme Font Languages).
        /// The returned value can be null or empty string.
        /// </summary>
        string IThemeProvider.GetFontName(ThemeFontCore themeFont)
        {
            // Select major or minor fonts.
            ThemeFonts themeFonts = ((themeFont & ThemeFontCore.GroupMask) == ThemeFontCore.Major) ? mMajorFonts : mMinorFonts;
            if (themeFonts == null)
                return null;

            // Select theme font language and the default font to use in case the font specified by the language will not be found.
            Language themeFontLang;
            FontInfo defaultFont;
            switch (themeFont & ThemeFontCore.RegionMask)
            {
                case ThemeFontCore.Ascii:
                case ThemeFontCore.HAnsi:
                    themeFontLang = mThemeFontLanguages.Latin;
                    defaultFont = themeFonts.LatinFontInfo;
                    break;
                case ThemeFontCore.Bidi:
                    themeFontLang = mThemeFontLanguages.Bidi;
                    defaultFont = themeFonts.ComplexScriptFontInfo;
                    break;
                case ThemeFontCore.EastAsia:
                    themeFontLang = mThemeFontLanguages.EastAsia;
                    defaultFont = themeFonts.EastAsianFontInfo;
                    break;
                default:
                    themeFontLang = Language.NoProof;
                    defaultFont = null;
                    break;
            }

            // Try to select the font specified by the theme font language.
            string script = LanguageToScript(themeFontLang);
            if (StringUtil.HasChars(script))
            {
                ThemeSupplementalFont suppFont = themeFonts.SupplementalFonts.GetValueOrNull(script);
                if ((suppFont != null) && StringUtil.HasChars(suppFont.Typeface))
                    return suppFont.Typeface;
            }

            // Ok, a font according to the theme font language is not specified, return the default font for the region.
            return (defaultFont != null) ? defaultFont.Name : null;
        }

        DmlColor IThemeProvider.GetThemeColor(ThemeColor color)
        {
            // We return a clone to protect original color from modification
            DmlColor dmlColor = ColorScheme.GetColor(color);
            return (dmlColor == null) ? null : dmlColor.Clone();
        }

        DmlFill IThemeProvider.GetBackgroundFillStyle(int index)
        {
            // We return a clone to protect original fill from modification
            return FormatScheme.GetBackgroundFillStyle(index).Clone();
        }

        DmlFill IThemeProvider.GetFillStyle(int index)
        {
            // We return a clone to protect original fill from modification
            return FormatScheme.GetFillStyle(index).Clone();
        }

        DmlOutline IThemeProvider.GetLineStyle(int index)
        {
            // We return a clone to protect original line from modification
            return FormatScheme.GetLineStyle(index).Clone();
        }

        EffectStyle IThemeProvider.GetEffectStyle(int index)
        {
            // We return a clone to protect original line from modification
            return FormatScheme.GetEffectStyle(index).Clone();
        }

        void IThemeProvider.OnChange()
        {
            UpdateDocument();
        }

        #endregion

        /// <summary>
        /// Internal setter. Used during document loading.
        /// </summary>
        internal void SetMajorFonts(ThemeFonts majorFonts)
        {
            mMajorFonts = majorFonts;
        }

        /// <summary>
        /// Internal setter. Used during document loading.
        /// </summary>
        internal void SetMinorFonts(ThemeFonts minorFonts)
        {
            mMinorFonts = minorFonts;
        }

        /// <summary>
        /// At the moment returns just a shallow copy because we do not fully parse a theme.
        /// </summary>
        internal Theme Clone()
        {
            Theme lhs = (Theme)MemberwiseClone();
            lhs.SetThemePart(ThemePart.Clone(), mThemeNamespace);

            if (mColorScheme != null)
            {
                lhs.mColorScheme = mColorScheme.Clone();
                lhs.mColorScheme.SetTheme(lhs);
            }

            if (mFormatScheme != null)
                lhs.mFormatScheme = mFormatScheme.Clone();

            if (mMajorFonts != null)
                lhs.mMajorFonts = mMajorFonts.Clone();

            if (mMinorFonts != null)
                lhs.mMinorFonts = mMinorFonts.Clone();

            if (mRelatedParts != null)
            {
                lhs.mRelatedParts = new Dictionary<string, OpcPackagePart>();
                foreach (KeyValuePair<string, OpcPackagePart> entry in mRelatedParts)
                    lhs.mRelatedParts.Add(entry.Key, entry.Value.Clone());
            }

            if (mThemeFontLanguages != null)
                lhs.mThemeFontLanguages = mThemeFontLanguages.Clone();

            lhs.mExtensions = DmlExtensionListSource.CloneExtensions(mExtensions);

            return lhs;
        }

        /// <summary>
        /// Checks that both themes defines that same font set.
        /// </summary>
        /// <remarks>
        /// AM. Although it looks like bad realization of Theme.Equals we need to compare themes this way.
        /// The method is used in importing to correctly update fonts in imported content in case of KeepSourceFormat mode so
        /// we take care about fonts only.
        /// </remarks>
        internal static bool ThemeFontsEquals(Theme a, Theme b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (ReferenceEquals(null, a))
                return false;

            if (ReferenceEquals(null, b))
                return false;

            foreach (ThemeFontCore themeFont in RunPr.ThemeFontAttrs)
                if (((IThemeProvider)a).GetFontName(themeFont) != ((IThemeProvider)b).GetFontName(themeFont))
                    return false;

            return true;
        }

        internal static bool ThemeColorsEquals(Theme a, Theme b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (ArgumentUtil.OneIsNull(a, b))
                return false;

            if (!a.ColorScheme.Accent3.Equals(b.ColorScheme.Accent3))
                return false;

            return true;
        }

        /// <summary>
        /// Resolves theme attributes in given RunPr.
        /// </summary>
        /// <remarks>
        /// AM. Made static because given theme can be null.
        /// </remarks>
        internal static void Apply(Theme theme, RunPr runPr)
        {
            // Theme can be null and it's OK. In this case all theme-related fonts should be resolved to Times.

            RunPr appliedPr = new RunPr();

            bool applyThemeColor = false;
            for (int i = 0; i < runPr.Count; i++)
            {
                int key = runPr.GetKey(i);
                object value = runPr.GetByIndex(i);

                switch (key)
                {
                    case FontAttr.NameAscii:
                    case FontAttr.NameFarEast:
                    case FontAttr.NameOther:
                    case FontAttr.NameBi:
                        appliedPr.SetAttr(key, ComplexFontName.FromName(ComplexFontName.Resolve(value, theme)));
                        break;
                    case FontAttr.ThemeColor:
                        applyThemeColor = true;
                        // AM Apply theme color. The problem here is that we need to delete ThemeColor.
                        // It has to be gone when ThemeColor and Color is combined into one attribute.
                        break;
                    default:
                        break;
                }
            }

            appliedPr.ExpandTo(runPr);
            if (applyThemeColor)
            {
                // AM. In common both Color and ThemeColor are present, so we need to just delete ThemeColor.
                // But it will be incorrect if only ThemeColor specified. We need to correctly resolve this color.
                runPr.Remove(FontAttr.ThemeColor);
            }
        }

        /// <summary>
        /// Creates Theme with theme override XML.
        /// </summary>
        [JavaConvertCheckedExceptions]
        internal Theme CreateThemeOverride()
        {
            Theme themeOverride = this.Clone();

            // Build theme override part which is theme itself but has another root element.
            ThemePart.Stream.Position = 0;
            AnyXmlReader reader = new AnyXmlReader(ThemePart.Stream);

            MemoryStream overrideXmlStream = new MemoryStream();
            AnyXmlBuilder builder = new AnyXmlBuilder(overrideXmlStream, false);

            builder.StartDocument("a:themeOverride");
            builder.WriteAttributeString("xmlns:a", themeOverride.ThemeNamespace);

            reader.ReadChild("theme");
            reader.ReadChild("themeElements");

            while (!reader.IsEndElement("themeElements"))
                builder.WriteRaw(reader.ReadOuterXml());

            builder.EndDocument();

            overrideXmlStream.Position = 0;

            themeOverride.ThemePart.Stream = overrideXmlStream;

            return themeOverride;
        }

        /// <summary>
        /// Sets the theme's main XML part. The specified namespace indicates OOXML format,
        /// in which the theme is written.
        /// </summary>
        internal void SetThemePart(OpcPackagePart value, string themeNamespace)
        {
            mThemePart = value;
            mThemeNamespace = themeNamespace;
        }

        /// <summary>
        /// The theme's main XML part. At the moment we preserve it unparsed.
        /// </summary>
        internal OpcPackagePart ThemePart
        {
            get { return mThemePart; }
        }

        /// <summary>
        /// Returns namespace indicating OOXML format, in which the theme is stored in <see cref="ThemePart"/>.
        /// </summary>
        internal string ThemeNamespace
        {
            get { return mThemeNamespace; }
        }

        /// <summary>
        /// Collection of parts that the theme refers to.
        ///
        /// Key is a string rId (relationship id) that was specified in the original DOCX.
        /// We preserve it because we do not parse the theme XML.
        ///
        /// The value is an <see cref="OpcPackagePart"/> object taken from the original DOCX.
        /// </summary>
        internal Dictionary<string, OpcPackagePart> RelatedParts
        {
            get { return mRelatedParts; }
            set { mRelatedParts = value; }
        }

        /// <summary>
        /// Theme name. Can be null.
        /// </summary>
        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Font scheme name. Can be null.
        /// </summary>
        internal string FontSchemeName
        {
            get { return mFontSchemeName; }
            set { mFontSchemeName = value; }
        }

        /// <summary>
        /// Corresponds to 17.15.1.88 themeFontLang (Theme Font Languages).
        /// In DOCX this is stored in settings.xml but we need this data here to properly resolve font names.
        /// This data is actually stored in <see cref="DocPr.ThemeFontLanguages"/>. This property is just a reference to it.
        /// </summary>
        internal ThemeFontLanguages ThemeFontLanguages
        {
            get { return mThemeFontLanguages; }
            set { mThemeFontLanguages = value; }
        }

        /// <summary>
        /// Corresponds to 20.1.6.2 clrScheme (Color Scheme) and defines
        /// a set of colors which are referred to as a color scheme.
        /// The color scheme is responsible for defining a list of twelve colors.
        /// The twelve colors consist of six accent colors, two dark colors, two light
        /// colors and a color for each of a hyperlink and followed hyperlink.
        /// </summary>
        internal ThemeColors ColorScheme
        {
            get
            {
                if (mColorScheme == null)
                    mColorScheme = new ThemeColors(this);
                return mColorScheme;
            }
        }

        /// <summary>
        /// Corresponds to 20.1.4.1.14 fmtScheme (Format Scheme)
        /// and contains the background fill styles,
        /// effect styles, fill styles, and line styles which
        /// define the style matrix for a theme.
        /// </summary>
        internal FormatScheme FormatScheme
        {
            get
            {
                if (mFormatScheme == null)
                    mFormatScheme = new FormatScheme();
                return mFormatScheme;
            }
            set { mFormatScheme = value; }
        }

        /// <summary>
        /// Gets or sets an object that defines default shape, line, and textbox formatting properties.
        /// Corresponds to the element 20.1.6.7 objectDefaults (Object Defaults).
        /// The value can be <c>null</c> if no defaults is set.
        /// </summary>
        internal ThemeObjectDefaults ObjectDefaults
        {
            get { return mObjectDefaults; }
            set { mObjectDefaults = value; }
        }

        /// <summary>
        /// Corresponds to 20.1.2.2.15 extLst (Extension List, CT_OfficeArtExtensionList),
        /// see also 2.2.8 Themes [MS-ODRAWXML].
        /// An extension can only contain the element 2.4.1.1 themeFamily [MS-ODRAWXML].
        /// </summary>
        /// <remarks>
        /// The returned array list contains items of the <see cref="DmlExtension"/> type.
        /// Each item stores extension's XML as is.
        /// The <c>null</c> value may be returned.
        /// </remarks>
        StringToObjDictionary<DmlExtension> IDmlExtensionListSource.Extensions
        {
            get { return mExtensions; }
            set { mExtensions = value; }
        }

        /// <summary>
        /// Returns flag indicating that the theme is modified by customer.
        /// </summary>
        internal bool IsModified
        {
            get
            {
                return (mMajorFonts != null && mMajorFonts.IsModified) ||
                    (mMinorFonts != null && mMinorFonts.IsModified) ||
                    (mColorScheme != null && mColorScheme.IsModified);
            }
        }

        /// <summary>
        /// Attaches document to the theme.
        /// </summary>
        internal void Attach(Document document)
        {
            mDocument = document;
            if (mColorScheme != null && mColorScheme.IsModified)
                UpdateDocument();
        }

        /// <summary>
        /// Converts a <see cref="Language"/> value into a script name.
        /// Script names are according to http://unicode.org/iso15924/iso15924-codes.html
        /// </summary>
        private static string LanguageToScript(Language lang)
        {
            switch ((int)lang & 0x00ff)
            {
                case LanguageOnly.Japanese:
                    return "Jpan";
                case LanguageOnly.Korean:
                    return "Hang";
                case LanguageOnly.Chinese:
                    // WORDSNET-15818 Resolve either to 'Traditional' or to 'Simplified' Chinese script
                    // depending on the country code.
                    return UsesTraditionalChineseScript(lang) ? "Hant" : "Hans";
                case LanguageOnly.Arabic:
                    return "Arab";
                case LanguageOnly.Hebrew:
                    return "Hebr";
                case LanguageOnly.Thai:
                    return "Thai";
                case LanguageOnly.Ethiopian:
                    return "Ethi";
                case LanguageOnly.Bengali:
                    return "Beng";
                case LanguageOnly.Gujarati:
                    return "Gujr";
                case LanguageOnly.Khmer:
                    return "Khmr";
                case LanguageOnly.Kannada:
                    return "Knda";
                case LanguageOnly.Punjabi:
                    return "Guru"; // http://en.wikipedia.org/wiki/Gurmukh%C4%AB_script
                case LanguageOnly.Inuktitut:
                    return "Cans"; // http://en.wikipedia.org/wiki/Canadian_Aboriginal_syllabics
                case LanguageOnly.Cherokee:
                    return "Cher";
                case LanguageOnly.Yi:
                    return "Yiii";
                case LanguageOnly.Tibetan:
                    return "Tibt";
                case LanguageOnly.Divehi:
                    return "Thaa"; // http://en.wikipedia.org/wiki/T%C4%81na
                case LanguageOnly.Devangari:
                    return "Deva";
                case LanguageOnly.Telugu:
                    return "Telu";
                case LanguageOnly.Tamil:
                    return "Taml";
                case LanguageOnly.Syriac:
                    return "Syrc";
                case LanguageOnly.Oriya:
                    return "Orya";
                case LanguageOnly.Malayalam:
                    return "Mlym";
                case LanguageOnly.Lao:
                    return "Laoo";
                case LanguageOnly.Sinhalese:
                    return "Sing";
                case LanguageOnly.Mongolian:
                    return "Mong";
                case LanguageOnly.Vietnamese:
                    return "Viet";
                // WORDSNET-17652 Determine script for "Hindi" language to get correct theme font name.
                case LanguageOnly.Hindi:
                    return "Deva";
                default:
                    // No idea how to handle the "Uigh" value. It is for the Uyghur script.
                    // But I don't have a code for it http://en.wikipedia.org/wiki/Uyghur_people
                    return null;
            }
        }

        private static bool UsesTraditionalChineseScript(Language lang)
        {
            return (lang == Language.ChineseHongKong) ||
                (lang == Language.ChineseTaiwan) ||
                (lang == Language.ChineseMacao);
        }

        /// <summary>
        /// Updates document content colors from the theme.
        /// </summary>
        private void UpdateDocument()
        {
            if (mDocument != null)
                ThemeColorUpdater.Update(mDocument);
        }

        [JavaThrows(false)]
        private static Theme ReadBuiltInTheme(string resourceName)
        {
            try
            {
                // INSP DD: Do themes differ from word to word version? If yes we should probably consider Document.CompatibilityOptions.OptimizeFor
                using (Stream stream = ResourceUtil.FetchResourceStream(resourceName))
                {
                    Document doc = new Document(stream, null, false);
                    return doc.GetThemeInternal();
                }
            }
            catch (Exception e)
            {
                // JAVA: Catch and rethrow RuntimeException for java.
                // This needed to not propagate throws exception clause to attribute fetching methods.
                throw new InvalidOperationException("Cannot load built in theme from an embedded resource.", e);
            }
        }

        private ThemeColors mColorScheme;

        private string mFontSchemeName;
        private FormatScheme mFormatScheme;
        private ThemeFonts mMajorFonts;
        private ThemeFonts mMinorFonts;
        private string mName;
        private Dictionary<string, OpcPackagePart> mRelatedParts = new Dictionary<string, OpcPackagePart>();
        private ThemeObjectDefaults mObjectDefaults;
        private StringToObjDictionary<DmlExtension> mExtensions;

        /// <summary>
        /// This must not be null when DOCX is read.
        /// </summary>
        private ThemeFontLanguages mThemeFontLanguages;

        private OpcPackagePart mThemePart;

        /// Attached document to update content if theme is changed.
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private Document mDocument;

        private string mThemeNamespace;

        private static Theme gDefaultTheme;
        private static readonly object gDefaultThemeSyncRoot = new object();
    }
}
