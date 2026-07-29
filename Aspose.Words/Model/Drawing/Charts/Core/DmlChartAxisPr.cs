// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/07/2016 by Andrey Noskov

using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class contains chart axis properties.
    /// </summary>
    internal class DmlChartAxisPr
    {
        internal DmlChartAxisPr(DocumentBase document, bool isChartEx)
            : this(GetDefaults(document, isChartEx))
        {
        }

        private DmlChartAxisPr()
            : this((DmlChartAxisPr)null)
        {
        }

        private DmlChartAxisPr(DmlChartAxisPr defaults)
        {
            if (defaults != null)
                mPrBag.ParentBagProvider = new DmlChartAxisPrParentBagProvider(defaults);

            // Also all complex types must be present in properties directly.
            mPrBag.SetProperty((int)DmlChartAxisAttrs.SpPr, new DmlChartSpPr());
            mPrBag.SetProperty((int)DmlChartAxisAttrs.TxPr, new DmlChartTxPr());
            // MajorUnit and MinorUnit allows only positive values, let's set default values as in renderer.
            mPrBag.SetProperty((int)DmlChartAxisAttrs.MajorUnit, DoubleOrAutomatic.AsNull(1));
            mPrBag.SetProperty((int)DmlChartAxisAttrs.MinorUnit, DoubleOrAutomatic.AsNull(0.5));
            mPrBag.SetProperty((int)DmlChartAxisAttrs.Scaling, new AxisScaling());
        }

        internal DmlChartAxisPr Clone()
        {
            DmlChartAxisPr lhs = (DmlChartAxisPr)MemberwiseClone();
            lhs.mPrBag = mPrBag.Clone();

            DmlChartSpPr spPr = (DmlChartSpPr)GetProperty(DmlChartAxisAttrs.SpPr);
            lhs.SetProperty(DmlChartAxisAttrs.SpPr, spPr.Clone());

            DmlChartTxPr txPr = (DmlChartTxPr)GetProperty(DmlChartAxisAttrs.TxPr);
            lhs.SetProperty(DmlChartAxisAttrs.TxPr, txPr.Clone());

            DoubleOrAutomatic majorUnit = (DoubleOrAutomatic)GetProperty(DmlChartAxisAttrs.MajorUnit);
            lhs.SetProperty(DmlChartAxisAttrs.MajorUnit, majorUnit.Clone());

            DoubleOrAutomatic minorUnit = (DoubleOrAutomatic)GetProperty(DmlChartAxisAttrs.MinorUnit);
            lhs.SetProperty(DmlChartAxisAttrs.MinorUnit, minorUnit.Clone());

            AxisScaling scaling = (AxisScaling)GetProperty(DmlChartAxisAttrs.Scaling);
            lhs.SetProperty(DmlChartAxisAttrs.Scaling, scaling.Clone());

            DmlChartGridlines majorGridlines = (DmlChartGridlines)GetDirectProperty(DmlChartAxisAttrs.MajorGridlines);
            if (majorGridlines != null)
                lhs.SetProperty(DmlChartAxisAttrs.MajorGridlines, majorGridlines.Clone());

            DmlChartGridlines minorGridlines = (DmlChartGridlines)GetDirectProperty(DmlChartAxisAttrs.MinorGridlines);
            if (minorGridlines != null)
                lhs.SetProperty(DmlChartAxisAttrs.MinorGridlines, minorGridlines.Clone());

            CloneExtensionsAttribute(DmlChartAxisAttrs.TickLblExtensions, lhs);
            CloneExtensionsAttribute(DmlChartAxisAttrs.MajorTickMarkExtensions, lhs);
            CloneExtensionsAttribute(DmlChartAxisAttrs.MinorTickMarkExtensions, lhs);

            AxisDisplayUnit displayUnit = (AxisDisplayUnit)GetDirectProperty(DmlChartAxisAttrs.DisplayUnit);
            if (displayUnit != null)
                lhs.SetProperty(DmlChartAxisAttrs.DisplayUnit, displayUnit.Clone());

            DmlChartNumFormat numFmt = (DmlChartNumFormat)GetDirectProperty(DmlChartAxisAttrs.NumFmt);
            if (numFmt != null)
                lhs.SetProperty(DmlChartAxisAttrs.NumFmt, numFmt.Clone());

            return lhs;
        }

        /// <summary>
        /// Clones the specified extensions attribute.
        /// </summary>
        private void CloneExtensionsAttribute(DmlChartAxisAttrs attribute, DmlChartAxisPr destination)
        {
            StringToObjDictionary<DmlExtension> extensions = (StringToObjDictionary<DmlExtension>)GetDirectProperty(attribute);
            if (extensions != null)
                destination.SetProperty(attribute, DmlExtensionListSource.CloneExtensions(extensions));
        }

        internal object GetProperty(DmlChartAxisAttrs attr)
        {
            return mPrBag.GetProperty((int)attr);
        }

        internal void SetProperty(DmlChartAxisAttrs attr, object value)
        {
            mPrBag.SetProperty((int)attr, value);
        }

        /// <summary>
        /// Returns null if property is not set explicitly
        /// </summary>
        internal object GetDirectProperty(DmlChartAxisAttrs attr)
        {
            return mPrBag.GetDirectProperty((int)attr);
        }

        /// <summary>
        /// Determines whether the mPropertyBag contains the specified property, which was set directly.
        /// </summary>
        /// <param name="attr">the attr</param>
        /// <returns>"true", if the  property was set directly, "false" otherwise</returns>
        internal bool IsPropertySpecified(DmlChartAxisAttrs attr)
        {
            return mPrBag.IsPropertySpecified((int)attr);
        }

        private IDmlHierarchicalPropertyBag mPrBag = new DmlHierarchicalPropertyBag();

        static DmlChartAxisPr()
        {
            gDefaultPr = SetDefaults();
            gDefaultPrAfter2007 = SetDefaultsAfter2007();
            gChartExDefaultPr = SetChartExDefaults();
        }

        /// <summary>
        /// Gets a <seealso cref="DmlChartAxisPr"/> instance that should be used as default properties for
        /// an axis, given the specified document and Word 2016 chart flag.
        /// </summary>
        private static DmlChartAxisPr GetDefaults(DocumentBase document, bool isChartEx)
        {
            if (isChartEx)
                return gChartExDefaultPr;

            return DmlChartUtil.IsMsWord2007OrLower(document) ? gDefaultPr : gDefaultPrAfter2007;
        }

        private static DmlChartAxisPr SetDefaults()
        {
            DmlChartAxisPr defaults = new DmlChartAxisPr();
            // No default value.
            defaults.SetProperty(DmlChartAxisAttrs.AxId, 0);
            defaults.SetProperty(DmlChartAxisAttrs.IsAutoCategoryType, false);
            defaults.SetProperty(DmlChartAxisAttrs.IsDateCategoryAxis, false);
            // No default value. Attribute is required and cannot be omitted.
            defaults.SetProperty(DmlChartAxisAttrs.AxPos, AxisPosition.Bottom);
            defaults.SetProperty(DmlChartAxisAttrs.BaseTimeUnit, AxisTimeUnit.Automatic);
            // It seems MS Word always uses autoZero (Automatic) value. So lets use it as default. 
            defaults.SetProperty(DmlChartAxisAttrs.Crosses, AxisCrosses.Automatic);
            // The property is public, so set it to 0 by default.
            defaults.SetProperty(DmlChartAxisAttrs.CrossesAt, 0d);
            // No default value. Attribute is required.
            defaults.SetProperty(DmlChartAxisAttrs.CrossAx, 0);
            defaults.SetProperty(DmlChartAxisAttrs.CrossBetween, CrossBetween.Between);
            // By default this is false. Is true if 'delete' element is present.
            defaults.SetProperty(DmlChartAxisAttrs.LblAlgn, LabelAlignment.Default);
            // Default is 100.
            defaults.SetProperty(DmlChartAxisAttrs.TickLabelOffset, 100);
            // Default is days.
            defaults.SetProperty(DmlChartAxisAttrs.MajorUnitScale, AxisTimeUnit.Automatic);
            // Default is days.
            defaults.SetProperty(DmlChartAxisAttrs.MinorUnitScale, AxisTimeUnit.Automatic);
            // Tick marks are Cross as default in MS Office starting from the 2010 version.
            defaults.SetProperty(DmlChartAxisAttrs.MajorTickMark, AxisTickMark.Outside);
            defaults.SetProperty(DmlChartAxisAttrs.MinorTickMark, AxisTickMark.None);
            defaults.SetProperty(DmlChartAxisAttrs.NoMultiLvlLbl, false);
            defaults.SetProperty(DmlChartAxisAttrs.TickLblPos, AxisTickLabelPosition.Default);
            defaults.SetProperty(DmlChartAxisAttrs.TickLabelSpacing, 1);
            defaults.SetProperty(DmlChartAxisAttrs.TickLabelSpacingIsAuto, true);
            defaults.SetProperty(DmlChartAxisAttrs.TickMarkSpacing, 1);
            defaults.SetProperty(DmlChartAxisAttrs.Hidden, false);

            return defaults;
        }

        private static DmlChartAxisPr SetDefaultsAfter2007()
        {
            DmlChartAxisPr defaults = SetDefaults();

            // Tick marks are Cross as default in MS Office starting from the 2010 version.
            defaults.SetProperty(DmlChartAxisAttrs.MajorTickMark, AxisTickMark.Cross);
            defaults.SetProperty(DmlChartAxisAttrs.MinorTickMark, AxisTickMark.Cross);

            return defaults;
        }

        private static DmlChartAxisPr SetChartExDefaults()
        {
            DmlChartAxisPr defaults = SetDefaults();

            // Tick marks are None as default in Word 2016 charts.
            defaults.SetProperty(DmlChartAxisAttrs.MajorTickMark, AxisTickMark.None);
            defaults.SetProperty(DmlChartAxisAttrs.MinorTickMark, AxisTickMark.None);

            return defaults;
        }

        private static readonly DmlChartAxisPr gDefaultPr;
        private static readonly DmlChartAxisPr gDefaultPrAfter2007;
        private static readonly DmlChartAxisPr gChartExDefaultPr;

        private class DmlChartAxisPrParentBagProvider : IDmlHierarchicalPropertyBagParentProvider
        {
            internal DmlChartAxisPrParentBagProvider(DmlChartAxisPr parentPr)
            {
                mParentPr = parentPr;
            }

            public IDmlHierarchicalPropertyBag ParentBag
            {
                get { return (mParentPr != null) ? mParentPr.mPrBag : null; }
            }

            public IDmlHierarchicalPropertyBagParentProvider Clone()
            {
                DmlChartAxisPrParentBagProvider lhs = (DmlChartAxisPrParentBagProvider)MemberwiseClone();
                if (mParentPr != null)
                    lhs.mParentPr = mParentPr.Clone();
                return lhs;
            }

            private DmlChartAxisPr mParentPr;
        }
    }
}
