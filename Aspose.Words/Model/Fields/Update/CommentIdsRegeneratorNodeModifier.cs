// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2023 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Modifies identifiers of cloned comment nodes.
    /// </summary>
    /// <seealso cref="CommentIdsRegenerator"/>
    internal class CommentIdsRegeneratorNodeModifier : INodeModifier
    {
        Node INodeModifier.Modify(
            Node referenceNode,
            Node nodeToModify,
            bool modifyChildren,
            INodeCloningListener cloningListener)
        {
            mCommentIdsRegenerator.RegenerateCommentIds(nodeToModify);
            return nodeToModify;
        }

        private readonly CommentIdsRegenerator mCommentIdsRegenerator = new CommentIdsRegenerator();
    }
}
