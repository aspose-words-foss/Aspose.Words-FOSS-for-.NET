// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2006 by Vladimir Averkin

using Aspose.Common;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;


namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Writes diagram related elements and attributes to WordML.
    /// </summary>
    internal class VmlDiagramWriter
    {
        internal VmlDiagramWriter(IVmlShapeWriterContext context, NrxXmlBuilder builder)
        {
            mBuilder = builder;
            mContext = context;
        }

        /// <summary>
        /// Add diagram related attribute.
        /// </summary>
        /// <param name="key">Attribute key.</param>
        /// <param name="value">Attribute value.</param>
        internal void AddAttribute(int key, object value)
        {
            mAttrCount++;

            switch (key)
            {
                case ShapeAttr.PseudoInline:
                    // PseudoInline property is supported for DOC/RTF only.
                    mAttrCount--;
                    break;
                case ShapeAttr.EditAs:
                    mEditAs = VmlEnum.EditAsToVml((EditAs)value);
                    mAttrCount--;
                    break;
                case ShapeAttr.DiagramStyle:
                    mStyle = FormatterPal.IntToXml((int)value);
                    break;
                case ShapeAttr.DiagramScaleX:
                    mScaleX = FormatterPal.IntToXml((int)value);
                    break;
                case ShapeAttr.DiagramScaleY:
                    mScaleY = FormatterPal.IntToXml((int)value);
                    break;
                case ShapeAttr.DiagramFontSize:
                    mFontSize = FormatterPal.IntToXml((int)value);
                    break;
                case ShapeAttr.DiagramConstrainBounds:
                    mConstrainBounds = NrxXmlUtil.IntArrayToXml((int[])value);
                    break;
                case ShapeAttr.DiagramAutoFormat:
                    mAutoformat = VmlUtil.BoolToVml(value);
                    break;
                case ShapeAttr.DiagramReverse:
                    mReverse = VmlUtil.BoolToVml(value);
                    break;
                case ShapeAttr.DiagramRelationsTable:
                    mRelations = (DiagramNodeRelation[])value;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Write 'o:diagram' element.
        /// </summary>
        internal void WriteDiagramElement()
        {
            // Example:
            // <o:diagram v:ext="edit" dgmstyle="12" dgmscalex="31402" dgmscaley="25941" dgmfontsize="4" constrainbounds="2374,2279,9718,9623" autoformat="t" reverse="t">
            // <o:relationtable v:ext="edit">
            //        <o:rel v:ext="edit" idsrc="#_s1036" iddest="#_s1036" />
            // </o:relationtable>

            if (mAttrCount > 0)
            {
                mBuilder.StartElement("o:diagram");

                mBuilder.WriteAttribute("v:ext", "edit");
                mBuilder.WriteAttribute("dgmstyle", mStyle);
                mBuilder.WriteAttribute("dgmscalex", mScaleX);
                mBuilder.WriteAttribute("dgmscaley", mScaleY);
                mBuilder.WriteAttribute("dgmfontsize", mFontSize);
                mBuilder.WriteAttribute("constrainbounds", mConstrainBounds);
                mBuilder.WriteAttribute("autoformat", mAutoformat);
                mBuilder.WriteAttribute("reverse", mReverse);

                if (mRelations != null && mRelations.Length > 0)
                {
                    mBuilder.StartElement("o:relationtable");
                    mBuilder.WriteAttribute("v:ext", "edit");

                    foreach (DiagramNodeRelation relation in mRelations)
                    {
                        mBuilder.StartElement("o:rel");
                        mBuilder.WriteAttribute("v:ext", "edit");
                        mBuilder.WriteAttribute("idsrc", mContext.GetDiagramMemberName(relation.A));
                        mBuilder.WriteAttribute("iddest", mContext.GetDiagramMemberName(relation.B));
                        mBuilder.WriteAttribute("idcntr", mContext.GetDiagramMemberName(relation.C));
                        mBuilder.EndElement(); //o:rel
                    }

                    mBuilder.EndElement(); //o:relationtable
                }

                mBuilder.EndElement(); //o:diagram
            }
        }

        internal string EditAs
        {
            get { return mEditAs; }
        }

        private readonly NrxXmlBuilder mBuilder;
        private readonly IVmlShapeWriterContext mContext;

        private string mEditAs = null;

        private int mAttrCount = 0;

        private string mStyle;
        private string mScaleX;
        private string mScaleY;
        private string mFontSize;
        private string mConstrainBounds;
        private string mAutoformat;
        private string mReverse;

        private DiagramNodeRelation[] mRelations;
    }
}
