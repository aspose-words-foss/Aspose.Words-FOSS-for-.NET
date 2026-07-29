// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/12/2022 by Alexander Zhiltsov

using Aspose.Words.Drawing.Charts.Core.ComplexTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents an interface to work with text properties of a chart collection item that contains text.
    /// </summary>
    internal interface IChartItemTextProperties
    {
        /// <summary>
        /// Generates text to put to <see cref="ItemTxPr"/>.
        /// </summary>
        string GenerateItemText();

        /// <summary>
        /// Allows to fetch a default value of chart run properties whose default values differ from the default values
        /// of other chart elements.
        /// </summary>
        object FetchSpecialDefaultRunPropertyValue(int key);

        /// <summary>
        /// If font size is not specified for a chart title, MS Word uses 1.2 * [font size of chart space] value. This
        /// method is intended to perform such property value calculation.
        /// </summary>
        object GetRelativePropertyValue(int key, object value);

        /// <summary>
        /// Gets text element data of the chart collection item.
        /// </summary>
        /// <remarks>
        /// This is always <b>null</b> in Word 2016 charts.
        /// </remarks>
        DmlChartTx ItemTx { get; }

        /// <summary>
        /// Gets or sets text properties of the chart collection item.
        /// </summary>
        DmlChartTxPr ItemTxPr { get; set; }

        /// <summary>
        /// Gets shape properties of the chart collection item.
        /// </summary>
        DmlChartSpPr ItemSpPr { get; }

        /// <summary>
        /// Gets text properties of the parent chart collection.
        /// </summary>
        DmlChartTxPr CollectionTxPr { get; }
    }
}
