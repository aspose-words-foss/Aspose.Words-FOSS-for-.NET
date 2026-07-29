// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/05/2014 by Alexey Morozov

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// Interface for nodes which support revision tracking.
    /// </summary>
    internal interface ITrackableNode : IMoveTrackableNode
    {
        EditRevision InsertRevision { get; set; }

        EditRevision DeleteRevision { get; set; }
    }
}
