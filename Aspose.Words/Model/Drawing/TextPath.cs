// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/07/2006 by Roman Korchagin


using Aspose.Words.Drawing.Core;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Defines the text and formatting of the text path (of a WordArt object).
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-shapes/">Working with Shapes</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Use the <see cref="Shape.TextPath"/> property to access WordArt properties of a shape.
    /// You do not create instances of the <see cref="TextPath"/> class directly.</p>
    /// 
    /// <seealso cref="Shape.TextPath"/>
    /// </remarks>
    public class TextPath
    {
        internal TextPath(IShapeAttrSource parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// Defines whether the text is displayed. 
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool On
        {
            get { return (bool)FetchAttr(ShapeAttr.GeoTextOn); }
            set { SetAttr(ShapeAttr.GeoTextOn, value); }
        }

        /// <summary>
        /// Defines whether the text fits the path of a shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool FitPath
        {
            get { return (bool)FetchAttr(ShapeAttr.GeoTextBestFit); }
            set { SetAttr(ShapeAttr.GeoTextBestFit, value); }
        }

        /// <summary>
        /// Defines whether the text fits bounding box of a shape. 
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool FitShape
        {
            get { return (bool)FetchAttr(ShapeAttr.GeoTextStretch); }
            set { SetAttr(ShapeAttr.GeoTextStretch, value); }
        }

        /// <summary>
        /// Defines the family of the textpath font. 
        /// </summary>
        /// <remarks>
        /// <p>The default value is Arial.</p>
        /// </remarks>
        public string FontFamily
        {
            get { return (string)FetchAttr(ShapeAttr.GeoTextFont); }
            set { SetAttr(ShapeAttr.GeoTextFont, value); }
        }

        /// <summary>
        /// Defines the size of the font in points.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 36.</p>
        /// </remarks>
        public double Size
        {
            get { return ConvertUtilCore.FixedToDouble((int)FetchAttr(ShapeAttr.GeoTextSize)); }
            set { SetAttr(ShapeAttr.GeoTextSize, ConvertUtilCore.DoubleToFixed(value)); }
        }

        /// <summary>
        /// True if the font is formatted as bold.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool Bold
        {
            get { return (bool)FetchAttr(ShapeAttr.GeoTextBold); }
            set { SetAttr(ShapeAttr.GeoTextBold, value); }
        }

        /// <summary>
        /// True if the font is formatted as italic.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool Italic
        {
            get { return (bool)FetchAttr(ShapeAttr.GeoTextItalic); }
            set { SetAttr(ShapeAttr.GeoTextItalic, value); }
        }

        /// <summary>
        /// True if the font is formatted as small capital letters.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool SmallCaps
        {
            get { return (bool)FetchAttr(ShapeAttr.GeoTextSmallCaps); }
            set { SetAttr(ShapeAttr.GeoTextSmallCaps, value); }
        }

        /// <summary>
        /// Determines whether the letters of the text are rotated. 
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool RotateLetters
        {
            get { return (bool)FetchAttr(ShapeAttr.GeoTextVertical); }
            set { SetAttr(ShapeAttr.GeoTextVertical, value); }
        }

        /// <summary>
        /// Determines whether extra space is removed above and below the text.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool Trim
        {
            get { return (bool)FetchAttr(ShapeAttr.GeoTextShrinkFit); }
            set { SetAttr(ShapeAttr.GeoTextShrinkFit, value); }
        }
        
        /// <summary>
        /// Determines whether kerning is turned on.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool Kerning
        {
            get { return (bool)FetchAttr(ShapeAttr.GeoTextKerning); }
            set { SetAttr(ShapeAttr.GeoTextKerning, value); }
        }

        /// <summary>
        /// Defines whether a shadow is applied to the text on a text path.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool Shadow
        {
            get { return (bool)FetchAttr(ShapeAttr.GeoTextShadow); }
            set { SetAttr(ShapeAttr.GeoTextShadow, value); }
        }

        /// <summary>
        /// True if the font is underlined.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool Underline
        {
            get { return (bool)FetchAttr(ShapeAttr.GeoTextUnderline); }
            set { SetAttr(ShapeAttr.GeoTextUnderline, value); }
        }

        /// <summary>
        /// True if the font is formatted as strikethrough text.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool StrikeThrough
        {
            get { return (bool)FetchAttr(ShapeAttr.GeoTextStrikeThrough); }
            set { SetAttr(ShapeAttr.GeoTextStrikeThrough, value); }
        }

        /// <summary>
        /// Determines whether all letters will be the same height regardless of initial case.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool SameLetterHeights
        {
            get { return (bool)FetchAttr(ShapeAttr.GeoTextNormalize); }
            set { SetAttr(ShapeAttr.GeoTextNormalize, value); }
        }

        /// <summary>
        /// Defines the text of the text path.
        /// </summary>
        /// <remarks>
        /// <p>The default value is an empty string.</p>
        /// </remarks>
        public string Text
        {
            get { return (string)FetchAttr(ShapeAttr.GeoTextText); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetAttr(ShapeAttr.GeoTextText, value);
            }
        }

        /// <summary>
        /// Defines the alignment of text.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.TextPathAlignment.Center"/>.</p>
        /// </remarks>
        public TextPathAlignment TextPathAlignment
        {
            get { return (TextPathAlignment)FetchAttr(ShapeAttr.GeoTextAlign); }
            set { SetAttr(ShapeAttr.GeoTextAlign, value); }
        }

        /// <summary>
        /// Determines whether the layout order of rows is reversed.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// <p>If <c>true</c>, the layout order of rows is reversed. This attribute is used for vertical text layout.</p>
        /// </remarks>
        public bool ReverseRows
        {
            get { return (bool)FetchAttr(ShapeAttr.GeoTextReverseRows); }
            set { SetAttr(ShapeAttr.GeoTextReverseRows, value); }
        }

        /// <summary>
        /// Defines the amount of spacing for text. 1 means 100%.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1.</p>
        /// </remarks>
        public double Spacing
        {
            get { return ConvertUtilCore.FixedToDouble((int)FetchAttr(ShapeAttr.GeoTextSpacing)); }
            set
            {
                SetAttr(ShapeAttr.GeoTextSpacing, ConvertUtilCore.DoubleToFixed(value));
                SetAttr(ShapeAttr.GeoTextTight, true);
            }
        }

        /// <summary>
        /// Determines whether a straight textpath will be used instead of the shape path.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// <p>If <c>true</c>, the text runs along a path from left to right along the x value of 
        /// the lower boundary of the shape.</p>
        /// </remarks>
        public bool XScale
        {
            get { return (bool)FetchAttr(ShapeAttr.GeoTextDxMeasure); }
            set { SetAttr(ShapeAttr.GeoTextDxMeasure, value); }
        }
        
        private object FetchAttr(int key)
        {
            return mParent.FetchShapeAttr(key);
        }

        private void SetAttr(int key, object value)
        {
            mParent.SetShapeAttr(key, value);
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IShapeAttrSource mParent;
    }
}
