// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/03/2012 by Dmitry Burov

using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Matches the node by type.
    /// It's a default matcher for <see cref="NodeCollection"/>
    /// </summary>
    internal class NodeTypeMatcher : NodeMatcher
    {
        protected NodeTypeMatcher(NodeType type)
        {
            mType = type;
            SetupIsSkipMarkupNodes();
        }

        /// <summary>
        /// Gets matcher for the specified node type.
        /// </summary>
        /// <remarks>
        /// Performance optimization: use cached instances for the most used node types to reduce
        /// memory allocations and improve performance.
        /// </remarks>
        /// <param name="type">Node type.</param>
        /// <returns></returns>
        internal static NodeTypeMatcher GetMatcher(NodeType type)
        {
            switch (type)
            {
                case NodeType.Any:
                    return gAnyMatcher;
                case NodeType.SpecialChar:
                    return gSpecialCharMatcher;
                case NodeType.Run:
                    return gRunMatcher;
                default:
                    return new NodeTypeMatcher(type);
            }
        }

        private void SetupIsSkipMarkupNodes()
        {
            if ((mTypes != null) && (mTypes.Length > 0))
            {
                for (int i = 0; i < mTypes.Length; i++)
                {
                    // Check if at least one non-skipped type exists.
                    if (!NodeUtil.IsSkipMarkupNodesInFlatEnumeration(mTypes[i]))
                    {
                        mIsSkipMarkupNodes = false;
                        return;
                    }
                }

                mIsSkipMarkupNodes = true;
                return;
            }

            mIsSkipMarkupNodes = NodeUtil.IsSkipMarkupNodesInFlatEnumeration(mType);
        }

        public NodeTypeMatcher(NodeType[] types)
        {
            mTypes = types;
            SetupIsSkipMarkupNodes();
        }

        [JavaThrows(true)]
        internal override bool IsMatch(Node node)
        {
            if (mTypes != null)
            {
                foreach (NodeType nodeType in mTypes)
                {
                    if (node.NodeType == nodeType)
                        return true;
                }
                return false;
            }
            return (mType == NodeType.Any) || (node.NodeType == mType);
        }

        internal override bool IsSkipMarkupNodes
        {
            get { return mIsSkipMarkupNodes;  }
        }

        // Keeping two separate fields to avoid unnecessary array allocations 
        // for optimal performance.
        private readonly NodeType mType = NodeType.Any;
        private readonly NodeType[] mTypes;
        private bool mIsSkipMarkupNodes;

        private static NodeTypeMatcher gAnyMatcher = new NodeTypeMatcher(NodeType.Any);
        private static NodeTypeMatcher gSpecialCharMatcher = new NodeTypeMatcher(NodeType.SpecialChar);
        private static NodeTypeMatcher gRunMatcher = new NodeTypeMatcher(NodeType.Run);
    }
}
