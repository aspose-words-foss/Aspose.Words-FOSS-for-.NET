// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    internal interface IDmlChartTitleHolder
    {
        /// <summary>
        /// If font size is not specified for a chart title, MS Word uses 1.2 * [font size of chart space]. This method
        /// is used to get such calculated font size.
        /// </summary>
        int GetRelativeFontSize(int chartFontSize);

        /// <summary>
        /// Gets or sets title.
        /// </summary>
        DmlChartTitle DCTitle { get; set; }

        /// <summary>
        /// Returns the position of the title.
        /// </summary>
        TitlePosition TitlePosition { get; }

        /// <summary>
        /// Indicates whether the title is visible.
        /// </summary>
        bool IsVisible { get;}

        /// <summary>
        /// Returns true if title should not be displayed even if it exists.
        /// </summary>
        bool TitleDeleted { get; set; }

        /// <summary>
        /// Gets the default title text.
        /// </summary>
        string DefaultTitleText { get; }

        /// <summary>
        /// Gets the title font size in points that MS Word sets for created charts.
        /// </summary>
        double DefaultDisplayedFontSize { get; }

        /// <summary>
        /// Gets the title default font size in points.
        /// </summary>
        double DefaultFontSize { get; }

        /// <summary>
        /// Gets a style item that is applied to the title.
        /// </summary>
        DmlChartStyleItem StyleItem { get; }

        /// <summary>
        /// Gets the parent chart space.
        /// </summary>
        DmlChartSpace ChartSpace { get; }
    }
}
