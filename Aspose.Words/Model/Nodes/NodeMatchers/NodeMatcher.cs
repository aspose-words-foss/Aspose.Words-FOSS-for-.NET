// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/03/2012 by Dmitry Burov

using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Used to find nodes matching some pattern.
    /// Override to implement various matching patterns according to your needs.
    /// </summary>
    /// <remarks>
    /// DD: Be careful of node matcher/live collections combination. Live collection only tracks tree changes e.g. added/removed nodes.
    /// But if your node matcher uses more sophisticated criteria then collection will not update automatically.
    /// </remarks>
    internal abstract class NodeMatcher
    {
        /// <summary>
        /// Returns true if the specified node is matching the internal pattern.
        /// </summary>
        [JavaThrows(true)]
        internal abstract bool IsMatch(Node node);

        /// <summary>
        /// Returns true if markup nodes should be skipped.
        /// </summary>
        internal abstract bool IsSkipMarkupNodes { get; }
    }
}
