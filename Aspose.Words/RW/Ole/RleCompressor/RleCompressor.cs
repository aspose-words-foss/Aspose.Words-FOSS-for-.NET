// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2019 by Alexander Sevidov

using System;
using System.Collections.Generic;
using System.IO;

namespace Aspose.Words
{
    /// <summary>
    /// Implements RLE compression used in VBA projects.
    /// See [MS-OVBA] 2.4.1 Compression and Decompression.
    /// </summary>
    internal class RleCompressor
    {
        private RleCompressor(byte[] chunk, BinaryWriter writer)
        {
            mUncompressedChunk = chunk;
            mWriter = writer;
        }

        /// <summary>
        /// Implements RLE decompression used in VBA projects.
        /// See [MS-OVBA] 2.4.1 Compression and Decompression
        /// </summary>
        internal static void Decompress(Stream inStream, MemoryStream outStream)
        {
            BinaryReader reader = new BinaryReader(inStream);
            BinaryWriter writer = new BinaryWriter(outStream);

            byte signatureByte = reader.ReadByte();
            Debug.Assert(signatureByte == 1);

            while (reader.BaseStream.Position < reader.BaseStream.Length - 1)
            {
                // Chunks array.
                int decompressedChunkStart = (int)writer.BaseStream.Position;

                int chunkStart = (int)reader.BaseStream.Position;

                int compressedHeader = reader.ReadUInt16();
                int compressedChunkSize = (compressedHeader & 0xfff) + 3;
                int compressedChunkSignature = (compressedHeader >> 12) & 0x7;
                Debug.Assert(compressedChunkSignature == 3);
                bool compressedChunkFlag = (compressedHeader >> 15) != 0;

                if (compressedChunkFlag)
                {
                    while (reader.BaseStream.Position < chunkStart + compressedChunkSize)
                    {
                        // Tokens array.
                        byte flagByte = reader.ReadByte();
                        for (int i = 0; i < 8 && (reader.BaseStream.Position < chunkStart + compressedChunkSize); i++)
                        {
                            bool literal = (flagByte & 1) == 0;
                            if (literal)
                            {
                                byte literalToken = reader.ReadByte();
                                writer.Write(literalToken);
                            }
                            else
                            {
                                ushort copyToken = reader.ReadUInt16();
                                int difference = (int)writer.BaseStream.Position - decompressedChunkStart;

                                RleToken.WriteDecompressedToken(copyToken, difference, writer);
                            }

                            flagByte = (byte)(flagByte >> 1);
                        }
                    }
                }
                else
                {
                    // So far seen only compressed chunks.
                    throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// [MS-OVBA] 2.4.1 Compression.
        /// </summary>
        internal static void Compress(Stream inStream, MemoryStream outStream)
        {
            BinaryWriter writer = new BinaryWriter(outStream);
            
            writer.Write(SignatureByte);

            using (BinaryReader reader = new BinaryReader(inStream))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
                    if (bytesToRead > MaxBytesPerChunk)
                        bytesToRead = MaxBytesPerChunk;

                    byte[] decompressedChunk = reader.ReadBytes(bytesToRead);

                    RleCompressor compressor = new RleCompressor(decompressedChunk, writer);
                    compressor.WriteCompressedChunk();
                }
            }
        }

        /// <summary>
        /// [MS-OVBA] 2.4.1.3.7 Compressing a DecompressedChunk.
        /// </summary>
        private void WriteCompressedChunk()
        {
            ReadData();

            if (mCompressedSize >= MaxBytesPerChunk)
            {
                WriteRawChunk();
            }
            else
            {
                WriteCompressed();
            }
        }

        /// <summary>
        /// Reads data from uncompressed array and creates a list of TokenSequences 
        /// ([MS-OVBA]2.4.1.1.7 TokenSequence).
        /// </summary>
        private void ReadData()
        {
            mTokenSequences.Clear();

            mCompressedSize = 0;

            int position = 0;

            while ((position < mUncompressedChunk.Length) && (mCompressedSize < MaxBytesPerChunk))
            {
                RleToken token = GetToken(position);
                position += token.Length;
                AddToken(token);
                mCompressedSize += token.Bytes.Length;
            }

            if (mSequence.TokensCount > 0)
            {
                mTokenSequences.Add(mSequence);
                mCompressedSize++;
            }
        }

        private void AddToken(RleToken token)
        {
            if (mSequence.TokensCount == MaxTokensPerSequence)
            {
                mTokenSequences.Add(mSequence);
                mSequence = new TokenSequence();
                mCompressedSize++;
            }

            mSequence.AddToken(token);
        }

        /// <summary>
        /// [MS-OVBA] 2.4.1.3.9 Compressing a Token. 
        /// </summary>
        private RleToken GetToken(int position)
        {            
            // [MS-OVBA] 2.4.1.3.19.4 Matching
            int decompressedCurrent = position;
            int decompressedEnd = mUncompressedChunk.Length;
            const int decompressedChunkStart = 0;

            int candidate = decompressedCurrent - 1;

            int bestLength = 0;
            int bestCandidate = 0;

            while (candidate >= decompressedChunkStart)
            {
                int c = candidate;
                int d = decompressedCurrent;
                int len = 0;

                while ((d < decompressedEnd) && (mUncompressedChunk[d] == mUncompressedChunk[c]))
                {
                    len++;
                    c++;
                    d++;
                }

                if (len > bestLength)
                {
                    bestLength = len;
                    bestCandidate = candidate;
                }
                candidate--;
            }

            if (bestLength >= 3)
            {
                return RleToken.CopyToken(position, bestCandidate, bestLength);
            }
            else 
            {
                return RleToken.LiteralToken(mUncompressedChunk[position]);
            }            
        }

        /// <summary>
        /// [MS-OVBA] 2.4.1.3.10 Compressing a RawChunk.
        /// </summary>
        private void WriteRawChunk()
        {
            ushort header = (ushort)(0x3000 | (MaxBytesPerChunk - 1));

            mWriter.Write(header);
            mWriter.Write(mUncompressedChunk);

            int padLength = MaxBytesPerChunk - mUncompressedChunk.Length;
            for (int i = 0; i < padLength; i++)
                mWriter.Write(PaddingByte);
        }

        /// <summary>
        /// [MS-OVBA] 2.4.1.3.7 Compressing a DecompressedChunk.
        /// </summary>
        private void WriteCompressed()
        {
            ushort header = (ushort)(0xb000 | (mCompressedSize - 1));

            mWriter.Write(header);

            foreach (TokenSequence seq in mTokenSequences)
                mWriter.Write(seq.GetData());
        }

        private const byte SignatureByte = 0x1;
        private const int MaxBytesPerChunk = 4096;
        private const int MaxTokensPerSequence = 8;
        private const byte PaddingByte = 0x0;

        private readonly byte[] mUncompressedChunk;
        private readonly List<TokenSequence> mTokenSequences = new List<TokenSequence>();
        private TokenSequence mSequence = new TokenSequence();
        private readonly BinaryWriter mWriter;
        private int mCompressedSize;
    }
}
