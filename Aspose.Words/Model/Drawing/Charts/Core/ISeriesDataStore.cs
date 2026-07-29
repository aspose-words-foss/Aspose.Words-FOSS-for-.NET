// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/08/2022 by Alexander Zhiltsov

using Aspose.Words.Drawing.Charts.Core.ComplexTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents an interface for use by the chart data facade classes <see cref="ChartXValueCollection"/>,
    /// <see cref="ChartYValueCollection"/> and <see cref="BubbleSizeCollection"/> to retrieve and modify
    /// chart data values from/in the series's data store based on <see cref="DmlChartValueCollection"/>
    /// instances.
    /// </summary>
    internal interface ISeriesDataStore
    {
        /// <summary>
        /// Gets the X value as <see cref="ChartXValue"/> instance from the series's internal data store at the
        /// specified index. The required data type is specified by the <paramref name="valueType"/> parameter.
        /// </summary>
        ChartXValue GetXValue(int index, ChartXValueType valueType);

        /// <summary>
        /// Gets the Y value as <see cref="ChartYValue"/> instance from the series's internal data store at the
        /// specified index. The required data type is specified by the <paramref name="valueType"/> parameter.
        /// </summary>
        ChartYValue GetYValue(int index, ChartYValueType valueType);

        /// <summary>
        /// Gets the bubble size from the series's internal data store at the specified index.
        /// </summary>
        double GetBubbleSize(int index);

        /// <summary>
        /// Changes the X value in the series's internal data store at the specified index, according to the specified
        /// X value.
        /// </summary>
        void ChangeXValue(ChartXValue xValue, int index);

        /// <summary>
        /// Changes the Y value in the series's internal data store at the specified index, according to the specified
        /// Y value.
        /// </summary>
        void ChangeYValue(ChartYValue yValue, int index);

        /// <summary>
        /// Changes the bubble size in the series's internal data store at the specified index.
        /// </summary>
        void ChangeBubbleSize(double bubbleSize, int index);

        /// <summary>
        /// Gets or sets the X value type of the series.
        /// </summary>
        ChartXValueType XValueType { get; set; }

        /// <summary>
        /// Gets or sets the Y value type of the series.
        /// </summary>
        ChartYValueType YValueType { get; set; }

        /// <summary>
        /// Gets a flag indicating whether the series supports X values.
        /// </summary>
        bool IsXValueSupported { get; }

        /// <summary>
        /// Gets a flag indicating whether the series supports Y values.
        /// </summary>
        bool IsYValueSupported { get; }

        /// <summary>
        /// Gets a flag indicating whether the series supports bubble sizes.
        /// </summary>
        bool IsBubbleSizeSupported { get; }

        /// <summary>
        /// Gets or sets a number of values in the series.
        /// </summary>
        int ValueCount { get; set; }

        /// <summary>
        /// Gets or sets the format code applied to the X values.
        /// </summary>
        string XFormatCode { get; set; }

        /// <summary>
        /// Gets or sets the format code applied to the Y values.
        /// </summary>
        string YFormatCode { get; set; }

        /// <summary>
        /// Gets or sets the format code applied to the bubble sizes.
        /// </summary>
        string BubbleSizeFormatCode { get; set; }
    }
}
