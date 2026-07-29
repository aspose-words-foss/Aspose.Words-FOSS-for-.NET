// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/11/2024 by Alexander Zhiltsov

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Specifies how the values ​​that specify the location of a data label - the <see cref="ChartDataLabel.Left"/> and
    /// <see cref="ChartDataLabel.Top"/> properties - are interpreted.
    /// </summary>
    public enum ChartDataLabelLocationMode
    {
        /// <summary>
        /// The location of a data label is specified by an offset from the position defined by its
        /// <see cref="ChartDataLabel.Position"/> property.
        /// </summary>
        Offset,

        /// <summary>
        /// The location of a data label is specified using absolute coordinates, staring from the upper left corner
        /// of a chart.
        /// </summary>
        Absolute
    }
}
