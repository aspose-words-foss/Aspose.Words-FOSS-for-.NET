// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2017 by Nikolay Sezganov

using System.Collections.Generic;
using Aspose.Collections.Generic;
using Aspose.Words.Fields;
using Aspose.Words.Lists;
using Aspose.Words.RW.Html.Css;
using Aspose.Words.RW.Html.Reader.CommonBorder;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Arranges paragraphs imported from HTML and inserts them into the document model.
    /// </summary>
    internal class HtmlParagraphArranger
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal HtmlParagraphArranger(
            HtmlReader htmlReader,
            HtmlBidiTextArranger bidiTextArranger,
            DocumentBuilder builder,
            DocumentFormatter documentFormatter,
            HtmlCommonBorderResolver commonBorderResolver,
            ParaPr baseParaPr,
            HtmlListReaderManager listReaderManager,
            HtmlBlockReader htmlBlockReader,
            bool canReuseListItem)
        {
            mHtmlReader = htmlReader;
            mBidiTextArranger = bidiTextArranger;
            mBuilder = builder;
            mDocumentFormatter = documentFormatter;
            mCommonBorderResolver = commonBorderResolver;
            mBaseParaPr = baseParaPr;
            mListReaderManager = listReaderManager;

            IsParaStartNeeded = false;
            mCanReuseListItem = canReuseListItem;

            mIsLastParagraphCreatedImplicitly = true;

            mCurrentParagraphs = new Stack<Paragraph>();

            mEmptyInHtmlParagraphs = new HashSetGeneric<Paragraph>();
            mHtmlBlockReader = htmlBlockReader;
        }

        internal void StartParaIfNeeded()
        {
            if (IsParaStartNeeded)
            {
                bool newParagraphStarted = StartPara();
                if (newParagraphStarted)
                {
                    mCommonBorderResolver.CollectParagraphWithoutBorder(mBuilder.CurrentParagraph);
                    mIsLastParagraphCreatedImplicitly = true;
                }

                mHtmlReader.ApplyParagraphFormatting();

                if (newParagraphStarted)
                    mHtmlBlockReader.UpdateCurrentParagraph(false);
            }
        }

        internal bool StartPara()
        {
            // We explicitly wrote text at the end of the previous paragraph.
            Debug.Assert(mBidiTextArranger.IsEmpty);

            bool isNewParaStarted = false;

            bool reuseCurrentParagraph = CanReuseCurrentParagraph();
            if (!reuseCurrentParagraph)
            {
                mBidiTextArranger.RearrangeAndWriteText();

                mBuilder.EnsureAtStructuredDocumentTagEnd();

                mBuilder.InsertParagraph();
                isNewParaStarted = true;
            }

            IsParaStartNeeded = false;
            mIsLastParagraphCreatedImplicitly = false;

            // Let's preserve list id since paragraphs inside lists should be list items or should be indented.
            int oldListId = mBuilder.ParagraphFormat.ListId;
            ListLevel oldListLevel = (oldListId != 0) && (mListReaderManager != null) &&
                                     (mListReaderManager.CurrentListReader != null)
                ? mListReaderManager.CurrentListReader.CurrentListLevel
                : null;

            // Preserve the HTML block for the current paragraph.
            // It allows to prevent breaking the HTML block
            // during inserting the HTML document into paragraph enclosed by the HTML block.
            int htmlBlockId = mBuilder.CurrentParagraph.ParaPr.HtmlBlockId;

            // This is the beginning of building up new paragraph formatting upon the base formatting.
            mBuilder.CurrentParagraph.ParaPr = mBaseParaPr.Clone();

            if (htmlBlockId != 0)
                mBuilder.CurrentParagraph.ParaPr.HtmlBlockId = htmlBlockId;

            // RK We are preserving list formatting. I am not sure why, I am just adding a comment.
            if ((mListReaderManager != null) && (mListReaderManager.CurrentListReader != null) &&
                (mListReaderManager.CurrentListReader.CurrentLevelNumber != -1) && (oldListId != 0) &&
                reuseCurrentParagraph)
            {
                //Do that if some text wasn't already read
                mBuilder.ParagraphFormat.ListId = oldListId;
                // WORDSNET-7191 <p> inside <li> which is nested in a <ol> is imported incorrectly.
                // Restore paragraph list level to avoid this issue.
                if (oldListLevel != null)
                    mBuilder.ParagraphFormat.ListLevel = oldListLevel.LevelNumber;
            }

            // Font formatting of the HTML block element being processed is translated into font formatting of the corresponding
            // paragraph break character. Paragraph break character formatting affects list label formatting, so it is important
            // to keep it correct.
            mHtmlReader.ApplyFontFormatting(true);
            mBuilder.CurrentParagraph.ParagraphBreakRunPr = mBuilder.GetRunPrCopy();

            return isNewParaStarted;
        }

        internal void EndPara()
        {
            // We explicitly wrote pending text at the end of the block element.
            Debug.Assert(mBidiTextArranger.IsEmpty);

            // WORDSNET-22574 List items must always be closed at end tags, because the corresponding paragraphs must
            // not be reused. If we don't do this, empty list items will not be imported from HTML in sertain scenarios.
            if (mDocumentFormatter.CurrentElementHasChildren || IsAtStartOfListItem())
            {
                IsParaStartNeeded = true;
            }
        }

        /// <summary>
        /// Sets a value indicating that the current paragraph is empty in the HTML document.
        /// </summary>
        internal void MarkCurrentParagraphAsEmptyInHtml()
        {
            mEmptyInHtmlParagraphs.Add(mBuilder.CurrentParagraph);
        }

        /// <summary>
        /// Starts a new paragraph if the current paragraph is empty in the HTML document.
        /// </summary>
        internal void CloseCurrentParagraphIfEmptyInHtml()
        {
            if (IsParagraphEmptyInHtml(mBuilder.CurrentParagraph))
            {
                mBuilder.InsertParagraph();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified paragraph is empty in the HTML document.
        /// </summary>
        internal bool IsParagraphEmptyInHtml(Paragraph paragraph)
        {
            return mEmptyInHtmlParagraphs.Contains(paragraph);
        }

        /// <summary>
        /// Remembers the current paragraph of DocumentBuilder and moves its cursor to the specified node.
        /// Used to temporarily move cursor while writing footnotes, endnotes, headers, footers, and SDTs.
        /// </summary>
        internal void RememberCurrentParagraphAndMoveTo(Node node)
        {
            Debug.Assert(node != null);
            Debug.Assert(mBidiTextArranger.IsEmpty);
            Debug.Assert(mBuilder.CurrentParagraph != null);

            mCurrentParagraphs.Push(mBuilder.CurrentParagraph);
            mBuilder.MoveTo(node);
        }

        /// <summary>
        /// Restores the current paragraph of DocumentBuilder after writing footnotes, endnotes, headers, footers, and SDTs.
        /// </summary>
        internal void RestoreCurrentParagraphIfNeeded()
        {
            Debug.Assert(mBidiTextArranger.IsEmpty);

            if (mCurrentParagraphs.Count > 0)
            {
                Paragraph currentParagraph = mCurrentParagraphs.Pop();
                mBuilder.MoveTo(currentParagraph);
            }
        }

        internal void InsertEmptyParagraphToListItemIfNeeded()
        {
            if (IsAtStartOfListItem() && !IsListItemNoneListStyle())
            {
                Debug.Assert(mBuilder.ParagraphFormat.IsListItem);
                mBuilder.InsertParagraph();
            }
        }

        internal void FlushLastParagraphIfNeeded(ParaPr paraPr, RunPr paragraphBreakRunPr)
        {
            if (IsParaStartNeeded)
            {
                // WORDSNET-22574 List items must always be closed at end tags, because the corresponding paragraphs must
                // not be reused. If we don't do this, empty list items will not be imported from HTML in sertain scenarios.
                if (!mBuilder.IsAtStartOfParagraph || mBuilder.CurrentParagraph.IsListItem)
                {
                    bool needToMoveLastParagraph =
                        (mBuilder.CurrentParagraph.NextSibling != null) &&
                        (mBuilder.CurrentParagraph.NextSibling.NodeType == NodeType.BookmarkEnd);

                    mBuilder.EnsureAtStructuredDocumentTagEnd();

                    Paragraph lastParagraph = mBuilder.InsertParagraph();
                    // We should move the last paragraph at the end of all nodes if
                    // the previous paragraph has the end of bookmarks after itself.
                    if (needToMoveLastParagraph)
                    {
                        Node lastNode = mBuilder.CurrentParagraph.NextSibling;
                        while (lastNode.NextSibling != null)
                        {
                            lastNode = lastNode.NextSibling;
                        }
                        lastNode.InsertNext(lastParagraph);
                    }

                }
                mBuilder.CurrentParagraph.ParaPr = paraPr;
                mBuilder.CurrentParagraph.ParagraphBreakRunPr = paragraphBreakRunPr;
            }
        }

        internal void InsertParagraphImmediately()
        {
            mBuilder.InsertParagraph();
        }

        internal void SetIsParaStartNeededIfNodeIsBlockLevel()
        {
            if (!IsParaStartNeeded && mDocumentFormatter.IsDisplayedAsBlock())
            {
                IsParaStartNeeded = true;
            }
        }

        internal void DebugAssertHasNoRememberedParagraphs()
        {
            Debug.Assert(mCurrentParagraphs.Count == 0);
        }

        /// <summary>
        /// Checks if the current paragraph can be re-used instead of inserting a new paragraph.
        /// </summary>
        internal bool CanReuseCurrentParagraph()
        {
            // Empty paragraphs exported by Aspose.Words must remain empty and cannot be re-used.
            if (IsParagraphEmptyInHtml(mBuilder.CurrentParagraph))
            {
                return false;
            }

            // WORDSNET-18009, WORDSNET-18968 If HTML document is inserted into empty list item, this list item is allowed
            // to be re-used by another list item, so this check is allowed only once for the first paragraph.
            // Other list items can never be re-used by list items.
            bool canReuseListItem = mCanReuseListItem;
            mCanReuseListItem = false;

            // WORDSNET-11569 Empty list item in multilevel list should contain at least one paragraph.
            if (mBuilder.CurrentParagraph.IsListItem &&
                (mDocumentFormatter.ElementDisplayType == CssDisplayType.ListItem) &&
                !canReuseListItem)
            {
                return false;
            }

            // Since empty blocks collapse in HTML, empty paragraphs can be re-used.
            if (mBuilder.IsAtStartOfParagraph)
            {
                return true;
            }

            // WORDSNET-17841 We incorrectly handle cases where a block is nested inside an inline-level anchor
            // element. For example, <a><img style="display:block"></a>
            // We open a hyperlink field when we process the <a> element and this makes the current paragraph non-empty.
            // As a result, when we process the <img> element, we insert a paragraph break, because the image is block-level
            // and it must be placed in a separate paragraph.
            // The code below detects the case where the current paragraph contains nothing but a hyperlink start and allows
            // to re-use the paragraph for block-level elements by marking it empty.
            //
            // Please note that this is a quick fix and it doesn't cover all similar scenarios. However, it seems impossible
            // to implement a proper fix without pre-calculation of CSS styles (multi-stage parsing of HTML).
            if (mBuilder.CurrentParagraph.GetChildNodes(NodeType.Any, false).Count == 3)
            {
                // An opened hyperlink field consists of 3 nodes: field start, field code run, and field separator.
                FieldStart fieldStart = mBuilder.CurrentParagraph.FirstChild as FieldStart;
                FieldSeparator fieldSeparator = mBuilder.CurrentParagraph.LastChild as FieldSeparator;
                if ((fieldStart != null) &&
                    (fieldSeparator != null) &&
                    (fieldStart.FieldType == FieldType.FieldHyperlink) &&
                    (fieldSeparator.FieldType == FieldType.FieldHyperlink))
                {
                    return true;
                }
            }

            // Paragraphs containing nothing but floating shapes are considered empty and can be re-used.
            if (HtmlUtil.IsParagraphHasOnlyFloatingShapes(mBuilder.CurrentParagraph))
            {
                return true;
            }

            // Paragraphs containing nothing but page or column breaks are considered empty and can be re-used.
            // WORDSNET-17475 Re-use paragraph with only page or column breaks only if this paragraph was created
            // implicitly.
            if (HtmlUtil.IsParagraphHasOnlyPageOrColumnBreaks(mBuilder.CurrentParagraph) &&
                mIsLastParagraphCreatedImplicitly)
            {
                return true;
            }

            if (HtmlUtil.IsParagraphHasOnlyEmptyRuns(mBuilder.CurrentParagraph))
            {
                return true;
            }

            // Otherwise, the current paragraph is not empty and thus cannot be re-used.
            return false;
        }

        /// <summary>
        /// Gets or sets a value indicating that we reached the end of a block-level element in HTML and should start
        /// a new paragraph before writing more text.
        /// </summary>
        internal bool IsParaStartNeeded { get; set; }

        private bool IsAtStartOfListItem()
        {
            return (mListReaderManager != null) && (mListReaderManager.CurrentListReader != null) &&
                   mListReaderManager.CurrentListReader.IsAtStartOfListItem;
        }

        private bool IsListItemNoneListStyle()
        {
            return (mListReaderManager != null) && (mListReaderManager.CurrentListReader != null) &&
                   mListReaderManager.CurrentListReader.IsListItemNoneListStyle;
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly HtmlReader mHtmlReader;
        private readonly HtmlBidiTextArranger mBidiTextArranger;
        private readonly DocumentBuilder mBuilder;
        private readonly DocumentFormatter mDocumentFormatter;
        // WORDSNET-21193 HtmlParagraphArranger now works with mListReaderManager.CurrentListReader.
        private readonly HtmlListReaderManager mListReaderManager;
        private readonly HtmlCommonBorderResolver mCommonBorderResolver;
        private readonly ParaPr mBaseParaPr;

        private bool mIsLastParagraphCreatedImplicitly;
        private bool mCanReuseListItem;

        /// <summary>
        /// Stack of <see cref="Paragraph"/> elements.
        /// Used to store DocumentBuilder's current paragraph in order to restore it
        /// after moving to another node using DocumentBuilder.MoveTo() function.
        /// Used, for example, when reading content of footnotes, endnotes, headers, footers and SDTs.
        /// </summary>
        private readonly Stack<Paragraph> mCurrentParagraphs;

        /// <summary>
        /// A set of <see cref="Paragraph"/> nodes that correspond to empty paragraphs in the HTML document.
        /// </summary>
        /// <remarks>
        /// Empty paragraphs are actually exported as non-empty HTML elements. In order to prevent empty paragraphs
        /// from collapsing in browsers, AW writes a non-breaking space span into each such paragraph. On import we ignore
        /// that special run but have to process such paragraphs differently from paragraphs that contain nothing (and, as a
        /// result, collapse in the HTML document). That's why we keep track of paragraphs imported from such elements.
        /// </remarks>
        private readonly HashSetGeneric<Paragraph> mEmptyInHtmlParagraphs = new HashSetGeneric<Paragraph>();

        private readonly HtmlBlockReader mHtmlBlockReader;
    }
}
