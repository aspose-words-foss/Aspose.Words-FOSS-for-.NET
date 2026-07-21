// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/01/2007 by Roman Korchagin

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Words.Drawing;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Used to validate bookmarks (remove nodes of bookmarks with duplicate names and
    /// remove bookmarks that have no matching start or end node).
    /// </summary>
    internal class BookmarkValidator
    {
        /// <summary>
        /// Ctr.
        /// </summary>
        /// <param name="warningCallback">Object which capture warnings that can occur during document loading or saving.</param>
        /// <param name="truncateNames"></param>
        internal BookmarkValidator(IWarningCallback warningCallback, bool truncateNames)
        {
            mTruncateNames = truncateNames;
            mWarningCallback = warningCallback;
        }

        internal void VisitBookmarkStart(BookmarkStart bookmarkStart, StoryType storyType)
        {
            if (PostponeDmlFallbackBookmark(bookmarkStart))
                return;

            OnBookmarkStart(bookmarkStart, storyType);
        }

        internal void VisitBookmarkEnd(BookmarkEnd bookmarkEnd, StoryType storyType)
        {
            if (PostponeDmlFallbackBookmark(bookmarkEnd))
                return;

            OnBookmarkEnd(bookmarkEnd, storyType);
        }

        internal void VisitDrawingMLStart(ShapeBase drawingML)
        {
            if (drawingML.FallbackShape == null)
                return;

            mDmlStack.Push(drawingML);
        }

        internal void VisitDrawingMLEnd(ShapeBase drawingML)
        {
            if (drawingML != GetCurrentDml())
                return;

            mDmlStack.Pop();
        }

        /// <summary>
        /// Call this at the end. Removes any bookmark orphans that have starts only.
        /// </summary>
        internal void VisitDocumentEnd()
        {
            OnDocumentEnd();

            foreach (DmlBookmarks dmlBookmarks in mDmlBookmarks.Values)
            {
                BookmarkValidator dmlFallbackValidator = new BookmarkValidator(mWarningCallback, mTruncateNames);

                ISetGeneric<string> preservedNames = dmlBookmarks.GetPreservedMainBookmarkNames();
                foreach (Node bookmarkNode in dmlBookmarks.FallbackBookmarks)
                {
                    if (!preservedNames.Contains(((IBookmarkNode)bookmarkNode).Name))
                    {
                        bookmarkNode.Remove();
                        continue;
                    }

                    if (bookmarkNode.NodeType == NodeType.BookmarkStart)
                        dmlFallbackValidator.OnBookmarkStart((BookmarkStart)bookmarkNode, StoryType.MainText);
                    else
                        dmlFallbackValidator.OnBookmarkEnd((BookmarkEnd)bookmarkNode, StoryType.MainText);
                }

                dmlFallbackValidator.OnDocumentEnd();
            }
        }

        private bool PostponeDmlFallbackBookmark(Node bookmarkNode)
        {
            ShapeBase dml = GetCurrentDml();
            if (dml == null)
                return false;

            DmlBookmarks dmlBookmarks;
            if (!mDmlBookmarks.TryGetValue(dml, out dmlBookmarks))
            {
                dmlBookmarks = new DmlBookmarks();
                mDmlBookmarks.Add(dml, dmlBookmarks);
            }

            bool isFallbackBookmark = (dml.FallbackShape != null) && bookmarkNode.IsAncestorNode(dml.FallbackShape);
            dmlBookmarks.AddBookmark(bookmarkNode, isFallbackBookmark);

            return isFallbackBookmark;
        }

        private ShapeBase GetCurrentDml()
        {
            return mDmlStack.Top();
        }

        /// <summary>
        /// Check is it necessary to trim bookmark name.
        /// </summary>
        /// <remarks>
        /// WORDSNET-12492 Allow creation of bookmarks over 40 chars long when the target document has Pdf format.
        /// For resolve this issue does not trim bookmark name while saving document to Pdf format.
        /// Also was removed bookmark name trimming in <see cref="Aspose.Words.Fields.FieldRef"/>
        /// and <see cref="Aspose.Words.Fields.FieldUnknown"/> classes for update field operations.
        /// </remarks>
        /// <param name="bookmarkName">Name of the bookmark.</param>
        /// <returns>Return "True" when bookmark name need to be trimmed.</returns>
        private bool IsNeedTrimBookmarkName(string bookmarkName)
        {
            return (mTruncateNames && (bookmarkName.Length > Bookmark.MaxNameLength));
        }

        private void OnBookmarkStart(BookmarkStart bookmarkStart, StoryType storyType)
        {
            if (IsNeedTrimBookmarkName(bookmarkStart.Name))
            {
                WarningUtil.WarnUnexpected(mWarningCallback, WarningStrings.BookmarkValidatorTooLongName);
                bookmarkStart.SetNameInternal(bookmarkStart.Name.Substring(0, Bookmark.MaxNameLength));
            }

            string bookmarkItemKey = BookmarkValidatorItem.GetKey(bookmarkStart);
            BookmarkValidatorItem validatorItem = mValidatorItemsByName[bookmarkItemKey];
            if (validatorItem == null)
            {
                // A bookmark with this name is encountered for the first time, remember it.
                mValidatorItemsByName.Add(bookmarkItemKey, new BookmarkValidatorItem(bookmarkStart, storyType));
            }
            else
            {
                if (validatorItem.HasStartOnly)
                {
                    // A bookmark with this name already exists.
                    // Delete the extra bookmark node that we just found.
                    bookmarkStart.Remove();
                }
                else
                {
                    // A bookmark with this name already exists.
                    // Delete the bookmark nodes that we found before.
                    validatorItem.BookmarkStart.Remove();
                    validatorItem.BookmarkEnd.Remove();

                    mValidatorItemsByName[bookmarkItemKey] = new BookmarkValidatorItem(bookmarkStart, storyType);
                }

                Warn(WarningType.DataLoss, string.Format(WarningStrings.BookmarkValidatorDuplicateName, bookmarkStart.Name));
            }
        }

        private void OnBookmarkEnd(BookmarkEnd bookmarkEnd, StoryType storyType)
        {
            if (IsNeedTrimBookmarkName(bookmarkEnd.Name))
            {
                Warn(WarningType.DataLoss, WarningStrings.BookmarkValidatorTooLongName);
                bookmarkEnd.SetNameInternal(bookmarkEnd.Name.Substring(0, Bookmark.MaxNameLength));
            }

            string bookmarkItemKey = BookmarkValidatorItem.GetKey(bookmarkEnd);
            BookmarkValidatorItem validatorItem = mValidatorItemsByName[bookmarkItemKey];

            if (IsBookmarkEndValid(validatorItem, bookmarkEnd, storyType))
            {
                // We seen bookmark start only and have not seen bookmark end yet, its great.
                validatorItem.BookmarkEnd = bookmarkEnd;
            }
            else
            {
                // Either got end when have not seen start yet, get end second time or end is in different story.
                // Delete the extra bookmark node that we just found.
                bookmarkEnd.Remove();

                Warn(WarningType.DataLoss, string.Format(WarningStrings.BookmarkValidatorNoStart, bookmarkEnd.Name));
            }
        }

        private void OnDocumentEnd()
        {
            foreach (BookmarkValidatorItem validatorItem in mValidatorItemsByName.Values)
            {
                if (validatorItem.HasStartOnly)
                {
                    Warn(WarningType.DataLoss, string.Format(WarningStrings.BookmarkValidatorNoEnd, validatorItem.BookmarkStart.Name));
                    validatorItem.BookmarkStart.Remove();
                }
            }
        }

        /// <summary>
        /// Checks that BookmarkEnd node is correctly placed and has corresponding BookmarkStart node.
        /// </summary>
        private static bool IsBookmarkEndValid(BookmarkValidatorItem validatorItem, BookmarkEnd bookmarkEnd, StoryType bookmarkEndStoryType)
        {
            if (validatorItem == null)
                return false;

            if (!validatorItem.HasStartOnly)
                return false;

            if (validatorItem.BookmarkStoryType != bookmarkEndStoryType)
                return false;

            // WORDSNET-19516 Special case for Comment type. Both start and end bookmark nodes must be inside one Comment node.
            if (bookmarkEndStoryType == StoryType.Comments)
            {
                Comment endComment = (Comment)bookmarkEnd.GetAncestor(NodeType.Comment);
                Comment startComment = (Comment)validatorItem.BookmarkStart.GetAncestor(NodeType.Comment);

                if (!ReferenceEquals(startComment, endComment))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns bookmark names.
        /// </summary>
        internal ISetGeneric<string> GetBookmarkNames()
        {
            return new HashSetGeneric<string>(mValidatorItemsByName.Keys);
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private void Warn(WarningType warningType, string description)
        {
            if (mWarningCallback != null)
                mWarningCallback.Warning(new WarningInfo(warningType, WarningSource.Validator, description));
        }

        /// <summary>
        /// Key is case insensitive bookmark name. Value is a <see cref="BookmarkValidatorItem"/>.
        /// </summary>
        private readonly StringToObjDictionary<BookmarkValidatorItem> mValidatorItemsByName =
            new StringToObjDictionary<BookmarkValidatorItem>(false);

        /// <summary>
        /// Collects bookmarks nodes which are contained in DML nodes preserving order of appearance.
        /// </summary>
        private readonly IDictionary<ShapeBase, DmlBookmarks> mDmlBookmarks = new Dictionary<ShapeBase, DmlBookmarks>();

        private readonly Stack<ShapeBase> mDmlStack = new Stack<ShapeBase>();

        private readonly IWarningCallback mWarningCallback;

        /// <summary>
        /// Indicates that bookmark names should be truncated.
        /// </summary>
        private readonly bool mTruncateNames;

        private class DmlBookmarks
        {
            internal DmlBookmarks()
            {
                FallbackBookmarks = new List<Node>();
            }

            internal ICollection<Node> FallbackBookmarks { get; }

            internal void AddBookmark(Node node, bool isFallbackBookmark)
            {
                if (isFallbackBookmark)
                    FallbackBookmarks.Add(node);
                else
                    mMainBookmarks.Add(node);
            }

            internal ISetGeneric<string> GetPreservedMainBookmarkNames()
            {
                CaseInsensitiveHashSet result = new CaseInsensitiveHashSet(mMainBookmarks.Count / 2);
                foreach (Node bookmark in mMainBookmarks)
                {
                    string bookmarkName = ((IBookmarkNode)bookmark).Name;
                    if (result.Contains(bookmarkName))
                        continue;

                    if (bookmark.IsRemoved)
                        continue;

                    result.Add(bookmarkName);
                }

                return result;
            }

            private readonly ICollection<Node> mMainBookmarks = new List<Node>();
        }
    }
}
