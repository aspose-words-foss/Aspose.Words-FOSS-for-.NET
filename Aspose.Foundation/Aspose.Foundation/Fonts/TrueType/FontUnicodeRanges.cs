// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2016 by Konstantin Kornilov

using System;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents Unicode ranges supported by the font.
    /// </summary>
    /// <remarks>
    /// See http://www.microsoft.com/typography/otspec/os2.htm#ur for more info.
    /// </remarks>
    public sealed class FontUnicodeRanges
    {
        public FontUnicodeRanges(uint range1, uint range2, uint range3, uint range4)
        {
            mRange1 = range1;
            mRange2 = range2;
            mRange3 = range3;
            mRange4 = range4;
        }

        public FontUnicodeRanges(byte[] bytes, int start)
        {
            Debug.Assert(bytes.Length >= start + 4 * 4);

            mRange1 = BitConverter.ToUInt32(bytes, start);
            mRange2 = BitConverter.ToUInt32(bytes, start + 4);
            mRange3 = BitConverter.ToUInt32(bytes, start + 8);
            mRange4 = BitConverter.ToUInt32(bytes, start + 12);
        }

        public uint Range1
        {
            get { return mRange1; }
        }

        public uint Range2
        {
            get { return mRange2; }
        }

        public uint Range3
        {
            get { return mRange3; }
        }

        public uint Range4
        {
            get { return mRange4; }
        }



        /// <summary>
        /// Indicates whether the Unicode range for the specified character is considered functional in the font.
        /// </summary>
        public bool IsCharInFunctionalRange(int character)
        {
            int bitIndex = FindBitForUnicodeRange(character);

            return IsRangeFunctional(bitIndex);
        }

        /// <summary>
        /// Indicates whether the specified Unicode range is considered functional in the font.
        /// </summary>
        public bool IsRangeFunctional(int rangeBitIndex)
        {
            if (rangeBitIndex == -1)
                return false;

            uint rangeToCheck;
            if (rangeBitIndex < 32)
            {
                rangeToCheck = mRange1;
            }
            else if (rangeBitIndex < 64)
            {
                rangeBitIndex = rangeBitIndex - 32;
                rangeToCheck = mRange2;
            }
            else if (rangeBitIndex < 96)
            {
                rangeBitIndex = rangeBitIndex - 64;
                rangeToCheck = mRange3;
            }
            else
            {
                rangeBitIndex = rangeBitIndex - 96;
                rangeToCheck = mRange4;
            }

            uint mask = (uint)1 << rangeBitIndex;

            return (rangeToCheck & mask) == mask;
        }

        /// <summary>
        /// Indicates whether the font supports complex script.
        /// </summary>
        public bool IsComplexScriptSupported()
        {
            // Check if the font supports Hebrew (11th bit) or Arabic (13th bit) Unicode ranges.
            // https://docs.microsoft.com/en-us/typography/opentype/spec/os2#ur
            return IsRangeFunctional(11) || IsRangeFunctional(13);
        }

        public bool IsKoreanOrJapanese()
        {
            return IsRangeFunctional(56) || // Korean
                IsRangeFunctional(49); // Japanese

        }

        private static int FindBitForUnicodeRange(int value)
        {
            int res = ArrayUtil.BinarySearch(RangeCodes, 0, RangeCodes.Length, value);

            if (res >= 0)
            {
                int bitIndex = res / 2;
                return RangeBitPositions[bitIndex];
            }

            if (res < 0)
            {
                int index = ~res;
                if (index < RangeCodes.Length)
                {
                    int bitIndex = index / 2;
                    bool isEven = index % 2 == 0;
                    bool isInHoleBetweenRanges = isEven && index > 0 && (RangeCodes[index] - RangeCodes[index - 1] > 1);

                    if (!isInHoleBetweenRanges)
                        return RangeBitPositions[bitIndex];
                }
            }

            return -1;
        }

        #region Equality members

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        private bool Equals(FontUnicodeRanges other)
        {
            return (mRange1 == other.mRange1) && (mRange2 == other.mRange2) &&
                   (mRange3 == other.mRange3) && (mRange4 == other.mRange4);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((FontUnicodeRanges)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)mRange1;
                hashCode = (hashCode * 397) ^ (int)mRange2;
                hashCode = (hashCode * 397) ^ (int)mRange3;
                hashCode = (hashCode * 397) ^ (int)mRange4;
                return hashCode;
            }
        }

        #endregion

        private readonly uint mRange1;
        private readonly uint mRange2;
        private readonly uint mRange3;
        private readonly uint mRange4;

        public static readonly FontUnicodeRanges Empty = new FontUnicodeRanges(0, 0, 0, 0);

        /// <summary>
        /// Array of Unicode ranges start/end codes.
        /// Even indices are range starts. Odd indices are range ends.
        /// </summary>
        internal static readonly int[] RangeCodes = new int[] {
                0x0000, 0x007F, 0x0080, 0x00FF, 0x0100, 0x017F, 0x0180, 0x024F,
                0x0250, 0x02AF, 0x02B0, 0x02FF, 0x0300, 0x036F, 0x0370, 0x03FF,
                0x0400, 0x04FF, 0x0500, 0x052F, 0x0530, 0x058F, 0x0590, 0x05FF,
                0x0600, 0x06FF, 0x0700, 0x074F, 0x0750, 0x077F, 0x0780, 0x07BF,
                0x07C0, 0x07FF, 0x0900, 0x097F, 0x0980, 0x09FF, 0x0A00, 0x0A7F,
                0x0A80, 0x0AFF, 0x0B00, 0x0B7F, 0x0B80, 0x0BFF, 0x0C00, 0x0C7F,
                0x0C80, 0x0CFF, 0x0D00, 0x0D7F, 0x0D80, 0x0DFF, 0x0E00, 0x0E7F,
                0x0E80, 0x0EFF, 0x0F00, 0x0FFF, 0x1000, 0x109F, 0x10A0, 0x10FF,
                0x1100, 0x11FF, 0x1200, 0x137F, 0x1380, 0x139F, 0x13A0, 0x13FF,
                0x1400, 0x167F, 0x1680, 0x169F, 0x16A0, 0x16FF, 0x1700, 0x171F,
                0x1720, 0x173F, 0x1740, 0x175F, 0x1760, 0x177F, 0x1780, 0x17FF,
                0x1800, 0x18AF, 0x1900, 0x194F, 0x1950, 0x197F, 0x1980, 0x19DF,
                0x19E0, 0x19FF, 0x1A00, 0x1A1F, 0x1B00, 0x1B7F, 0x1B80, 0x1BBF,
                0x1C00, 0x1C4F, 0x1C50, 0x1C7F, 0x1D00, 0x1D7F, 0x1D80, 0x1DBF,
                0x1DC0, 0x1DFF, 0x1E00, 0x1EFF, 0x1F00, 0x1FFF, 0x2000, 0x206F,
                0x2070, 0x209F, 0x20A0, 0x20CF, 0x20D0, 0x20FF, 0x2100, 0x214F,
                0x2150, 0x218F, 0x2190, 0x21FF, 0x2200, 0x22FF, 0x2300, 0x23FF,
                0x2400, 0x243F, 0x2440, 0x245F, 0x2460, 0x24FF, 0x2500, 0x257F,
                0x2580, 0x259F, 0x25A0, 0x25FF, 0x2600, 0x26FF, 0x2700, 0x27BF,
                0x27C0, 0x27EF, 0x27F0, 0x27FF, 0x2800, 0x28FF, 0x2900, 0x297F,
                0x2980, 0x29FF, 0x2A00, 0x2AFF, 0x2B00, 0x2BFF, 0x2C00, 0x2C5F,
                0x2C60, 0x2C7F, 0x2C80, 0x2CFF, 0x2D00, 0x2D2F, 0x2D30, 0x2D7F,
                0x2D80, 0x2DDF, 0x2DE0, 0x2DFF, 0x2E00, 0x2E7F, 0x2E80, 0x2EFF,
                0x2F00, 0x2FDF, 0x2FF0, 0x2FFF, 0x3000, 0x303F, 0x3040, 0x309F,
                0x30A0, 0x30FF, 0x3100, 0x312F, 0x3130, 0x318F, 0x3190, 0x319F,
                0x31A0, 0x31BF, 0x31C0, 0x31EF, 0x31F0, 0x31FF, 0x3200, 0x32FF,
                0x3300, 0x33FF, 0x3400, 0x4DBF, 0x4DC0, 0x4DFF, 0x4E00, 0x9FFF,
                0xA000, 0xA48F, 0xA490, 0xA4CF, 0xA500, 0xA63F, 0xA640, 0xA69F,
                0xA700, 0xA71F, 0xA720, 0xA7FF, 0xA800, 0xA82F, 0xA840, 0xA87F,
                0xA880, 0xA8DF, 0xA900, 0xA92F, 0xA930, 0xA95F, 0xAA00, 0xAA5F,
                0xAC00, 0xD7AF, 0xD800, 0xDFFF, 0xE000, 0xF8FF, 0xF900, 0xFAFF,
                0xFB00, 0xFB4F, 0xFB50, 0xFDFF, 0xFE00, 0xFE0F, 0xFE10, 0xFE1F,
                0xFE20, 0xFE2F, 0xFE30, 0xFE4F, 0xFE50, 0xFE6F, 0xFE70, 0xFEFF,
                0xFF00, 0xFFEF, 0xFFF0, 0xFFFF, 0x10000, 0x1007F, 0x10080, 0x100FF,
                0x10100, 0x1013F, 0x10140, 0x1018F, 0x10190, 0x101CF, 0x101D0, 0x101FF,
                0x10280, 0x1029F, 0x102A0, 0x102DF, 0x10300, 0x1032F, 0x10330, 0x1034F,
                0x10380, 0x1039F, 0x103A0, 0x103DF, 0x10400, 0x1044F, 0x10450, 0x1047F,
                0x10480, 0x104AF, 0x10800, 0x1083F, 0x10900, 0x1091F, 0x10920, 0x1093F,
                0x10A00, 0x10A5F, 0x12000, 0x123FF, 0x12400, 0x1247F, 0x1D000, 0x1D0FF,
                0x1D100, 0x1D1FF, 0x1D200, 0x1D24F, 0x1D300, 0x1D35F, 0x1D360, 0x1D37F,
                0x1D400, 0x1D7FF, 0x1F000, 0x1F02F, 0x1F030, 0x1F09F, 0x20000, 0x2A6DF,
                0x2F800, 0x2FA1F, 0xE0000, 0xE007F, 0xE0100, 0xE01EF, 0xFF000, 0xFFFFD,
                0x100000, 0x10FFFD
        };

        /// <summary>
        /// Bit positions for Unicode ranges.
        /// The bit positions are aligned with the ranges specified in <see cref="RangeCodes"/>.
        /// <see cref="FindBitForUnicodeRange"/> is used to find the bit position for the Unicode range.
        /// </summary>
        internal static readonly int[] RangeBitPositions = new int[] {
                0, 1, 2, 3, 4, 5, 6, 7, 9, 9, 10, 11, 13, 71, 13, 72, 14, 15, 16,
                17, 18, 19, 20, 21, 22, 23, 73, 24, 25, 70, 74, 26, 28, 75, 75, 76,
                77, 78, 79, 84, 84, 84, 84, 80, 81, 93, 94, 95, 80, 96, 27, 112, 113,
                114, 4, 4, 6, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42,
                43, 44, 45, 46, 47, 38, 37, 82, 37, 38, 38, 37, 97, 29, 8, 26, 98, 75,
                9, 31, 59, 59, 59, 48, 49, 50, 51, 52, 59, 51, 61, 50, 54, 55, 59, 99,
                59, 83, 83, 12, 9, 5, 29, 100, 53, 115, 116, 117, 118, 56, 57, 60, 61,
                62, 63, 91, 65, 64, 65, 66, 67, 68, 69, 101, 101, 101, 102, 119, 120,
                121, 121, 85, 86, 103, 104, 87, 105, 106, 107, 58, 121, 108, 110, 110,
                88, 88, 88, 109, 111, 89, 122, 122, 59, 61, 92, 91, 90, 90
        };
    }
}
