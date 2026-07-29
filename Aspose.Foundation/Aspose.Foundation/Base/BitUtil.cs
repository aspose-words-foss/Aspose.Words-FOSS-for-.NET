// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/11/2003 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose
{
    /// <summary>
    /// Bit utility functions.
    /// Functions that operate on bits within values of integral types.
    /// </summary>
    public static class BitUtil
    {
        /// <summary>
        /// Changes byte order.
        /// </summary>
        [CppSkipDefinition(false)] // replaced with compiler intrinsics
        public static long SwapInt64(long value)
        {
            return
                ((value & 0x00000000000000FF) << 56) |
                ((value & 0x000000000000FF00) << 40) |
                ((value & 0x0000000000FF0000) << 24) |
                ((value & 0x00000000FF000000) << 8)  |
                ((value & 0x000000FF00000000) >> 8)  |
                ((value & 0x0000FF0000000000) >> 24) |
                ((value & 0x00FF000000000000) >> 40) |
                ((value >> 56) & 0x00000000000000FF);
        }

        /// <summary>
        /// Changes byte order.
        /// </summary>
        [CppSkipDefinition(false)] // replaced with compiler intrinsics
        public static int SwapInt32(int value)
        {
            return (int)SwapUInt32((uint)value);
        }

        /// <summary>
        /// Changes byte order.
        /// </summary>
        [CppSkipDefinition(false)] // replaced with compiler intrinsics
        public static uint SwapUInt32(uint value)
        {
            return
                (((value & 0x000000FF) << 24) |
                ((value & 0x0000FF00) << 8) |
                ((value & 0x00FF0000) >> 8) |
                ((value & 0xFF000000) >> 24));    //Since the value is unsigned, it is safe to shift right.
        }

        /// <summary>
        /// Changes byte order.
        /// </summary>
        [CppSkipDefinition(false)] // replaced with compiler intrinsics
        public static short SwapInt16(short value)
        {
            return (short)SwapUInt16((ushort)value);
        }

        /// <summary>
        /// Changes byte order.
        /// </summary>
        [CppSkipDefinition(false)] // replaced with compiler intrinsics
        public static ushort SwapUInt16(ushort value)
        {
            //Since the value is unsigned, it is safe to shift right.
            return (ushort)(((value & 0x00FF) << 8) | ((value & 0xFF00) >> 8));
        }

        /// <summary>
        /// Sets bit(s) specified by the bitmask to 0 or 1 in the specified value.
        /// </summary>
        public static int SetBit(int curValue, int bitMask, bool newValue)
        {
            if (newValue)
                return curValue | bitMask;
            else
                return curValue & ~bitMask;
        }

        /// <summary>
        /// Returns true if the bit is set, false otherwise.
        /// </summary>
        public static bool IsSetInt32(int curValue, int bitMask)
        {
            return ((curValue & bitMask) != 0);
        }

        /// <summary>
        /// Returns true if the bit is set, false otherwise.
        /// </summary>
        public static bool IsSetUInt32(uint curValue, uint bitMask)
        {
            return ((curValue & bitMask) != 0);
        }

        /// <summary>
        /// Returns true if the bit is set, false otherwise.
        /// </summary>
        public static bool IsSetUInt16(ushort curValue, ushort bitMask)
        {
            return ((curValue & bitMask) != 0);
        }

        /// <summary>
        /// Returns true if the bit is set, false otherwise.
        /// </summary>
        public static bool IsSetByte(byte curValue, byte bitMask)
        {
            return ((curValue & bitMask) != 0);
        }

        public static int InvertBit(int curValue, int bitMask)
        {
            return curValue ^ bitMask;
        }

        /// <summary>
        /// This is from http://everything2.com/index.pl?node=counting%201%20bits
        /// </summary>
        [CppSkipDefinition(false)] // replaced with compiler intrinsics
        public static int CountBitsInt32(int i)
        {
            const int MASK1  = 0x55555555;
            const int MASK2  = 0x33333333;
            const int MASK4  = 0x0f0f0f0f;
            const int MASK8  = 0x00ff00ff;
            const int MASK16 = 0x0000ffff;

            i = (i & MASK1) + ((i >> 1) & MASK1);
            i = (i & MASK2) + ((i >> 2) & MASK2);
            i = (i & MASK4) + ((i >> 4) & MASK4);
            i = (i & MASK8) + ((i >> 8) & MASK8);
            i = (i & MASK16) + ((i >> 16) & MASK16);

            return i;
        }

        /// <summary>
        /// This is from http://everything2.com/index.pl?node=counting%201%20bits
        /// Expanded to 64 bit.
        /// </summary>
        [CppSkipDefinition(false)] // replaced with compiler intrinsics
        public static int CountBitsInt64(long i)
        {
            const long MASK1  = 0x5555555555555555;
            const long MASK2  = 0x3333333333333333;
            const long MASK4  = 0x0f0f0f0f0f0f0f0f;
            const long MASK8  = 0x00ff00ff00ff00ff;
            const long MASK16 = 0x0000ffff0000ffff;
            const long MASK32 = 0x00000000ffffffff;

            i = (i & MASK1) + ((i >> 1) & MASK1);
            i = (i & MASK2) + ((i >> 2) & MASK2);
            i = (i & MASK4) + ((i >> 4) & MASK4);
            i = (i & MASK8) + ((i >> 8) & MASK8);
            i = (i & MASK16) + ((i >> 16) & MASK16);
            i = (i & MASK32) + ((i >> 32) & MASK32);

            return (int)i;
        }

        public static byte ReverseBits(byte b)
        {
            return (byte)(((b * 0x0802LU & 0x22110LU) | (b * 0x8020LU & 0x88440LU)) * 0x10101LU >> 16);
        }

        /// <summary>
        /// Converts byte to bool array.
        /// </summary>
        public static bool[] ByteToBool(byte input)
        {
            bool[] output = new bool[8];
            for (int i = 0; i < output.Length; i++)
                output[i] = ((input & (1 << i)) != 0);

            return output;
        }

        /// <summary>
        /// Returns number of bits used in the specified value (position of the last '1' bit in value).
        /// Zero value also uses one bit.
        /// </summary>
        public static int BitsUsed(int value)
        {
            return BitsUsedUint32((uint)value);
        }

        /// <summary>
        /// Returns number of bits used in the specified value (position of the last '1' bit in value).
        /// Zero value also uses one bit.
        /// </summary>
        public static int BitsUsedUint32(uint value)
        {
            uint curBit = 0x80000000;
            int bitsUsed = 32;
            while (((value & curBit) == 0) && (bitsUsed > 1))
            {
                curBit >>= 1;
                bitsUsed--;
            }

            return bitsUsed;
        }

        /// <summary>
        /// Sets high-order bits starting from <paramref name="position"/> (zero-based) to 0.
        /// </summary>
        public static int TruncateBits(int value, int position)
        {
            Debug.Assert(position >= 0);
            return value & ((1 << position) - 1);
        }
    }
}
