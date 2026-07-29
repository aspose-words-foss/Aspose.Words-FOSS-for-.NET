// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/09/2011 by Roman Korchagin

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Determines how Aspose.Words resizes the table when you invoke the <see cref="Table.AutoFit"/> method.
    /// </summary>
    public enum AutoFitBehavior
    {
        /// <summary>
        /// <para>Aspose.Words enables the AutoFit option, removes the preferred width from the table and all cells and then updates the table layout.</para>
        /// 
        /// <para>In the resulting table, cell widths are updated to fit the table contents. Most likely, the table will shrink.</para>
        /// </summary>
        AutoFitToContents,

        /// <summary>
        /// <para>When you use this value, Aspose.Words enables the AutoFit option, sets the preferred width for the table to 100%, 
        /// removes preferred widths from all cells and then updates the table layout.</para>
        /// 
        /// <para>As a result, the table occupies all available width and the cell widths are updated to fit table contents.</para>
        /// </summary>
        AutoFitToWindow,

        /// <summary>
        /// <para>Aspose.Words disables the AutoFit option and removes the preferred with from the table.</para>
        /// 
        /// <para>The widths of the cells remain as they are specified by their <see cref="CellFormat.Width"/> properties.</para>
        /// </summary>
        FixedColumnWidths
    }
}
