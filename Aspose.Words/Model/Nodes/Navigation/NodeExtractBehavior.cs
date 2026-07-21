// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2012 by Ivan Lyagin

namespace Aspose.Words
{
    /// <summary>
    /// Specifies an internal behavior for a <see cref="NodeEnumerator.ExtractCurrentNode"/> method call
    /// relative to child nodes' involving and node cloning conditions.
    /// </summary>
    /// <remarks>
    /// There is the alternative approach which is to use an INodeExtractBehavior interface with two
    /// methods: NeedModifyChildren() and NeedClone() in conjunction with the strategy pattern.
    /// 
    /// However, it is useful to see all of the logic altogether concentrated in a single place,
    /// which is the NodeEnumerator class at the moment.
    /// 
    /// It is useful simply because some of the alternative interface implementations' methods' result
    /// combinations could be erroneous. For instance, it would be the mistake to let to clone a part
    /// of nodes and to modify children at the same time, as it requires extra node binding of cloned
    /// and non-cloned nodes to take place, which simply would not be right in every particular scenario.
    /// 
    /// But if all of the logic is located in a single place, it is easier to distinguish wrong and right.
    /// 
    /// However, if the number of different node extract behaviors (or their features) will grow, then
    /// the alternative approach would be more preferable.
    /// </remarks>
    internal enum NodeExtractBehavior
    {
        /// <summary>
        /// Specifies that child nodes should be modified and all of the nodes should be cloned.
        /// This one should be used in conjunction with a <see cref="NodeTraverser.Traverse"/> method call.
        /// </summary>
        ModifyChildrenCloneAll,
        /// <summary>
        /// Specifies that child nodes should not be modified and only runs should be cloned.
        /// This one should be used when all of the node range nodes (including children) are coherently encountered.
        /// </summary>
        DontModifyChildrenCloneRuns
    }
}
