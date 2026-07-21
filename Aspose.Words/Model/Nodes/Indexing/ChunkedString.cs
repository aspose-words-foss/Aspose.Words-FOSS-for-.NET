// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/11/2022 by Alexey Morozov

using System;
using System.Collections.Generic;
using System.Text;

namespace Aspose.Words
{
    /// <summary>
    /// Implements fast insertion string.
    /// </summary>
    /// <remarks>
    /// AM. During WORDSNET-22225 profiling I found that half of execution time spent to string.Insert() operations.
    /// WORDSNET-22225 documents are huge and contains about 300k of text and insert operations made many times on such
    /// string are slow.
    ///
    /// This class implements string split into smaller chunks which allow to do insert operation much faster.
    /// </remarks>
    internal class ChunkedString
    {
        internal ChunkedString(string text)
        {
            const int defaultChunkLength = 1000;

            int completeItems = text.Length / defaultChunkLength;
            int completeItemsLength = completeItems * defaultChunkLength;

            for (int i = 0; i < completeItems; i++)
                mChunks.Add(text.Substring(i * defaultChunkLength, defaultChunkLength));

            if (text.Length - completeItemsLength > 0)
                mChunks.Add(text.Substring(completeItemsLength, text.Length - completeItemsLength));

            Length = text.Length;

            Invalidate();
        }

        internal string Substring(int startIndex, int length)
        {
            if (length == 0)
                return string.Empty;

            int itemIndex = 0;
            int chunkStart = 0;
            int thisIndex = startIndex;

            if (mLastChunkStart <= thisIndex)
            {
                itemIndex = mLastChunkIndex;
                chunkStart = mLastChunkStart;

                thisIndex -= mLastChunkStart;
            }

            for (int i = itemIndex; i < mChunks.Count; i++)
            {
                if (thisIndex < mChunks[i].Length)
                {
                    // Here we found needed part.
                    int thisLength = length;

                    mLastChunkIndex = i;
                    mLastChunkStart = chunkStart;

                    StringBuilder sb = new StringBuilder(length);
                    while (thisLength > 0)
                    {
                        string part = mChunks[i].Substring(thisIndex, System.Math.Min(thisLength, mChunks[i].Length - thisIndex));

                        sb.Append(part);

                        thisLength -= part.Length;
                        thisIndex = 0;
                        i++;
                    }

                    return sb.ToString();
                }

                thisIndex -= mChunks[i].Length;
                chunkStart += mChunks[i].Length;
            }

            throw new IndexOutOfRangeException();
        }

        internal void Insert(int startIndex, string value)
        {
            int thisIndex = startIndex;
            for (int i = 0; i < mChunks.Count; i++)
            {
                if (thisIndex <= mChunks[i].Length)
                {
                    mChunks[i] = mChunks[i].Insert(thisIndex, value);

                    Length += value.Length;

                    Invalidate();

                    return;
                }

                thisIndex -= mChunks[i].Length;
            }

            throw new IndexOutOfRangeException();
        }

        internal char this[int index]
        {
            get
            {
                int itemIndex = 0;
                int chunkStart = 0;

                if (mLastChunkStart <= index)
                {
                    itemIndex = mLastChunkIndex;
                    chunkStart = mLastChunkStart;
                    index -= mLastChunkStart;
                }

                for (int i = itemIndex; i < mChunks.Count; i++)
                {
                    string chunk = mChunks[i];

                    if (index < chunk.Length)
                    {
                        mLastChunkIndex = i;
                        mLastChunkStart = chunkStart;

                        return chunk[index];
                    }

                    index -= chunk.Length;
                    chunkStart += chunk.Length;
                }

                throw new IndexOutOfRangeException();
            }
        }

        private void Invalidate()
        {
            mLastChunkIndex = 0;
            mLastChunkStart = 0;
        }

        internal int Length { get; private set; }

        internal string AsString
        {
            get
            {
                StringBuilder sb = new StringBuilder(Length);

                foreach (string chunk in mChunks)
                    sb.Append(chunk);

                return sb.ToString();

            }
        }

        private int mLastChunkIndex;
        private int mLastChunkStart;

        private readonly List<string> mChunks = new List<string>();
    }
}
