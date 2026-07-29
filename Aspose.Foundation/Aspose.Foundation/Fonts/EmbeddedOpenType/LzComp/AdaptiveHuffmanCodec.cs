// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2011 by Konstantin Kornilov

using Aspose.Collections;
using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.LzComp
{
    /// <summary>
    /// Allows to decode symbols from bit array using adaptive Huffman tree.
    /// </summary>
    /// <remarks>
    /// This code is ported and refactored version of C code.
    /// Original C code is available at http://www.w3.org/Submission/2008/SUBM-MTX-20080305/ 
    /// or http://code.google.com/p/ttf2eot/source/browse/trunk/ (files ahuff.c and ahuff.h).
    /// All LZCOMP specific operations are moved to LzCompDecoder class and this class is reference implementation 
    /// of Adaptive Huffman Coding. 
    /// But for any modification of this class the tree should stay exactly the same on any step of decoding. Any difference 
    /// in the tree at any step of decoding may produce wrong output.
    /// </remarks>
    internal class AdaptiveHuffmanCodec
    {
        /// <summary>
        /// Creates codec for symbols from 0 to <paramref name="symbolsCount"/>-1.
        /// Initial symbols weight equals 1.
        /// </summary>
        internal AdaptiveHuffmanCodec(int symbolsCount, BitBinaryIoBase bitIo)
        {
            mBitIo = bitIo;
            // For the simplicity node array will be initialized as follows:
            // Root node has index = 1.
            // For each node except leafs left child index = index*2.
            // For each node except leafs right child index = index*2+1.
            // For each node except root parent index = index/2.
            // Leaf node indices begins from symbolsCount ordered by increasing symbol value.
            // Array element with index = 1 will not be used.
            int treeSize = 2*symbolsCount;
            mNodes = new HuffmanTreeNode[treeSize];
            for (int i = 1; i < treeSize; i++)
            {
                HuffmanTreeNode node = new HuffmanTreeNode();
                node.Index = i;
                mNodes[i] = node;
            }

            for (int i = 1; i < treeSize; i++)
            {
                HuffmanTreeNode node = mNodes[i];

                if (i > 1)
                    node.Parent = mNodes[i/2];

                if (i < symbolsCount)
                {
                    node.LeftChild = mNodes[2*i];
                    node.RightChild = mNodes[2*i + 1];
                }

                if (i >= symbolsCount)
                {
                    node.Symbol = i - symbolsCount;
                    node.Weight = 1;
                    mLeafs.Add(node.Symbol, node);
                }
            }

            mRoot = mNodes[1];
            mRoot.CalculateWeight();
        }

        /// <summary>
        /// Increases specified symbol weight by 1.
        /// </summary>
        internal void IncreaseSymbolWeight(int symbol)
        {
            // In order to keep array ordering rule we should rearrange the array after increasing node weight.
            // The algorithm of rearranging:
            // 1. Start from the leaf node.
            // 2. Increase its weight.
            // 3. If ordering rule is broken, find a better place for the node and swap them in the array.
            // Ordering rule will be broken after increasing node weight by 1 only when some nodes
            // with lower indices has the same weight. So we will swap the current node with the last of them.
            // 4. Go to the node parent and repeat from step 2.
            HuffmanTreeNode node = mLeafs[symbol];
            while (true)
            {
                node.Weight++;

                if (node == mRoot)
                    break;

                if (mNodes[node.Index - 1].Weight == node.Weight - 1)
                {
                    int nodeIndexToSwap = node.Index - 1;
                    while (true)
                    {
                        if (mNodes[nodeIndexToSwap - 1].Weight >= node.Weight)
                        {
                            SwapNodes(node.Index, nodeIndexToSwap);
                            break;
                        }
                        nodeIndexToSwap--;
                        Debug.Assert(nodeIndexToSwap >= 1);
                    }
                }

                node = node.Parent;
            }
        }

        /// <summary>
        /// Reads next symbol from <see cref="mBitIo"/> and updates the tree.
        /// </summary>
        internal int ReadSymbolAndUpdateTree()
        {
            int symbol = ReadSymbol();
            IncreaseSymbolWeight(symbol);
            return symbol;
        }

        /// <summary>
        /// Writes next symbol to <see cref="mBitIo"/> and updates the tree.
        /// </summary>
        internal void WriteSymbolAndUpdateTree(int symbol)
        {
            WriteSymbol(symbol);
            IncreaseSymbolWeight(symbol);
        }

        /// <summary>
        /// Returns cost of symbol.
        /// </summary>
        /// <remarks>This is the length of path from the specified symbol node to the root.</remarks>
        internal int GetSymbolCost(int symbol)
        {
            HuffmanTreeNode symbolNode = mLeafs[symbol];
            int stepsToRoot = 0;
            while (symbolNode != mRoot)
            {
                stepsToRoot++;
                symbolNode = symbolNode.Parent;
            }

            return stepsToRoot * StepWeight;
        }

        /// <summary>
        /// Reads next symbol from <see cref="mBitIo"/>.
        /// </summary>
        private int ReadSymbol()
        {
            HuffmanTreeNode currentNode = mRoot;

            BitBinaryReader reader = (BitBinaryReader) mBitIo;
            while (!currentNode.IsLeaf)
            {
                bool bit = reader.ReadBit();
                currentNode = bit ? currentNode.RightChild : currentNode.LeftChild;
            }

            return currentNode.Symbol;
        }

        /// <summary>
        /// Writes next symbol to <see cref="mBitIo"/>.
        /// </summary>
        private void WriteSymbol(int symbol)
        {
            HuffmanTreeNode symbolNode = mLeafs[symbol];

            int bitsToWrite = 0;
            int curBit = 1;
            int bitsCount = 0;
            
            while (symbolNode != mRoot)
            {
                HuffmanTreeNode parent = symbolNode.Parent;
                bool bitValue = (parent.RightChild == symbolNode);
                bitsToWrite = BitUtil.SetBit(bitsToWrite, curBit, bitValue);

                curBit <<= 1;
                bitsCount++;
                symbolNode = parent;
            }
            BitBinaryWriter writer = (BitBinaryWriter) mBitIo;
            writer.WriteValue(bitsToWrite, bitsCount);
        }

        /// <summary>
        /// Swap two nodes in the array.
        /// </summary>
        private void SwapNodes(int index1, int index2)
        {
            HuffmanTreeNode node1 = mNodes[index1];
            HuffmanTreeNode node2 = mNodes[index2];

            mNodes[index1] = node2;
            mNodes[index2] = node1;

            mNodes[index1].Index = index1;
            mNodes[index2].Index = index2;

            SwapNodesParents(node1, node2);
        }

        private static void SwapNodesParents(HuffmanTreeNode node1, HuffmanTreeNode node2)
        {
            HuffmanTreeNode parent1 = node1.Parent;
            HuffmanTreeNode parent2 = node2.Parent;

            if(parent1.LeftChild == node1)
                parent1.SetLeftChild(node2);
            else
                parent1.SetRightChild(node2);

            if (parent2.LeftChild == node2)
                parent2.SetLeftChild(node1);
            else
                parent2.SetRightChild(node1);
        }

        /// <summary>
        /// Array of nodes.
        /// </summary>
        /// <remarks>
        /// Nodes in the array should be arranged by weight from high to low.
        /// Updating node weight requires rearranging the array.
        /// </remarks>
        private readonly HuffmanTreeNode[] mNodes;

        /// <summary>
        /// Leaf nodes hashtable.
        /// Key - node symbol.
        /// Value - node.
        /// </summary>
        private readonly IntToObjDictionary<HuffmanTreeNode> mLeafs = new IntToObjDictionary<HuffmanTreeNode>();

        /// <summary>
        /// Root node of the tree.
        /// </summary>
        private readonly HuffmanTreeNode mRoot;

        private readonly BitBinaryIoBase mBitIo;

        /// <summary>
        /// The multiplier for cost of one step in path from node to root (1 LSH 16).
        /// </summary>
        private const int StepWeight = 65536;
    }
}
