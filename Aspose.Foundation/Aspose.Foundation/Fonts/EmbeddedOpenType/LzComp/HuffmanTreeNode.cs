// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2011 by Konstantin Kornilov

namespace Aspose.Fonts.EmbeddedOpenType.LzComp
{
    /// <summary>
    /// Represents single node of Huffman Tree.
    /// </summary>
    internal class HuffmanTreeNode
    {
        /// <summary>
        /// Recursively calculates node weight for inner nodes.
        /// Does nothing for leaf nodes.
        /// </summary>
        public void CalculateWeight()
        {
            if (IsLeaf)
                return;

            LeftChild.CalculateWeight();
            RightChild.CalculateWeight();

            Weight = LeftChild.Weight + RightChild.Weight;
        }

        public void SetLeftChild(HuffmanTreeNode child)
        {
            LeftChild = child;
            child.Parent = this;
        }

        public void SetRightChild(HuffmanTreeNode child)
        {
            RightChild = child;
            child.Parent = this;
        }

        /// <summary>
        /// True if the node is a leaf.
        /// </summary>
        public bool IsLeaf
        {
            get { return (LeftChild == null || RightChild == null); }
        }

        /// <summary>
        /// Index of the node in array.
        /// </summary>
        public int Index;

        /// <summary>
        /// Parent node.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        public HuffmanTreeNode Parent;

        /// <summary>
        /// Left child node.
        /// </summary>
        public HuffmanTreeNode LeftChild;

        /// <summary>
        /// Right child node.
        /// </summary>
        public HuffmanTreeNode RightChild;

        /// <summary>
        /// Symbol represented by the node.
        /// </summary>
        public int Symbol;

        /// <summary>
        /// Weight of the tree node.
        /// </summary>
        public long Weight;
    }
}
