// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/05/2011 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Conditional formatting property collection type. 
    /// </summary>
    /// <remarks>
    /// Table style may contain conditional formatting for certain cell/row set. This enumerates types of conditional formatting property collections.
    /// </remarks>
    internal enum AttrCollectionType
    {
        /// <summary>
        /// Character format collection.
        /// </summary>
        RunPr,

        /// <summary>
        /// Paragraph format collection.
        /// </summary>
        ParaPr,

        /// <summary>
        /// Cell format collection.
        /// </summary>
        CellPr
    }
}
