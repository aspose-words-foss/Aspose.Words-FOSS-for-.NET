// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/08/2007 by Vladimir Averkin

using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Writes v:fill sub-element of v:shape to WordML.
    /// </summary>
    internal class VmlFillWriter
    {
        internal VmlFillWriter(ShapeBase shape, NrxXmlBuilder builder, IVmlShapeWriterContext context)
        {
            mBuilder = builder;
            mShape = shape;
            mContext = context;
        }

        /// <summary>
        /// Add v:fill related attribute.
        /// </summary>
        /// <param name="key">Attribute key.</param>
        /// <param name="value">Attribute value.</param>
        internal void AddAttribute(int key, object value)
        {
            mAttrCount++;

            switch (key)
            {
                case ShapeAttr.FillAngle:
                    {
                        mFillAngle = VmlUtil.FixedToVml(value);
                        break;
                    }
                case ShapeAttr.FillBackColor:
                    {
                        mFillBackColor = mContext.ColorToVml((DrColor)value);
                        break;
                    }
                case ShapeAttr.FillBackColorExt:
                    {
                        mBackBaseColor = VmlUtil.VmlBaseColor(value);
                        break;
                    }
                case ShapeAttr.FillBackColorExtCMY:
                    {
                        break;
                    }
                case ShapeAttr.FillBackColorExtK:
                    {
                        break;
                    }
                case ShapeAttr.FillBackColorExtMod:
                    {
                        mBackModifier = VmlUtil.VmlModifier(value);
                        break;
                    }
                case ShapeAttr.FillBackOpacity:
                    {
                        mFillBackOpacity = VmlUtil.FixedToVml(value);
                        break;
                    }
                case ShapeAttr.FillBlipName:
                    {
                        mFillBlipName = (string)value;
                        break;
                    }
                case ShapeAttr.FillBlipNameFlags:
                    {
                        break;
                    }
                case ShapeAttr.FillColor:
                    {
                        mFillColor = mContext.ColorToVml((DrColor)value);
                        mAttrCount--;
                        break;
                    }
                case ShapeAttr.FillColorExt:
                    {
                        mBaseColor = VmlUtil.VmlBaseColor(value);
                        mAttrCount--;
                        break;
                    }
                case ShapeAttr.FillColorExtCMY:
                    {
                        mAttrCount--;
                        break;
                    }
                case ShapeAttr.FillColorExtK:
                    {
                        mAttrCount--;
                        break;
                    }
                case ShapeAttr.FillColorExtMod:
                    {
                        mModifier = VmlUtil.VmlModifier(value);
                        break;
                    }
                case ShapeAttr.FillCrMod:
                    {
                        break;
                    }
                case ShapeAttr.FillDimensionType:
                    {
                        mFillDimensionType = VmlEnum.DzTypeToVml((FillDimensionType)value);
                        break;
                    }
                case ShapeAttr.Filled:
                    {
                        mFilled = VmlUtil.BoolToVml(value);
                        mAttrCount--;
                        break;
                    }
                case ShapeAttr.FillFocus:
                    {
                        mFillFocus = VmlUtil.PercentsToVml((int)value);
                        break;
                    }
                case ShapeAttr.FillHitTest:
                    {
                        // Ignored.
                        mAttrCount--;
                        break;
                    }
                case ShapeAttr.FillImageBytes:
                    {
                        mImageBytes = (byte[])value;
                        break;
                    }
                case ShapeAttr.FillNoFillHitTest:
                    {
                        mFillNoFillHitTest = VmlUtil.BoolToVml(value);
                        break;
                    }
                case ShapeAttr.FillOpacity:
                    {
                        mFillOpacity = VmlUtil.FixedToVml(value);
                        break;
                    }
                case ShapeAttr.FillOriginX:
                    {
                        mFillOriginX = VmlUtil.FixedToVml(value);
                        break;
                    }
                case ShapeAttr.FillOriginY:
                    {
                        mFillOriginY = VmlUtil.FixedToVml(value);
                        break;
                    }
                case ShapeAttr.FillPresetTexture:
                    {
                        break;
                    }
                case ShapeAttr.FillRecolorAsPicture:
                    {
                        if ((bool)value)
                            mFillRecolorAsPicture = VmlUtil.BoolToVml(value);
                        else
                            mAttrCount--;
                        break;
                    }
                case ShapeAttr.FillRectBottom:
                    {
                        // Ignored.
                        break;
                    }
                case ShapeAttr.FillRectLeft:
                    {
                        // Ignored.
                        break;
                    }
                case ShapeAttr.FillRectRight:
                    {
                        // Ignored.
                        break;
                    }
                case ShapeAttr.FillRectTop:
                    {
                        // Ignored.
                        break;
                    }
                case ShapeAttr.FillShadeColors:
                    {
                        mFillShadeColors = VmlUtil.ColorsToVml((GradientColor[])value, mContext);
                        break;
                    }
                case ShapeAttr.FillShadeType:
                    {
                        if ((int)value == (int)VmlEnum.LinearSigmaGradient)
                            mFillShadeType = "linear sigma";
                        else
                            mFillShadeType = "none";
                        break;
                    }
                case ShapeAttr.FillShape:
                    {
                        // Ignored.
                        mAttrCount--;
                        break;
                    }
                case ShapeAttr.FillShapeOriginX:
                    {
                        mFillShapeOriginX = VmlUtil.FixedToVml(value);
                        break;
                    }
                case ShapeAttr.FillShapeOriginY:
                    {
                        mFillShapeOriginY = VmlUtil.FixedToVml(value);
                        break;
                    }
                case ShapeAttr.FillToBottom:
                    {
                        // Ignored.
                        break;
                    }
                case ShapeAttr.FillToLeft:
                    {
                        mFillToLeft = VmlUtil.FixedToVml((int)value, false);
                        break;
                    }
                case ShapeAttr.FillToRight:
                    {
                        // Ignored.
                        break;
                    }
                case ShapeAttr.FillToTop:
                    {
                        mFillToTop = VmlUtil.FixedToVml((int)value, false);
                        break;
                    }
                case ShapeAttr.FillType:
                    {
                        mFillType = VmlEnum.FillTypeToVml((FillTypeCore)value);

                        // andrnosk: WORDSNET-4164 Need to check 'o:fill' Gradient type if 'v:fill' type is ShadeCenter or ShadeUnscale.
                        if ((FillTypeCore)value == FillTypeCore.ShadeCenter)
                            mIsGradientCentered = true;
                        else if ((FillTypeCore)value == FillTypeCore.ShadeUnscale)
                            mIsGradientUnscaled = true;
                        break;
                    }
                case ShapeAttr.FillUseRect:
                    {
                        // Ignored.
                        mAttrCount--;
                        break;
                    }
                case ShapeAttr.FillUseShapeAnchor:
                    {
                        if ((bool)value)
                            mFillUseShapeAnchor = VmlUtil.BoolToVml(value);
                        else
                            mAttrCount--;
                        break;
                    }
                case ShapeAttr.FillWidth:
                    {
                        break;
                    }
                case ShapeAttr.FillHeight:
                    {
                        break;
                    }
                default:
                    return;
            }
        }

        /// <summary>
        /// Write fill attributes for shape element.
        /// </summary>
        internal void WriteAttributes()
        {
            if (mShape.ShapeType != ShapeType.Line)
                mBuilder.WriteAttribute("filled", mFilled);

            WriteFillColorAttributes("fillcolor", mFillColor, mBaseColor, mModifier);

            // TODO 1 Implement fillcolor CMYK.
            //ShapeAttr.FillColorExtCMY:
            //ShapeAttr.FillColorExtK:
        }

        /// <summary>
        /// Write 'v:fill' element.
        /// </summary>
        internal void Write()
        {
            if (mAttrCount <= 0)
                return;

            mBuilder.StartElement("v:fill");
            mBuilder.WriteAttribute(mContext.ImageSrcAttributeName, mImageName);

            // If fill has image, the "o:title" attribute should be written
            if (mImageName != null)
                mBuilder.WriteAttributeString("o:title", mFillBlipName);

            mBuilder.WriteAttribute("opacity", mFillOpacity);

            WriteFillColorAttributes("color2", mFillBackColor, mBackBaseColor, mBackModifier);

            // TODO 1 Implement fill CMYK backcolor
            //ShapeAttr.FillBackColorExtCMY:
            //ShapeAttr.FillBackColorExtK:

            // Not sure where to get data for 'size' attribute.
            mBuilder.WriteAttribute("o:opacity2", mFillBackOpacity);
            mBuilder.WriteAttribute("aspect", mFillDimensionType);
            mBuilder.WriteSet("origin", mFillOriginX, mFillOriginY);
            mBuilder.WriteSet("position", mFillShapeOriginX, mFillShapeOriginY);
            mBuilder.WriteAttribute("recolor", mFillRecolorAsPicture);
            mBuilder.WriteAttribute("rotate", mFillUseShapeAnchor);
            mBuilder.WriteAttribute("angle", mFillAngle);
            mBuilder.WriteAttribute("colors", mFillShadeColors);

            if ((mFillToLeft != null) || (mFillToTop != null))
            {
                mBuilder.WriteSet("focusposition", mFillToLeft, mFillToTop);
                // Have not seen focussize taking value other than empty string.
                mBuilder.WriteAttributeString("focussize", "");
            }

            mBuilder.WriteAttribute("method", mFillShadeType);
            mBuilder.WriteAttribute("focus", mFillFocus);
            mBuilder.WriteAttribute("type", mFillType);
            mBuilder.WriteAttribute("o:detectmouseclick", mFillNoFillHitTest);

            // andrnosk: WORDSNET-4146 Write 'o:fill' attribute according to FillType.
            if (mIsGradientCentered || mIsGradientUnscaled)
                mBuilder.WriteElementWithAttributes(
                    "o:fill",
                    "v:ext", "view",
                    "type", mIsGradientCentered ? "gradientCenter" : "gradientUnscaled");

            mBuilder.EndElement(); //v:fill
        }

        /// <summary>
        /// Write fill color attributes.
        /// </summary>
        private void WriteFillColorAttributes(string attributeName, string fillColor, string baseColor, string modifier)
        {
            string fillColorStr = (StringUtil.HasChars(baseColor) && StringUtil.HasChars(modifier))
                ? string.Format("{0} [{1} {2}]", fillColor, mBaseColor, mModifier)
                : fillColor;
            mBuilder.WriteAttribute(attributeName, fillColorStr);
        }

        internal byte[] ImageBytes
        {
            get { return mImageBytes; }
        }

        internal string ImageName
        {
            get { return mImageName; }
            set { mImageName = value; }
        }

        private readonly NrxXmlBuilder mBuilder;
        private readonly ShapeBase mShape;
        private readonly IVmlShapeWriterContext mContext;

        private int mAttrCount = 0;

        private byte[] mImageBytes;
        private string mImageName = null;

        private bool mIsGradientCentered = false;
        private bool mIsGradientUnscaled = false;

        private string mFillType = null;
        private string mFillColor = null;
        private string mBaseColor = null;
        private string mModifier = null;
        private string mBackBaseColor = null;
        private string mBackModifier = null;

        private string mFillOpacity = null;
        private string mFillBackColor = null;
        private string mFillBackOpacity = null;
        private string mFillBlipName = null;
        private string mFillAngle = null;
        private string mFillFocus = null;
        private string mFillToLeft = null;
        private string mFillToTop = null;
        private string mFillDimensionType = null;
        private string mFillShadeColors = null;
        private string mFillOriginX = null;
        private string mFillOriginY = null;
        private string mFillShapeOriginX = null;
        private string mFillShapeOriginY = null;
        private string mFillShadeType = null;
        private string mFillRecolorAsPicture = null;
        private string mFillUseShapeAnchor = null;
        private string mFilled = null;
        private string mFillNoFillHitTest = null;

        //on        Yes    defined in shape[filled] attribute
        //color        Yes    defined in shape[fillcolor] attribute
        //src        Yes
        //o:title    Yes
        //opacity    Yes
        //color2    Yes
        //o:opacity2    Yes
        //aspect    Yes
        //origin    Yes
        //position    Yes
        //size        Not found    Seen only size="0,0" in TestFillStyle ms.wml(542) and that didn't correspond to any attribute in the model.
        //recolor    Yes
        //rotate    Yes
        //angle        Yes
        //colors    Yes
        //focusposition    Yes
        //focussize    Yes
        //method    Yes
        //focus        Yes
        //type        Yes
        //o:detectmouseclick    Yes
        //o:fill     Yes    sub-element specifying gradientCenter subtype of gradientRadial type
        //href    No need    Not seen in our test files.
        //althref    No need    Not seen in our test files.
    }
}
