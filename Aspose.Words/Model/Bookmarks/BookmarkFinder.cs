// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/05/2004 by Roman Korchagin
using System;
using System.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// Finds a bookmark by name.
    /// </summary>
    internal class BookmarkFinder : DocumentVisitor
    {
        private BookmarkFinder(string bookmarkName, bool isLookingForStart)
        {
            mBookmarkName = bookmarkName;
            mIsLookingForStart = isLookingForStart;
            mRefBookmarkStart = null;
        }

        /// <summary>
        /// Helper function to find a bookmark start in the document. Returns the bookmark start or null.
        /// </summary>
        internal static BookmarkStart FindBookmarkStart(Node node, string bookmarkName)
        {
            return FindBookmarkStart(node, bookmarkName, null);
        }

        /// <summary>
        /// Helper function to find a bookmark start that corresponds to the specified bookmark end in the document.
        /// Returns the bookmark start or <b>null</b>.
        /// </summary>
        internal static BookmarkStart FindBookmarkStart(Node node, string bookmarkName, BookmarkEnd refBookmarkEnd)
        {
            BookmarkFinder finder = new BookmarkFinder(bookmarkName, true);
            finder.mRefBookmarkEnd = refBookmarkEnd;
            if (refBookmarkEnd != null)
                finder.mNotClosedStarts = new Stack<BookmarkStart>();

            node.Accept(finder);
            return finder.mBookmarkStart;
        }

        internal static BookmarkEnd FindBookmarkEnd(Node node, string bookmarkName)
        {
            // AW returns first occurrence for duplicated bookmarks for some operations, like moving cursor of the document
            // builder to the end of bookmark with specified name. However, client can be confused when try to move to
            // the end of duplicated bookmark.
            return FindBookmarkEnd(node, bookmarkName, null);
        }

        /// <summary>
        /// Helper function to find a bookmark end in the document. Returns the bookmark end or null.
        /// </summary>
        internal static BookmarkEnd FindBookmarkEnd(Node node, string bookmarkName, BookmarkStart refBookmarkStart)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            BookmarkFinder finder = new BookmarkFinder(bookmarkName, false);
            finder.mRefBookmarkStart = refBookmarkStart;

            node.Accept(finder);
            return finder.mBookmarkEnd;
        }

        internal static BookmarkEnd FetchBookmarkEnd(Node node, string bookmarkName, BookmarkStart refBookmarkStart)
        {
            BookmarkEnd result = FindBookmarkEnd(node, bookmarkName, refBookmarkStart);

            if (result == null)
                throw new InvalidOperationException(string.Format("Cannot find bookmark '{0}' in the document.", bookmarkName));
            return result;
        }

        public override VisitorAction VisitBookmarkStart(BookmarkStart node)
        {
            if (mIsLookingForStart && StringUtil.EqualsIgnoreCase(mBookmarkName, node.Name))
            {
                // Store bookmark start until referenced "end" node is not found.
                if (mRefBookmarkEnd != null)
                {
                    mNotClosedStarts.Push(node);
                    return VisitorAction.Continue;
                }

                mBookmarkStart = node;
                return VisitorAction.Stop;
            }
            else if (!mIsLookingForStart) // Skip unneeded operations.
            {
                // When referenced "start" node is not found, name of bookmark start node and current node are equal,
                // then increment number of opened bookmarks. This counter will be used to find appropriate "end" node.
                if (StringUtil.EqualsIgnoreCase(mBookmarkName, node.Name) && (mRefBookmarkStart != null))
                    ++mNotClosedStartCount;

                // Check that current node is the referenced "start" node for which "end" node is looking for.
                // And reset referenced "start" node to show that appropriate "start" node is found.
                if (ReferenceEquals(mRefBookmarkStart, node))
                    mRefBookmarkStart = null;
            }

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitBookmarkEnd(BookmarkEnd node)
        {
            if (!mIsLookingForStart && StringUtil.EqualsIgnoreCase(mBookmarkName, node.Name))
            {
                // Return last bookmark at any case, for resiliency.
                mBookmarkEnd = node;
                // Decrement number of "start" nodes with the same name.
                --mNotClosedStartCount;

                // Skip bookmark end when referenced "start" node is not found yet.
                if (mRefBookmarkStart != null)
                    return VisitorAction.Continue;

                // WORDSNET-15823 Return appropriate “BookmarkEnd” followed after referenced “BookmarkStart”.
                // When number of "start" nodes equal to zero, then it is mean that appropriate "end" node is found.
                return (mNotClosedStartCount > 0) ? VisitorAction.Continue : VisitorAction.Stop;
            }
            else if (mIsLookingForStart &&
                     (mRefBookmarkEnd != null) &&
                     StringUtil.EqualsIgnoreCase(mBookmarkName, node.Name))
            {
                if (node == mRefBookmarkEnd)
                {
                    // The desired bookmark start is the last non-closed bookmark start with this name.
                    mBookmarkStart = (mNotClosedStarts.Count > 0) ? mNotClosedStarts.Peek() : null;
                    return VisitorAction.Stop;
                }

                // Close the last bookmark start with this name.
                if (mNotClosedStarts.Count > 0)
                    mNotClosedStarts.Pop();
            }

            return VisitorAction.Continue;
        }

        private readonly string mBookmarkName;
        private readonly bool mIsLookingForStart;
        private BookmarkStart mBookmarkStart;
        private BookmarkEnd mBookmarkEnd;

        private BookmarkStart mRefBookmarkStart;
        private int mNotClosedStartCount;
        private BookmarkEnd mRefBookmarkEnd;
        private Stack<BookmarkStart> mNotClosedStarts;
    }
}
