// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2016 by Michael Morozoff

using System;
using System.Drawing;
using Aspose.Fonts.TrueType;

namespace Aspose.Drawing.Fonts
{
    /// <summary>
    /// Use this class instead of GDI+ Font to make code autoportable to Java.
    ///
    /// Why do we have this class instead of implementing ms.System.Drawing.Font?
    /// .NET's Font is very tightly related to GDI+ behavior, for example it only allows to use fonts properly installed in Windows.
    /// But we need to support Java and Mono and using a class that depends on GDI+ is not possible. Hence an automatically portable abstraction.
    /// </summary>
    public class DrFont
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="sizePoints">Size (points).</param>
        /// <param name="requestedStyle">Font style.</param>
        /// <param name="trueTypeFont">Base TrueType font.</param>
        /// <param name="isVertical">Indicates whether vertical metrics should be used.</param>
        /// <param name="adjustCjkFontMetrics">Indicates whether CJK font metrics should be adjusted.</param>
        /// <param name="useWord97FontMetrics">Indicates whether font metrics should rounded to replicate Word 97 behaviour.</param>
        public DrFont(float sizePoints,
            FontStyle requestedStyle,
            TTFont trueTypeFont,
            bool isVertical,
            bool adjustCjkFontMetrics,
            bool useWord97FontMetrics)
            : this(sizePoints, requestedStyle, trueTypeFont, isVertical, adjustCjkFontMetrics, useWord97FontMetrics, false, false, false)
        {
        }
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="sizePoints">Size (points).</param>
        /// <param name="requestedStyle">Font style.</param>
        /// <param name="trueTypeFont">Base TrueType font.</param>
        /// <param name="isVertical">Indicates whether vertical metrics should be used.</param>
        /// <param name="adjustCjkFontMetrics">Indicates whether CJK font metrics should be adjusted.</param>
        /// <param name="useWord97FontMetrics">Indicates whether font metrics should rounded to replicate Word 97 behaviour.</param>
        /// <param name="isSmallCaps">Indicates whether font size should be calculated for small capitals.</param>
        /// <param name="isSuperscript">Indicates whether the font size should be calculated for superscript text.</param>
        /// <param name="isSubscript">Indicates whether font size should be calculated for subscript text.</param>
        public DrFont(float sizePoints,
            FontStyle requestedStyle,
            TTFont trueTypeFont,
            bool isVertical,
            bool adjustCjkFontMetrics,
            bool useWord97FontMetrics,
            bool isSmallCaps,
            bool isSuperscript,
            bool isSubscript)
        {
            if (trueTypeFont == null)
                throw new ArgumentNullException("trueTypeFont");

            mTTFont = trueTypeFont;
            mRequestedStyle = requestedStyle;
            mIsVertical = isVertical;
            mAdjustCjkFontMetrics = adjustCjkFontMetrics;
            mUseWord97FontMetrics = useWord97FontMetrics;
            mHalfAvgCharWidthToSpaceWidth = (mTTFont.AvgCharWidth / 2f) / mTTFont.GetCharAdvanceWidthDesignUnits(SpaceCharCode);

            mSizePoints = (isSmallCaps || isSuperscript || isSubscript) ?
                GetRenderSize(sizePoints, isSmallCaps, isSuperscript, isSubscript) :
                sizePoints;
        }

        /// <summary>
        /// Gets font metrics used by this instance. This is normally <see cref="TTFontMetrics"/> but on .NET can also be set to printer metrics.
        /// </summary>
        /// <remarks>
        /// This property has been introduced to allow using font metrics other than TTF ones when
        /// the "Use printer metrics to lay out document" compatibility option is enabled for document.
        /// </remarks>
        private IDrFontMetrics Metrics
        {
            get
            {
                // If no metrics has been assigned, default to the TTF metrics.
                if (mMetrics == null)
                {
                    mMetrics = mIsVertical
                        ? (IDrFontMetrics)new TTFontVerticalMetrics(mTTFont, mSizePoints, IsSimulateBold, mAdjustCjkFontMetrics)
                        : new TTFontHorizontalMetrics(mTTFont, mSizePoints, IsSimulateBold, mAdjustCjkFontMetrics, mUseWord97FontMetrics);
                }

                return mMetrics;
            }
        }

        /// <summary>
        /// The requested font style. It may not be equal to the underlying <see cref="TTFont"/> style.
        /// </summary>
        public FontStyle RequestedStyle
        {
            get { return mRequestedStyle; }
        }

        /// <summary>
        /// Returns true if bold style simulation should be performed.
        /// </summary>
        public bool IsSimulateBold
        {
            get { return ((mRequestedStyle & FontStyle.Bold) != 0 && !mTTFont.IsBold); }
        }

        /// <summary>
        /// Gets name of this font.
        /// </summary>
        public string FamilyName
        {
            get { return mTTFont.FamilyName; }
        }

