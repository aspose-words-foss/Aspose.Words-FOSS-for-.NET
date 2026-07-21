// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/05/2012 by Alexey Butalov

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Text line's type
    /// </summary>
    internal enum TxtLineType
    {
        /// <summary>
        /// Line with a text
        /// </summary>
        TextLine,

        /// <summary>
        /// Line starts a list
        /// </summary>
        NumberingLine,

        /// <summary>
        /// End of file marker
        /// </summary>
        Eof,

        /// <summary>
        /// Empty line
        /// </summary>
        EmptyLine,

        /// <summary>
        /// Delimiter line like:
        /// -------------------------------------
        /// ***************************************
        /// ==============================
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// </summary>
        DelimiterLine
    }
}