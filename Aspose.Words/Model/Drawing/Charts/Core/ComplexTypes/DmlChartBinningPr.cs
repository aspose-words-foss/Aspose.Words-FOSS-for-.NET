// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/03/2017 by Alexander Zhiltsov

using System;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// This class represents the 2.24.3.7 CT_Binning complex type [MS-ODRAWXML].
    /// It specifies data binning properties.
    /// </summary>
    internal class DmlChartBinningPr
    {
        internal DmlChartBinningPr()
        {
            SetParent(gDefaults);
            SetProperty(DmlChartBinningAttr.Underflow, DoubleOrAutomatic.AsNull(0));
            SetProperty(DmlChartBinningAttr.Overflow, DoubleOrAutomatic.AsNull(0));
        }

        /// <summary>
        /// Returns a copy of this instance.
        /// </summary>
        internal DmlChartBinningPr Clone()
        {
            DmlChartBinningPr lhs = (DmlChartBinningPr)MemberwiseClone();
            lhs.mPropertyBag = mPropertyBag.Clone();

            DoubleOrAutomatic underflow = (DoubleOrAutomatic)GetProperty(DmlChartBinningAttr.Underflow);
            lhs.SetProperty(DmlChartBinningAttr.Underflow, underflow.Clone());
            DoubleOrAutomatic overflow = (DoubleOrAutomatic)GetProperty(DmlChartBinningAttr.Overflow);
            lhs.SetProperty(DmlChartBinningAttr.Overflow, overflow.Clone());

            return lhs;
        }

        /// <summary>
        /// Set parent container of property values.
        /// </summary>
        internal void SetParent(DmlChartBinningPr parentBag)
        {
            mPropertyBag.ParentBagProvider = new DmlChartBinningPrParentBagProvider(parentBag);
        }

        /// <summary>
        /// Sets property value.
        /// </summary>
        internal void SetProperty(DmlChartBinningAttr attr, object value)
        {
            mPropertyBag.SetProperty((int)attr, value);
        }

        /// <summary>
        /// Gets property value. If it is not defined, default value is returned.
        /// </summary>
        internal object GetProperty(DmlChartBinningAttr attr)
        {
            return mPropertyBag.GetProperty((int)attr);
        }

        /// <summary>
        /// Gets property value. If it is not defined, <c>null</c> is returned.
        /// </summary>
        internal object GetDirectProperty(DmlChartBinningAttr attr)
        {
            return mPropertyBag.GetDirectProperty((int)attr);
        }

        /// <summary>
        /// Determines whether the mPropertyBag contains the specified property, which was set directly.
        /// </summary>
        /// <param name="attr">the attr</param>
        /// <returns>"true", if the  property was set directly, "false" otherwise</returns>
        internal bool IsPropertySpecified(DmlChartBinningAttr attr)
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
        /// Specifies the bin count. If it is <b>-1</b>, bins are defined by bin size or are calculated automatically.
        /// </summary>
        internal int BinCount
        {
            get { return (int)GetProperty(DmlChartBinningAttr.BinCount); }
            set { SetProperty(DmlChartBinningAttr.BinCount, value); }
        }

        /// <summary>
        /// Specifies the bin size. If it is <see cref="double.NaN"/>, explicit bin count is defined, or bins are
        /// calculated automatically.
        /// </summary>
        internal double BinSize
        {
            get { return (double)GetProperty(DmlChartBinningAttr.BinSize); }
            set { SetProperty(DmlChartBinningAttr.BinSize, value); }
        }

        /// <summary>
        /// Allows to specify the custom value for underflow bin.
        /// </summary>
        internal DoubleOrAutomatic Underflow
        {
            get { return (DoubleOrAutomatic)GetProperty(DmlChartBinningAttr.Underflow); }
            set { SetProperty(DmlChartBinningAttr.Underflow, value); }
        }

        /// <summary>
        /// Allows to specify the custom value for overflow bin.
        /// </summary>
        internal DoubleOrAutomatic Overflow
        {
            get { return (DoubleOrAutomatic)GetProperty(DmlChartBinningAttr.Overflow); }
            set { SetProperty(DmlChartBinningAttr.Overflow, value); }
        }

        private IDmlHierarchicalPropertyBag mPropertyBag = new DmlHierarchicalPropertyBag();

        /// <summary>
        /// Static ctor that inits default property values.
        /// </summary>
        static DmlChartBinningPr()
        {
            gDefaults = new DmlChartBinningPr();
            gDefaults.SetProperty(DmlChartBinningAttr.BinSize, Double.NaN);
            gDefaults.SetProperty(DmlChartBinningAttr.BinCount, -1);
            gDefaults.SetProperty(DmlChartBinningAttr.IntervalClosed, null);
        }

        private static readonly DmlChartBinningPr gDefaults;

        private class DmlChartBinningPrParentBagProvider : IDmlHierarchicalPropertyBagParentProvider
        {
            internal DmlChartBinningPrParentBagProvider(DmlChartBinningPr parentPr)
            {
                mParentPr = parentPr;
            }

            public IDmlHierarchicalPropertyBag ParentBag
            {
                get { return (mParentPr != null) ? mParentPr.mPropertyBag : null; }
            }

            public IDmlHierarchicalPropertyBagParentProvider Clone()
            {
                DmlChartBinningPrParentBagProvider clone = 
                    (DmlChartBinningPrParentBagProvider)MemberwiseClone();
                if (mParentPr != null)
                    clone.mParentPr = mParentPr.Clone();
                return clone;
            }

            private DmlChartBinningPr mParentPr;
        }
    }
}
