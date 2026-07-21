// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/07/2012 by Denis Darkin

using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Selects only nodes with revisions.
    /// </summary>
    internal class RevisionNodeMatcher : NodeMatcher
    {
        internal override bool IsMatch(Node node)
        {
            Debug.Assert(node != null);

            if ((ParentNode != null) && !((ParentNode == node) || node.IsAncestorNode(ParentNode)))
                return false;

            return RevisionUtil.HasRevision(node);
        }

        internal override bool IsSkipMarkupNodes
        {
            get { return false; }
        }

        internal Node ParentNode { get; set; }
    }
}
