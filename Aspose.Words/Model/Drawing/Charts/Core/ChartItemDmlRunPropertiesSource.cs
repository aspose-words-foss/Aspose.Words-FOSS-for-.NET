// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/12/2022 by Alexander Zhiltsov

using System.Collections.Generic;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Text;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents a run attribute source for a chart collection item that contains text.
    /// </summary>
    /// <remarks>
    /// Run properties hierarchy is processed for a chart collection item in the following way (in terms of
    /// IChartItemTextProperties property names):
    /// 1. If ItemTx exists and has a non-null <see cref="DmlChartTx.RichText"/>, its first run properties or the
    /// default run properties of its first paragraph are used to get the value of the property. If the property
    /// is not defined, if ItemTxPr is null or empty, and ItemSpPr is null, CollectionTxPr is used first then
    /// <see cref="DmlChartSpace.TxPr"/> and the default value. But if ItemTxPr is not empty or ItemSpPr is not null,
    /// CollectionTxPr and <see cref="DmlChartSpace.TxPr"/> are not taken into account, other default values are used
    /// only. These defaults are very different from the other defaults: for example, the default font is Arial at
    /// 18pt, while in the all other cases it is Calibri at 10pt.
    /// When you set a property, it is set in all runs and default run properties of all paragraphs.
    /// 2. If ItemTx is null or its <see cref="DmlChartTx.RichText"/> is null, ItemTxPr is checked. If ItemTxPr is
    /// not empty or ItemSpPr is not null, the following sequence is used to get the property value: ItemTxPr,
    /// <see cref="DmlChartSpace.TxPr"/>, the default value.
    /// 3. If ItemTxPr is null or empty and ItemSpPr is null, CollectionTxPr is used to find the value of the property.
    /// Then <see cref="DmlChartSpace.TxPr"/> and the defaults are taken into consideration.
    /// </remarks>
    internal class ChartItemDmlRunPropertiesSource : ChartDmlRunPropertiesSourceBase
    {
        /// <summary>
        /// Ctor to create an instance of this class.
        /// </summary>
        internal ChartItemDmlRunPropertiesSource(IChartItemTextProperties chartItemTextProperties, DmlChartSpace chartSpace)
            : base(chartSpace)
        {
            mChartItemTextProperties = chartItemTextProperties;
        }

        /// <summary>
        /// Clears all properties of all instances of <see cref="DmlRunProperties"/> related to the current chart
        /// collection item.
        /// </summary>
        protected override void ClearAllRunProperties()
        {
            // IRunAttrSource.ClearRunAttrs/Font.ClearFormatting must make the run properties have default values,
            // that is, values defined in the parent collection.

            base.ClearAllRunProperties();

            // Don't do anything else for a single (non-collection) item.
            if (CollectionTxPr == null)
                return;

            // Clear the flag indicating that run properties of the paragraphs are not empty.
            foreach (DmlParagraph paragraph in GetParagraphEnumerable())
                paragraph.Properties.HasDefaultRunProperties = false;

            // For pre-Word 2016 charts, if ItemTxPr has no non-run properties defined, we can just clear it so that
            // CollectionTxPr will be used. But if ItemTxPr, for example, contains some paragraph properties or ItemSpPr
            // exists, we have to copy run properties from CollectionTxPr to ItemTxPr because MS Word doesn't look into
            // CollectionTxPr at this case.
            if (HasItemTxPrOrSpPr && !IsChartEx)
            {
                EnsureGeneralRunPropertiesExist(false);

                foreach (DmlRunProperties runProperties in GetRunPropertiesEnumerable())
                    CollectionTxPr.RunPr.CopyTo(runProperties);

                foreach (DmlParagraph paragraph in GetParagraphEnumerable())
                    paragraph.Properties.HasDefaultRunProperties = true;
            }
        }

        /// <summary>
        /// Fetches the specified property from the parent paragraph, collection or chart space. Run defaults are not
        /// included.
        /// </summary>
        protected override object FetchInheritedRunPropertyValue(int key)
        {
            // If ItemTx is not null and has no a property value, if ItemTxPr or ItemSpPr exists, they are not used,
            // and neither CollectionTxPr nor ChartSpaceTxPr is used to resolve property value (it seems this is a
            // bug of MS Word).
            if (AreItemTxRunPropertiesUsed && HasItemTxPrOrSpPr)
            {
                DmlRunProperties runProperties = ItemTx.FirstParagraph.Properties.DefaultRunProperties;
                if (runProperties == null)
                    return null;

                return (RunPrConverter.IsPropertySpecified(runProperties, key))
                    ? GetPropertyValue(key, runProperties)
                    : null;
            }

            DmlRunProperties parentProperties = null;

            // Get value from parent paragraph in Word 2016 chart.
            if (IsChartEx && (ItemTxPr != null) && RunPrConverter.IsPropertySpecified(ItemTxPr.RunPr, key))
                parentProperties = ItemTxPr.RunPr;

            // Get value from the parent collection. In pre-Word 2016 charts, if ItemTxPr is non empty or ItemSpPr
            // exists, CollectionTxPr is not used to get property value (MS Word behaviour).
            if ((parentProperties == null) &&
                (CollectionTxPr != null) &&
                (!HasItemTxPrOrSpPr || IsChartEx))
            {
                // EndParagraphRunProperties has priority over DefaultRunPr in Word 2016 charts.
                DmlRunProperties paragraphEndProperties = CollectionTxPr.FirstParagraph.EndParagraphRunProperties;
                if (IsChartEx && RunPrConverter.IsPropertySpecified(paragraphEndProperties, key))
                    parentProperties = paragraphEndProperties;
                else if (RunPrConverter.IsPropertySpecified(CollectionTxPr.RunPr, key))
                    parentProperties = CollectionTxPr.RunPr;
            }

            // Get value from the chart space. MS Word doesn't get a value from a chart space in Word 2016 charts.
            if ((parentProperties == null) && !IsChartEx && RunPrConverter.IsPropertySpecified(ChartSpaceTxPr.RunPr, key))
                parentProperties = ChartSpaceTxPr.RunPr;

            if (parentProperties != null)
            {
                object value = GetValueFromParentRunProperties(key, parentProperties);
                return mChartItemTextProperties.GetRelativePropertyValue(key, value);
            }

            return null;
        }

        /// <summary>
        /// Returns the default chart run property value.
        /// </summary>
        protected override object FetchDefaultRunPropertyValue(int key)
        {
            // If ItemTx is not null, has no property value and has no default run properties ('defRPr' does not exist),
            // if ItemTxPr is non-empty or ItemSpPr exists, they aren't used, and neither CollectionTxPr nor ChartSpaceTxPr
            // is used to resolve property value, but "strange" default values are used.
            if (AreItemTxRunPropertiesUsed &&
                ((ItemTx.RichText.Paragraphs.Count == 0) ||
                 !ItemTx.RichText.Paragraphs[0].Properties.HasDefaultRunProperties) &&
                HasItemTxPrOrSpPr)
            {
                return FetchDefaultTxRunPropertyValue(key);
            }

            object value = mChartItemTextProperties.FetchSpecialDefaultRunPropertyValue(key);
            if (value != null)
                return value;

            return base.FetchDefaultRunPropertyValue(key);
        }

        /// <summary>
        /// Gets an enumerable over all instances of <see cref="DmlRunProperties"/> within <see cref="ItemTx"/> or
        /// <see cref="ItemTxPr"/>.
        /// </summary>
        protected override IEnumerable<DmlRunProperties> GetRunPropertiesEnumerable()
        {
            return new DmlRunPropertiesEnumerable(GetParagraphs(), IsChartEx);
        }

        /// <summary>
        /// Gets an enumerable over all instances of <see cref="DmlParagraph"/> within <see cref="ItemTx"/> or
        /// <see cref="ItemTxPr"/>.
        /// </summary>
        protected override IEnumerable<DmlParagraph> GetParagraphEnumerable()
        {
            IList<DmlParagraph> paragraphs = GetParagraphs();
            return (paragraphs != null) ? paragraphs : new List<DmlParagraph>();
        }

        /// <summary>
        /// Generates <see cref="GeneralRunProperties"/> if it is <b>null</b>.
        /// </summary>
        protected override void EnsureGeneralRunPropertiesExist(bool toSetProperty)
        {
            if (AreItemTxRunPropertiesUsed)
                return;

            if (IsChartEx)
            {
                if (ItemTxPr == null)
                    ItemTxPr = new DmlChartTxPr();

                ItemTxPr.EnsureParagraphExists();

                if (ItemTxPr.FirstParagraph.Elements.Count > 0)
                    return;

                // Let's generate TxPr in the similar way as MS Word does.
                DmlParagraph paragraph = (CollectionTxPr != null)
                    ? CollectionTxPr.FirstParagraph
                    : ItemTxPr.FirstParagraph;

                DmlRunProperties runProperties = paragraph.EndParagraphRunProperties.Clone();
                DmlRun run = new DmlRun(runProperties);
                run.Text = mChartItemTextProperties.GenerateItemText();

                ItemTxPr.FirstParagraph.AddElement(run);
            }
            else
            {
                if ((ItemTxPr == null) ||
                    (ItemTxPr.IsEmpty && (CollectionTxPr != null) && !CollectionTxPr.IsEmpty))
                {
                    // Since ItemTxPr is empty, need to clone the parent properties, which are defaults for items:
                    // when ItemTxPr is not empty, CollectionTxPr is not used by MS Word to resolve a property value.
                    ItemTxPr = (CollectionTxPr != null)
                        ? CollectionTxPr.Clone()
                        : new DmlChartTxPr();
                }

                if (toSetProperty)
                    ItemTxPr.EnsureParagraphExists();
            }
        }

        /// <summary>
        /// Gets a list of paragraphs within <see cref="ItemTx"/> or <see cref="ItemTxPr"/>.
        /// </summary>
        private IList<DmlParagraph> GetParagraphs()
        {
            return AreItemTxRunPropertiesUsed
                ? ItemTx.RichText.Paragraphs
                : (ItemTxPr != null) ? ItemTxPr.Paragraphs : null;
        }

        /// <summary>
        /// Returns the default chart run property value for a case when <see cref="ItemTx"/> exists, and either
        /// non-empty <see cref="ItemTxPr"/> or <see cref="ItemSpPr"/> exists too.
        /// </summary>
        private static object FetchDefaultTxRunPropertyValue(int key)
        {
            switch (key)
            {
                case FontAttr.NameAscii:
                    return ComplexFontName.FromName("Arial");
                case FontAttr.Size:
                    return ConvertUtilCore.PointToHalfPoint(18);
                default:
                    return RunPr.FetchDefaultAttr(key);
            }
        }

        /// <summary>
        /// Gets text element data of the collection item.
        /// </summary>
        /// <remarks>
        /// This instance contains a paragraph with text elements: fields and/or runs.
        /// This is always <b>null</b> in Word 2016 charts.
        /// </remarks>
        private DmlChartTx ItemTx
        {
            get { return mChartItemTextProperties.ItemTx; }
        }

        /// <summary>
        /// Gets or sets text properties of the collection item.
        /// </summary>
        /// <remarks>
        /// This <see cref="DmlChartTxPr"/> instance contains a paragraph, which is empty in a pre-Word 2016 chart, and
        /// contains a single run in a Word 2016 chart.
        /// </remarks>
        private DmlChartTxPr ItemTxPr
        {
            get { return mChartItemTextProperties.ItemTxPr; }
            set { mChartItemTextProperties.ItemTxPr = value; }
        }

        /// <summary>
        /// Gets shape properties of the collection item.
        /// </summary>
        private DmlChartSpPr ItemSpPr
        {
            get { return mChartItemTextProperties.ItemSpPr; }
        }

        /// <summary>
        /// Gets text properties of the parent collection.
        /// </summary>
        private DmlChartTxPr CollectionTxPr
        {
            get { return mChartItemTextProperties.CollectionTxPr; }
        }

        /// <summary>
        /// Gets DML run properties that is used to get direct property value.
        /// </summary>
        internal override DmlRunProperties GeneralRunProperties
        {
            get
            {
                // In Word 2016 chart, the first (single) run of a ItemTxPr paragraph is used to get the general run
                // properties.
                // In pre-Word 2016 chart, if ItemTx is defined, its first field or run is used as a general run
                // properties source. If ItemTx is null, ItemTxPr is used, which contains a single empty paragraph;
                // the general run properties are the default run properties of that paragraph.

                DmlParagraph firstParagraph;
                if ((ItemTx == null) || (ItemTx.RichText == null))
                {
                    if (ItemTxPr == null)
                        return null;

                    firstParagraph = ItemTxPr.FirstParagraph;
                }
                else
                {
                    if (ItemTx.FirstParagraph == null)
                        ItemTx.RichText.Paragraphs.Add(new DmlParagraph());

                    firstParagraph = ItemTx.FirstParagraph;
                }

                return (firstParagraph.FirstElement != null)
                    ? firstParagraph.FirstElement.RunProperties
                    : firstParagraph.Properties.DefaultRunProperties;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether <see cref="ItemTx"/> is used to format text of the item.
        /// </summary>
        private bool AreItemTxRunPropertiesUsed
        {
            get { return (ItemTx != null) && (ItemTx.RichText != null); }
        }

        /// <summary>
        /// Gets a flag indicating whether the item has non-empty <see cref="ItemTxPr"/> or <see cref="ItemSpPr"/>.
        /// </summary>
        private bool HasItemTxPrOrSpPr
        {
            get
            {
                return
                    ((ItemTxPr != null) && !ItemTxPr.IsEmpty) ||
                    (ItemSpPr != null);
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IChartItemTextProperties mChartItemTextProperties;
    }
}
