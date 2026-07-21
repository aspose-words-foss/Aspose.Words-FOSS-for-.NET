// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/05/2005 by Roman Korchagin
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using Aspose.Fonts;
using Aspose.Fonts.TrueType;
using Aspose.IO;

namespace Aspose.Words.Fonts
{
    /// <summary>
    /// Specifies information about a font used in the document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fonts/">Working with Fonts</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>You do not create instances of this class directly.
    /// Use the <see cref="DocumentBase.FontInfos"/> property to access the collection of fonts
    /// defined in a document.</para>
    ///
    /// <seealso cref="FontInfoCollection"/>
    /// <seealso cref="DocumentBase.FontInfos"/>
    /// </remarks>
    public class FontInfo
    {
        /// <summary>
        /// Don't need public ctor.
        /// </summary>
        internal FontInfo()
        {
        }

        /// <summary>
        /// Don't need public ctor.
        /// </summary>
        internal FontInfo(string name)
        {
            SetName(name);
        }

        /// <summary>
        /// Use only when loading font infos. Must call before adding font info to the font infos collection.
        /// </summary>
        internal void SetName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Debug.Assert(string.IsNullOrEmpty(mName), "Name is already set, but I expect name to be set only once during loading.");

            mName = name;
        }

        /// <summary>
        /// Clones this font info, including embedded fonts.
        /// </summary>
        internal FontInfo Clone()
        {
            //Don't think I need to clone other fields.
            FontInfo lhs = (FontInfo)MemberwiseClone();

            if (mEmbeddedFonts != null)
                lhs.mEmbeddedFonts = mEmbeddedFonts.Clone();

            if (mPanose != null)
                lhs.mPanose = (byte[])mPanose.Clone();

            if (mSig != null)
                lhs.mSig = (byte[])mSig.Clone();

            return lhs;
        }

        /// <summary>
        /// <para>Merges new <see cref="FontInfo"/> to this one.</para>
        /// <para>It's acceptable for <see cref="FontInfo"/> not to contain some of possible attributes. Also,
        /// <see cref="FontInfo"/> doesn't often contain one or all embedded fonts in its <see cref="FontInfoCollection"/>.</para>
        /// <para>Merge operation adds all missing attributes and embedded fonts from <paramref name="srcFontInfo"/>
        /// concerning the same font. Different <paramref name="srcFontInfo"/> can be obtained from other file or other
        /// font entry in the same file.</para>
        /// <para>Merge operations replaces subsetted embedded fonts with full versions.</para>
        /// </summary>
        internal void Merge(FontInfo srcFontInfo)
        {
            MergeAttributes(srcFontInfo);

            if (srcFontInfo.mEmbeddedFonts == null)
            {
                return;
            }
            else if (mEmbeddedFonts == null)
            {
                mEmbeddedFonts = new EmbeddedFontCollection();
            }

            mEmbeddedFonts.Merge(srcFontInfo.mEmbeddedFonts);
        }

        /// <summary>
        /// <para>Provides access to embedded fonts list to iterate over it.</para>
        /// <para>Don't use to add records to embedded fonts list,
        /// use <see cref="FontInfo.AddEmbeddedFont(byte[], EmbeddedFontFormat, EmbeddedFontStyle, bool)"/> instead.</para>
        /// <para>Use <see cref="FontInfo.GetEmbeddedFont"/> to get safe access to particular element.</para>
        /// <para>Each <see cref="FontInfo"/> can contain font files of different formats (see <see cref="EmbeddedFontFormat"/>)
        /// and up to four font files for each font format (see <see cref="EmbeddedFontStyle"/>).</para>
        /// </summary>
        /// <remarks>
        /// <para>Can be <c>null</c>.</para>
        /// </remarks>
        internal EmbeddedFont[] GetEmbeddedFonts(EmbeddedFontFormat format)
        {
            return (mEmbeddedFonts == null) ? null : mEmbeddedFonts.GetFonts(format);
        }

