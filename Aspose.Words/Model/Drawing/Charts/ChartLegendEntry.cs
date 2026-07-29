// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2012 by Alexey Noskov

using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents a chart legend entry.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>A legend entry corresponds to a specific chart series or trendline.</p>
    /// <p>The text of the entry is the name of the series or trendline. The text cannot be changed.</p>
    ///
    /// <seealso cref="ChartSeries.LegendEntry"/>
    /// </remarks>
    /// <dev>
    /// Represents 5.7.2.95 legendEntry (Legend Entry) element.
    /// </dev>
    public class ChartLegendEntry : IDmlExtensionListSource, IChartItemTextProperties
    {
        /// <summary>
        /// Ctor for internal use only.
        /// </summary>
        internal ChartLegendEntry()
        {
        }

        internal ChartLegendEntry Clone()
        {
            ChartLegendEntry lhs = (ChartLegendEntry)MemberwiseClone();
            if (mTxPr != null)
                lhs.mTxPr = mTxPr.Clone();

            if (mExtensions != null)
                lhs.mExtensions = DmlExtensionListSource.CloneExtensions(mExtensions);

            lhs.mFont = null;

            return lhs;
        }

        /// <summary>
        /// Sets a <see cref="DmlChartFormat"/> instance that is a parent of this legend entry.
        /// </summary>
        internal void SetParentDmlChartFormat(DmlChartFormat chartFormat)
        {
            mChartFormat = chartFormat;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this entry is hidden in the chart legend.
        /// The default value is <b>false</b>.
        /// </summary>
        /// <remarks>
        /// When a chart legend entry is hidden, it does not affect the corresponding chart series or trendline that
        /// is still displayed on the chart.
        /// </remarks>
        public bool IsHidden
        {
            get { return mIsHidden; }
            set { mIsHidden = value; }
        }

        /// <summary>
        /// Provides access to the font formatting of this legend entry.
        /// </summary>
        public Font Font
        {
            get
            {
                if (mFont == null)
                {
                    IRunAttrSource runAttrSource = new ChartItemDmlRunPropertiesSource(this, mChartFormat.DmlChartSpace);
                    mFont = Font.MakeFont(runAttrSource, mChartFormat.Document);
                }

                return mFont;
            }
        }

        internal int Index
        {
            get { return mIndex; }
            set { mIndex = value; }
        }

        internal DmlChartTxPr TxPr
        {
            get
            {
                if (mTxPr == null)
                    mTxPr = new DmlChartTxPr();

                return mTxPr;
            }
        }

        /// <summary>
        /// Returns a flag indicating whether any properties of this legend entry have values ​​that differ from
        /// the corresponding properties of the parent collection.
        /// </summary>
        internal bool HasNonDefaultFormatting
        {
            get
            {
                return
                    IsHidden ||
                    ((mTxPr != null) && !mTxPr.IsEmpty) ||
                    ((mExtensions != null) && (mExtensions.Count > 0));
            }
        }

        /// <summary>
        /// Represents extLst: a CT_OfficeArtExtensionList ([ISO/IEC29500-1:2012] section A.4.1) element that specifies
        /// the extension list in which all future extensions of element type ext is defined.
        ///  </summary>
        /// <dev>
        /// Explicit implementation hides this from public.
        /// </dev>
        StringToObjDictionary<DmlExtension> IDmlExtensionListSource.Extensions
        {
            get { return mExtensions; }
            set { mExtensions = value; }
        }

        #region IChartItemTextProperties members

        string IChartItemTextProperties.GenerateItemText()
        {
            return null; // Not used for a legend entry.
        }

        object IChartItemTextProperties.FetchSpecialDefaultRunPropertyValue(int key)
        {
            return null; // No special default values.
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
            set { mTxPr = value; }
        }

        DmlChartSpPr IChartItemTextProperties.ItemSpPr
        {
            get { return null; }
        }

        DmlChartTxPr IChartItemTextProperties.CollectionTxPr
        {
            get { return mChartFormat.Legend.TxPr; }
        }

        #endregion

        private bool mIsHidden;
        private int mIndex;
        private DmlChartTxPr mTxPr;
        private Font mFont;
        private StringToObjDictionary<DmlExtension> mExtensions;
        private DmlChartFormat mChartFormat;
    }
}
