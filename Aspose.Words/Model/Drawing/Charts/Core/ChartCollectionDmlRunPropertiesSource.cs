// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2022 by Alexander Zhiltsov

using System.Collections.Generic;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Text;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents a run attribute source for a chart collection whose items contain text.
    /// </summary>
    internal class ChartCollectionDmlRunPropertiesSource : ChartDmlRunPropertiesSourceBase
    {
        /// <summary>
        /// Ctor to create an instance of this class.
        /// </summary>
        internal ChartCollectionDmlRunPropertiesSource(DmlChartTxPr txPr, DmlChartSpace chartSpace)
            : base(chartSpace)
        {
            mTxPr = txPr;
        }

        /// <summary>
        /// Fetches the specified property from the parent paragraph or chart space. Run defaults are not returned.
        /// </summary>
        protected override object FetchInheritedRunPropertyValue(int key)
        {
            if (IsChartEx)
            {
                // Get the value from the parent paragraph.
                // For pre-Word 2016 charts, GetDirectRunAttr uses default run properties of the parent paragraph
                // itself to get the value. So it's omitted for pre-Word 2016 charts.
                if (RunPrConverter.IsPropertySpecified(mTxPr.RunPr, key))
                    return GetValueFromParentRunProperties(key, mTxPr.RunPr);
            }
            else
            {
                // Get the value from the chart space. A chart space is not used in Word 2016 charts.
                if (RunPrConverter.IsPropertySpecified(ChartSpaceTxPr.RunPr, key))
                    return GetValueFromParentRunProperties(key, ChartSpaceTxPr.RunPr);
            }

            return null;
        }

        /// <summary>
        /// Gets an enumerable over all instances of <see cref="DmlRunProperties"/> within <see cref="mTxPr"/>.
        /// </summary>
        protected override IEnumerable<DmlRunProperties> GetRunPropertiesEnumerable()
        {
            // Text properties of Word 2016 chart collection are located in paragraph break properties: enumerate them too.
            return new DmlRunPropertiesEnumerable(mTxPr.Paragraphs, IsChartEx);
        }

        /// <summary>
        /// Gets an enumerable over all instances of <see cref="DmlParagraph"/> within <see cref="mTxPr"/>.
        /// </summary>
        protected override IEnumerable<DmlParagraph> GetParagraphEnumerable()
        {
            return mTxPr.Paragraphs;
        }

        /// <summary>
        /// Generates <see cref="GeneralRunProperties"/> if it is <b>null</b>.
        /// </summary>
        protected override void EnsureGeneralRunPropertiesExist(bool toSetProperty)
        {
            mTxPr.EnsureParagraphExists();
            Debug.Assert(GeneralRunProperties != null);
        }

        /// <summary>
        /// Gets DML run properties that is used to get direct property value.
        /// </summary>
        internal override DmlRunProperties GeneralRunProperties
        {
            get
            {
                // In Word 2016 charts, text properties are taken from paragraph break properties, but from paragraph
                // default run properties in the older chart types.
                return IsChartEx
                    ? mTxPr.FirstParagraph.EndParagraphRunProperties
                    : mTxPr.RunPr;
            }
        }

        private readonly DmlChartTxPr mTxPr;
    }
}
