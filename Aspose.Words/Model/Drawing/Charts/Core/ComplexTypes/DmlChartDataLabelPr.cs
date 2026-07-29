// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/06/2012 by Alexey Noskov

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Text;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Container of data label properties.
    /// </summary>
    internal class DmlChartDataLabelPr
    {
        private DmlChartDataLabelPr() : this(null)
        {
        }

        internal DmlChartDataLabelPr(DmlChart chart) : this(null, chart)
        {
        }
      
        internal DmlChartDataLabelPr(DmlChartDataLabelPr parentBag , DmlChart chart) : 
            this(parentBag, ((chart == null) ? null : chart.Document),
                ((chart != null) && (chart.ChartType == DmlChartType.ChartExChart)))
        {
        }

        internal DmlChartDataLabelPr(DmlChartDataLabelPr parentBag, DocumentBase doc, bool isChartEx)
        {
            DmlChartDataLabelPr parentPr = (parentBag == null) ? GetDefaults(doc, isChartEx) : parentBag;
            mPropertyBag.ParentBagProvider = new DmlChartDataLabelPrParentBagProvider(parentPr);
        }

        internal DmlChartDataLabelPr Clone()
        {
            DmlChartDataLabelPr lhs = (DmlChartDataLabelPr)MemberwiseClone();
            lhs.mPropertyBag = mPropertyBag.Clone();

            DmlChartManualLayout layout = (DmlChartManualLayout)GetDirectProperty(DmlChartDataLabelAttrs.Layout);
            if (layout != null)
                lhs.SetProperty(DmlChartDataLabelAttrs.Layout, layout.Clone());

            DmlChartNumFormat format = (DmlChartNumFormat)GetDirectProperty(DmlChartDataLabelAttrs.NumFmt);
            if (format != null)
                lhs.SetProperty(DmlChartDataLabelAttrs.NumFmt, format.Clone());

            DmlChartSpPr spPr = (DmlChartSpPr)GetDirectProperty(DmlChartDataLabelAttrs.SpPr);
            if (spPr != null)
                lhs.SetProperty(DmlChartDataLabelAttrs.SpPr, spPr.Clone());

            DmlChartTx tx = (DmlChartTx)GetDirectProperty(DmlChartDataLabelAttrs.Tx);
            if (tx != null)
                lhs.SetProperty(DmlChartDataLabelAttrs.Tx, tx.Clone());

            DmlChartTxPr txPr = (DmlChartTxPr)GetDirectProperty(DmlChartDataLabelAttrs.TxPr);
            if (txPr != null)
                lhs.SetProperty(DmlChartDataLabelAttrs.TxPr, txPr.Clone());

            DmlChartSpPr leaderLines = (DmlChartSpPr)GetDirectProperty(DmlChartDataLabelAttrs.LeaderLines);
            if (leaderLines != null)
                lhs.SetProperty(DmlChartDataLabelAttrs.LeaderLines, leaderLines.Clone());

            StringToObjDictionary<DmlExtension> extensions = 
                (StringToObjDictionary<DmlExtension>)GetDirectProperty(DmlChartDataLabelAttrs.Extensions);
            if (extensions != null)
                lhs.SetProperty(DmlChartDataLabelAttrs.Extensions, DmlExtensionListSource.CloneExtensions(extensions));

            return lhs;
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
        /// Returns a separator.
        /// </summary>
        /// <remarks>
        /// If the separator is not specified, returns the default separator. In case of using a pie chart,
        /// the default separator is a new line.
        /// </remarks>
        internal string GetSeparator(bool isPieChart)
        {
            // WORDSNET-12467 According to specification there is an exception, when in pie chart 
            // category name and percentage value are displayed, new line is used as default separator,
            // in all other cases coma is used.
            // WORDSNET-16520 If a pie chart is used, and a separator is directly specified, the specified separator
            // must be used. The separator can be specified in the dLbls element or directly in the data label. 
            if (isPieChart &&
                ShowCategoryAndPercentageOnly && 
                !IsPropertySpecified(DmlChartDataLabelAttrs.Separator) && 
                !IsParentPropertySpecified(DmlChartDataLabelAttrs.Separator))
                return ControlChar.CrLf;

            return (string)GetProperty(DmlChartDataLabelAttrs.Separator);
        }

        internal void SetProperty(DmlChartDataLabelAttrs attr, object value)
        {
            mPropertyBag.SetProperty((int)attr, value);

            // Set extension properties.
            if ((attr == DmlChartDataLabelAttrs.Extensions) && (value != null))
            {
                DmlExtension extension = ((StringToObjDictionary<DmlExtension>)value)[DmlExtensionUri.DataLabels];
                if ((extension != null) && (extension.DataLabelPr != null))
                    mPropertyBag.ExtensionProperties = extension.DataLabelPr.mPropertyBag;
            }
        }

        /// <summary>
        /// Sets the property to the specified value if the property is not defined, i.e. if parent value is used.
        /// </summary>
        internal void SetPropertyIfNotDefined(DmlChartDataLabelAttrs attribute, object value)
        {
            if (!mPropertyBag.IsPropertySpecified((int)attribute))
                SetProperty(attribute, value);
        }

        internal object GetProperty(DmlChartDataLabelAttrs attr)
        {
            return mPropertyBag.GetProperty((int)attr);
        }

        internal object GetDirectProperty(DmlChartDataLabelAttrs attr)
        {
            return mPropertyBag.GetDirectProperty((int)attr);
        }

        /// <summary>
        /// Returns property from extension properties.
        /// </summary>
        /// <param name="attr">The specified property</param>
        /// <returns>Specified property, if extension is set and extension properties is specified. Otherwise - null</returns>
        internal object GetExtensionProperty(DmlChartDataLabelAttrs attr)
        {
            return (mPropertyBag.ExtensionProperties != null) 
                ? mPropertyBag.ExtensionProperties.GetDirectProperty((int)attr) 
                : null;
        }

        /// <summary>
        /// Sets value of the specified extension property.
        /// </summary>
        internal void SetExtensionProperty(DmlChartDataLabelAttrs attr, object value)
        {
            CreateExtensionPropertyBagIfNone();
            mPropertyBag.ExtensionProperties.SetProperty((int)attr, value);
        }

        /// <summary>
        /// Removes the specified property.
        /// </summary>
        internal void RemoveProperty(DmlChartDataLabelAttrs attr)
        {
            mPropertyBag.Remove((int)attr);
        }

        /// <summary>
        /// Removes the specified extension property.
        /// </summary>
        internal void RemoveExtensionProperty(DmlChartDataLabelAttrs attr)
        {
            if (mPropertyBag.ExtensionProperties != null)
                mPropertyBag.ExtensionProperties.Remove((int)attr);
        }

        /// <summary>
        /// Creates uniqueId extension of a label if it does not exist yet.
        /// </summary>
        internal void CreateUniqueIdExtensionIfNone()
        {
            StringToObjDictionary<DmlExtension> extensions = GetOrCreateExtensions();
            DmlExtension extension = DmlExtension.GetOrCreateExtension(extensions, DmlExtensionUri.UniqueId);

            if (extension.DataLabelId == Guid.Empty)
            {
                object index = GetProperty(DmlChartDataLabelAttrs.Idx);
                if (index == null)
                    index = 0;

                extension.DataLabelId = RandomUtil.NewGuid(52731 + (int)index);
            }
        }

        /// <summary>
        /// Determines whether the mPropertyBag contains the specified property, which is set directly.
        /// </summary>
        /// <param name="attr">The specified property</param>
        /// <returns>"True", if the property is set directly, "false" otherwise</returns>
        internal bool IsPropertySpecified(DmlChartDataLabelAttrs attr)
        {
            return mPropertyBag.IsPropertySpecified((int)attr);
        }

        /// <summary>
        /// Checks if the specified property is set directly in the parent bag provider.
        /// </summary>
        /// <param name="attr">The specified property</param>
        /// <returns>"True", if the property is set directly in the parent bag provider, "false" otherwise</returns>
        internal bool IsParentPropertySpecified(DmlChartDataLabelAttrs attr)
        {
            if (mPropertyBag.ParentBagProvider == null)
                return false;

            if (IsPropertySpecified(attr))
                return true;

            return mPropertyBag.ParentBagProvider.ParentBag.IsPropertySpecified((int)attr);
        }

        /// <summary>
        /// Gets values of the specified attributes from the parent property bag and puts them into the current bag if
        /// the attributes are not defined directly.
        /// </summary>
        internal void ExpandParentProperties(DmlChartDataLabelAttrs[] attributes)
        {
            foreach (DmlChartDataLabelAttrs attribute in attributes)
            {
                if (GetDirectProperty(attribute) == null)
                    SetProperty(attribute, GetProperty(attribute));
            }
        }

        /// <summary>
        /// Gets values of the specified attributes from the parent property bag and puts them into the current extension
        /// bag if the attributes are not defined directly.
        /// </summary>
        internal void ExpandParentExtensionProperties(DmlChartDataLabelAttrs[] attributes)
        {
            foreach (DmlChartDataLabelAttrs attribute in attributes)
            {
                if (GetExtensionProperty(attribute) == null)
                {
                    object parentValue = GetProperty(attribute);
                    if (parentValue != null)
                        SetExtensionProperty(attribute, parentValue);
                }
            }
        }

        /// <summary>
        /// Removes the specified attributes from the property bag if their values are the same as in the parent bag.
        /// </summary>
        internal void CollapseParentProperties(DmlChartDataLabelAttrs[] attributes)
        {
            foreach (DmlChartDataLabelAttrs attribute in attributes)
            {
                object value = GetDirectProperty(attribute);
                if (value != null)
                {
                    object parentValue = mPropertyBag.ParentBagProvider.ParentBag.GetProperty((int)attribute);
                    if ((parentValue != null) && parentValue.Equals(value))
                        mPropertyBag.Remove((int)attribute);
                }
            }
        }

        /// <summary>
        /// Generates default value for the property <see cref="DmlChartDataLabelAttrs.Tx"/>, if it is not defined yet,
        /// to display information specified by the ShowXXX properties. MS Word needs this property written when the
        /// <see cref="DmlChartDataLabelAttrs.ShowDataLabelsRange"/> extension property is set to On.
        /// </summary>
        internal void GenerateTx(bool isCategoryAbscissa)
        {
            if (GetDirectProperty(DmlChartDataLabelAttrs.Tx) != null)
                return;

            DmlChartTx tx = new DmlChartTx();
            SetProperty(DmlChartDataLabelAttrs.Tx, tx);

            DmlTextBody reachText = new DmlTextBody();
            tx.RichText = reachText;

            DmlChartTxPr txPr = (DmlChartTxPr)GetProperty(DmlChartDataLabelAttrs.TxPr);
            if (txPr != null)
            {
                reachText.Properties = txPr.BodyPr.Clone();
                reachText.TextListStyles = txPr.LstStyle.Clone();
                if (txPr.Paragraphs.Count > 0)
                    reachText.Paragraphs.Add(txPr.Paragraphs[0].Clone());
            }

            if (reachText.Paragraphs.Count == 0)
                reachText.Paragraphs.Add(new DmlParagraph());

            DmlParagraph paragraph = reachText.Paragraphs[0];

            string separator = (string)GetProperty(DmlChartDataLabelAttrs.Separator) + " ";

            if ((bool)GetProperty(DmlChartDataLabelAttrs.ShowDataLabelsRange))
                AddDmlField(paragraph, CellRangeFieldType, CellRangeFieldCode, separator);

            if ((bool)GetProperty(DmlChartDataLabelAttrs.ShowSerName))
                AddDmlField(paragraph, SeriesFieldType, SeriesFieldCode, separator);

            if ((bool)GetProperty(DmlChartDataLabelAttrs.ShowCatName))
            {
                if (isCategoryAbscissa)
                    AddDmlField(paragraph, CategoryFieldType, CategoryFieldCode, separator);
                else
                    AddDmlField(paragraph, XValueFieldType, XValueFieldCode, separator);
            }

            if ((bool)GetProperty(DmlChartDataLabelAttrs.ShowVal))
                AddDmlField(paragraph, ValueFieldType, ValueFieldCode, separator);

            if ((bool)GetProperty(DmlChartDataLabelAttrs.ShowBubbleSize))
                AddDmlField(paragraph, BubbleSizeFieldType, BubbleSizeFieldCode, separator);

            if ((bool)GetProperty(DmlChartDataLabelAttrs.ShowPercent))
                AddDmlField(paragraph, PercentageFieldType, PercentageFieldCode, separator);
        }

        /// <summary>
        /// Creates a <see cref="DmlTextField"/> with the specified property values and adds it into the paragraph.
        /// A separator run is also created if the paragraph already contains children.
        /// </summary>
        private static void AddDmlField(DmlParagraph paragraph, string type, string text, string separator)
        {
            if (paragraph.Elements.Count > 0)
            {
                DmlRun run = new DmlRun();
                run.SetParagraph(paragraph);
                paragraph.Elements.Add(run);
                run.Text = separator;
            }

            DmlTextField field = new DmlTextField();
            field.SetParagraph(paragraph);
            paragraph.Elements.Add(field);

            field.Type = type;
            field.Text = text;
            field.Id = RandomUtil.NewGuid(paragraph.Elements.Count);
        }

        /// <summary>
        /// Creates a property bag for extension properties if it does not exist yet.
        /// </summary>
        private void CreateExtensionPropertyBagIfNone()
        {
            if (mPropertyBag.ExtensionProperties != null)
                return;

            StringToObjDictionary<DmlExtension> extensions = GetOrCreateExtensions();

            DmlExtension extension = new DmlExtension(DmlExtensionUri.DataLabels, null);
            extensions[DmlExtensionUri.DataLabels] = extension;
            extension.DataLabelPr = new DmlChartDataLabelPr();
            mPropertyBag.ExtensionProperties = extension.DataLabelPr.mPropertyBag;
        }

        /// <summary>
        /// Gets an extensions dictionary of this label properties collection. Creates the dictionary if it does not
        /// exist yet.
        /// </summary>
        private StringToObjDictionary<DmlExtension> GetOrCreateExtensions()
        {
            StringToObjDictionary<DmlExtension> extensions =
                (StringToObjDictionary<DmlExtension>)GetDirectProperty(DmlChartDataLabelAttrs.Extensions);

            if (extensions == null)
            {
                extensions = new StringToObjDictionary<DmlExtension>();
                mPropertyBag.SetProperty((int)DmlChartDataLabelAttrs.Extensions, extensions);
            }

            return extensions;
        }

        /// <summary>
        /// If ShowDataLabelsRange is On, MS Word generates and writes Tx object with fields for ShowXXX properties.
        /// This method returns <c>true</c> if the specified object is such default Tx generated by Word.
        /// </summary>
        private bool IsDefaultTx(DmlChartTx tx)
        {
            DmlTextBody reachText = tx.RichText;
            if ((tx.TxType != DmlChartTxType.RichText) ||
                (tx.StrRef != null) ||
                (tx.Formula != null) ||
                !reachText.Properties.IsEmpty ||
                !reachText.TextListStyles.IsEmpty ||
                reachText.Paragraphs.Count != 1)
                return false;

            string separator = (string)GetProperty(DmlChartDataLabelAttrs.Separator) + " ";
            DmlParagraph paragraph = reachText.Paragraphs[0];
            int runIndex = 0;
            
            if ((bool)GetProperty(DmlChartDataLabelAttrs.ShowDataLabelsRange))
            {
                if (!CheckTxRun(paragraph.Elements, runIndex, CellRangeFieldType, CellRangeFieldCode, separator))
                    return false;
                runIndex += (runIndex == 0) ? 1 : 2;
            }

            if ((bool)GetProperty(DmlChartDataLabelAttrs.ShowSerName))
            {
                if (!CheckTxRun(paragraph.Elements, runIndex, SeriesFieldType, SeriesFieldCode, separator))
                    return false;
                runIndex += (runIndex == 0) ? 1 : 2;
            }

            if ((bool)GetProperty(DmlChartDataLabelAttrs.ShowCatName))
            {
                if (!CheckTxRun(paragraph.Elements, runIndex, XValueFieldType, XValueFieldCode, separator) &&
                    !CheckTxRun(paragraph.Elements, runIndex, CategoryFieldType, CategoryFieldCode, separator))
                    return false;
                runIndex += (runIndex == 0) ? 1 : 2;
            }

            if ((bool)GetProperty(DmlChartDataLabelAttrs.ShowVal))
            {
                if (!CheckTxRun(paragraph.Elements, runIndex, ValueFieldType, ValueFieldCode, separator))
                    return false;
                runIndex += (runIndex == 0) ? 1 : 2;
            }

            if ((bool)GetProperty(DmlChartDataLabelAttrs.ShowBubbleSize))
            {
                if (!CheckTxRun(paragraph.Elements, runIndex, BubbleSizeFieldType, BubbleSizeFieldCode, separator))
                    return false;
                runIndex += (runIndex == 0) ? 1 : 2;
            }

            if ((bool)GetProperty(DmlChartDataLabelAttrs.ShowPercent))
            {
                if (!CheckTxRun(paragraph.Elements, runIndex, PercentageFieldType, PercentageFieldCode, separator))
                    return false;
                runIndex += (runIndex == 0) ? 1 : 2;
            }

            return paragraph.Elements.Count == runIndex;
        }

        /// <summary>
        /// Checks that the paragraph element list contains a <see cref="DmlTextField"/> with a separator before if
        /// necessary at the specified position with the specified property values.
        /// </summary>
        private bool CheckTxRun(IList<DmlParagraphTextElementBase> elements, int runIndex,
            string expectedFieldType, string expectedFieldText, string separator)
        {
            if (elements.Count <= runIndex)
                return false;

            // Only the first field has no separator before it. Subsequent ones have.
            if (runIndex > 0)
            {
                DmlRun run = elements[runIndex] as DmlRun;
                if ((run == null) || HasRunProperties(run) || (run.Text != separator))
                    return false;

                runIndex++;
                if (elements.Count <= runIndex)
                    return false;
            }

            DmlTextField field = elements[runIndex] as DmlTextField;
            return
                (field != null) &&
                (field.Type == expectedFieldType) &&
                (field.Text == expectedFieldText) &&
                !HasRunProperties(field);
        }

        /// <summary>
        /// Returns <c>true</c> if the specified DML paragraph element has non-default run formatting defined.
        /// </summary>
        private static bool HasRunProperties(DmlParagraphTextElementBase element)
        {
            DmlRunProperties properties = element.RunProperties;
            return 
                ((properties.Effects != null) && (properties.Effects.Count > 0)) ||
                ((properties.Extensions != null) && (properties.Extensions.Count > 0)) ||
                // Word may write these properties into chart XML, let's just ignore them for now.
                properties.HasNonDefaultFormatting(
                    new [] { (int)DmlRunPropertiesIds.Baseline, (int)DmlRunPropertiesIds.Language }) ||
                !MathUtil.AreEqual(properties.Baseline, 0);
        }

        /// <summary>
        /// Gets defaults depending on the document version.
        /// </summary>
        /// <param name="doc">The specified <see cref="DocumentBase"/></param>
        /// <param name="isChartEx">Indicates that the parent chart is of the
        /// http://schemas.microsoft.com/office/drawing/2014/chartex schema [MS-ODRAWXML]. Charts of the schema are
        /// introduced in MS Office 2016.</param>
        /// <returns>The defaults for data label properties</returns>
        private static DmlChartDataLabelPr GetDefaults(DocumentBase doc, bool isChartEx)
        {
            if (isChartEx)
                return gChartExDefaults;

            // WORDSNET-20873 The defaults depend on the document version (7.2.2.2 AppVersion (Application Version)). 
            // The version in CompatibilityOptions does not affect the defaults.
            return DmlChartUtil.IsMsWord2007OrLower(doc) ? gDefaults : gDefaultsAfter2007;
        }

        /// <summary>
        /// Returns true if only category name and percentage value are displayed by this data label properties.
        /// Special rules must be applied in this case for Pie charts.
        /// </summary>
        private bool ShowCategoryAndPercentageOnly
        {
            get
            {
                return
                    (bool)GetProperty(DmlChartDataLabelAttrs.ShowCatName) &&
                    (bool)GetProperty(DmlChartDataLabelAttrs.ShowPercent) &&
                    !(bool)GetProperty(DmlChartDataLabelAttrs.ShowBubbleSize) &&
                    !(bool)GetProperty(DmlChartDataLabelAttrs.ShowSerName) &&
                    !(bool)GetProperty(DmlChartDataLabelAttrs.ShowVal);
            }
        }

        internal int Count
        {
            get { return mPropertyBag.Count; }
        }

        /// <summary>
        /// Returns <c>true</c> if this property collection redefines any properties of the parent collection.
        /// </summary>
        /// <dev>
        /// Now the property may not work correctly for complex attributes Tx and TxPr since the corresponding classes
        /// do not override the <see cref="object.Equals(object)"/> method.
        /// </dev>
        internal bool HasNonDefaultFormatting
        {
            get
            {
                IntList attributesToIgnore = new IntList();

                attributesToIgnore.Add((int)DmlChartDataLabelAttrs.Idx); // ignore since each label has such property defined
                attributesToIgnore.Add((int)DmlChartDataLabelAttrs.Tx); // checked separately below
                attributesToIgnore.Add((int)DmlChartDataLabelAttrs.Extensions); // checked separately below

                // Ignore empty leader lines properties.
                DmlChartSpPr leaderLines = (DmlChartSpPr)GetDirectProperty(DmlChartDataLabelAttrs.LeaderLines);
                if ((leaderLines != null) && leaderLines.IsEmpty)
                    attributesToIgnore.Add((int)DmlChartDataLabelAttrs.LeaderLines);

                // Ignore empty SpPr properties.
                DmlChartSpPr spPr = (DmlChartSpPr)GetDirectProperty(DmlChartDataLabelAttrs.SpPr);
                if ((spPr != null) && spPr.IsEmpty)
                    attributesToIgnore.Add((int)DmlChartDataLabelAttrs.SpPr);

                // Ignore empty TxPr properties.
                DmlChartTxPr txPr = (DmlChartTxPr)GetDirectProperty(DmlChartDataLabelAttrs.TxPr);
                if ((txPr != null) && txPr.IsEmpty)
                    attributesToIgnore.Add((int)DmlChartDataLabelAttrs.TxPr);

                if (mPropertyBag.HasNonDefaultFormatting(attributesToIgnore.ToArray(), null))
                    return true;

                // If ShowDataLabelsRange is ON, MS Word generates and writes Tx object with fields for each ShowXXX
                // property set to On. Let's recognize such Tx values and treat as default formatting.
                DmlChartTx tx = (DmlChartTx)GetDirectProperty(DmlChartDataLabelAttrs.Tx);
                if (tx != null)
                {
                    // It seems Word generates Tx only if ShowDataLabelsRange is On.
                    if (!(bool)GetProperty(DmlChartDataLabelAttrs.ShowDataLabelsRange))
                        return true;

                    // If Tx is defined in parent, this Tx may override it, at this case the collection has non-default
                    // formatting.
                    object parentTx =
                        mPropertyBag.ParentBagProvider.ParentBag.GetDirectProperty((int)DmlChartDataLabelAttrs.Tx);
                    if ((parentTx != null) && !tx.Equals(parentTx)) // Now Equals is not defined, it checks references only.
                        return true;

                    if (!IsDefaultTx(tx))
                        return true;
                }

                IDmlHierarchicalPropertyBag extensionProperties = mPropertyBag.ExtensionProperties;
                IDmlHierarchicalPropertyBag parentExtensionProperties =
                    mPropertyBag.ParentBagProvider.ParentBag.ExtensionProperties;
                if ((extensionProperties != null) && 
                    extensionProperties.HasNonDefaultFormatting(gExtensionAttributesToIgnore, parentExtensionProperties))
                    return true;

                StringToObjDictionary<DmlExtension> extensions =
                    (StringToObjDictionary<DmlExtension>)GetDirectProperty(DmlChartDataLabelAttrs.Extensions);

                if (extensions != null)
                {
                    foreach (string uri in extensions.Keys)
                    {
                        // Ignore UniqueId since it seems it has no any useful information.
                        // Ignore DataLabels since it is mPropertyBag.ExtensionProperties: already compared.
                        // If another extension exists, just consider that the extensions are different (no way to
                        // compare them now).
                        if (!StringUtil.EqualsOrdinalIgnoreCase(uri, DmlExtensionUri.DataLabels) &&
                            !StringUtil.EqualsOrdinalIgnoreCase(uri, DmlExtensionUri.UniqueId))
                            return true;
                    }
                }

                return false;
            }
        }

        private IDmlHierarchicalPropertyBag mPropertyBag = new DmlHierarchicalPropertyBag();

        static DmlChartDataLabelPr()
        {
            gDefaults = GetDefaults();
            gDefaultsAfter2007 = GetDefaultsAfter2007();
            gChartExDefaults = GetChartExDefaults();
        }

        /// <summary>
        /// Gets defaults for data labels if the version of the MS Word is higher than Word2007.
        /// </summary>
        /// <remarks>
        /// MS Word renders the series name, category name, value , bubble size (only if chart type is "BubbleChart"), 
        /// percentages (only if chart type is "PieChart") if data labels are specified for the chart or for the series and 
        /// properties are not set directly and the document version is higher than "12.0000". 
        /// </remarks>
        private static DmlChartDataLabelPr GetDefaultsAfter2007()
        {
            DmlChartDataLabelPr defaults = GetDefaults();

            defaults.SetProperty(DmlChartDataLabelAttrs.ShowBubbleSize, true);
            defaults.SetProperty(DmlChartDataLabelAttrs.ShowCatName, true);
            defaults.SetProperty(DmlChartDataLabelAttrs.ShowLegendKey, true);
            defaults.SetProperty(DmlChartDataLabelAttrs.ShowPercent, true);
            defaults.SetProperty(DmlChartDataLabelAttrs.ShowSerName, true);
            defaults.SetProperty(DmlChartDataLabelAttrs.ShowVal, true);

            return defaults;
        }

        /// <summary>
        /// Gets defaults for data labels of new chart types of Word 2016.
        /// </summary>
        private static DmlChartDataLabelPr GetChartExDefaults()
        {
            DmlChartDataLabelPr defaults = GetDefaults();

            defaults.SetProperty(DmlChartDataLabelAttrs.ShowVal, true);

            return defaults;
        }

        private static DmlChartDataLabelPr GetDefaults()
        {
            DmlChartDataLabelPr defaults = new DmlChartDataLabelPr();

            defaults.SetProperty(DmlChartDataLabelAttrs.Delete, false);
            defaults.SetProperty(DmlChartDataLabelAttrs.DLblPos, DefaultPosition);
            defaults.SetProperty(DmlChartDataLabelAttrs.Idx, 0);
            defaults.SetProperty(DmlChartDataLabelAttrs.Layout, null);
            defaults.SetProperty(DmlChartDataLabelAttrs.NumFmt, null);
            defaults.SetProperty(DmlChartDataLabelAttrs.Separator, ",");
            defaults.SetProperty(DmlChartDataLabelAttrs.ShowBubbleSize, false);
            defaults.SetProperty(DmlChartDataLabelAttrs.ShowCatName, false);
            defaults.SetProperty(DmlChartDataLabelAttrs.ShowLegendKey, false);
            defaults.SetProperty(DmlChartDataLabelAttrs.ShowPercent, false);
            defaults.SetProperty(DmlChartDataLabelAttrs.ShowSerName, false);
            defaults.SetProperty(DmlChartDataLabelAttrs.ShowVal, false);
            defaults.SetProperty(DmlChartDataLabelAttrs.ShowLeaderLines, false);
            defaults.SetProperty(DmlChartDataLabelAttrs.ShowDataLabelsRange, false);
            defaults.SetProperty(DmlChartDataLabelAttrs.SpPr, null);
            defaults.SetProperty(DmlChartDataLabelAttrs.Tx, null);
            defaults.SetProperty(DmlChartDataLabelAttrs.TxPr, null);
            defaults.SetProperty(DmlChartDataLabelAttrs.LeaderLines, null);
            defaults.SetProperty(DmlChartDataLabelAttrs.XForSave, false);
            defaults.SetProperty(DmlChartDataLabelAttrs.Extensions, null);

            return defaults;
        }

        private static readonly DmlChartDataLabelPr gDefaultsAfter2007;
        private static readonly DmlChartDataLabelPr gChartExDefaults;
        private static readonly DmlChartDataLabelPr gDefaults;

        private static readonly int[] gExtensionAttributesToIgnore = { (int)DmlChartDataLabelAttrs.XForSave };

        private class DmlChartDataLabelPrParentBagProvider : IDmlHierarchicalPropertyBagParentProvider
        {
            internal DmlChartDataLabelPrParentBagProvider(DmlChartDataLabelPr parentPr)
            {
                mParentPr = parentPr;
            }

            public IDmlHierarchicalPropertyBag ParentBag
            {
                get { return (mParentPr != null) ? mParentPr.mPropertyBag : null; }
            }

            public IDmlHierarchicalPropertyBagParentProvider Clone()
            {
                DmlChartDataLabelPrParentBagProvider clone = (DmlChartDataLabelPrParentBagProvider)MemberwiseClone();
                if (mParentPr != null)
                    clone.mParentPr = mParentPr.Clone();
                return clone;
            }

            private DmlChartDataLabelPr mParentPr;
        }

        internal const ChartDataLabelPosition DefaultPosition = (ChartDataLabelPosition)(-1);

        private const string CellRangeFieldType = "CELLRANGE";
        private const string CellRangeFieldCode = "[CELLRANGE]";
        private const string SeriesFieldType = "SERIESNAME";
        private const string SeriesFieldCode = "[SERIES NAME]";
        private const string CategoryFieldType = "CATEGORYNAME";
        private const string CategoryFieldCode = "[CATEGORY NAME]";
        private const string XValueFieldType = "XVALUE";
        private const string XValueFieldCode = "[X VALUE]";
        private const string ValueFieldType = "VALUE";
        private const string ValueFieldCode = "[VALUE]";
        private const string BubbleSizeFieldType = "BUBBLESIZE";
        private const string BubbleSizeFieldCode = "[BUBBLESIZE]";
        private const string PercentageFieldType = "PERCENTAGE";
        private const string PercentageFieldCode = "[PERCENTAGE]";
    }
}
