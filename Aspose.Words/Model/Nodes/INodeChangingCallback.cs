// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2005 by Roman Korchagin

using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Implement this interface if you want to receive notifications when nodes are inserted or removed in the document.
    /// </summary>
    public interface INodeChangingCallback
    {
        /// <summary>
        /// Called just before a node belonging to this document is about to be inserted into another node.
        /// </summary>
        [JavaThrows(true)]
        void NodeInserting(NodeChangingArgs args);

        /// <summary>
        /// Called when a node belonging to this document has been inserted into another node. 
        /// </summary>
        [JavaThrows(true)]
        void NodeInserted(NodeChangingArgs args);

        /// <summary>
        /// Called just before a node belonging to this document is about to be removed from the document.
        /// </summary>
        [JavaThrows(true)]
        void NodeRemoving(NodeChangingArgs args);

        /// <summary>
        /// Called when a node belonging to this document has been removed from its parent.
        /// </summary>
        [JavaThrows(true)]
        void NodeRemoved(NodeChangingArgs args);
    }
}
