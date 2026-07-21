// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2017 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestBinaryExpressionUInt
    {
        [Test]
        public void TestSByteBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(uints[i] == sbytes[i], Is.True);
                Assert.That(sbytes[i] == uints[i], Is.True);
            }
            Assert.That(uints[4] != sbytes[4], Is.True);
            Assert.That(sbytes[4] != uints[4], Is.True);
            uint uintValue = (uint)sbytes[4];
            Assert.That(uintValue == sbytes[4], Is.True);
            Assert.That(sbytes[4] == uintValue, Is.True);
            //compare expression
            Assert.That(uintValue > sbytes[0], Is.True);
            Assert.That(uintValue >= sbytes[1], Is.True);
            Assert.That(uintValue < sbytes[2], Is.False);
            Assert.That(uintValue <= sbytes[4], Is.True);
            Assert.That(sbytes[0] > uintValue, Is.False);
            Assert.That(sbytes[1] >= uintValue, Is.False);
            Assert.That(sbytes[2] < uintValue, Is.True);
            Assert.That(sbytes[4] <= uintValue, Is.True);
        }

        [Test]
        public void TestByteBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(uints[i] == bytes[i], Is.True);
                Assert.That(bytes[i] == uints[i], Is.True);
            }
            Assert.That(uints[4] != bytes[4], Is.True);
            Assert.That(bytes[4] != uints[4], Is.True);
            uint uintValue = (uint)bytes[4];
            Assert.That(uintValue == bytes[4], Is.True);
            Assert.That(bytes[4] == uintValue, Is.True);
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
        public void TestShortBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(uints[i] == shorts[i], Is.True);
                Assert.That(shorts[i] == uints[i], Is.True);
            }
            Assert.That(uints[4] != shorts[4], Is.True);
            Assert.That(shorts[4] != uints[4], Is.True);
            uint uintValue = (uint)shorts[4];
            Assert.That(uintValue == shorts[4], Is.True);
            Assert.That(shorts[4] == uintValue, Is.True);
            //compare expression
            Assert.That(uintValue > shorts[0], Is.True);
            Assert.That(uintValue >= shorts[1], Is.True);
            Assert.That(uintValue < shorts[2], Is.False);
            Assert.That(uintValue <= shorts[4], Is.True);
            Assert.That(shorts[0] > uintValue, Is.False);
            Assert.That(shorts[1] >= uintValue, Is.False);
            Assert.That(shorts[2] < uintValue, Is.True);
            Assert.That(shorts[4] <= uintValue, Is.True);
        }

        [Test]
        public void TestUShortBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(uints[i] == ushorts[i], Is.True);
                Assert.That(ushorts[i] == uints[i], Is.True);
            }
            Assert.That(uints[4] != ushorts[4], Is.True);
            Assert.That(ushorts[4] != uints[4], Is.True);
            uint uintValue = (uint)ushorts[4];
            Assert.That(uintValue == ushorts[4], Is.True);
            Assert.That(ushorts[4] == uintValue, Is.True);
            //compare expression
            Assert.That(uintValue > ushorts[0], Is.True);
            Assert.That(uintValue >= ushorts[1], Is.True);
            Assert.That(uintValue < ushorts[2], Is.False);
            Assert.That(uintValue <= ushorts[4], Is.True);
            Assert.That(ushorts[0] > uintValue, Is.False);
            Assert.That(ushorts[1] >= uintValue, Is.False);
            Assert.That(ushorts[2] < uintValue, Is.True);
            Assert.That(ushorts[4] <= uintValue, Is.True);
        }

        [Test]
        public void TestIntBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(uints[i] == ints[i], Is.True);
                Assert.That(ints[i] == uints[i], Is.True);
            }
            Assert.That(uints[4] != ints[4], Is.True);
            Assert.That(ints[4] != uints[4], Is.True);
            uint uintValue = (uint)ints[4];
            Assert.That(uintValue == ints[4], Is.True);
            Assert.That(ints[4] == uintValue, Is.True);
            //compare expression
            Assert.That(uintValue > ints[0], Is.True);
            Assert.That(uintValue >= ints[1], Is.True);
            Assert.That(uintValue < ints[2], Is.False);
            Assert.That(uintValue <= ints[4], Is.True);
            Assert.That(ints[0] > uintValue, Is.False);
            Assert.That(ints[1] >= uintValue, Is.False);
            Assert.That(ints[2] < uintValue, Is.True);
            Assert.That(ints[4] <= uintValue, Is.True);
        }

        [Test]
        public void TestUIntBinaryExpression()
        {
            //equality expression
            uint uintValue = (uint)ints[4];
            Assert.That(uintValue != uints[4], Is.True);
            Assert.That(uints[4] != uintValue, Is.True);
            Assert.That(uintValue == 2147483647, Is.True);
            uintValue += 2;
            Assert.That(uintValue == 2147483649, Is.True);
            for (int i = 0; i < 5; i++)
            {
                uintValue = uints[i];
                Assert.That(uintValue == uints[i], Is.True);
            }
            //compare expression
            Assert.That(uintValue > uints[0], Is.True);
            Assert.That(uintValue >= uints[1], Is.True);
            Assert.That(uintValue < uints[2], Is.False);
            Assert.That(uintValue <= uints[4], Is.True);
            Assert.That(uints[0] > uintValue, Is.False);
            Assert.That(uints[1] >= uintValue, Is.False);
            Assert.That(uints[2] < uintValue, Is.True);
            Assert.That(uints[4] <= uintValue, Is.True);
            //compare uint literal
            Assert.That(uintValue > 2147483649, Is.True);
            Assert.That(uintValue >= 2147483649, Is.True);
            uintValue = (uint)ints[4];
            Assert.That(uintValue < 2147483649, Is.True);
            Assert.That(uintValue <= 2147483649, Is.True);
        }

        [Test]
        public void TestLongBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(uints[i] == longs[i], Is.True);
                Assert.That(longs[i] == uints[i], Is.True);
            }
            Assert.That(uints[4] != longs[4], Is.True);
            Assert.That(longs[4] != uints[4], Is.True);
            long longValue = (long)uints[4];
            Assert.That(longValue == uints[4], Is.True);
            Assert.That(uints[4] == longValue, Is.True);
            //compare expression
            Assert.That(longValue > uints[0], Is.True);
            Assert.That(longValue >= uints[1], Is.True);
            Assert.That(longValue < uints[2], Is.False);
            Assert.That(longValue <= uints[4], Is.True);
            Assert.That(uints[0] > longValue, Is.False);
            Assert.That(uints[1] >= longValue, Is.False);
            Assert.That(uints[2] < longValue, Is.True);
            Assert.That(uints[4] <= longValue, Is.True);
        }

        [Test]
        public void TestFloatBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(uints[i] == floats[i], Is.True);
                Assert.That(floats[i] == uints[i], Is.True);
            }
            //compare expression
            float floatValue = uints[4];
            Assert.That(floatValue > uints[0], Is.True);
            Assert.That(floatValue >= uints[1], Is.True);
            Assert.That(floatValue < uints[2], Is.False);
            Assert.That(floatValue <= uints[4], Is.True);
            Assert.That(uints[0] > floatValue, Is.False);
            Assert.That(uints[1] >= floatValue, Is.False);
            Assert.That(uints[2] < floatValue, Is.True);
            Assert.That(uints[4] <= floatValue, Is.True);
        }

        [Test]
        public void TestDoubleBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(uints[i] == doubles[i], Is.True);
                Assert.That(doubles[i] == uints[i], Is.True);
            }
            //compare expression
            double doubleValue = uints[4];
            Assert.That(doubleValue > uints[0], Is.True);
            Assert.That(doubleValue >= uints[1], Is.True);
            Assert.That(doubleValue < uints[2], Is.False);
            Assert.That(doubleValue <= uints[4], Is.True);
            Assert.That(uints[0] > doubleValue, Is.False);
            Assert.That(uints[1] >= doubleValue, Is.False);
            Assert.That(uints[2] < doubleValue, Is.True);
            Assert.That(uints[4] <= doubleValue, Is.True);
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
