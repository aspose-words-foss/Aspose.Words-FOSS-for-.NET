// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/05/2016 by Alexander Zhiltsov

using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Styles;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 2.8.3.4 CT_FontReference [MS-ODRAWXML]
    /// A reference to the document's font scheme. This element is identical to CT_FontReference ([ISO/IEC29500-1:2012]
    /// section A.4.1) but also allows for a CT_StyleColor (section 2.8.3.6) element and a modifier list.
    /// </summary>
    internal class DmlChartFontReference : DmlFontReference
    {
        /// <summary>
        /// Clones this <see cref="DmlChartFontReference"/> object.
        /// </summary>
        internal override DmlStyleReferenceBase Clone()
        {
            DmlChartFontReference lhs = (DmlChartFontReference)base.Clone();

            if (mModifiers != null)
            {
                lhs.mModifiers = new string[mModifiers.Length];
                for (int i = 0; i < mModifiers.Length; i++)
                    lhs.mModifiers[i] = mModifiers[i];
            }

            return lhs;
        }

        /// <summary>
        /// Gets <see cref="DmlSolidFill"/>
        /// </summary>
        internal DmlSolidFill GetDmlSolidFill()
        {
            DmlSolidFill fill = new DmlSolidFill();
            fill.Color = Color;
            return fill;
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
