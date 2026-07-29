// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/12/2014 by Ilya Navrotskiy

using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder.InsertDocument
{
    /// <summary>
    /// WORDSNET-5251 Consider providing a one line method in the API for InsertDocument.
    /// Tests insertion of document with a table and started with a paragraph.
    /// </summary>
    [TestFixture]
    public class TestTableDocStartsWithPara : TestInsertDocumentBase
    {
        /// <summary>
        /// Checks that paragraph before inserted content is a list item.
        /// </summary>
        [Test]
        public void TestListParaBefore()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.TableDocStartsWithPara, location);

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
                TestInsertDocument(TestInsertSrcDocType.TableDocStartsWithPara, location);

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
                TestInsertDocument(TestInsertSrcDocType.TableDocStartsWithPara, location);

                if (location == TestInsertDstDocLocation.InlineSdt)
                {
                    Assert.That(FirstInsertedPara, IsNot.Null());
                    Assert.That(FirstInsertedPara.IsListItem, Is.False);
                }
                else
                {
                    Paragraph currPara = FirstInsertedPara;
                    for (int i = 0; i < 24; i++)
                    {
                        Assert.That(currPara, IsNot.Null());
                        switch (location)
                        {
                            case TestInsertDstDocLocation.BeginningOfListItem:
                                if ((i == 0) || ((i >= 10) && (i <= 12)) || (i == 23))
                                    Assert.That(currPara.IsListItem, Is.True);
                                else
                                    Assert.That(currPara.IsListItem, Is.False);
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
                TestInsertDocument(TestInsertSrcDocType.TableDocStartsWithPara, location);

                Paragraph currPara = FirstInsertedPara;
                if (location == TestInsertDstDocLocation.InlineSdt)
                {
                    Assert.That(FirstInsertedPara, IsNot.Null());
                    Assert.That(FirstInsertedPara.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Normal));
                }
                else
                {
                    for (int i = 0; i < 24; i++)
                    {
                        Assert.That(currPara, IsNot.Null());
                        StyleIdentifier currParaStyle = currPara.ParagraphStyle.StyleIdentifier;
                        switch (i)
                        {
                            case 0:
                            case 12:
                                Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.User));
                                break;
                            case 1:
                                if (location == TestInsertDstDocLocation.DocumentEnd)
                                {
                                    Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.FootnoteText));
                                    currPara = (Paragraph) currPara.NextPreOrderOfType(currPara.Document, NodeType.Paragraph);
                                }
                                else
                                    Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.Normal));
                                break;
                            case 10:
                            case 23:
                                if (location == TestInsertDstDocLocation.BeginningOfListItem)
                                    Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.ListParagraph));
                                else
                                    Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.Normal));
                                break;
                            case 11:
                                Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.ListParagraph));
                                break;
                            default:
                                Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.Normal));
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
                TestInsertDocument(TestInsertSrcDocType.TableDocStartsWithPara, location);

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
                TestInsertDocument(TestInsertSrcDocType.TableDocStartsWithPara, location);

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

