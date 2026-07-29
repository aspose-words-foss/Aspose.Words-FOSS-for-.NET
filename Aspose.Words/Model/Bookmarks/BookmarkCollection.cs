// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin
using System;
using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// A collection of <see cref="Bookmark"/> objects that represent the bookmarks in the specified range.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-bookmarks/">Working with Bookmarks</a> documentation article.</para>
    /// </summary>
    public class BookmarkCollection : IEnumerable<Bookmark>
    {
        internal BookmarkCollection(Node parent)
        {
            if (parent.IsComposite)
                mBookmarkStarts = ((CompositeNode)parent).GetChildNodes(NodeType.BookmarkStart, true);
            else
                mBookmarkStarts = EmptyNodeCollection.CreateEmpty();
        }

        internal BookmarkCollection(NodeRange nodeRange)
        {
            mBookmarkStarts = new NodeCollection(nodeRange.Document, new BookmarkMatcher(nodeRange), true);
        }

        /// <summary>
        /// Returns the number of bookmarks in the collection.
        /// </summary>
        public int Count
        {
            get { return mBookmarkStarts.Count; }
        }

        /// <summary>
        /// Returns a bookmark at the specified index.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        /// <param name="index">An index into the collection.</param>
        public Bookmark this[int index]
        {
            get { return ((BookmarkStart)mBookmarkStarts[index]).Bookmark; }
        }

        /// <summary>
        /// Returns a bookmark by name.
        /// </summary>
        /// <remarks>
        /// <p>Returns <c>null</c> if the bookmark with the specified name cannot be found.</p>
        /// </remarks>
        /// <param name="bookmarkName">Case-insensitive name of the bookmark.</param>
        public Bookmark this[string bookmarkName]
        {
            get
            {
                ArgumentUtil.CheckNotNull(bookmarkName, "bookmarkName");

                foreach (BookmarkStart bookmarkStart in mBookmarkStarts)
                    if (StringUtil.EqualsIgnoreCase(bookmarkName, bookmarkStart.Name))
                        return bookmarkStart.Bookmark;

                return null;
            }
        }

#if PYNET // Indexers for non int type is not supported in Python.
        /// <summary>
        /// Returns a bookmark by name.
        /// </summary>
        public Bookmark GetByName(string bookmarkName)
        {
            return this[bookmarkName];
        }
#endif

        /// <summary>
        /// Returns index of bookmark with the specified name.
        /// </summary>
        /// <param name="bookmarkName">Name of the bookmark to find the index for.</param>
        /// <returns>Bookmark index in this collection. Returns -1 if the bookmark with the specified name does not exist in the collection.</returns>
        internal int IndexOf(string bookmarkName)
        {
            int i = 0;
            foreach (BookmarkStart bookmarkStart in mBookmarkStarts)
            {
                if (StringUtil.EqualsIgnoreCase(bookmarkName, bookmarkStart.Name))
                    return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Removes the specified bookmark from the document.
        /// </summary>
        /// <param name="bookmark">The bookmark to remove.</param>
        public void Remove(Bookmark bookmark)
        {
            if (bookmark == null)
                throw new ArgumentNullException("bookmark");
            bookmark.Remove();
        }

        /// <summary>
        /// Removes a bookmark with the specified name.
        /// </summary>
        /// <param name="bookmarkName">The case-insensitive name of the bookmark to remove.</param>
        public void Remove(string bookmarkName)
        {
            ArgumentUtil.CheckNotNull(bookmarkName, "bookmarkName");
            Remove(this[bookmarkName]);
        }

        /// <summary>
        /// Removes a bookmark at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the bookmark to remove.</param>
        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        /// <summary>
        /// Removes all bookmarks from this collection and from the document.
        /// </summary>
        public void Clear()
        {
            // SPEED This is a proper way for removing.
            int remaining = Count;
            while (remaining > 0)
            {
                RemoveAt(0);
                remaining--;
            }
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<Bookmark> GetEnumerator()
        {
            return new BookmarkIterator(mBookmarkStarts);
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BookmarkIterator(mBookmarkStarts);
        }

        /// <summary>
        /// This is a live collection of bookmark start nodes.
        /// </summary>
        private readonly NodeCollection mBookmarkStarts;

        /// <summary>
        /// Enumerator through items of <see cref="BookmarkCollection"/>.
        /// </summary>
        private sealed class BookmarkIterator : IEnumerator<Bookmark>
        {
            /// <summary>
            /// Ctor.
            /// </summary>
            internal BookmarkIterator(NodeCollection bookmarkStarts)
            {
                mBookmarkStartEnumerator = bookmarkStarts.GetEnumerator();
            }

            public void Dispose()
            {
                // Nothing to dispose.
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            public Bookmark Current
            {
                get
                {
                    BookmarkStart bookmarkStart = (BookmarkStart)mBookmarkStartEnumerator.Current;
                    return bookmarkStart != null ? bookmarkStart.Bookmark : null;
                }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                return mBookmarkStartEnumerator.MoveNext();
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                ((NodeCollectionEnumerator<Node>)mBookmarkStartEnumerator).Reset();
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            private readonly IEnumerator<Node> mBookmarkStartEnumerator;
        }

        private class BookmarkMatcher : NodeMatcher
        {

            internal BookmarkMatcher(NodeRange range)
            {
                mNodeRange = range;
                UpdateBookmarkCache();
            }

            /// <summary>
            /// Returns true if the specified node matches the appropriate Bookmark node.
            /// </summary>
            /// <remarks>
            /// MS Word approach is implemented when the bookmark range, and not just BookmarkStart, is taken into account.
            /// Ex: The document has three sections. BookmarkStart is in the first section, BookmarkEnd is in the third one.
            ///     BookmarkMatcher will indicate that this Bookmark is in all the three sections, and not just in the first one.
            /// </remarks>
            internal override bool IsMatch(Node node)
            {
                if (mInitialChangeCount != Document.TreeChangeCount)
                    UpdateBookmarkCache();

                if ((node.NodeType != NodeType.BookmarkStart) || mRangeIsEmpty)
                    return false;

                BookmarkStart bmkStart = (BookmarkStart)node;
                BookmarkEnd bmkEnd = bmkStart.Bookmark.BookmarkEnd;

                return mBmkStarts.Contains(bmkStart) && mBmkEnds.Contains(bmkEnd);
            }

            internal override bool IsSkipMarkupNodes
            {
                get { return true; }
            }

            private void UpdateBookmarkCache()
            {
                mInitialChangeCount = Document.TreeChangeCount;

                mRangeIsEmpty = mNodeRange.IsEmpty || mNodeRange.IsVoid ||
                                mNodeRange.Start.Node.IsRemoved ||
                                mNodeRange.End.Node.IsRemoved ||
                                (NodeFinder.FindNodes(mNodeRange, NodeType.Any).Count == 0);

                if (mRangeIsEmpty)
                    return;

                NodeRange highRange = new NodeRange(Document.FirstSection.Body.FirstChild,
                    true, mNodeRange.End.Node, mNodeRange.IsEndIncluded);
                mBmkStarts = NodeFinder.FindNodes(highRange, NodeType.BookmarkStart);

                NodeRange tailRange = new NodeRange(mNodeRange.Start.Node, mNodeRange.IsStartIncluded,
                    Document.LastChild, true);
                mBmkEnds = NodeFinder.FindNodes(tailRange, NodeType.BookmarkEnd);
            }

            private Document Document
            {
                get { return mNodeRange.Document.FetchDocument(); }
            }

            private bool mRangeIsEmpty;
            private IList<Node> mBmkStarts;
            private IList<Node> mBmkEnds;
            private int mInitialChangeCount;
            private readonly NodeRange mNodeRange;
        }
    }
}
