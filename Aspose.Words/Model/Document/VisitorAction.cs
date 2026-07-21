// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/01/2006 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Allows the visitor to control the enumeration of nodes.
    /// </summary>
    public enum VisitorAction
    {
        /// <summary>
        /// The visitor requests the enumeration to continue.
        /// </summary>
        Continue,
        /// <summary>
        /// The visitor requests to skip the current node and continue enumeration.
        /// </summary>
        SkipThisNode,
        /// <summary>
        /// The visitor requests the enumeration of nodes to stop.
        /// </summary>
        Stop
    }
}
