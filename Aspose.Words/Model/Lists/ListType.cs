// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/05/2006 by Roman Korchagin

namespace Aspose.Words.Lists
{
    /// <summary>
    /// Specifies the list type.
    /// </summary>
    internal enum ListType
    {
        /// <summary>
        /// Specifies that this numbering definition defines a numbering format consisting of a
        /// single level only.
        ///
        /// Word 97 emitted simple or multilevel (not simple) lists.
        /// </summary>
        SingleLevel,
        /// <summary>
        /// Word 97 emitted simple or multilevel (not simple) lists.
        ///
        /// Specifies that this numbering definition defines a numbering format consisting of a multiple levels,
        /// each of the same type (bullets vs. level text).
        /// </summary>
        MultiLevel,
        /// <summary>
        /// Specifies that this numbering definition defines a numbering format consisting of a multiple levels,
        /// each of a potentially different type (bullets vs. level text).
        ///
        /// Word 2000, Word 2002, and Word 2003 emit hybrid lists, which are essentially collections of simple
        /// lists. The main difference between Word 2000, Word 2002, and Word 2003 hybrid lists and Word 97
        /// multilevel lists is that each level of a hybrid list has a unique identifier.
        /// </summary>
        HybridMultiLevel
    }
}
