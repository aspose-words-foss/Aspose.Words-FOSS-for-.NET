// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

namespace Aspose.Words.Fields
{
    internal class NodeFieldBuildingBlock : IFieldBuildingBlock
    {
        internal NodeFieldBuildingBlock(Node node)
        {
            mNode = node;
        }

        void IFieldBuildingBlock.BuildBlock(DocumentBuilder documentBuilder)
        {
            documentBuilder.InsertNode(mNode);
        }

        private readonly Node mNode;
    }
}