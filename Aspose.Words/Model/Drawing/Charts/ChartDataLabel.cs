// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/06/2012 by Alexey Noskov

using System;
using System.Text;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents data label on a chart point or trendline.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// On a series, the <see cref="ChartDataLabel"/> object is a member of the <see cref="ChartDataLabelCollection"/>.
    /// The <see cref="ChartDataLabelCollection"/> contains a <see cref="ChartDataLabel"/> object for each point.
    /// </remarks>
    public class ChartDataLabel : INumberFormatProvider, IChartItemTextProperties, IChartFormatSource
    {
        internal ChartDataLabel(DmlChartDataLabelPr parentPr, ChartSeries series)
        {
            mLabelPr = new DmlChartDataLabelPr(parentPr, (series != null) ? series.Chart : null);
            mSeries = series;
        }

        /// <summary>
        /// Clears format of this data label. The properties are set to the default values defined in the parent data
        /// label collection.
        /// </summary>
        public void ClearFormat()
        {
            int index = Index;
            mLabelPr.Clear();
            Index = index;
        }

        internal ChartDataLabel Clone()
        {
            ChartDataLabel lhs = (ChartDataLabel)MemberwiseClone();
            if (mLabelPr != null)
                lhs.mLabelPr = mLabelPr.Clone();

            if (mTx != null)
                lhs.mTx = mTx.Clone();

            return lhs;
        }

        internal void Warn(WarningType warningType, string message)
        {
            // For data labels of PivotFormat, chart is null. So added check.
            // This must be revised when PivotFormat will be moved to public API.
            if (Chart != null)
                Chart.Warn(warningType, message);
        }

        /// <summary>
        /// Sets the parent chart of this data label.
        /// </summary>
        internal void SetSeries(ChartSeries series)
        {
            mSeries = series;
        }

        /// <summary>
        /// Generates <see cref="Tx"/> if current data label does not have explicitly set text.
        /// </summary>
        internal void UpdateText(ChartSeries series)
        {
            // Generate Tx if current data label does not have it.
            if (Tx == null)
                throw new NotSupportedException("FOSS");
            else
                UpdateFields(Tx, series);
        }

        /// <summary>
        /// Update fields within data labels tx.
        /// </summary>
        private void UpdateFields(DmlChartTx tx, ChartSeries series)
        {
            if ((tx == null) || (tx.RichText == null) || (series == null))
                return;

            foreach (DmlParagraph paragraph in tx.RichText.Paragraphs)
            {
                foreach (DmlParagraphTextElementBase element in paragraph.Elements)
                {
                    DmlTextField field = element as DmlTextField;
                    if (field == null)
                        continue;

                    switch (field.Type)
                    {
                        case ChartDataLabelFieldType.CellRange:
                            field.Text = series.DataLabelsRangeData.GetValue(SeriesElementIndex).StringValue;
                            break;
                        // WORDSNET-15672 Update [VALUE] field.
                        case ChartDataLabelFieldType.Value:
                        // WORDSNET-23267 Update [Y VALUE] field.
                        case ChartDataLabelFieldType.YValue:
                            field.Text = GetValue(series);
                            break;
                        // WORDSNET-16609 Update [CATEGORYNAME], [SERIESNAME], [PERCENTAGE] fields.
                        case ChartDataLabelFieldType.CategoryName:
                        // WORDSNET-25342 Update [XVALUE] field. Rendering x-value of data point.
                        case ChartDataLabelFieldType.XValue:
                            field.Text = GetCategoryName(series);
                            break;
                        case ChartDataLabelFieldType.SeriesName:
                            field.Text = series.Name;
                            break;
                        case ChartDataLabelFieldType.Percentage:
                            field.Text = GetPercentage(series);
                            break;
                        // WORDSNET-23851 Update [BUBBLE SIZE] field.
                        case ChartDataLabelFieldType.BubbleSize:
                            field.Text = GetValue(series.Size.GetValue(SeriesElementIndex));
                            break;
                        default:
                            // Do nothing for default.
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets actual layout.
        /// </summary>
        /// <remarks>
        /// If the extension layout and direct layout are specified, then the actual layout must be composed of the parameters
        /// of the both layouts. X and Y values must be taken from the direct layout, and other parameters -
        /// from the extension layout.
        /// </remarks>
        private DmlChartManualLayout GetActualLayout()
        {
            DmlChartManualLayout actualLayout;
            DmlChartManualLayout extManual = (DmlChartManualLayout)mLabelPr.GetExtensionProperty(DmlChartDataLabelAttrs.Layout);
            DmlChartManualLayout directLayout = (DmlChartManualLayout)mLabelPr.GetDirectProperty(DmlChartDataLabelAttrs.Layout);

            // WORDSNET-16640 The X and Y values from the direct layout have a higher priority than the values
            // from the extension layout, but the width and the hight must be taken from the extension layout.
            if ((extManual != null) && (directLayout != null))
            {
                actualLayout = extManual.Clone();
                actualLayout.Top = directLayout.Top;
                actualLayout.Left = directLayout.Left;
            }
            else
            {
                actualLayout = (DmlChartManualLayout)mLabelPr.GetProperty(DmlChartDataLabelAttrs.Layout);
            }

            return actualLayout;
        }

        /// <summary>
        /// Generates text string for the current data label.
        /// </summary>
        /// <remarks>
        /// Order matters.
        /// </remarks>
        private string GenerateDataLabelText(ChartSeries series)
        {
            StringBuilder labelBuilder = new StringBuilder();

            // WORDSNET-23624 Data labels range rendered first.
            if (ShowDataLabelsRange)
                AddTextToDataLabel(labelBuilder, series.DataLabelsRangeData.GetValue(SeriesElementIndex).StringValue);

            if (ShowSeriesName)
                AddTextToDataLabel(labelBuilder, series.Name);

            bool useCurrentSeries = Chart.IsPieChart || Chart.IsScatter;

            // The first series is used to load category data.
            // WORDSNET-21748 For a scatter chart use the current series (not the first series) to get correct data.
            if (ShowCategoryName)
                AddTextToDataLabel(labelBuilder, GetCategoryName(useCurrentSeries ? series : Chart.FirstSeries));

            if (ShowValue)
                AddTextToDataLabel(labelBuilder, GetValue(series));

            // Percents are rendered only for pie charts.
            if (ShowPercentage && Chart.IsPieChart)
                AddTextToDataLabel(labelBuilder, GetPercentage(series));

            // Bubble size are rendered only for bubble charts.
            // WORDSNET-23851 The bubble size is rendered at the end of the label text.
            if (ShowBubbleSize && Chart.IsBubbleChart)
                AddTextToDataLabel(labelBuilder, GetValue(series.Size.GetValue(SeriesElementIndex)));

            return labelBuilder.ToString();
        }

        /// <summary>
        /// Appends the data label separator and the specified text to the specified label builder.
        /// </summary>
        /// <param name="labelBuilder">The specified <see cref="StringBuilder"/></param>
        /// <param name="addedText">The specified text</param>
        private void AddTextToDataLabel(StringBuilder labelBuilder, string addedText)
        {
            AppendSeparator(labelBuilder);
            labelBuilder.Append(addedText);
        }

        /// <summary>
        /// Returns a string with the percentage value.
        /// </summary>
        /// <param name="series">The specified <see cref="ChartSeries"/></param>
        /// <returns>String with the percentage value</returns>
        private string GetPercentage(ChartSeries series)
        {
            // MS Word shows whole numbers of percents, if a format code is not specified, so use "0%" format.
            string formatCode = (NumFmt != null) && !DmlChartFormatCodeValidator.IsGeneralFormatCode(NumFmt.FormatCode)
                ? NumFmt.FormatCode
                : "0%";

            double val = series.Y.Data.GetPercentValue(SeriesElementIndex, formatCode);
            return FormatterPal.DoubleToStr(val, formatCode);
        }

        /// <summary>
        /// Returns a string with the category value.
        /// </summary>
        /// <param name="series">The specified <see cref="ChartSeries"/></param>
        /// <returns>String with category value</returns>
        private string GetCategoryName(ChartSeries series)
        {
            if (series == null)
                return string.Empty;

            DmlChartDimensionData xValues = series.X;

            // WORDSNET-21690 The source x-values may be not specified. In this case return an empty string.
            if (xValues.IsEmpty || MathUtil.IsZero(xValues.Values.ValueCount))
                return string.Empty;

            DmlChartValue xValue = xValues.Values[SeriesElementIndex];

            if (DmlChartValue.IsNullOrNaN(xValue) || (xValue.StringValue == DmlChartValue.NanStringValue))
                return string.Empty;

            DmlChartNumValue numVal = xValue as DmlChartNumValue;

            // If NumFmt is specified and not linked to the source - use the format code from the data label NumFmt,
            // otherwise, if format code is specified for the value - use the it, else - use the series format code.
            string formatCode = IsNumFmtUsed
                ? NumFmt.FormatCode
                : ((numVal == null) || (numVal.FormatCode == null)) ? series.X.Data.FormatCode : numVal.FormatCode;

            if (numVal == null)
                return xValue.StringValue;

            if (xValue.IsDate)
            {
                DateTime dateTime = GetDate(xValue);
                throw new NotSupportedException("FOSS");
            }

            return GetStringValue(xValue, formatCode);

        }

        /// <summary>
        /// Sets the text color, if it is specified in the format code.
        /// </summary>
        /// <param name="seriesFormatCode">The source format code of the specified <see cref="ChartSeries"/> </param>
        /// <param name="dmlVal">The specified <see cref="DmlChartValue"/></param>
        private void SetLabelTextColor(string seriesFormatCode, DmlChartValue dmlVal)
        {
            DmlChartNumValue numVal = dmlVal as DmlChartNumValue;

            if (numVal == null)
                return;

            string sourceFormatCode = IsNumFmtUsed
                ? NumFmt.SourceFormatCode
                : ((numVal.SourceFormatCode == null)) ? seriesFormatCode : numVal.FormatCode;

            if (DmlChartFormatCodeValidator.IsGeneralFormatCode(sourceFormatCode))
                return;

            DrColor color = DmlChartFormatCodeColors.GetValueColor(dmlVal.Value, sourceFormatCode);

            if (color.IsEmpty)
                return;

            // mLabelPr should not be changed. Clone TxPr.
            if (IsTxPrSpecified)
                mTxPr = TxPr.Clone();

            TxPr.RunPr.Fill = new DmlSolidFill(DmlColor.CreateFromDrColor(color));
        }

        /// <summary>
        /// Gets a string representation of <see cref="DmlChartValue"/> linked with the current data label and sets the color.
        /// </summary>
        /// <param name="series">The specified <see cref="ChartSeries"/></param>
        /// <returns>The string  representation of <see cref="DmlChartValue"/></returns>
        private string GetValue(ChartSeries series)
        {
            DmlChartValue dmlVal = series.Y.Data.GetOriginalValue(SeriesElementIndex);
            string seriesFormatCode = series.Y.Values.FormatCode;
            SetLabelTextColor(seriesFormatCode, dmlVal);

            return GetValue(dmlVal);
        }

        /// <summary>
        /// Gets a string representation of the specified <see cref="DmlChartValue"/>.
        /// </summary>
        /// <param name="dmlVal">The specified <see cref="DmlChartValue"/></param>
        /// <returns>The string  representation of the specified <see cref="DmlChartValue"/></returns>
        private string GetValue(DmlChartValue dmlVal)
        {
            // It seems when percents and values are shown in data label and format code is percents format code,
            // it is not used for values. See TestJira9191.
            if (DmlChartValue.IsNullOrNaN(dmlVal))
                return (dmlVal.IsNaN) ? dmlVal.StringValue : string.Empty;

            string formatCode = IsNumFmtUsed ? NumFmt.FormatCode : string.Empty;

            // WORDSNET-24288 The "ShowPercentage" is enough to ignore the specified format code.
            if (ShowPercentage)
                formatCode = null;

            return GetStringValue(dmlVal, formatCode);
        }

        /// <summary>
        /// Returns a date, taking into account axis BaseTimeUnit.
        /// </summary>
        /// <param name="dmlVal">The specified <see cref="DmlChartValue"/></param>
        /// <returns>The date</returns>
        private DateTime GetDate(DmlChartValue dmlVal)
        {
            DateTime baseDate = DmlChartUtil.GetDateFromDouble(dmlVal.Value);

            IDmlChart2D chart2D = Chart as IDmlChart2D;

            if (chart2D == null)
                return baseDate;

            throw new NotSupportedException("FOSS");
        }

        private static string GetStringValue(DmlChartValue dmlVal, string formatCode)
        {
            string result;

            if (DmlChartFormatCodeValidator.IsGeneralFormatCode(formatCode))
            {
                result = (dmlVal == null) ? string.Empty : dmlVal.StringValue;
            }
            else
            {
                throw new NotSupportedException("FOSS");
            }

            // WORDSNET-24130 MS Word trims the spaces for data labels.
            result = result.Trim();
            return result;
        }

        private static double GetDoubleValue(DmlChartValue dmlVal)
        {
            if (dmlVal == null)
                return 0.0d;

            DmlChartNumValue numValue = dmlVal as DmlChartNumValue;
            // WORDSNET-22431 The display unit should be taken into account if the value type is "DmlChartNumValue".
            return (numValue != null) ? numValue.DisplayUnitValue : dmlVal.Value;
        }

        /// <summary>
        /// Appends data label separator to label builder if required.
        /// </summary>
        private void AppendSeparator(StringBuilder labelBuilder)
        {
            if (labelBuilder.Length != 0)
            {
                labelBuilder.Append(Separator);
                labelBuilder.Append(" ");
            }
        }

        /// <summary>
        /// Converts the specified <see cref="ChartDataLabelLocationMode"/> value to <see cref="LayoutMode"/>.
        /// </summary>
        private static LayoutMode LocationModeToLayoutMode(ChartDataLabelLocationMode value)
        {
            switch (value)
            {
                case ChartDataLabelLocationMode.Absolute:
                    return LayoutMode.Edge;
                case ChartDataLabelLocationMode.Offset:
                default:
                    return LayoutMode.Factor;
            }
        }

        /// <summary>
        /// Converts the specified <see cref="LayoutMode"/> value to <see cref="ChartDataLabelLocationMode"/>.
        /// </summary>
        private static ChartDataLabelLocationMode LayoutModeToLocationMode(LayoutMode value)
        {
            switch (value)
            {
                case LayoutMode.Edge:
                    return ChartDataLabelLocationMode.Absolute;
                case LayoutMode.Factor:
                default:
                    return ChartDataLabelLocationMode.Offset;
            }
        }

        #region IChartFormatSource implementation

        void IChartFormatSource.MaterializeSpPr()
        {
            // This un-links data label SpPr from parent SpPr. Do it only on changing properties, not on reading, for
            // the behavior to not depend on a fact whether a customer has read ChartFormat properties or hasn't.

            if (mLabelPr.IsPropertySpecified(DmlChartDataLabelAttrs.SpPr))
                return;

            // Copy formatting of the parent collection.
            DmlChartSpPr parentSpPr = (DmlChartSpPr)mLabelPr.GetProperty(DmlChartDataLabelAttrs.SpPr);
            DmlChartSpPr spPr = (parentSpPr != null) ? parentSpPr.Clone() : SpPr;
            mLabelPr.SetProperty(DmlChartDataLabelAttrs.SpPr, spPr);
        }

        bool IChartFormatSource.IsFillSupported
        {
            get { return true; }
        }

        DmlFill IChartFormatSource.Fill
        {
            get { return SpPr.Fill; }
            set { SpPr.Fill = value; }
        }

        DmlOutline IChartFormatSource.Outline
        {
            get { return SpPr.Outline; }
            set { SpPr.Outline = value; }
        }

        ChartShapeType IChartFormatSource.ShapeType
        {
            get
            {
                DmlChartShapeProperties shapePr =
                    (DmlChartShapeProperties)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShapePr);

                return ((shapePr != null) && (shapePr.Geometry != null))
                    ? ChartShapeTypeUtil.PresetGeometryToChartShapeType(shapePr.Geometry.PresetName)
                    : ChartShapeType.Default;
            }
            set
            {
                if (((IChartFormatSource)this).ShapeType == value)
                    return;

                if (((IChartFormatSource)mSeries.DataLabels).ShapeType == value)
                {
                    mLabelPr.RemoveExtensionProperty(DmlChartDataLabelAttrs.ShapePr);
                    return;
                }

                DmlChartShapeProperties shapePr =
                    (DmlChartShapeProperties)mLabelPr.GetExtensionProperty(DmlChartDataLabelAttrs.ShapePr);
                if (shapePr == null)
                {
                    shapePr = new DmlChartShapeProperties();
                    mLabelPr.SetExtensionProperty(DmlChartDataLabelAttrs.ShapePr, shapePr);
                }

                if (value == ChartShapeType.Default)
                {
                    shapePr.Geometry = null;
                }
                else
                {
                    if (shapePr.Geometry == null)
                        shapePr.Geometry = new DmlGeometry();

                    shapePr.Geometry.PresetName = ChartShapeTypeUtil.ChartShapeTypeToDmlPresetGeom(value);
                }
            }
        }

        IThemeProvider IChartFormatSource.ThemeProvider
        {
            get { return Chart.Document.GetThemeInternal(); }
        }

        bool IChartFormatSource.IsFormatDefined
        {
            get { return mLabelPr.IsPropertySpecified(DmlChartDataLabelAttrs.SpPr) && !SpPr.IsEmpty; }
        }

        #endregion

        /// <summary>
        /// Specifies the index of the containing element.
        /// This index shall determine which of the parent's children collection this element applies to.
        /// Default value is 0.
        /// </summary>
        public int Index
        {
            get { return (int)mLabelPr.GetProperty(DmlChartDataLabelAttrs.Idx); }
            internal set { mLabelPr.SetProperty(DmlChartDataLabelAttrs.Idx, value); }
        }

        /// <summary>
        /// Allows to specify if category name is to be displayed for the data labels on a chart.
        /// Default value is <c>false</c>.
        /// </summary>
        public bool ShowCategoryName
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowCatName); }
            set { mLabelPr.SetProperty(DmlChartDataLabelAttrs.ShowCatName, value); }
        }

        /// <summary>
        /// Allows to specify if bubble size is to be displayed for the data labels on a chart.
        /// Applies only to Bubble charts.
        /// Default value is <c>false</c>.
        /// </summary>
        public bool ShowBubbleSize
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowBubbleSize); }
            set
            {
                if (Chart.IsBubbleChart)
                    mLabelPr.SetProperty(DmlChartDataLabelAttrs.ShowBubbleSize, value);
                else
                    Chart.Warn(WarningType.MinorFormattingLoss,
                        "ShowBubbleSize is not supported by this type of chart, value will not be set.");
            }
        }

        /// <summary>
        /// Allows to specify if legend key is to be displayed for the data labels on a chart.
        /// Default value is <c>false</c>.
        /// </summary>
        public bool ShowLegendKey
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowLegendKey); }
            set { mLabelPr.SetProperty(DmlChartDataLabelAttrs.ShowLegendKey, value); }
        }

        /// <summary>
        /// Allows to specify if percentage value is to be displayed for the data labels on a chart.
        /// Default value is <c>false</c>.
        /// </summary>
        public bool ShowPercentage
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowPercent); }
            set { mLabelPr.SetProperty(DmlChartDataLabelAttrs.ShowPercent, value); }
        }

        /// <summary>
        /// Returns or sets a Boolean to indicate the series name display behavior for the data labels on a chart.
        /// <c>true</c> to show the series name; <c>false</c> to hide. By default <c>false</c>.
        /// </summary>
        public bool ShowSeriesName
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowSerName); }
            set { mLabelPr.SetProperty(DmlChartDataLabelAttrs.ShowSerName, value); }
        }

        /// <summary>
        /// Allows to specify if values are to be displayed in the data labels.
        /// Default value is <c>false</c>.
        /// </summary>
        public bool ShowValue
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowVal); }
            set { mLabelPr.SetProperty(DmlChartDataLabelAttrs.ShowVal, value); }
        }

        /// <summary>
        /// Allows to specify if data label leader lines need be shown.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Applies to Pie charts only.
        /// Leader lines create a visual connection between a data label and its corresponding data point.
        /// </remarks>
        public bool ShowLeaderLines
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowLeaderLines); }
            set { mLabelPr.SetProperty(DmlChartDataLabelAttrs.ShowLeaderLines, value); }
        }

        /// <summary>
        /// Allows to specify if values from data labels range to be displayed in the data labels.
        /// Default value is <c>false</c>.
        /// </summary>
        public bool ShowDataLabelsRange
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowDataLabelsRange); }
            set
            {
                mLabelPr.SetExtensionProperty(DmlChartDataLabelAttrs.ShowDataLabelsRange, value);
                // When the property is defined, UniqueId extension is required.
                mLabelPr.CreateUniqueIdExtensionIfNone();
            }
        }

        /// <summary>
        /// Gets or sets string separator used for the data labels on a chart.
        /// The default is a comma, except for pie charts showing only category name and percentage, when a line break
        /// shall be used instead.
        /// </summary>
        public string Separator
        {
            get { return mLabelPr.GetSeparator(Chart.IsPieChart); }
            set { mLabelPr.SetProperty(DmlChartDataLabelAttrs.Separator, value); }
        }

        /// <summary>
        /// Gets or sets the orientation of the label text.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="ShapeTextOrientation.Horizontal"/>.
        /// </remarks>
        public ShapeTextOrientation Orientation
        {
            get { return BodyPr.TextOrientation; }
            set { BodyPrForUpdate.TextOrientation = value; }
        }

        /// <summary>
        /// Gets or sets the rotation of the label in degrees.
        /// </summary>
        /// <remarks>
        /// <para>The range of acceptable values is from -180 to 180 inclusive. The default value is 0.</para>
        /// <para>If the <see cref="Orientation"/> value is <see cref="ShapeTextOrientation.Horizontal"/>, the
        /// label shape, if it exists, is rotated along with the label text. Otherwise, only the label text is rotated.
        /// </para>
        /// </remarks>
        public int Rotation
        {
            get { return (int)System.Math.Round(BodyPr.Rotation.ValueInDegrees); }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, -180, 180, "value");
                BodyPrForUpdate.Rotation = DmlAngle.FromDegrees(value);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this data label has something to display.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return ShowBubbleSize || ShowCategoryName || ShowLegendKey|| ShowPercentage || ShowSeriesName || ShowValue ||
                    ShowDataLabelsRange;
            }
        }

        /// <summary>
        /// Returns number format of the parent element.
        /// </summary>
        public ChartNumberFormat NumberFormat
        {
            get
            {
                if (mNumberFormat == null)
                    mNumberFormat = new ChartNumberFormat(this, Chart);

                return mNumberFormat;
            }
        }

        /// <summary>
        /// Gets/sets a flag indicating whether this label is hidden.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool IsHidden
        {
            get
            {
                if (HasNonDefaultFormatting)
                {
                    // If properties specified use direct 'Delete' properly since it is not inheritable.
                    object val = mLabelPr.GetDirectProperty(DmlChartDataLabelAttrs.Delete);
                    return (val != null) && (bool)val;
                }
                else
                {
                    return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.Delete);
                }
            }

            set { mLabelPr.SetProperty(DmlChartDataLabelAttrs.Delete, value); }
        }

        /// <summary>
        /// Provides access to the font formatting of this data label.
        /// </summary>
        public Font Font
        {
            get
            {
                if (mFont == null)
                {
                    IRunAttrSource runAttrSource = new ChartItemDmlRunPropertiesSource(this, Chart.ChartSpace);
                    mFont = Font.MakeFont(runAttrSource, Chart.Document);
                }

                return mFont;
            }
        }

        /// <summary>
        /// Provides access to fill and line formatting of the data label.
        /// </summary>
        public ChartFormat Format
        {
            get
            {
                if (mFormat == null)
                    mFormat = new ChartFormat(this);

                return mFormat;
            }
        }

        /// <summary>
        /// Gets or sets the distance of the data label in points from the left edge of the chart or from the position
        /// specified by its <see cref="Position"/> property, depending on the value of the <see cref="LeftMode"/>
        /// property.
        /// </summary>
        /// <remarks>
        /// <para>The value of the property changes proportionally if the chart shape is resized.</para>
        /// <para>The property cannot be set in a Word 2016 chart.</para>
        /// </remarks>
        public double Left
        {
            get { return LayoutSafe.IsXSet ? LayoutSafe.Left * ShapeWidth : 0; }
            set
            {
                if (IsChartEx)
                    throw new InvalidOperationException(DmlChartUtil.ChartExUnsupportedProperty);

                if (double.IsNaN(value))
                    throw new ArgumentOutOfRangeException("value");

                const double defaultWidth = 100;
                double shapeWidth = !MathUtil.IsZero(ShapeWidth) ? ShapeWidth : defaultWidth;
                LayoutSafe.Left =  value / shapeWidth;
            }
        }

        /// <summary>
        /// Gets or sets the interpretation mode of the <see cref="Left"/> property value: whether it sets the location
        /// of the data label from the left edge of the chart of from the position specified by its <see cref="Position"/>
        /// property.
        /// </summary>
        /// <remarks>
        /// The property cannot be set in a Word 2016 chart.
        /// </remarks>
        public ChartDataLabelLocationMode LeftMode
        {
            get { return LayoutModeToLocationMode(LayoutSafe.LeftMode); }
            set
            {
                if (IsChartEx)
                    throw new InvalidOperationException(DmlChartUtil.ChartExUnsupportedProperty);

                LayoutSafe.LeftMode = LocationModeToLayoutMode(value);
            }
        }

        /// <summary>
        /// Gets or sets the distance of the data label in points from the top edge of the chart or from the position
        /// specified by its <see cref="Position"/> property, depending on the value of the <see cref="TopMode"/>
        /// property.
        /// </summary>
        /// <remarks>
        /// <para>The value of the property changes proportionally if the chart shape is resized.</para>
        /// <para>The property cannot be set in a Word 2016 chart.</para>
        /// </remarks>
        public double Top
        {
            get { return LayoutSafe.IsYSet ? LayoutSafe.Top * ShapeHeight : 0; }
            set
            {
                if (IsChartEx)
                    throw new InvalidOperationException(DmlChartUtil.ChartExUnsupportedProperty);

                if (double.IsNaN(value))
                    throw new ArgumentOutOfRangeException("value");

                const double defaultHeight = 100;
                double shapeHeight = !MathUtil.IsZero(ShapeHeight) ? ShapeHeight : defaultHeight;
                LayoutSafe.Top = value / shapeHeight;
            }
        }

        /// <summary>
        /// Gets or sets the interpretation mode of the <see cref="Top"/> property value: whether it sets the location
        /// of the data label from the top edge of the chart of from the position specified by its <see cref="Position"/>
        /// property.
        /// </summary>
        /// <remarks>
        /// The property cannot be set in a Word 2016 chart.
        /// </remarks>
        public ChartDataLabelLocationMode TopMode
        {
            get { return LayoutModeToLocationMode(LayoutSafe.TopMode); }
            set
            {
                if (IsChartEx)
                    throw new InvalidOperationException(DmlChartUtil.ChartExUnsupportedProperty);

                LayoutSafe.TopMode = LocationModeToLayoutMode(value);
            }
        }

        /// <summary>
        /// Gets or sets the position of the data label.
        /// </summary>
        /// <remarks>
        /// <para>The position can be set for data labels of the following chart series types:</para>
        /// <para>- <see cref="ChartSeriesType.Bar"/>, <see cref="ChartSeriesType.Column"/>,
        /// <see cref="ChartSeriesType.Histogram"/>, <see cref="ChartSeriesType.Pareto"/>,
        /// <see cref="ChartSeriesType.Waterfall"/>; allowed values: <see cref="ChartDataLabelPosition.Center"/>,
        /// <see cref="ChartDataLabelPosition.InsideBase"/>, <see cref="ChartDataLabelPosition.InsideEnd"/> and
        /// <see cref="ChartDataLabelPosition.OutsideEnd"/>;</para>
        /// <para>- <see cref="ChartSeriesType.BarStacked"/>, <see cref="ChartSeriesType.BarPercentStacked"/>,
        /// <see cref="ChartSeriesType.ColumnStacked"/>, <see cref="ChartSeriesType.ColumnPercentStacked"/>; allowed
        /// values: <see cref="ChartDataLabelPosition.Center"/>, <see cref="ChartDataLabelPosition.InsideBase"/>
        /// and <see cref="ChartDataLabelPosition.InsideEnd"/>;</para>
        /// <para>- <see cref="ChartSeriesType.Bubble"/>, <see cref="ChartSeriesType.Bubble3D"/>,
        /// <see cref="ChartSeriesType.Line"/>, <see cref="ChartSeriesType.LineStacked"/>,
        /// <see cref="ChartSeriesType.LinePercentStacked"/>, <see cref="ChartSeriesType.Scatter"/>,
        /// <see cref="ChartSeriesType.Stock"/>; allowed values: <see cref="ChartDataLabelPosition.Center"/>,
        /// <see cref="ChartDataLabelPosition.Left"/>, <see cref="ChartDataLabelPosition.Right"/>,
        /// <see cref="ChartDataLabelPosition.Above"/> and <see cref="ChartDataLabelPosition.Below"/>;</para>
        /// <para>- <see cref="ChartSeriesType.Pie"/>, <see cref="ChartSeriesType.Pie3D"/>,
        /// <see cref="ChartSeriesType.PieOfBar"/>, <see cref="ChartSeriesType.PieOfPie"/>; allowed values:
        /// <see cref="ChartDataLabelPosition.Center"/>, <see cref="ChartDataLabelPosition.InsideEnd"/>,
        /// <see cref="ChartDataLabelPosition.OutsideEnd"/> and <see cref="ChartDataLabelPosition.BestFit"/>;</para>
        /// <para>- <see cref="ChartSeriesType.BoxAndWhisker"/>; allowed values:
        /// <see cref="ChartDataLabelPosition.Left"/>, <see cref="ChartDataLabelPosition.Right"/>,
        /// <see cref="ChartDataLabelPosition.Above"/> and <see cref="ChartDataLabelPosition.Below"/>.</para>
        /// </remarks>
        public ChartDataLabelPosition Position
        {
            get
            {
                ChartDataLabelPosition position =
                    (ChartDataLabelPosition)mLabelPr.GetProperty(DmlChartDataLabelAttrs.DLblPos);

                return (position != DmlChartDataLabelPr.DefaultPosition)
                    ? position
                    : DmlChartUtil.GetDataLabelDefaultPosition(mSeries.SeriesType);
            }
            set
            {
                if (value == (ChartDataLabelPosition)mLabelPr.GetProperty(DmlChartDataLabelAttrs.DLblPos))
                    return;

                if (!DmlChartUtil.IsDataLabelPositionSupported(mSeries.SeriesType, value))
                {
                    throw new InvalidOperationException(
                        "This series type doesn't support setting the position of a data label.");
                }

                mLabelPr.SetProperty(DmlChartDataLabelAttrs.DLblPos, value);
            }
        }

        /// <summary>
        /// Specifies the position of the data label.
        /// <see cref="ChartDataLabelPosition"/>.
        /// </summary>
        internal ChartDataLabelPosition DLblPos
        {
            get { return (ChartDataLabelPosition)mLabelPr.GetProperty(DmlChartDataLabelAttrs.DLblPos); }
        }

        /// <summary>
        /// Specifies how the chart element is placed on the chart.
        /// <see cref="DmlChartManualLayout"/>.
        /// </summary>
        internal DmlChartManualLayout Layout
        {
            get
            {
                if (mLayout == null)
                    mLayout = GetActualLayout();

                return mLayout;
            }
            set { mLabelPr.SetProperty(DmlChartDataLabelAttrs.Layout, value); }
        }

        /// <summary>
        /// Indicates whether the data label has layout.
        /// </summary>
        internal bool HasLayout
        {
            get { return (Layout != null); }
        }

        /// <summary>
        /// Indicates whether the data label position should be changed.
        /// </summary>
        internal bool IsPositionChanged
        {
            get { return HasLayout && (Layout.IsXSet || Layout.IsYSet); }
        }

        /// <summary>
        /// Indicates whether size of label should be changed.
        /// </summary>
        internal bool IsSizeChanged
        {
            get { return HasLayout && (Layout.IsWidthSet || Layout.IsHeightSet); }
        }

        /// <summary>
        /// Specifies the formatting for the parent chart element.
        /// </summary>
        internal DmlChartSpPr SpPr
        {
            get
            {
                DmlChartSpPr spPr = (DmlChartSpPr)mLabelPr.GetProperty(DmlChartDataLabelAttrs.SpPr);
                if (spPr != null)
                    return spPr;

                if (mSpPr == null)
                    mSpPr = new DmlChartSpPr();

                return mSpPr;
            }
        }

        /// <summary>
        /// Specifies text to use on a chart, including rich text formatting.
        /// <see cref="DmlChartTx"/>.
        /// </summary>
        internal DmlChartTx Tx
        {
            get
            {
                // WORDSNET-12553 Do not use label property to store generated tx used for rendering.
                // Instead store it in private field.
                if (mTx == null)
                    mTx = (DmlChartTx)mLabelPr.GetProperty(DmlChartDataLabelAttrs.Tx);

                return mTx;
            }
        }

        /// <summary>
        /// Checks the combination SpPr and Tx properties.
        /// </summary>
        /// <returns>
        /// "True" if SpPr is not set directly or is empty or Tx is not set directly or TX is string reference, otherwise - "false"
        /// </returns>
        internal bool IsParentRunPrUsed
        {
            get
            {
                return !mLabelPr.IsPropertySpecified(DmlChartDataLabelAttrs.SpPr)
                       || SpPr.IsEmpty || !mLabelPr.IsPropertySpecified(DmlChartDataLabelAttrs.Tx) || (Tx.StrRef != null);
            }
        }

        private bool IsTxPrSpecified
        {
            get
            {
                return (mLabelPr.GetProperty(DmlChartDataLabelAttrs.TxPr) != null) && IsParentRunPrUsed;
            }
        }

        /// <summary>
        /// Specifies text formatting.
        /// <see cref="DmlChartTxPr"/>.
        /// </summary>
        internal DmlChartTxPr TxPr
        {
            get
            {
                // WORDSNET-16609 Strange behavior of MS Word. If SpPr is not set directly or is empty,
                // or Tx is not set directly, the TxPr must be set by default (must not be inherited).
                if (mTxPr == null)
                {
                    mTxPr = IsTxPrSpecified
                        ? (DmlChartTxPr)mLabelPr.GetProperty(DmlChartDataLabelAttrs.TxPr)
                        : new DmlChartTxPr();
                }

                return mTxPr;
            }
        }

        internal DmlChartSpPr LeaderLines
        {
            get
            {
                if (mLabelPr.GetProperty(DmlChartDataLabelAttrs.LeaderLines) == null)
                    mLabelPr.SetProperty(DmlChartDataLabelAttrs.LeaderLines, new DmlChartSpPr());

                return (DmlChartSpPr)mLabelPr.GetProperty(DmlChartDataLabelAttrs.LeaderLines);
            }
        }

        internal DmlChartDataLabelPr LabelPr
        {
            get { return mLabelPr; }
        }

        /// <summary>
        /// Specifies number formatting for the parent element.
        /// </summary>
        internal DmlChartNumFormat NumFmt
        {
            get { return (DmlChartNumFormat)mLabelPr.GetProperty(DmlChartDataLabelAttrs.NumFmt); }
            set { mLabelPr.SetProperty(DmlChartDataLabelAttrs.NumFmt, value); }
        }

        /// <summary>
        /// Returns a flag indicating whether the label has default formatting and it was saved to document markup by
        /// MS Word for its internal purposes only.
        /// </summary>
        /// <remarks>
        /// Represents a xForSave element of [MS-ODRAWXML] section 2.6.1.26 that specifies whether this data label was
        /// created solely for the purpose of saving. If it is <c>true</c>, it means that when the document is loaded,
        /// this data label can be removed from the parent collection as having default formatting.
        /// </remarks>
        internal bool XForSave
        {
            get
            {
                object xForSave = mLabelPr.GetExtensionProperty(DmlChartDataLabelAttrs.XForSave);
                return ((xForSave is bool) && (bool)xForSave);
            }
        }

        /// <summary>
        /// Gets the parent chart of this label.
        /// </summary>
        internal DmlChart Chart
        {
            get { return (mSeries != null) ? mSeries.Chart : null; }
        }

        /// <summary>
        /// The index of the series element that is linked with the current data label.
        /// </summary>
        /// <remarks>
        /// In normal cases, this series element index is equal to the data label index. But sometimes not (see WORDSNET-17879)
        /// </remarks>
        internal int SeriesElementIndex { get; set; }

        /// <summary>
        /// Returns a flag indicating whether some properties of this data label have different values than the corresponding
        /// properties of the parent collection.
        /// </summary>
        internal bool HasNonDefaultFormatting
        {
            get { return mLabelPr.HasNonDefaultFormatting; }
        }

        /// <summary>
        /// Indicates that <see cref="DmlChartNumFormat"/> is not null and is not linked to the source.
        /// </summary>
        private bool IsNumFmtUsed
        {
            get
            {
                return (NumFmt != null) && !NumFmt.SourceLinked;
            }
        }

        /// <summary>
        /// Gets a <see cref="DmlTextBodyProperties"/> instance to get text properties of the label.
        /// </summary>
        private DmlTextBodyProperties BodyPr
        {
            get
            {
                if ((Tx != null) && (Tx.RichText != null))
                    return Tx.RichText.Properties;

                return TxPr.BodyPr;
            }
        }

        /// <summary>
        /// Gets a <see cref="DmlTextBodyProperties"/> instance to set text properties of the label.
        /// </summary>
        private DmlTextBodyProperties BodyPrForUpdate
        {
            get
            {
                // Tx cannot be inherited from collection: return as is.
                if ((Tx != null) && (Tx.RichText != null))
                    return Tx.RichText.Properties;

                // Materialize TxPr if it is taken from the parent collection.
                if (mLabelPr.GetDirectProperty(DmlChartDataLabelAttrs.TxPr) == null)
                {
                    mTxPr = TxPr.Clone();
                    mTxPr.EnsureBodyPrExists();
                    mLabelPr.SetProperty(DmlChartDataLabelAttrs.TxPr, mTxPr);
                }

                return TxPr.BodyPr;
            }
        }

        /// <summary>
        /// Gets the value of the <see cref="Layout"/> property of the data label. Creates and sets the property if
        /// it is <b>null</b>.
        /// </summary>
        private DmlChartManualLayout LayoutSafe
        {
            get
            {
                if (Layout == null)
                {
                    mLayout = new DmlChartManualLayout();
                    mLabelPr.SetProperty(DmlChartDataLabelAttrs.Layout, mLayout);
                }

                return mLayout;
            }
        }

        /// <summary>
        /// Gets the width of the parent chart in points.
        /// </summary>
        private double ShapeWidth
        {
            get { return mSeries.Chart.ChartSpace.Dml.Width; }
        }

        /// <summary>
        /// Gets the height of the parent chart in points.
        /// </summary>
        private double ShapeHeight
        {
            get { return mSeries.Chart.ChartSpace.Dml.Height; }
        }

        /// <summary>
        /// Gets a value indicating whether this data label belongs to a MS Word 2016 chart.
        /// </summary>
        private bool IsChartEx
        {
            get { return mSeries.Chart.ChartSpace.IsChartEx; }
        }

        #region INumberFormatProvider members

        DmlChartNumFormat INumberFormatProvider.NumFmt_INumberFormatProvider
        {
            get { return NumFmt; }
            set { NumFmt = value; }
        }

        bool INumberFormatProvider.IsInherited
        {
            get { return mLabelPr.GetDirectProperty(DmlChartDataLabelAttrs.NumFmt) == null; }
        }

        #endregion

        #region IChartItemTextProperties members

        string IChartItemTextProperties.GenerateItemText()
        {
            // TODO: Implement a label index to value index translator so that this method can be used for Word 2016 charts.
            if (Chart.ChartType == DmlChartType.ChartExChart)
                return null;

            SeriesElementIndex = Index;
            return GenerateDataLabelText(mSeries);
        }

        object IChartItemTextProperties.FetchSpecialDefaultRunPropertyValue(int key)
        {
            return null; // No special default values.
        }

        object IChartItemTextProperties.GetRelativePropertyValue(int key, object value)
        {
            return value;
        }

        DmlChartTx IChartItemTextProperties.ItemTx
        {
            get { return (DmlChartTx)mLabelPr.GetDirectProperty(DmlChartDataLabelAttrs.Tx); }
        }

        DmlChartTxPr IChartItemTextProperties.ItemTxPr
        {
            get { return (DmlChartTxPr)mLabelPr.GetDirectProperty(DmlChartDataLabelAttrs.TxPr); }
            set
            {
                mTxPr = value;
                mLabelPr.SetProperty(DmlChartDataLabelAttrs.TxPr, value);
            }
        }

        DmlChartSpPr IChartItemTextProperties.ItemSpPr
        {
            get { return (DmlChartSpPr)mLabelPr.GetDirectProperty(DmlChartDataLabelAttrs.SpPr); }
        }

        DmlChartTxPr IChartItemTextProperties.CollectionTxPr
        {
            get { return (mSeries != null) ? mSeries.DataLabels.TxPr : null; }
        }

        #endregion

        private DmlChartManualLayout mLayout;
        private DmlChartDataLabelPr mLabelPr;
        private DmlChartTx mTx;
        private DmlChartTxPr mTxPr;
        private DmlChartSpPr mSpPr;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private ChartSeries mSeries;
        private ChartNumberFormat mNumberFormat;
        private Font mFont;
        private ChartFormat mFormat;
    }
}
