// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/12/2014 by Ilya Navrotskiy

namespace Aspose.Words.Tests.DocBuilder.InsertDocument
{
    /// <summary>
    /// Enumerates possible locations where we are going to insert test documents.
    /// There are bookmarks in the destination document corresponded to these locations.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    internal enum TestInsertDstDocLocation
    {
        BeginningOfListItem,
        BlockLevelSdt,
        Cell,
        Comment,
        DocumentEnd,
        DocumentStart,
        EndOfListItem,
        Footnote,
        Header,
        InlineSdt,
        MiddleWithSpaceAfter,
        MiddleWithSpaceBefore,
        Middle,
        MiddleOfListItem,
        TextBox
    }
}
