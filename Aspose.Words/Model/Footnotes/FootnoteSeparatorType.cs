// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words.Notes
{
    /// <summary>
    /// Specifies the type of the footnote/endnote separator.
    /// </summary>
    public enum FootnoteSeparatorType
    {
        /// <summary>
        /// Separator between main text and footnote text.
        /// </summary>
        /// <dev>
        /// In Word 2003 files this is \x03\r
        /// Do not renumber! Values below are used by DOC import/export.
        /// </dev>
        FootnoteSeparator = 0,

        /// <summary>
        /// Printed above footnote text on a page when the text must be continued from a previous page.
        /// </summary>
        /// <dev>
        /// Usually \x04\r
        /// </dev>
        FootnoteContinuationSeparator,

        /// <summary>
        /// Printed below footnote text on a page when footnote text must be continued on a succeeding page.
        /// </summary>
        /// <dev>
        /// Usually empty.
        /// </dev>
        FootnoteContinuationNotice,

        /// <summary>
        /// Separator between main text and endnote text.
        /// </summary>
        /// <dev>
        /// Usually \x03\r
        /// </dev>
        EndnoteSeparator,

        /// <summary>
        /// Printed above endnote text on a page when the text must be continued from a previous page.
        /// </summary>
        /// <dev>
        /// Usually \x04\r
        /// </dev>
        EndnoteContinuationSeparator,

        /// <summary>
        /// Printed below endnote text on a page when endnote text must be continued on a succeeding page.
        /// </summary>
        /// <dev>
        /// Usually empty.
        /// </dev>
        EndnoteContinuationNotice
    }
}
