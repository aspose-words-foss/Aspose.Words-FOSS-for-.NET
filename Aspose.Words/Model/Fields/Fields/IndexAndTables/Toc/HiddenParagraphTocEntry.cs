// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2017 by Edward Voronov

using System;
using Aspose.Words.Lists;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// A TOC entry that is represented by a paragraph with list label whithin field code.
    /// </summary>
    internal class HiddenParagraphTocEntry : ITocEntry
    {
        internal HiddenParagraphTocEntry(Paragraph paragraph, int level)
        {
            Debug.Assert(paragraph.IsListItemFinal);
            mParagraph = paragraph;
            mLevel = level;
        }

        NodeRange ITocEntry.InsertBookmark(string bookmarkName)
        {
            BookmarkStart bookmarkStart = new BookmarkStart(Document, bookmarkName);
            BookmarkEnd bookmarkEnd = new BookmarkEnd(Document, bookmarkName);

            mParagraph.AppendChild(bookmarkStart);
            mParagraph.AppendChild(bookmarkEnd);

            return NodeRange.Void;
        }

        int ITocEntry.Level
        {
            get { return mLevel; }
        }

        bool ITocEntry.OmitPageNumber
        {
            get { return false; }
        }

        Paragraph ITocEntry.Paragraph { get { return mParagraph; } }

        string ITocEntry.GetDocumentOutlineTitle()
        {
            return GetParagraphListLabel();
        }

        NodeRange ITocEntry.GetLabelRange()
        {
            Run run = new Run(Document, GetParagraphListLabel());
            return new NodeRange(run, run);
        }

        private string GetParagraphListLabel()
        {
            int listId = (int)((IParaAttrSource)mParagraph).FetchParaAttr(ParaAttr.ListId);
            int listLevel = (int)((IParaAttrSource)mParagraph).FetchParaAttr(ParaAttr.ListLevel);

            List list = mParagraph.Document.Lists.FetchListByListId(listId);

            ListNumberState numberState = new ListNumberState(list);
            numberState.NextItem(list, listLevel);

            return ListLabelUtil.BuildFullListLabel(numberState, null, null);
        }

        bool ITocEntry.IsInFieldCode
        {
            get { return true; }
        }

        bool ITocEntry.HasBookmark
        {
            get { return true; }
        }

        bool ITocEntry.IsLinkedStyleTocEntry
        {
            get { return false; }
        }

        int ITocEntry.GetSequenceValue(string sequenceIdentifier)
        {
            throw new InvalidOperationException();
        }

        int ITocEntry.GetPageNumber()
        {
            throw new InvalidOperationException();
        }

        private Document Document
        {
            get { return mParagraph.FetchDocument(); }
        }

        private readonly Paragraph mParagraph;
        private readonly int mLevel;
    }
}
