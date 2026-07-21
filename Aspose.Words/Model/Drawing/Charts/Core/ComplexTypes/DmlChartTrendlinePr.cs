// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/29/2014 by Alexey Noskov

using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents set of <see cref="DmlChartTrendline"/> properties.
    /// </summary>
    internal class DmlChartTrendlinePr
    {
        internal DmlChartTrendlinePr() : this(gDefaults)
        {
        }

        private DmlChartTrendlinePr(DmlChartTrendlinePr parentBag)
        {
            mPropertyBag.ParentBagProvider = new DmlChartTrendlinePrParentBagProvider((parentBag != null) ? parentBag : gDefaults);
        }

        internal DmlChartTrendlinePr Clone()
        {
            DmlChartTrendlinePr lhs = (DmlChartTrendlinePr)MemberwiseClone();
            lhs.mPropertyBag = mPropertyBag.Clone();

            DmlChartSpPr spPr = (DmlChartSpPr)GetDirectProperty(DmlChartTrendlineAttr.SpPr);
            if (spPr != null)
                lhs.SetProperty(DmlChartTrendlineAttr.SpPr, spPr.Clone());

            DmlChartTrendlineLabel label = (DmlChartTrendlineLabel)GetDirectProperty(DmlChartTrendlineAttr.TrendlineLbl);
            if (label != null)
                lhs.SetProperty(DmlChartTrendlineAttr.TrendlineLbl, label.Clone());

            StringToObjDictionary<DmlExtension> extensions = 
                (StringToObjDictionary<DmlExtension>)GetDirectProperty(DmlChartTrendlineAttr.Extensions);
            if (extensions != null)
                lhs.SetProperty(DmlChartTrendlineAttr.Extensions, DmlExtensionListSource.CloneExtensions(extensions));

            return lhs;
        }

        internal void SetProperty(DmlChartTrendlineAttr attr, object value)
        {
            mPropertyBag.SetProperty((int)attr, value);
        }

        internal object GetProperty(DmlChartTrendlineAttr attr)
        {
            return mPropertyBag.GetProperty((int)attr);
        }

        internal object GetDirectProperty(DmlChartTrendlineAttr attr)
        {
            return mPropertyBag.GetDirectProperty((int)attr);
        }

        /// <summary>
        /// Determines whether the mPropertyBag contains the specified property, which was set directly.
        /// </summary>
        /// <param name="attr">the attr</param>
        /// <returns>"true", if the  property was set directly, "false" otherwise</returns>
        internal bool IsPropertySpecified(DmlChartTrendlineAttr attr)
        {
            return mPropertyBag.IsPropertySpecified((int)attr);
        }

        private IDmlHierarchicalPropertyBag mPropertyBag = new DmlHierarchicalPropertyBag();

        static DmlChartTrendlinePr()
        {
            gDefaults = new DmlChartTrendlinePr();
            gDefaults.SetProperty(DmlChartTrendlineAttr.Backward, 0.0d);
            gDefaults.SetProperty(DmlChartTrendlineAttr.DispEq, false);
            gDefaults.SetProperty(DmlChartTrendlineAttr.DispRSqr, false);
            gDefaults.SetProperty(DmlChartTrendlineAttr.Forward, 0.0d);
            gDefaults.SetProperty(DmlChartTrendlineAttr.Intercept, double.NaN);
            gDefaults.SetProperty(DmlChartTrendlineAttr.Name, null);
            gDefaults.SetProperty(DmlChartTrendlineAttr.Order, 2);
            gDefaults.SetProperty(DmlChartTrendlineAttr.Period, 2);
            gDefaults.SetProperty(DmlChartTrendlineAttr.TrendlineLbl, null);
            gDefaults.SetProperty(DmlChartTrendlineAttr.TrendlineType, TrendlineType.Linear);
        }

        private static readonly DmlChartTrendlinePr gDefaults;

        private class DmlChartTrendlinePrParentBagProvider : IDmlHierarchicalPropertyBagParentProvider
        {
            public DmlChartTrendlinePrParentBagProvider(DmlChartTrendlinePr parentPr)
            {
                mParentPr = parentPr;
            }

            public IDmlHierarchicalPropertyBag ParentBag
            {
                get { return (mParentPr != null) ? mParentPr.mPropertyBag : null; }
            }

            public IDmlHierarchicalPropertyBagParentProvider Clone()
            {
                DmlChartTrendlinePrParentBagProvider clone = (DmlChartTrendlinePrParentBagProvider)MemberwiseClone();
                if (mParentPr != null)
                    clone.mParentPr = mParentPr.Clone();
                return clone;
            }

            private DmlChartTrendlinePr mParentPr;
        }

    }
}
