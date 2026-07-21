// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/01/2011 by Andrey Soldatov

using System;
using Aspose.Fonts.EmbeddedOpenType;
using Aspose.Fonts.TrueType;

namespace Aspose.Words.Fonts
{
    /// <summary>
    /// <para>Contains embedded font data, its format, style and flag whether font data subsetted
    /// (i.e. contains incomplete set of characters).</para>
    /// </summary>
    internal class EmbeddedFont
    {
        /// <summary>
        /// Builds <see cref="EmbeddedFont"/> object from its components.
        /// </summary>
        /// <param name="fontData"></param>
        /// <param name="format"></param>
        /// <param name="style"></param>
        /// <param name="isSubsetted">Specifies whether font data is complete or contains only subset of characters.</param>
        internal EmbeddedFont(byte[] fontData, EmbeddedFontFormat format, EmbeddedFontStyle style, bool isSubsetted)
        {
            if (fontData == null)
                throw new ArgumentNullException("fontData");

            mData = fontData;
            mFormat = format;
            mStyle = style;
            mIsSubsetted = isSubsetted;
        }

        /// <summary>
        /// Creates embedded font of type <see cref="EmbeddedFontFormat.OpenType"/> from this embedded font.
        /// If it has this type already, then returns this instance.
        /// On error returns null.
        /// </summary>
        /// <remarks>
        /// We do not create new instance of this object or clone it
        /// if it has type <see cref="EmbeddedFontFormat.OpenType"/> already,
        /// because this object is immutable in concept and it can be very large.
        /// </remarks>
        internal EmbeddedFont GetAsOpenType()
        {
            if (mData == null)
                return null;

            if (mFormat == EmbeddedFontFormat.OpenType)
                return this;

            byte[] openTypeData = EotFontFiler.TryExtractOpenTypeFromEot(mData);

            return (openTypeData == null) ? null : new EmbeddedFont(openTypeData, EmbeddedFontFormat.OpenType, Style, IsSubsetted);
        }

        /// <summary>
        /// Creates embedded font of type <see cref="EmbeddedFontFormat.EmbeddedOpenType"/> from this embedded font.
        /// If it has this type already, then returns this instance.
        /// On error returns null.
        /// </summary>
        /// <remarks>
        /// We do not create new instance of this object or clone it
        /// if it has type <see cref="EmbeddedFontFormat.EmbeddedOpenType"/> already,
        /// because this object is immutable in concept and it can be very large.
        /// </remarks>
        internal EmbeddedFont GetAsEot(string fontName)
        {
            if (mData == null)
                return null;

            if (mFormat == EmbeddedFontFormat.EmbeddedOpenType)
                return this;

            byte[] eotData = null;
            try
            {
                eotData = EotFontFiler.CompressOpenTypeToEot(mData, fontName, IsSubsetted);
            }
            catch
            {
                // Ignore any errors.
            }

            if (eotData == null)
                return null;

            return new EmbeddedFont(eotData, EmbeddedFontFormat.EmbeddedOpenType, Style, IsSubsetted);
        }

        /// <summary>
        /// Gets embedded font data.
        /// </summary>
        internal byte[] Data
        {
            get { return mData; }
        }

        /// <summary>
        /// Indicates format of embedded font data. See <see cref="EmbeddedFontFormat"/>.
        /// </summary>
        internal EmbeddedFontFormat Format
        {
            get { return mFormat; }
        }

        /// <summary>
        /// Indicates style of embedded font data. See <see cref="EmbeddedFontStyle"/>.
        /// </summary>
        internal EmbeddedFontStyle Style
        {
            get { return mStyle; }
        }

        /// <summary>
        /// Indicates whether font data is complete or contains only subset of characters.
        /// </summary>
        internal bool IsSubsetted
        {
            get { return mIsSubsetted; }
        }

        private readonly byte[] mData;
        private readonly EmbeddedFontFormat mFormat;
        private readonly EmbeddedFontStyle mStyle;
        private readonly bool mIsSubsetted;
    }
}
