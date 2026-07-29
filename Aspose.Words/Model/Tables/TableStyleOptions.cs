// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/06/2009 by Roman Korchagin

using System;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Specifies how table style is applied to a table.
    /// </summary>
    /// <seealso cref="Table.StyleOptions"/>
    /// <dev>
    /// 2.4.51 tblLook (Table Style Conditional Formatting Settings)
    /// The DOC spec defines more values here, but it says they are ignored by Word 2007 so we ignore them too.
    /// </dev>
    [Flags]
    public enum TableStyleOptions
    {
        /// <summary>
        /// No table style formatting is applied.
        /// </summary>
        None = 0x0000,

        // Borders - they only from auto-format
        // Shading - only from auto-format
        // Font - only from auto-format
        // Color - only from auto-format
        // BestFit - only from auto-format

        /// <summary>
        /// Apply first row conditional formatting.
        /// </summary>
        FirstRow = 0x0020,
        /// <summary>
        /// Apply last row conditional formatting.
        /// </summary>
        LastRow = 0x0040,
        /// <summary>
        /// Apply 1 first column conditional formatting.
        /// </summary>
        FirstColumn = 0x0080,
        /// <summary>
        /// Apply last column conditional formatting.
        /// </summary>
        LastColumn = 0x0100,
        /// <summary>
        /// Apply row banding conditional formatting.
        /// </summary>
        RowBands = 0x0200,
        /// <summary>
        /// Apply column banding conditional formatting.
        /// </summary>
        ColumnBands = 0x0400,

        //JAVA: moved here to remove illegal forward reference error.
        /// <summary>
        /// Row and column banding is applied. This is Microsoft Word default for old formats such as DOC, WML and RTF.
        /// </summary>
        Default2003 = RowBands | ColumnBands,

        /// <summary>
        /// This is Microsoft Word defaults.
        /// </summary>
        Default = FirstRow | FirstColumn | RowBands
    }
}
