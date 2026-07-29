// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/12/2014 by Ilya Navrotskiy

using System;
using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder.InsertDocument
{
    /// <summary>
    /// WORDSNET-5251 Consider providing a one line method in the API for InsertDocument.
    /// Base class for all tests for DocumentBuilder.InsertDocument() method.
    /// </summary>
    public class TestInsertDocumentBase
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Inserts source document of srcType into the destination document at the specified location with Word2002_2010 options by default.
        /// </summary>
        internal void TestInsertDocument(TestInsertSrcDocType srcDocType, TestInsertDstDocLocation location)
        {
            ImportFormatOptions options = new ImportFormatOptions();
            SetWord2010Defaults(options);
            TestInsertDocument(srcDocType, location, options);
        }

        /// <summary>
        /// Inserts source document of srcType into the destination document at the specified location with the specified insertion options.
        /// </summary>
        internal void TestInsertDocument(TestInsertSrcDocType srcDocType, TestInsertDstDocLocation location, ImportFormatOptions options)
        {
            DocumentBuilder builder = OpenDstDocument(location);
            Document srcDoc = OpenSrcDocument(srcDocType);

            // For testing purposes we should know which node is located immediately after inserted content.
            // Current builder's node is always either BookmarkEnd or last run of inline SDT
            // in accordance with this test design.
            Node curNode = (builder.CurrentNode.NodeType == NodeType.BookmarkEnd)
                ? builder.CurrentNode.NextSibling
                : builder.CurrentNode;
            
            // If we are at the end of paragraph, then we should know next node to determine node after inserted content.
            if (curNode == null)
                curNode = builder.CurrentParagraph.NextSibling;
            
            mFirstInsertedNode = new DocumentInserter(builder).InsertDocument(srcDoc, ImportFormatMode.UseDestinationStyles, options);

            // If we was positioned at the end of paragraph, we also might be at the end of its parent. For example, at the end of document.
            // In this case we determine node after inserted content as last child of node where new content was inserted.
            if (curNode == null)
                mNodeAfterInsertedContent = mFirstInsertedNode.FirstNonMarkupParentNode.LastChild;
            else
                mNodeAfterInsertedContent = (curNode.NodeType == NodeType.Paragraph) ? curNode.PreviousSibling : curNode;

            string outFileParams = String.Format(@"{0}\{1}_{2}", srcDocType, location, options);
            Debug.WriteLine(outFileParams);

            // SonarIgnoreStart
            // Uncomment to generate out files in DocBuilder\InsertDocument directory.
            // string outFileName = String.Format(@"DocBuilder\InsertDocument\{0} out.docx", outFileParams);
            // mDocument.Save(TestUtil.GetInTestOutPath(outFileName));
            // SonarIgnoreEnd
        }

        /// <summary>
        /// Returns true, if source document type can not be inserted at the specified destination document location.
        /// </summary>
        internal static bool IsBadDstDocTestLocation(TestInsertSrcDocType srcDocType, TestInsertDstDocLocation location)
        {
            if (srcDocType != TestInsertSrcDocType.MultiSectionDoc)
                return false;

            switch (location)
            {
                case TestInsertDstDocLocation.Cell:
                case TestInsertDstDocLocation.Footnote:
                case TestInsertDstDocLocation.Header:
                case TestInsertDstDocLocation.TextBox:
                case TestInsertDstDocLocation.Comment:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns how many times specified character occurred in specified string.
        /// </summary>
        internal static int GetCharCount(string text, char ch)
        {
            int res = 0;

            int pos = text.IndexOf(ch);
            while (pos > -1)
            {
                res++;
                pos = text.IndexOf(ch, pos + 1);
            }

            return res;
        }

        /// <summary>
        /// Opens source document with appropriate content.
        /// </summary>
        internal static Document OpenSrcDocument(TestInsertSrcDocType srcDocType)
        {
            string srcFileName = GetSourceName(srcDocType);
            if (srcFileName == null)
                throw new ArgumentException("Unexpected source document.");

            Document doc = TestUtil.Open(@"DocBuilder\InsertDocument\Test" + srcFileName + ".docx");
            return doc;
        }

        /// <summary>
        /// Opens destination document and moves <see cref="DocumentBuilder"/> cursor to the location
        /// where we will insert another document.
        /// </summary>
        internal DocumentBuilder OpenDstDocument(TestInsertDstDocLocation location)
        {
            mDocument = TestUtil.Open(@"DocBuilder\InsertDocument\TestInsertDocumentDst.docx");

            DocumentBuilder builder = new DocumentBuilder(mDocument);
            string bookmarkName = GetLocationName(location);
            if (bookmarkName == null)
                throw new ArgumentException("Unexpected location.");

            builder.MoveToBookmark(bookmarkName, true, true);

            if (location == TestInsertDstDocLocation.InlineSdt)
            {
                Node node = ((CompositeNode)(builder.CurrentNode)).LastChild;

                builder.MoveTo(node);
            }

            mCurrentDstDocLocation = location;

            return builder;
        }

        /// <summary>
        /// Sets default options for Word 2002-2010.
        /// </summary>
        private static void SetWord2010Defaults(ImportFormatOptions options)
        {
            options.AdjustSentenceAndWordSpacing = true;
            options.SmartStyleBehavior = true;
            options.MergePastedLists = true;
        }

        private static string GetLocationName(TestInsertDstDocLocation value) 
        {
            switch (value)
        {
                case TestInsertDstDocLocation.BeginningOfListItem:
                    return "BeginningOfListItem";
                case TestInsertDstDocLocation.BlockLevelSdt:
                    return "BlockLevelSdt";
                case TestInsertDstDocLocation.Cell:
                    return "Cell";
                case TestInsertDstDocLocation.Comment:
                    return "Comment";
                case TestInsertDstDocLocation.DocumentEnd:
                    return "DocumentEnd";
                case TestInsertDstDocLocation.DocumentStart:
                    return "DocumentStart";
                case TestInsertDstDocLocation.EndOfListItem:
                    return "EndOfListItem";
                case TestInsertDstDocLocation.Footnote:
                    return "Footnote";
                case TestInsertDstDocLocation.Header:
                    return "Header";
                case TestInsertDstDocLocation.InlineSdt:
                    return "InlineSdt";
                case TestInsertDstDocLocation.MiddleWithSpaceAfter:
                    return "MiddleWithSpaceAfter";
                case TestInsertDstDocLocation.MiddleWithSpaceBefore:
                    return "MiddleWithSpaceBefore";
                case TestInsertDstDocLocation.Middle:
                    return "Middle";
                case TestInsertDstDocLocation.MiddleOfListItem:
                    return "MiddleOfListItem";
                case TestInsertDstDocLocation.TextBox:
                    return "TextBox";
                default:
                    throw new ArgumentOutOfRangeException("Unknown TestInsertDstDocLocation enum value: " + value);
            }
        }

        private static string GetSourceName(TestInsertSrcDocType value)
        {
            switch (value)
            {
                case TestInsertSrcDocType.OneParaDoc:
                    return "OneParaDoc";
                case TestInsertSrcDocType.OneListItemDoc:
                    return "OneListItemDoc";
                case TestInsertSrcDocType.ListItemsDoc:
                    return "ListItemsDoc";
                case TestInsertSrcDocType.TableDocStartsWithPara:
                    return "TableDocStartsWithPara";
                case TestInsertSrcDocType.BlockLevelSdtDocStartsWithPara:
                    return "BlockLevelSdtDocStartsWithPara";
                case TestInsertSrcDocType.BlockLevelSdtDocEndsWithPara:
                    return "BlockLevelSdtDocEndsWithPara";
                case TestInsertSrcDocType.OneParaDocInsideTable:
                    return "OneParaDocInsideTable";
                case TestInsertSrcDocType.TableDocStartsWithTable:
                    return "TableDocStartsWithTable";
                case TestInsertSrcDocType.BlockLevelSdtDoc:
                    return "BlockLevelSdtDoc";
                case TestInsertSrcDocType.MultiSectionDoc:
                    return "MultiSectionDoc";
                case TestInsertSrcDocType.MultiColumnDoc:
                    return "MultiColumnDoc";
                case TestInsertSrcDocType.BlockBookmarksDoc:
                    return "BlockBookmarks";
                case TestInsertSrcDocType.ParaWithNumberedStyle:
                    return "ParaWithNumberedStyle";
                default:
                    throw new ArgumentOutOfRangeException("Unknown TestInsertSrcDocType enum value: " + value);
            }
        }

        /// <summary>
        /// Gets all possible locations in the document where we can insert another one.
        /// </summary>
        internal TestInsertDstDocLocation[] DstDocPossibleLocations
        {
            get { return gPossibleLocations; }
        }

        /// <summary>
        /// Gets all possible locations inside list item.
        /// </summary>
        internal static TestInsertDstDocLocation[] ListItemLocations
        {
            get { return gListItemLocations; }
        }

        /// <summary>
        /// Gets first node of inserted content.
        /// </summary>
        internal Node FirstInsertedNode
        {
            get { return mFirstInsertedNode; }
        }

        /// <summary>
        /// Gets first paragraph of inserted content.
        /// </summary>
        internal Paragraph FirstInsertedPara
        {
            get
            {
                if (mCurrentDstDocLocation == TestInsertDstDocLocation.InlineSdt)
                    return (Paragraph) FirstInsertedNode.FirstNonMarkupParentNode;
                if ((FirstInsertedNode.NodeType == NodeType.BookmarkStart) &&
                    (((BookmarkStart)FirstInsertedNode).Name == "BlockLevelBookmark1") &&
                    (FirstInsertedNode.ParentNode.NodeType == NodeType.Paragraph))
                    return (Paragraph)FirstInsertedNode.ParentNode;

                return (FirstInsertedNode.NodeType == NodeType.Paragraph)
                    ? (Paragraph) FirstInsertedNode
                    : (Paragraph)FirstInsertedNode.NextPreOrderOfType(FirstInsertedNode.Document, NodeType.Paragraph);
            }
        }

        /// <summary>
        /// Gets paragraph before inserted content.
        /// </summary>
        internal Paragraph ParaBeforeInsertedContent
        {
            get
            {
                if (mCurrentDstDocLocation == TestInsertDstDocLocation.InlineSdt)
                    return (Paragraph)FirstInsertedPara.PreviousSibling;

                Node prevNode = FirstInsertedNode.PreviousSibling;

                if ((FirstInsertedNode.NodeType == NodeType.BookmarkStart) &&
                    (((BookmarkStart)FirstInsertedNode).Name == "BlockLevelBookmark1") &&
                    (FirstInsertedNode.ParentNode.NodeType == NodeType.Paragraph))
                    prevNode = FirstInsertedNode.ParentNode.PreviousSibling;


                return ((prevNode != null) && (prevNode.NodeType == NodeType.Paragraph))
                    ? (Paragraph)prevNode
                    : null;
            }
        }

        /// <summary>
        /// Gets paragraph after inserted content.
        /// </summary>
        internal Paragraph ParaAfterInsertedContent
        {
            get
            {
                if (mCurrentDstDocLocation == TestInsertDstDocLocation.InlineSdt)
                    return (Paragraph) mNodeAfterInsertedContent.FirstNonMarkupParentNode.NextSibling;

                // By design node after inserted content either is a run or a paragraph.
                if (mNodeAfterInsertedContent.NodeType == NodeType.Paragraph)
                    return (Paragraph) mNodeAfterInsertedContent;

                return (Paragraph) mNodeAfterInsertedContent.ParentNode;
            }
        }

        /// <summary>
        /// Gets first inserted section.
        /// </summary>
        internal Section FirstInsertedSection
        {
            get
            {
                return (Section)FirstInsertedNode.GetAncestor(NodeType.Section);
            }
        }

        /// <summary>
        /// Gets second inserted section.
        /// </summary>
        internal Section SecondInsertedSection
        {
            get
            {
                return (Section)FirstInsertedSection.NextSibling;
            }
        }

        /// <summary>
        /// Gets third inserted section.
        /// </summary>
        internal Section ThirdInsertedSection
        {
            get
            {
                return (SecondInsertedSection == null) ? null : (Section)SecondInsertedSection.NextSibling;
            }
        }

        /// <summary>
        /// Gets text of first paragraph of inserted content.
        /// </summary>
        internal string FirstInsertedParaText
        {
            get
            {
                return FirstInsertedPara.GetText();
            }
        }

        /// <summary>
        /// Gets first paragraph in second section of inserted content.
        /// </summary>
        internal Paragraph FirstInsertedParaInSecondSection
        {
            get
            {
                if (SecondInsertedSection == null)
                    return null;

                return (Paragraph)SecondInsertedSection.Body.GetChild(NodeType.Paragraph, 0, true);
            }
        }

        /// <summary>
        /// Gets first paragraph in third section of inserted content.
        /// </summary>
        internal Paragraph FirstInsertedParaInThirdSection
        {
            get
            {
                if (ThirdInsertedSection == null)
                    return null;

                return (Paragraph)ThirdInsertedSection.Body.GetChild(NodeType.Paragraph, 0, true);
            }
        }

        private Node mFirstInsertedNode;
        private Node mNodeAfterInsertedContent;
        private Document mDocument;

        // Current location in destination document.
        private TestInsertDstDocLocation mCurrentDstDocLocation;

        /// <summary>
        /// All possible locations in the document where we can insert another one.
        /// JAVA-changed to not use enum.GetValues() and enum.GetName().
        /// </summary>
        private static readonly TestInsertDstDocLocation[] gPossibleLocations = 
        { 
            TestInsertDstDocLocation.BeginningOfListItem,
            TestInsertDstDocLocation.BlockLevelSdt,
            TestInsertDstDocLocation.Cell,
            TestInsertDstDocLocation.Comment,
            TestInsertDstDocLocation.DocumentEnd,
            TestInsertDstDocLocation.DocumentStart,
            TestInsertDstDocLocation.EndOfListItem,
            TestInsertDstDocLocation.Footnote,
            TestInsertDstDocLocation.Header,
            TestInsertDstDocLocation.InlineSdt,
            TestInsertDstDocLocation.MiddleWithSpaceAfter,
            TestInsertDstDocLocation.MiddleWithSpaceBefore,
            TestInsertDstDocLocation.Middle,
            TestInsertDstDocLocation.MiddleOfListItem,
            TestInsertDstDocLocation.TextBox
        };

        /// <summary>
        /// All possible locations inside list item.
        /// </summary>
        private static readonly TestInsertDstDocLocation[] gListItemLocations =
        {
            TestInsertDstDocLocation.BeginningOfListItem,
            TestInsertDstDocLocation.MiddleOfListItem,
            TestInsertDstDocLocation.EndOfListItem
        };
    }
}
