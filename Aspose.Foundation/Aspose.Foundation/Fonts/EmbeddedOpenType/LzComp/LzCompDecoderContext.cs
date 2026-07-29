// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2011 by Konstantin Kornilov

using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.LzComp
{
    /// <summary>
    /// Represents context of the LZCOMP decoder.
    /// </summary>
    internal class LzCompDecoderContext
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal LzCompDecoderContext(int outputLength, BitBinaryIoBase bitIo)
        {
            mOutputLength = outputLength;

            InitSymbolsCount();

            CreateDistanceCoder(bitIo);
            CreateLengthCoder(bitIo);
            CreateSymbolCoder(bitIo);

            InitPreLoadData();
        }

        /// <summary>
        /// Initializes symbols count based on output length.
        /// </summary>
        private void InitSymbolsCount()
        {
            int numberDistanceRanges = 1;
            mDistanceMax = (DistanceMin + (1 << (DistanceWidth * numberDistanceRanges)) - 1);
            while (DistanceMax < mOutputLength)
            {
                numberDistanceRanges++;
                mDistanceMax = (DistanceMin + (1 << (DistanceWidth * numberDistanceRanges)) - 1);
            }
            mDup2CommandSymbol = 256 + (1 << LenghtWidth) * numberDistanceRanges;
            mDup4CommandSymbol = mDup2CommandSymbol + 1;
            mDup6CommandSymbol = mDup4CommandSymbol + 1;
            mSymbolsCount = mDup6CommandSymbol + 1;
        }

        /// <summary>
        /// Creates distance coder.
        /// </summary>
        private void CreateDistanceCoder(BitBinaryIoBase bitIo)
        {
            mDistanceCoder = new AdaptiveHuffmanCodec((1 << DistanceWidth), bitIo);

            for (int i = 0; i < 2; i++)
                for (int j = 0; j < (1 << DistanceWidth); j++)
                {
                    mDistanceCoder.IncreaseSymbolWeight(j);
                }
        }

        /// <summary>
        /// Creates length coder.
        /// </summary>
        private void CreateLengthCoder(BitBinaryIoBase bitIo)
        {
            mLengthCoder = new AdaptiveHuffmanCodec((1 << LenghtWidth), bitIo);

            for (int i = 0; i < 2; i++)
                for (int j = 0; j < (1 << LenghtWidth); j++)
                {
                    mLengthCoder.IncreaseSymbolWeight(j);
                }
        }

        /// <summary>
        /// Creates symbol coder.
        /// </summary>
        private void CreateSymbolCoder(BitBinaryIoBase bitIo)
        {
            mSymbolCoder = new AdaptiveHuffmanCodec(mSymbolsCount, bitIo);

            mSymbolCoder.IncreaseSymbolWeight(256);
            mSymbolCoder.IncreaseSymbolWeight(257);

            for (int i = 0; i < 12; i++)
            {
                mSymbolCoder.IncreaseSymbolWeight(mDup2CommandSymbol);
            }

            for (int i = 0; i < 6; i++)
            {
                mSymbolCoder.IncreaseSymbolWeight(mDup4CommandSymbol);
            }
        }

        /// <summary>
        /// Initializes pre-load data.
        /// </summary>
        private void InitPreLoadData()
        {
            mPreLoadData = new byte[PreLoadDataSize];
            int position = 0;

            for (byte i = 0; i < 32; i++)
                for (byte j = 0; j < 96; j++)
                {
                    mPreLoadData[position++] = i;
                    mPreLoadData[position++] = j;
                }

            for (int i = 0; i < 256; i++)
            {
                mPreLoadData[position++] = (byte)i;
                mPreLoadData[position++] = (byte)i;
                mPreLoadData[position++] = (byte)i;
                mPreLoadData[position++] = (byte)i;
            }
        }

        /// <summary>
        /// Symbols table of predefined values for hashes.
        /// </summary>
        internal byte[] PreLoadData
        {
            get { return mPreLoadData; }
        }

        /// <summary>
        /// Distance coder.
        /// </summary>
        internal AdaptiveHuffmanCodec DistanceCoder
        {
            get { return mDistanceCoder; }
        }

        /// <summary>
        /// Length coder.
        /// </summary>
        internal AdaptiveHuffmanCodec LengthCoder
        {
            get { return mLengthCoder; }
        }

        /// <summary>
        /// Symbol coder.
        /// </summary>
        internal AdaptiveHuffmanCodec SymbolCoder
        {
            get { return mSymbolCoder; }
        }

        /// <summary>
        /// DUP2 command symbol.
        /// </summary>
        internal int Dup2CommandSymbol
        {
            get { return mDup2CommandSymbol; }
        }

        /// <summary>
        /// DUP4 command symbol.
        /// </summary>
        internal int Dup4CommandSymbol
        {
            get { return mDup4CommandSymbol; }
        }

        /// <summary>
        /// DUP6 command symbol.
        /// </summary>
        internal int Dup6CommandSymbol
        {
            get { return mDup6CommandSymbol; }
        }

        /// <summary>
        /// Maximum allowed distance.
        /// </summary>
        internal long DistanceMax
        {
            get { return mDistanceMax; }
        }

        /// <summary>
        /// Maximum allowed index of data elements.
        /// </summary>
        internal int MaxIndex
        {
            get { return mOutputLength + PreLoadDataSize; }
        }

        private readonly int mOutputLength;

        private AdaptiveHuffmanCodec mDistanceCoder;
        private AdaptiveHuffmanCodec mLengthCoder;
        private AdaptiveHuffmanCodec mSymbolCoder;

        private int mDup2CommandSymbol;
        private int mDup4CommandSymbol;
        private int mDup6CommandSymbol;
        private int mSymbolsCount;
        private byte[] mPreLoadData;

        private int mDistanceMax;

        /// <summary>
        /// Predefined by LZCOMP algorithm.
        /// </summary>
        internal const int PreLoadDataSize = 2 * 32 * 96 + 4 * 256;
        internal const int DistanceWidth = 3;
        private const int LenghtWidth = 3;
        private const int DistanceMin = 1;
    }
}
