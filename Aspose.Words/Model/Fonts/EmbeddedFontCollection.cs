// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/04/2011 by Andrey Soldatov

using System.Collections.Generic;
using Aspose.Fonts;
using Aspose.Fonts.EmbeddedOpenType;
using Aspose.Fonts.TrueType;
using Aspose.JavaAttributes;

namespace Aspose.Words.Fonts
{
    internal class EmbeddedFontCollection
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal EmbeddedFontCollection()
        {
            mEmbeddedFonts = new EmbeddedFont[FormatsQuantity][];
            for (int i = 0; i < FormatsQuantity; i++)
                mEmbeddedFonts[i] = new EmbeddedFont[StylesQuantity];
        }

        /// <summary>
        /// Need to clone collection of embedded fonts, but not embedded fonts itself. EmbeddedFont is object
        /// immutable in concept and large enough to not want to clone it, so just assign references.
        /// </summary>
        internal EmbeddedFontCollection Clone()
        {
            EmbeddedFontCollection lhs = new EmbeddedFontCollection();

            for (int formatIndex = 0; formatIndex < mEmbeddedFonts.Length; formatIndex++)
            {
                EmbeddedFont[] source = mEmbeddedFonts[formatIndex];
                EmbeddedFont[] copy = new EmbeddedFont[source.Length];
                source.CopyTo(copy, 0);
                lhs.mEmbeddedFonts[formatIndex] = copy;
            }

            lhs.ClearParsedFontsCache();

            return lhs;
        }

        internal void Merge(EmbeddedFontCollection srcEmbeddedFontCollection)
        {
            Debug.Assert(srcEmbeddedFontCollection != null);

            for (int format = 0; format < FormatsQuantity; format++)
            {
                EmbeddedFont[] srcEmbeddedFontsOfSpecificFormat = srcEmbeddedFontCollection.mEmbeddedFonts[format];

                for (int style = 0; style < StylesQuantity; style++)
                {
                    EmbeddedFont srcEmbeddedFont = srcEmbeddedFontsOfSpecificFormat[style];

                    if (srcEmbeddedFont == null)
                        continue;

                    EmbeddedFont myEmbeddedFont = mEmbeddedFonts[format][style];

                    if ((myEmbeddedFont == null) ||
                        (myEmbeddedFont.IsSubsetted && !srcEmbeddedFont.IsSubsetted))
                    {
                        // AS EmbeddedFont is object immutable in concept and large enough to not want to clone it,
                        // so just assign reference.
                        mEmbeddedFonts[format][style] = srcEmbeddedFont;
                    }
                }
            }

            ClearParsedFontsCache();
        }

        /// <summary>
        /// <para>Provides access to embedded fonts of specific format to iterate over them.</para>
        /// </summary>
        /// <remarks>
        /// <para>Can not be <c>null</c>. Each of elements of returned array can be <c>null</c>.</para>
        /// </remarks>
        internal EmbeddedFont[] GetFonts(EmbeddedFontFormat fontFormat)
        {
            return mEmbeddedFonts[(int)fontFormat];
        }

        /// <summary>
        /// <para>Provides access to particular embedded font file.</para>
        /// <para>Returns <c>null</c> if embedded font is absent.</para>
        /// </summary>
        internal EmbeddedFont Get(EmbeddedFontFormat fontFormat, EmbeddedFontStyle embeddedFontStyle)
        {
            return mEmbeddedFonts[(int)fontFormat][(int)embeddedFontStyle];
        }

        /// <summary>
        /// Adds new embedded font to the collection. If font with the same <see cref="EmbeddedFontFormat"/>
        /// and <see cref="EmbeddedFontStyle"/> already exists in the collection, replaces it.
        /// </summary>
        /// <param name="font"></param>
        internal void Add(EmbeddedFont font)
        {
            mEmbeddedFonts[(int)font.Format][(int)font.Style] = font;

            ClearParsedFontsCache();
        }

        internal TTFont GetTtFont(EmbeddedFontStyle style)
        {
            ParseEmbeddedData();
            return mParsedFontsCache[(int)style];
        }

        internal TTFont GetTtFontAnyStyle()
        {
            ParseEmbeddedData();
            foreach (TTFont font in mParsedFontsCache)
            {
                if (font != null)
                    return font;
            }

            return null;
        }

        private void ClearParsedFontsCache()
        {
            mIsEmbeddedDataParsed = false;
            for (int i = 0; i < StylesQuantity; i++)
                mParsedFontsCache[i] = null;
        }

