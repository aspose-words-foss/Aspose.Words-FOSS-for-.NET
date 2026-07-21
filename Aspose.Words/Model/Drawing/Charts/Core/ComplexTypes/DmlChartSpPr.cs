// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents SpPr element in charts. It is similar to SpPr in regular DML, but some elements
    /// are not supported.
    /// From the list of supported properties we need only Fill and Outline.
    /// </summary>
    internal class DmlChartSpPr : DmlExtensionListSource
    {
        internal DmlChartSpPr Clone()
        {
            DmlChartSpPr lhs = (DmlChartSpPr)MemberwiseClone();

            if (mFill != null)
                lhs.mFill = mFill.Clone();

            if (mOutline != null)
                lhs.mOutline = mOutline.Clone();

            if (mEffects != null)
                lhs.mEffects = mEffects.Clone();

            if (mScene3DProp != null)
                lhs.Scene3DProp = mScene3DProp.Clone();

            if (mShape3DProp != null)
                lhs.Shape3DProp = mShape3DProp.Clone();

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlChartSpPr value = (DmlChartSpPr)obj;

            return
                Fill.Equals(value.Fill) &&
                Outline.Equals(value.Outline) &&
                (ArgumentUtil.BothAreNull(Scene3DProp, value.Scene3DProp) ||
                    ((Scene3DProp != null) && Scene3DProp.Equals(value.Scene3DProp))) &&
                (ArgumentUtil.BothAreNull(Shape3DProp, value.Shape3DProp) ||
                    ((Shape3DProp != null) && Shape3DProp.Equals(value.Shape3DProp))) &&
                (ArgumentUtil.BothAreNull(Effects, value.Effects) ||
                    ((Effects != null) && Effects.Equals(value.Effects)));
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash ^= Fill.GetHashCode();
            hash ^= Outline.GetHashCode();
            if (Scene3DProp != null)
                hash ^= Scene3DProp.GetHashCode();
            if (Shape3DProp != null)
                hash ^= Shape3DProp.GetHashCode();
            if (Effects != null)
                hash ^= Effects.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Clears the properties so that default formatting is used.
        /// </summary>
        internal void Clear()
        {
            mFill = null;
            mOutline = null;
            mEffects = null;
        }

        internal DmlFill Fill
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get
            {
                if (mFill != null)
                    return mFill;

                return mFill = new DmlStyleFill();
            }
            set { mFill = value; }
        }

        internal DmlOutline Outline
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get
            {
                if (mOutline != null)
                    return mOutline;

                return mOutline = new DmlOutline();
            }
            set { mOutline = value; }
        }

        internal DmlFill DirectFill
        {
            get { return mFill; }
        }

        internal DmlOutline DirectOutline
        {
            get { return mOutline; }
        }

        internal bool IsEmpty
        {
            // WORDSNET-12517 FillType.StyleFill means direct fill is not specified, so SpPr is empty.
            get { return (mFill == null || mFill.DmlFillType == DmlFillType.StyleFill) &&
                    (mOutline == null || mOutline.DirectPropertiesCount == 0) && ((Effects == null) || (Effects.Count == 0));
            }
        }

        internal DmlShapeEffectsCollection Effects
        {
            get { return mEffects; }
            set { mEffects = value; }
        }

        /// <summary>
        /// Optional scene-level 3D properties.
        /// </summary>
        internal DmlScene3DProperties Scene3DProp
        {
            get { return mScene3DProp; }
            set { mScene3DProp = value; }
        }

        /// <summary>
        /// 3D properties associated with a shape.
        /// </summary>
        internal DmlShape3DProperties Shape3DProp
        {
            get { return mShape3DProp; }
            set { mShape3DProp = value; }
        }

        /// <summary>
        /// Indicates whether the outline is not specified.
        /// </summary>
        internal bool IsOutlineEmpty
        {
            get { return DmlChartRenderingUtil.IsOutlineEmpty(mOutline); }
        }

        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private DmlFill mFill;
        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private DmlOutline mOutline;
        private DmlScene3DProperties mScene3DProp;
        private DmlShape3DProperties mShape3DProp;
        private DmlShapeEffectsCollection mEffects;
    }
}
