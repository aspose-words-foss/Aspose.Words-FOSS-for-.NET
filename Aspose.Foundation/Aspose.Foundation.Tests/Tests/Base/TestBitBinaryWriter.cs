// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2016 by Ilya Navrotskiy

using System;
using System.IO;
using Aspose.IO;
using NUnit.Framework;

namespace Aspose.Tests.Base
{
    /// <summary>
    /// Tests bit binary writer.
    /// </summary>
    [TestFixture]
    public class TestBitBinaryWriter
    {
        /// <summary>
        /// Tests writing of arbitrary number of bits.
        /// </summary>
        // 1 bit.
        [TestCase(new bool[] { false }, new byte[] { 0x00 }, "1 bit: 0x00.")] 
        [TestCase(new bool[] { true }, new byte[] { 0x80 }, "1 bit: 0x80.")]
        // 5 bits.
        [TestCase(new bool[] { false, true, false, true, false }, new byte[] { 0x50 }, "5 bits: 0x50.")]
        [TestCase(new bool[] { true, false, true, false, true }, new byte[] { 0xA8 }, "5 bits: 0xA8.")]
        // 8 bits.
        [TestCase(new bool[] { false, false, true, true, false, true, false, true }, new byte[] { 0x35 }, "8 bits: 0x35.")]
        [TestCase(new bool[] { true, true, false, false, true, false, true, false }, new byte[] { 0xCA }, "8 bits: 0xCA.")]
        // 9 bits.
        [TestCase(new bool[] { true, true, true, true, false, false, true, false, true }, new byte[] { 0xF2, 0x80 }, "9 bits: 0xF2, 0x80.")]
        [TestCase(new bool[] { true, true, false, false, true, false, true, false, false }, new byte[] { 0xCA, 0x00 }, "9 bits: 0xCA, 0x00.")]
        // 16 bits.
        [TestCase(new bool[] { true, true, true, true, false, false, true, false,
            false, false, true, true, false, false, false, true }, new byte[] { 0xF2, 0x31 }, "16 bits: 0xF2, 0x31.")]
        [TestCase(new bool[] { false, true, true, true, false, false, true, true,
            true, false, true, true, false, false, false, false }, new byte[] { 0x73, 0xB0 }, "16 bits: 0x73, 0xB0.")]
        public void TestWriteArbitraryBits(bool[] data, byte[] expectedResultWithMsbOrdering, string message)
        {
            VerifyResultByteArray(data, expectedResultWithMsbOrdering, message);
        }

        /// <summary>
        /// Tests writing of various bit values. 
        /// </summary>
        [TestCase("0")]
        [TestCase("1")]
        [TestCase("001")]
        [TestCase("000")]
        [TestCase("000000001")]
        [TestCase("000000000")]
        [TestCase("000011001")]
        [TestCase("101010101")]
        [TestCase("1111111101010101")]
        [TestCase("11111111010101011111111111")]
        [TestCase("001111111101010101111111111100")]
        [TestCase("1011111111010101011111111111001")]
        public void TestWriteValue(string value)
        {
            VerifyWriteValue(value);
        }
        
        /// <summary>
        /// Verifies writing value from binary string.
        /// </summary>
        private static void VerifyWriteValue(string bitsValue)
        {
            int expectedValue = Convert.ToInt32(bitsValue, 2);
            VerifyWriteValue(expectedValue, bitsValue.Length);
        }

        /// <summary>
        /// Verifies writing of arbitrary number of bits.
        /// </summary>
        private static void VerifyWriteValue(int expectedValue, int bitsCount)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                // Msb ordering.
                BitBinaryWriter writer = new BitBinaryWriter(stream, true);
                writer.WriteValue(expectedValue, bitsCount);
                writer.Flush();

                stream.Position = 0;
                uint value = ReadValue(stream, bitsCount, true);
                Assert.That(expectedValue, Is.EqualTo(value));

                // Lsb ordering.
                stream.Position = 0;
                writer = new BitBinaryWriter(stream, false);
                writer.WriteValue(expectedValue, bitsCount);
                writer.Flush();

                stream.Position = 0;
                value = ReadValue(stream, bitsCount, false);
                Assert.That(expectedValue, Is.EqualTo(value));
            }
        }

        /// <summary>
        /// Reads original value from the stream.
        /// </summary>
        private static uint ReadValue(Stream stream, int bitsCount, bool isMsb)
        {
            int bytesCount = (bitsCount - 1) / 8 + 1;
            byte[] valueBytes = new byte[4];
            stream.Read(valueBytes, 4 - bytesCount, bytesCount);

            if (!isMsb)
                for (int i = 0; i < bytesCount; i++)
                    valueBytes[3 - i] = BitUtil.ReverseBits(valueBytes[3 - i]);

            uint value = BitConverter.ToUInt32(valueBytes, 0);
            value = BitUtil.SwapUInt32(value);

            int shiftCount = (8 - (bitsCount % 8)) & 0x7;
            value >>= shiftCount;

            return value;
        }

        /// <summary>
        /// Verifies writing of bool array using both MSB and LSB ordering.
        /// </summary>
        private static void VerifyResultByteArray(bool [] data, byte[] expectedResultWithMsbOrdering, string message)
        {
            // Verify MSB byte ordering.
            VerifyMsbResultByteArray(data, expectedResultWithMsbOrdering, message);
            // Verify LSB byte ordering.
            VerifyLsbResultByteArray(data, expectedResultWithMsbOrdering, message);
        }

        /// <summary>
        /// Verifies writing of bool array using MSB order.
        /// </summary>
        private static void VerifyMsbResultByteArray(bool[] data, byte[] expectedResultWithMsbOrdering, string message)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BitBinaryWriter msbWriter = new BitBinaryWriter(stream, true);
                foreach (bool bit in data)
                    msbWriter.WriteBit(bit);
                msbWriter.Flush();

                Assert.That(stream.ToArray(), Is.EqualTo(expectedResultWithMsbOrdering), message);
            }
        }

        /// <summary>
        /// Verifies writing of bool array using LSB order.
        /// </summary>
        private static void VerifyLsbResultByteArray(bool[] data, byte[] expectedResultWithMsbOrdering, string message)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BitBinaryWriter msbWriter = new BitBinaryWriter(stream, false);
                foreach (bool bit in data)
                    msbWriter.WriteBit(bit);
                msbWriter.Flush();

                byte[] expectedResultWithLsbOrdering = GetLsbArray(expectedResultWithMsbOrdering);
                Assert.That(stream.ToArray(), Is.EqualTo(expectedResultWithLsbOrdering), message);
            }
        }

        /// <summary>
        /// Gets LSB array from the specified MSB array.
        /// </summary>
        private static byte[] GetLsbArray(byte[] msbArray)
        {
            byte[] lsbArray = new byte[msbArray.Length];

            for (int i = 0; i < msbArray.Length; i++)
                lsbArray[i] = BitUtil.ReverseBits(msbArray[i]);

            return lsbArray;
        }
    }
}