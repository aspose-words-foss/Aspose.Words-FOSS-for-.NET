// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/07/2016 by Andrey Noskov

using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// List of chart axis attributes.
    /// </summary>
    internal enum DmlChartAxisAttrs
    {
        /// <summary>
        /// Id for the axis.
        /// Integer.
        /// </summary>
        AxId,

        /// <summary>
        /// Specifies that this axis is a date or text axis based on the data that is used for the axis labels, not a specific choice.
        /// Boolean.
        /// </summary>
        IsAutoCategoryType,

        /// <summary>
        /// Specifies whether an axis is a datetime axis (otherwise it is an arbitrary set of categories).
        /// Boolean.
        /// </summary>
        IsDateCategoryAxis,

        /// <summary>
        /// Specifies the position of the axis on the chart.
        /// <see cref="AxisPosition"/>.
        /// </summary>
        AxPos,

        /// <summary>
        /// Returns or sets the smallest time unit that is represented on the date axis.
        /// <see cref="AxisTimeUnit"/>.
        /// </summary>
        BaseTimeUnit,

        /// <summary>
        /// Specifies how this axis crosses the perpendicular axis.
        /// </summary>
        Crosses,

        /// <summary>
        /// Specifies where on the axis the perpendicular axis crosses. 
        /// </summary>
        CrossesAt,

        /// <summary>
        /// Specifies the ID of axis that this axis crosses.
        /// </summary>
        CrossAx,

        /// <summary>
        /// Specifies whether the value axis crosses the category axis between categories.
        /// </summary>
        CrossBetween,

        /// <summary>
        /// Specifies the scaling value of the display units for the value axis.
        /// <see cref="AxisDisplayUnit"/>.
        /// </summary>
        DisplayUnit,

        /// <summary>
        /// Specifies the text alignment for the tick labels on the axis.
        /// <see cref="LabelAlignment"/>.
        /// </summary>
        LblAlgn,

        /// <summary>
        /// Specifies the distance of labels from the axis.
        /// Integer, 0 >= value >= 1000.
        /// </summary>
        TickLabelOffset,

        /// <summary>
        /// Returns or sets the distance between major ticks.
        /// Valid range is greater then zero.
        /// </summary>
        MajorUnit,

        /// <summary>
        /// Returns or sets the time unit for major tick marks.
        /// <see cref="AxisTimeUnit"/>.
        /// </summary>
        MajorUnitScale,

        /// <summary>
        /// Returns or sets the distance between minor tick marks.
        /// Valid range is greater then zero.
        /// </summary>
        MinorUnit,

        /// <summary>
        /// Returns or sets the time unit for the minor tick marks.
        /// <see cref="AxisTimeUnit"/>.
        /// </summary>
        MinorUnitScale,

        /// <summary>
        /// Returns or sets the major tick marks for the axis.
        /// </summary>
        MajorTickMark,

        /// <summary>
        /// Returns or sets the minor tick marks for the axis.
        /// </summary>
        MinorTickMark,

        /// <summary>
        /// Specifies major gridlines.
        /// <see cref="DmlChartSpPr"/>.
        /// </summary>
        MajorGridlines,

        /// <summary>
        /// Specifies the minor gridlines.
        /// <see cref="DmlChartSpPr"/>.
        /// </summary>
        MinorGridlines,

        /// <summary>
        /// Specifies the labels shall be shown as flat text.
        /// Boolean.
        /// </summary>
        NoMultiLvlLbl,

        /// <summary>
        /// Specifies number formatting for the parent element.
        /// <see cref="DmlChartNumFormat"/>.
        /// </summary>
        NumFmt,

        /// <summary>
        /// Contains additional axis settings.
        /// <see cref="AxisScaling"/>.
        /// </summary>
        Scaling,

        /// <summary>
        /// Specifies the formatting for the parent chart element.
        /// <see cref="DmlChartSpPr"/>.
        /// </summary>
        SpPr,

        /// <summary>
        /// Specifies the position of the tick labels on the axis.
        /// <see cref="AxisTickLabelPosition"/>.
        /// </summary>
        TickLblPos,

        /// <summary>
        /// Specifies the interval at which tick labels are drawn.
        /// Integer, value must be greater than or equal to 1.
        /// </summary>
        TickLabelSpacing,

        /// <summary>
        /// Specifies a flag indicating whether automatic interval of drawing tick labels shall be used.
        /// </summary>
        TickLabelSpacingIsAuto,

        /// <summary>
        /// Specifies tick labels extensions.
        /// </summary>
        /// <dev>
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        TickLblExtensions,

        /// <summary>
        /// Specifies how many tick marks shall be skipped before the next one shall be drawn.
        /// Integer, value must be greater or equals zero.
        /// </summary>
        TickMarkSpacing,

        /// <summary>
        /// Specifies major tick mark extensions.
        /// </summary>
        /// <dev>
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        MajorTickMarkExtensions,

        /// <summary>
        /// Specifies minor tick mark extensions.
        /// </summary>
        /// <dev>
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        MinorTickMarkExtensions,

        /// <summary>
        /// Specifies text formatting.
        /// <see cref="DmlChartTxPr"/>.
        /// </summary>
        TxPr,

        /// <summary>
        /// Specifies whether this axis is hidden or not.
        /// <see cref="DmlChartTxPr"/>.
        /// </summary>
        /// <dev>
        /// Corresponds to the 21.2.2.40 delete element [ISO/IEC 29500] and to the hidden attribute of the 2.24.3.3
        /// CT_Axis complex type [MS-ODRAWXML].
        /// </dev>
        Hidden
    }
}
