// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/03/2015 by Edward Voronov

using System.Collections.Generic;
using Aspose.Words.Drawing;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Applies MERGEFORMAT to a field with image result.
    /// </summary>
    internal class ImageResultFieldMergeFormatApplier : IFieldResultFormatApplier
    {
        internal ImageResultFieldMergeFormatApplier(IEnumerable<Node> oldResultNodes)
        {
            mOldResultNodes = oldResultNodes;
        }

        void IFieldResultFormatApplier.ApplyFormat(NodeRange result)
        {
            Shape oldResultShape = GetShapeNode(mOldResultNodes);
            Shape newResultShape = GetShapeNode(result);

            if (oldResultShape != null && newResultShape != null)
            {
                newResultShape.SetWidthSafe(oldResultShape.Width);
                newResultShape.SetHeightSafe(oldResultShape.Height);
            }
        }

        private static Shape GetShapeNode(IEnumerable<Node> range)
        {
            foreach (Node node in range)
            {
                if (node.NodeType == NodeType.Shape)
                {
                    return (Shape)node;
                }
            }

            return null;
        }

        private readonly IEnumerable<Node> mOldResultNodes;
    }
}
