// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2017 by Alexander Zhiltsov

using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// This class represents the 2.24.3.26 CT_FormatOverride complex type [MS-ODRAWXML].
    /// It specifies an override of a chart’s data point color style format.
    /// </summary>
    internal class DmlChartFormatOverride : DmlExtensionListSource
    {
        /// <summary>
        /// Returns a copy of this instance.
        /// </summary>
        internal DmlChartFormatOverride Clone()
        {
            DmlChartFormatOverride lhs = (DmlChartFormatOverride)MemberwiseClone();

            if (mSpPr != null)
                lhs.mSpPr = mSpPr.Clone();

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// Represents spPr: CT_ShapeProperties ([ISO/IEC29500-4:2012] section A.4.1) element that specifies the shape
        /// properties to override the chart’s color style format. A chart’s color style assigns a unique color format
        /// per data point according to an index.
        /// </summary>
        internal DmlChartSpPr SpPr
        {
            get { return mSpPr; }
            set { mSpPr = value; }
        }

        /// <summary>
        /// Gets or sets the index of the color format being overridden.
        /// </summary>
        internal int Index
        {
            get { return mIndex; }
            set { mIndex = value; }
        }

        private DmlChartSpPr mSpPr;
        private int mIndex;
    }
}
