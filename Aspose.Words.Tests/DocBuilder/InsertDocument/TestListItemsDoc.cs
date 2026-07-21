// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/12/2014 by Ilya Navrotskiy

using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder.InsertDocument
{
    /// <summary>
    /// WORDSNET-5251 Consider providing a one line method in the API for InsertDocument.
    /// Tests insertion of document with list items.
    /// </summary>
    [TestFixture]
    public class TestListItemsDoc : TestInsertDocumentBase
    {
        /// <summary>
        /// Checks that paragraph before inserted content is a list item.
        /// </summary>
        [Test]
        public void TestListParaBefore()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.ListItemsDoc, location);

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
                TestInsertDocument(TestInsertSrcDocType.ListItemsDoc, location);

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
        /// Checks that inserted paragraphs are list items.
        /// </summary>
        [Test]
        public void TestListInsertedParagraphs()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.ListItemsDoc, location);

                if (location == TestInsertDstDocLocation.InlineSdt)
                {
                    Assert.That(FirstInsertedPara, IsNot.Null());
                    Assert.That(FirstInsertedPara.IsListItem, Is.False);
                }
                else
                {
                    Paragraph currPara = FirstInsertedPara;
                    for (int i = 0; i < 7; i++)
                    {
                        if (i == 4)
                        {
                            Assert.That(currPara.IsListItem, Is.False);
                        }
                        else
                        {
                            Assert.That(currPara.IsListItem, Is.True);
                            if (ParaBeforeInsertedContent != null)
                                Assert.That(ParaBeforeInsertedContent.ListFormat.ListId, IsNot.EqualTo(currPara.ListFormat.ListId));
                        }

                        currPara = (Paragraph)currPara.NextSibling;
                    }
                }
            }
        }

        /// <summary>
        /// Checks styles of the inserted paragraphs.
        /// </summary>
        [Test]
        public void TestStyleInsertedParagraphs()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.ListItemsDoc, location);

                if (location == TestInsertDstDocLocation.InlineSdt)
                {
                    Assert.That(FirstInsertedPara, IsNot.Null());
                    Assert.That(FirstInsertedPara.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Normal));
                }
                else
                {
                    Paragraph currPara = FirstInsertedPara;
                    for (int i = 0; i < 7; i++)
                    {
                        Assert.That(currPara.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.User));
                        currPara = (Paragraph)currPara.NextSibling;
                    }
                }
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
                TestInsertDocument(TestInsertSrcDocType.ListItemsDoc, location);

                switch (location)
                {
                    case TestInsertDstDocLocation.DocumentStart:
                    case TestInsertDstDocLocation.Cell:
                    case TestInsertDstDocLocation.BlockLevelSdt:
                    case TestInsertDstDocLocation.Comment:
                    case TestInsertDstDocLocation.Header:
                    case TestInsertDstDocLocation.TextBox:
                    case TestInsertDstDocLocation.Footnote:
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
        /// Checks that first inserted node was concatenated with the paragraph before it.
        /// </summary>
        [Test]
        public void TestCheckFirstParasConcatenated()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.ListItemsDoc, location);

                int insertedParaTextIdx = FirstInsertedParaText.IndexOf("First inserted paragraph");
                switch (location)
                {
                    case TestInsertDstDocLocation.BeginningOfListItem:
                    case TestInsertDstDocLocation.Cell:
                    case TestInsertDstDocLocation.DocumentStart:
                        Assert.That(insertedParaTextIdx, Is.EqualTo(0));
                        break;
                    default:
                        Assert.That(insertedParaTextIdx, Is.GreaterThan(1));
                        break;
                }
            }
        }
    }
}

