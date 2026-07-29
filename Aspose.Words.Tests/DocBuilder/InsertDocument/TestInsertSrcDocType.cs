// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/12/2014 by Ilya Navrotskiy

namespace Aspose.Words.Tests.DocBuilder.InsertDocument
{
    /// <summary>
    /// Enumerates possible types of documents for insertion.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    internal enum TestInsertSrcDocType
    {
        /// <summary>
        /// Source document contains single paragraph with text.
        /// </summary>
        OneParaDoc,

        /// <summary>
        /// Source document contains single paragraph that is a list item.
        /// </summary>
        OneListItemDoc,
        
        /// <summary>
        /// Source document contains list items and starting and ending with list items.
        /// </summary>
        ListItemsDoc,
        
        /// <summary>
        /// Source document contains tables and starts with a paragraph.
        /// </summary>
        TableDocStartsWithPara,
        
        /// <summary>
        /// Source document contains a block level SDT and starts with a paragraph.
        /// </summary>
        BlockLevelSdtDocStartsWithPara,
        
        /// <summary>
        /// Source document contains a block level SDT and ends with a paragraph.
        /// </summary>
        BlockLevelSdtDocEndsWithPara,
        
        /// <summary>
        /// Source document contains a single table and a paragraph inside it.
        /// </summary>
        OneParaDocInsideTable,
        
        /// <summary>
        /// Source document contains tables and is started with a table.
        /// </summary>
        TableDocStartsWithTable,
        
        /// <summary>
        /// Source document starts with a block level SDT.
        /// </summary>
        BlockLevelSdtDoc,
        
        /// <summary>
        /// Source document is multisectional and contains headers/footers.
        /// </summary>
        MultiSectionDoc,
        
        /// <summary>
        /// Source document has multiple columns.
        /// </summary>
        MultiColumnDoc,

        /// <summary>
        /// Source document has block-level bookmarks.
        /// </summary>
        BlockBookmarksDoc,

        /// <summary>
        /// Source document contains paragraph with 'Heading1' style that has numbering.
        /// The same style exists in destination document, but it is without numbering there.
        /// </summary>
        ParaWithNumberedStyle
    }
}
