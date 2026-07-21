// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/08/2007 by Vladimir Averkin

using System.Text;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.Nrx.Writer;


namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Writes 'v:shadow' sub-element of v:shape to WordML.
    /// </summary>
    internal class VmlShadowWriter
    {
        internal VmlShadowWriter(NrxXmlBuilder builder, IVmlShapeWriterContext context)
        {
            mBuilder = builder;
            mContext = context;
        }

        /// <summary>
        /// Add 'v:shadow' related attribute.
        /// </summary>
        /// <param name="key">Attribute key.</param>
        /// <param name="value">Attribute value.</param>
        internal void AddAttribute(int key, object value)
        {
            // Determine if the added attribute is written inside 'v:shadow' element.
            // This is done to avoid writing of the empty 'v:shadow' declaration.
            if (!(value is bool && !((bool)value)))
                mAttrCount++;

            switch (key)
            {
                case ShapeAttr.ShadowColor:
                    mShadowColor = mContext.ColorToVml((DrColor)value);
                    break;
                case ShapeAttr.ShadowColorExt:
                    break;
                case ShapeAttr.ShadowColorExtCMY:
                    break;
                case ShapeAttr.ShadowColorExtK:
                    break;
                case ShapeAttr.ShadowColorExtMod:
                    break;
                case ShapeAttr.ShadowHighlight:
                    mShadowHighlight = mContext.ColorToVml((DrColor)value);
                    break;
                case ShapeAttr.ShadowHighlightExt:
                    break;
                case ShapeAttr.ShadowHighlightExtCMY:
                    break;
                case ShapeAttr.ShadowHighlightExtK:
                    break;
                case ShapeAttr.ShadowHighlightExtMod:
                    break;
                case ShapeAttr.ShadowObscured:
                    mShadowObscured = VmlUtil.BoolToVml(value, false);
                    break;
                case ShapeAttr.ShadowOffsetX:
                    mShadowOffsetX = VmlUtil.EmuToVmlPoints((int)value);
                    break;
                case ShapeAttr.ShadowOffsetY:
                    mShadowOffsetY = VmlUtil.EmuToVmlPoints((int)value);
                    break;
                case ShapeAttr.ShadowOn:
                    mShadowOn = VmlUtil.BoolToVml(value, false);
                    break;
                case ShapeAttr.ShadowOpacity:
                    mShadowOpacity = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.ShadowOriginX:
                    mShadowOriginX = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.ShadowOriginY:
                    mShadowOriginY = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.ShadowPerspectiveX:
                    mShadowPerspectiveX = VmlUtil.PerspectiveToVml((int)value);
                    break;
                case ShapeAttr.ShadowPerspectiveY:
                    mShadowPerspectiveY = VmlUtil.PerspectiveToVml((int)value);
                    break;
                case ShapeAttr.ShadowScaleXtoX:
                    mShadowScaleXToX = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.ShadowScaleXtoY:
                    mShadowScaleXToY = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.ShadowScaleYtoX:
                    mShadowScaleYToX = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.ShadowScaleYtoY:
                    mShadowScaleYToY = VmlUtil.FixedToVml(value);
                    break;
                case ShapeAttr.ShadowSecondOffsetX:
                    mShadowSecondOffsetX = VmlUtil.EmuToVmlPoints((int)value);
                    break;
                case ShapeAttr.ShadowSecondOffsetY:
                    mShadowSecondOffsetY = VmlUtil.EmuToVmlPoints((int)value);
                    break;
                case ShapeAttr.ShadowType:
                    mShadowType = VmlEnum.ShadowTypeToVml((ShadowTypeCore)value);
                    break;
                case ShapeAttr.ShadowWeight:
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Write 'v:shadow' element.
        /// </summary>
        internal void Write()
        {
            if (mAttrCount <= 0)
                return;

            mBuilder.StartElement("v:shadow");

            mBuilder.WriteAttribute("on", mShadowOn);
            mBuilder.WriteAttribute("type", mShadowType);
            mBuilder.WriteAttribute("color", mShadowColor);
            mBuilder.WriteAttribute("color2", mShadowHighlight);
            mBuilder.WriteAttribute("opacity", mShadowOpacity);
            mBuilder.WriteSet("origin", mShadowOriginX, mShadowOriginY);
            mBuilder.WriteSet("offset", mShadowOffsetX, mShadowOffsetY);
            mBuilder.WriteSet("offset2", mShadowSecondOffsetX, mShadowSecondOffsetY);
            mBuilder.WriteAttribute("matrix", ShadowMatrixToVml());
            mBuilder.WriteAttribute("obscured", mShadowObscured);

            mBuilder.EndElement(); //v:shadow
        }

        internal string ShadowMatrixToVml()
        {
            StringBuilder sb = new StringBuilder();

            bool hasValue = false;

            hasValue |= AppendIfHasChars(sb, mShadowScaleXToX);
            sb.Append(',');
            hasValue |= AppendIfHasChars(sb, mShadowScaleYToX);
            sb.Append(',');
            hasValue |= AppendIfHasChars(sb, mShadowScaleXToY);
            sb.Append(',');
            hasValue |= AppendIfHasChars(sb, mShadowScaleYToY);
            sb.Append(',');
            hasValue |= AppendIfHasChars(sb, mShadowPerspectiveX);
            sb.Append(',');
            hasValue |= AppendIfHasChars(sb, mShadowPerspectiveY);

            // There are 6 elements in the matrix, but yes, MS Word seems to trim commas at the end so there could be only 
            // 4 elements left for example. It works both with trimming and without, but we trim to match MS Word more.
            return (hasValue) ? sb.ToString().TrimEnd(',') : null;
        }

        private static bool AppendIfHasChars(StringBuilder sb, string value)
        {
            if (StringUtil.HasChars(value))
            {
                sb.Append(value);
                return true;
            }
            else
            {
                return false;
            }
        }

        private readonly NrxXmlBuilder mBuilder;
        private readonly IVmlShapeWriterContext mContext;

        private int mAttrCount = 0;

        private string mShadowType = null;
        private string mShadowColor = null;
        private string mShadowHighlight = null;
        private string mShadowOpacity = null;
        private string mShadowOffsetX = null;
        private string mShadowOffsetY = null;
        private string mShadowSecondOffsetX = null;
        private string mShadowSecondOffsetY = null;
        private string mShadowScaleXToX = null;
        private string mShadowScaleYToX = null;
        private string mShadowScaleXToY = null;
        private string mShadowScaleYToY = null;
        private string mShadowPerspectiveX = null;
        private string mShadowPerspectiveY = null;
        private string mShadowOriginX = null;
        private string mShadowOriginY = null;
        private string mShadowOn = null;
        private string mShadowObscured = null;

        //on        Yes    
        //type         Yes    
        //color        Yes    
        //color2    Yes    
        //opacity    Yes    
        //origin    Yes    
        //offset    Yes    
        //offset2    Yes    
        //matrix    Yes    
        //obscured    Yes    
        //id        No need    Not seen in our test files.
    }
}
