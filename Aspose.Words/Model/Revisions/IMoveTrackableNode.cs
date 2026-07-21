// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/04/2019 by Dmitry Sokolov

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// Interface for nodes which support tracking of "move" revision.
    /// </summary>
    /// <remarks>
    /// <see cref="ITrackableNode"/> was spitted into two interfaces, because there are nodes which support only "move" revisions,
    /// for example, node with type "Comment".
    /// </remarks>
    internal interface IMoveTrackableNode
    {
        MoveRevision MoveFromRevision { get; set; }

        MoveRevision MoveToRevision { get; set; }

        void RemoveMoveRevisions();
    }
}
