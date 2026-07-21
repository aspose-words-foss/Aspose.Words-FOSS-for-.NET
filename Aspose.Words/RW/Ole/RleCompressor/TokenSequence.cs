// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2019 by Alexander Sevidov

using System;
using System.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// [MS-OVBA] 2.4.1.1.7 TokenSequence
    /// A TokenSequence is a FlagByte followed by an array of Tokens. The number of Tokens in the final 
    /// TokenSequence MUST be greater than or equal to 1. The number of Tokens in the final 
    /// TokenSequence MUST less than or equal to eight. All other TokenSequences in the 
    /// CompressedChunkData MUST contain eight Tokens.
    /// </summary>
    internal class TokenSequence
    {
        internal TokenSequence()
        {
            mFlagByte = 0x0;
            mCopyTokensCount = 0;
            mLiteralTokensCount = 0;
            mTokens = new List<RleToken>();
        }

        /// <summary>
        /// Count of tokens in the sequence.
        /// </summary>
        internal int TokensCount
        {
            get { return mTokens.Count; }
        }

        /// <summary>
        /// Adds a token to the sequence.
        /// </summary>
        internal void AddToken(RleToken token)
        {
            if (token.IsLiteral)
            {
                AddLiteralToken(token);
            }
            else
            {
                AddCopyToken(token);
            }                
        }

        /// <summary>
        /// Returnes compressed data of the sequence.
        /// </summary>
        internal byte[] GetData()
        {
            int size = mCopyTokensCount * BytesPerCopyToken + mLiteralTokensCount + 1;
            byte[] data = new byte[size];

            data[0] = mFlagByte;
            int position = 1;

            foreach (RleToken token in mTokens)
            {
                Array.Copy(token.Bytes, 0, data, position, token.Bytes.Length);
                position += token.Bytes.Length;
            }

            return data;
        }

        private void AddCopyToken(RleToken token)
        {
            if (TokensCount >= MaxTokensPerSequence)
                throw new ArgumentOutOfRangeException();

            mFlagByte |= (byte)(1 << mTokens.Count);
            mCopyTokensCount++;
            mTokens.Add(token);
        }

        private void AddLiteralToken(RleToken token)
        {
            if (TokensCount >= MaxTokensPerSequence)
                throw new ArgumentOutOfRangeException();

            mLiteralTokensCount++;
            mTokens.Add(token);
        }

        private byte mFlagByte;
        private readonly List<RleToken> mTokens;
        private int mCopyTokensCount;
        private int mLiteralTokensCount;

        private const int MaxTokensPerSequence = 8;
        private const int BytesPerCopyToken = 2;
    }
}
