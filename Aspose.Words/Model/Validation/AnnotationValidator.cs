// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/02/2010 by Roman Korchagin

using System.Collections.Generic;
using Aspose.Collections;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Used to validate comments, comment ranges and editable ranges before saving a document.
    /// 
    /// Marks comments that have no ranges. We need this because when writing to RTF it makes a difference.
    /// Removes comment ranges/editable ranges that have end before start.
    /// Removes ranges that have only start or end. 
    /// Removes comment ranges that have no comment.
    /// Removes comment ranges that are empty.
    /// Removes comment ranges with duplicate ids.
    /// Generates the whole new sequence of comment/editable range ids and new ids for comments/editable range with duplicate ids.
    /// 
    /// Comment ids in the model can have any values and we have to generate new ids for all comments 
    /// to reliably fix comments with duplicate ids.
    /// </summary>
    internal class AnnotationValidator
    {
        internal AnnotationValidator(DocumentBase doc, SaveInfo saveInfo, IWarningCallback warningCallback)
        {
            mDoc = doc;
            mSaveInfo = saveInfo;
            mDoc.ResetNextAnnotationId();
            mWarningCallback = warningCallback;
        }

        /// <summary>
        /// Call this at the document end. Performs validation tasks that need to be done at the end.
        /// </summary>
        internal void VisitDocumentEnd()
        {
            ValidateComments();
            UpdateParentCommentIds();
            ValidateEditableRanges();
        }

        internal VisitorAction VisitComment(Comment comment)
        {
            comment.EnsureMinimum();

            CommentValidatorItem item = GetOrCreateCommentValidatorItem(comment.Id);
            if (item.Comment != null)
            {
                // Got a comment with a duplicate id so generate a new id for it.
                // Such comment will lose its range (if it had it), but that's the best we can do.
                ((INodeWithAnnotationId)comment).IdInternal = mDoc.GetNextAnnotationId();
            }
            else
            {
                item.Comment = comment;
            }

            return VisitorAction.Continue;
        }

        internal VisitorAction VisitCommentRangeStart(CommentRangeStart commentRangeStart)
        {
            CommentValidatorItem item = GetOrCreateCommentValidatorItem(commentRangeStart.Id);
            if (item.HasStart)
            {
                // Get a duplicate start, remove.
                commentRangeStart.Remove();
                return VisitorAction.SkipThisNode;
            }

            item.CommentRangeStart = commentRangeStart;
            return VisitorAction.Continue;
        }

        internal VisitorAction VisitCommentRangeEnd(CommentRangeEnd commentRangeEnd)
        {
            CommentValidatorItem item = GetOrCreateCommentValidatorItem(commentRangeEnd.Id);
            if (item.HasEnd || !item.HasStart)
            {
                // Got a duplicate end or got and end without a start, remove.
                commentRangeEnd.Remove();
                return VisitorAction.SkipThisNode;
            }

            item.CommentRangeEnd = commentRangeEnd;
            return VisitorAction.Continue;
        }

        internal VisitorAction VisitEditableRangeStart(EditableRangeStart editableRangeStart)
        {
            EditableRangeValidatorItem item = GetOrCreateEditableRangeValidatorItem(editableRangeStart.Id);
            if (item.HasStart)
            {
                // Get a duplicate start, remove.
                editableRangeStart.Remove();
                return VisitorAction.SkipThisNode;
            }

            item.EditableRangeStart = editableRangeStart;
            return VisitorAction.Continue;
        }

        internal VisitorAction VisitEditableRangeEnd(EditableRangeEnd editableRangeEnd)
        {
            EditableRangeValidatorItem item = GetOrCreateEditableRangeValidatorItem(editableRangeEnd.Id);
            if (item.HasEnd || !item.HasStart)
            {
                // Got a duplicate end or got and end without a start, remove.
                editableRangeEnd.Remove();
                return VisitorAction.SkipThisNode;
            }

            item.EditableRangeEnd = editableRangeEnd;
            return VisitorAction.Continue;
        }

        private void ValidateEditableRanges()
        {
            foreach (EditableRangeValidatorItem item in mEditableRangeValidatorItems)
            {
                if (item.HasStart && !item.HasEnd)
                {
                    WarnUnexpected(WarningStrings.EditableRangeValidatorNoEnd);
                    item.RemoveEditableRange();
                }

                if (!item.HasStart && item.HasEnd)
                {
                    WarnUnexpected(WarningStrings.EditableRangeValidatorNoStart);
                    item.RemoveEditableRange();
                }

                // Now ready to apply the newly generated ids.
                item.SetNewId();
            }
        }

        private void ValidateComments()
        {
            foreach (CommentValidatorItem item in mCommentIdToValidatorItem.Values)
            {
                if (item.HasStart && !item.HasEnd)
                {
                    WarnUnexpected(WarningStrings.CommentValidatorNoEnd);
                    item.RemoveCommentRange();
                }

                if (!item.HasStart && item.HasEnd)
                {
                    WarnUnexpected(WarningStrings.CommentValidatorNoStart);
                    item.RemoveCommentRange();
                }

                if ((item.Comment == null) && item.HasStart && item.HasEnd)
                {
                    WarnUnexpected(WarningStrings.CommentValidatorNoContent);
                    item.RemoveCommentRange();
                }

                if (item.IsEmptyRange)
                {
                    WarnUnexpected(WarningStrings.CommentValidatorEmptyRange);
                    item.RemoveCommentRange();
                }
                else if ((item.Comment != null) && item.HasStart && item.Comment.IsAbove(item.CommentRangeStart))
                {
                    WarnUnexpected(WarningStrings.CommentAboveCommentRange);
                    item.RemoveCommentRange();
                }

                if (item.Comment != null)
                {
                    if((mSaveInfo != null) && (item.HasStart && item.HasEnd))
                        mSaveInfo.SetHasRange(item.Comment);
                }

                // Now ready to apply the newly generated ids.
                item.SetNewId();
            }
        }

        private CommentValidatorItem GetOrCreateCommentValidatorItem(int id)
        {
            CommentValidatorItem item = mCommentIdToValidatorItem[id];
            if (item == null)
            {
                item = new CommentValidatorItem();
                
                // Generate a new id now, this will result in "nice" ids that increase 
                // with the order the comments occur in the document.
                item.NewId = mDoc.GetNextAnnotationId();
                mCommentIdToValidatorItem[id] = item;
            }
            return item;
        }

        private EditableRangeValidatorItem GetOrCreateEditableRangeValidatorItem(int id)
        {
            EditableRangeValidatorItem resultItem = null;

            foreach (EditableRangeValidatorItem item in mEditableRangeValidatorItems)
            {
                if ((item.HasStart) && (item.EditableRangeStart.Id == id) && (!item.HasEnd))
                {
                    resultItem = item;
                    break;
                }
            }

            if (resultItem == null)
            {
                resultItem = new EditableRangeValidatorItem();

                // Generate a new id now, this will result in "nice" ids that increase 
                // with the order the editable range occur in the document.
                resultItem.NewId = mDoc.GetNextAnnotationId();
                mEditableRangeValidatorItems.Add(resultItem);
            }

            return resultItem;
        }

        /// <summary>
        /// Updates references to parent comments.
        /// </summary>
        private void UpdateParentCommentIds()
        {
            IntToObjDictionary<CommentValidatorItem>.Enumerator enumerator = mCommentIdToValidatorItem.GetEnumerator();

            while (enumerator.MoveNext())
            {
                CommentValidatorItem item = enumerator.CurrentValue;

                if (item.Comment == null)
                    continue;

                int newParentId = Comment.NoParent;

                if (item.Comment.ParentId != Comment.NoParent)
                {
                    int parentId = item.Comment.ParentId;
                    CommentValidatorItem parentItem = mCommentIdToValidatorItem[parentId];
                    // WORDSNET-14055 When the comment does not have parent item, the special (NoParent) value
                    // have to be set as parent identifier.
                    newParentId = (parentItem != null) ? parentItem.NewId : Comment.NoParent;
                }

                item.Comment.ParentId = newParentId;
            }
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private void WarnUnexpected(string description)
        {
            if (mWarningCallback != null)
                mWarningCallback.Warning(new WarningInfo(WarningType.UnexpectedContent, WarningSource.Unknown, description));
        }

        private readonly DocumentBase mDoc;
        
        /// <summary>
        /// Key is an integer comment id. Value is a <see cref="CommentValidatorItem"/>.
        /// </summary>
        private readonly IntToObjDictionary<CommentValidatorItem> mCommentIdToValidatorItem = 
            new IntToObjDictionary<CommentValidatorItem>();

        /// <summary>
        /// A list of EditableRangeValidatorItems. <see cref="EditableRangeValidatorItem"/>.
        /// </summary>
        private readonly List<EditableRangeValidatorItem> mEditableRangeValidatorItems = 
            new List<EditableRangeValidatorItem>();

        private readonly IWarningCallback mWarningCallback;

        private readonly SaveInfo mSaveInfo;
    }
}
