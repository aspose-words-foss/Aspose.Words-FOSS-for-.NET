// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/12/2014 by Ilya Navrotskiy

using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder.InsertDocument
{
    /// <summary>
    /// WORDSNET-5251 Consider providing a one line method in the API for InsertDocument.
    /// Tests insertion of document with only single list item paragraph.
    /// </summary>
    [TestFixture]
    public class TestOneListItemDoc : TestInsertDocumentBase
    {
        /// <summary>
        /// Checks that paragraph before inserted content is a list item.
        /// </summary>
        [Test]
        public void TestListParaBefore()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.OneListItemDoc, location);

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
                TestInsertDocument(TestInsertSrcDocType.OneListItemDoc, location);

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
        /// Checks numbering styles of inserted paragraphs with default options.
        /// </summary>
        [Test]
        public void TestListInsertedParagraphsWithDefaultOptions()
        {
            TestInsertDocument(TestInsertSrcDocType.OneListItemDoc, TestInsertDstDocLocation.BeginningOfListItem);
            Assert.That(FirstInsertedPara, IsNot.Null());
            Assert.That(FirstInsertedPara.ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.Arabic));

            TestInsertDocument(TestInsertSrcDocType.OneListItemDoc, TestInsertDstDocLocation.MiddleOfListItem);
            Assert.That(FirstInsertedPara, IsNot.Null());
            Assert.That(FirstInsertedPara.ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.Bullet));

            TestInsertDocument(TestInsertSrcDocType.OneListItemDoc, TestInsertDstDocLocation.MiddleOfListItem);
            Assert.That(FirstInsertedPara, IsNot.Null());
            Assert.That(FirstInsertedPara.ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.Bullet));
        }

        /// <summary>
        /// Checks numbering styles of inserted paragraphs with disabled 'MergeLists' option.
        /// </summary>
        [Test]
        public void TestListInsertedParagraphsWithMergeOptionDisabled()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.OneListItemDoc, location, new ImportFormatOptions());

                switch (location)
                {
                    case TestInsertDstDocLocation.InlineSdt:
                        Assert.That(FirstInsertedPara, IsNot.Null());
                        Assert.That(FirstInsertedPara.IsListItem, Is.False);
                        break;
                    default:
                        Assert.That(FirstInsertedPara, IsNot.Null());
                        Assert.That(FirstInsertedPara.ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.Bullet));
                        break;
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
                TestInsertDocument(TestInsertSrcDocType.OneListItemDoc, location);

                Assert.That(FirstInsertedPara, IsNot.Null());

                Assert.That(FirstInsertedPara.ParagraphStyle.StyleIdentifier, Is.EqualTo((location == TestInsertDstDocLocation.InlineSdt)
                    ? StyleIdentifier.Normal : StyleIdentifier.User));
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
                TestInsertDocument(TestInsertSrcDocType.OneListItemDoc, location);

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
        /// Checks that first inserted node was concatenated with the paragraph before it.
        /// </summary>
        [Test]
        public void TestCheckFirstParasConcatenated()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.OneListItemDoc, location);

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

