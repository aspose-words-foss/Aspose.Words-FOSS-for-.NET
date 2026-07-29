// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/29/2014 by Alexey Noskov

using Aspose.Collections;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents set of <see cref="ChartMarker"/> properties.
    /// </summary>
    internal class DmlChartMarkerPr
    {
        internal DmlChartMarkerPr()
        {
            SetParent(gDefaults);
        }

        internal void SetParent(DmlChartMarkerPr parentBag)
        {
            mPropertyBag.ParentBagProvider = new DmlChartMarkerPrParentBagProvider(parentBag);
        }

        internal DmlChartMarkerPr Clone()
        {
            DmlChartMarkerPr lhs = (DmlChartMarkerPr)MemberwiseClone();
            lhs.mPropertyBag = mPropertyBag.Clone();

            DmlChartSpPr spPr = (DmlChartSpPr)GetDirectProperty(DmlChartMarkerAttr.SpPr);
            if (spPr != null)
                lhs.SetProperty(DmlChartMarkerAttr.SpPr, spPr.Clone());

            StringToObjDictionary<DmlExtension> extensions = 
                (StringToObjDictionary<DmlExtension>)GetDirectProperty(DmlChartMarkerAttr.Extensions);
            if (extensions != null)
                lhs.SetProperty(DmlChartMarkerAttr.Extensions, DmlExtensionListSource.CloneExtensions(extensions));

            return lhs;
        }

        internal void SetProperty(DmlChartMarkerAttr attr, object value)
        {
            mPropertyBag.SetProperty((int)attr, value);
        }

        internal object GetProperty(DmlChartMarkerAttr attr)
        {
            return mPropertyBag.GetProperty((int)attr);
        }

        internal object GetDirectProperty(DmlChartMarkerAttr attr)
        {
            return mPropertyBag.GetDirectProperty((int)attr);
        }

        /// <summary>
        /// Removes the specified property.
        /// </summary>
        internal void RemoveProperty(DmlChartMarkerAttr attr)
        {
            mPropertyBag.Remove((int)attr);
        }

        /// <summary>
        /// Determines whether the mPropertyBag contains the specified property, which was set directly.
        /// </summary>
        /// <param name="attr">the attr</param>
        /// <returns>"true", if the  property was set directly, "false" otherwise</returns>
        internal bool IsPropertySpecified(DmlChartMarkerAttr attr)
        {
            return mPropertyBag.IsPropertySpecified((int)attr);
        }

        /// <summary>
        /// Clears contents of this property collection.
        /// </summary>
        internal void Clear()
        {
            mPropertyBag.RemoveAll();
            mPropertyBag.ExtensionProperties = null;
        }

        internal int Count
        {
            get { return mPropertyBag.Count; }
        }

        /// <summary>
        /// Returns <c>true</c> if this property collection redefines any properties of the parent collection.
        /// </summary>
        internal bool HasNonDefaultFormatting
        {
            get
            {
                IntList attributesToIgnore = new IntList();

                // Ignore empty SpPr properties.
                DmlChartSpPr spPr = (DmlChartSpPr)GetDirectProperty(DmlChartMarkerAttr.SpPr);
                if ((spPr == null) || spPr.IsEmpty)
                    attributesToIgnore.Add((int)DmlChartDataLabelAttrs.SpPr);

                return mPropertyBag.HasNonDefaultFormatting(attributesToIgnore.ToArray(), null);
            }
        }

        private IDmlHierarchicalPropertyBag mPropertyBag = new DmlHierarchicalPropertyBag();

        static DmlChartMarkerPr()
        {
            gDefaults = new DmlChartMarkerPr();
            gDefaults.SetProperty(DmlChartMarkerAttr.Symbol, MarkerSymbol.Default);
            gDefaults.SetProperty(DmlChartMarkerAttr.Size, 7);
            gDefaults.SetProperty(DmlChartMarkerAttr.SpPr, new DmlChartSpPr());
            gDefaults.SetProperty(DmlChartMarkerAttr.Extensions, null);
        }

        private static readonly DmlChartMarkerPr gDefaults;

        private class DmlChartMarkerPrParentBagProvider : IDmlHierarchicalPropertyBagParentProvider
        {
            internal DmlChartMarkerPrParentBagProvider(DmlChartMarkerPr parentPr)
            {
                mParentPr = parentPr;
            }

            public IDmlHierarchicalPropertyBag ParentBag
            {
                get { return (mParentPr != null) ? mParentPr.mPropertyBag : null; }
            }

            public IDmlHierarchicalPropertyBagParentProvider Clone()
            {
                DmlChartMarkerPrParentBagProvider clone = (DmlChartMarkerPrParentBagProvider)MemberwiseClone();
                if (mParentPr != null)
                    clone.mParentPr = mParentPr.Clone();
                return clone;
            }

            private DmlChartMarkerPr mParentPr;
        }
    }
}
