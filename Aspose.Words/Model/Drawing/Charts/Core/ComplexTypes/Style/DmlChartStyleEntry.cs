// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/05/2016 by Alexander Zhiltsov

using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 2.8.3.7 CT_StyleEntry [MS-ODRAWXML]
    /// This complex type specifies the default formatting for a single type of element on a chart. This element allows
    /// for properties to be explicitly specified or hold references to the document's theme.
    /// </summary>
    internal class DmlChartStyleEntry : DmlExtensionListSource
    {
        /// <summary>
        /// Clones this <see cref="DmlChartStyleEntry"/> object.
        /// </summary>
        internal DmlChartStyleEntry Clone()
        {
            DmlChartStyleEntry lhs = (DmlChartStyleEntry)MemberwiseClone();

            lhs.mLineReference = CloneReference(mLineReference);
            lhs.mFillReference = CloneReference(mFillReference);
            lhs.mEffectReference = CloneReference(mEffectReference);

            if (mFontReference != null)
                lhs.mFontReference = (DmlChartFontReference)mFontReference.Clone();
            if (mShapePr != null)
                lhs.mShapePr = mShapePr.Clone();
            if (mDefaultRunPr != null)
                lhs.mDefaultRunPr = mDefaultRunPr.Clone();
            if (mTextBodyPr != null)
                lhs.mTextBodyPr = mTextBodyPr.Clone();

            if (mModifiers != null)
            {
                lhs.mModifiers = new string[mModifiers.Length];
                for (int i = 0; i < mModifiers.Length; i++)
                    lhs.mModifiers[i] = mModifiers[i];
            }

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// Clones the specified style reference object if it is not null.
        /// </summary>
        private static DmlChartStyleReference CloneReference(DmlChartStyleReference reference)
        {
            return (reference != null) ? (DmlChartStyleReference)reference.Clone() : null;
        }

        /// <summary>
        /// Represents lnRef: a CT_StyleReference element that specifies a reference to a line style within the style 
        /// matrix.
        /// </summary>
        internal DmlChartStyleReference LineReference
        {
            get
            {
                if (mLineReference == null)
                    mLineReference = new DmlChartStyleReference();
                return mLineReference;
            }
            set { mLineReference = value; }
        }

        /// <summary>
        /// Represents lineWidthScale: a double element that specifies a multiplier to apply to the line width.
        /// </summary>
        internal double LineWidthScale
        {
            get { return mLineWidthScale; }
            set { mLineWidthScale = value; }
        }

        /// <summary>
        /// Represents fillRef: a CT_StyleReference element that specifies a reference to a fill style within 
        /// the style matrix.
        /// </summary>
        internal DmlChartStyleReference FillReference
        {
            get
            {
                if (mFillReference == null)
                    mFillReference = new DmlChartStyleReference();
                return mFillReference;
            }
            set { mFillReference = value; }
        }

        /// <summary>
        /// Represents effectRef: a CT_StyleReference element that specifies a reference to an effect style within 
        /// the style matrix.
        /// </summary>
        internal DmlChartStyleReference EffectReference
        {
            get
            {
                if (mEffectReference == null)
                    mEffectReference = new DmlChartStyleReference();
                return mEffectReference;
            }
            set { mEffectReference = value; }
        }

        /// <summary>
        /// Represents fontRef: a CT_FontReference element that specifies a reference to a themed font.
        /// </summary>
        internal DmlChartFontReference FontReference
        {
            get
            {
                if (mFontReference == null)
                    mFontReference = new DmlChartFontReference();
                return mFontReference;
            }
            set { mFontReference = value; }
        }

        /// <summary>
        /// Represents spPr: a CT_ShapeProperties ([ISO/IEC29500-1:2012] section A.4.1) element that specifies visual
        /// shape properties of the part of the chart associated with this CT_StyleEntry. These properties override
        /// properties that are specified by fillRef, lnRef and effectRef.
        /// </summary>
        internal DefaultShapeProperties ShapePr
        {
            get { return mShapePr; }
            set { mShapePr = value; }
        }

        /// <summary>
        /// Represents defRPr: a CT_TextCharacterProperties ([ISO/IEC29500-1:2012] section A.4.1) element that 
        /// specifies the default text character properties for a text body on a chart which is associated with this 
        /// CT_StyleEntry. If a CT_SchemeColor ([ISO/IEC29500-1:2012] section A.4.1) element within this element has 
        /// a value of phClr, then the color is resolved by replacing it with the color specified by fontRef.
        /// </summary>
        internal DmlRunProperties DefaultRunPr
        {
            get { return mDefaultRunPr; }
            set { mDefaultRunPr = value; }
        }

        /// <summary>
        /// Represents bodyPr: a CT_TextBodyProperties ([ISO/IEC29500-1:2012] section A.4.1) element that specifies 
        /// the body properties for a text body on a chart that is associated with this CT_StyleEntry.
        /// </summary>
        internal DmlTextBodyProperties TextBodyPr
        {
            get { return mTextBodyPr; }
            set { mTextBodyPr = value; }
        }

        /// <summary>
        /// Represents mods: an ST_StyleEntryModifierList attribute that specifies modifiers for this style entry.
        /// </summary>
        internal string[] Modifiers
        {
            get { return mModifiers; }
            set { mModifiers = value; }
        }

        private DmlChartStyleReference mLineReference;
        private double mLineWidthScale = LineWidthScaleDefault;
        private DmlChartStyleReference mFillReference;
        private DmlChartStyleReference mEffectReference;
        private DmlChartFontReference mFontReference;
        private DefaultShapeProperties mShapePr;
        private DmlRunProperties mDefaultRunPr;
        private DmlTextBodyProperties mTextBodyPr;
        private string[] mModifiers;

        internal const double LineWidthScaleDefault = 1d;
    }
}
