// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2023 by Alexander Zhiltsov

using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Text;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents properties of axis tick mark labels.
    /// </summary>
    public class AxisTickLabels : IChartItemTextProperties
    {
        internal AxisTickLabels(ChartAxis parentAxis)
        {
            mAxis = parentAxis;
        }

        /// <summary>
        /// Gets or sets the position of the tick labels on the axis.
        /// </summary>
        /// <remarks>
        /// The property is not supported by MS Office 2016 new charts.
        /// </remarks>
        public AxisTickLabelPosition Position
        {
            get { return (AxisTickLabelPosition)ChartAxisPr.GetProperty(DmlChartAxisAttrs.TickLblPos); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.TickLblPos, value); }
        }

        /// <summary>
        /// Gets or sets the distance of the tick labels from the axis.
        /// </summary>
        /// <remarks>
        /// <para>The property represents a percentage of the default label offset.</para>
        /// <para>Valid range is from 0 to 1000 percent inclusive. The default value is 100%.</para>
        /// <para>The property has effect only for category axes. It is not supported by MS Office 2016 new charts.</para>
        /// </remarks>
        public int Offset
        {
            get { return (int)ChartAxisPr.GetProperty(DmlChartAxisAttrs.TickLabelOffset); }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, 0, 1000, "value");
                ChartAxisPr.SetProperty(DmlChartAxisAttrs.TickLabelOffset, value);
            }
        }

        /// <summary>
        /// Gets or sets the interval at which the tick labels are drawn.
        /// </summary>
        /// <remarks>
        /// <para>The property has effect for text category and series axes. It is not supported by MS Office 2016
        /// new charts. Valid range of a value is greater than or equal to 1.</para>
        /// <para>Setting this property sets the <see cref="IsAutoSpacing"/> property to <c>false</c>.</para>
        /// </remarks>
        /// <dev>
        /// The ISO standard states that this element specifies how many tick labels to skip between label that is drawn.
        /// In MS Office, this element specifies the interval at which tick labels are drawn.
        /// </dev>
        public int Spacing
        {
            get { return (int)ChartAxisPr.GetProperty(DmlChartAxisAttrs.TickLabelSpacing); }
            set
            {
                ArgumentUtil.CheckPositive(value, "value");
                ChartAxisPr.SetProperty(DmlChartAxisAttrs.TickLabelSpacing, value);
                IsAutoSpacing = false;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether to use automatic interval for drawing the tick labels.
        /// </summary>
        /// <remarks>
        /// <para>The default value is <c>true</c>.</para>
        /// <para>The property has effect for text category and series axes. It is not supported by MS Office 2016
        /// new charts.</para>
        /// </remarks>
        public bool IsAutoSpacing
        {
            get { return (bool)ChartAxisPr.GetProperty(DmlChartAxisAttrs.TickLabelSpacingIsAuto); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.TickLabelSpacingIsAuto, value); }
        }

        /// <summary>
        /// Gets or sets text alignment of the axis tick labels.
        /// </summary>
        /// <remarks>
        /// <para>This property has effect only for multi-line labels.</para>
        /// <para>The default value is <see cref="ParagraphAlignment.Center"/>.</para>.
        /// </remarks>
        public ParagraphAlignment Alignment
        {
            get { return TxPr.FirstParagraph.Properties.Alignment; }
            set { TxPr.FirstParagraph.Properties.Alignment = value; }
        }

        /// <summary>
        /// Gets or sets the orientation of the tick label text.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ShapeTextOrientation.Horizontal"/>.</p>
        /// <p>Note that some <see cref="ShapeTextOrientation"/> values do not affect the orientation of tick label text
        /// in value axes.</p>
        /// </remarks>
        public ShapeTextOrientation Orientation
        {
            get { return BodyPr.TextOrientation; }
            set { BodyPrForUpdate.TextOrientation = value; }
        }

        /// <summary>
        /// Gets or sets the rotation of the tick labels in degrees.
        /// </summary>
        /// <remarks>
        /// The range of acceptable values is from -180 to 180 inclusive. The default value is 0.
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
        /// Provides access to font formatting of the tick labels.
        /// </summary>
        public Font Font
        {
            get
            {
                if (mFont == null)
                    mFont = new Font(FontRunPropertiesSource, mAxis.Document);

                return mFont;
            }
        }

        /// <summary>
        /// Gets a <see cref="ChartItemDmlRunPropertiesSource"/> instance that is the source of the tick labels
        /// font properties.
        /// </summary>
        private ChartItemDmlRunPropertiesSource FontRunPropertiesSource
        {
            get
            {
                if (mFontRunPropertiesSource == null)
                    mFontRunPropertiesSource = new ChartItemDmlRunPropertiesSource(this, mAxis.Chart.ChartSpace);

                return mFontRunPropertiesSource;
            }
        }

        /// <summary>
        /// Specifies text formatting.
        /// </summary>
        private DmlChartTxPr TxPr
        {
            get { return (DmlChartTxPr)ChartAxisPr.GetProperty(DmlChartAxisAttrs.TxPr); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.TxPr, value); }
        }

        /// <summary>
        /// Gets a <see cref="DmlTextBodyProperties"/> instance to get text properties of the tick labels.
        /// </summary>
        private DmlTextBodyProperties BodyPr
        {
            get { return TxPr.BodyPr; }
        }

        /// <summary>
        /// Gets a <see cref="DmlTextBodyProperties"/> instance to set text properties of the tick labels.
        /// </summary>
        private DmlTextBodyProperties BodyPrForUpdate
        {
            get
            {
                TxPr.EnsureBodyPrExists();
                return TxPr.BodyPr;
            }
        }

        /// <summary>
        /// Specifies the formatting for the parent chart element.
        /// </summary>
        private DmlChartSpPr SpPr
        {
            get { return (DmlChartSpPr)ChartAxisPr.GetProperty(DmlChartAxisAttrs.SpPr); }
        }

        /// <summary>
        /// Gets the axis properties collection.
        /// </summary>
        private DmlChartAxisPr ChartAxisPr
        {
            get { return mAxis.ChartAxisPr; }
        }

        #region IChartItemTextProperties members

        string IChartItemTextProperties.GenerateItemText()
        {
            return null; // Not used for tick labels.
        }

        object IChartItemTextProperties.FetchSpecialDefaultRunPropertyValue(int key)
        {
            if (key == FontAttr.Size)
                return ConvertUtilCore.PointToHalfPoint(mAxis.GetDefaultFontSizeInternal());

            return null;
        }

        object IChartItemTextProperties.GetRelativePropertyValue(int key, object value)
        {
            return value;
        }

        DmlChartTx IChartItemTextProperties.ItemTx
        {
            get { return null; }
        }

        DmlChartTxPr IChartItemTextProperties.ItemTxPr
        {
            get { return TxPr; }
            set { TxPr = value; }
        }

        DmlChartSpPr IChartItemTextProperties.ItemSpPr
        {
            get { return SpPr; }
        }

        DmlChartTxPr IChartItemTextProperties.CollectionTxPr
        {
            get { return null; }
        }

        #endregion

        private readonly ChartAxis mAxis;
        private Font mFont;
        private ChartItemDmlRunPropertiesSource mFontRunPropertiesSource;
    }
}
