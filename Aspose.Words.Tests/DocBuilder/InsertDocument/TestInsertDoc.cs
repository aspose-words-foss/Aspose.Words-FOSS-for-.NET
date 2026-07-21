// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/05/2022 by Evgeniy Zaytsev

using System;
using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder.InsertDocument
{
    /// <summary>
    /// Tests insertion of a document.
    /// </summary>
    public class TestInsertDoc : TestInsertDocumentBase
    {


        /// <summary>
        /// Checks that exception is thrown when inserting into the bad destination location.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestBadDstLocations()
        {
            foreach (TestInsertSrcDocType srcDocType in gSrcDocTypes)
                foreach (TestInsertDstDocLocation dstDocLocation in DstDocPossibleLocations)
                {
                    if (IsBadDstDocTestLocation(srcDocType, dstDocLocation))
                        TestInsertDocument(srcDocType, dstDocLocation);
                }
        }

        /// <summary>
        /// All possible types of source document for insertion.
        /// </summary>
        private static readonly TestInsertSrcDocType[] gSrcDocTypes =
        {
            TestInsertSrcDocType.OneParaDoc,
            TestInsertSrcDocType.OneListItemDoc,
            TestInsertSrcDocType.ListItemsDoc,
            TestInsertSrcDocType.TableDocStartsWithPara,
            TestInsertSrcDocType.BlockLevelSdtDocStartsWithPara,
            TestInsertSrcDocType.BlockLevelSdtDocEndsWithPara,
            TestInsertSrcDocType.OneParaDocInsideTable,
            TestInsertSrcDocType.TableDocStartsWithTable,
            TestInsertSrcDocType.BlockLevelSdtDoc,
            TestInsertSrcDocType.MultiSectionDoc,
            TestInsertSrcDocType.MultiColumnDoc,
            TestInsertSrcDocType.BlockBookmarksDoc,
            TestInsertSrcDocType.ParaWithNumberedStyle
        };

        /// <summary>
        /// All source test documents starts with the following text.
        /// </summary>
        private const string InsertedText = "First inserted paragraph";
    }
}
