// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/08/2007 by Vladimir Averkin

using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.Nrx.Writer;


namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Writes 'v:stroke' sub-element of v:shape to WordML.
    /// </summary>
    internal class VmlStrokeWriter
    {
        internal VmlStrokeWriter(NrxXmlBuilder builder, IVmlShapeWriterContext context)
        {
            mBuilder = builder;
            mContext = context;
        }

        /// <summary>
        /// Add 'v:stroke' related attribute.
        /// </summary>
        /// <param name="key">Attribute key.</param>
        /// <param name="value">Attribute value.</param>
        internal void AddAttribute(int key, object value)
        {
            mAttrCount++;

            switch (key)
            {
                case ShapeAttr.LineArrowHeadsOK:
                {
                    // This attribute is written in VmlGeoWriter
                    mAttrCount--;
                    break;
                }
                case ShapeAttr.LineBackColor:
                {
                    mLineBackColor = mContext.ColorToVml((DrColor)value);
                    break;
                }
                case ShapeAttr.LineBackColorExt:
                {
                    mLineBackBaseColor = VmlUtil.VmlBaseColor(value);
                    break;
                }
                case ShapeAttr.LineBackColorExtMod:
                {
                    mLineBackModifier = VmlUtil.VmlModifier(value);
                    break;
                }
                case ShapeAttr.LineBackColorExtK:
                case ShapeAttr.LineBackColorExtCMY:
                {
                    // RK Until we support writing an attribute, we should not increment count,
                    // otherwise an empty v:stroke element will be written.
                    mAttrCount--;
                    break;
                }
                case ShapeAttr.LineColor:
                {
                    mLineColor = mContext.ColorToVml((DrColor)value);
                    mAttrCount--;
                    break;
                }
                case ShapeAttr.LineColorExt:
                {
                    mLineBaseColor = VmlUtil.VmlBaseColor(value);
                    mAttrCount--;
                    break;
                }
                case ShapeAttr.LineColorExtMod:
                {
                    mLineModifier = VmlUtil.VmlModifier(value);
                    break;
                }
                case ShapeAttr.LineColorExtCMY:
                case ShapeAttr.LineColorExtK:
                case ShapeAttr.LineCrMod:
                {
                    // Not yet supported.
                    mAttrCount--;
                    break;
                }
                case ShapeAttr.LineDashStyle:
                {
                    mLineDashing = VmlEnum.DashStyleToVml((DashStyle)value);
                    break;
                }
                case ShapeAttr.LineDashData:
                {
                    // Not yet supported.
                    mAttrCount--;
                    break;
                }
                case ShapeAttr.LineEndArrow:
                {
                    mLineEndArrow = VmlEnum.ArrowTypeToVml((ArrowType)value);
                    break;
                }
                case ShapeAttr.LineEndArrowLength:
                {
                    mLineEndArrowLength = VmlEnum.ArrowLengthToVml((ArrowLength)value);
                    break;
                }
                case ShapeAttr.LineEndArrowWidth:
                {
                    mLineEndArrowWidth = VmlEnum.ArrowWidthToVml((ArrowWidth)value);
                    break;
                }
                case ShapeAttr.LineEndCapStyle:
                {
                    mLineEndCapStyle = VmlEnum.EndCapToVml((EndCap)value);
                    break;
                }
                case ShapeAttr.LineFillBlipName:
                {
                    mLineFillBlipName = (string)value;
                    break;
                }
                case ShapeAttr.LineFillBlipNameFlags:
                case ShapeAttr.LineFillDimensionType:
                case ShapeAttr.LineFillHeight:
                case ShapeAttr.LineFillPresetTexture:
                case ShapeAttr.LineFillShape:
                {
                    // Not written.
                    mAttrCount--;
                    break;
                }
                case ShapeAttr.LineFillType:
                {
                    mLineFillType = VmlEnum.LineFillTypeToVml((LineFillType)value);
                    break;
                }
                case ShapeAttr.LineFillWidth:
                case ShapeAttr.LineHitTest:
                {
                    // Not written.
                    mAttrCount--;
                    break;
                }
                case ShapeAttr.LineImageBytes:
                {
                    mImageBytes = (byte[])value;
                    break;
                }
                case ShapeAttr.LineInsetPen:
                {
                    mLineInsetPen = VmlUtil.BoolToVml(value);
                    mAttrCount--;
                    break;
                }
                case ShapeAttr.LineJoinStyle:
                {
                    mLineJoinStyle = VmlEnum.JoinStyleToVml((JoinStyle)value);
                    break;
                }
                case ShapeAttr.LineMiterLimit:
                case ShapeAttr.LineNoLineDrawDash:
                {
                    // Not written.
                    mAttrCount--;
                    break;
                }
                case ShapeAttr.LineOn:
                {
                    mLineOn = VmlUtil.BoolToVml(value);
                    mAttrCount--;
                    break;
                }
                case ShapeAttr.LineOpacity:
                {
                    mLineOpacity = VmlUtil.FixedToVml(value);
                    break;
                }
                case ShapeAttr.LineRecolorFillAsPicture:
                {
                    // Not written.
                    mAttrCount--;
                    break;
                }
                case ShapeAttr.LineStartArrow:
                {
                    mLineStartArrow = VmlEnum.ArrowTypeToVml((ArrowType)value);
                    break;
                }
                case ShapeAttr.LineStartArrowLength:
                {
                    mLineStartArrowLength = VmlEnum.ArrowLengthToVml((ArrowLength)value);
                    break;
                }
                case ShapeAttr.LineStartArrowWidth:
                {
                    mLineStartArrowWidth = VmlEnum.ArrowWidthToVml((ArrowWidth)value);
                    break;
                }
                case ShapeAttr.LineStyle:
                {
                    mLineStyle = VmlEnum.ShapeLineStyleToVml((ShapeLineStyle)value);
                    break;
                }
                case ShapeAttr.LineInsetPenOk:
                case ShapeAttr.LineUseShapeAnchor:
                {
                    // Not written.
                    mAttrCount--;
                    break;
                }
                case ShapeAttr.LineWidth:
                {
                    if ((int)value > 0)
                        mLineWidth = VmlUtil.EmuToVmlPoints((int)value);

                    mAttrCount--;
                    break;
                }
                default:
                    // Do nothing.
                    break;
            }
        }

        /// <summary>
        /// Write stroke attributes for shape element.
        /// </summary>
        internal void WriteAttributes()
        {
            mBuilder.WriteAttribute("stroked", mLineOn);

            string lineColorStr = mLineColor;
            if (StringUtil.HasChars(mLineBaseColor) && StringUtil.HasChars(mLineModifier))
                lineColorStr += string.Format(" [{0} {1}]", mLineBaseColor, mLineModifier);
            mBuilder.WriteAttribute("strokecolor", lineColorStr);

            // TODO 1 Implement strokecolor CMYK.
            //ShapeAttr.LineColorExtCMY:
            //ShapeAttr.LineColorExtK:

            mBuilder.WriteAttribute("strokeweight", mLineWidth);
            mBuilder.WriteAttribute("insetpen", mLineInsetPen);
        }

        /// <summary>
        /// Write 'v:stroke' element.
        /// </summary>
        internal void Write()
        {
            if (mAttrCount <= 0)
                return;

            mBuilder.StartElement("v:stroke");

            mBuilder.WriteAttribute(mContext.ImageSrcAttributeName, mImageName);

            if ((mLineFillBlipName != null) || StringUtil.HasChars(mImageName))
                mBuilder.WriteAttributeString("o:title", mLineFillBlipName);

            mBuilder.WriteAttribute("joinstyle", mLineJoinStyle);
            mBuilder.WriteAttribute("dashstyle", mLineDashing);
            mBuilder.WriteAttribute("linestyle", mLineStyle);
            mBuilder.WriteAttribute("endcap", mLineEndCapStyle);
            mBuilder.WriteAttribute("startarrow", mLineStartArrow);
            mBuilder.WriteAttribute("startarrowwidth", mLineStartArrowWidth);
            mBuilder.WriteAttribute("startarrowlength", mLineStartArrowLength);
            mBuilder.WriteAttribute("endarrow", mLineEndArrow);
            mBuilder.WriteAttribute("endarrowwidth", mLineEndArrowWidth);
            mBuilder.WriteAttribute("endarrowlength", mLineEndArrowLength);
            mBuilder.WriteAttribute("opacity", mLineOpacity);

            string lineBackColorStr = mLineBackColor;
            if (StringUtil.HasChars(mLineBackBaseColor) && StringUtil.HasChars(mLineBackModifier))
                lineBackColorStr += string.Format(" [{0} {1}]", mLineBackBaseColor, mLineBackModifier);
            mBuilder.WriteAttribute("color2", lineBackColorStr);

            // TODO 1 Implement stroke color2 CMYK.
            //ShapeAttr.LineBackColorExtCMY:
            //ShapeAttr.LineBackColorExtK:

            mBuilder.WriteAttribute("filltype", mLineFillType);

            mBuilder.EndElement(); //v:stroke
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

        internal string LineJoinStyle
        {
            get { return mLineJoinStyle; }
            set
            {
                if (!StringUtil.HasChars(mLineJoinStyle) && StringUtil.HasChars(value))
                    mAttrCount++;

                mLineJoinStyle = value;
            }
        }

        private readonly NrxXmlBuilder mBuilder;
        private readonly IVmlShapeWriterContext mContext;

        private int mAttrCount = 0;

        private byte[] mImageBytes;
        private string mImageName = null;

        private string mLineColor = null;
        private string mLineOpacity = null;
        private string mLineBackColor = null;
        private string mLineFillType = null;
        private string mLineFillBlipName = null;
        private string mLineWidth = null;
        private string mLineStyle = null;
        private string mLineDashing = null;
        private string mLineStartArrow = null;
        private string mLineEndArrow = null;
        private string mLineStartArrowWidth = null;
        private string mLineStartArrowLength = null;
        private string mLineEndArrowWidth = null;
        private string mLineEndArrowLength = null;
        private string mLineJoinStyle = null;
        private string mLineEndCapStyle = null;
        private string mLineInsetPen = null;
        private string mLineOn = null;
        private string mLineBaseColor = null;
        private string mLineModifier = null;
        private string mLineBackBaseColor = null;
        private string mLineBackModifier = null;


        //on            Yes    defined in shape[stroked] attribute
        //color            Yes    defined in shape[strokecolor] attribute
        //weight        Yes    defined in shape[strokeweight] attribute
        //insetpen        Yes    defined in shape[insetpen] attribute
        //src            Yes    
        //o:title        Yes    
        //joinstyle        Yes    
        //dashstyle        Yes    
        //linestyle        Yes    
        //endcap        Yes    
        //startarrow    Yes    
        //startarrowwidth    Yes    
        //startarrowlength    Yes    
        //endarrow        Yes    
        //endarrowwidth    Yes    
        //endarrowlength    Yes    
        //opacity        Yes    
        //color2        Yes    
        //filltype        Yes    
        //id            No need    Not seen in our test files.
        //miterlimit    No need    Not seen in our test files.
        //imageaspect    No need    Not seen in our test files.
        //imagesize        No need    Not seen in our test files.
        //imagealignshape    No need    Not seen in our test files.
        //o:href        No need    Not seen in our test files.
        //o:althref        No need    Not seen in our test files.
        //o:forcedash    No need    Not seen in our test files.
    }
}
