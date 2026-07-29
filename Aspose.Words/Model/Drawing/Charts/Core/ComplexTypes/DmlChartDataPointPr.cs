// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/29/2014 by Alexey Noskov

using System;
using Aspose.Collections;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents set of <see cref="ChartDataPoint"/> properties.
    /// </summary>
    internal class DmlChartDataPointPr
    {
        private DmlChartDataPointPr(bool isDefaultTag)
        {
            // intentionally left blank
            // WORDSNET-12521
            // Because unexpected behaviour while in multi-threading mode
            // We have to create DEFAULTS every time when DmlChartDataPointPr is created.
            // It helps to avoid changing of DEFAULT tags.
            // It will be removed after the issue is fixed.
        }

        internal DmlChartDataPointPr(DocumentBase doc)
        {
            SetParent(GetDefaults(doc));
        }

        internal void SetParent(DmlChartDataPointPr parentBag)
        {
            mPropertyBag.ParentBagProvider = new DmlChartDataPointPrParentBagProvider(parentBag);
            mParentPr = parentBag;

            SetMarkerParent((ChartMarker)GetDirectProperty(DmlChartDataPointAttr.Marker));
        }

        internal DmlChartDataPointPr Clone()
        {
            DmlChartDataPointPr lhs = (DmlChartDataPointPr)MemberwiseClone();
            lhs.mPropertyBag = mPropertyBag.Clone();

            ChartMarker marker = (ChartMarker)GetDirectProperty(DmlChartDataPointAttr.Marker);
            if (marker != null)
                lhs.SetProperty(DmlChartDataPointAttr.Marker, marker.Clone());

            DmlChartSpPr spPr = (DmlChartSpPr)GetDirectProperty(DmlChartDataPointAttr.SpPr);
            if (spPr != null)
                lhs.SetProperty(DmlChartDataPointAttr.SpPr, spPr.Clone());

            DmlChartPictureOptions options = 
                (DmlChartPictureOptions)GetDirectProperty(DmlChartDataPointAttr.PictureOptions);
            if (options != null)
                lhs.SetProperty(DmlChartDataPointAttr.PictureOptions, options.Clone());

            StringToObjDictionary<DmlExtension> extensions = 
                (StringToObjDictionary<DmlExtension>)GetDirectProperty(DmlChartDataPointAttr.Extensions);
            if (extensions != null)
                lhs.SetProperty(DmlChartDataPointAttr.Extensions, DmlExtensionListSource.CloneExtensions(extensions));

            return lhs;
        }

        internal void SetProperty(DmlChartDataPointAttr attr, object value)
        {
            mPropertyBag.SetProperty((int)attr, value);

            if (attr == DmlChartDataPointAttr.Marker)
                SetMarkerParent((ChartMarker)value);
        }

        /// <summary>
        /// Removes the specified property.
        /// </summary>
        internal void RemoveProperty(DmlChartDataPointAttr attr)
        {
            mPropertyBag.Remove((int)attr);
        }

        internal object GetProperty(DmlChartDataPointAttr attr)
        {
            return mPropertyBag.GetProperty((int)attr);
        }

        internal object GetDirectProperty(DmlChartDataPointAttr attr)
        {
            return mPropertyBag.GetDirectProperty((int)attr);
        }

        /// <summary>
        /// Gets the property value from the parent bag provider.
        /// </summary>
        internal object GetInheritedProperty(DmlChartDataPointAttr attr)
        {
            return mPropertyBag.ParentBagProvider.ParentBag.GetProperty((int)attr);
        }

        /// <summary>
        /// Determines whether the mPropertyBag contains the specified property, which was set directly.
        /// </summary>
        /// <param name="attr">the attr</param>
        /// <returns>"true", if the  property was set directly, "false" otherwise</returns>
        internal bool IsPropertySpecified(DmlChartDataPointAttr attr)
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

        /// <summary>
        /// Copies properties set directly from the specified data point property collection.
        /// The <see cref="DmlChartDataPointAttr.Index"/> property is not copied.
        /// </summary>
        internal void CopyDirectPropertiesFrom(DmlChartDataPointPr pointPr)
        {
            foreach (DmlChartDataPointAttr attribute in Enum.GetValues(typeof(DmlChartDataPointAttr)))
            {
                if (!pointPr.IsPropertySpecified(attribute) || (attribute == DmlChartDataPointAttr.Index))
                    continue;

                object value = pointPr.GetDirectProperty(attribute);

                switch (attribute)
                {
                    case DmlChartDataPointAttr.Marker:
                    {
                        if (value != null)
                            value = ((ChartMarker)value).Clone();

                        break;
                    }
                    case DmlChartDataPointAttr.SpPr:
                    {
                        if (value != null)
                            value = ((DmlChartSpPr)value).Clone();

                        break;
                    }
                    case DmlChartDataPointAttr.PictureOptions:
                    {
                        if (value != null)
                            value = ((DmlChartPictureOptions)value).Clone();

                        break;
                    }
                    case DmlChartDataPointAttr.Extensions:
                        CopyExtensionsFrom((StringToObjDictionary<DmlExtension>)value);
                        continue;
                }

                SetProperty(attribute, value);
            }
        }

        private void CopyExtensionsFrom(StringToObjDictionary<DmlExtension> source)
        {
            if (source == null)
                return;

            StringToObjDictionary<DmlExtension> extensions =
                (StringToObjDictionary<DmlExtension>)GetDirectProperty(DmlChartDataPointAttr.Extensions);
            if (extensions == null)
                extensions = new StringToObjDictionary<DmlExtension>(false);

            bool areExtensionsCopied = false;

            foreach (DmlExtension ext in source.Values)
            {
                // Let's skip copying unique ID.
                if (StringUtil.EqualsOrdinalIgnoreCase(ext.Uri, DmlExtensionUri.UniqueId))
                    continue;

                extensions[ext.Uri] = ext.Clone();
                areExtensionsCopied = true;
            }

            if (areExtensionsCopied)
                SetProperty(DmlChartDataPointAttr.Extensions, extensions);
        }

        /// <summary>
        /// Returns <c>true</c> if this property collection redefines any properties of the parent collection.
        /// </summary>
        internal bool HasNonDefaultFormatting
        {
            get
            {
                ChartMarker marker = (ChartMarker)GetDirectProperty(DmlChartDataPointAttr.Marker);
                if ((marker != null) && marker.HasNonDefaultFormatting)
                    return true;

                IntList attributesToIgnore = new IntList();

                // Ignore since each point has its own index.
                attributesToIgnore.Add((int)DmlChartDataPointAttr.Index);
                attributesToIgnore.Add((int)DmlChartDataPointAttr.Marker); // Already checked.
                attributesToIgnore.Add((int)DmlChartDataPointAttr.Extensions); // Checked separately below.

                // Ignore empty SpPr properties.
                DmlChartSpPr spPr = (DmlChartSpPr)GetDirectProperty(DmlChartDataPointAttr.SpPr);
                if ((spPr == null) || spPr.IsEmpty)
                    attributesToIgnore.Add((int)DmlChartDataPointAttr.SpPr);

                if (mPropertyBag.HasNonDefaultFormatting(attributesToIgnore.ToArray(), null))
                    return true;

                StringToObjDictionary<DmlExtension> extensions =
                    (StringToObjDictionary<DmlExtension>)GetDirectProperty(DmlChartDataPointAttr.Extensions);

                if (extensions != null)
                {
                    foreach (string uri in extensions.Keys)
                    {
                        // Ignore UniqueId since it seems it has no any useful information.
                        // If another extension exists, just consider that the extensions are different (no way to
                        // compare them now).
                        if (!StringUtil.EqualsOrdinalIgnoreCase(uri, DmlExtensionUri.UniqueId))
                            return true;
                    }
                }

                return false;
            }
        }

        private void SetMarkerParent(ChartMarker marker)
        {
            if (marker == null)
                return;

            ChartMarker parentMarker = (ChartMarker)mParentPr.GetDirectProperty(DmlChartDataPointAttr.Marker);
            if (parentMarker != null)
                marker.MarkerPr.SetParent(parentMarker.MarkerPr);
        }

        private IDmlHierarchicalPropertyBag mPropertyBag = new DmlHierarchicalPropertyBag();
        private DmlChartDataPointPr mParentPr;

        // WORDSNET-12521
        private static DmlChartDataPointPr GetDefaults(DocumentBase doc)
        {
            // WORDSNET-24940 The defaults depend on the document version (7.2.2.2 AppVersion (Application Version)).
            bool is2007OrLower = DmlChartUtil.IsMsWord2007OrLower(doc);

            DmlChartDataPointPr defaults = new DmlChartDataPointPr(true);
            defaults.SetProperty(DmlChartDataPointAttr.Index, 0);
            defaults.SetProperty(DmlChartDataPointAttr.Explosion, -1);
            defaults.SetProperty(DmlChartDataPointAttr.InvertIfNegative, !is2007OrLower);
            defaults.SetProperty(DmlChartDataPointAttr.Bubble3D, false);
            defaults.SetProperty(DmlChartDataPointAttr.Marker, null);
            defaults.SetProperty(DmlChartDataPointAttr.SpPr, null);
            defaults.SetProperty(DmlChartDataPointAttr.PictureOptions, null);
            defaults.SetProperty(DmlChartDataPointAttr.Extensions, null);
            return defaults;
        }

        private class DmlChartDataPointPrParentBagProvider : IDmlHierarchicalPropertyBagParentProvider
        {
            public DmlChartDataPointPrParentBagProvider(DmlChartDataPointPr parentPr)
            {
                mParentPr = parentPr;
            }

            public IDmlHierarchicalPropertyBag ParentBag
            {
                get { return (mParentPr != null) ? mParentPr.mPropertyBag : null; }
            }

            public IDmlHierarchicalPropertyBagParentProvider Clone()
            {
                DmlChartDataPointPrParentBagProvider clone = (DmlChartDataPointPrParentBagProvider)MemberwiseClone();
                if (mParentPr != null)
                    clone.mParentPr = mParentPr.Clone();
                return clone;
            }

            private DmlChartDataPointPr mParentPr;
        }
    }
}
