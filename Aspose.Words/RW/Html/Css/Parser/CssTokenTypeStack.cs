// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/06/2022 by Victor Chebotok

using System;
using Aspose.Collections;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// A stack of <see cref="CssTokenType"/> values.
    /// </summary>
    /// <remarks>
    /// This class is optimized to reduce memory usage in scenarios where most of stored values are equal.
    /// It utilizes a run-length encoding to reduce the number of values in memory.
    /// </remarks>
    internal class CssTokenTypeStack
    {
        internal void Push(CssTokenType tokenType)
        {
            // Check if the new token is equal to the token that has been pushed last.
            if (!IsEmpty && (tokenType == Peek()))
            {
                // If tokens are equal, we don't push the new token and just increase the token counter instead.
                // That is, we use run-length encoding for space optimization.
                int lastItemIndex = mTokenCount.Count - 1;
                mTokenCount[lastItemIndex] = mTokenCount[lastItemIndex] + 1;
            }
            else
            {
                // If tokens are not equal (or no tokens have been pushed yet), we push the new token normally. 
                mTokens.Add((int)tokenType);
                mTokenCount.Add(1);
            }
        }

        internal void Pop()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException();
            }

            int index = mTokenCount.Count - 1;
            mTokenCount[index] = mTokenCount[index] - 1;

            if (mTokenCount[index] <= 0)
            {
                mTokens.RemoveAt(index);
                mTokenCount.RemoveAt(index);
            }
        }

        internal CssTokenType Peek()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException();
            }

            return (CssTokenType)mTokens[mTokens.Count - 1];
        }

        internal bool IsEmpty
        {
            get { return mTokens.Count == 0; }
        }

        private readonly IntList mTokens = new IntList();
        private readonly IntList mTokenCount = new IntList();
    }
}
