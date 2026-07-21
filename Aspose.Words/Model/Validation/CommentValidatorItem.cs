// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/02/2010 by Roman Korchagin

namespace Aspose.Words.Validation
{
    /// <summary>
    /// A data holder. Keeps info about comment and comment range start/end during document validation.
    /// </summary>
    internal class CommentValidatorItem
    {
        /// <summary>
        /// Used to renumber comment and comment range nodes during validation.
        /// </summary>
        internal void SetNewId()
        {
            if (Comment != null)
                ((INodeWithAnnotationId)Comment).IdInternal = NewId;
            if (CommentRangeStart != null)
                CommentRangeStart.Id = NewId;
            if (CommentRangeEnd != null)
                CommentRangeEnd.Id = NewId;
        }

        internal void RemoveCommentRange()
        {
            // TODO 2 See Aspose.Words.TestData\Model\Para\TestTextFrameDetection.docx
            // The code crashes because CommentRangeStart.Parent == null.
            // I don't know how it comes to be null. This is probably an issue with text frame to text box conversion.

            if (CommentRangeStart != null)
            {
                if (CommentRangeStart.ParentNode != null)
                    CommentRangeStart.Remove();
                CommentRangeStart = null;
            }
            
            if (CommentRangeEnd != null)
            {
                if (CommentRangeEnd.ParentNode != null)
                    CommentRangeEnd.Remove();
                CommentRangeEnd = null;
            }
        }

        internal bool HasStart
        {
            get { return (CommentRangeStart != null); }
        }

        internal bool HasEnd
        {
            get { return (CommentRangeEnd != null); }
        }

        internal bool IsSameParent
        {
            get { return HasStart && HasEnd && (CommentRangeStart.ParentNode == CommentRangeEnd.ParentNode); }
        }

        internal bool IsEmptyRange
        {
            get
            {
                // WORDSNET-4661 Code assumes that comment range start/end are children of the same parent but it's not true for
                // defect file. Fix avoids to remove such range nodes.
                if(!IsSameParent)
                    return false;

                Node node = CommentRangeStart;
                while ((node != null) && (node != CommentRangeEnd))
                {
                    if (node.GetTextLength() > 0)
                        return false;

                    node = node.NextSibling;
                }
                return true;                
            }
        }

        internal Comment Comment;
        internal CommentRangeStart CommentRangeStart;
        internal CommentRangeEnd CommentRangeEnd;
        internal int NewId;
    }
}
