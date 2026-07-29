// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/12/2014 by Ilya Navrotskiy

using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder.InsertDocument
{
    /// <summary>
    /// WORDSNET-5251 Consider providing a one line method in the API for InsertDocument.
    /// Tests insertion of document with block level SDT and paragraph at the end.
    /// </summary>
    [TestFixture]
    public class TestBlockLevelSdtDocEndsWithPara : TestInsertDocumentBase
    {
        /// <summary>
        /// Checks that paragraph before inserted content is list item.
        /// </summary>
        [Test]
        public void TestListParaBefore()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDocEndsWithPara, location);

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
        /// Checks that paragraph after inserted content is list item and
        /// its 'id' is the same, as paragraph's list 'id' before inserted content.
        /// </summary>
        [Test]
        public void TestListParaAfter()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDocEndsWithPara, location);

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
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDocEndsWithPara, location);

                if (location == TestInsertDstDocLocation.InlineSdt)
                {
                    Assert.That(FirstInsertedPara.IsListItem, Is.False);
                }
                else
                {
                    Paragraph currPara = FirstInsertedPara;
                    for (int i = 0; i < 8; i++)
                    {
                        switch (location)
                        {
                            case TestInsertDstDocLocation.BeginningOfListItem:
                                Assert.That(currPara.IsListItem, Is.True);
                                break;
                            default:
                                Assert.That(currPara.IsListItem, Is.False);
                                break;
                        }
                        currPara = (Paragraph)currPara.NextPreOrderOfType(currPara.Document, NodeType.Paragraph);
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
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDocEndsWithPara, location);

                Paragraph currPara = FirstInsertedPara;
                if (location == TestInsertDstDocLocation.InlineSdt)
                {
                    Assert.That(currPara.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Normal)); 
                }
                else
                {
                    // Check all inserted paragraphs.
                    for (int i = 0; i < 8; i++)
                    {
                        StyleIdentifier currParaStyle = currPara.ParagraphStyle.StyleIdentifier;
                        switch (i)
                        {
                            case 1:
                                if (location == TestInsertDstDocLocation.DocumentEnd)
                                {
                                    Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.FootnoteText));
                                    currPara = (Paragraph) currPara.NextPreOrderOfType(currPara.Document, NodeType.Paragraph);
                                }
                                else
                                    Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.User));
                                break;
                            case 2:
                                if (location == TestInsertDstDocLocation.BeginningOfListItem)
                                    Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.ListParagraph));
                                else
                                    Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.Normal));
                                break;
                            case 4:
                            case 5:
                                Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.NoSpacing));
                                break;
                            default:
                                Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.User));
                                break;
                        }

                        currPara = (Paragraph) currPara.NextPreOrderOfType(currPara.Document, NodeType.Paragraph);
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
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDocEndsWithPara, location);

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
        /// Checks that first inserted paragraph was concatenated with split paragraph inside destination document.
        /// </summary>
        [Test]
        public void TestCheckFirstParasConcatenated()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDocEndsWithPara, location);
                // Inserted content begins with this text.
                int firstInsertedParaTextIdx = FirstInsertedParaText.IndexOf("First inserted paragraph");
                switch (location)
                {
                    case TestInsertDstDocLocation.BeginningOfListItem:
                    case TestInsertDstDocLocation.Cell:
                    case TestInsertDstDocLocation.DocumentStart:
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

