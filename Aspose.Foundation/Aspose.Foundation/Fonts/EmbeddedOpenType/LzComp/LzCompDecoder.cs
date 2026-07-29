// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2011 by Konstantin Kornilov

using System;
using System.IO;
using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.LzComp
{
    /// <summary>
    /// Allows to decompress data using LZCOMP algorithm.
    /// </summary>
    /// <remarks>
    /// This code is ported and refactored version of C code.
    /// Original C code is available at http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#Sample
    /// or http://code.google.com/p/ttf2eot/source/browse/trunk/ (files lzcomp.c and lzcomp.h).
    /// All constant values are taken from C code and altering them may lead to incorrect decoding result.
    /// </remarks>
    internal static class LzCompDecoder
    {
        /// <summary>
        /// Unpacks data.
        /// </summary>
        /// <param name="data">Compressed data.</param>
        /// <returns>Decompressed data.</returns>
        internal static byte[] UnpackData(byte[] data)
        {
            // kvk: C code from http://code.google.com/p/ttf2eot/source/browse/trunk/ always uses Version0
            // for compression and decompression. I have no idea where this value should be taken from.
            return UnpackData(data, LzCompVersion.Version0);
        }

        /// <summary>
        /// Unpacks data.
        /// </summary>
        /// <param name="data">Compressed data.</param>
        /// <param name="version">Codec version.</param>
        /// <returns>Decompressed data.</returns>
        private static byte[] UnpackData(byte[] data, LzCompVersion version)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException("data");

            using (MemoryStream stream = new MemoryStream(data))
            {
                BitBinaryReader reader = new BitBinaryReader(stream, true);

                bool rleUsed = false;
                if (version == LzCompVersion.Version0)
                    rleUsed = reader.ReadBit();

                int outputLength = (int)reader.ReadUnsignedValue(24);

                byte[] decodedLzComp = DecodeLzComp(reader, outputLength);

                if (rleUsed)
                    return RleCoder.Decode(decodedLzComp);
                else
                    return decodedLzComp;
            }
        }

        /// <summary>
        /// Decodes data from <paramref name="reader"/>.
        /// </summary>
        private static byte[] DecodeLzComp(BitBinaryReader reader, int outputLength)
        {
            if (outputLength == 0)
                return ArrayUtil.EmptyByteArray;

            using (MemoryStream outputStream = new MemoryStream(outputLength))
            {
                LzCompDecoderContext context = new LzCompDecoderContext(outputLength, reader);

                while (outputStream.Position < outputLength)
                {
                    int symbol = context.SymbolCoder.ReadSymbolAndUpdateTree();

                    if (symbol < 256)
                    {
                        // Literal item.
                        outputStream.WriteByte((byte)symbol);
                    }
                    else if (symbol == context.Dup2CommandSymbol)
                    {
                        // One byte copy item.
                        byte copyItem = GetOneByteCopyItem(context, outputStream, 2);
                        outputStream.WriteByte(copyItem);
                    }
                    else if (symbol == context.Dup4CommandSymbol)
                    {
                        // One byte copy item.
                        byte copyItem = GetOneByteCopyItem(context, outputStream, 4);
                        outputStream.WriteByte(copyItem);
                    }
                    else if (symbol == context.Dup6CommandSymbol)
                    {
                        // One byte copy item.
                        byte copyItem = GetOneByteCopyItem(context, outputStream, 6);
                        outputStream.WriteByte(copyItem);
                    }
                    else
                    {
                        // Multi byte copy item.
                        int length = DecodeLength(context, symbol);
                        int distance = DecodeDistance(context, symbol);
                        if (distance >= 512)
                            length++;
                        byte[] copyItem = GetCopyItem(context, outputStream, distance + length - 1, length);
                        outputStream.Write(copyItem, 0, copyItem.Length);
                    }
                }
                return outputStream.ToArray();
            }
        }

        /// <summary>
        /// Reads one byte copy item from stream. Stream position is not modified.
        /// </summary>
        /// <param name="context">Decoder context.</param>
        /// <param name="stream">Stream.</param>
        /// <param name="offset">Offset of the copy item from current stream position.</param>
        /// <returns>Copy item.</returns>
        private static byte GetOneByteCopyItem(LzCompDecoderContext context, Stream stream, int offset)
        {
            byte[] copyItem = GetCopyItem(context, stream, offset, 1);
            return copyItem[0];
        }

        /// <summary>
        /// Reads copy item from stream. Stream position is not modified.
        /// </summary>
        /// <param name="context">Decoder context.</param>
        /// <param name="stream">Stream.</param>
        /// <param name="offset">Offset of the beginning of copy item from current stream position.</param>
        /// <param name="length">Length of copy item.</param>
        /// <returns>Copy item.</returns>
        private static byte[] GetCopyItem(LzCompDecoderContext context, Stream stream, int offset, int length)
        {
            // Copy item start position should not exceeds PreLoadData.
            if (stream.Position - offset < -LzCompDecoderContext.PreLoadDataSize)
                throw new InvalidOperationException("Invalid copy item offset.");

            // Copy item start position should not be greater than offset.
            if (length > offset)
                throw new InvalidOperationException("Invalid copy item length.");

            byte[] copyItem = new byte[length];

            long startPosition = stream.Position - offset;

            // If copy item position exceeds the stream we should read bytes from the end of mPreLoadData array.
            if (startPosition < 0)
            {
                long startPositionInPreLoadData = LzCompDecoderContext.PreLoadDataSize + startPosition;
                long lenghtInPreLoadData = Math.Min(LzCompDecoderContext.PreLoadDataSize - startPositionInPreLoadData, length);
                Array.Copy(context.PreLoadData, startPositionInPreLoadData, copyItem, 0, lenghtInPreLoadData);
            }

            if (startPosition + length > 0)
            {
                long startPositionInStream = Math.Max(0, startPosition);
                int lenghtInStream = (int)(startPosition + length - startPositionInStream);
                byte[] dataInStream = ReadDataFromStremAndKeepPosition(stream, startPositionInStream, lenghtInStream);
                Array.Copy(dataInStream, 0, copyItem, length - lenghtInStream, lenghtInStream);
            }

            return copyItem;
        }

        private static byte[] ReadDataFromStremAndKeepPosition(Stream stream, long startPosition, int length)
        {
            long originalPosition = stream.Position;

            byte[] data = new byte[length];
            stream.Position = startPosition;
            StreamUtil.Read(stream, data, 0, length);

            stream.Position = originalPosition;

            return data;
        }


        private static int DecodeLength(LzCompDecoderContext context, int symbol)
        {
            const int stopBitMask = 0x4;
            const int valueMask = 0x3;

            // Read values from reader using mLengthDecoder until stop bit found.
            // First value is (symbol - 256)%8.
            // First 2 bits of the value is next 2 bits of length.
            // 3rd bit is stop bit.
            int bits = (symbol - 256)%8;
            int length = 0;
            while (true)
            {
                length = length << 2;
                length += (bits & valueMask);
                if ((bits & stopBitMask) == 0)
                    break;

                bits = context.LengthCoder.ReadSymbolAndUpdateTree();
            }
            length += 2;

            return length;
        }

        private static int DecodeDistance(LzCompDecoderContext context, int symbol)
        {
            int numberDistanceRanges = (symbol - 256)/8 + 1;

            int distance = 0;
            for (int i = 0; i < numberDistanceRanges; i++)
            {
                int bits = context.DistanceCoder.ReadSymbolAndUpdateTree();
                distance = distance << 3;
                distance += bits;
            }
            distance++;

            return distance;
        }
    }
}
