// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/12/2014 by Ilya Navrotskiy

using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder.InsertDocument
{
    /// <summary>
    /// WORDSNET-5251 Consider providing a one line method in the API for InsertDocument.
    /// Tests insertion of document with block level SDT.
    /// </summary>
    public class TestBlockLevelSdtDoc : TestInsertDocumentBase
    {
        /// <summary>
        /// Checks that paragraph before inserted content is list item.
        /// </summary>
        [Test]
        public void TestListParaBefore()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDoc, location);
                
                switch (location)
                {
                    case TestInsertDstDocLocation.BeginningOfListItem:
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
        /// Checks that paragraph after inserted content is list item and
        /// its 'id' is the same as paragraph's list 'id' before inserted content.
        /// </summary>
        [Test]
        public void TestListParaAfter()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDoc, location);

                switch (location)
                {
                    case TestInsertDstDocLocation.BeginningOfListItem:
                        Assert.That(ParaAfterInsertedContent.ListFormat.ListId, Is.EqualTo(ParaBeforeInsertedContent.ListFormat.ListId));
                        break;
                    case TestInsertDstDocLocation.MiddleOfListItem:
                    case TestInsertDstDocLocation.EndOfListItem:
                        Assert.That(ParaAfterInsertedContent.ListFormat.ListId, Is.EqualTo(((Paragraph)(ParaBeforeInsertedContent.PreviousSibling)).ListFormat.ListId));
                        break;
                    case TestInsertDstDocLocation.InlineSdt:
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
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDoc, location);
                if (location != TestInsertDstDocLocation.InlineSdt)
                {
                    Paragraph currPara = FirstInsertedPara;
                    for (int i = 0; i < 14; i++)
                    {
                        Assert.That(currPara, IsNot.Null());
                        switch (location)
                        {
                            case TestInsertDstDocLocation.BeginningOfListItem:
                                // All paragraphs should be the list items, but those which are inside the tables.
                                if ((i == 5) || (i == 6) || (i == 9) || (i == 10))
                                    Assert.That(currPara.IsListItem, Is.False);
                                else
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
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDoc, location);

                // MS Word preserves styles of the inserted paragraphs.
                // However at the beginning of the list item, MS Word applies style of
                // destination document paragraph, if it has "Normal" style.
                if (location == TestInsertDstDocLocation.InlineSdt)
                {
                    Assert.That(FirstInsertedPara.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Normal));
                }
                else
                {
                    Paragraph currPara = FirstInsertedPara;
                    for (int i = 0; i < 14; i++)
                    {
                        Assert.That(currPara, IsNot.Null());
                        StyleIdentifier currParaStyle = currPara.ParagraphStyle.StyleIdentifier;
                        switch (i)
                        {
                            case 1:
                                Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.NoSpacing));
                                break;

                            case 2:
                            case 7:
                                if (location == TestInsertDstDocLocation.BeginningOfListItem)
                                    Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.ListParagraph));
                                else
                                    Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.Normal));
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
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDoc, location);

                switch (location)
                {
                    case TestInsertDstDocLocation.InlineSdt:
                        Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                        Assert.That(ParaBeforeInsertedContent.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Normal));
                        break;
                    case TestInsertDstDocLocation.DocumentStart:
                    case TestInsertDstDocLocation.Cell:
                        Assert.That(ParaBeforeInsertedContent, Is.Null);
                        break;
                    case TestInsertDstDocLocation.BeginningOfListItem:
                        Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                        Assert.That(ParaBeforeInsertedContent.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.ListParagraph));
                        break;
                    default:
                        Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                        Assert.That(ParaBeforeInsertedContent.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.User));
                        break;
                }
            }
        }

        /// <summary>
        /// Checks whether first inserted node was concatenated with paragraph before it.
        /// </summary>
        [Test]
        public void TestCheckFirstParasConcatenated()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDoc, location);
                // Inserted content begins with this text.
                int firstInsertedParaTextIdx = FirstInsertedParaText.IndexOf("First inserted paragraph");
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

        /// <summary>
        /// Checks that control chars were inserted into inline level SDT exactly the specified number of times.
        /// </summary>
        [Test]
        public void TestSpecialCharsInsideInlineSdt()
        {
            TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDoc, TestInsertDstDocLocation.InlineSdt);

            Assert.That(FirstInsertedParaText.Contains("Cell2\rThis is content outside sdtCell outside\tSDT outside\rThis"), Is.True);
            // There are 2 in text and 1 at the paragraph end.
            Assert.That(GetCharCount(FirstInsertedParaText, '\r'), Is.EqualTo(3));
            Assert.That(GetCharCount(FirstInsertedParaText, '\t'), Is.EqualTo(1));
        }
    }
}

