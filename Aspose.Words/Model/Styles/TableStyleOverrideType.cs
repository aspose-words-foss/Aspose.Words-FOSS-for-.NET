// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/06/2009 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Represents possible table areas to which conditional formatting may be defined in a table style.
    /// </summary>
    /// <remarks>
    /// This enum is the same as <see cref="ConditionalStyleType"/>, but includes two additional items for
    /// internal usage.
    /// </remarks>
    [CppEnumEnableMetadata]
    internal enum TableStyleOverrideType
    {
        None,
        /// <summary>
        /// Specifies formatting of the first row of a table.
        /// </summary>
        FirstRow,
        /// <summary>
        /// Specifies formatting of the first column of a table.
        /// </summary>
        FirstColumn,
        /// <summary>
        /// Specifies formatting of the last row of a table.
        /// </summary>
        LastRow,
        /// <summary>
        /// Specifies formatting of the last column of a table.
        /// </summary>
        LastColumn,
        /// <summary>
        /// Specifies formatting of odd-numbered row stripe.
        /// </summary>
        OddRowBanding,
        /// <summary>
        /// Specifies formatting of odd-numbered column stripe.
        /// </summary>
        OddColumnBanding,
        /// <summary>
        /// Specifies formatting of even-numbered row stripe.
        /// </summary>
        EvenRowBanding,
        /// <summary>
        /// Specifies formatting of even-numbered column stripe.
        /// </summary>
        EvenColumnBanding,
        /// <summary>
        /// Specifies formatting of the top left cell of a table.
        /// </summary>
        TopLeftCell,
        /// <summary>
        /// Specifies formatting of the top right cell of a table.
        /// </summary>
        TopRightCell,
        /// <summary>
        /// Specifies formatting of the bottom left cell of a table.
        /// </summary>
        BottomLeftCell,
        /// <summary>
        /// Specifies formatting of the bottom right cell of a table.
        /// </summary>
        BottomRightCell,
        /// <summary>
        /// Specifies formatting of the whole table.
        /// </summary>
        /// <remarks>
        /// The WholeTable value is not present in the public <see cref="ConditionalStyleType"/> enum (and in VBA)
        /// since 'Word does not apply and discards on save any properties within the tblStylePr element when the "type"
        /// attribute has a value of "wholeTable"' [MS-OI29500].
        /// </remarks>
        WholeTable
    }
}
