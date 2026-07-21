// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/08/2013 by Roman Korchagin

using NUnit.Framework;

namespace Aspose.Tests.Base
{
    [TestFixture]
    public class TestBitUtil
    {
        [TestCase((long)0x0807060504030201, (long)0x0102030405060708)]
        [TestCase((long)-9007199254740992000, (long)0x0000000000000083)] //-9,007,199,254,740,992,000 == 0x8300000000000000
        [TestCase((long)0x0000000000000083, (long)-9007199254740992000)]
        public void TestSwapInt64(long expectedValue, long value)
        {
            Assert.That(BitUtil.SwapInt64(value), Is.EqualTo(expectedValue));
        }

        [TestCase((uint)0x04030201, (uint)0x01020304)]
        [TestCase((uint)0xf4030201, (uint)0x010203f4)]
        [TestCase((uint)0x040302f1, (uint)0xf1020304)]
        public void TestSwapUInt32(uint expectedValue, uint value)
        {
            Assert.That(BitUtil.SwapUInt32(value), Is.EqualTo(expectedValue));
        }

        [TestCase((int)0x04030201, (int)0x01020304)]
        [TestCase((int)-2097152000, (int)0x00000083)] //-2,097,152,000 == 0x83000000
        [TestCase((int)0x00000083, (int)-2097152000)]
        public void TestSwapInt32(int expectedValue, int value)
        {
            Assert.That(BitUtil.SwapInt32(value), Is.EqualTo(expectedValue));
        }

        [TestCase((ushort)0x0201, (ushort)0x0102)]
        [TestCase((ushort)0xf201, (ushort)0x01f2)]
        [TestCase((ushort)0x02f1, (ushort)0xf102)]
        public void TestSwapUInt16(ushort expectedValue, ushort value)
        {
            Assert.That(BitUtil.SwapUInt16(value), Is.EqualTo(expectedValue));
        }

        [TestCase((short)0x0201, (short)0x0102)]
        [TestCase((short)-32000, (short)0x0083)] //-32,000 == 0x8300
        [TestCase((short)0x0083, (short)-32000)]
        public void TestSwapInt16(short expectedValue, short value)
        {
            Assert.That(BitUtil.SwapInt16(value), Is.EqualTo(expectedValue));
        }

        [TestCase(4, 0x0000000f)]
        [TestCase(11, 0x1050e20f)]
        [TestCase(32, unchecked((int)0xffffffff))]
        [TestCase(0, 0x0000000000000000)]
        public void TestCountBitsInt32(int expectedCount, int value)
        {
            Assert.That(BitUtil.CountBitsInt32(value), Is.EqualTo(expectedCount));
        }

        [TestCase(44, unchecked((long)0xf0f1fff23fec07ff))]
        [TestCase(3, 0x0010000080000200)]
        public void TestCountBitsInt64(int expectedCount, long value)
        {
            Assert.That(BitUtil.CountBitsInt64(value), Is.EqualTo(expectedCount));
        }

        [Test]
        public void TestReverseBits()
        {
            byte b = 0xC1;
            byte result = BitUtil.ReverseBits(b);
            Assert.That(result, Is.EqualTo(0x83));
        }

        /// <summary>
        /// Checks number of bits in various values.
        /// </summary>
        [TestCase(1, 0x0)]
        [TestCase(1, 0x1)]
        [TestCase(2, 0x2)]
        [TestCase(3, 0x4)]
        [TestCase(4, 0x8)]
        [TestCase(5, 0x10)]
        [TestCase(6, 0x20)]
        [TestCase(7, 0x40)]
        [TestCase(8, 0x80)]
        [TestCase(10, 0x200)]
        [TestCase(12, 0x800)]
        [TestCase(14, 0x2000)]
        [TestCase(16, 0x8000)]
        [TestCase(24, 0x800000)]
        [TestCase(29, 0x10000000)]
        [TestCase(31, 0x40000000)]
        [TestCase(31, 0x7fffffff)]
        [TestCase(32, unchecked((int)0xffffffff))]
        [TestCase(32, -500)]
        public void TestBitsUsed(int expectedCount, int value)
        {
            Assert.That(BitUtil.BitsUsed(value), Is.EqualTo(expectedCount));
        }

        [TestCase(85, 0, 0)]   // 1010101 -> 0000000
        [TestCase(85, 1, 1)]   // 1010101 -> 0000001
        [TestCase(85, 3, 5)]   // 1010101 -> 0000101
        [TestCase(85, 5, 21)]  // 1010101 -> 0010101
        [TestCase(85, 7, 85)]  // 1010101 -> 1010101
        [TestCase(63, 0, 0)]   // 111111 -> 000000
        [TestCase(63, 1, 1)]   // 111111 -> 000001
        [TestCase(63, 3, 7)]   // 111111 -> 000111
        [TestCase(63, 5, 31)]  // 111111 -> 011111
        [TestCase(63, 7, 63)]  // 111111 -> 111111
        [TestCase(9, 0, 0)]    // 1001 -> 0000
        [TestCase(9, 1, 1)]    // 1001 -> 0001
        [TestCase(9, 3, 1)]    // 1001 -> 0001
        [TestCase(9, 5, 9)]    // 1001 -> 1001
        [TestCase(9, 7, 9)]    // 1001 -> 1001
        public void TestTruncateBits(int value, int position, int expectedResult)
        {
            Assert.That(BitUtil.TruncateBits(value, position), Is.EqualTo(expectedResult));
        }
    }
}
