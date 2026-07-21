// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/06/2012 by Alexey Noskov

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Themes;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents a collection of <see cref="ChartDataLabel"/>.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    public class ChartDataLabelCollection : IEnumerable<ChartDataLabel>, INumberFormatProvider, IChartFormatSource
    {
        // Represents 5.7.2.49 dLbls (Data Labels) element
        // that specifies the settings for the data labels for an entire series or the entire chart.

        internal ChartDataLabelCollection(DmlChart chart, ChartSeries series)
        {
            mChart = chart;
            mSeries = series;
            mLabelPr = new DmlChartDataLabelPr(chart);
        }

        internal ChartDataLabelCollection Clone()
        {
            ChartDataLabelCollection lhs = new ChartDataLabelCollection(mChart, mSeries);

            if (LabelPr != null)
                lhs.LabelPr = LabelPr.Clone();

            foreach (ChartDataLabel label in MaterializedDataLabels)
            {
                if (label.HasNonDefaultFormatting)
                {
                    ChartDataLabel cloned = label.Clone();
                    lhs.AddLabel(cloned);
                }
            }

            return lhs;
        }

        /// <summary>
        /// Sets the parent chart of this collection.
        /// </summary>
        internal void SetChart(DmlChart chart, ChartSeries series)
        {
            mChart = chart;
            mSeries = series;

            foreach (ChartDataLabel label in MaterializedDataLabels)
                label.SetSeries(series);
        }

        /// <summary>
        /// Returns <see cref="ChartDataLabel"/> for the specified index.
        /// </summary>
        public ChartDataLabel this[int index]
        {
            get { return MaterializeItem(index); }
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<ChartDataLabel> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Clears format of all <see cref="ChartDataLabel"/> in this collection.
        /// </summary>
        public void ClearFormat()
        {
            // Let's keep currently stored label objects.
            foreach (ChartDataLabel label in MaterializedDataLabels)
                label.ClearFormat();
        }

        internal void AddLabel(ChartDataLabel label)
        {
            mDataLabels.Add(label.Index, label);
        }

        /// <summary>
        /// Inserts a data label with default formatting at the specified index. The indexes of the labels following
        /// the inserted one are incremented by 1.
        /// </summary>
        internal void Insert(int index)
        {
            if (mDataLabels.Count == 0)
                return;

            SortedList<int, ChartDataLabel> dataLabels = new SortedList<int, ChartDataLabel>(mDataLabels.Count);

            foreach (ChartDataLabel label in mDataLabels.Values)
            {
                if (label.Index >= index)
                    label.Index += 1;

                dataLabels.Add(label.Index, label);
            }

            mDataLabels = dataLabels;
        }

        /// <summary>
        /// Removes the data label at the specified index. The indexes of the labels following the removed one are
        /// decremented by 1.
        /// </summary>
        internal void Remove(int index)
        {
            if (mDataLabels.Count == 0)
                return;

            SortedList<int, ChartDataLabel> dataLabels = new SortedList<int, ChartDataLabel>(mDataLabels.Count);

            foreach (ChartDataLabel label in mDataLabels.Values)
            {
                if (label.Index == index)
                    continue;

                if (label.Index > index)
                    label.Index -= 1;

                dataLabels.Add(label.Index, label);
            }

            mDataLabels = dataLabels;
        }

        /// <summary>
        /// Returns <c>true</c> if the label at the specified index has non-default formatting.
        /// </summary>
        internal bool HasNonDefaultItemFormatting(int index)
        {
            ChartDataLabel label = mDataLabels.GetValueOrNull(index);
            return (label != null) && label.HasNonDefaultFormatting;
        }

        /// <summary>
        /// Gets data labels with necessary formatting to write in form required by MS Word.
        /// </summary>
        /// <remarks>
        /// When displaying a data label with non-default formatting, MS Word does not take inherited values of ShowXXX
        /// and DLblPos properties from a data label collection. So, these properties should be always written to data
        /// label markup.
        /// And when the property <see cref="ShowDataLabelsRange"/> is set, MS Word writes all data labels with ShowXXX
        /// and Tx properties included into output XML file even if the labels have default formatting only.
        /// </remarks>
        internal List<ChartDataLabel> GetLabelsToWrite()
        {
            List<ChartDataLabel> labels = new List<ChartDataLabel>();

            bool needAllLabelsExplicitly = ShowDataLabelsRange;

            // Default values of ShowXXX properties depend on document version, so, they may be different from AW default
            // values: need to write the properties. And MS Word does the same.
            LabelPr.ExpandParentProperties(gPropertiesWithVersionDependentDefaults);

            if (needAllLabelsExplicitly)
            {
                // Mimic MS Word in generating output XML.

                foreach (ChartDataLabel label in this)
                {
                    ChartDataLabel clonedLabel = label.Clone();

                    if (clonedLabel.ShowDataLabelsRange)
                    {
                        // Make the ShowDataLabelsRange property to be explicitly defined.
                        clonedLabel.ShowDataLabelsRange = true;

                        if (mSeries.HasDataLabelsRangeData)
                            GenerateTx(clonedLabel);
                    }

                    ExpandNonInheritedParentProperties(clonedLabel.LabelPr);

                    if (!label.HasNonDefaultFormatting)
                        clonedLabel.LabelPr.SetExtensionProperty(DmlChartDataLabelAttrs.XForSave, true);

                    labels.Add(clonedLabel);
                }
            }
            else
            {
                foreach (ChartDataLabel label in MaterializedDataLabels)
                {
                    if (!label.HasNonDefaultFormatting)
                        continue;

                    // Mimic MS Word in generating output XML.

                    ChartDataLabel clonedLabel = label.Clone();

                    if (mSeries.HasDataLabelsRangeData)
                        GenerateTx(clonedLabel);

                    ExpandNonInheritedParentProperties(clonedLabel.LabelPr);

                    labels.Add(clonedLabel);
                }
            }

            return labels;
        }

        /// <summary>
        /// Expand data label properties that do not inherit their value from the parent collection.
        /// </summary>
        /// <param name="labelPr">Properties of the individual data label.</param>
        private void ExpandNonInheritedParentProperties(DmlChartDataLabelPr labelPr)
        {
            // These properties are always not inherited.
            labelPr.ExpandParentProperties(gNonInheritedProperties);

            DmlChartNumFormat numFmt = (DmlChartNumFormat)labelPr.GetDirectProperty(DmlChartDataLabelAttrs.NumFmt);
            object txPr = labelPr.GetDirectProperty(DmlChartDataLabelAttrs.TxPr);
            object spPr = labelPr.GetDirectProperty(DmlChartDataLabelAttrs.SpPr);
            DmlChartShapeProperties shapePr =
                (DmlChartShapeProperties)labelPr.GetExtensionProperty(DmlChartDataLabelAttrs.ShapePr);

            // If any of these properties are defined: txPr, spPr, numFmt, they are not taken from the parent collection,
            // need to expand missing ones.
            // If ShapePr is defined but all the properties mentioned above are null, MS Word ignores it: need to
            // expand the properties.
            if ((numFmt != null) || (txPr != null) || (spPr != null) || (shapePr != null))
            {
                labelPr.ExpandParentProperties(new DmlChartDataLabelAttrs[] { DmlChartDataLabelAttrs.NumFmt,
                    DmlChartDataLabelAttrs.TxPr, DmlChartDataLabelAttrs.SpPr });
                labelPr.ExpandParentExtensionProperties(new DmlChartDataLabelAttrs[] { DmlChartDataLabelAttrs.ShapePr });

                if (numFmt != null)
                {
                    bool hasSpPrOrTxPr = (txPr != null) || (spPr != null);

                    // If a data label has either spPr or txPr element, and value is not defined, global default
                    // (source-linked) is used: no need to write this value.
                    // If a label has neither spPr nor txPr, it inherits number format from the parent collection:
                    // can skip writing at this case too.
                    if ((hasSpPrOrTxPr && numFmt.SourceLinked) ||
                        (!hasSpPrOrTxPr && numFmt.Equals(NumFmt)))
                    {
                        labelPr.RemoveProperty(DmlChartDataLabelAttrs.NumFmt);
                    }
                }
            }
        }

        /// <summary>
        /// Generates default value for the <see cref="DmlChartDataLabelAttrs.Tx"/> property, if it is not defined yet,
        /// to display information specified by the ShowXXX properties. MS Word needs this property written when the
        /// <see cref="ShowDataLabelsRange"/> property is set to On.
        /// </summary>
        private void GenerateTx(ChartDataLabel label)
        {
            DmlChartValueType pointType = mSeries.X.ValueType;
            bool isCategory = (pointType == DmlChartValueType.String) || (pointType == DmlChartValueType.MultiLvlString);
            label.LabelPr.GenerateTx(isCategory);
        }

        /// <summary>
        /// Calculates real values of <see cref="NumberFormat"/> and ShowXXX properties of child labels. Collapses
        /// values of the properties if they have the same values as this parent collection. Removes labels that have
        /// default formatting.
        /// </summary>
        /// <remarks>
        /// The method is executed after loading the chart from the document file.
        /// Real values of the <see cref="NumberFormat"/> and ShowXXX properties of labels depend on the other properties
        /// and document version. This method calculates them.
        /// And we need to normalize label objects after loading, otherwise setting properties of the entire collection
        /// will not have effect for such non-normalized labels.
        /// </remarks>
        internal void NormalizeAfterLoad()
        {
            // When ShowDataLabelRange property is set, all attributes from gNonInheritedProperties are explicitly
            // written, we need collapse them.
            // Also XForSave attribute indicates that a label has default formatting and its properties were explicitly
            // filled for writing purposes only: need to clear format at this case.

            foreach (ChartDataLabel label in MaterializedDataLabels)
            {
                // When xForSave flag is set, it means that a label has default formatting, and it was saved to document
                // markup by MS Word for internal purposes only.
                if (label.XForSave)
                {
                    label.ClearFormat();
                    continue;
                }

                FixPropertyInheritance(label.LabelPr);

                CollapsePropertiesExpandedForSaving(label);
            }
        }

        /// <summary>
        /// Returns <b>true</b> if the specified data label is contained in this collection.
        /// </summary>
        internal bool Contains(ChartDataLabel label)
        {
            return mDataLabels.ContainsValue(label);
        }

        /// <summary>
        /// If any of the following attributes are defined in a data label: <see cref="DmlChartDataLabelAttrs.TxPr"/>,
        /// <see cref="DmlChartDataLabelAttrs.SpPr"/>, <see cref="DmlChartDataLabelAttrs.NumFmt"/>, and some of them is
        /// missing, it is not taken from the parent data label collection, but the default value is used. This method
        /// sets the missing properties to the default values.
        /// </summary>
        private void FixPropertyInheritance(DmlChartDataLabelPr labelPr)
        {
            object labelTxPr = labelPr.GetDirectProperty(DmlChartDataLabelAttrs.TxPr);
            object labelSpPr = labelPr.GetDirectProperty(DmlChartDataLabelAttrs.SpPr);
            object labelNumFmt = labelPr.GetDirectProperty(DmlChartDataLabelAttrs.NumFmt);

            // Return if none of the properties are defined.
            if ((labelTxPr == null) && (labelSpPr == null) && (labelNumFmt == null))
            {
                // MS Word ignores ShapePr if no of these elements are defined.
                labelPr.RemoveExtensionProperty(DmlChartDataLabelAttrs.ShapePr);
                return;
            }

            // Set the default values to the missing properties.

            if ((labelTxPr == null) && (LabelPr.GetDirectProperty(DmlChartDataLabelAttrs.TxPr) != null))
                labelPr.SetProperty(DmlChartDataLabelAttrs.TxPr, new DmlChartTxPr());

            if ((labelSpPr == null) && (LabelPr.GetDirectProperty(DmlChartDataLabelAttrs.SpPr) != null))
                labelPr.SetProperty(DmlChartDataLabelAttrs.SpPr, new DmlChartSpPr());

            if ((labelNumFmt == null) && (LabelPr.GetDirectProperty(DmlChartDataLabelAttrs.NumFmt) != null))
            {
                DmlChartNumFormat numberFormat = new DmlChartNumFormat();
                numberFormat.SourceLinked = true;
                labelPr.SetProperty(DmlChartDataLabelAttrs.NumFmt, numberFormat);
            }

            // The similar is with ShapePr, but in difference, it does not affect the other properties.
            if ((((IChartFormatSource)this).ShapeType != ChartShapeType.Default) &&
                (labelPr.GetExtensionProperty(DmlChartDataLabelAttrs.ShapePr) == null))
            {
                labelPr.SetExtensionProperty(DmlChartDataLabelAttrs.ShapePr, new DmlChartShapeProperties());
            }
        }

        /// <summary>
        /// Collapses label properties that MS Word and AW expand for saving.
        /// </summary>
        private void CollapsePropertiesExpandedForSaving(ChartDataLabel label)
        {
            if (!label.HasNonDefaultFormatting)
                return;

            // When the ShowDataLabelsRange property is set in a label or in a collection, MS Word writes
            // label(s) with ShowXXX and Tx properties included in output XML file even if the properties
            // have default values only.
            bool needCollapse = ShowDataLabelsRange || label.ShowDataLabelsRange;
            if (!needCollapse)
                return;

            label.LabelPr.CollapseParentProperties(gNonInheritedProperties);
            if (label.ShowDataLabelsRange == ShowDataLabelsRange)
                label.LabelPr.RemoveExtensionProperty(DmlChartDataLabelAttrs.ShowDataLabelsRange);
        }

        /// <summary>
        /// If the collection does not contain a data label at the specified index yet, this method creates it and puts
        /// into the collection.
        /// </summary>
        private ChartDataLabel MaterializeItem(int index)
        {
            // The index is not compared with Count now since we cannot calculate Count exactly for some chart types
            // of MS Word 2016.
            if ((index < 0) ||
                ((mSeries != null) && !mSeries.HasDataLabels))
                throw new ArgumentOutOfRangeException("index");

            ChartDataLabel label = mDataLabels.GetValueOrNull(index);
            if (label == null)
            {
                label = new ChartDataLabel(mLabelPr, mSeries);
                label.Index = index;

                AddLabel(label);
            }

            return label;
        }

        /// <summary>
        /// Counts labels that have non-default formatting starting with the specified label index.
        /// </summary>
        private int CountLabelsFrom(int labelIndex)
        {
            int count = 0;

            for (int i = mDataLabels.Count - 1; i >= 0; i--)
            {
                if (mDataLabels.Keys[i] < labelIndex)
                    break;

                if (mDataLabels.Values[i].HasNonDefaultFormatting)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Sets the data label defaults property. If data labels are not visible yet, they are displayed.
        /// </summary>
        private void SetProperty(DmlChartDataLabelAttrs attr, object value)
        {
            mLabelPr.SetProperty(attr, value);

            if (mSeries != null)
                mSeries.SetHasDataLabels(true);
        }

        #region IChartFormatSource implementation

        void IChartFormatSource.MaterializeSpPr()
        {
            // Do nothing.
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

                if (value == ChartShapeType.Default)
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

                if (shapePr.Geometry == null)
                    shapePr.Geometry = new DmlGeometry();

                shapePr.Geometry.PresetName = ChartShapeTypeUtil.ChartShapeTypeToDmlPresetGeom(value);
            }
        }

        IThemeProvider IChartFormatSource.ThemeProvider
        {
            get { return mChart.Document.GetThemeInternal(); }
        }

        bool IChartFormatSource.IsFormatDefined
        {
            get { return mLabelPr.IsPropertySpecified(DmlChartDataLabelAttrs.SpPr) && !SpPr.IsEmpty; }
        }

        #endregion

        /// <summary>
        /// Returns the number of <see cref="ChartDataLabel"/> in this collection.
        /// </summary>
        public int Count
        {
            get
            {
                ChartSeriesHelper helper = new ChartSeriesHelper(mSeries);
                ChartSeriesHelper.ItemCountInfo countInfo = helper.GetDataLabelCount();

                return
                    countInfo.Count +
                    // Count labels with indexes larger than the last label index possible to display if such labels
                    // have non-default formatting.
                    CountLabelsFrom(countInfo.LastUsedIndex + 1);
            }
        }

        /// <summary>
        /// Allows to specify whether category name is to be displayed for the data labels of the entire series.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Value defined for this property can be overridden for an individual data label with using the
        /// <see cref="ChartDataLabel.ShowCategoryName"/> property.
        /// </remarks>
        public bool ShowCategoryName
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowCatName); }
            set { SetProperty(DmlChartDataLabelAttrs.ShowCatName, value); }
        }

        /// <summary>
        /// Allows to specify whether bubble size is to be displayed for the data labels of the entire series.
        /// Applies only to Bubble charts.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Value defined for this property can be overridden for an individual data label with using the
        /// <see cref="ChartDataLabel.ShowBubbleSize"/> property.
        /// </remarks>
        public bool ShowBubbleSize
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowBubbleSize); }
            set
            {
                if (Chart.IsBubbleChart)
                    SetProperty(DmlChartDataLabelAttrs.ShowBubbleSize, value);
                else
                    mChart.Warn(WarningType.MinorFormattingLoss,
                        "ShowBubbleSize is not supported by this type of chart, value will not be set.");
            }
        }

        /// <summary>
        /// Allows to specify whether legend key is to be displayed for the data labels of the entire series.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Value defined for this property can be overridden for an individual data label with using the
        /// <see cref="ChartDataLabel.ShowLegendKey"/> property.
        /// </remarks>
        public bool ShowLegendKey
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowLegendKey); }
            set { SetProperty(DmlChartDataLabelAttrs.ShowLegendKey, value); }
        }

        /// <summary>
        /// Allows to specify whether percentage value is to be displayed for the data labels of the entire series.
        /// Default value is <c>false</c>. Applies only to Pie charts.
        /// </summary>
        /// <remarks>
        /// Value defined for this property can be overridden for an individual data label with using the
        /// <see cref="ChartDataLabel.ShowPercentage"/> property.
        /// </remarks>
        public bool ShowPercentage
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowPercent); }
            set { SetProperty(DmlChartDataLabelAttrs.ShowPercent, value); }
        }

        /// <summary>
        /// Returns or sets a Boolean to indicate the series name display behavior for the data labels of the entire series.
        /// <c>true</c> to show the series name; <c>false</c> to hide. By default <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Value defined for this property can be overridden for an individual data label with using the
        /// <see cref="ChartDataLabel.ShowSeriesName"/> property.
        /// </remarks>
        public bool ShowSeriesName
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowSerName); }
            set { SetProperty(DmlChartDataLabelAttrs.ShowSerName, value); }
        }

        /// <summary>
        /// Allows to specify whether values are to be displayed in the data labels of the entire series.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Value defined for this property can be overridden for an individual data label with using the
        /// <see cref="ChartDataLabel.ShowValue"/> property.
        /// </remarks>
        public bool ShowValue
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowVal); }
            set { SetProperty(DmlChartDataLabelAttrs.ShowVal, value); }
        }

        /// <summary>
        /// Allows to specify whether data label leader lines need be shown for the data labels of the entire series.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>Applies to Pie charts only.
        /// Leader lines create a visual connection between a data label and its corresponding data point.</para>
        /// <para>Value defined for this property can be overridden for an individual data label with using the
        /// <see cref="ChartDataLabel.ShowLeaderLines"/> property.</para>
        /// </remarks>
        public bool ShowLeaderLines
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowLeaderLines); }
            set { SetProperty(DmlChartDataLabelAttrs.ShowLeaderLines, value); }
        }

        /// <summary>
        /// Allows to specify whether values from data labels range to be displayed in the data labels of the entire series.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Value defined for this property can be overridden for an individual data label with using the
        /// <see cref="ChartDataLabel.ShowDataLabelsRange"/> property.
        /// </remarks>
        public bool ShowDataLabelsRange
        {
            get { return (bool)mLabelPr.GetProperty(DmlChartDataLabelAttrs.ShowDataLabelsRange); }
            set { mLabelPr.SetExtensionProperty(DmlChartDataLabelAttrs.ShowDataLabelsRange, value); }
        }

        /// <summary>
        /// Gets or sets string separator used for the data labels of the entire series.
        /// The default is a comma, except for pie charts showing only category name and percentage, when a line break
        /// shall be used instead.
        /// </summary>
        /// <remarks>
        /// Value defined for this property can be overridden for an individual data label with using the
        /// <see cref="ChartDataLabel.Separator"/> property.
        /// </remarks>
        public string Separator
        {
            get { return mLabelPr.GetSeparator(Chart.IsPieChart); }
            set { SetProperty(DmlChartDataLabelAttrs.Separator, value); }
        }

        /// <summary>
        /// Gets or sets the text orientation of the data labels of the entire series.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="ShapeTextOrientation.Horizontal"/>.
        /// </remarks>
        public ShapeTextOrientation Orientation
        {
            get { return TxPr.BodyPr.TextOrientation; }
            set
            {
                TxPr.EnsureBodyPrExists();
                TxPr.BodyPr.TextOrientation = value;
            }
        }

        /// <summary>
        /// Gets or sets the rotation of the data labels of the entire series in degrees.
        /// </summary>
        /// <remarks>
        /// <para>The range of acceptable values is from -180 to 180 inclusive. The default value is 0.</para>
        /// <para>If the <see cref="Orientation"/> value is <see cref="ShapeTextOrientation.Horizontal"/>, label shapes,
        /// if they exist, are rotated along with the label text. Otherwise, only the label text is rotated.</para>
        /// </remarks>
        public int Rotation
        {
            get { return (int)System.Math.Round(TxPr.BodyPr.Rotation.ValueInDegrees); }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, -180, 180, "value");
                TxPr.EnsureBodyPrExists();
                TxPr.BodyPr.Rotation = DmlAngle.FromDegrees(value);
            }
        }

        /// <summary>
        /// Gets an <see cref="ChartNumberFormat"/> instance allowing to set number format for the data labels of the
        /// entire series.
        /// </summary>
        public ChartNumberFormat NumberFormat
        {
            get
            {
                if (mNumberFormat == null)
                    mNumberFormat = new ChartNumberFormat(this, mChart);

                return mNumberFormat;
            }
        }

        /// <summary>
        /// Provides access to the font formatting of the data labels of the entire series.
        /// </summary>
        /// <remarks>
        /// Value defined for this property can be overridden for an individual data label with using the
        /// <see cref="ChartDataLabel.Font"/> property.
        /// </remarks>
        public Font Font
        {
            get
            {
                if (mFont == null)
                {
                    IRunAttrSource attrSource = new ChartCollectionDmlRunPropertiesSource(TxPr, Chart.ChartSpace);
                    mFont = Font.MakeFont(attrSource, Chart.Document);
                }

                return mFont;
            }
        }

        /// <summary>
        /// Provides access to fill and line formatting of the data labels.
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
        /// Gets or sets the position of the data labels.
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
                        "This series type doesn't support setting the position of data labels.");
                }

                mLabelPr.SetProperty(DmlChartDataLabelAttrs.DLblPos, value);
            }
        }

        internal DmlChartDataLabelPr LabelPr
        {
            get { return mLabelPr; }
            set { mLabelPr = value; }
        }

        /// <summary>
        /// Gets the parent chart of this data label collection.
        /// </summary>
        internal DmlChart Chart
        {
            get { return mChart; }
        }

        /// <summary>
        /// Specifies number formatting for the parent element.
        /// </summary>
        internal DmlChartNumFormat NumFmt
        {
            get { return (DmlChartNumFormat)mLabelPr.GetProperty(DmlChartDataLabelAttrs.NumFmt); }
            set { SetProperty(DmlChartDataLabelAttrs.NumFmt, value); }
        }

        /// <summary>
        /// Returns <c>true</c> if this collection contains at least one explicitly-defined label.
        /// </summary>
        internal bool HasItems
        {
            get { return mDataLabels.Count > 0; }
        }

        /// <summary>
        /// Returns <c>true</c> if any properties of this collection have non-default values.
        /// </summary>
        internal bool HasNonDefaultFormatting
        {
            // If Series or Chart has tag "dLbls" the labels properties should be written.
            get
            {
                return mLabelPr.HasNonDefaultFormatting || (mSeries != null && mSeries.HasDataLabels) ||
                    (mSeries == null && mChart != null && mChart.ChartPr.IsPropertySpecified(DmlChartAttrs.DLbls));
            }
        }

        #region INumberFormatProvider members

        DmlChartNumFormat INumberFormatProvider.NumFmt_INumberFormatProvider
        {
            get { return NumFmt; }
            set { NumFmt = value; }
        }

        bool INumberFormatProvider.IsInherited
        {
            get { return false; }
        }

        #endregion

        /// <summary>
        /// Gets an enumerable over all materialized data labels.
        /// </summary>
        internal IEnumerable<ChartDataLabel> MaterializedDataLabels
        {
            get { return mDataLabels.Values; }
        }

        /// <summary>
        /// Specifies text formatting.
        /// </summary>
        internal DmlChartTxPr TxPr
        {
            get
            {
                DmlChartTxPr txPr = (DmlChartTxPr)mLabelPr.GetProperty(DmlChartDataLabelAttrs.TxPr);
                if (txPr == null)
                {
                    txPr = new DmlChartTxPr();
                    mLabelPr.SetProperty(DmlChartDataLabelAttrs.TxPr, txPr);
                }

                return txPr;
            }
        }

        /// <summary>
        /// Specifies the formatting for the data labels.
        /// </summary>
        internal DmlChartSpPr SpPr
        {
            get
            {
                if (mSpPr == null)
                {
                    mSpPr = (DmlChartSpPr)mLabelPr.GetProperty(DmlChartDataLabelAttrs.SpPr);
                    if (mSpPr == null)
                    {
                        mSpPr = new DmlChartSpPr();
                        mLabelPr.SetProperty(DmlChartDataLabelAttrs.SpPr, mSpPr);
                    }
                }

                return mSpPr;
            }
        }

        /// <summary>
        /// Gets the parent series of the data labels.
        /// </summary>
        internal ChartSeries Series
        {
            get { return mSeries; }
        }

        private DmlChartDataLabelPr mLabelPr;
        private SortedList<int, ChartDataLabel> mDataLabels = new SortedList<int, ChartDataLabel>();

        [CppWeakPtr]
        private DmlChart mChart;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private ChartSeries mSeries;
        private ChartNumberFormat mNumberFormat;
        private Font mFont;
        private ChartFormat mFormat;
        private DmlChartSpPr mSpPr;

        /// <summary>
        /// For these properties, when displaying a data label with non-default formatting, MS Word does not take
        /// inherited value from a data label collection. So, these properties should be always written to data label
        /// markup.
        /// </summary>
        private static readonly DmlChartDataLabelAttrs[] gNonInheritedProperties =
            { DmlChartDataLabelAttrs.ShowSerName, DmlChartDataLabelAttrs.ShowBubbleSize, DmlChartDataLabelAttrs.ShowCatName,
                DmlChartDataLabelAttrs.ShowLegendKey, DmlChartDataLabelAttrs.ShowPercent, DmlChartDataLabelAttrs.ShowVal,
                DmlChartDataLabelAttrs.DLblPos };

        private static readonly DmlChartDataLabelAttrs[] gPropertiesWithVersionDependentDefaults =
        { DmlChartDataLabelAttrs.ShowSerName, DmlChartDataLabelAttrs.ShowBubbleSize, DmlChartDataLabelAttrs.ShowCatName,
            DmlChartDataLabelAttrs.ShowLegendKey, DmlChartDataLabelAttrs.ShowPercent, DmlChartDataLabelAttrs.ShowVal };

        /// <summary>
        /// Enumerator over data labels that are included in a value returned by the <see cref="Count"/> property.
        /// </summary>
        /// <remarks>
        /// The following are included in the enumeration: 1) labels that are displayed by default by MS Word for the
        /// current series data and series options even if they have the same formatting as this collection has;
        /// hidden/deleted labels are included too; 2) labels that are not displayed with the current data, but which have
        /// non-default formatting.
        /// </remarks>
        private sealed class Enumerator : IEnumerator<ChartDataLabel>
        {
            internal Enumerator(ChartDataLabelCollection collection)
            {
                ChartSeriesHelper helper = new ChartSeriesHelper(collection.mSeries);
                mCollection = collection;
                mLabelIndexes = helper.GetLabelIndexesDeterminedByData();
                mLastUsedIndex = (mLabelIndexes != null)
                    ? mLabelIndexes[mLabelIndexes.Count - 1]
                    : helper.GetDataLabelCount().Count - 1;
            }

            public bool MoveNext()
            {
                if (mCollection.mSeries == null)
                    return false;

                if (!mCollection.mSeries.HasDataLabels)
                    return false;

                mCurrentIndex++;

                if (mLabelIndexes != null)
                {
                    // All labels with indexes in mLabelIndexes and labels with non-default formatting, which exists in
                    // the collection, are enumerated.
                    while ((mCurrentIndex <= mLastUsedIndex) &&
                           (mLabelIndexes.BinarySearch(mCurrentIndex) < 0) &&
                           !mCollection.HasNonDefaultItemFormatting(mCurrentIndex))
                    {
                        mCurrentIndex++;
                    }
                }

                if (mCurrentIndex <= mLastUsedIndex)
                    return true;

                SortedList<int, ChartDataLabel> dataLabels = mCollection.mDataLabels;

                // Check for labels after the last visible label. If the collection contains such invisible labels with
                // non-default formatting, include them into enumeration too.
                int maxExistingIndex = (dataLabels.Count > 0) ? dataLabels.Keys[dataLabels.Count - 1] : -1;
                while (mCurrentIndex <= maxExistingIndex)
                {
                    if (mCollection.HasNonDefaultItemFormatting(mCurrentIndex))
                        return true;

                    mCurrentIndex++;
                }

                return false;
            }

            public void Reset()
            {
                mCurrentIndex = -1;
            }

            public void Dispose()
            {
                // Nothing to dispose.
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public ChartDataLabel Current
            {
                get { return mCollection[mCurrentIndex]; }
            }

            private readonly ChartDataLabelCollection mCollection;
            private readonly IntList mLabelIndexes;
            private readonly int mLastUsedIndex;
            private int mCurrentIndex = -1;
        }
    }
}
