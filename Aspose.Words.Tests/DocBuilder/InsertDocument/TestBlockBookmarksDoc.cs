// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/08/2018 by Alexander Zhiltsov

using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder.InsertDocument
{
    /// <summary>
    /// Tests insertion of a document with block level bookmarks.
    /// </summary>
    public class TestBlockBookmarksDoc : TestInsertDocumentBase
    {
        /// <summary>
        /// Checks that paragraph before inserted content is a list item.
        /// </summary>
        [Test]
        public void TestListParaBefore()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockBookmarksDoc, location);

                switch (location)
                {
                    case TestInsertDstDocLocation.BeginningOfListItem:
                    case TestInsertDstDocLocation.MiddleOfListItem:
                    case TestInsertDstDocLocation.EndOfListItem:
                    case TestInsertDstDocLocation.DocumentEnd:
                        Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                        Assert.That(ParaBeforeInsertedContent.IsListItem, Is.True);
                        break;
                    case TestInsertDstDocLocation.Middle:
                    case TestInsertDstDocLocation.MiddleWithSpaceAfter:
                    case TestInsertDstDocLocation.MiddleWithSpaceBefore:
                    case TestInsertDstDocLocation.InlineSdt:
                        Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                        Assert.That(ParaBeforeInsertedContent.IsListItem, Is.False);
                        break;
                    default:
                        Assert.That(ParaBeforeInsertedContent, Is.Null);
                        break;
                }
            }
        }

        /// <summary>
        /// Checks that paragraph after inserted content is a list item and
        /// its 'id' is the same as paragraph's list 'id' before inserted content.
        /// </summary>
        [Test]
        public void TestListParaAfter()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockBookmarksDoc, location);

                switch (location)
                {
                    case TestInsertDstDocLocation.BeginningOfListItem:
                    case TestInsertDstDocLocation.MiddleOfListItem:
                    case TestInsertDstDocLocation.EndOfListItem:
                        Assert.That(ParaAfterInsertedContent.ListFormat.ListId, Is.EqualTo(ParaBeforeInsertedContent.ListFormat.ListId));
                        break;
                    case TestInsertDstDocLocation.InlineSdt:
                        Assert.That(ParaAfterInsertedContent, IsNot.Null());
                        Assert.That(ParaAfterInsertedContent.IsListItem, Is.True);
                        break;
                    default:
                        Assert.That(ParaAfterInsertedContent, IsNot.Null());
                        Assert.That(ParaAfterInsertedContent.IsListItem, Is.False);
                        break;
                }
            }
        }

        /// <summary>
        /// Checks that inserted paragraph is a list item.
        /// </summary>
        [Test]
        public void TestListFirstInsertedPara()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockBookmarksDoc, location);

                Assert.That(FirstInsertedPara, IsNot.Null());

                if (location == TestInsertDstDocLocation.BeginningOfListItem)
                    Assert.That(FirstInsertedPara.IsListItem, Is.True);
                else
                    Assert.That(FirstInsertedPara.IsListItem, Is.False);
            }
        }

        /// <summary>
        /// Checks style of the inserted paragraph.
        /// </summary>
        [Test]
        public void TestStyleFirstInsertedPara()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockBookmarksDoc, location);

                Assert.That(FirstInsertedPara, IsNot.Null());

                if (location == TestInsertDstDocLocation.InlineSdt)
                    Assert.That(FirstInsertedPara.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Normal));
                else
                    Assert.That(FirstInsertedPara.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.User));
            }
        }

        /// <summary>
        /// Checks style of the paragraph before inserted content.
        /// </summary>
        [Test]
        public void TestStyleParaBefore()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockBookmarksDoc, location);

                switch (location)
                {
                    case TestInsertDstDocLocation.DocumentStart:
                    case TestInsertDstDocLocation.Cell:
                    case TestInsertDstDocLocation.BlockLevelSdt:
                    case TestInsertDstDocLocation.Comment:
                    case TestInsertDstDocLocation.Footnote:
                    case TestInsertDstDocLocation.Header:
                    case TestInsertDstDocLocation.TextBox:
                        Assert.That(ParaBeforeInsertedContent, Is.Null);
                        break;
                    case TestInsertDstDocLocation.BeginningOfListItem:
                    case TestInsertDstDocLocation.MiddleOfListItem:
                    case TestInsertDstDocLocation.EndOfListItem:
                    case TestInsertDstDocLocation.DocumentEnd:
                        Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                        Assert.That(ParaBeforeInsertedContent.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.ListParagraph));
                        break;
                    default:
                        Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                        Assert.That(ParaBeforeInsertedContent.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Normal));
                        break;
                }
            }
        }

        /// <summary>
        /// Checks that inserted paragraph was concatenated with the paragraph before it.
        /// </summary>
        [Test]
        public void TestCheckFirstParasConcatenated()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockBookmarksDoc, location);

                int firstInsertedParaTextIdx = FirstInsertedParaText.IndexOf("First inserted paragraph");
                switch (location)
                {
                    case TestInsertDstDocLocation.Cell:
                    case TestInsertDstDocLocation.DocumentStart:
                    case TestInsertDstDocLocation.BeginningOfListItem:
                        Assert.That(firstInsertedParaTextIdx, Is.EqualTo(0));
                        break;
                    default:
                        Assert.That(firstInsertedParaTextIdx, Is.GreaterThan(1));
                        break;
                }
            }
        }
    }
}
