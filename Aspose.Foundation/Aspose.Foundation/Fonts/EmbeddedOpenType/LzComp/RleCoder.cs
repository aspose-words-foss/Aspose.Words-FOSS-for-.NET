// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2011 by Konstantin Kornilov

using System.IO;

namespace Aspose.Fonts.EmbeddedOpenType.LzComp
{
    /// <summary>
    /// Allows to process data using RLE.
    /// </summary>
    internal class RleCoder
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal RleCoder()
        {
            mState = RleDecoderState.Initial;
        }

        /// <summary>
        /// Encodes data.
        /// </summary>
        internal static byte[] Encode(byte[] data)
        {
            int[] counters = new int[256];

            // Initialize counters.
            for (int i = 0; i < data.Length; i++)
                counters[data[i]]++;

            // Find the least frequently used byte to use as the 'escape' symbol.
            byte escape = (byte) GetIndexOfMinElement(counters);

            using (MemoryStream stream = new MemoryStream())
            {
                // First write 'escape' symbol.
                stream.WriteByte(escape);

                int i = 0;
                while (i < data.Length)
                {
                    byte runLength = 1;
                    // Find chains of same symbols (runs).
                    while (((i + runLength) < data.Length) && (data[i + runLength] == data[i]) && (runLength < 255))
                        runLength++;

                    if ( runLength > 3 )
                    {
                        // Write: escape, runLength and byte symbol
                        stream.WriteByte(escape);
                        stream.WriteByte(runLength);
                        stream.WriteByte(data[i]);
                    }
                    else
                    {
                        runLength = 1;
                        // Just write out the byte symbol
                        stream.WriteByte(data[i]);

                        // If current symbol is 'escape' symbol,
                        // we should write 0 after 'escape'.
                        if (data[i] == escape)
                            stream.WriteByte(0);
                    }

                    i += runLength;
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Decodes data.
        /// </summary>
        internal static byte[] Decode(byte[] data)
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                RleCoder coder = new RleCoder();
                for (int i = 0; i < data.Length; i++)
                {
                    byte[] decodedData = coder.DecodeByte(data[i]);
                    if (decodedData.Length > 0)
                        outputStream.Write(decodedData, 0, decodedData.Length);
                }
                return outputStream.ToArray();
            }
        }

        /// <summary>
        /// Decodes single byte.
        /// </summary>
        private byte[] DecodeByte(byte value)
        {
            switch (mState)
            {
                case RleDecoderState.Initial:
                    mEscapeValue = value;
                    mState = RleDecoderState.Normal;
                    return ArrayUtil.EmptyByteArray;
                case RleDecoderState.Normal:
                    if (value == mEscapeValue)
                    {
                        mState = RleDecoderState.SeenEscape;
                        return ArrayUtil.EmptyByteArray;
                    }
                    else
                    {
                        return GetByteArray(value, 1);
                    }
                case RleDecoderState.SeenEscape:
                    if (value == 0)
                    {
                        mState = RleDecoderState.Normal;
                        return GetByteArray(mEscapeValue, 1);
                    }
                    else
                    {
                        mCount = value;
                        mState = RleDecoderState.NeedBytes;
                        return ArrayUtil.EmptyByteArray;
                    }
                case RleDecoderState.NeedBytes:
                    mState = RleDecoderState.Normal;
                    return GetByteArray(value, mCount);
                default:
                    return ArrayUtil.EmptyByteArray;
            }
        }

        /// <summary>
        /// Returns byte array specified length initialized by <paramref name="value"/> values.
        /// </summary>
        private static byte[] GetByteArray(byte value, int length)
        {
            byte[] result = new byte[length];
            for (int i = 0; i < length; i++)
                result[i] = value;
            return result;
        }


        /// <summary>
        /// Returns index of minimal element in array.
        /// </summary>
        private static int GetIndexOfMinElement(int[] data)
        {
            int minIndex = 0;
            int minValue = data[0];

            for (int i = 1; i < data.Length; i++)
            {
                // Despite array has 'int' type, its elements cannot be negative,
                // as it has a special purpose of symbols frequencies in RLE.
                if (minValue == 0)
                    return minIndex;

                if (data[i] >= minValue)
                    continue;

                minIndex = i;
                minValue = data[i];
            }

            return minIndex;
        }

        private RleDecoderState mState;
        private byte mEscapeValue;
        private int mCount;
    }
}
