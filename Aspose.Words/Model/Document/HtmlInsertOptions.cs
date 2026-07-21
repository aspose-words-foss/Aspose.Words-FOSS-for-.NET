// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/04/2014 by Victor Chebotok

using System;

namespace Aspose.Words
{
    /// <summary>
    /// Specifies options for the <see cref="DocumentBuilder.InsertHtml(string, HtmlInsertOptions)"/> method.
    /// </summary>
    [Flags]
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum HtmlInsertOptions
    {
        /// <summary>
        /// Use the default options when inserting HTML.
        /// </summary>
        None = 0,

        /// <summary>
        /// Use font and paragraph formatting specified in <see cref="DocumentBuilder"/> as base formatting for text
        /// inserted from HTML.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this option is not specified, formatting of <see cref="DocumentBuilder"/> is ignored and text is inserted
        /// with default HTML formatting. As a result, the text looks as it is rendered in browsers.
        /// </para>
        /// <para>
        /// If this option is specified, formatting of inserted text is based on formatting specified in
        /// <see cref="DocumentBuilder"/>, and the text looks as if it were inserted using <see cref="DocumentBuilder.Write(String)"/>.
        /// </para>
        /// </remarks>
        UseBuilderFormatting = 1,

        /// <summary>
        /// Remove the empty paragraph that is normally inserted after HTML that ends with a block-level element.
        /// </summary>
        /// <remarks>
        /// By default, <see cref="DocumentBuilder"/> makes sure that the last block-level element imported from HTML
        /// is closed after import and inserts a paragraph break after the element. This paragraph break separates
        /// content imported from HTML from content of the template document. However, if a HTML fragment is inserted into
        /// an empty paragraph, that paragraph break will create an extra empty paragraph. If this behavior is undesired,
        /// specify this option.
        /// </remarks>
        RemoveLastEmptyParagraph = 2,

        /// <summary>
        /// Preserve properties of block-level elements.
        /// </summary>
        /// <remarks>
        /// <para>
        /// By default, properties of parent blocks are merged and stored on their child elements (i.e. paragraphs or tables).
        /// If this option is specified, properties of each block are stored separately in a special logical structure.
        /// As a result, this option allows to better preserve individual borders and margins seen in the HTML document
        /// and get better conversion results. The downside is that the resulting document gets harder to modify, since borders
        /// and margins stored in the logical structure are not available for editing.
        /// </para>
        /// <para>
        /// Only margins and borders of 'body', 'div', and 'blockquote' HTML elements are preserved. Properties of each HTML
        /// element are stored separately.
        /// </para>
        /// <para>
        /// If this option is specified, Aspose.Words mimics MS Word's behavior regarding import of block properties.
        /// </para>
        /// </remarks>
        PreserveBlocks = 4
    }
}
