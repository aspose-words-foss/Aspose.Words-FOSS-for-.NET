// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2011 by Roman Korchagin

using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Specifies the unit of measurement for the preferred width of a table or cell.
    /// </summary>
    /// <seealso cref="PreferredWidth"/>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue",
        Justification = "Public API, as designed.")]
    public enum PreferredWidthType
    {
        /// <summary>
        /// The preferred width is not specified. The actual width of the table or cell is either specified using the explicit width or 
        /// will be determined automatically by the table layout algorithm when the table is displayed, depending on the table auto fit setting.
        /// </summary>
        Auto = 1,
        /// <summary>
        /// Measure the current item width using a specified percentage.
        /// </summary>
        Percent = 2,
        /// <summary>
        /// Measure the current item width using a specified number of points (1/72 inch).
        /// </summary>
        Points = 3,
    }
}
