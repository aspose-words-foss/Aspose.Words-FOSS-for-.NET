// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class contains chart properties.
    /// </summary>
    internal class DmlChartPr
    {
        internal DmlChartPr() : this(null)
        {
        }

        internal DmlChartPr(DocumentBase doc)
        {
            // WORDSNET-20872 The defaults depend on the document version (7.2.2.2 AppVersion (Application Version)).
            // The version in CompatibilityOptions does not affect the defaults.
            DmlChartPr defaults = (DmlChartUtil.IsMsWord2007OrLower(doc)) ? gDefaultPr : gDefaultPrAfter2007;
            mPrBag.ParentBagProvider = new DmlChartPrParentBagProvider(defaults);
            // Collection of series must be present in properties directly.
            mPrBag.SetProperty((int)DmlChartAttrs.Series, new List<ChartSeries>());
            // Also all complex types must be present in properties directly.
            mPrBag.SetProperty((int)DmlChartAttrs.DropLines, new DmlChartSpPr());
            mPrBag.SetProperty((int)DmlChartAttrs.HiLowLines, new DmlChartSpPr());
            mPrBag.SetProperty((int)DmlChartAttrs.SerLines, new DmlChartSpPr());
            mPrBag.SetProperty((int)DmlChartAttrs.BandFmts, new DmlChartBandFormats());
        }

        internal DmlChartPr Clone()
        {
            DmlChartPr lhs = (DmlChartPr)MemberwiseClone();
            lhs.mPrBag = mPrBag.Clone();

            IList<ChartSeries> cloningSeries = (IList<ChartSeries>)GetDirectProperty(DmlChartAttrs.Series);
            if (cloningSeries != null)
            {
                IList<ChartSeries> clonedSeries = new List<ChartSeries>();
                foreach (ChartSeries series in cloningSeries)
                {
                    ChartSeries cloned = series.Clone();
                    clonedSeries.Add(cloned);
                }
                lhs.SetProperty(DmlChartAttrs.Series, clonedSeries);

                // Assign owner
                for (int i = 0; i < cloningSeries.Count; i++)
                {
                    ChartSeries series = cloningSeries[i];
                    if (series.Owner != null)
                    {
                        int ownerIndex = cloningSeries.IndexOf(series.Owner);
                        clonedSeries[i].Owner = clonedSeries[ownerIndex];
                    }
                }
            }

            ChartDataLabelCollection labels = (ChartDataLabelCollection)GetDirectProperty(DmlChartAttrs.DLbls);
            if (labels != null)
                lhs.SetProperty(DmlChartAttrs.DLbls, labels.Clone());

            DmlChartSpPr dropLines = (DmlChartSpPr)GetDirectProperty(DmlChartAttrs.DropLines);
            if (dropLines != null)
                lhs.SetProperty(DmlChartAttrs.DropLines, dropLines.Clone());

            DmlChartSpPr hiLowLines = (DmlChartSpPr)GetDirectProperty(DmlChartAttrs.HiLowLines);
            if (hiLowLines != null)
                lhs.SetProperty(DmlChartAttrs.HiLowLines, hiLowLines.Clone());

            DmlChartSpPr serLines = (DmlChartSpPr)GetDirectProperty(DmlChartAttrs.SerLines);
            if (serLines != null)
                lhs.SetProperty(DmlChartAttrs.SerLines, serLines.Clone());

            DmlChartUpDownBars upDownBars = (DmlChartUpDownBars)GetDirectProperty(DmlChartAttrs.UpDownBars);
            if (upDownBars != null)
                lhs.SetProperty(DmlChartAttrs.UpDownBars, upDownBars.Clone());

            int[] split = (int[])GetDirectProperty(DmlChartAttrs.CustSplit);
            if (split != null)
                lhs.SetProperty(DmlChartAttrs.CustSplit, split.Clone());

            DmlChartBandFormats bandFormats = (DmlChartBandFormats)GetDirectProperty(DmlChartAttrs.BandFmts);
            if (bandFormats != null)
                lhs.SetProperty(DmlChartAttrs.BandFmts, bandFormats.Clone());

            StringToObjDictionary<DmlExtension> extensions =
                (StringToObjDictionary<DmlExtension>)GetDirectProperty(DmlChartAttrs.Extensions);
            if (extensions != null)
                lhs.SetProperty(DmlChartAttrs.Extensions, DmlExtensionListSource.CloneExtensions(extensions));

            return lhs;
        }

        internal object GetProperty(DmlChartAttrs attr)
        {
            return mPrBag.GetProperty((int)attr);
        }

        internal void SetProperty(DmlChartAttrs attr, object value)
        {
            mPrBag.SetProperty((int)attr, value);
        }

        /// <summary>
        /// Returns null if property is not set explicitly
        /// </summary>
        internal object GetDirectProperty(DmlChartAttrs attr)
        {
            return mPrBag.GetDirectProperty((int) attr);
        }

        /// <summary>
        /// Determines whether the mPropertyBag contains the specified property, which was set directly.
        /// </summary>
        /// <param name="attr">the attr</param>
        /// <returns>"true", if the  property was set directly, "false" otherwise</returns>
        internal bool IsPropertySpecified(DmlChartAttrs attr)
        {
            return mPrBag.IsPropertySpecified((int)attr);
        }

        private IDmlHierarchicalPropertyBag mPrBag = new DmlHierarchicalPropertyBag();

        static DmlChartPr()
        {
            gDefaultPr = GetDefaults();
            gDefaultPrAfter2007 = GetDefaultsAfter2007();
        }

        private static DmlChartPr GetDefaults()
        {
            DmlChartPr defaults = new DmlChartPr();
            defaults.SetProperty(DmlChartAttrs.AxIdX, 0);
            defaults.SetProperty(DmlChartAttrs.AxIdY, 1);
            defaults.SetProperty(DmlChartAttrs.AxIdZ, 2);
            defaults.SetProperty(DmlChartAttrs.DLbls, null);
            defaults.SetProperty(DmlChartAttrs.IsDropLinesVisible, false);
            defaults.SetProperty(DmlChartAttrs.IsHiLowLinesVisible, false);
            defaults.SetProperty(DmlChartAttrs.GapDepth, 150);
            defaults.SetProperty(DmlChartAttrs.GapWidth, 150);
            defaults.SetProperty(DmlChartAttrs.Grouping, Grouping.Standard);
            defaults.SetProperty(DmlChartAttrs.VaryColors, false);
            defaults.SetProperty(DmlChartAttrs.BarDir, BarDirection.Bar);
            defaults.SetProperty(DmlChartAttrs.Shape, BarShape.Box);
            defaults.SetProperty(DmlChartAttrs.Overlap, 0);
            defaults.SetProperty(DmlChartAttrs.BubbleScale, 0);
            defaults.SetProperty(DmlChartAttrs.ShowNegBubbles, false);
            defaults.SetProperty(DmlChartAttrs.ShowSerLine, false);
            defaults.SetProperty(DmlChartAttrs.SizeRepresents, SizeRepresents.Area);
            defaults.SetProperty(DmlChartAttrs.FirstSliceAng, 0);
            defaults.SetProperty(DmlChartAttrs.HoleSize, 10);
            defaults.SetProperty(DmlChartAttrs.ShowMarker, false);
            defaults.SetProperty(DmlChartAttrs.Smooth, false);
            defaults.SetProperty(DmlChartAttrs.CustSplit, new int[0]);
            defaults.SetProperty(DmlChartAttrs.OfPieType, OfPieType.Pie);
            defaults.SetProperty(DmlChartAttrs.SecondPieSize, 75);
            defaults.SetProperty(DmlChartAttrs.SplitPos, double.NaN);
            defaults.SetProperty(DmlChartAttrs.SplitType, SplitType.Auto);
            defaults.SetProperty(DmlChartAttrs.RadarStyle, RadarStyle.Standard);
            defaults.SetProperty(DmlChartAttrs.ScatterStyle, ScatterStyle.None);
            defaults.SetProperty(DmlChartAttrs.Wireframe, false);
            defaults.SetProperty(DmlChartAttrs.Extensions, null);

            return defaults;
        }

        private static DmlChartPr GetDefaultsAfter2007()
        {
            DmlChartPr defaults = GetDefaults();
            defaults.SetProperty(DmlChartAttrs.VaryColors, true);

            return defaults;
        }

        /// <summary>
        /// Gets or sets the grouping of the chart.
        /// </summary>
        internal Grouping Grouping
        {
            get { return (Grouping)GetProperty(DmlChartAttrs.Grouping); }
            set { SetProperty(DmlChartAttrs.Grouping, value); }
        }

        /// <summary>
        /// Gets or sets the type of a combo Pie chart: Pie of Pie or Bar of Pie.
        /// </summary>
        internal OfPieType OfPieType
        {
            get { return (OfPieType)GetProperty(DmlChartAttrs.OfPieType); }
            set { SetProperty(DmlChartAttrs.OfPieType, value); }
        }

        /// <summary>
        /// Gets or sets the bar direction of the chart.
        /// </summary>
        internal BarDirection BarDirection
        {
            get { return (BarDirection)GetProperty(DmlChartAttrs.BarDir); }
            set { SetProperty(DmlChartAttrs.BarDir, value); }
        }

        private static readonly DmlChartPr gDefaultPr;
        private static readonly DmlChartPr gDefaultPrAfter2007;

        private class DmlChartPrParentBagProvider : IDmlHierarchicalPropertyBagParentProvider
        {
            internal DmlChartPrParentBagProvider(DmlChartPr parentPr)
            {
                mParentPr = parentPr;
            }

            public IDmlHierarchicalPropertyBag ParentBag
            {
                get { return (mParentPr != null) ? mParentPr.mPrBag : null; }
            }

            public IDmlHierarchicalPropertyBagParentProvider Clone()
            {
                DmlChartPrParentBagProvider lhs = (DmlChartPrParentBagProvider)MemberwiseClone();
                if (mParentPr != null)
                    lhs.mParentPr = mParentPr.Clone();
                return lhs;
            }

            private DmlChartPr mParentPr;
        }
    }
}
