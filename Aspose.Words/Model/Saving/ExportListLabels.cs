// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/11/2011 by Andrey Kamimura

using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how list labels are exported to HTML, MHTML and EPUB.
    /// </summary>
    /// <seealso cref="HtmlSaveOptions.ExportListLabels"/>
    [SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames",
        Justification = "Public API, as designed.")]
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum ExportListLabels
    {
        /// <summary>
        /// Outputs list labels in auto mode. Uses HTML native elements when possible.
        /// </summary>
        /// <remarks>
        /// HTML &lt;ul&gt; and &lt;ol&gt; tags are used for list label representation if it doesn't cause formatting loss, otherwise the HTML &lt;p&gt; tag is used.
        /// </remarks>
        Auto = 0,

        /// <summary>
        /// Outputs all list labels as inline text.
        /// </summary>
        /// <remarks>
        /// HTML &lt;p&gt; tag is used for any list label representation.
        /// </remarks>
        AsInlineText = 1,

        /// <summary>
        /// Outputs all list labels as HTML native elements.
        /// </summary>
        /// <remarks>
        /// HTML &lt;ul&gt; and &lt;ol&gt; tags are used for list label representation. Some formatting loss is possible.
        /// </remarks>
        ByHtmlTags = 2
    }
}
