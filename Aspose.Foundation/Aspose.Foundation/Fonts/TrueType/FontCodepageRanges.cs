// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2016 by Konstantin Kornilov

using System;
using Aspose.Collections;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents codepages supported by the font.
    /// </summary>
    public sealed class FontCodepageRanges
    {
        public FontCodepageRanges(uint range1, uint range2)
        {
            mRange1 = range1;
            mRange2 = range2;
        }

        public FontCodepageRanges(byte[] bytes, int start)
        {
            Debug.Assert(bytes.Length >= start + 2 * 4);

            mRange1 = BitConverter.ToUInt32(bytes, start);
            mRange2 = BitConverter.ToUInt32(bytes, start + 4);
        }

        public uint Range1
        {
            get { return mRange1; }
        }

        public uint Range2
        {
            get { return mRange2; }
        }

        public bool IsSymbolCharsetUsed
        {
            get { return BitUtil.IsSetUInt32(mRange1, Range1Symbol); }
        }

        internal bool IsCjkMetrics
        {
            get { return BitUtil.IsSetUInt32(mRange1, Range1CjkMetricsMask); }
        }

        public bool IsAnsiCodepageUsed
        {
            get { return BitUtil.IsSetUInt32(mRange1, Range1Ansi); }
        }

        public bool IsArabicCodepageUsed
        {
            get { return BitUtil.IsSetUInt32(mRange1, Range1Arabic); }
        }

        public byte GetCharset()
        {
            if (gRange1ToCharset.ContainsKey(mRange1))
                return gRange1ToCharset[mRange1];

            // Default charset.
            return 0x01;
        }

        public FontCodepageRanges AddCharset(byte charset)
        {
            if (!gCharsetToRange1.ContainsKey(charset))
                return this;

            uint charsetBit = gCharsetToRange1[charset];
            return new FontCodepageRanges(mRange1 | charsetBit, mRange2);
        }

        #region Equality members

        private bool Equals(FontCodepageRanges other)
        {
            return (mRange1 == other.mRange1) && (mRange2 == other.mRange2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((FontCodepageRanges)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)mRange1 * 397) ^ (int)mRange2;
            }
        }

        #endregion

        private readonly uint mRange1;
        private readonly uint mRange2;

        static FontCodepageRanges()
        {
            // Charset values are taken from ECMA 376 spec.
            AddCharsetToMap(0x00000001, 0x00); // Default ANSI.
            AddCharsetToMap(0x00000002, 0xEE); // Eastern European
            AddCharsetToMap(0x00000004, 0xCC); // Russian
            AddCharsetToMap(0x00000008, 0xA1); // Greek
            AddCharsetToMap(0x00000010, 0xA2); // Turkish
            AddCharsetToMap(0x00000020, 0xB1); // Hebrew
            AddCharsetToMap(0x00000040, 0xB2); // Arabic
            AddCharsetToMap(0x00000080, 0xBA); // Baltic
            AddCharsetToMap(0x00000100, 0xA3); // Vietnamese
            AddCharsetToMap(0x00010000, 0xDE); // Thai
            AddCharsetToMap(0x00020000, 0x80); // Shift JIS
            AddCharsetToMap(0x00040000, 0x86); // GB2312
            AddCharsetToMap(0x00080000, 0x81); // Hangul
            AddCharsetToMap(0x00100000, 0x88); // Big5
            AddCharsetToMap(0x00200000, 0x82); // Johab
            AddCharsetToMap(0x20000000, 0x4D); // Mac Roman
            AddCharsetToMap(0x40000000, 0xFF); // OEM
            AddCharsetToMap(0x80000000, 0x02); // Symbol
        }

        private static void AddCharsetToMap(uint codepageBit, byte charset)
        {
            gRange1ToCharset.Add(codepageBit, charset);
            gCharsetToRange1.Add(charset, codepageBit);
        }

        /// <summary>
        /// Maps ulCodePageRange1 of OS/2 metrics table to character set.
        /// </summary>
        private static readonly UIntToByteDictionary gRange1ToCharset = new UIntToByteDictionary();
        private static readonly ByteToUIntDictionary gCharsetToRange1 = new ByteToUIntDictionary();

        public static readonly FontCodepageRanges Empty = new FontCodepageRanges(0, 0);
        public const uint Range1Ansi = 0x00000001;
        public const uint Range1Symbol = 0x80000000;
        public const uint Range1Arabic = 0x00000040;

        /// <summary>
        /// Experiments shows that MW performs CJK metrics adjustment according to the presence of following codepages:
        /// Bit 17 - JIS/Japan
        /// Bit 18 - Chinese: Simplified chars—PRC and Singapore
        /// Bit 19 - Korean Wansung
        /// Bit 20 - Chinese: Traditional chars—Taiwan and Hong Kong
        /// </summary>
        private const uint Range1CjkMetricsMask = 0x001E0000;
    }
}
