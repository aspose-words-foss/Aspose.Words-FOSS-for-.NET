// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/09/2006 by Vladimir Averkin

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Writes v:textpath subelement of v:shape to WordML.
    /// </summary>
    internal class VmlGeoTextWriter 
    {
        internal VmlGeoTextWriter(NrxXmlBuilder builder)
        {
            mBuilder = builder;
            mVmlBuilder = new VmlBuilder(builder);
        }

        /// <summary>
        /// Add v:textpath related attribute.
        /// </summary>
        /// <param name="key">Attribute key.</param>
        /// <param name="value">Attribute value.</param>
        internal void AddAttribute(int key, object value)
        {
            mAttrCount++;

            switch (key)
            {
                case ShapeAttr.GeoTextOn:
                {
                    mOn = VmlUtil.BoolToVml(value);
                    break;
                }
                case ShapeAttr.GeoTextStretch:
                {
                    mFitShape = VmlUtil.BoolToVml(value);
                    break;
                }
                case ShapeAttr.GeoTextShrinkFit:
                {
                    mTrim = VmlUtil.BoolToVml(value);
                    break;
                }
                case ShapeAttr.GeoTextBestFit:
                {
                    mFitPath = VmlUtil.BoolToVml(value);
                    break;
                }
                case ShapeAttr.GeoTextDxMeasure:
                {
                    mXScale = VmlUtil.BoolToVml(value);
                    break;
                }
                case ShapeAttr.GeoTextText:
                {
                    // WORDSNET-9375 According to MSW behavior, we should use LF instead of CRLF.
                    mString = ((string)value).Replace("\r\n", "\n");
                    break;
                }
                case ShapeAttr.GeoTextFont:
                {
                    mFontFamily = (string)value;
                    break;
                }
                case ShapeAttr.GeoTextSize:
                {
                    mFontSize = VmlUtil.FixedToVml((int)value) + "pt";
                    break;
                }
                case ShapeAttr.GeoTextItalic:
                {
                    if ((bool)value)
                        mFontStyle = "italic";
                    break;
                }
                case ShapeAttr.GeoTextBold:
                {
                    if ((bool)value)
                        mFontWeight = "bold";
                    break;
                }
                case ShapeAttr.GeoTextSmallCaps:
                {
                    if ((bool)value)
                        mFontVariant = "small-caps";
                    break;
                }
                case ShapeAttr.GeoTextUnderline:
                {
                    if ((bool)value)
                        mTextDecoration = "underline";
                    break;
                }
                case ShapeAttr.GeoTextStrikeThrough:
                {
                    if ((bool)value)
                        mTextDecoration = "line-through";
                    break;
                }
                case ShapeAttr.GeoTextShadow:
                {
                    if ((bool)value)
                        mMsoTextShadow = "auto";
                    break;
                }
                case ShapeAttr.GeoTextAlign:
                {
                    mVTextAlign = VmlEnum.TextPathAlignmentToVml((TextPathAlignment)value);
                    break;
                }
                case ShapeAttr.GeoTextTight:
                {
                    if ((bool)value)
                        mVTextSpacingMode = "tightening";
                    break;
                }
                case ShapeAttr.GeoTextSpacing:
                {
                    mVTextSpacing = VmlUtil.FixedToVml((int)value);
                    break;
                }
                case ShapeAttr.GeoTextKerning:
                {
                    mVTextKern = VmlUtil.BoolToVml(value);
                    break;
                }
                case ShapeAttr.GeoTextReverseRows:
                {
                    mVTextReverse = VmlUtil.BoolToVml(value);
                    break;
                }
                case ShapeAttr.GeoTextNormalize:
                {
                    mVSameLetterHeights = VmlUtil.BoolToVml(value);
                    break;
                }
                case ShapeAttr.GeoTextVertical:
                {
                    mVRotateLetters = VmlUtil.BoolToVml(value);
                    break;
                }
                default:
                    return;
            }
        }

        /// <summary>
        /// Write 'v:textpath' element.
        /// </summary>
        internal void Write()
        {
            if (mAttrCount > 0)
            {
                mBuilder.StartElement("v:textpath");

                mVmlBuilder.WriteVmlAttributeIfNotDefault("on", mOn, "t");

                VmlCssStyleBuilder style = new VmlCssStyleBuilder();

                // Do not write if we don't have the value because on Java it might appear as "null".
                if (StringUtil.HasChars(mFontFamily))
                    style.AddFontFamily(mFontFamily);

                style.Add("font-size", mFontSize);
                style.Add("font-weight", mFontWeight);
                style.Add("font-style", mFontStyle);
                style.Add("font-variant", mFontVariant);
                style.Add("v-text-align", mVTextAlign);
                style.Add("v-rotate-letters", mVRotateLetters);
                style.Add("v-same-letter-heights", mVSameLetterHeights);
                style.Add("v-text-kern", mVTextKern);
                style.Add("v-text-reverse", mVTextReverse);
                style.Add("v-text-spacing", mVTextSpacing);
                style.Add("v-text-spacing-mode", mVTextSpacingMode);
                style.Add("mso-text-shadow", mMsoTextShadow);
                style.Add("text-decoration", mTextDecoration);
                mBuilder.WriteAttribute("style", style.ToCss());

                mVmlBuilder.WriteVmlAttributeIfNotDefault("fitshape", mFitShape, "t");
                mVmlBuilder.WriteVmlAttribute("trim", mTrim);
                mVmlBuilder.WriteVmlAttribute("fitpath", mFitPath);
                mVmlBuilder.WriteVmlAttribute("xscale", mXScale);
                mVmlBuilder.WriteVmlAttribute("string", mString);

                mBuilder.EndElement(); //v:textpath
            }
        }

        private readonly NrxXmlBuilder mBuilder;
        private readonly VmlBuilder mVmlBuilder;

        private int mAttrCount = 0;

        private object mOn;
        private string mFitShape;
        private string mTrim;
        private string mFitPath;
        private string mXScale;
        private string mString;

        // style attributes:
        private string mFontFamily;
        private string mFontSize;
        private string mFontWeight;
        private string mFontStyle;
        private string mFontVariant;
        private string mVTextAlign;
        private string mVTextSpacing;
        private string mVTextReverse;
        private string mVRotateLetters;
        private string mVTextKern;
        private string mVTextSpacingMode;
        private string mVSameLetterHeights;
        private string mMsoTextShadow;
        private string mTextDecoration;

        // <v:textpath
        // style="
        //   font-family:&quot Arial Black&quot
        //   font-size:20pt
        //   font-weight:bold
        //   font-style:italic
        //   font-variant:small-caps
        //   v-text-align:letter-justify
        //   v-text-spacing:1.5
        //   v-text-reverse:t
        //   v-rotate-letters:t
        //   v-text-kern:t
        //   v-text-spacing-mode:tightening
        //   v-same-letter-heights:t
        //   mso-text-shadow:auto
        //   text-decoration:line-through"
        // fitshape="f"
        // trim="t"
        // fitpath="t"
        // xscale="f"
        // string="Your Text Here" />
        
        //    on
        //    style
        //        font-family
        //        font-size
        //        font-style
        //            normal (default) 
        //            italic
        //        font-weight
        //            normal (default)
        //            bold
        //        font-variant
        //            normal (default)
        //            small-caps
        //        text-decoration
        //            none (default)
        //            underline
        //            line-through
        //        mso-text-shadow
        //            auto
        //        v-text-align
        //            left (default)
        //            right
        //            center
        //            justify
        //            letter-justify
        //            stretch-justify
        //        v-text-spacing-mode
        //            tracking (default)
        //            tightening
        //        v-text-spacing
        //        v-text-kern
        //        v-text-reverse
        //        v-same-letter-heights
        //        v-rotate-letters
        //    fitshape
        //    trim
        //    fitpath
        //    xscale
        //    string
    }
}
