// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/12/2014 by Ilya Navrotskiy

using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder.InsertDocument
{
    /// <summary>
    /// WORDSNET-5251 Consider providing a one line method in the API for InsertDocument.
    /// Tests insertion of document started with a table.
    /// </summary>
    [TestFixture]
    public class TestTableDocStartsWithTable : TestInsertDocumentBase
    {
        /// <summary>
        /// Checks that paragraph before inserted content is a list item.
        /// </summary>
        [Test]
        public void TestListParaBefore()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.TableDocStartsWithTable, location);

                switch (location)
                {
                    case TestInsertDstDocLocation.BeginningOfListItem:
                    case TestInsertDstDocLocation.MiddleOfListItem:
                    case TestInsertDstDocLocation.EndOfListItem:
                        Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                        Assert.That(ParaBeforeInsertedContent.IsListItem, Is.True);
                        break;
                    case TestInsertDstDocLocation.Cell:
                    case TestInsertDstDocLocation.DocumentStart:
                        Assert.That(ParaBeforeInsertedContent, Is.Null);
                        break;
                    default:
                        Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                        Assert.That(ParaBeforeInsertedContent.IsListItem, Is.False);
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
                TestInsertDocument(TestInsertSrcDocType.TableDocStartsWithTable, location);

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
                TestInsertDocument(TestInsertSrcDocType.TableDocStartsWithTable, location);

                switch (location)
                {
                    case TestInsertDstDocLocation.InlineSdt:
                        Assert.That(FirstInsertedPara.IsListItem, Is.False);
                        break;

                    case TestInsertDstDocLocation.BeginningOfListItem:
                    case TestInsertDstDocLocation.MiddleOfListItem:
                    case TestInsertDstDocLocation.EndOfListItem:
                    {
                        Assert.That(FirstInsertedPara.IsListItem, Is.True);
                        Paragraph secondInsertedPara = (Paragraph)FirstInsertedNode.NextSibling;
                        Assert.That(secondInsertedPara.IsListItem, Is.False);
                        break;
                    }
                    default:
                    {
                        Assert.That(FirstInsertedPara.IsListItem, Is.False);
                        Paragraph secondInsertedPara = (Paragraph)FirstInsertedNode.NextSibling;
                        Assert.That(secondInsertedPara.IsListItem, Is.False);
                        break;
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
                TestInsertDocument(TestInsertSrcDocType.TableDocStartsWithTable, location);

                switch (location)
                {
                    case TestInsertDstDocLocation.InlineSdt:
                        Assert.That(FirstInsertedPara.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Normal));
                        break;

                    default:
                        Assert.That(FirstInsertedPara.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.User));
                        Paragraph secondInsertedPara = (Paragraph)FirstInsertedNode.NextSibling;
                        Assert.That(secondInsertedPara.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Normal));
                        break;
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
                TestInsertDocument(TestInsertSrcDocType.TableDocStartsWithTable, location);

                switch (location)
                {
                    case TestInsertDstDocLocation.BeginningOfListItem:
                    case TestInsertDstDocLocation.MiddleOfListItem:
                    case TestInsertDstDocLocation.EndOfListItem:
                        Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                        Assert.That(ParaBeforeInsertedContent.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.ListParagraph));
                        break;
                    case TestInsertDstDocLocation.Comment:
                        Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                        Assert.That(ParaBeforeInsertedContent.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.CommentText));
                        break;
                    case TestInsertDstDocLocation.Footnote:
                        Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                        Assert.That(ParaBeforeInsertedContent.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.FootnoteText));
                        break;
                    case TestInsertDstDocLocation.Header:
                        Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                        Assert.That(ParaBeforeInsertedContent.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Header));
                        break;
                    case TestInsertDstDocLocation.Cell:
                    case TestInsertDstDocLocation.DocumentStart:
                        Assert.That(ParaBeforeInsertedContent, Is.Null);
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
                TestInsertDocument(TestInsertSrcDocType.TableDocStartsWithTable, location);

                int firstInsertedParaTextIdx = FirstInsertedPara.GetText().IndexOf("First inserted paragraph");
                switch (location)
                {
                    case TestInsertDstDocLocation.InlineSdt:
                        Assert.That(firstInsertedParaTextIdx, Is.GreaterThan(1));
                        break;
                    default:
                        Assert.That(firstInsertedParaTextIdx, Is.EqualTo(0));
                        break;
                }
            }
        }
    }
}

