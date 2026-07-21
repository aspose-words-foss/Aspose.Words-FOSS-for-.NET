// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/11/2013 by Ivan Lyagin

using Aspose.Collections;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a bookmark cache making access to bookmarks contained in a document by their names faster.
    /// After the cache instance is attached to a document, it tracks and reflects changes to its bookmark nodes.
    /// </summary>
    internal class BookmarkCache : INodeChangingCallback
    {
        internal BookmarkCache(Document document)
        {
            Debug.Assert(document != null);
            mDocument = document;
        }

        /// <summary>
        /// Attaches the cache to the specified document. A single instance of a cache can be attached to a single
        /// document at the same time.
        /// </summary>
        private void EnsureAttached()
        {
            if (mBookmarkNameToBundleMap != null)
                return;

            mBookmarkNameToBundleMap = new StringToObjDictionary<BookmarkBundle>(false);

            // Initialize the state of the cache.
            MapInitializer initializer = new MapInitializer(this);
            mDocument.Accept(initializer);

            // Subscribe to bookmark start inserting/removing the cache to be autorefreshable.
            mDocument.AddInternalNodeChangingCallback(this);
        }

        /// <summary>
        /// Detaches the cache from a document it was previously attached to.
        /// </summary>
        internal void Clear()
        {
            if (mBookmarkNameToBundleMap == null)
                return;

            mDocument.RemoveInternalNodeChangingCallback(this);
            mBookmarkNameToBundleMap = null;
        }

        void INodeChangingCallback.NodeInserting(NodeChangingArgs args)
        {
            // Do nothing.
        }

        void INodeChangingCallback.NodeInserted(NodeChangingArgs args)
        {
            if (args.NewParent.IsRemoved)
                return;

            ProcessNodeChange(args.Node, true);
        }

        void INodeChangingCallback.NodeRemoving(NodeChangingArgs args)
        {
            // Do nothing.
        }

        void INodeChangingCallback.NodeRemoved(NodeChangingArgs args)
        {
            if (args.OldParent.IsRemoved)
                return;

            ProcessNodeChange(args.Node, false);
        }

        /// <summary>
        /// Refreshes the cache when a bookmark start or end is inserted or removed.
        /// </summary>
        private void ProcessNodeChange(Node node, bool isInserted)
        {
            if (node.IsComposite)
            {
                // Process possible nested bookmark starts.
                CompositeNode parentNode = (CompositeNode)node;
                for (Node childNode = parentNode.FirstChild; childNode != null; childNode = childNode.NextSibling)
                    ProcessNodeChange(childNode, isInserted);
            }
            else if (node.NodeType == NodeType.BookmarkStart)
            {
                BookmarkStart bookmarkStart = (BookmarkStart)node;
                CacheBookmarkStart(bookmarkStart.Name, (isInserted ? bookmarkStart : null));
            }
            else if (node.NodeType == NodeType.BookmarkEnd)
            {
                BookmarkEnd bookmarkEnd = (BookmarkEnd)node;
                CacheBookmarkEnd(bookmarkEnd.Name, (isInserted ? bookmarkEnd : null));
            }
        }

        private void CacheBookmarkStart(string bookmarkName, BookmarkStart bookmarkStart)
        {
            GetBookmarkBundle(bookmarkName, true).SetStart(bookmarkStart);
        }

        private void CacheBookmarkEnd(string bookmarkName, BookmarkEnd bookmarkEnd)
        {
            GetBookmarkBundle(bookmarkName, true).SetEnd(bookmarkEnd);
        }

        /// <summary>
        /// Returns a <see cref="BookmarkBundle"/> instance corresponding to the specified bookmark name.
        /// Optionally creates it, if not found.
        /// </summary>
        private BookmarkBundle GetBookmarkBundle(string bookmarkName, bool createIfNull)
        {
            BookmarkBundle bundle = mBookmarkNameToBundleMap[bookmarkName];
            if ((bundle == null) && createIfNull)
            {
                bundle = new BookmarkBundle();
                mBookmarkNameToBundleMap[bookmarkName] = bundle;
            }

            return bundle;
        }

        /// <summary>
        /// Gets a bookmark for the corresponding bookmark start and end from the cache.
        /// </summary>
        internal Bookmark this[string bookmarkName]
        {
            get
            {
                EnsureAttached();

                BookmarkBundle bundle = GetBookmarkBundle(bookmarkName, false);
                return (bundle != null) ? bundle.GetBookmark() : null;
            }
        }

        /// <summary>
        /// Initializes the state of a particular bookmark cache.
        /// </summary>
        private class MapInitializer : DocumentVisitor
        {
            internal MapInitializer(BookmarkCache cache)
            {
                mCache = cache;
            }

            public override VisitorAction VisitBookmarkStart(BookmarkStart bookmarkStart)
            {
                mCache.CacheBookmarkStart(bookmarkStart.Name, bookmarkStart);
                return VisitorAction.Continue;
            }

            public override VisitorAction VisitBookmarkEnd(BookmarkEnd bookmarkEnd)
            {
                mCache.CacheBookmarkEnd(bookmarkEnd.Name, bookmarkEnd);
                return VisitorAction.Continue;
            }

            private readonly BookmarkCache mCache;
        }

        /// <summary>
        /// Stores information about a bookmark start and end pair being cached.
        /// </summary>
        private class BookmarkBundle
        {
            internal void SetStart(BookmarkStart start)
            {
                mStart = start;
            }

            internal void SetEnd(BookmarkEnd end)
            {
                mEnd = end;
            }

            internal Bookmark GetBookmark()
            {
                return ((mStart != null) && (mEnd != null)) ? new Bookmark(mStart, mEnd) : null;
            }

            private BookmarkStart mStart;

            private BookmarkEnd mEnd;
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Document mDocument;
        private StringToObjDictionary<BookmarkBundle> mBookmarkNameToBundleMap;
    }
}
