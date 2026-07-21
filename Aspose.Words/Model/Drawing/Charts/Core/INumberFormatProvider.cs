// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/07/2017 by Andrey Noskov

using Aspose.Words.Drawing.Charts.Core.ComplexTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// When implemented, provides necessary internal DmlChartNumFormat object to work 
    /// with public ChartDataLabel/ChartAxis NumberFormat.
    /// </summary>
    internal interface INumberFormatProvider
    {
        /// <summary>
        /// Specifies number formatting for the parent element.
        /// <see cref="DmlChartNumFormat"/>.
        /// </summary>
        DmlChartNumFormat NumFmt_INumberFormatProvider { get; set; }

        /// <summary>
        /// Returns a flag indicating whether the provider uses number format that is inherited from the parent number
        /// format provider.
        /// </summary>
        bool IsInherited { get; }
    }
}
