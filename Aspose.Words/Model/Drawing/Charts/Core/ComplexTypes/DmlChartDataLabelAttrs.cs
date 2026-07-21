// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Contains list of attributes of 5.7.2.47 dLbl (Data Label).
    /// </summary>
    internal enum DmlChartDataLabelAttrs
    {
        /// <summary>
        /// This is zero value, according to "CA1008 Enums should have zero value" guideline.
        /// </summary>
        None,

        /// <summary>
        /// Specifies that the chart element specified by its containing element shall be deleted from the chart.
        /// Boolean.
        /// </summary>
        Delete,

        /// <summary>
        /// Specifies the position of the data label.
        /// <see cref="ChartDataLabelPosition"/>.
        /// </summary>
        DLblPos,

        /// <summary>
        /// Specifies the index of the containing element. 
        /// This index shall determine which of the parent's 14 children collection this element applies to.
        /// Integer.
        /// </summary>
        Idx,

        /// <summary>
        /// Specifies how the chart element is placed on the chart.
        /// <see cref="DmlChartManualLayout"/>.
        /// </summary>
        Layout,

        /// <summary>
        /// Specifies number formatting for the parent element.
        /// <see cref="DmlChartNumFormat"/>.
        /// </summary>
        NumFmt,

        /// <summary>
        /// specifies text that shall be used to separate the parts of a data label. 
        /// The default is a comma, except for pie charts showing only category name and percentage, when a 
        /// line break shall be used instead.
        /// String.
        /// </summary>
        Separator,

        /// <summary>
        /// Specifies the bubble size shall be shown in a data label.
        /// Boolean.
        /// </summary>
        ShowBubbleSize,

        /// <summary>
        /// Specifies that the category name shall be shown in the data label.
        /// Boolean.
        /// </summary>
        ShowCatName,

        /// <summary>
        /// Specifies legend keys shall be shown in data labels.
        /// Boolean.
        /// </summary>
        ShowLegendKey,

        /// <summary>
        /// Specifies that the percentage shall be shown in a data label.
        /// Boolean.
        /// </summary>
        ShowPercent,

        /// <summary>
        /// Specifies that the series name shall be shown in a data label.
        /// Boolean.
        /// </summary>
        ShowSerName,

        /// <summary>
        /// Specifies that the value shall be shown in a data label.
        /// Boolean.
        /// </summary>
        ShowVal,

        /// <summary>
        /// Specifies that leader line should be drawn to the data label. Applies only to Pie Chart.
        /// Boolean.
        /// </summary>
        ShowLeaderLines,

        /// <summary>
        /// Specifies that value from data labels range must be displayed.
        /// Boolean.
        /// </summary>
        ShowDataLabelsRange,

        /// <summary>
        /// Specifies the formatting for the parent chart element.
        /// <see cref="DmlChartSpPr"/>.
        /// </summary>
        SpPr,

        /// <summary>
        /// Specifies text to use on a chart, including rich text formatting.
        /// <see cref="DmlChartTx"/>.
        /// </summary>
        Tx,

        /// <summary>
        /// Specifies text formatting.
        /// <see cref="DmlChartTxPr"/>.
        /// </summary>
        TxPr,

        /// <summary>
        /// Specifies line formatting for leader lines.
        /// <see cref="DmlChartSpPr"/>.
        /// </summary>
        LeaderLines,

        /// <summary>
        /// extLst (Chart Extensibility) This element contains tags used for future extensibility of the file format
        /// </summary>
        Extensions,

        /// <summary>
        /// Specifies whether a Datalabel was created as an exception entry only for saving, but is treated the same as
        /// the prototype Datalabel in the collection.
        /// Boolean.
        /// </summary>
        XForSave,

        /// <summary>
        /// Represents spPr extension of a data label that is used to specify a shape of a label.
        /// </summary>
        ShapePr,

        /// <summary>
        /// Represents a data label field table, each entry of which corresponds to a text field in a data label whose
        /// value is obtained from a formula reference. Currently, entries are represented as plain XML.
        /// </summary>
        FieldTable
    }
}