        /// <summary>
        /// Gets size of this font (points).
        /// </summary>
        public float SizePoints
        {
            get { return mSizePoints; }
        }

        /// <summary>
        /// Returns the cell ascent in points.
        /// </summary>
        public float AscentPoints
        {
            get { return Metrics.AscentPoints; }
        }

        /// <summary>
        /// Returns the cell descent in points.
        /// </summary>
        public float DescentPoints
        {
            get { return Metrics.DescentPoints; }
        }

        /// <summary>
        /// Returns the cell original ascent in points.
        /// </summary>
        public float AscentRawPoints
        {
            get { return Metrics.AscentRawPoints; }
        }

        /// <summary>
        /// Returns the cell original descent in points.
        /// </summary>
        public float DescentRawPoints
        {
            get { return Metrics.DescentRawPoints; }
        }

        /// <summary>
        /// Shortcut for <see cref="AscentPoints"/> + <see cref="DescentPoints"/>.
        /// </summary>
        public float CellHeightPoints
        {
            get { return AscentPoints + DescentPoints; }
        }

        /// <summary>
        /// Returns the internal leading (points).
        /// </summary>
        public float InternalLeadingPoints
        {
            get { return (AscentPoints + DescentPoints) - SizePoints; }
        }

        /// <summary>
        /// Returns the ascent with the suppressed top spacing (points).
        /// </summary>
        /// <remarks>
        /// This kind of ascent is used when the "Suppress extra line spacing the way WordPerfect 5.x does" compatibility option is enabled.
        /// </remarks>
        public float AscentSuppressedTopSpacingWPPoints
        {
            get { return SizePoints + 2f - DescentPoints; }
        }

        /// <summary>
        /// Returns cell spacing of this font (lis).
        /// This is a vertical distance between baselines of the two glyphs.
        /// </summary>
        public int LineSpacingLis
        {
            get
            {
                if (mLineSpacingLis == -1)
                    mLineSpacingLis = ConvertUtilCore.PointToLi(LineSpacingPoints);

                return mLineSpacingLis;
            }
        }

        /// <summary>
        /// Returns cell spacing of this font (points).
        /// This is a vertical distance between baselines of the two glyphs.
        /// </summary>
        public float LineSpacingPoints
        {
            get { return Metrics.LineSpacingPoints; }
        }

        /// <summary>
        /// Returns width of the character (points).
        /// </summary>
        public float GetCharWidthPoints(int c)
        {
            return Metrics.GetCharWidthPoints(c, mSizePoints);
        }

        public float GetTextWidthPoints(string text)
        {
            return Metrics.GetTextWidthPoints(text, mSizePoints);
        }

        /// <summary>
        /// Returns measurement text box of the text in points.
        /// </summary>
        public SizeF GetTextSizePoints(string text)
        {
            return new SizeF(GetTextWidthPoints(text), CellHeightPoints);
        }

        /// <summary>
        /// Cell ascent of this font (lis).
        /// This is a vertical distance from cell top to cell baseline.
        /// </summary>
        /// <remarks>This value is also called <b>cell baseline</b>.</remarks>
        public int AscentLis
        {
            get
            {
                if (mAscentLis == -1)
                    mAscentLis = ConvertUtilCore.PointToLi(AscentPoints);

                return mAscentLis;
            }
        }

        /// <summary>
        /// Cell descent of this font (lis).
        /// This is a vertical distance from cell bottom to cell baseline.
        /// </summary>
        public int DescentLis
        {
            get
            {
                if (mDescentLis == -1)
                    mDescentLis = ConvertUtilCore.PointToLi(DescentPoints);

                return mDescentLis;
            }
        }

        /// <summary>
        /// Returns cell height of this font (lis).
        /// This is a shortcut for <see cref="AscentLis"/> + <see cref="DescentLis"/>.
        /// </summary>
        public int CellHeightLis
        {
            get
            {
                if (mCellHeightLis == -1)
                    mCellHeightLis = AscentLis + DescentLis;

                return mCellHeightLis;
            }
        }

        /// <summary>
        /// Ratio of half of the average character width to the space width.
        /// </summary>
        /// <remarks>
        /// Ratio used for the space shrinkability calculation.
        /// </remarks>>
        public float HalfAvgCharWidthToSpaceWidth
        {
            get { return mHalfAvgCharWidthToSpaceWidth; }
        }

        /// <summary>
        /// Indicates whether at least one of the CJK Unicode ranges is considered functional in this font.
        /// </summary>
        /// <remarks>
        /// This property is mainly used to detect whether the font is Asian.
        /// </remarks>
        public bool IsCjkSupported
        {
            get { return mTTFont.IsCjkMetrics; }
        }

        private float SubscriptFactor
        {
            get { return (float)SubscriptSize / mTTFont.EmHeight; }
        }

