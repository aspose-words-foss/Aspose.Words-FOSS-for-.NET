// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/01/2013 by Ivan Lyagin

using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a listener to <see cref="NodeCopier"/> events.
    /// </summary>
    internal interface INodeCopierListener : INodeCloningListener
    {
        /// <summary>
        /// This method is invoked when the whole node range has been copied.
        /// </summary>
        [JavaThrows(true)]
        void NotifyNodeRangeCopied(NodeRange sourceRange, NodeRange insertedRange);
    }
}
