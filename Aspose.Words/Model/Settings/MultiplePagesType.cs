// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/04/2012 by Andrey Noskov

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Specifies how document is printed out. 
    /// </summary>
    public enum MultiplePagesType
    {
        /// <summary>
        /// Normal printing, no multiple pages specified.
        /// </summary>
        Normal,

        /// <summary>
        /// Swaps left and right margins on facing pages.
        /// </summary>
        MirrorMargins,

        /// <summary>
        /// Prints two pages per sheet.
        /// </summary>
        TwoPagesPerSheet,

        /// <summary>
        /// Specifies whether to print the document as a book fold.
        /// </summary>
        BookFoldPrinting,

        /// <summary>
        /// Specifies whether to print the document as a reverse book fold.
        /// </summary>
        BookFoldPrintingReverse,

        /// <summary>
        /// Default value is <see cref="Normal"/>
        /// </summary>
        Default = Normal
    }
}
