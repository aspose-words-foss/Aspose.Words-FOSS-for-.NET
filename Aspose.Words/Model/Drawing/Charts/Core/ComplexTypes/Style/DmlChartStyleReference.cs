// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/05/2016 by Alexander Zhiltsov

using Aspose.Words.Drawing.Core.Dml.Styles;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 2.8.3.8 CT_StyleReference [MS-ODRAWXML]
    /// A reference to the document's theme style matrix. This element is identical to CT_StyleMatrixReference 
    /// ([ISO/IEC29500-1:2012] section A.4.1) but also allows for a CT_StyleColor (section 2.8.3.6) element and 
    /// a modifier list.
    /// </summary>
    internal class DmlChartStyleReference : DmlEffectReference
    {
        /// <summary>
        /// Clones this <see cref="DmlChartStyleReference"/> object.
        /// </summary>
        internal override DmlStyleReferenceBase Clone()
        {
            DmlChartStyleReference lhs = (DmlChartStyleReference)base.Clone();

            if (mModifiers != null)
            {
                lhs.mModifiers = new string[mModifiers.Length];
                for (int i = 0; i < mModifiers.Length; i++)
                    lhs.mModifiers[i] = mModifiers[i];
            }

            return lhs;
        }

        /// <summary>
        /// Represents mods: an ST_StyleReferenceModifierList attribute that specifies a list of modifiers for 
        /// this reference.
        /// </summary>
        internal string[] Modifiers
        {
            get { return mModifiers; }
            set { mModifiers = value; }
        }

        private string[] mModifiers;
    }
}
