// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/05/2013 by Ivan Lyagin

namespace Aspose.Words
{
    /// <summary>
    /// Applies an <see cref="INodeModifier"/> instance to a node range in the same manner as <see cref="NodeCopier"/>.
    /// The difference is that source nodes are not copied before being modified.
    /// </summary>
    /// <remarks>
    /// If the corresponding node range starts/ends inside a run, then this run is cloned before being modified.
    /// </remarks>
    internal class NodeModifierApplier : NodeTraverser
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal NodeModifierApplier(NodeRange range, INodeModifier modifier)
            : base(range)
        {
            mModifier = modifier;
        }

        /// <summary>
        /// Applies the modifier to the node range.
        /// </summary>
        internal void ApplyModifier()
        {
            Traverse();
        }

        protected override void OnNonCompositeNode()
        {
            ProcessCurrentNode();
        }

        protected override void OnMiddleNodeAncestor()
        {
            ProcessCurrentNode();
        }

        private void ProcessCurrentNode()
        {
            // Apply the modifier to the range through NodeEnumerator.ExtractCurrentNode()
            // as it correctly cuts runs and clones them if needed.
            ExtractRangeNode(CurrentNode, mModifier, null, false, true);
        }

        private readonly INodeModifier mModifier;
    }
}
