// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2016 by Ilya Navrotskiy

using Aspose.Collections;
using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.LzComp
{
    /// <summary>
    /// Represents context of the LZCOMP encoder.
    /// </summary>
    internal class LzCompEncoderContext : LzCompDecoderContext
    {

        /// <summary>
        /// Ctor.
        /// </summary>
        internal LzCompEncoderContext(byte[] data, BitBinaryWriter writer) : base(data.Length, writer)
        {
            // IN. This is speed optimization. Actually, we could use two separate arrays here
            // and return appropriate symbol in GetSymbol() method depending on requested index.
            // But access to GetSymbol() method is very intensive and
            // use of integrated array considerably increases performance.
            mData = new byte[PreLoadDataSize + data.Length];
            PreLoadData.CopyTo(mData, 0);
            data.CopyTo(mData, PreLoadDataSize);

            for (int i = 1; i < PreLoadDataSize; i++)
                UpdateHashTable(i);
        }

        /// <summary>
        /// Updates hash table with symbol in the specified index position.
        /// </summary>
        internal void UpdateHashTable(int index)
        {
            if (index < 1)
                return;

            byte curByte = mData[index];
            ushort prevByte = mData[index - 1];

            int pos = (prevByte << 8) | curByte;

            HashNode hNode = new HashNode(index-1, HashTable[pos]);
            HashTable[pos] = hNode;
        }

        /// <summary>
        /// Returns symbol from the data array.
        /// </summary>
        /// <remarks>
        /// There is PreLoadData at the beginning of data array and
        /// there are data which should be encoded after that.
        /// See <see cref="LzCompEncoderContext(byte[], BitBinaryWriter)"/> for more details.
        /// </remarks>
        internal byte GetSymbol(int index)
        {
            return mData[index];
        }

        /// <summary>
        /// Returns two-byte symbol.
        /// Gets high byte from the current index and lo byte from the next index.
        /// This is used in <see cref="HashTable"/>.
        /// </summary>
        internal int GetTwoBytesSymbol(int index)
        {
            int symbol = mData[index];
            symbol = symbol << 8;
            symbol = symbol | mData[index + 1];

            return symbol;
        }

        /// <summary>
        /// Returns number of equal symbols in <see cref="mData"/>
        /// starting from <paramref name="startIndex1"/> and <paramref name="startIndex2"/>.
        /// </summary>
        internal int CalcLength(int startIndex1, int startIndex2, int maxLength)
        {
            int length = 0;
            while (length < maxLength)
            {
                if (mData[startIndex1++] != mData[startIndex2++])
                    break;

                length++;
            }

            return length;
        }

        /// <summary>
        /// Clears chain of hash nodes for specified symbol.
        /// </summary>
        internal void ClearHash(int symbol, HashNode prevHashNode, HashNode curHashNode)
        {
            if (HashTable[symbol] == curHashNode)
                HashTable.Remove(symbol);
            else if (prevHashNode != null)
                prevHashNode.Next = null;

            HashNode curNode = curHashNode;
            while (curNode != null)
            {
                HashNode prevNode = curNode;
                curNode = curNode.Next;
                prevNode.Next = null;
            }
        }

        /// <summary>
        /// Symbols hash table.
        /// The key is symbol position.
        /// The value is hash node for this symbol.
        /// </summary>
        internal IntToObjDictionary<HashNode> HashTable
        {
            get { return mHashTable; }
        }

        /// <summary>
        /// Maximum allowed copy distance.
        /// </summary>
        internal const int MaxCopyDistance = 0x7fffffff;

        private readonly IntToObjDictionary<HashNode> mHashTable = new IntToObjDictionary<HashNode>();

        /// <summary>
        /// The data for encoding.
        /// </summary>
        private readonly byte[] mData;
    }

    /// <summary>
    /// Hash node for symbols and their positions.
    /// </summary>
    /// <remarks>This type is only for the internal use by <see cref="LzCompEncoderContext"/>.</remarks>
    internal class HashNode
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal HashNode(int index, HashNode nextNode)
        {
            mIndex = index;
            mNext = nextNode;
        }

        /// <summary>
        /// Index of symbol, which this HashNode represents.
        /// </summary>
        internal int Index
        {
            get { return mIndex; }
            set { mIndex = value; }
        }

        /// <summary>
        /// Hash node for next symbol.
        /// </summary>
        internal HashNode Next
        {
            get { return mNext; }
            set { mNext = value; }
        }

        private int mIndex;
        private HashNode mNext;
    }
}

