// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2017 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestBinaryExpressionByte
    {
        [Test]
        public void TestSByteBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(bytes[i] == sbytes[i], Is.True);
                Assert.That(sbytes[i] == bytes[i], Is.True);
            }
            Assert.That(bytes[4] != sbytes[4], Is.True);
            Assert.That(sbytes[4] != bytes[4], Is.True);
            byte byteValue = (byte)sbytes[4];
            Assert.That(byteValue == sbytes[4], Is.True);
            Assert.That(sbytes[4] == byteValue, Is.True);
            //compare expression
            Assert.That(byteValue > sbytes[0], Is.True);
            Assert.That(byteValue >= sbytes[1], Is.True);
            Assert.That(byteValue < sbytes[2], Is.False);
            Assert.That(byteValue <= sbytes[4], Is.True);
            Assert.That(sbytes[0] > byteValue, Is.False);
            Assert.That(sbytes[1] >= byteValue, Is.False);
            Assert.That(sbytes[2] < byteValue, Is.True);
            Assert.That(sbytes[4] <= byteValue, Is.True);
        }

        [Test]
        public void TestByteBinaryExpression()
        {
            //equality expression
            byte byteValue = (byte)sbytes[4];
            Assert.That(byteValue != bytes[4], Is.True);
            Assert.That(bytes[4] != byteValue, Is.True);
            Assert.That(byteValue == 127, Is.True);
            byteValue += 2;
            Assert.That(byteValue == 129, Is.True);
            for (int i = 0; i < 5; i++)
            {
                byteValue = bytes[i];
                Assert.That(byteValue == bytes[i], Is.True);
            }
            //compare expression
            Assert.That(byteValue > bytes[0], Is.True);
            Assert.That(byteValue >= bytes[1], Is.True);
            Assert.That(byteValue < bytes[2], Is.False);
            Assert.That(byteValue <= bytes[4], Is.True);
            Assert.That(bytes[0] > byteValue, Is.False);
            Assert.That(bytes[1] >= byteValue, Is.False);
            Assert.That(bytes[2] < byteValue, Is.True);
            Assert.That(bytes[4] <= byteValue, Is.True);
        }

        [Test]
        public void TestShortBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(bytes[i] == shorts[i], Is.True);
                Assert.That(shorts[i] == bytes[i], Is.True);
            }
            Assert.That(bytes[4] != shorts[4], Is.True);
            Assert.That(shorts[4] != bytes[4], Is.True);
            short shortValue = (short)bytes[4];
            Assert.That(bytes[4] == shortValue, Is.True);
            Assert.That(shortValue == bytes[4], Is.True);
            //compare expression
            Assert.That(shortValue > bytes[0], Is.True);
            Assert.That(shortValue >= bytes[1], Is.True);
            Assert.That(shortValue < bytes[2], Is.False);
            Assert.That(shortValue <= bytes[4], Is.True);
            Assert.That(bytes[0] > shortValue, Is.False);
            Assert.That(bytes[1] >= shortValue, Is.False);
            Assert.That(bytes[2] < shortValue, Is.True);
            Assert.That(bytes[4] <= shortValue, Is.True);
        }

        [Test]
        public void TestUShortBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(bytes[i] == ushorts[i], Is.True);
                Assert.That(ushorts[i] == bytes[i], Is.True);
            }
            Assert.That(bytes[4] != ushorts[4], Is.True);
            Assert.That(ushorts[4] != bytes[4], Is.True);
            ushort ushortValue = (ushort)bytes[4];
            Assert.That(bytes[4] == ushortValue, Is.True);
            Assert.That(ushortValue == bytes[4], Is.True);
            //compare expression
            Assert.That(ushortValue > bytes[0], Is.True);
            Assert.That(ushortValue >= bytes[1], Is.True);
            Assert.That(ushortValue < bytes[2], Is.False);
            Assert.That(ushortValue <= bytes[4], Is.True);
            Assert.That(bytes[0] > ushortValue, Is.False);
            Assert.That(bytes[1] >= ushortValue, Is.False);
            Assert.That(bytes[2] < ushortValue, Is.True);
            Assert.That(bytes[4] <= ushortValue, Is.True);
        }

        [Test]
        public void TestIntBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(bytes[i] == ints[i], Is.True);
                Assert.That(ints[i] == bytes[i], Is.True);
            }
            Assert.That(bytes[4] != ints[4], Is.True);
            Assert.That(ints[4] != bytes[4], Is.True);
            int intValue = (int)bytes[4];
            Assert.That(bytes[4] == intValue, Is.True);
            Assert.That(intValue == bytes[4], Is.True);
            //compare expression
            Assert.That(intValue > bytes[0], Is.True);
            Assert.That(intValue >= bytes[1], Is.True);
            Assert.That(intValue < bytes[2], Is.False);
            Assert.That(intValue <= bytes[4], Is.True);
            Assert.That(bytes[0] > intValue, Is.False);
            Assert.That(bytes[1] >= intValue, Is.False);
            Assert.That(bytes[2] < intValue, Is.True);
            Assert.That(bytes[4] <= intValue, Is.True);
        }

        [Test]
        public void TestUIntBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(bytes[i] == uints[i], Is.True);
                Assert.That(uints[i] == bytes[i], Is.True);
            }
            Assert.That(bytes[4] != uints[4], Is.True);
            Assert.That(uints[4] != bytes[4], Is.True);
            uint uintValue = (uint)bytes[4];
            Assert.That(bytes[4] == uintValue, Is.True);
            Assert.That(uintValue == bytes[4], Is.True);
            //compare expression
            Assert.That(uintValue > bytes[0], Is.True);
            Assert.That(uintValue >= bytes[1], Is.True);
            Assert.That(uintValue < bytes[2], Is.False);
            Assert.That(uintValue <= bytes[4], Is.True);
            Assert.That(bytes[0] > uintValue, Is.False);
            Assert.That(bytes[1] >= uintValue, Is.False);
            Assert.That(bytes[2] < uintValue, Is.True);
            Assert.That(bytes[4] <= uintValue, Is.True);
        }

        [Test]
        public void TestLongBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(bytes[i] == longs[i], Is.True);
                Assert.That(longs[i] == bytes[i], Is.True);
            }
            Assert.That(bytes[4] != longs[4], Is.True);
            Assert.That(longs[4] != bytes[4], Is.True);
            long longValue = (long)bytes[4];
            Assert.That(bytes[4] == longValue, Is.True);
            Assert.That(longValue == bytes[4], Is.True);
            //compare expression
            Assert.That(longValue > bytes[0], Is.True);
            Assert.That(longValue >= bytes[1], Is.True);
            Assert.That(longValue < bytes[2], Is.False);
            Assert.That(longValue <= bytes[4], Is.True);
            Assert.That(bytes[0] > longValue, Is.False);
            Assert.That(bytes[1] >= longValue, Is.False);
            Assert.That(bytes[2] < longValue, Is.True);
            Assert.That(bytes[4] <= longValue, Is.True);
        }

        [Test]
        public void TestFloatBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(bytes[i] == floats[i], Is.True);
                Assert.That(floats[i] == bytes[i], Is.True);
            }
            //compare expression
            float floatValue = bytes[4];
            Assert.That(floatValue > bytes[0], Is.True);
            Assert.That(floatValue >= bytes[1], Is.True);
            Assert.That(floatValue < bytes[2], Is.False);
            Assert.That(floatValue <= bytes[4], Is.True);
            Assert.That(bytes[0] > floatValue, Is.False);
            Assert.That(bytes[1] >= floatValue, Is.False);
            Assert.That(bytes[2] < floatValue, Is.True);
            Assert.That(bytes[4] <= floatValue, Is.True);
        }

        [Test]
        public void TestDoubleBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(bytes[i] == doubles[i], Is.True);
                Assert.That(doubles[i] == bytes[i], Is.True);
            }
            //compare expression
            double doubleValue = bytes[4];
            Assert.That(doubleValue > bytes[0], Is.True);
            Assert.That(doubleValue >= bytes[1], Is.True);
            Assert.That(doubleValue < bytes[2], Is.False);
            Assert.That(doubleValue <= bytes[4], Is.True);
            Assert.That(bytes[0] > doubleValue, Is.False);
            Assert.That(bytes[1] >= doubleValue, Is.False);
            Assert.That(bytes[2] < doubleValue, Is.True);
            Assert.That(bytes[4] <= doubleValue, Is.True);
        }

        private readonly sbyte[] sbytes = { 1, 2, 3, 50, SByte.MaxValue };
        private readonly byte[] bytes = { 1, 2, 3, 50, Byte.MaxValue };
        private readonly short[] shorts = { 1, 2, 3, 50, Int16.MaxValue };
        private readonly ushort[] ushorts = { 1, 2, 3, 50, UInt16.MaxValue };
        private readonly int[] ints = { 1, 2, 3, 50, Int32.MaxValue };
        private readonly uint[] uints = { 1, 2, 3, 50, UInt32.MaxValue };
        private readonly long[] longs = { 1L, 2L, 3L, 50, Int64.MaxValue };
        private readonly float[] floats = { 1.0F, 2.0F, 3.0F, 50 };
        private readonly double[] doubles = { 1.0, 2.0, 3.0, 50 };
    }
}
