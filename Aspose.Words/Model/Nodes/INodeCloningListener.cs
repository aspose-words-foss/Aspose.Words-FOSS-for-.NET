// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2012 by Ivan Lyagin

namespace Aspose.Words
{
    /// <summary>
    /// Represents listener to node cloning event.
    /// </summary>
    internal interface INodeCloningListener
    {
        /// <summary>
        /// This method is invoked when the listened node is cloned.
        /// </summary>
        /// <param name="source">The listened node.</param>
        /// <param name="clone">The clone of the listened node.</param>
        void NotifyNodeCloned(Node source, Node clone);
    }
}