        /// <summary>
        /// Gets a specific embedded font file.
        /// </summary>
        /// <param name="format">Specifies the font format to retrieve.</param>
        /// <param name="style">Specifies the font style to retrieve.</param>
        /// <returns>Returns <c>null</c> if the specified font is not embedded.</returns>
        public byte[] GetEmbeddedFont(EmbeddedFontFormat format, EmbeddedFontStyle style)
        {
            if (mEmbeddedFonts == null)
                return null;

            EmbeddedFont embeddedFont = mEmbeddedFonts.Get(format, style);

            return (embeddedFont == null) ? null : embeddedFont.Data;
        }

        /// <summary>
        /// Gets an embedded font file in OpenType format. Fonts in Embedded OpenType format are converted to OpenType.
        /// </summary>
        /// <param name="style">Specifies the font style to retrieve.</param>
        /// <returns>Returns <c>null</c> if the specified font is not embedded.</returns>
        public byte[] GetEmbeddedFontAsOpenType(EmbeddedFontStyle style)
        {
            if (mEmbeddedFonts == null)
                return null;

            return mEmbeddedFonts.GetOpenTypeData(style);
        }

        /// <summary>
        /// Gets embedded font of specified style with not empty data.
        /// </summary>
        internal EmbeddedFont GetEmbeddedFontWithData(FontStyle style)
        {
            if (mEmbeddedFonts == null)
                return null;

            EmbeddedFontStyle embeddedStyle = FontStyleToEmbeddedFontStyle(style);

            EmbeddedFont font = mEmbeddedFonts.Get(EmbeddedFontFormat.OpenType, embeddedStyle);
            if (font != null && font.Data != null)
                return font;

            font = mEmbeddedFonts.Get(EmbeddedFontFormat.EmbeddedOpenType, embeddedStyle);
            if (font != null && font.Data != null)
                return font;

            return null;
        }

        /// <summary>
        /// Returns <see cref="EmbeddedFontStyle"/> corresponded to the specified <paramref name="style"/>.
        /// </summary>
        internal static EmbeddedFontStyle FontStyleToEmbeddedFontStyle(FontStyle style)
        {
            if (((style & FontStyle.Italic) != 0) && ((style & FontStyle.Bold) != 0))
                return EmbeddedFontStyle.BoldItalic;
            if ((style & FontStyle.Italic) != 0)
                return EmbeddedFontStyle.Italic;
            if ((style & FontStyle.Bold) != 0)
                return EmbeddedFontStyle.Bold;
            return EmbeddedFontStyle.Regular;
        }

        internal TTFont GetEmbeddedFontParsed(FontStyle style)
        {
            return mEmbeddedFonts == null
                ? null
                : mEmbeddedFonts.GetTtFont(FontStyleToEmbeddedFontStyle(style));
        }

        internal TTFont GetEmbeddedFontParsedAnyStyle()
        {
            return mEmbeddedFonts == null
                ? null
                : mEmbeddedFonts.GetTtFontAnyStyle();
        }

        /// <summary>
        /// <para>Adds new font to embedded fonts collection or replaces existing font with the same format and style.</para>
        /// </summary>
        internal void AddEmbeddedFont(byte[] fontData, EmbeddedFontFormat fontFormat, EmbeddedFontStyle fontStyle,
            bool isSubsetted)
        {
            if (mEmbeddedFonts == null)
                mEmbeddedFonts = new EmbeddedFontCollection();

            mEmbeddedFonts.Add(new EmbeddedFont(fontData, fontFormat, fontStyle, isSubsetted));
        }

        /// <summary>
        /// <para>Adds new font of <see cref="EmbeddedFontFormat.OpenType"/> format to embedded fonts collection
        /// or replaces existing font with the same format and style.</para>
        /// </summary>
        internal void AddEmbeddedFont(TTFont ttFont)
        {
            using (Stream fontDataStream = ttFont.Data.OpenStream())
            {
                byte[] fontData = StreamUtil.CopyStreamToByteArray(fontDataStream);
                EmbeddedFontStyle style = FontStyleToEmbeddedFontStyle(ttFont.Style);

                AddEmbeddedFont(fontData, EmbeddedFontFormat.OpenType, style, false);
            }
        }

        /// <summary>
        /// <para>Gets list of all alternative names (except main) for the font.</para>
        /// <para><seealso cref="AltName"/>.</para>
        /// </summary>
        /// <remarks>Implemented logic is from DOCX spec.</remarks>
        internal IList<string> GetAltNameList()
        {
            List<string> answer = new List<string>();

            foreach (string fontName in mAltName.Split(','))
            {
                string fontNameTrimmed = fontName.Trim(' ');
                if (StringUtil.HasChars(fontNameTrimmed))
                {
                    answer.Add(fontNameTrimmed);
                }
            }

            return answer;
        }

