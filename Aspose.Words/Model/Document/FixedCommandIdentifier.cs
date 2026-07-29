// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/03/2010 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// [MD-DOC] 2.9.75 Fci, a 13-bit unsigned integer that specifies a built-in command.
    /// 
    /// RK I have declared only fci values that are used by the 2.9.1 Acd structure. 
    /// The complete list of fci values is too long so we should do it only if really becomes necessary.
    /// </summary>
    internal enum FixedCommandIdentifier
    {
        /// <summary>
        /// RK Added this. Indicates the command is unused. Used to maintain the proper command index according to the original DOC structure.
        /// </summary>
        None = 0x0000,
        /// <summary>
        /// Applies the indicated style to the selected text.
        /// </summary>
        ApplyStyleName = 0x0065,
        /// <summary>
        /// Applies the indicated font to the selected text.
        /// </summary>
        ApplyFontName = 0x0164,
        /// <summary>
        /// Inserts the indicated AutoText entry in the document.
        /// </summary>
        ApplyAutoTextName = 0x0211,
        /// <summary>
        /// Changes the number of columns in the selected sections.
        /// </summary>
        Columns = 0x0212,
        /// <summary>
        /// Sets the font character spacing of the selection to condensed.
        /// </summary>
        Condensed = 0x0213,
        /// <summary>
        /// Sets the font character spacing of the selection to expanded.
        /// </summary>
        Expanded = 0x0214,
        /// <summary>
        /// Changes the font size of the selection.
        /// </summary>
        FontSize = 0x0215,
        /// <summary>
        /// Lowers the selection below the base line.
        /// </summary>
        Lowered = 0x0216,
        /// <summary>
        /// Raises the selection above the base line.
        /// </summary>
        Raised = 0x0217,
        /// <summary>
        /// Opens a document.
        /// </summary>
        FileOpenFile = 0x0218,
        /// <summary>
        /// Changes the background shading of paragraphs and table cells.
        /// </summary>
        Shading = 0x0222,
        /// <summary>
        /// Changes the borders of paragraphs, table cells, and pictures.
        /// </summary>
        Borders = 0x0223,
        /// <summary>
        /// Changes the color of the selected text.
        /// </summary>
        Color = 0x0224,
        /// <summary>
        /// Inserts a special character.
        /// </summary>
        Symbol = 0x022B
    }
}
