// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/03/2017 by Alexander Zhiltsov

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// This class represents the 2.24.3.72 CT_SeriesLayoutProperties complex type [MS-ODRAWXML].
    /// It specifies series layout properties.
    /// </summary>
    internal class DmlChartSeriesLayoutPr
    {
        internal DmlChartSeriesLayoutPr()
        {
            SetParent(gDefaults);
        }

        /// <summary>
        /// Returns a copy of this instance.
        /// </summary>
        internal DmlChartSeriesLayoutPr Clone()
        {
            DmlChartSeriesLayoutPr lhs = (DmlChartSeriesLayoutPr)MemberwiseClone();
            lhs.mPropertyBag = mPropertyBag.Clone();

            DmlChartBinningPr binning = (DmlChartBinningPr)GetDirectProperty(DmlChartSeriesLayoutAttr.Binning);
            if (binning != null)
                SetProperty(DmlChartSeriesLayoutAttr.Binning, binning.Clone());

            List<int> subtotals = (List<int>)GetDirectProperty(DmlChartSeriesLayoutAttr.Subtotals);
            if (subtotals != null)
            {
                List<int> clonedSubtotals = new List<int>();
                foreach (int index in subtotals)
                    clonedSubtotals.Add(index);

                SetProperty(DmlChartSeriesLayoutAttr.Subtotals, clonedSubtotals);
            }

            StringToObjDictionary<DmlExtension> extensions = 
                (StringToObjDictionary<DmlExtension>)GetDirectProperty(DmlChartSeriesLayoutAttr.Extensions);
            if (extensions != null)
                SetProperty(DmlChartSeriesLayoutAttr.Extensions, DmlExtensionListSource.CloneExtensions(extensions));

            return lhs;
        }

        /// <summary>
        /// Set parent container of property values.
        /// </summary>
        internal void SetParent(DmlChartSeriesLayoutPr parentBag)
        {
            mPropertyBag.ParentBagProvider = new DmlChartSeriesLayoutPrParentBagProvider(parentBag);
        }

        /// <summary>
        /// Sets property value.
        /// </summary>
        internal void SetProperty(DmlChartSeriesLayoutAttr attr, object value)
        {
            mPropertyBag.SetProperty((int)attr, value);
        }

        /// <summary>
        /// Gets property value. If it is not defined, default value is returned.
        /// </summary>
        internal object GetProperty(DmlChartSeriesLayoutAttr attr)
        {
            return mPropertyBag.GetProperty((int)attr);
        }

        /// <summary>
        /// Gets property value. If it is not defined, <c>null</c> is returned.
        /// </summary>
        internal object GetDirectProperty(DmlChartSeriesLayoutAttr attr)
        {
            return mPropertyBag.GetDirectProperty((int)attr);
        }

        /// <summary>
        /// Determines whether the mPropertyBag contains the specified property, which was set directly.
        /// </summary>
        /// <param name="attr">the attr</param>
        /// <returns>"true", if the  property was set directly, "false" otherwise</returns>
        internal bool IsPropertySpecified(DmlChartSeriesLayoutAttr attr)
        {
            return mPropertyBag.IsPropertySpecified((int)attr);
        }

        /// <summary>
        /// Number of defined properties.
        /// </summary>
        internal int Count
        {
            get { return mPropertyBag.Count; }
        }

        /// <summary>
        /// Specifies the visibility of a line connecting all mean points.
        /// </summary>
        internal bool IsMeanLineVisible
        {
            get { return (bool)GetProperty(DmlChartSeriesLayoutAttr.IsMeanLineVisible); }
            set { SetProperty(DmlChartSeriesLayoutAttr.IsMeanLineVisible, value); }
        }

        /// <summary>
        /// Specifies the visibility of markers denoting the mean.
        /// </summary>
        internal bool IsMeanMarkerVisible
        {
            get { return (bool)GetProperty(DmlChartSeriesLayoutAttr.IsMeanMarkerVisible); }
            set { SetProperty(DmlChartSeriesLayoutAttr.IsMeanMarkerVisible, value); }
        }

        /// <summary>
        /// Specifies the visibility of non-outlier data points.
        /// </summary>
        internal bool IsNonOutliersVisible
        {
            get { return (bool)GetProperty(DmlChartSeriesLayoutAttr.IsNonOutliersVisible); }
            set { SetProperty(DmlChartSeriesLayoutAttr.IsNonOutliersVisible, value); }
        }

        /// <summary>
        /// Specifies the visibility of outlier data points.
        /// </summary>
        internal bool IsOutliersVisible
        {
            get { return (bool)GetProperty(DmlChartSeriesLayoutAttr.IsOutliersVisible); }
            set { SetProperty(DmlChartSeriesLayoutAttr.IsOutliersVisible, value); }
        }

        /// <summary>
        /// Specifies the quartile calculation method.
        /// </summary>
        internal QuartileMethod QuartileMethod
        {
            get { return (QuartileMethod)GetProperty(DmlChartSeriesLayoutAttr.QuartileMethod); }
            set { SetProperty(DmlChartSeriesLayoutAttr.QuartileMethod, value); }
        }

        /// <summary>
        /// Specifies whether data aggregation is performed.
        /// </summary>
        internal bool IsAggregation
        {
            get { return (bool)GetProperty(DmlChartSeriesLayoutAttr.IsAggregation); }
            set { SetProperty(DmlChartSeriesLayoutAttr.IsAggregation, value); }
        }

        /// <summary>
        /// Specifies the data binning properties for the series.
        /// </summary>
        internal DmlChartBinningPr Binning
        {
            get { return (DmlChartBinningPr)GetProperty(DmlChartSeriesLayoutAttr.Binning); }
            set { SetProperty(DmlChartSeriesLayoutAttr.Binning, value); }
        }

        /// <summary>
        /// Specifies a list of indexes of subtotal data points.
        /// </summary>
        internal List<int> SubTotals
        {
            get { return (List<int>)GetDirectProperty(DmlChartSeriesLayoutAttr.Subtotals); }
            set { SetProperty(DmlChartSeriesLayoutAttr.Subtotals, value); }
        }

        /// <summary>
        /// Specifies  the visibility of connector lines between data points.
        /// </summary>
        internal bool IsConnectorLinesVisible
        {
            get { return (bool)GetProperty(DmlChartSeriesLayoutAttr.IsConnectorLinesVisible); }
            set { SetProperty(DmlChartSeriesLayoutAttr.IsConnectorLinesVisible, value); }
        }

        private IDmlHierarchicalPropertyBag mPropertyBag = new DmlHierarchicalPropertyBag();

        /// <summary>
        /// Static ctor that inits default property values.
        /// </summary>
        static DmlChartSeriesLayoutPr()
        {
            gDefaults = new DmlChartSeriesLayoutPr();
            gDefaults.SetProperty(DmlChartSeriesLayoutAttr.ParentLabelLayout, ParentLabelLayout.None);
            gDefaults.SetProperty(DmlChartSeriesLayoutAttr.RegionLabelLayout, RegionLabelLayout.None);
            gDefaults.SetProperty(DmlChartSeriesLayoutAttr.IsConnectorLinesVisible, true);
            gDefaults.SetProperty(DmlChartSeriesLayoutAttr.IsMeanLineVisible, false);
            gDefaults.SetProperty(DmlChartSeriesLayoutAttr.IsMeanMarkerVisible, true);
            gDefaults.SetProperty(DmlChartSeriesLayoutAttr.IsNonOutliersVisible, true);
            gDefaults.SetProperty(DmlChartSeriesLayoutAttr.IsOutliersVisible, true);
            gDefaults.SetProperty(DmlChartSeriesLayoutAttr.IsAggregation, false);
            gDefaults.SetProperty(DmlChartSeriesLayoutAttr.Binning, null);
            gDefaults.SetProperty(DmlChartSeriesLayoutAttr.Geography, null);
            gDefaults.SetProperty(DmlChartSeriesLayoutAttr.QuartileMethod, QuartileMethod.ExclusiveMedian);
            gDefaults.SetProperty(DmlChartSeriesLayoutAttr.Subtotals, null);
            gDefaults.SetProperty(DmlChartSeriesLayoutAttr.Extensions, null);
        }

        private static readonly DmlChartSeriesLayoutPr gDefaults;

        private class DmlChartSeriesLayoutPrParentBagProvider : IDmlHierarchicalPropertyBagParentProvider
        {
            internal DmlChartSeriesLayoutPrParentBagProvider(DmlChartSeriesLayoutPr parentPr)
            {
                mParentPr = parentPr;
            }

            public IDmlHierarchicalPropertyBag ParentBag
            {
                get { return (mParentPr != null) ? mParentPr.mPropertyBag : null; }
            }

            public IDmlHierarchicalPropertyBagParentProvider Clone()
            {
                DmlChartSeriesLayoutPrParentBagProvider clone = 
                    (DmlChartSeriesLayoutPrParentBagProvider)MemberwiseClone();
                if (mParentPr != null)
                    clone.mParentPr = mParentPr.Clone();
                return clone;
            }

            private DmlChartSeriesLayoutPr mParentPr;
        }
    }
}