        /// <summary>
        /// Returns array of embedded fonts of those of <see cref="EmbeddedFontFormat"/>
        /// that has at least one not null embedded font.
        /// </summary>
        internal EmbeddedFont[] GetEmbeddedFonts()
        {

            if (HasEmbeddedFontsByFormat(EmbeddedFontFormat.OpenType))
                return GetEmbeddedFonts(EmbeddedFontFormat.OpenType);

            if (HasEmbeddedFontsByFormat(EmbeddedFontFormat.EmbeddedOpenType))
                return GetEmbeddedFonts(EmbeddedFontFormat.EmbeddedOpenType);

            return null;
        }

        /// <summary>
        /// Returns true if this font info has not null embedded font of any embedded format.
        /// </summary>
        internal bool HasEmbeddedFonts
        {
            get
            {
                return HasEmbeddedFontsByFormat(EmbeddedFontFormat.OpenType) ||
                       HasEmbeddedFontsByFormat(EmbeddedFontFormat.EmbeddedOpenType);
            }
        }

        /// <summary>
        /// Returns true if this font info has embedded fonts
        /// of the specified format with not empty data.
        /// </summary>
        internal bool HasEmbeddedFontsByFormat(EmbeddedFontFormat fontFormat)
        {
            return (GetEmbeddedFontsCount(fontFormat) != 0);
        }

        /// <summary>
        /// Returns true if this font info has embedded fonts
        /// of the specified style with not empty data.
        /// </summary>
        internal bool HasEmbeddedFontsByStyle(FontStyle fontStyle)
        {
            return (GetEmbeddedFontWithData(fontStyle) != null);
        }

        /// <summary>
        /// Removes embedded fonts.
        /// </summary>
        internal void RemoveEmbeddedFonts()
        {
            mEmbeddedFonts = null;
        }

        /// <summary>
        /// Returns number of not null embedded fonts of the specified format.
        /// </summary>
        internal int GetEmbeddedFontsCount(EmbeddedFontFormat fontFormat)
        {
            int embeddedFontsCount = 0;

            EmbeddedFont[] embeddedFonts = GetEmbeddedFonts(fontFormat);

            if (embeddedFonts != null)
            {
                foreach (EmbeddedFont embeddedFont in embeddedFonts)
                    if (embeddedFont != null)
                        embeddedFontsCount++;
            }

            return embeddedFontsCount;
        }

        /// <summary>
        /// <para>It's acceptable for <see cref="FontInfo"/> not to contain some of possible attributes.</para>
        /// <para>MergeAttributes operation adds all missing attributes from <paramref name="srcFontInfo"/>
        /// concerning the same font. <paramref name="srcFontInfo"/> can be obtained from other file or other
        /// font entry in the same file.</para>
        /// <para>In general, these attributes should be filled from installed font or be kept unchanged if there is no
        /// installed font with specified name. But currently attributes of the font cannot be got from font file.</para>
        /// <para>The algorithm below successfully fills missing attributes if they exist in another <see cref="FontInfo"/>
        /// but it cannot correctly handle the case when fonts with the same name are essentially different fonts.</para>
        /// <para><see cref = "FontSignature" /> is updated in accordance with the charsets, which must be supported
        /// by the font.</para>
        /// </summary>
        /// <remarks>
        /// For handling of merge of essentially different fonts with the same name requesting of attributes from
        /// installed fonts and embedded fonts is needed.
        /// </remarks>
        private void MergeAttributes(FontInfo srcFontInfo)
        {
            Debug.Assert(StringUtil.EqualsIgnoreCase(mName, srcFontInfo.mName));

            if (mPitch == FontPitch.Default)
                mPitch = srcFontInfo.mPitch;

            if (mAltName == "")
                mAltName = srcFontInfo.mAltName;

            if (mFamily == FontFamily.Auto)
                mFamily = srcFontInfo.mFamily;

            if (mWeight == 0)
                mWeight = srcFontInfo.mWeight;

            if (mIsTrueType)
                mIsTrueType = srcFontInfo.mIsTrueType;

            if ((!IsCharsetDefined || (mCharset == FontUtil.AnsiCharset)) && !StringUtil.HasChars(mIanaCharset))
            {
                mCharset = srcFontInfo.mCharset;
                mIanaCharset = srcFontInfo.mIanaCharset;
            }

            // AS It's safer to make clones of arrays here but
            // I make the behavour consistent with that in Clone() operation.
            if (mPanose == null)
                mPanose = srcFontInfo.mPanose;

            if (mSig == null)
                mSig = srcFontInfo.mSig;

            // If signature was not taken from merged font lets build it.
            FontSignature signature = new FontSignature(mSig);
            signature.AddCharset(srcFontInfo.Charset);
            signature.AddCharset(mCharset);
            mSig = signature.Data;
        }

