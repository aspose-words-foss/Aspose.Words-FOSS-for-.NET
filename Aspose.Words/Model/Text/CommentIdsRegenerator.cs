// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/02/2021 by Edward Voronov

using System.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// Regenerates identifiers of cloned comment nodes (<see cref="Comment"/>,
    /// <see cref="CommentRangeStart"/> and <see cref="CommentRangeEnd"/>).
    /// </summary>
    internal class CommentIdsRegenerator
    {
        internal void RegenerateCommentIds(Node node)
        {
            if ((node.NodeType != NodeType.Comment) &&
                (node.NodeType != NodeType.CommentRangeStart) &&
                (node.NodeType != NodeType.CommentRangeEnd))
            {
                return;
            }

            INodeWithAnnotationId nodeWithAnnotationId = (INodeWithAnnotationId)node;

            if (mClonedCommentIdMap == null)
                mClonedCommentIdMap = new Dictionary<int, int>();

            int newId;
            if (!mClonedCommentIdMap.TryGetValue(nodeWithAnnotationId.IdInternal, out newId))
            {
                newId = node.Document.GetNextAnnotationId();
                mClonedCommentIdMap.Add(nodeWithAnnotationId.IdInternal, newId);
            }

            nodeWithAnnotationId.IdInternal = newId;

            int parentNewId;
            if (mClonedCommentIdMap.TryGetValue(nodeWithAnnotationId.ParentIdInternal, out parentNewId))
                nodeWithAnnotationId.ParentIdInternal = parentNewId;
        }

        private Dictionary<int, int> mClonedCommentIdMap;
    }
}
