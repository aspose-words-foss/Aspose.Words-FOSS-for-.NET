// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2012 by Alexey Noskov

using System;
using System.Drawing;
using Aspose.Images;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents a chart data marker.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    /// <dev>
    /// Represents 5.7.2.106 marker (Marker) element.
    /// </dev>
    public class ChartMarker : IChartFormatSource
    {
        internal ChartMarker(DmlChart chart)
            : this(chart.Document.GetThemeInternal())
        {
            mChart = chart;
        }

        internal ChartMarker(IThemeProvider themeProvider)
        {
            mThemeProvider = themeProvider;
        }

        internal ChartMarker Clone()
        {
            ChartMarker lhs = (ChartMarker)MemberwiseClone();
            lhs.mMarkerPr = mMarkerPr.Clone();
            lhs.mFormat = null;
            return lhs;
        }

        /// <summary>
        /// Sets the marker symbol without applying the chart style.
        /// </summary>
        internal void SetSymbolInternal(MarkerSymbol value)
        {
            mMarkerPr.SetProperty(DmlChartMarkerAttr.Symbol, value);
        }

        #region IChartFormatSource implementation

        void IChartFormatSource.MaterializeSpPr()
        {
            // This un-links marker SpPr from SpPr of series. Do it only on changing properties, not on reading, for
            // the behavior to not depend on a fact whether a customer has read ChartFormat properties or hasn't.

            if (mMarkerPr.IsPropertySpecified(DmlChartMarkerAttr.SpPr))
                return;

            DmlChartSpPr parentSpPr = (DmlChartSpPr)mMarkerPr.GetProperty(DmlChartMarkerAttr.SpPr);
            DmlChartSpPr spPr = (parentSpPr != null) ? parentSpPr.Clone() : new DmlChartSpPr();
            mMarkerPr.SetProperty(DmlChartMarkerAttr.SpPr, spPr);
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
                return ChartShapeType.Default;
            }
            set
            {
                if (value != ChartShapeType.Default)
                    throw new InvalidOperationException("Cannot change the shape type of this chart element.");
            }
        }

        IThemeProvider IChartFormatSource.ThemeProvider
        {
            get { return mThemeProvider; }
        }

        bool IChartFormatSource.IsFormatDefined
        {
            get { return mMarkerPr.IsPropertySpecified(DmlChartMarkerAttr.SpPr) && !SpPr.IsEmpty; }
        }

        #endregion

        /// <summary>
        /// Gets or sets chart marker symbol.
        /// </summary>
        public MarkerSymbol Symbol
        {
            get { return (MarkerSymbol)mMarkerPr.GetProperty(DmlChartMarkerAttr.Symbol); }
            set
            {
                bool needApplyStyle = (Symbol == MarkerSymbol.None) && (value != MarkerSymbol.None);
                SetSymbolInternal(value);

                if (needApplyStyle && (mChart != null))
                {
                    int seriesIndex = 0;
                    for (int i = 0; i < mChart.Series.Count; i++)
                    {
                        if (mChart.Series[i].Marker == this)
                        {
                            seriesIndex = i;
                            break;
                        }
                    }

                    mChart.ChartSpace.ApplyChartStyle(this, seriesIndex);

                    if (value != MarkerSymbol.Default)
                        SetSymbolInternal(value); // The symbol can be changed during applying style.
                }
            }
        }

        /// <summary>
        /// Gets or sets chart marker size.
        /// Default value is 7.
        /// </summary>
        public int Size
        {
            get { return (int)mMarkerPr.GetProperty(DmlChartMarkerAttr.Size); }
            set
            {
                // Value cannot be less than 2 and more than 72
                if (value >= 2 && value <= 72)
                    mMarkerPr.SetProperty(DmlChartMarkerAttr.Size, value);
                else
                    throw new ArgumentException("Marker size has to be in range from 2 to 72.");
            }
        }

        /// <summary>
        /// Provides access to fill and line formatting of this marker.
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

        internal DmlChartSpPr SpPr
        {
            get { return (DmlChartSpPr)mMarkerPr.GetProperty(DmlChartMarkerAttr.SpPr); }
        }

        internal DmlChartMarkerPr MarkerPr
        {
            get { return mMarkerPr; }
        }

        internal bool IsDefaultSize
        {
            get
            {
                // False means that size is not set and default size must be used.
                return (!mMarkerPr.IsPropertySpecified(DmlChartMarkerAttr.Size));
            }
        }

        /// <summary>
        /// Gets the bounds of marker.
        /// </summary>
        internal RectangleF GetMarkerBounds(PointF center, float specialSize)
        {
            RectangleF boundingBox;

            if (Symbol == MarkerSymbol.Picture && IsRendered)
            {
                DmlBlipFill blipFill = SpPr.Fill as DmlBlipFill;
                ImageSizeCore imageSize = ImageUtil.GetImageSize(blipFill.ImageBytes);
                boundingBox = new RectangleF((center.X - imageSize.WidthEmus / 2),
                (center.Y - imageSize.HeightEmus / 2), imageSize.WidthEmus, imageSize.HeightEmus);
            }
            else
            {
                float size = ConvertUtilCore.PointToEmu(MathUtil.IsZero(specialSize) ? Size : specialSize);
                boundingBox = new RectangleF((center.X - size / 2), (center.Y - size / 2), size, size);
            }

            return boundingBox;
        }

        /// <summary>
        /// Gets the bounds of marker.
        /// </summary>
        internal RectangleF GetMarkerBounds(PointF center)
        {
            return GetMarkerBounds(center, 0);
        }

        /// <summary>
        /// Indicates whether to render the marker.
        /// </summary>
        internal bool IsRendered
        {
            get
            {
                if (Symbol == MarkerSymbol.Picture)
                {
                    DmlBlipFill blipFill = SpPr.Fill as DmlBlipFill;
                    // WORDSNET-20219 If "picture" is specified as a marker symbol, but there is no image file, then
                    // MS Word does not render markers.
                    return (blipFill != null) && (blipFill.ImageBytes != null) && (blipFill.ImageBytes.Length != 0);
                }

                // If the marker symbol is "None" do not render it.
                return (Symbol != MarkerSymbol.None);
            }
        }

        /// <summary>
        /// Returns a flag indicating whether any properties of this chart marker have different values than the
        /// corresponding properties of the parent data point collection.
        /// </summary>
        internal bool HasNonDefaultFormatting
        {
            get { return mMarkerPr.HasNonDefaultFormatting; }
        }

        private DmlChartMarkerPr mMarkerPr = new DmlChartMarkerPr();
        private ChartFormat mFormat;
        private readonly IThemeProvider mThemeProvider;
        private readonly DmlChart mChart;

        /// <summary>
        ///  According to the specification, default size is 5, but MS Word uses 7 as default, so do the same.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int DefaultMarkerSize = 7;
    }
}