        private float SupersciptFactor
        {
            get { return (float)SuperscriptSize / mTTFont.EmHeight; }
        }

        private int SuperscriptSize
        {
            get { return mTTFont.SuperscriptSize; }
        }

        private int SubscriptSize
        {
            get { return mTTFont.SubscriptSize; }
        }

        private int SuperscriptOffset
        {
            get { return mTTFont.SuperscriptOffset; }
        }

        private int SubscriptOffset
        {
            get { return mTTFont.SubscriptOffset; }
        }

        /// <summary>
        /// Returns font size which depends on case, caps and script of span. This is size
        /// of the font which is used for span rendering, not for measurements.
        /// </summary>
        /// <remarks>
        /// The actual size could be smaller than the specified font size for small caps
        /// and/or subscript/superscript font.
        /// </remarks>
        private float GetRenderSize(float sizePoints, bool isSmallCaps, bool isSuperscript, bool isSubscript)
        {
            // 1) Normal | (SmallCaps & !isLower)                   = 1
            // 2) SmallCaps & Lower                                 = *SmallCapsSizeFactor
            // 3) Subscript | Superscript                           = *SubscriptSizeFactor
            // 4) SmallCaps & Lower & (Subscript | Superscript)     = *SmallCapsSizeFactor *SubscriptSizeFactor

            // We use half points because Word rounds this way.
            float result = ConvertUtilCore.PointToHalfPoint(sizePoints);

            float superscriptFactor = SupersciptFactor;
            float subscriptFactor = SubscriptFactor;

            if (isSuperscript || isSubscript)
            {
                if (IsRegularScriptScalingApplicable((int)result))
                {
                    if (isSuperscript)
                        result = result * superscriptFactor;
                    else if (isSubscript)
                        result = result * subscriptFactor;
                }
                else
                {
                    result = result * ScriptFactor;
                    if (result < MinFontSizeHalfPoints)
                        result = MinFontSizeHalfPoints;
                }
            }

            if (isSmallCaps)
                result = result * SmallCapsSizeFactor;

            // WORDSNET-3751 It seems that MS Word performs rounding to half points after applying size factor.
            return (float)ConvertUtilCore.HalfPointToPoint((int)Math.Round(result, MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// Indicates whether superscript and subscript font sizes should be based on the OS/2 table data.
        /// </summary>
        /// <param name="fontSize">The font size in half points.</param>
        private bool IsRegularScriptScalingApplicable(int fontSize)
        {
            int oneHalf = fontSize / 2;
            int oneForth = fontSize / 4;
            int threeForth = oneHalf + oneForth;
            int oneThird = fontSize / 3 + 1;

            int superscriptSize = DesignUnitsToHalfPointsRounded(SuperscriptSize, fontSize);
            int subscriptSize = DesignUnitsToHalfPointsRounded(SubscriptSize, fontSize);
            int superscriptOffset = DesignUnitsToHalfPointsRounded(SuperscriptOffset, fontSize);
            int subscriptOffset = DesignUnitsToHalfPointsRounded(SubscriptOffset, fontSize);

            return (oneThird <= superscriptSize && superscriptSize <= fontSize) &&
                   (oneThird <= subscriptSize && subscriptSize <= fontSize) &&
                   (0 <= subscriptOffset && subscriptOffset <= oneHalf) &&
                   (oneForth <= superscriptOffset && superscriptOffset <= threeForth);
        }

        private int DesignUnitsToHalfPointsRounded(int designUnits, int halfPoints)
        {
            return (int)Math.Round(mTTFont.DesignUnitsToPoints(designUnits, halfPoints), MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// This is additional magnification factor used for lowercase case small which is
        /// at the same time subscript or superscript.
        /// </summary>
        private const float SmallCapsSizeFactor = 0.80f;

        /// <summary>
        /// This factor is used for superscript and subscript when conditions in <see cref="IsRegularScriptScalingApplicable(int)"/> are not met.
        /// </summary>
        private const float ScriptFactor = 0.6f;

        private const int MinFontSizeHalfPoints = 2;

        private readonly TTFont mTTFont;
        private readonly FontStyle mRequestedStyle;
        private readonly float mSizePoints;
        private IDrFontMetrics mMetrics;
        /// <summary>
        /// Indicates whether vertical metrics should be used instead of horizontal metrics,
        /// i.e. advance height is used instead of advance width.
        /// </summary>
        private readonly bool mIsVertical;
        private readonly bool mAdjustCjkFontMetrics;
        private readonly bool mUseWord97FontMetrics;
        private int mCellHeightLis = -1;
        private int mLineSpacingLis = -1;
        private int mAscentLis = -1;
        private int mDescentLis = -1;
        private readonly float mHalfAvgCharWidthToSpaceWidth;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int SpaceCharCode = 32;
    }
}
