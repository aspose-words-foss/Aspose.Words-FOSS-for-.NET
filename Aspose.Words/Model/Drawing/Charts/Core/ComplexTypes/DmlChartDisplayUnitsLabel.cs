// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 5.7.2.46 dispUnitsLbl (Display Units Label) element.
    /// This element specifies the display unit label for the value axis in the specified chart.
    /// </summary>
    internal class DmlChartDisplayUnitsLabel : DmlChartTitle
    {
        internal DmlChartDisplayUnitsLabel(IDmlChartTitleHolder chartTitleHolder): base(chartTitleHolder)
        {
        }

        /// <summary>
        /// Clones this instance of display units label.
        /// </summary>
        internal override DmlChartTitle Clone()
        {
            DmlChartDisplayUnitsLabel lhs = (DmlChartDisplayUnitsLabel)MemberwiseClone();

            CopyComplexPropertiesTo(lhs);

            return lhs;
        }
    }
}