        /// <summary>
        /// The pitch indicates if the font is fixed pitch, proportionally spaced, or relies on a default setting.
        /// </summary>
        public FontPitch Pitch
        {
            get { return mPitch; }
            set { mPitch = value; }
        }

        /// <summary>
        /// Indicates that this font is a TrueType or OpenType font as opposed to a raster or vector font.
        /// Default is <c>true</c>.
        /// </summary>
        public bool IsTrueType
        {
            get { return mIsTrueType; }
            set { mIsTrueType = value; }
        }

        /// <summary>
        /// Gets or sets the font family this font belongs to.
        /// </summary>
        public FontFamily Family
        {
            get { return mFamily; }
            set { mFamily = value; }
        }

        /// <summary>
        /// Base weight of font.
        /// </summary>
        internal int Weight
        {
            get { return mWeight; }
            set { mWeight = value; }
        }

        /// <summary>
        /// Gets or sets the character set for the font.
        /// </summary>
        /// <dev>
        /// Such charset code is replaced by IANA name of character set in ISO/IEC 29500.
        /// We may not know charset codes for some IANA charset names. Also there may be unknown charset codes.
        /// For example AW test documents contain the undocumented 0x4E and 0x50 codes.
        /// So, now we store the both: charset codes and charset IANA names to be able to write charset as it was read.
        /// </dev>
        public int Charset
        {
            get
            {
                if (IsCharsetDefined)
                    return mCharset;

                int charset = FontUtil.ResolveCharset(mIanaCharset, mName);
                return (charset != FontUtil.UndefinedCharset) ? charset : FontUtil.AnsiCharset;
            }
            set
            {
                mCharset = value;
                if (IsCharsetDefined)
                    mIanaCharset = null;
            }
        }

        /// <summary>
        /// Gets or sets the IANA character set name for the font.
        /// </summary>
        /// <dev>
        /// IANA name is used instead of character set code in ISO/IEC 29500.
        /// We may not know charset codes for some IANA charset names. Also there may be unknown charset codes.
        /// For example AW test documents contain the undocumented 0x4E and 0x50 codes.
        /// So, now we store the both: charset codes and charset IANA names to be able to write charset as it was read.
        /// </dev>
        internal string IanaCharset
        {
            get
            {
                if (StringUtil.HasChars(mIanaCharset))
                    return mIanaCharset;

                if (IsCharsetDefined)
                    return FontUtil.IanaNameByCharset(mCharset);
                else
                    return "";
            }
            set
            {
                mIanaCharset = value;
                if (StringUtil.HasChars(mIanaCharset))
                    mCharset = FontUtil.UndefinedCharset;
            }
        }