        private void ParseEmbeddedData()
        {
            if (mIsEmbeddedDataParsed)
                return;

            mIsEmbeddedDataParsed = true;

            TTFont[] subsets = new TTFont[StylesQuantity];
            subsets[(int)EmbeddedFontStyle.Regular] = ReadFont(GetOpenTypeData(EmbeddedFontStyle.Regular));
            subsets[(int)EmbeddedFontStyle.Bold] = ReadFont(GetOpenTypeData(EmbeddedFontStyle.Bold));
            subsets[(int)EmbeddedFontStyle.Italic] = ReadFont(GetOpenTypeData(EmbeddedFontStyle.Italic));
            subsets[(int)EmbeddedFontStyle.BoldItalic] = ReadFont(GetOpenTypeData(EmbeddedFontStyle.BoldItalic));

            // In case of style simulation MW splits font subsets per style for some reason. We should join subsets
            // in order to process them properly.

            // All other styles could be simulated from regular style. Only BoldItalic style could be simulated from Bold or
            // Italic style.
            if (subsets[(int)EmbeddedFontStyle.Regular] != null)
            {
                mParsedFontsCache[(int)EmbeddedFontStyle.Regular] = subsets[(int)EmbeddedFontStyle.Regular];
                JoinSubsets(subsets, EmbeddedFontStyle.Regular, EmbeddedFontStyle.Bold, EmbeddedFontStyle.Italic,
                    EmbeddedFontStyle.BoldItalic);
            }

            if (mParsedFontsCache[(int)EmbeddedFontStyle.Bold] == null && subsets[(int)EmbeddedFontStyle.Bold] != null)
            {
                mParsedFontsCache[(int)EmbeddedFontStyle.Bold] = subsets[(int)EmbeddedFontStyle.Bold];
                JoinSubsets(subsets, EmbeddedFontStyle.Bold, EmbeddedFontStyle.BoldItalic);
            }

            if (mParsedFontsCache[(int)EmbeddedFontStyle.Italic] == null && subsets[(int)EmbeddedFontStyle.Italic] != null)
            {
                mParsedFontsCache[(int)EmbeddedFontStyle.Italic] = subsets[(int)EmbeddedFontStyle.Italic];
                JoinSubsets(subsets, EmbeddedFontStyle.Italic, EmbeddedFontStyle.BoldItalic);
            }

            if (mParsedFontsCache[(int)EmbeddedFontStyle.BoldItalic] == null && subsets[(int)EmbeddedFontStyle.BoldItalic] != null)
                mParsedFontsCache[(int)EmbeddedFontStyle.BoldItalic] = subsets[(int)EmbeddedFontStyle.BoldItalic];
        }

        private static bool CanJoinSubsets(TTFont[] subsets, EmbeddedFontStyle style1, EmbeddedFontStyle style2)
        {
            TTFont sub1 = subsets[(int)style1];
            TTFont sub2 = subsets[(int)style2];

            if (sub1 == null || sub2 == null)
                return false;

            // When splitting subsets MW keeps source FullFontName and Style.
            return StringUtil.EqualsIgnoreCase(sub1.FullFontName, sub2.FullFontName) && sub1.Style == sub2.Style;
        }

        private void JoinSubsets(TTFont[] subsets, EmbeddedFontStyle baseStyle, params EmbeddedFontStyle[] otherStyles)
        {
            List<EmbeddedFontStyle> joinStyles = new List<EmbeddedFontStyle>();
            joinStyles.Add(baseStyle);
            foreach (EmbeddedFontStyle style in otherStyles)
            {
                if (CanJoinSubsets(subsets, baseStyle, style))
                    joinStyles.Add(style);
            }

            if (joinStyles.Count <= 1)
                return;

            List<TTFont> joinFonts = new List<TTFont>();
            foreach (EmbeddedFontStyle style in joinStyles)
                joinFonts.Add(subsets[(int)style]);

            TTFont joinedFont = TTFontFiler.JoinEmbeddedSubsets(joinFonts);

            mParsedFontsCache[(int)baseStyle] = joinedFont;
            foreach (EmbeddedFontStyle style in joinStyles)
                mParsedFontsCache[(int)style] = joinedFont;
        }

        internal byte[] GetOpenTypeData(EmbeddedFontStyle style)
        {
            EmbeddedFont font = Get(EmbeddedFontFormat.OpenType, style);
            if (font != null && font.Data != null)
                return font.Data;

            font = Get(EmbeddedFontFormat.EmbeddedOpenType, style);
            return ((font != null) && (font.Data != null))
                ? EotFontFiler.TryExtractOpenTypeFromEot(font.Data)
                : null;
        }

        [JavaThrows(false)]
        private static TTFont ReadFont(byte[] fontBytes)
        {
            if (fontBytes == null)
                return null;

            try
            {
                PhysicalFontData fontData = new PhysicalFontData(new MemoryFontData(fontBytes, null, true));
                return TTFontBuilder.Read(fontData);
            }
            catch
            {
                // If there are exceptions while parsing the font then simply return null.
                return null;
            }
        }

        /// <summary>
        /// <para>First index is (int)<see cref="EmbeddedFontFormat"/>.</para>
        /// <para>Second index is (int)<see cref="EmbeddedFontStyle"/>.</para>
        /// </summary>
        private readonly EmbeddedFont[][] mEmbeddedFonts;

        private const int FormatsQuantity = 2;
        private const int StylesQuantity = 4;

        private bool mIsEmbeddedDataParsed;
        private readonly TTFont[] mParsedFontsCache = new TTFont[StylesQuantity];
    }
}
