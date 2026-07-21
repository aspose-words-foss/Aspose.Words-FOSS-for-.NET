// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/06/2013 by Andrey Noskov

using System;

namespace Aspose.Words
{
    /// <summary>
    /// Finds an editable range by Id.
    /// </summary>
    internal class EditableRangeFinder : DocumentVisitor
    {
        private EditableRangeFinder(int editableRangeId, bool isLookingForStart)
        {
            mEditableRangeId = editableRangeId;
            mIsLookingForStart = isLookingForStart;
        }

        /// <summary>
        /// Helper function to find an editable range start in the document. Returns the editable range start or null.
        /// </summary>
        internal static EditableRangeStart FindEditableRangeStart(Node node, int editableRangeId)
        {
            EditableRangeFinder finder = new EditableRangeFinder(editableRangeId, true);
            node.Accept(finder);
            
            return finder.mEditableRangeStart;
        }

        /// <summary>
        /// Helper function to find an editable range end in the document. Returns the editable range end or null.
        /// </summary>
        internal static EditableRangeEnd FindEditableRangeEnd(Node node, int editableRangeId)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            EditableRangeFinder finder = new EditableRangeFinder(editableRangeId, false);
            node.Accept(finder);
            
            return finder.mEditableRangeEnd;
        }

        internal static EditableRangeStart FetchEditableRangeStart(Node node, int editableRangeId)
        {
            EditableRangeStart result = FindEditableRangeStart(node, editableRangeId);
            if (result == null)
                throw new InvalidOperationException(string.Format("Cannot find an editable range with Id='{0}' in the document.", editableRangeId));
            
            return result;
        }

        internal static EditableRangeEnd FetchEditableRangeEnd(Node node, int editableRangeId)
        {
            EditableRangeEnd result = FindEditableRangeEnd(node, editableRangeId);
            if (result == null)
                throw new InvalidOperationException(string.Format("Cannot find an editable range with Id='{0}' in the document.", editableRangeId));
            
            return result;
        }

        public override VisitorAction VisitEditableRangeStart(EditableRangeStart editableRangeStart)
        {
            if (mIsLookingForStart && (mEditableRangeId == editableRangeStart.Id))
            {
                mEditableRangeStart = editableRangeStart;
                return VisitorAction.Stop;
            }

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitEditableRangeEnd(EditableRangeEnd editableRangeEnd)
        {
            if (!mIsLookingForStart && (mEditableRangeId == editableRangeEnd.Id))
            {
                mEditableRangeEnd = editableRangeEnd;
                return VisitorAction.Stop;
            }

            return VisitorAction.Continue;
        }

        private readonly int mEditableRangeId;
        private readonly bool mIsLookingForStart;
        private EditableRangeStart mEditableRangeStart;
        private EditableRangeEnd mEditableRangeEnd;
    }
}
