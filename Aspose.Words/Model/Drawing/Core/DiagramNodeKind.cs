// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2006 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Specifies kind of node in a diagram. 
    /// Taken from RTF specification. No guarantee this is correct.
    /// </summary>
    internal enum DiagramNodeKind
    {
        /// <summary>
        /// Used for normal subordinates? Why not Subordinate?
        /// </summary>
        Node = 0,
        /// <summary>
        /// Seen, looks correct.
        /// </summary>
        Root = 1, 
        /// <summary>
        /// Seen, looks correct.
        /// </summary>
        Assistant = 2,
        CoWorker = 3,
        Subordinate = 4,
        Auxiliary = 5,
        /// <summary>
        /// Strange, not seen.
        /// </summary>
        Default = 6,
        /// <summary>
        /// Seen in TestDiagram.doc, not sure what it is for.
        /// </summary>
        Undocumented65535 = 65535
    }
}
