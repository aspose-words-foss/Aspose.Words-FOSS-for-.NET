// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/04/2018 by Ilya Egorov

namespace Aspose.Words.Drawing.Charts.Core
{
    internal class ChartDataLabelFieldType
    {
        /// <summary>
        /// Indicates that the cell range of the data point must be specified in the text field.
        /// </summary>
        internal const string CellRange = "CELLRANGE";

        /// <summary>
        /// Indicates that the cell range of the data point must be specified in the text field.
        /// </summary>
        internal const string CellRef = "CELLREF";

        /// <summary>
        /// Indicates that the value of the data point must be specified in the text field. 
        /// </summary>
        internal const string Value = "VALUE";

        /// <summary>
        /// Indicates that the value of the data point must be specified in the text field. 
        /// </summary>
        /// <remarks>
        /// WORDSNET-23267 This type is used for "Scatter" charts.
        /// </remarks>
        internal const string YValue = "YVALUE";

        /// <summary>
        /// Indicates that the x-value of the data point must be specified in the text field. 
        /// </summary>
        /// <remarks>
        /// WORDSNET-25342 This type is used for "Scatter" charts.
        /// </remarks>
        internal const string XValue = "XVALUE";

        /// <summary>
        /// Indicates that the category name must be specified in the text field. 
        /// </summary>
        internal const string CategoryName = "CATEGORYNAME";

        /// <summary>
        /// Indicates that the series name must be specified in the text field.
        /// </summary>
        internal const string SeriesName = "SERIESNAME";

        /// <summary>
        /// Indicates that the percent value of the data point must be specified in the text field.
        /// </summary>
        internal const string Percentage = "PERCENTAGE";

        /// <summary>
        /// Indicates that the size of bubble of the data point must be specified in the text field.
        /// </summary>
        internal const string BubbleSize = "BUBBLESIZE";
    }
}
