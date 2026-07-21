// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2016 by Ilya Navrotskiy

using System;
using System.IO;
using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.LzComp
{
    /// <summary>
    /// Allows to compress data using LZCOMP algorithm.
    /// </summary>
    /// <remarks>
    /// This code is ported and refactored version of C code.
    /// Original C code is available at http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#Sample 
    /// or http://code.google.com/p/ttf2eot/source/browse/trunk/ (files lzcomp.c and lzcomp.h).
    /// All constant values are taken from C code and altering them may lead to incorrect encoding result.
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal static class LzCompEncoder
    {
        /// <summary>
        /// Packs data.
        /// </summary>
        internal static byte[] PackData(byte[] data)
        {
            // MS Word packs empty data arrays into four zero bytes.
            if (data == null || data.Length == 0)
                return new byte[4];
            
            // LZComp encodes data only with size less than three bytes length.
            if ((data.Length & 0xFF000000) != 0)
                throw new ArgumentOutOfRangeException("data");

            byte[] rleEncoded = RleCoder.Encode(data);
            // Only use run-length encoding if there is a clear benefit.
            bool isRleEncoded = (rleEncoded.Length < (data.Length * 3 / 4));
            byte [] dataSource = (isRleEncoded) ? rleEncoded : data;

            return Encode(dataSource, isRleEncoded);
        }

        /// <summary>
        /// Encodes specified data.
        /// </summary>
        private static byte[] Encode(byte[] data, bool isRleEncoded)
        {
            MemoryStream stream = new MemoryStream(); // Will be disposed with writer.
            using (BitBinaryWriter writer = new BitBinaryWriter(stream, true))
            {
                writer.WriteBit(isRleEncoded);
                writer.WriteValue(data.Length, 24);

                LzCompEncoderContext context = new LzCompEncoderContext(data, writer);
                
                int index = context.PreLoadData.Length;
                while (index < context.MaxIndex)
                {
                    int curPos = index;
                    Match match = MakeCopyDecision(context, index++);
                    if (match.Length > 0)
                    {
                        // The piece of data which already met is found. Write length and distance for it.
                        int distanceRanges = GetNumberOfDistanceRanges(match.Distance);
                        EncodeLength(context, match.Length, match.Distance, distanceRanges);
                        EncodeDistance(context, match.Distance, distanceRanges);

                        for (int j = 1; j < match.Length; j++)
                            context.UpdateHashTable(index++);
                    }
                    else
                    {
                        byte symbol = context.GetSymbol(curPos);
                        if ((curPos >= 2) && (symbol == context.GetSymbol(curPos - 2)))
                        {
                            // The symbol met two positions earlier. Write one byte command.
                            context.SymbolCoder.WriteSymbolAndUpdateTree(context.Dup2CommandSymbol);
                        }
                        else
                        {
                            if ((curPos >= 4) && (symbol == context.GetSymbol(curPos - 4)))
                            {
                                // The symbol met four positions earlier. Write one byte command.
                                context.SymbolCoder.WriteSymbolAndUpdateTree(context.Dup4CommandSymbol);
                            }
                            else
                            {
                                if ((curPos >= 6) && (symbol == context.GetSymbol(curPos - 6)))
                                {
                                    // The symbol met six positions earlier. Write one byte command.
                                    context.SymbolCoder.WriteSymbolAndUpdateTree(context.Dup6CommandSymbol);
                                }
                                else
                                {
                                    // The symbol did not meet yet. Just write it.
                                    context.SymbolCoder.WriteSymbolAndUpdateTree(symbol);
                                }
                            }
                        }
                    }
                }

                writer.Flush();
                return stream.ToArray();
            }
        }
        
        /// <summary>
        /// Encodes the distance.
        /// </summary>
        private static void EncodeDistance(LzCompEncoderContext context, int distance, int distanceRanges)
        {
            int value = distance - MinDistance;
            int distanceRangesCount = (distanceRanges - 1) * WidthOfDistance;

            for (int i = distanceRangesCount; i >= 0; i -= WidthOfDistance)
            {
                int distanceSymbol = (value >> i) & BitMask;
                context.DistanceCoder.WriteSymbolAndUpdateTree(distanceSymbol);
            }
        }

        /// <summary>
        /// Encodes the length and the number of distance ranges used for this distance.
        /// </summary>
        private static void EncodeLength(LzCompEncoderContext context, int length, int distance, int numDistRanges)
        {
            int value = length - ((distance >= MaxTwoBytesDistance) ? MinLength3 : MinLength);

            int bitsUsed = BitUtil.BitsUsed(value);

            int i = BitRange;
            while (i < bitsUsed)
                i += BitRange;
            int mask = 1 << (i-1);
            
            int symbol = (bitsUsed > BitRange) ? 2 : 0;

            // Repeat gBitRange times, hard-wire so that we do not have to loop
            /* 1 */
            if ((value & mask) !=0)
                symbol |= 1;
            mask >>= 1;
            /* 2 */
            symbol <<= 1;
            if ((value & mask) != 0)
                symbol |= 1;
            mask >>= 1;


            context.SymbolCoder.WriteSymbolAndUpdateTree(256 + symbol + (numDistRanges - 1) * (1 << WidthOfLength));

            i = bitsUsed - BitRange;
            while(i >= 1)
            {
                symbol = (i > BitRange) ? 2 : 0;

                // Repeat gBitRange times, hard-wire so that we do not have to loop
                /* 1 */
                if ((value & mask) != 0)
                    symbol |= 1;
                mask >>= 1;
                /* 2 */
                symbol <<= 1;
                if ((value & mask) != 0)
                    symbol |= 1;
                mask >>= 1;
        
                i -= BitRange;

                context.LengthCoder.WriteSymbolAndUpdateTree(symbol);
            }
        }

        /// <summary>
        /// Finds matches in already browsed data.
        /// </summary>
        private static Match FindMatch(LzCompEncoderContext context, int indexInData)
        {
            const int lenMaxCostCache = 32;
            int[] literalCostCache = new int[lenMaxCostCache + 1];
            
            Match bestMatch = new Match();

            int hashNodesCount = 0;
            int maxIndexMinusIndex = context.MaxIndex - indexInData;
            int maxComputedLength = 0;

            if (maxIndexMinusIndex > 1)
            {
                // Store in hash 2-bytes values.
                int symbol = context.GetTwoBytesSymbol(indexInData);
                
                HashNode prevHashNode = null;
                HashNode curHashNode = context.HashTable[symbol];
                while (curHashNode != null)
                {
                    int indexBrowsed = curHashNode.Index;
                    int distance = indexInData - indexBrowsed;

                    hashNodesCount++;
                    if ((hashNodesCount > MaxSymbolNodesInCache) || (distance > LzCompEncoderContext.MaxCopyDistance))
                    {
                        context.ClearHash(symbol, prevHashNode, curHashNode);
                        break;
                    }

                    prevHashNode = curHashNode;
                    curHashNode = curHashNode.Next;

                    int maxLength = Math.Min(maxIndexMinusIndex, indexInData - indexBrowsed);
                    if ( maxLength < MinLength )
                        continue;

                    // Calculate length of the found coinciding piece.
                    // Since we found first symbol (2 bytes) in hash, we can skip it.
                    int length = context.CalcLength(indexBrowsed+2, indexInData+2, maxLength-2) + 2;

                    distance = distance - length + 1; 
                    if (distance > context.DistanceMax)
                        continue;

                    if ((length == 2) && (distance >= MaxTwoBytesDistance))
                        continue;

                    if ((length <= bestMatch.Length) && (distance > bestMatch.Distance))
                    {
                        // Speed optimization.
                        if (length <= bestMatch.Length - 2)
                            continue;

                        if (distance > (bestMatch.Distance << LzCompDecoderContext.DistanceWidth))
                        {
                            // Speed optimization.
                            if (length < bestMatch.Length)
                                continue;
                            // Speed optimization.
                            if (distance > (bestMatch.Distance << (LzCompDecoderContext.DistanceWidth + 1)))
                                continue;
                        }
                    }

                    int literalCost;
                    if (length > maxComputedLength)
                    {
                        int computedLength = (length > lenMaxCostCache) ? lenMaxCostCache : length;

                        for (int i = maxComputedLength; i < computedLength; i++)
                        {
                            literalCost = context.SymbolCoder.GetSymbolCost(context.GetSymbol(indexInData + i));
                            literalCostCache[i + 1] = literalCostCache[i] + literalCost;
                        }

                        maxComputedLength = computedLength;
                        if (length > lenMaxCostCache)
                        {
                            literalCost = literalCostCache[lenMaxCostCache];
                            // Just approximate.
                            literalCost += literalCost / lenMaxCostCache * (length - lenMaxCostCache);
                        }
                        else
                        {
                            literalCost = literalCostCache[length];
                        }
                    }
                    else
                    {
                        literalCost = literalCostCache[length];
                    }

                    if (literalCost > bestMatch.Gain)
                    {
                        int distRanges = GetNumberOfDistanceRanges(distance);
                        int copyCost = GetEncodedLengthCost(context, length, distance, distRanges);

                        // Conservatively assumes only one bit per range for distanceBitCount.
                        if (literalCost - copyCost - (distRanges << 16) > bestMatch.Gain)
                        {
                            copyCost += GetEncodedDistanceCost(context, distance, distRanges);
                            int gain = literalCost - copyCost;

                            if (gain > bestMatch.Gain)
                            {
                                bestMatch.Gain = gain;
                                bestMatch.Length = length;
                                bestMatch.Distance = distance;
                                bestMatch.CopyCost = copyCost;
                            }
                        }
                    }
                }
            }

            return bestMatch;
        }

        /// <summary>
        /// Returns cost of encoded distance.
        /// </summary>
        private static int GetEncodedDistanceCost(LzCompEncoderContext context, int distance, int distanceRanges)
        {
            int value = distance - MinDistance;
            int distanceRangesCount = (distanceRanges - 1) * WidthOfDistance;

            int distanceCost = 0;
            for (int i = distanceRangesCount; i >= 0; i -= WidthOfDistance)
            {
                int distanceSymbol = (value >> i) & BitMask;
                distanceCost += context.DistanceCoder.GetSymbolCost(distanceSymbol);
            }

            return distanceCost;
        }

        /// <summary>
        /// Returns number of distance ranges.
        /// </summary>
        private static int GetNumberOfDistanceRanges(int distance)
        {
            int bitsNeeded = BitUtil.BitsUsed(distance - MinDistance);
            int distanceRanges = (bitsNeeded + WidthOfDistance - 1) / WidthOfDistance;
            return distanceRanges;
        }

        /// <summary>
        /// Returns cost of the encoded length.
        /// </summary>
        private static int GetEncodedLengthCost(LzCompEncoderContext context, int length, int distance, int numDistRanges)
        {
            int value = length - ((distance >= MaxTwoBytesDistance) ? MinLength3 : MinLength);

            int bitsUsed = BitUtil.BitsUsed(value);
            int i = BitRange;
            while (i < bitsUsed)
                i += BitRange;

            int mask = 1 << (i - 1);
            int symbol = (bitsUsed > BitRange) ? 2 : 0;

            //Repeat gBitRange times, hard-wire so that we do not have to loop.
            /* 1 */
            if ((value & mask) != 0)
                symbol |= 1;
            mask = mask >> 1;
            /* 2 */
            symbol = symbol << 1;
            if ((value & mask) != 0)
                symbol |= 1;
            mask = mask >> 1;
            
            int lengthCost = context.SymbolCoder.GetSymbolCost(256 + symbol +  (numDistRanges - 1) * (1 << WidthOfLength));
            
            i = bitsUsed - BitRange;
            while (i >= 1)
            {
                symbol = (i > BitRange) ? 2 : 0;
                
                //Repeat gBitRange times, hard-wire so that we do not have to loop.
                /* 1 */
                if ((value & mask) != 0)
                    symbol |= 1;
                mask >>= 1;
                /* 2 */
                symbol = symbol << 1;
                if ((value & mask) != 0)
                    symbol = symbol | 1;
                mask >>=1;
        
                i -= BitRange;

                lengthCost += context.LengthCoder.GetSymbolCost(symbol);
            }

            return lengthCost;
        }

        /// <summary>
        /// Returns match with the optimal length and distance.
        /// </summary>
        /// <remarks>
        /// Makes a decision on whether to use a copy item, and then if it decides
        /// to use a copy item it decides on an optimal length & distance for the copy.
        /// </remarks>
        private static Match MakeCopyDecision(LzCompEncoderContext context, int index)
        {
            Match match1 = FindMatch(context, index);

            int curPos = index;
            context.UpdateHashTable(index++);
            
            if (match1.Gain > 0)
            {
                Match match2 = FindMatch(context, index);
                int symbol = context.GetSymbol(curPos);
                int symbolCost = context.SymbolCoder.GetSymbolCost(symbol);

                if ((match2.Gain >= match1.Gain) &&
                    (match1.CostPerByte > (match2.CostPerByte * match2.Length + symbolCost) / (match2.Length + 1)))
                {
                    match1.Length = 0;
                }
                else if (match1.Length > 3)
                {
                    // Explore cutting back on match1.Length by one unit.
                    match2 = FindMatch(context, curPos + match1.Length);
                    if (match2.Length >= 2)
                    {
                        Match match3 = FindMatch(context, curPos + match1.Length - 1);
                        if ((match3.Length > match2.Length) && (match3.CostPerByte < match2.CostPerByte))
                        {
                            int distRanges = GetNumberOfDistanceRanges(match1.Distance + 1);

                            int lenBitCount = GetEncodedLengthCost(context, match1.Length - 1, match1.Distance + 1, distRanges);
                            int distBitCount = GetEncodedDistanceCost(context, match1.Distance + 1, distRanges);
                            
                            int cost1B = lenBitCount + distBitCount;
                            cost1B += match3.CostPerByte * match3.Length;

                            int cost1A = match1.CostPerByte * match1.Length;
                            cost1A += match2.CostPerByte * match2.Length;

                            if ((cost1A / (match1.Length + match2.Length)) > (cost1B / (match1.Length - 1 + match3.Length)))
                            {
                                match1.Length--;
                                match1.Distance++;
                            }
                        }
                    }
                }

                if (match1.Length == 2)
                {
                    int dup2Cost;
                    if ((curPos >= 2) && (context.GetSymbol(curPos) == context.GetSymbol(curPos - 2)))
                    {
                        dup2Cost = context.SymbolCoder.GetSymbolCost(context.Dup2CommandSymbol);
                        if ((match1.CostPerByte * 2) > (dup2Cost + context.SymbolCoder.GetSymbolCost(context.GetSymbol(curPos + 1))))
                            match1.Length = 0;
                    }
                    else if ((curPos >= 1) && ((curPos + 1) < context.MaxIndex) && (context.GetSymbol(curPos + 1) == context.GetSymbol(curPos - 1)))
                    {
                        dup2Cost = context.SymbolCoder.GetSymbolCost(context.Dup2CommandSymbol);
                        if ((match1.CostPerByte * 2) > (symbolCost + dup2Cost))
                            match1.Length = 0;
                    }
                }
            }
            return match1;
        }

        /// <summary>
        /// Constants taken from the LZCOMP.
        /// </summary>
        private const int MaxTwoBytesDistance = 512;
        private const int MinLength = 2;
        private const int MinLength3 = 3;
        private const int WidthOfLength = 3;
        private const int BitRange = WidthOfLength - 1;
        private const int MinDistance = 1;
        private const int WidthOfDistance = 3;
        private const int BitMask = (1 << WidthOfDistance) - 1;

        /// <summary>
        /// Limits maximum number of chains in cash for symbol.
        /// </summary>
        /// <remarks>
        /// Note, that original value in algorithm is 256,
        /// but experiments shows that decreasing this value improves 
        /// performance significantly and size of encoded data is not grows.
        /// </remarks>
        private const int MaxSymbolNodesInCache = 16;
        
        /// <summary>
        /// Match type that helps to make a decision on whether to use a copy item.
        /// </summary>
        /// <remarks>This type is only for the internal use by <see cref="LzCompEncoder"/>.</remarks>
        private class Match
        {
            // Match length.
            internal int Length
            {
                get { return mLength; }
                set { mLength = value; }
            }

            // Match distance.
            internal int Distance
            {
                get { return mDistance; }
                set { mDistance = value; }
            }

            // Match gain.
            internal int Gain
            {
                get { return mGain; }
                set { mGain = value; }
            }

            // Match copy cost.
            internal int CopyCost
            {
                get { return mCopyCost; }
                set { mCopyCost = value; }
            }

            // Match cost per byte.
            internal int CostPerByte
            {
                get { return (Length != 0) ? CopyCost / Length : 0; }
            }

            private int mLength;
            private int mDistance;
            private int mGain;
            private int mCopyCost;
        }
    }
}
