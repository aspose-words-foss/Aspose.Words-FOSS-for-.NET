// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2017 by Alexander Zhiltsov

using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// This class represents the CT_ChartLines complex type [ISO/IEC 29500] and the 2.24.3.52 CT_Gridlines
    /// complex type [MS-ODRAWXML].
    /// </summary>
    internal class DmlChartGridlines : DmlExtensionListSource
    {
        /// <summary>
        /// Returns a copy of this instance.
        /// </summary>
        internal DmlChartGridlines Clone()
        {
            DmlChartGridlines lhs = (DmlChartGridlines)MemberwiseClone();

            if (mSpPr != null)
                lhs.mSpPr = mSpPr.Clone();

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// Gets the OfficeArt shape properties for the gridlines.
        /// </summary>
        internal DmlChartSpPr SpPr
        {
            get { return mSpPr; }
        }

        private DmlChartSpPr mSpPr = new DmlChartSpPr();
    }
}
