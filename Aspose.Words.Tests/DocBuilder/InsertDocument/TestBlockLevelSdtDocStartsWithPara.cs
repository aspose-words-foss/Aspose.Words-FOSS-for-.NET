// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/12/2014 by Ilya Navrotskiy

using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder.InsertDocument
{
    /// <summary>
    /// WORDSNET-5251 Consider providing a one line method in the API for InsertDocument.
    /// Tests insertion of document with block level SDT and paragraph at the beginning.
    /// </summary>
    [TestFixture]
    public class TestBlockLevelSdtDocStartsWithPara : TestInsertDocumentBase
    {
        /// <summary>
        /// Checks that paragraph before inserted content is a list item.
        /// </summary>
        [Test]
        public void TestListParaBefore()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDocStartsWithPara, location);

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
        /// Checks that paragraph after inserted with default options content is a list item and
        /// its 'id' is not the same as paragraph's list 'id' before inserted content
        /// when inserting at the beginning or middle of list item.
        /// </summary>
        [Test]
        public void TestListParaAfterWithDefaultOptions()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDocStartsWithPara, location);
                switch (location)
                {
                    case TestInsertDstDocLocation.BeginningOfListItem:
                    case TestInsertDstDocLocation.MiddleOfListItem:
                        Assert.That(ParaAfterInsertedContent.ListFormat.ListId, IsNot.EqualTo(ParaBeforeInsertedContent.ListFormat.ListId));
                        break;
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
        /// Checks that paragraph after inserted with disabled 'MergeLists' option content is a list item and
        /// its 'id' is the same as paragraph's list 'id' before inserted content.
        /// </summary>
        [Test]
        public void TestListParaAfterWithMergeOptionDisabled()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDocStartsWithPara, location, new ImportFormatOptions());
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
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDocStartsWithPara, location);
                int parasInsertedCount = (location == TestInsertDstDocLocation.InlineSdt) ? 1 : 6;
                Paragraph currPara = FirstInsertedPara;
                for (int i = 0; i < parasInsertedCount; i++)
                {
                    Assert.That(currPara, IsNot.Null());
                    Assert.That(currPara.IsListItem, Is.False);
                    currPara = (Paragraph)currPara.NextPreOrderOfType(currPara.Document, NodeType.Paragraph);
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
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDocStartsWithPara, location);

                Paragraph currPara = FirstInsertedPara;
                if (location == TestInsertDstDocLocation.InlineSdt)
                {
                    Assert.That(FirstInsertedPara, IsNot.Null());
                    Assert.That(FirstInsertedPara.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Normal));
                }
                else
                {
                    // Totally inserted srcDoc has 6 paragraphs. Check them all.
                    Assert.That(currPara, IsNot.Null());
                    for (int i = 0; i < 6; i++)
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
                                    Assert.That(currParaStyle, Is.EqualTo(StyleIdentifier.NoSpacing));
                                break;
                            case 2:
                            case 4:
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
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDocStartsWithPara, location);

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
        /// Checks that first inserted node was concatenated with paragraph before it.
        /// </summary>
        [Test]
        public void TestCheckFirstParasConcatenated()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDocStartsWithPara, location);
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

        /// <summary>
        /// Checks 'AdjustSpaces' option when inserting document.
        /// </summary>
        [Test]
        public void TestAjustSpacing()
        {
            //Checks when AdjustSpaces is enabled.
            TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDocStartsWithPara, TestInsertDstDocLocation.Middle);
            Assert.That(FirstInsertedParaText.IndexOf("mid First"), Is.EqualTo(25));
            Assert.That(ParaAfterInsertedContent.GetText().StartsWith(" dle of the body"), Is.True);

            //Checks when AdjustSpaces is disabled.
            TestInsertDocument(TestInsertSrcDocType.BlockLevelSdtDocStartsWithPara, TestInsertDstDocLocation.Middle, new ImportFormatOptions());
            Assert.That(FirstInsertedParaText.IndexOf("midFirst"), Is.EqualTo(25));
            Assert.That(ParaAfterInsertedContent.GetText().StartsWith("dle of the body"), Is.True);
        }
    }
}

