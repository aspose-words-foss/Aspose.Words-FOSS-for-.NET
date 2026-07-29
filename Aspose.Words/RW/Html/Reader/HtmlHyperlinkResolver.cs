// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/10/2022 by Artem Shabarshin

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.RW.Html.Parser;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Adds bookmarks for imported HTML elements with 'id' attributes
    /// so that they can be referenced by hyperlinks in the same document.
    /// </summary>
    /// <remarks>
    /// Inline, anchor and block bookmarks are processed in different ways.
    /// Inline and anchor bookmarks are inserted into the document model immediately.
    /// Block bookmarks are remembered and inserted after
    /// the whole HTML tree has been processed. This allows to take the structure of block-level elements into account.
    /// </remarks>
    internal class HtmlHyperlinkResolver
    {
        internal HtmlHyperlinkResolver(IHyperlinkProcessor hyperlinkProcessor, HashSetGeneric<string> bookmarkNames)
        {
            mHyperlinkProcessor = hyperlinkProcessor;
            mReferencedElementIds = new HashSetGeneric<string>();
            mBookmarkInfos = new BookmarkInfos();
            mRememberedBookmarks = bookmarkNames;
            mStartedAnchorBookmarks = new HashSetGeneric<string>();
            mProcessedAnchorBookmarks = new HashSetGeneric<string>();
            BookmarkEnds = new StringToObjDictionary<BookmarkEnd>();
        }

        /// <summary>
        /// Initializes the HTML hyperlink resolver.
        /// </summary>
        internal void Init(
            DocumentBuilder documentBuilder,
            HtmlBidiTextArranger bidiTextArranger,
            bool rememberExistingBookmarks)
        {
            mBuilder = documentBuilder;
            mBidiTextArranger = bidiTextArranger;

            // If the calling code doesn't want this class to remember existing bookmarks, it shouldn't provide a bookmark
            // name cache to the constructor of this class.
            Debug.Assert(!rememberExistingBookmarks || (mRememberedBookmarks == null));

            // If the bookmark cache is non-null, collection of existing bookmarks in this class is turned off.
            // This happens in two cases:
            //   either the calling code collects and provides remembered bookmarks on its own;
            //   or the calling code doesn't want this class to collect and check existing bookmarks at all.
            if (!rememberExistingBookmarks && (mRememberedBookmarks == null))
            {
                mRememberedBookmarks = new HashSetGeneric<string>();
            }
        }

        /// <summary>
        ///  Starts a bookmark from anchor or bookmark span in model.
        /// </summary>
        internal void StartAnchorBookmarkIfNeeded(string bookmarkName, bool parentNodeIsParagraph, bool isBookmarkSpan)
        {
            if (!StringUtil.HasChars(bookmarkName))
                return;

            string bookmarkNameLower = bookmarkName.ToLowerInvariant();
            if (ContainsBookmark(bookmarkNameLower))
                return;

            if (isBookmarkSpan)
            {
                // Bookmark boundaries are hard to preserve during reordering by the Unicode bidirectional algorithm (UBA),
                // so at the moment bookmark contents are isolated from the UBA to make sure it is reordered as a whole.
                mBidiTextArranger.RearrangeAndWriteText();
            }

            // WORDSNET-20553 If start of a bookmark is located just before SDT
            // and it is not in a paragraph then it should be moved to the end of the SDT.
            if (!parentNodeIsParagraph &&
                (mBuilder.CurrentParagraph.PrevNode != null) &&
                !mBuilder.CurrentParagraph.HasChildNodes &&
                (mBuilder.CurrentParagraph.PrevNode.NodeType == NodeType.StructuredDocumentTag))
            {
                BookmarkStart start = new BookmarkStart(mBuilder.Document, bookmarkName);
                StructuredDocumentTag sdt = (StructuredDocumentTag)mBuilder.CurrentParagraph.PrevNode;
                sdt.InsertAfter(start, sdt.LastChild);
            }
            else
            {
                mBuilder.StartBookmark(bookmarkName);
            }

            mStartedAnchorBookmarks.Add(bookmarkNameLower);
        }

        /// <summary>
        ///  Ends a bookmark from anchor or bookmark span in model.
        /// </summary>
        internal void EndAnchorBookmarkIfNeeded(string bookmarkName, bool parentNodeIsParagraph, bool isBookmarkSpan)
        {
            if (!StringUtil.HasChars(bookmarkName))
                return;

            string bookmarkNameLower = bookmarkName.ToLowerInvariant();
            if (mProcessedAnchorBookmarks.Contains(bookmarkNameLower))
                return;

            if (!mStartedAnchorBookmarks.Contains(bookmarkNameLower))
                return;

            if (isBookmarkSpan)
            {
                // Bookmark boundaries are hard to preserve during reordering by the Unicode bidirectional algorithm (UBA),
                // so at the moment bookmark contents are isolated from the UBA to make sure it is reordered as a whole.
                mBidiTextArranger.RearrangeAndWriteText();
            }

            if (BookmarkEnds.ContainsKey(bookmarkName))
            {
                BookmarkEnd bookmarkEnd = BookmarkEnds[bookmarkName];
                bookmarkEnd.Remove();
                BookmarkEnds.Remove(bookmarkName);
            }
            BookmarkEnd newBookmarkEnd;

            // WORDSNET-20553 If start of a bookmark is located just before SDT
            // and it is not in a paragraph then it should be moved to the end of the SDT.
            if (!parentNodeIsParagraph &&
                (mBuilder.CurrentParagraph.PrevNode != null) &&
                !mBuilder.CurrentParagraph.HasChildNodes &&
                (mBuilder.CurrentParagraph.PrevNode.NodeType == NodeType.StructuredDocumentTag))
            {
                newBookmarkEnd = new BookmarkEnd(mBuilder.Document, bookmarkName);
                StructuredDocumentTag sdt = (StructuredDocumentTag)mBuilder.CurrentParagraph.PrevNode;
                sdt.InsertAfter(newBookmarkEnd, sdt.LastChild);
            }
            else
            {
                newBookmarkEnd = mBuilder.EndBookmark(bookmarkName);
            }
            BookmarkEnds.Add(bookmarkName, newBookmarkEnd);

            if (isBookmarkSpan)
                mProcessedAnchorBookmarks.Add(bookmarkNameLower);
        }

        /// <summary>
        /// Start a new inline bookmark.
        /// </summary>
        internal BookmarkStart StartInlineBookmarkIfNeeded(IHtmlElementProvider element)
        {
            string elementId = GetElementId(element);
            if (!StringUtil.HasChars(elementId))
                return null;

            if (ContainsBookmark(elementId))
                return null;

            // Create an inline bookmark inside the current paragraph.
            BookmarkStart bookmarkStart = mBuilder.StartBookmark(elementId);
            BookmarkEnd bookmarkEnd = new BookmarkEnd(mBuilder.Document);
            bookmarkEnd.Name = elementId;
            Bookmark bookmark = new Bookmark(bookmarkStart, bookmarkEnd);
            mBookmarkInfos.AddInlineBookmark(elementId, bookmark);
            return bookmarkStart;
        }

        /// <summary>
        /// Ends an inline bookmark.
        /// </summary>
        internal BookmarkEnd EndInlineBookmarkIfNeeded(IHtmlElementProvider element)
        {
            string elementId = GetElementId(element);
            if (!StringUtil.HasChars(elementId))
                return null;

            Bookmark bookmark = mBookmarkInfos.GetInlineBookmark(elementId);
            if (bookmark == null)
                return null;

            // It is a duplicated end of bookmark.
            if (bookmark.BookmarkEnd.ParentNode != null)
                return null;

            mBuilder.InsertNode(bookmark.BookmarkEnd);
            BookmarkEnds.Add(elementId, bookmark.BookmarkEnd);
            return bookmark.BookmarkEnd;
        }

        /// <summary>
        /// Marks current paragraph as a node after which bookmark will be added.
        /// </summary>
        internal void AddPendingBookmarkIfNeeded(HtmlElementNode elementNode)
        {
            string elementId = GetElementId(elementNode);
            if (!StringUtil.HasChars(elementId))
                return;

            if (ContainsBookmark(elementId))
                return;

            NodePair nodePair = new NodePair();
            nodePair.StartBookmarkNode = mBuilder.CurrentParagraph;
            mBookmarkInfos.AddPendingBookmark(elementId, mBuilder.CurrentParagraph, nodePair);
        }

        /// <summary>
        /// Marks the table as a node after which bookmark will be added.
        /// </summary>
        internal void AddPendingBookmarkForTableIfNeeded(HtmlElementNode tableNode, Table table)
        {
            // The started table is inserted before current paragraph.
            // We should update the node before which the bookmarks will be inserted.
            mBookmarkInfos.UpdatePendingStartBookmark(mBuilder.CurrentParagraph, table);

            string elementId = GetElementId(tableNode);
            if (!StringUtil.HasChars(elementId))
                return;

            if (ContainsBookmark(elementId))
                return;

            NodePair nodePair = new NodePair();
            nodePair.StartBookmarkNode = table;
            nodePair.EndBookmarkNode = table;
            mBookmarkInfos.AddPendingBookmark(elementId, mBuilder.CurrentParagraph, nodePair);
        }

        /// <summary>
        /// Marks current paragraph as a node after which a start of bookmark will be added.
        /// </summary>
        internal void AddPendingStartBookmarkIfNeeded(HtmlElementNode elementNode)
        {
            string elementId = GetElementId(elementNode);
            if (!StringUtil.HasChars(elementId))
                return;

            if (ContainsBookmark(elementId))
                return;

            NodePair nodePair = new NodePair();
            nodePair.StartBookmarkNode = mBuilder.CurrentParagraph;
            mBookmarkInfos.AddPendingBookmark(elementId, mBuilder.CurrentParagraph, nodePair);
        }

        /// <summary>
        /// Marks current paragraph as a node after which an end of bookmark will be added.
        /// </summary>
        internal void AddPendingEndBookmarkIfNeeded(HtmlElementNode elementNode)
        {
            string elementId = GetElementId(elementNode);
            if (!StringUtil.HasChars(elementId))
                return;

            NodePair nodePair = mBookmarkInfos.GetPendingBookmark(elementId);
            if (nodePair == null)
                return;

            // If the end of bookmark is not null then the element node has duplicated bookmark.
            if (nodePair.EndBookmarkNode == null)
                nodePair.EndBookmarkNode = mBuilder.CurrentParagraph;
        }

        /// <summary>
        /// Inserts pending bookmarks and optionally removes unreferenced bookmarks.
        /// </summary>
        /// <remarks>
        /// A bookmark is unreferenced if it is created from an element with the 'id' attribute but is not referenced
        /// by any hyperlink in the document.
        /// </remarks>
        internal void ProcessBookmarksAndHyperlinks(bool keepUnreferencedIdBookmarks)
        {
            foreach (string hyperlink in mBookmarkInfos.GetPendingBookmarks())
            {
                // We create unreferenced pending bookmarks only if we're asked to keep them. We don't want to create bookmarks
                // that we'll have to remove right away.
                if (keepUnreferencedIdBookmarks || mReferencedElementIds.Contains(hyperlink))
                {
                    NodePair nodePair = mBookmarkInfos.GetPendingBookmark(hyperlink);
                    if (nodePair == null)
                        continue;

                    CompositeNode startBookmarkParentNode = nodePair.StartBookmarkNode.ParentNode;
                    BookmarkStart bookmarkStart = new BookmarkStart(mBuilder.Document, hyperlink);
                    BookmarkEnd bookmarkEnd = new BookmarkEnd(mBuilder.Document, hyperlink);
                    if (nodePair.EndBookmarkNode != null)
                    {
                        startBookmarkParentNode.InsertBefore(bookmarkStart, nodePair.StartBookmarkNode);
                        nodePair.EndBookmarkNode.InsertNext(bookmarkEnd);
                    }
                    else
                    {
                        // Insert a bookmark after the corresponding node.
                        startBookmarkParentNode.InsertAfter(bookmarkStart, nodePair.StartBookmarkNode);
                        startBookmarkParentNode.InsertAfter(bookmarkEnd, bookmarkStart);
                    }
                    BookmarkEnds.Add(hyperlink, bookmarkEnd);
                }
            }

            // Remove unused inline bookmarks if asked to.
            if (!keepUnreferencedIdBookmarks)
            {
                StringToObjDictionary<Bookmark>.Enumerator enumerator = mBookmarkInfos.GetInlineBookmarksEnumerator();
                while (enumerator.MoveNext())
                {
                    if (mReferencedElementIds.Contains(enumerator.CurrentKey))
                        continue;

                    if (enumerator.CurrentValue != null)
                    {
                        enumerator.CurrentValue.Remove();
                        BookmarkEnds.Remove(enumerator.CurrentValue.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a hyperlink if it starts from '#'.
        /// </summary>
        internal void AddHyperlink(string hyperlink)
        {
            if (UriUtil.IsSubAddressOnly(hyperlink))
            {
                string hyperlinkName = UriUtil.GetSubAddress(hyperlink).ToLowerInvariant();
                if (!mReferencedElementIds.Contains(hyperlinkName))
                    mReferencedElementIds.Add(hyperlinkName);
            }
        }

        /// <summary>
        /// Indicates whether the corresponding anchor bookmark was created or not.
        /// </summary>
        internal bool ContainsStartedAnchorBookmark(string bookmarkName)
        {
            return mStartedAnchorBookmarks.Contains(bookmarkName.ToLowerInvariant());
        }

        /// <summary>
        /// Holds the ends of bookmarks created during import HTML document.
        /// Keys - bookmark names. Values - ends of bookmarks.
        /// </summary>
        internal StringToObjDictionary<BookmarkEnd> BookmarkEnds { get; }

        /// <summary>
        /// Indicates whether the corresponding bookmark (the start and the end) was created before reading the HTML document or
        /// during reading the HTML document.
        /// </summary>
        private bool ContainsBookmark(string bookmarkName)
        {
            string bookmarkNameLower = bookmarkName.ToLowerInvariant();
            if (mBookmarkInfos.ContainsBookmark(bookmarkNameLower) ||
                mStartedAnchorBookmarks.Contains(bookmarkNameLower))
            {
                return true;
            }

            // Check if a bookmark with the same name had been created in the document before we started to import HTML.
            // WORDSNET-28606 This check implies traversing the whole document tree in order to find all bookmarks, which may
            // be quite slow for big documents. Since not all HTML documents create bookmarks, it is a good idea to delay
            // tree traversal until we really need to insert a bookmark.
            if (mRememberedBookmarks == null)
            {
                mRememberedBookmarks = new HashSetGeneric<string>();
                // WORDSNET-28606 We used to use Document.GetChildNodes(NodeType.BookmarkStart, true) but it was noticeably
                // slower than the approach with DocumentVisitor.
                mBuilder.Document.Accept(new BookmarkStartCollector(mRememberedBookmarks));
            }
            return mRememberedBookmarks.Contains(bookmarkNameLower);
        }

        /// <summary>
        /// Returns normalized element Id if the corresponding HTML element node contains 'id' attribute
        /// or an empty string otherwise.
        /// </summary>
        private string GetElementId(IHtmlElementProvider element)
        {
            string idValue = element.GetAttributeValue("id");
            if (idValue == null)
            {
                return string.Empty;
            }

            // Element IDs are used as bookmark names, which are case-insensitive.
            idValue = idValue.ToLowerInvariant();

            // We need to perform format-specific processing of element IDs, because they will be used as bookmark names.
            if (mHyperlinkProcessor != null)
            {
                idValue = mHyperlinkProcessor.MapBookmarkName(idValue);
            }

            return idValue;
        }

        /// <summary>
        /// If specified, this processor is used to map bookmark names imported from HTML into unique bookmark names that
        /// can be used in the resulting document without conflicts.
        /// </summary>
        private readonly IHyperlinkProcessor mHyperlinkProcessor;

        /// <summary>
        /// Holds IDs of HTML elements that are referenced by hyperlinks. Items are in lower case.
        /// </summary>
        private readonly HashSetGeneric<string> mReferencedElementIds;

        private readonly BookmarkInfos mBookmarkInfos;

        /// <summary>
        /// Holds names of started anchor bookmarks.
        /// </summary>
        private readonly HashSetGeneric<string> mStartedAnchorBookmarks;

        /// <summary>
        /// Holds names of processed anchor bookmarks.
        /// </summary>
        private readonly HashSetGeneric<string> mProcessedAnchorBookmarks;

        private DocumentBuilder mBuilder;

        private HtmlBidiTextArranger mBidiTextArranger;

        /// <summary>
        /// Holds names of bookmarks that already were in the model before loading of HTML started. Items are in lower case.
        /// </summary>
        private HashSetGeneric<string> mRememberedBookmarks;

        /// <summary>
        /// Holds the nodes after or before which starts or ends of bookmark will be inserted
        /// during processing bookmarks and hyperlinks.
        /// </summary>
        private class NodePair
        {
            internal Node StartBookmarkNode { get; set; }

            internal Node EndBookmarkNode { get; set; }
        }

        /// <summary>
        /// Holds information about bookmarks created from the HTML elements with 'id' attribute.
        /// </summary>
        private class BookmarkInfos
        {
            internal BookmarkInfos()
            {
                mInlineBookmarksById = new StringToObjDictionary<Bookmark>();
                mPendingBookmarkNodes = new StringToObjDictionary<NodePair>();
                mPendingBookmarkOrder = new List<string>();
                mPendingNodes = new Dictionary<Node, List<string>>();
            }

            internal void AddInlineBookmark(string elementId, Bookmark bookmark)
            {
                Debug.Assert(elementId == elementId.ToLowerInvariant());
                Debug.Assert(!mInlineBookmarksById.ContainsKey(elementId));
                mInlineBookmarksById.Add(elementId, bookmark);
            }

            internal Bookmark GetInlineBookmark(string elementId)
            {
                return mInlineBookmarksById[elementId];
            }

            internal void AddPendingBookmark(string elementId, Node pendingNode, NodePair nodePair)
            {
                Debug.Assert(elementId == elementId.ToLowerInvariant());
                Debug.Assert(!mPendingBookmarkNodes.ContainsKey(elementId));
                mPendingBookmarkNodes.Add(elementId, nodePair);
                mPendingBookmarkOrder.Add(elementId);
                List<string> bookmarks;
                if (!mPendingNodes.TryGetValue(pendingNode, out bookmarks))
                {
                    bookmarks = new List<string>();
                    mPendingNodes.Add(pendingNode, bookmarks);
                }
                bookmarks.Add(elementId);
            }

            internal NodePair GetPendingBookmark(string elementId)
            {
                return mPendingBookmarkNodes[elementId];
            }

            internal List<string> GetPendingBookmarks()
            {
                return mPendingBookmarkOrder;
            }

            internal StringToObjDictionary<Bookmark>.Enumerator GetInlineBookmarksEnumerator()
            {
                return mInlineBookmarksById.GetEnumerator();
            }

            internal bool ContainsBookmark(string bookmarkName)
            {
                Debug.Assert(bookmarkName == bookmarkName.ToLowerInvariant());
                return mInlineBookmarksById.ContainsKey(bookmarkName) || mPendingBookmarkNodes.ContainsKey(bookmarkName);
            }

            /// <summary>
            /// Replaces the old node with new one for the pending start of bookmark.
            /// </summary>
            internal void UpdatePendingStartBookmark(Node oldNode, Node newNode)
            {
                List<string> bookmarkNames;
                if (mPendingNodes.TryGetValue(oldNode, out bookmarkNames))
                {
                    mPendingNodes.Remove(oldNode);
                    mPendingNodes[newNode] = bookmarkNames;
                    foreach (string bookmarkName in bookmarkNames)
                    {
                        NodePair nodePair = GetPendingBookmark(bookmarkName);
                        if (nodePair != null)
                        {
                            nodePair.StartBookmarkNode = newNode;
                        }
                    }
                }
            }

            /// <summary>
            /// Holds the bookmarks created from HTML elements with 'id' attribute.
            /// Keys - values of 'id' attribute. Values - bookmarks.
            /// </summary>
            private readonly StringToObjDictionary<Bookmark> mInlineBookmarksById;

            /// <summary>
            /// Holds nodes after which a start and an end of bookmark will be inserted during processing bookmarks and hyperlinks.
            /// Keys - bookmark names. Values - pairs of nodes.
            /// </summary>
            private readonly StringToObjDictionary<NodePair> mPendingBookmarkNodes;

            /// <summary>
            /// Holds bookmarks in the order of their creation.
            /// </summary>
            private readonly List<string> mPendingBookmarkOrder;

            /// <summary>
            /// Holds nodes for which bookmarks will be inserted during processing bookmarks and hyperlinks.
            /// Keys - Nodes. Values - bookmark names.
            /// </summary>
            private readonly Dictionary<Node, List<string>> mPendingNodes;
        }

        /// <summary>
        /// Collects lowercase names of all bookmark start nodes in a document.
        /// </summary>
        private class BookmarkStartCollector : DocumentVisitor
        {
            internal BookmarkStartCollector(HashSetGeneric<string> bookmarkNames)
            {
                mBookmarkNames = bookmarkNames;
            }

            public override VisitorAction VisitBookmarkStart(BookmarkStart bookmarkStart)
            {
                mBookmarkNames.Add(bookmarkStart.Name.ToLowerInvariant());
                return VisitorAction.Continue;
            }

            private readonly HashSetGeneric<string> mBookmarkNames;
        }
    }
}