        /// <summary>
        /// Gets or sets the PANOSE typeface classification number.
        /// </summary>
        /// <remarks>
        /// <para>PANOSE is a compact 10-byte description of a fonts critical visual characteristics,
        /// such as contrast, weight, and serif style. The digits represent Family Kind, Serif Style,
        /// Weight, Proportion, Contrast, Stroke Variation, Arm Style, Letterform, Midline, and X-Height.</para>
        ///
        /// <para>Can be <c>null</c>.</para>
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "Public API, as designed.")]
        public byte[] Panose
        {
            get { return mPanose; }
            set
            {
                if ((value != null) && (value.Length != PanoseLength))
                    throw new ArgumentException("Incorrect array length.");

                mPanose = value;
            }
        }

        /// <summary>
        /// Contains information identifying the code pages and Unicode subranges for which the font provides glyphs.
        /// </summary>
        /// <remarks>
        /// <para>Can be <c>null</c>.</para>
        /// </remarks>
        internal byte[] Sig
        {
            get { return mSig; }
            set
            {
                if ((value != null) && (value.Length != SigLength))
                    throw new ArgumentException("Incorrect array length.");

                mSig = value;
            }
        }

        /// <summary>
        /// Gets the name of the font.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c>. Can be an empty string.</para>
        /// </remarks>
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Gets or sets the alternate name for the font.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c>. Can be an empty string.</para>
        /// </remarks>
        public string AltName
        {
            get { return mAltName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                mAltName = value;
            }
        }

        /// <summary>
        /// Gets the embedded font licensing rights.
        /// </summary>
        /// <remarks>
        /// <para>The value may be null if font is not embedded.</para>
        /// </remarks>
        public FontEmbeddingLicensingRights EmbeddingLicensingRights
        {
            get
            {
                // Note: In theory there may be different fsType values on different embedded fonts.
                // Do not consider this case for now because it is a rare case. The font styles from the same family
                // should have the same fsType values. The difference may occur in case when font styles are not really
                // from the same family (e.g. from different font producers or different font versions).
                TTFont font = GetEmbeddedFontParsedAnyStyle();
                return font != null ? FontEmbeddingLicensingRights.FromFsType(font.FsType) : null;
            }
        }

        /// <summary>
        /// Returns true if character set of font is defined.
        /// </summary>
        internal bool IsCharsetDefined
        {
            get { return mCharset != FontUtil.UndefinedCharset; }
        }

        /// <summary>
        /// Returns UnicodeRanges. Returns <see cref="FontUnicodeRanges.Empty"/> if <see cref="Sig"/> is null
        /// </summary>
        internal FontUnicodeRanges GetUnicodeRanges()
        {
            FontUnicodeRanges ranges = (mSig == null) ? FontUnicodeRanges.Empty : new FontUnicodeRanges(mSig, 0);
            return ranges;
        }

        internal FontSubstitutionInfo GetSubstitutionInfo()
        {
            FontPanose panose = new FontPanose(mPanose);

            FontUnicodeRanges ranges = GetUnicodeRanges();

            FontCodepageRanges codepages =
                mSig == null
                    ? FontCodepageRanges.Empty
                    : new FontCodepageRanges(mSig, 16);

            return
                new FontSubstitutionInfo(panose, ranges, codepages, FontFamilyToCore(mFamily), FontPitchToCore(mPitch),
                    mCharset, mWeight);
        }

        private static FontFamilyCore FontFamilyToCore(FontFamily value)
        {
            switch (value)
            {
                case FontFamily.Auto:
                    return FontFamilyCore.Auto;
                case FontFamily.Roman:
                    return FontFamilyCore.Roman;
                case FontFamily.Swiss:
                    return FontFamilyCore.Swiss;
                case FontFamily.Modern:
                    return FontFamilyCore.Modern;
                case FontFamily.Script:
                    return FontFamilyCore.Script;
                case FontFamily.Decorative:
                    return FontFamilyCore.Decorative;
                default:
                    return FontFamilyCore.Auto;
            }
        }

        private static FontPitchCore FontPitchToCore(FontPitch value)
        {
            switch (value)
            {
                case FontPitch.Default:
                    return FontPitchCore.Default;
                case FontPitch.Fixed:
                    return FontPitchCore.Fixed;
                case FontPitch.Variable:
                    return FontPitchCore.Variable;
                default:
                    return FontPitchCore.Default;
            }
        }

        private FontPitch mPitch;
        private bool mIsTrueType = true;
        private FontFamily mFamily;
        private int mWeight;
        private int mCharset = FontUtil.UndefinedCharset;
        private string mIanaCharset;
        private byte[] mPanose;
        private byte[] mSig;
        private string mName = "";
        private string mAltName = "";
        private EmbeddedFontCollection mEmbeddedFonts;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int PanoseLength = 10;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int SigLength = 24;

        /// <summary>
        /// Seems that font name has maximum allowed length.
        /// </summary>
        internal const int MaxFontNameLength = 31;
    }
}
