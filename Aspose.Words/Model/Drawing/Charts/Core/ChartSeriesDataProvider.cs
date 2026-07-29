// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/11/2022 by Alexander Zhiltsov

using System;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Contains methods used to provide a public API for getting/modifying chart series data.
    /// </summary>
    /// <remarks>
    /// Also the class implements the <see cref="ISeriesDataStore"/> interface that is used by API classes
    /// <see cref="ChartXValueCollection"/>, <see cref="ChartYValueCollection"/> and <see cref="BubbleSizeCollection"/>
    /// to get chart data and perform changes in it.
    /// </remarks>
    internal class ChartSeriesDataProvider : ISeriesDataStore
    {
        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        internal ChartSeriesDataProvider(ChartSeries series)
        {
            mSeries = series;
        }

        /// <summary>
        /// Adds the X value, Y value and bubble size to the end of the series.
        /// </summary>
        internal void AddValue(ChartXValue xValue, ChartYValue yValue, double bubbleSize)
        {
            int index = ValueCount;

            // Set the Count properties of the all internal DmlChartValue collections to ensure that the number of
            // values is the same.
            ValueCount = index + 1;

            if (IsXValueSupported)
            {
                mSeries.XValues.SetValue(index, xValue);
                DmlChartValue dmlValue = ChartXValueToDmlChartValue(xValue, index);
                XDmlChartValues.AddNullAware(dmlValue, index);
            }

            if (IsYValueSupported)
            {
                mSeries.YValues.SetValue(index, yValue);
                DmlChartValue dmlValue = ChartYValueToDmlChartValue(yValue, index);
                YDmlChartValues.AddNullAware(dmlValue, index);
            }

            if (IsBubbleSizeSupported)
            {
                mSeries.BubbleSizes[index] = bubbleSize;
                DmlChartValue dmlValue = BubbleSizeToDmlChartValue(bubbleSize, index);
                BubbleSizeDmlChartValues.AddNullAware(dmlValue, index);
            }
        }

        /// <summary>
        /// Inserts the X value, Y value and bubble size to the series at the specified index.
        /// </summary>
        internal void InsertValue(int index, ChartXValue xValue, ChartYValue yValue, double bubbleSize)
        {
            Debug.Assert((index >= 0) && (index <= ValueCount));

            DataPointInfo[] oldPointInfos = null;
            int oldPointCount = 0;
            if (HasDataPointsOrLabels && IsComplexCorrespondenceBetweenDataPointsAndXValues)
            {
                oldPointInfos = IsXValueAffectsNextValueDataPoints
                    ? DataPointInfoRetriever.GetDataPointInfos(index, false)
                    : new DataPointInfo[0];
                oldPointCount = DataPoints.Count;
            }

            if (IsXValueSupported)
            {
                mSeries.XValues.Insert(index, xValue);
                DmlChartValue dmlValue = ChartXValueToDmlChartValue(xValue, index);
                XDmlChartValues.Insert(index, dmlValue);
            }

            if (IsYValueSupported)
            {
                mSeries.YValues.Insert(index, yValue);
                DmlChartValue dmlValue = ChartYValueToDmlChartValue(yValue, index);
                YDmlChartValues.Insert(index, dmlValue);
            }

            if (IsBubbleSizeSupported)
            {
                mSeries.BubbleSizes.Insert(index, bubbleSize);
                DmlChartValue dmlValue = BubbleSizeToDmlChartValue(bubbleSize, index);
                BubbleSizeDmlChartValues.Insert(index, dmlValue);
            }

            if (HasDataPointsOrLabels && !IsNoCorrespondenceBetweenDataPointsAndValues)
            {
                if (IsComplexCorrespondenceBetweenDataPointsAndXValues)
                {
                    DataPointInfo[] newPointInfos = DataPointInfoRetriever.GetDataPointInfos(index, true);
                    int newPointCount = DataPoints.Count;

                    // Make ValueIndex in oldPointInfos compatible with newPointInfos.
                    // oldPointInfos contains point infos generated before the insertion for the current 'index + 1' value.
                    // 'i' is an index from the end of oldPointInfos and newPointInfos.
                    for (int i = 0; i < System.Math.Min(oldPointInfos.Length, newPointInfos.Length); i++)
                    {
                        int newInfoIndex = newPointInfos.Length - i - 1;

                        // Process only point infos related to the value after the inserted one. If there are still
                        // non-processed point infos left in oldPointInfos, they now refers to the inserted value.
                        if (newPointInfos[newInfoIndex].ValueIndex != index + 1)
                            break;

                        int oldInfoIndex = oldPointInfos.Length - i - 1;
                        oldPointInfos[oldInfoIndex].ValueIndex += 1;
                    }

                    UpdatePoints(oldPointInfos, oldPointCount, newPointInfos, newPointCount);
                }
                else
                {
                    DataPoints.Insert(index);
                    DataLabels.Insert(index);
                }
            }

            DataChanged();
        }

        /// <summary>
        /// Removes the X value, Y value and bubble size from the series at the specified index.
        /// </summary>
        internal void RemoveValue(int index)
        {
            DataPointInfo[] oldPointInfos = null;
            int oldPointCount = 0;
            if (HasDataPointsOrLabels && !IsNoCorrespondenceBetweenDataPointsAndValues)
            {
                if (IsComplexCorrespondenceBetweenDataPointsAndXValues)
                {
                    oldPointInfos = DataPointInfoRetriever.GetDataPointInfos(index, true);
                    oldPointCount = DataPoints.Count;
                }
                else
                {
                    DataPoints.Remove(index);
                    DataLabels.Remove(index);
                }
            }

            if (IsXValueSupported)
            {
                mSeries.XValues.Remove(index);
                XDmlChartValues.Remove(index);
            }

            if (IsYValueSupported)
            {
                mSeries.YValues.Remove(index);
                YDmlChartValues.Remove(index);
            }

            if (IsBubbleSizeSupported)
            {
                mSeries.BubbleSizes.Remove(index);
                BubbleSizeDmlChartValues.Remove(index);
            }

            if (HasDataPointsOrLabels && IsComplexCorrespondenceBetweenDataPointsAndXValues)
            {
                DataPointInfo[] newPointInfos = IsXValueAffectsNextValueDataPoints
                    ? DataPointInfoRetriever.GetDataPointInfos(index, false)
                    : new DataPointInfo[0];
                int newPointCount = DataPoints.Count;

                // Make ValueIndex in newPointInfos compatible with oldPointInfos.
                // oldPointInfos contains point infos generated before the insertion for the removed and next values.
                // 'i' is an index from the end of oldPointInfos and newPointInfos.
                for (int i = 0; i < System.Math.Min(oldPointInfos.Length, newPointInfos.Length); i++)
                {
                    int oldInfoIndex = oldPointInfos.Length - i - 1;

                    // Process only point infos related to the value after the removed one. If there are still
                    // non-processed point infos left in newInfosIndex, they have become irrelevant after the value
                    // removal, and the corresponding data points/labels should be removed.
                    if (oldPointInfos[oldInfoIndex].ValueIndex != index + 1)
                        break;

                    int newInfoIndex = newPointInfos.Length - i - 1;
                    newPointInfos[newInfoIndex].ValueIndex += 1;
                }

                UpdatePoints(oldPointInfos, oldPointCount, newPointInfos, newPointCount);
            }

            DataChanged();
        }

        /// <summary>
        /// Removes all data values from the series.
        /// </summary>
        internal void RemoveAllValues()
        {
            if (mSeries.ChartExChartData != null)
            {
                foreach (DmlChartDataSource dataSource in mSeries.ChartExChartData.DataSources)
                    dataSource.Values.Clear();
            }

            if (IsXValueSupported)
                mSeries.XValues.Clear();
            if (XDmlChartValues != null)
                XDmlChartValues.Clear();

            if (IsYValueSupported)
                mSeries.YValues.Clear();
            if (YDmlChartValues != null)
                YDmlChartValues.Clear();

            if (IsBubbleSizeSupported)
                mSeries.BubbleSizes.Clear();
            if (BubbleSizeDmlChartValues != null)
                BubbleSizeDmlChartValues.Clear();

            DataChanged();
        }

        /// <summary>
        /// Converts the specified X value to an instance of a <see cref="DmlChartValue"/> descendant.
        /// </summary>
        private DmlChartValue ChartXValueToDmlChartValue(ChartXValue value, int index)
        {
            if (value == null)
                return null;

            switch (value.ValueType)
            {
                case ChartXValueType.String:
                {
                    switch (XDmlChartValues.ValueType)
                    {
                        case DmlChartValueType.String:
                            return new DmlChartStrValue(index, value.StringValue);
                        case DmlChartValueType.MultiLvlString:
                        {
                            DmlChartMultiLvlStrValue multiLevelValue = new DmlChartMultiLvlStrValue(index);
                            multiLevelValue.AddLevelValue(value.StringValue, 0);
                            return multiLevelValue;
                        }
                        default:
                            Debug.Assert(false);
                            return null;
                    }
                }
                case ChartXValueType.Multilevel:
                {
                    Debug.Assert(XDmlChartValues.ValueType == DmlChartValueType.MultiLvlString);
                    DmlChartMultiLvlStrValue multiLevelValue = new DmlChartMultiLvlStrValue(index);
                    ChartMultilevelValue multilevel = value.MultilevelValue;

                    // Levels in DmlChartMultiLvlStrValue are in the reverse order.
                    // If the destination DmlChartValueCollection and the creating value have different level numbers,
                    // this is resolved in DmlChartValueCollection.AddPointNullAware.

                    int level = 0;
                    if (multilevel.Level3 != null)
                    {
                        multiLevelValue.AddLevelValue(multilevel.Level3, level);
                        level++;
                    }

                    if ((multilevel.Level2 != null) || (level > 0))
                    {
                        multiLevelValue.AddLevelValue(multilevel.Level2, level);
                        level++;
                    }

                    multiLevelValue.AddLevelValue(multilevel.Level1, level);

                    return multiLevelValue;
                }
                case ChartXValueType.Double:
                    return CreateNumberBasedChartValue(index, value.DoubleValue, XDmlChartValues);
                case ChartXValueType.DateTime:
                    double wordDateTime = DmlChartUtil.GetDoubleFromDate(value.DateTimeValue);
                    return CreateNumberBasedChartValue(index, wordDateTime, XDmlChartValues);
                case ChartXValueType.Time:
                    double numericTime = value.TimeValue.TotalDays;
                    return CreateNumberBasedChartValue(index, numericTime, XDmlChartValues);
                default:
                    Debug.Assert(false);
                    return null;
            }
        }

        /// <summary>
        /// Creates an instance of a <see cref="DmlChartValue"/> descendant for the specified numeric value.
        /// </summary>
        private static DmlChartValue CreateNumberBasedChartValue(int index, double value,
            DmlChartValueCollection collection)
        {
            switch (collection.ValueType)
            {
                case DmlChartValueType.Numeric:
                    return new DmlChartNumValue(index, value);
                case DmlChartValueType.MultiLvlNumeric:
                {
                    DmlChartMultiLvlNumValue multiLevelValue =
                        new DmlChartMultiLvlNumValue(index, collection.LevelProperties);
                    multiLevelValue.AddLevelValue(value, 0);
                    return multiLevelValue;
                }
                default:
                    Debug.Assert(false);
                    return null;
            }
        }

        /// <summary>
        /// Converts the specified Y value to an instance of a <see cref="DmlChartValue"/> descendant.
        /// </summary>
        private DmlChartValue ChartYValueToDmlChartValue(ChartYValue value, int index)
        {
            if (value == null)
                return null;

            double numericValue;
            switch (value.ValueType)
            {
                case ChartYValueType.Double:
                    numericValue = value.DoubleValue;
                    break;
                case ChartYValueType.DateTime:
                    numericValue = DmlChartUtil.GetDoubleFromDate(value.DateTimeValue);
                    break;
                case ChartYValueType.Time:
                    numericValue = value.TimeValue.TotalDays;
                    break;
                default:
                    Debug.Assert(false);
                    return null;
            }

            return CreateNumberBasedChartValue(index, numericValue, YDmlChartValues);
        }

        /// <summary>
        /// Converts the specified bubble size value to an instance of a <see cref="DmlChartValue"/> descendant.
        /// </summary>
        private DmlChartValue BubbleSizeToDmlChartValue(double value, int index)
        {
            return !IsNullBubbleSize(value)
                ? (DmlChartValue)new DmlChartNumValue(index, value, BubbleSizeDmlChartValues.FormatCode)
                : null;
        }

        /// <summary>
        /// Returns a flag indicating whether the specified bubble size value represents an empty value.
        /// </summary>
        private static bool IsNullBubbleSize(double value)
        {
            return double.IsNaN(value);
        }

        /// <summary>
        /// Updates data points and labels based on data point infos before and after the data change.
        /// </summary>
        private void UpdatePoints(DataPointInfo[] oldPointInfos, int oldPointCount,
            DataPointInfo[] newPointInfos, int newPointCount)
        {
            // For any of the change of the X value collection of a Treemap or Sunburst charts, i.e. removing/inserting/
            // updating a value, except the data points/labels of the affected X value itself, it is needed to check the
            // data points/labels of the next X value, because there may be added/removed data points/labels related to
            // it. For example, when an X value is being removed, its data points/labels for branch/stem/leaf are needed
            // to be removed too. And if branch and stem of the next X value equal to branch and stem of the previous X
            // value, but are different than branch and stem of the removing X value, data points/labels related to branch
            // and stem of the next X value must be removed too.

            Debug.Assert((oldPointInfos != null) && (newPointInfos != null));

            // It is expected that infos are ordered by DataPointIndex. The indexes of the contained data points are
            // continuous except for a Box&Whisker chart.
            for (int i = 1; i < oldPointInfos.Length; i++)
                Debug.Assert(oldPointInfos[i - 1].Index < oldPointInfos[i].Index);
            for (int i = 1; i < newPointInfos.Length; i++)
                Debug.Assert(newPointInfos[i - 1].Index < newPointInfos[i].Index);

            // Preserve data points/labels if possible.

            // Check from the end at first because the oldPointInfos/newPointInfos arrays may contain data points for
            // the non-affected next data value.
            for (int i = 0; i < System.Math.Min(oldPointInfos.Length, newPointInfos.Length); i++)
            {
                int oldPointInfoIndex = oldPointInfos.Length - i - 1;
                DataPointInfo oldPointInfo = oldPointInfos[oldPointInfoIndex];
                int newPointInfoIndex = newPointInfos.Length - i - 1;
                DataPointInfo newPointInfo = newPointInfos[newPointInfoIndex];

                if ((oldPointInfo.ValueIndex != newPointInfo.ValueIndex) ||
                    (oldPointInfo.ValueLevel != newPointInfo.ValueLevel))
                {
                    break;
                }

                int oldPointIndexFromEnd = oldPointCount - oldPointInfo.Index - 1;
                int newPointIndexFromEnd = newPointCount - newPointInfo.Index - 1;
                if (oldPointIndexFromEnd != newPointIndexFromEnd)
                    break;

                // Preserve this data point and label.
                oldPointInfos[oldPointInfoIndex] = null;
                newPointInfos[newPointInfoIndex] = null;
            }

            // Check from the beginning.
            for (int i = 0; i < System.Math.Min(oldPointInfos.Length, newPointInfos.Length); i++)
            {
                DataPointInfo oldPointInfo = oldPointInfos[i];
                DataPointInfo newPointInfo = newPointInfos[i];
                if ((oldPointInfo == null) || (newPointInfo == null))
                    break;

                if ((oldPointInfo.Index != newPointInfo.Index) ||
                    (oldPointInfo.ValueIndex != newPointInfo.ValueIndex) ||
                    (oldPointInfo.ValueLevel != newPointInfo.ValueLevel))
                {
                    break;
                }

                // Preserve this data point and label.
                oldPointInfos[i] = null;
                newPointInfos[i] = null;
            }

            // Remove data points/labels.
            for (int i = oldPointInfos.Length - 1; i >= 0; i--)
            {
                DataPointInfo info = oldPointInfos[i];
                if ((info == null) || info.IsPointReferencedByOtherValues)
                    continue; // The point/label should be preserved.

                DataPoints.Remove(info.Index);
                DataLabels.Remove(info.Index);
            }

            // Insert data points/labels.
            foreach (DataPointInfo info in newPointInfos)
            {
                // IsPointReferencedByOtherValues means that the data point existed before the change.
                if ((info == null) || info.IsPointReferencedByOtherValues)
                    continue; // Data point/label already exists: no need to insert.

                DataPoints.Insert(info.Index);
                DataLabels.Insert(info.Index);
            }
        }

        /// <summary>
        /// Updates other chart structures to be actual after changing the chart data. Removes the embedded XLSX file
        /// that stores the chart data, and marks the data as literal. Removes the fallback image shape.
        /// </summary>
        private void DataChanged()
        {
            // Box&Whisker chart may have unknown 'size' dimension. There was an issues, that after changing data in
            // AW, and then opening the document in MS Word, if this dimension is preserved, after selecting Edit Data,
            // MS Word copies some data range from it to the Y values dimension. Remove it.
            if (IsChartExChart && (LayoutType == SeriesLayout.BoxWhisker))
                mSeries.ChartExChartData.RemoveDataSource(DimensionType.Size);

            // Remove a fallback image shape as it has become non-actual after data was changed. The fallback will be
            // regenerated in DmlShapeValidator.ProcessFallback.
            RunPr shapeRunPr = Chart.ChartSpace.Dml.RunPr;
            if (shapeRunPr.AlternateContent != null)
            {
                // For Word 2016 charts, let's remove the alternate content at all because the renderer throws exceptions
                // when draws such charts. TODO: When the renderer will support drawing these chart types, just remove
                // the fallback like for the older chart types so that it will be regenerated in the shape validator.
                if (IsChartExChart)
                    shapeRunPr.Remove(FontAttr.AlternateContent);
                else
                    shapeRunPr.AlternateContent.FallBack = null;
            }

            // Need to mark the collections as literal data to remove XLSX formulas, otherwise MS Word may generate
            // wrong data when building the XLSX document.

            if (IsXValueSupported && (mSeries.X.DataSource.ValueRef != null))
                mSeries.X.DataSource.ValueRef.MarkAsLiteralData();

            if (IsYValueSupported && (mSeries.Y.DataSource.ValueRef != null))
                mSeries.Y.DataSource.ValueRef.MarkAsLiteralData();

            if (IsBubbleSizeSupported && (mSeries.Size.DataSource.ValueRef != null))
                mSeries.Size.DataSource.ValueRef.MarkAsLiteralData();

            if (mSeries.Tx != null && mSeries.Tx.StrRef != null)
                mSeries.Tx.StrRef.MarkAsLiteralData();

            // After changing series data, an existing embedded/linked XLSX document does not contain the changes, we
            // need to remove it, otherwise when clicking Edit Data in MS Word, it restores the old data from XLSX.
            if (Chart.ChartSpace.HasExternalData)
            {
                Chart.ChartSpace.RemoveExternalDataLinkage();

                // Removed XLSX document might contain some formatting or additional data: generate a warning.
                Chart.Document.Warn(WarningType.DataLoss, WarningSource.DrawingML, WarningStrings.ChartDataRegeneration);
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the specified format code is a datetime format. Returns <b>false</b> for a
        /// time-only format.
        /// </summary>
        private static bool IsDateTimeFormat(string formatCode)
        {
            return
                DmlChartFormatCodeValidator.IsDateFormat(formatCode) &&
                !DmlChartFormatCodeValidator.IsTimeFormat(formatCode);
        }

        /// <summary>
        /// Gets a flag indicating whether the specified format code is a numeric format.
        /// </summary>
        private static bool IsNumericFormat(string formatCode)
        {
            // Let's just check that the format is not a date or time.
            return
                StringUtil.HasChars(formatCode) &&
                !DmlChartFormatCodeValidator.IsDateFormat(formatCode);
        }

        /// <summary>
        /// Gets a flag indicating whether the specified X value type is supported by the series.
        /// </summary>
        private bool IsXValueTypeSupported(ChartXValueType value)
        {
            if (!IsChartExChart)
            {
                // Although string and multilevel values are not displayed and replaced with numbers in Bubble and
                // Scatter charts, but the SeriesCollection.Add method can be used to create series with string values
                // in these chart types: so let's allow the all value types in non-Word 2016 charts.
                return true;
            }

            switch (LayoutType)
            {
                // Looks like MS Word creates only string dimensions for X values of these chart types and numeric
                // values may be not displayed.
                case SeriesLayout.BoxWhisker:
                case SeriesLayout.Funnel:
                case SeriesLayout.Waterfall:
                case SeriesLayout.RegionMap:
                    return (value == ChartXValueType.String);
                case SeriesLayout.Treemap:
                case SeriesLayout.Sunburst:
                    return (value == ChartXValueType.Multilevel);
                case SeriesLayout.ClusteredColumn:
                {
                    if (mSeries.LayoutPr.IsAggregation)
                    {
                        // Pareto chart
                        return (value == ChartXValueType.String);
                    }
                    else
                    {
                        // Histogram chart
                        return (value != ChartXValueType.String) && (value != ChartXValueType.Multilevel);
                    }
                }
                case SeriesLayout.ParetoLine:
                    return false; // Pareto line cannot has data.
                default:
                    Debug.Assert(false);
                    return false;
            }
        }

        /// <summary>
        /// Sets the category type of the specified axis to <see cref="AxisCategoryType.Automatic"/> and resets
        /// the date category flag.
        /// </summary>
        private static void SetAutomaticAxisCategoryType(ChartAxis axis)
        {
            // Set using attributes because ChartAxis.CategoryType.set does not change IsDateCategoryAxis when
            // specifying the AxisCategoryType.Automatic value.
            axis.ChartAxisPr.SetProperty(DmlChartAxisAttrs.IsAutoCategoryType, true);
            axis.ChartAxisPr.SetProperty(DmlChartAxisAttrs.IsDateCategoryAxis, false);
        }

        /// <summary>
        /// Adjusts the format codes according to the specified X value type.
        /// </summary>
        private void AdjustXFormatCodes(ChartXValueType xValueType)
        {
            IDmlChart2D chart2d = (Chart as IDmlChart2D);
            ChartAxis axis = (chart2d != null) ? chart2d.AxX : null;

            switch (xValueType)
            {
                case ChartXValueType.String:
                case ChartXValueType.Multilevel:
                {
                    XDmlChartValues.FormatCode = null;

                    if (axis != null)
                    {
                        SetAutomaticAxisCategoryType(axis);
                        axis.NumberFormat.FormatCode = ChartNumberFormat.GeneralFormatCode;
                        axis.NumberFormat.IsLinkedToSource = true;
                    }

                    break;
                }
                case ChartXValueType.Double:
                {
                    // Do not change current format if it is already numeric.
                    if (!IsNumericFormat(XDmlChartValues.FormatCode))
                        XDmlChartValues.FormatCode = ChartNumberFormat.GeneralFormatCode;

                    if (axis != null)
                    {
                        SetAutomaticAxisCategoryType(axis);

                        // Do not change current format if it is already numeric.
                        if (!IsNumericFormat(axis.NumberFormat.FormatCode))
                            axis.NumberFormat.FormatCode = ChartNumberFormat.GeneralFormatCode;
                    }

                    break;
                }
                case ChartXValueType.DateTime:
                {
                    // Do not change current format if it is already DateTime.
                    if (!IsDateTimeFormat(XDmlChartValues.FormatCode))
                        XDmlChartValues.FormatCode = ChartNumberFormat.DefaultDateFormatCode;

                    if (axis != null)
                    {
                        axis.CategoryType = AxisCategoryType.Time;

                        // Do not change current format if it is already DateTime.
                        if (!IsDateTimeFormat(axis.NumberFormat.FormatCode))
                            axis.NumberFormat.FormatCode = ChartNumberFormat.DefaultDateFormatCode;
                    }

                    break;
                }
                case ChartXValueType.Time:
                {
                    // Do not change current format if it is already Time.
                    if (!DmlChartFormatCodeValidator.IsTimeFormat(XDmlChartValues.FormatCode))
                        XDmlChartValues.FormatCode = ChartNumberFormat.DefaultTimeFormatCode;

                    if (axis != null)
                    {
                        // Need to set the Automatic category type, not Time, otherwise MS Word does not display data.
                        axis.CategoryType = AxisCategoryType.Automatic;

                        // Do not change current format if it is already DateTime.
                        if (!DmlChartFormatCodeValidator.IsTimeFormat(axis.NumberFormat.FormatCode))
                            axis.NumberFormat.FormatCode = ChartNumberFormat.DefaultTimeFormatCode;
                    }

                    break;
                }
                default:
                    Debug.Assert(false);
                    break;
            }

        }

        /// <summary>
        /// Adjusts the format codes according to the specified Y value type.
        /// </summary>
        private void AdjustYFormatCodes(ChartYValueType yValueType)
        {
            IDmlChart2D chart2d = (Chart as IDmlChart2D);
            ChartAxis axis = (chart2d != null) ? chart2d.AxY : null;

            switch (yValueType)
            {
                case ChartYValueType.Double:
                {
                    // Do not change current format if it is already numeric.
                    if (DmlChartFormatCodeValidator.IsDateFormat(YDmlChartValues.FormatCode))
                        YDmlChartValues.FormatCode = ChartNumberFormat.GeneralFormatCode;

                    if (mSeries.HasDataLabels &&
                        DataLabels.ShowValue &&
                        DmlChartFormatCodeValidator.IsDateFormat(DataLabels.NumberFormat.FormatCode))
                    {
                        DataLabels.NumberFormat.FormatCode = ChartNumberFormat.GeneralFormatCode;
                    }

                    if (axis != null)
                    {
                        SetAutomaticAxisCategoryType(axis);

                        // Do not change current format if it is already numeric.
                        if (DmlChartFormatCodeValidator.IsDateFormat(axis.NumberFormat.FormatCode))
                            axis.NumberFormat.FormatCode = ChartNumberFormat.GeneralFormatCode;
                    }

                    break;
                }
                case ChartYValueType.DateTime:
                {
                    // Do not change current format if it is already DateTime.
                    if (!IsDateTimeFormat(YDmlChartValues.FormatCode))
                        YDmlChartValues.FormatCode = ChartNumberFormat.DefaultDateFormatCode;

                    if (mSeries.HasDataLabels &&
                        DataLabels.ShowValue &&
                        !IsDateTimeFormat(DataLabels.NumberFormat.FormatCode))
                    {
                        DataLabels.NumberFormat.FormatCode = ChartNumberFormat.DefaultDateFormatCode;
                    }

                    if ((axis != null) && !Chart.IsPercentStacked)
                    {
                        axis.CategoryType = AxisCategoryType.Time;

                        // Do not change current format if it is already DateTime.
                        if (!IsDateTimeFormat(axis.NumberFormat.FormatCode))
                            axis.NumberFormat.FormatCode = ChartNumberFormat.DefaultDateFormatCode;
                    }

                    break;
                }
                case ChartYValueType.Time:
                {
                    // Do not change current format if it is already Time.
                    if (!DmlChartFormatCodeValidator.IsTimeFormat(YDmlChartValues.FormatCode))
                        YDmlChartValues.FormatCode = ChartNumberFormat.DefaultTimeFormatCode;

                    if (mSeries.HasDataLabels &&
                        DataLabels.ShowValue &&
                        !DmlChartFormatCodeValidator.IsTimeFormat(DataLabels.NumberFormat.FormatCode))
                    {
                        DataLabels.NumberFormat.FormatCode = ChartNumberFormat.DefaultTimeFormatCode;
                    }

                    if ((axis != null) && !Chart.IsPercentStacked)
                    {
                        // Need to set the Automatic category type, not Time, otherwise MS Word does not display data.
                        axis.CategoryType = AxisCategoryType.Automatic;

                        // Do not change current format if it is already Time.
                        if (!DmlChartFormatCodeValidator.IsTimeFormat(axis.NumberFormat.FormatCode))
                            axis.NumberFormat.FormatCode = ChartNumberFormat.DefaultTimeFormatCode;
                    }

                    break;
                }
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        /// <summary>
        /// Gets format code of the specified dimension data.
        /// </summary>
        private static string GetFormatCode(DmlChartDimensionData dimensionData)
        {
            string formatCode = dimensionData.Values.FormatCode;
            if (StringUtil.HasChars(formatCode))
                return formatCode;

            DmlChartValue chartValue = dimensionData.Values.GetFirstNotNullValue();

            DmlChartNumValue chartNumValue = chartValue as DmlChartNumValue;
            if (chartNumValue != null)
                return chartNumValue.FormatCode;

            DmlChartMultiLvlNumValue chartMultilevelValue = chartValue as DmlChartMultiLvlNumValue;
            if (chartMultilevelValue != null)
                return chartMultilevelValue.GetFormatCode(0);

            return null;
        }

        /// <summary>
        /// Gets a X value type that corresponds to the specified format code.
        /// </summary>
        private static ChartXValueType GetXValueType(string formatCode)
        {
            bool isDateTime =
                StringUtil.HasChars(formatCode) &&
                DmlChartFormatCodeValidator.IsDateFormat(formatCode);

            if (!isDateTime)
                return ChartXValueType.Double;

            return DmlChartFormatCodeValidator.IsTimeFormat(formatCode)
                ? ChartXValueType.Time
                : ChartXValueType.DateTime;
        }

        /// <summary>
        /// Gets a Y value type that corresponds to the specified format code.
        /// </summary>
        private static ChartYValueType GetYValueType(string formatCode)
        {
            bool isDateTime =
                StringUtil.HasChars(formatCode) &&
                DmlChartFormatCodeValidator.IsDateFormat(formatCode);

            if (!isDateTime)
                return ChartYValueType.Double;

            return DmlChartFormatCodeValidator.IsTimeFormat(formatCode)
                ? ChartYValueType.Time
                : ChartYValueType.DateTime;
        }

        #region ISeriesDataStore

        ChartXValue ISeriesDataStore.GetXValue(int index, ChartXValueType valueType)
        {
            DmlChartValue dmlValue = XDmlChartValues[index];
            if (DmlChartValue.IsNullOrNone(dmlValue))
                return null;

            switch (valueType)
            {
                case ChartXValueType.String:
                {
                    switch (dmlValue.ValueType)
                    {
                        case DmlChartValueType.String:
                            return ChartXValue.FromString(dmlValue.StringValue);
                        case DmlChartValueType.MultiLvlString:
                        {
                            DmlChartMultiLvlStrValue multiLevelValue = (DmlChartMultiLvlStrValue)dmlValue;
                            Debug.Assert(multiLevelValue.LevelsCount == 1);
                            return ChartXValue.FromString((string)multiLevelValue.Levels[0]);
                        }
                        default:
                            Debug.Assert(false);
                            return null;
                    }
                }
                case ChartXValueType.Multilevel:
                {
                    Debug.Assert(dmlValue.ValueType == DmlChartValueType.MultiLvlString);
                    DmlChartMultiLvlStrValue value = (DmlChartMultiLvlStrValue)dmlValue;
                    return ChartXValue.FromMultilevelValue(new ChartMultilevelValue(
                        value.GetLevelTextInReverseOrder(0),
                        value.GetLevelTextInReverseOrder(1),
                        value.GetLevelTextInReverseOrder(2)));
                }
                case ChartXValueType.Double:
                {
                    Debug.Assert(
                        (dmlValue.ValueType == DmlChartValueType.Numeric) ||
                        (dmlValue.ValueType == DmlChartValueType.MultiLvlNumeric));
                    return ChartXValue.FromDouble(dmlValue.Value);
                }
                case ChartXValueType.DateTime:
                {
                    Debug.Assert((dmlValue.ValueType == DmlChartValueType.Numeric) && dmlValue.IsDate);
                    DateTime dateTime = DmlChartUtil.GetDateFromDouble(dmlValue.Value);
                    return ChartXValue.FromDateTime(dateTime);
                }
                case ChartXValueType.Time:
                {
                    Debug.Assert((dmlValue.ValueType == DmlChartValueType.Numeric) && dmlValue.IsDate);
                    TimeSpan timeSpan = TimeSpan.FromDays(dmlValue.Value);
                    return ChartXValue.FromTimeSpan(timeSpan);
                }
                default:
                    Debug.Assert(false);
                    return null;
            }
        }

        ChartYValue ISeriesDataStore.GetYValue(int index, ChartYValueType valueType)
        {
            DmlChartValue dmlValue = YDmlChartValues[index];
            if (DmlChartValue.IsNullOrNone(dmlValue))
                return null;

            Debug.Assert(
                (dmlValue.ValueType == DmlChartValueType.Numeric) ||
                ((dmlValue.ValueType == DmlChartValueType.MultiLvlNumeric) && (dmlValue.LevelsCount == 1)));

            switch (valueType)
            {
                case ChartYValueType.Double:
                    return ChartYValue.FromDouble(dmlValue.Value);
                case ChartYValueType.DateTime:
                {
                    Debug.Assert(YDmlChartValues.IsDate || dmlValue.IsDate);
                    DateTime dateTime = DmlChartUtil.GetDateFromDouble(dmlValue.Value);
                    return ChartYValue.FromDateTime(dateTime);
                }
                case ChartYValueType.Time:
                {
                    Debug.Assert(YDmlChartValues.IsDate || dmlValue.IsDate);
                    TimeSpan timeSpan = TimeSpan.FromDays(dmlValue.Value);
                    return ChartYValue.FromTimeSpan(timeSpan);
                }
                default:
                    Debug.Assert(false);
                    return null;
            }
        }

        double ISeriesDataStore.GetBubbleSize(int index)
        {
            DmlChartValue dmlValue = BubbleSizeDmlChartValues[index];
            return !DmlChartValue.IsNullOrNone(dmlValue)
                ? dmlValue.Value
                : NullBubbleSize;
        }

        /// <summary>
        /// Changes the value in the <see cref="XDmlChartValues"/> collection at the specified index.
        /// </summary>
        void ISeriesDataStore.ChangeXValue(ChartXValue xValue, int index)
        {
            DataPointInfo[] oldPointInfos = null;
            int oldPointCount = 0;
            if (HasDataPointsOrLabels && IsComplexCorrespondenceBetweenDataPointsAndXValues)
            {
                oldPointInfos = DataPointInfoRetriever.GetDataPointInfos(index, true);
                oldPointCount = DataPoints.Count;
            }

            DmlChartValue dmlValue = ChartXValueToDmlChartValue(xValue, index);
            XDmlChartValues.AddNullAware(dmlValue, index);

            if (HasDataPointsOrLabels && IsComplexCorrespondenceBetweenDataPointsAndXValues)
            {
                DataPointInfo[] newPointInfos = DataPointInfoRetriever.GetDataPointInfos(index, true);
                int newPointCount = DataPoints.Count;
                UpdatePoints(oldPointInfos, oldPointCount, newPointInfos, newPointCount);
            }

            DataChanged();
        }

        /// <summary>
        /// Changes the value in the <see cref="YDmlChartValues"/> collection at the specified index.
        /// </summary>
        void ISeriesDataStore.ChangeYValue(ChartYValue yValue, int index)
        {
            DataPointInfo[] oldPointInfos = null;
            int oldPointCount = 0;
            if (HasDataPointsOrLabels && IsComplexCorrespondenceBetweenDataPointsAndYValues)
            {
                oldPointInfos = DataPointInfoRetriever.GetDataPointInfos(index, true);
                oldPointCount = DataPoints.Count;
            }

            DmlChartValue dmlValue = ChartYValueToDmlChartValue(yValue, index);
            YDmlChartValues.AddNullAware(dmlValue, index);

            if (HasDataPointsOrLabels && IsComplexCorrespondenceBetweenDataPointsAndYValues)
            {
                DataPointInfo[] newPointInfos = DataPointInfoRetriever.GetDataPointInfos(index, true);
                int newPointCount = DataPoints.Count;
                UpdatePoints(oldPointInfos, oldPointCount, newPointInfos, newPointCount);
            }

            DataChanged();
        }

        /// <summary>
        /// Changes the value in the <see cref="BubbleSizeDmlChartValues"/> collection at the specified index.
        /// </summary>
        void ISeriesDataStore.ChangeBubbleSize(double bubbleSize, int index)
        {
            DmlChartValue dmlValue = BubbleSizeToDmlChartValue(bubbleSize, index);
            BubbleSizeDmlChartValues.AddNullAware(dmlValue, index);

            DataChanged();
        }

        ChartXValueType ISeriesDataStore.XValueType
        {
            get
            {
                if (XDmlChartValues == null)
                    return ChartXValueType.Double; // Just return some value, it should not be used.

                DmlChartValueType dmlValueType = XDmlChartValues.ValueType;

                switch (dmlValueType)
                {
                    case DmlChartValueType.None:
                        Debug.Assert(false);
                        return ChartXValueType.Double; // Just return some value, it should not be used.
                    case DmlChartValueType.String:
                        return ChartXValueType.String;
                    case DmlChartValueType.MultiLvlString:
                    {
                        return
                            (!IsChartExChart ||
                             ((LayoutType == SeriesLayout.Treemap) || (LayoutType == SeriesLayout.Sunburst)))
                                ? ChartXValueType.Multilevel
                                : ChartXValueType.String;
                    }
                    case DmlChartValueType.Numeric:
                    case DmlChartValueType.MultiLvlNumeric:
                    {
                        Debug.Assert(
                            (dmlValueType != DmlChartValueType.MultiLvlNumeric) ||
                            (XDmlChartValues.LevelProperties.Count == 1));

                        string formatCode = GetFormatCode(mSeries.X);

                        return GetXValueType(formatCode);
                    }
                    default:
                        Debug.Assert(false);
                        return ChartXValueType.Double;
                }
            }
            set
            {
                if (((ISeriesDataStore)this).XValueType == value)
                    return;

                if (!IsXValueSupported)
                    throw new InvalidOperationException("X values are not supported by this series type.");

                if (!IsXValueTypeSupported(value))
                    throw new InvalidOperationException("The series cannot contain an X value of this type.");

                Debug.Assert(XDmlChartValues != null);

                if (XDmlChartValues.HasNonEmptyValues)
                {
                    // This exception is generated when ChartXValue is set/inserted to ChartXValueCollection.
                    throw new InvalidOperationException(
                        "The value must be of the same type as the existing items in the collection.");
                }

                switch (value)
                {
                    case ChartXValueType.String:
                    case ChartXValueType.Multilevel:
                    {
                        XDmlChartValues.ValueType = (IsChartExChart || (value == ChartXValueType.Multilevel))
                            ? DmlChartValueType.MultiLvlString
                            : DmlChartValueType.String;

                        break;
                    }
                    default:
                    {
                        XDmlChartValues.ValueType = IsChartExChart
                            ? DmlChartValueType.MultiLvlNumeric
                            : DmlChartValueType.Numeric;

                        break;
                    }
                }

                AdjustXFormatCodes(value);
            }
        }

        ChartYValueType ISeriesDataStore.YValueType
        {
            get
            {
                if (YDmlChartValues == null)
                    return ChartYValueType.Double;

                DmlChartValueType dmlValueType = YDmlChartValues.ValueType;
                if ((dmlValueType != DmlChartValueType.Numeric) && (dmlValueType != DmlChartValueType.MultiLvlNumeric))
                {
                    Debug.Assert(false);
                    return ChartYValueType.Double;
                }

                string formatCode = GetFormatCode(mSeries.Y);
                return GetYValueType(formatCode);
            }
            set
            {
                if (((ISeriesDataStore)this).YValueType == value)
                    return;

                if (!IsYValueSupported)
                    throw new InvalidOperationException("Y values are not supported by this series type.");

                Debug.Assert(YDmlChartValues != null);

                if (YDmlChartValues.HasNonEmptyValues)
                {
                    // This exception is generated when ChartYValue is set/inserted to ChartYValueCollection.
                    throw new InvalidOperationException(
                        "The value must be of the same type as the existing items in the collection.");
                }

                YDmlChartValues.ValueType = IsChartExChart
                    ? DmlChartValueType.MultiLvlNumeric
                    : DmlChartValueType.Numeric;

                AdjustYFormatCodes(value);
            }
        }

        string ISeriesDataStore.XFormatCode
        {
            get
            {
                return (IsXValueSupported && StringUtil.HasChars(XDmlChartValues.FormatCode))
                    ? XDmlChartValues.FormatCode
                    : string.Empty;
            }
            set
            {
                if (!IsXValueSupported)
                    throw new InvalidOperationException("X values are not supported by this series type.");

                // Let's disallow setting format code for string values in a Word 2016 chart because it may cause
                // error when opening the document in MS Word.
                ChartXValueType valueType = ((ISeriesDataStore)this).XValueType;
                bool isStringData = ((valueType == ChartXValueType.String) || (valueType == ChartXValueType.Multilevel));
                if (IsChartExChart && isStringData)
                {
                    throw new InvalidOperationException(
                        "Format code cannot be specified for string X values in Word 2016 charts.");
                }

                XDmlChartValues.FormatCode = value;
            }
        }

        string ISeriesDataStore.YFormatCode
        {
            get
            {
                return (IsYValueSupported && StringUtil.HasChars(YDmlChartValues.FormatCode))
                    ? YDmlChartValues.FormatCode
                    : string.Empty;
            }
            set
            {
                if (!IsYValueSupported)
                    throw new InvalidOperationException("Y values are not supported by this series type.");

                YDmlChartValues.FormatCode = value;
            }
        }

        string ISeriesDataStore.BubbleSizeFormatCode
        {
            get
            {
                return (IsBubbleSizeSupported && StringUtil.HasChars(BubbleSizeDmlChartValues.FormatCode))
                    ? BubbleSizeDmlChartValues.FormatCode
                    : string.Empty;
            }
            set
            {
                if (!IsBubbleSizeSupported)
                    throw new InvalidOperationException("Bubble sizes are not supported by this series type.");

                BubbleSizeDmlChartValues.FormatCode = value;
            }
        }

        #endregion

        /// <summary>
        /// Gets a flag indicating whether the series supports X values.
        /// </summary>
        public bool IsXValueSupported
        {
            get
            {
                // Word 2016 Pareto line series does not have data.
                return !IsChartExChart || (LayoutType != SeriesLayout.ParetoLine);
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the series supports Y values.
        /// </summary>
        public bool IsYValueSupported
        {
            get
            {
                if (!IsChartExChart)
                    return true;

                return
                    // Word 2016 Histogram chart has only X values.
                    !((LayoutType == SeriesLayout.ClusteredColumn) && !mSeries.LayoutPr.IsAggregation) &&
                    // Word 2016 Pareto line series does not have data.
                    (LayoutType != SeriesLayout.ParetoLine);
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the series supports bubble sizes.
        /// </summary>
        public bool IsBubbleSizeSupported
        {
            get { return (Chart.ChartType == DmlChartType.BubbleChart); }
        }

        /// <summary>
        /// Gets or sets a number of values in the series.
        /// </summary>
        public int ValueCount
        {
            get
            {
                int count = 0;

                // When last values of a collection are empty, sometimes MS Word reduces PointsCount, thus we must
                // check PointsCount of all collections here.

                if (IsXValueSupported && (XDmlChartValues != null))
                    count = XDmlChartValues.ValueCount;

                if (IsYValueSupported && (YDmlChartValues != null))
                    count = System.Math.Max(count, YDmlChartValues.ValueCount);

                if (IsBubbleSizeSupported && (BubbleSizeDmlChartValues != null))
                    count = System.Math.Max(count, BubbleSizeDmlChartValues.ValueCount);

                return count;
            }
            set
            {
                if (IsXValueSupported)
                    XDmlChartValues.ValueCount = value;

                if (IsYValueSupported)
                    YDmlChartValues.ValueCount = value;

                if (IsBubbleSizeSupported)
                    BubbleSizeDmlChartValues.ValueCount = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="DmlChartValueCollection"/> of the X values of the series.
        /// </summary>
        private DmlChartValueCollection XDmlChartValues
        {
            get { return mSeries.X.Values; }
        }

        /// <summary>
        /// Gets the <see cref="DmlChartValueCollection"/> of the Y values of the series.
        /// </summary>
        private DmlChartValueCollection YDmlChartValues
        {
            get { return mSeries.Y.Values; }
        }

        /// <summary>
        /// Gets the <see cref="DmlChartValueCollection"/> of the bubble sizes of the series.
        /// </summary>
        private DmlChartValueCollection BubbleSizeDmlChartValues
        {
            get { return mSeries.Size.Values; }
        }

        /// <summary>
        /// Gets the parent chart of the series.
        /// </summary>
        private DmlChart Chart
        {
            get { return mSeries.Chart; }
        }

        /// <summary>
        /// Gets a flag indicating whether the parent chart is a Word 2016 chart.
        /// </summary>
        private bool IsChartExChart
        {
            get { return (Chart.ChartType == DmlChartType.ChartExChart); }
        }

        /// <summary>
        /// Gets the layout type of the series. Relates to Word 2016 charts only.
        /// </summary>
        private SeriesLayout LayoutType
        {
            get { return mSeries.LayoutType; }
        }

        /// <summary>
        /// Gets a flag indicating whether data points/labels of the next chart data value may be affected by changing
        /// an X value.
        /// </summary>
        private bool IsXValueAffectsNextValueDataPoints
        {
            get
            {
                if (!IsChartExChart)
                    return false;

                switch (LayoutType)
                {
                    case SeriesLayout.Treemap:
                    case SeriesLayout.Sunburst:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Gets a flag indicating whether inserting/removing an X value may affect inserting/removing multiple data
        /// points/labels associated with it.
        /// </summary>
        private bool IsComplexCorrespondenceBetweenDataPointsAndXValues
        {
            get
            {
                if (!IsChartExChart)
                    return false;

                switch (LayoutType)
                {
                    case SeriesLayout.BoxWhisker:
                    case SeriesLayout.Treemap:
                    case SeriesLayout.Sunburst:
                        return true;
                    case SeriesLayout.ClusteredColumn:
                        return mSeries.LayoutPr.IsAggregation; // Pareto chart.
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Gets a flag indicating whether inserting/removing an Y value may affect inserting/removing multiple data
        /// points/labels associated with it.
        /// </summary>
        private bool IsComplexCorrespondenceBetweenDataPointsAndYValues
        {
            get { return IsChartExChart && (LayoutType == SeriesLayout.BoxWhisker); }
        }

        /// <summary>
        /// Gets a flag indicating whether changing a chart data value does not affect inserting/removing data
        /// points/labels or MS Word algorithm of this is too complex.
        /// </summary>
        private bool IsNoCorrespondenceBetweenDataPointsAndValues
        {
            get
            {
                return
                    // Histogram chart
                    IsChartExChart &&
                    (LayoutType == SeriesLayout.ClusteredColumn) &&
                    !mSeries.LayoutPr.IsAggregation;
            }
        }

        /// <summary>
        /// Gets the data point collection of the series.
        /// </summary>
        private ChartDataPointCollection DataPoints
        {
            get { return mSeries.DataPoints; }
        }

        /// <summary>
        /// Gets the data label collection of the series.
        /// </summary>
        private ChartDataLabelCollection DataLabels
        {
            get { return mSeries.DataLabels; }
        }

        /// <summary>
        /// Gets a flag indicating whether the series contains any individual data points or labels.
        /// </summary>
        private bool HasDataPointsOrLabels
        {
            get { return DataLabels.HasItems || DataPoints.HasItems; }
        }

        /// <summary>
        /// Get an instance of the <see cref="ChartExDataPointInfoRetriever"/> class used to get information about
        /// data points/labels related to a particular data value.
        /// </summary>
        private ChartExDataPointInfoRetriever DataPointInfoRetriever
        {
            get
            {
                if (mDataPointInfoRetriever == null)
                    mDataPointInfoRetriever = new ChartExDataPointInfoRetriever(mSeries, true);

                return mDataPointInfoRetriever;
            }
        }

        private readonly ChartSeries mSeries;
        private ChartExDataPointInfoRetriever mDataPointInfoRetriever;

        private const double NullBubbleSize = double.NaN;
    }
}
