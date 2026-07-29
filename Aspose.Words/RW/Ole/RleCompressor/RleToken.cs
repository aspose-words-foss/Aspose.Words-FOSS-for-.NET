// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2019 by Alexander Sevidov

using System;
using System.IO;

namespace Aspose.Words
{
    internal class RleToken
    {
        private RleToken(byte[] bytes, int length)
        {
            mBytes = bytes;
            mLength = length;
        }

        internal static RleToken LiteralToken(byte b)
        {
            byte[] bytes = new byte[1];
            bytes[0] = b;
            return new RleToken(bytes, 1);
        }

        /// <summary>
        /// [MS-OVBA] 2.4.1.3.19.3 Pack CopyToken.
        /// </summary>
        /// <remarks> 
        /// The comments above the code is a corresponding pseudocode from the specification.
        /// </remarks>
        internal static RleToken CopyToken(int difference, int bestCandidate, int bestLength)
        {
            // [MS-OVBA] 2.4.1.3.19.1 CopyToken Help
            int bitCount = GetBitCount(difference);

            // SET MaximumLength TO (0xFFFF RIGHT SHIFT BY BitCount) PLUS 3
            ushort maximumLength = (ushort)((0xffff >> bitCount) + 3);

            // [MS-OVBA] 2.4.1.3.19.3 Pack CopyToken

            // SET Length TO the MINIMUM of BestLength and MaximumLength
            int length = System.Math.Min(bestLength, maximumLength);

            // SET Offset TO DecompressedCurrent MINUS BestCandidate
            ushort offset = (ushort)(difference - bestCandidate);

            //SET temp1 TO Offset MINUS 1
            ushort temp1 = (ushort)(offset - 1);

            //SET temp2 TO 16 MINUS BitCount
            ushort temp2 = (ushort)(16 - bitCount);

            //SET temp3 TO Length MINUS 3
            ushort temp3 = (ushort)(length - 3);

            //SET Token TO (temp1 LEFT SHIFT BY temp2) BITWISE OR temp3
            ushort packedToken = (ushort)((temp1 << temp2) | temp3);

            byte[] bytes = BitConverter.GetBytes(packedToken);

            return new RleToken(bytes, length);
        }

        /// <summary>
        /// Decompresses copyToken and write it with using of <paramref name="writer"/>
        /// </summary>
        internal static void WriteDecompressedToken(ushort copyToken, int difference, BinaryWriter writer)
        {
            int bitCount = GetBitCount(difference);

            ushort lengthMask = (ushort)(0xffff >> bitCount);
            ushort offsetMask = (ushort)~lengthMask;

            int length = (copyToken & lengthMask) + 3;
            int temp1 = copyToken & offsetMask;
            int temp2 = 16 - bitCount;
            int offset = (temp1 >> temp2) + 1;

            int start = (int)writer.BaseStream.Position - offset;
            for (int j = 0; j < length; j++)
            {
                byte[] buffer = ((MemoryStream)writer.BaseStream).GetBuffer();
                byte b = buffer[start + j];
                writer.Write(b);
            }
        }

        [JavaAttributes.JavaThrows(true)]
        private static int GetBitCount(int difference)
        {
            // SET BitCount TO the smallest integer that is GREATER THAN OR EQUAL TO LOGARITHM base 2 
            // of difference
            int bitCount = (int)System.Math.Ceiling(System.Math.Log(difference, 2));

            // The number of bits used to encode Length MUST be greater than or equal to four. The 
            // number of bits used to encode Length MUST be less than or equal to 12
            // SET BitCount TO the maximum of BitCount and 4
            bitCount = System.Math.Max(bitCount, 4);

            if (bitCount > 12)
                throw new ArgumentException();

            return bitCount;
        }

        /// <summary>
        /// Compressed bytes of a token.
        /// </summary>
        internal byte[] Bytes
        {
            get { return mBytes;  }
        }

        /// <summary>
        /// Amount of bytes from uncompressed data.
        /// </summary>
        internal int Length
        {
            get { return mLength; }
        }

        /// <summary>
        /// Returnes true if this token is a literal token.
        /// </summary>
        internal bool IsLiteral
        {
            get { return (mBytes.Length == 1); }
        }

        private readonly byte[] mBytes;
        private readonly int mLength;
    }
}
