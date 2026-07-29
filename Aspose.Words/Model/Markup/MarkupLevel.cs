// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2010 by Denis Darkin

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Specifies the level in the document tree where a particular <see cref="StructuredDocumentTag"/> can occur.
    /// </summary>
    public enum MarkupLevel
    {
        /// <summary>
        /// Specifies the unknown or invalid value.
        /// </summary>
        Unknown,

        /// <summary>
        /// The element occurs at the inline level (e.g. among as runs of text).
        /// </summary>
        Inline,

        /// <summary>
        /// The element occurs at the block level (e.g. among tables and paragraphs).
        /// </summary>
        Block,

        /// <summary>
        /// The element occurs among rows in a table.
        /// </summary>
        Row,

        /// <summary>
        /// The element occurs among cells in a row.
        /// </summary>
        Cell
    }
}
