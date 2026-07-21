// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/05/2016 by Alexander Zhiltsov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 2.8.1.2 colorStyle, 2.8.3.2 CT_ColorStyle [MS-ODRAWXML]
    /// The complex type specifies colors used to resolve CT_StyleColor (section 2.8.3.6) colors in a CT_ChartStyle
    /// (section 2.8.3.1). The color style consists of a list of colors, a list of variations and a method for
    /// iterating the total set of colors.
    /// </summary>
    internal class DmlChartColorStyle : DmlExtensionListSource
    {
        /// <summary>
        /// Clones this <see cref="DmlChartColorStyle"/> object.
        /// </summary>
        internal DmlChartColorStyle Clone()
        {
            DmlChartColorStyle lhs = (DmlChartColorStyle)MemberwiseClone();
            if (mColors != null)
            {
                lhs.mColors = new DmlColor[mColors.Length];
                for (int i = 0; i < mColors.Length; i++)
                    lhs.mColors[i] = mColors[i].Clone();
            }

            if (mVariations != null)
            {
                lhs.mVariations = new DmlChartColorStyleVariation[mVariations.Length];
                for (int i = 0; i < mVariations.Length; i++)
                    lhs.mVariations[i] = mVariations[i].Clone();
            }

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// Gets the DML color at the specified index.
        /// </summary>
        internal DmlColor GetColor(int index)
        {
            Debug.Assert(mColors.Length > 0);

            int colorIndex = index % mColors.Length;
            DmlColor color = mColors[colorIndex].Clone();

            int variationIndex = index / mColors.Length;
            if (mVariations.Length > 0)
            {
                variationIndex %= mVariations.Length;

                IList<IDmlColorModifier> colorModifiers = mVariations[variationIndex].ColorModifiers;
                if (color.ColorModifiers == null)
                    color.ColorModifiers = new List<IDmlColorModifier>(colorModifiers.Count);

                foreach (IDmlColorModifier colorModifier in colorModifiers)
                    color.ColorModifiers.Add(colorModifier.Clone());
            }

            return color;
        }

        /// <summary>
        /// Gets or sets style colors. A color style can contain 6 colors.
        /// </summary>
        internal DmlColor[] Colors
        {
            get { return mColors; }
            set { mColors = value; }
        }

        /// <summary>
        /// Gets or sets color variations.
        /// Represents variation: a CT_ColorStyleVariation element that specifies a variation applied to all colors to
        /// create a longer set of colors without having to explicitly list them all.
        /// </summary>
        internal DmlChartColorStyleVariation[] Variations
        {
            get { return mVariations; }
            set { mVariations = value; }
        }

        /// <summary>
        /// Represents meth: an ST_ColorStyleMethod attribute that specifies the method for mapping an index for
        /// an element in a chart to the total set of colors contained in this CT_ColorStyle.
        /// </summary>
        internal string Method
        {
            get { return mMethod; }
            set { mMethod = value; }
        }

        /// <summary>
        /// Represents id: an unsignedInt ([XMLSCHEMA2] section 3.3.22) attribute that specifies the identifier for
        /// this CT_ColorStyle.
        /// </summary>
        internal string Id
        {
            get { return mId; }
            set { mId = value; }
        }

        private DmlColor[] mColors;
        private DmlChartColorStyleVariation[] mVariations;
        private string mMethod;
        private string mId;
    }
}
