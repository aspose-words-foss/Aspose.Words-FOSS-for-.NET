// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/05/2016 by Alexander Zhiltsov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 2.8.3.3 CT_ColorStyleVariation [MS-ODRAWXML]
    /// The complex type specifies a list of transforms that are appended to all colors in a CT_ColorStyle to produce
    /// a variation of the color style.
    /// </summary>
    internal class DmlChartColorStyleVariation
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        internal DmlChartColorStyleVariation()
        {
        }

        /// <summary>
        /// Ctor with specifying color modifiers.
        /// </summary>
        internal DmlChartColorStyleVariation(params IDmlColorModifier[] modifiers)
        {
            foreach (IDmlColorModifier modifier in modifiers)
                ColorModifiers.Add(modifier);
        }

        /// <summary>
        /// Clones this <see cref="DmlChartColorStyleVariation"/> object.
        /// </summary>
        internal DmlChartColorStyleVariation Clone()
        {
            DmlChartColorStyleVariation lhs = (DmlChartColorStyleVariation)MemberwiseClone();

            if (mColorModifiers != null)
            {
                lhs.mColorModifiers = new List<IDmlColorModifier>();
                for (int i = 0; i < mColorModifiers.Count; i++)
                    lhs.mColorModifiers.Add(mColorModifiers[i].Clone());
            }

            return lhs;
        }

        /// <summary>
        /// Gets or sets a list of transforms that are appended to colors.
        /// </summary>
        public IList<IDmlColorModifier> ColorModifiers
        {
            get 
            {
                if (mColorModifiers == null)
                    mColorModifiers = new List<IDmlColorModifier>();
                return mColorModifiers;
            }
            set { mColorModifiers = value; }
        }

        private IList<IDmlColorModifier> mColorModifiers;
    }
}
