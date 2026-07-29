// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/01/2007 by Roman Korchagin

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how CSS (Cascading Style Sheet) styles are exported to HTML.
    /// </summary>
    /// <seealso cref="HtmlSaveOptions.CssStyleSheetType"/>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum CssStyleSheetType
    {
        /// <summary>
        /// CSS styles are written inline (as a value of the <b>style</b> attribute on every element).
        /// </summary>
        Inline = 0,
        /// <summary>
        /// CSS styles are written separately from the content in a style sheet embedded in the HTML file.
        /// </summary>
        Embedded = 1,
        /// <summary>
        /// CSS styles are written separately from the content in a style sheet in an external file.
        /// The HTML file links the style sheet. 
        /// </summary>
        External = 2
    }
}
