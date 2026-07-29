// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2005 by Roman Korchagin

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Contains information about a glyph in a true type font.
    /// </summary>
    public class TTGlyph
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public TTGlyph(int glyphIndex, int advanceWidth, int leftSideBearing, int advanceHeight, int topSideBearing)
        {
            mGlyphIndex = glyphIndex;
            mAdvanceWidth = advanceWidth;
            mLeftSideBearing = leftSideBearing;
            mAdvanceHeight = advanceHeight;
            mTopSideBearing = topSideBearing;
        }


        /// <summary>
        /// One of the charcodes mapped to this glyph.
        /// May be zero if no charcodes are mapped.
        /// </summary>
        /// <remarks>
        /// Note that in font file it is a many to one relationship between charcode to glyphId. I.e. several charcodes may
        /// be mapped to a single glyph. Or glyph may not have a related charcode at all.
        /// So this field is a bit unnatural. It is kept here for now to keep the existing behavior.
        /// </remarks>
        public int CharCode
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mCharCode; }
            set { mCharCode = value; }
        }

        /// <summary>
        /// The original glyph index as read from the TTF file.
        /// </summary>
        /// <remarks>
        /// Note that when we subset a TTF file we write a different glyph index.
        /// </remarks>
        public int GlyphIndex
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mGlyphIndex; }
        }

        /// <summary>
        /// Advance width of the glyph in font design units.
        /// </summary>
        public int AdvanceWidth
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mAdvanceWidth; }
        }

        /// <summary>
        /// The left side bearing of the glyph (not sure what is this) in font design units.
        /// </summary>
        public int LeftSideBearing
        {
            get { return mLeftSideBearing; }
        }

        /// <summary>
        /// Advance height of the glyph in font design units (used for vertical writing).
        /// </summary>
        public int AdvanceHeight
        {
            get { return mAdvanceHeight; }
        }

        /// <summary>
        /// The top side bearing of the glyph in font design units (used for vertical writing).
        /// </summary>
        public int TopSideBearing
        {
            get { return mTopSideBearing; }
        }

        /// <summary>
        /// Information about colored version of a glyph.
        /// </summary>
        public TTGlyphColoredInfo ColoredInfo { get; set; }

        /// <summary>
        /// Returns true if this glyph may be colored.
        /// </summary>
        public bool IsColored
        {
            get { return ColoredInfo != null; }
        }

        private int mCharCode;
        private readonly int mGlyphIndex;
        private readonly int mAdvanceWidth;
        private readonly int mLeftSideBearing;
        private readonly int mAdvanceHeight;
        private readonly int mTopSideBearing;

#if DEBUG
        public override string ToString()
        {
            return string.Format("'{0}' \\{0}\t{1}", CharCode, GlyphIndex);
        }
#endif
    }
}
