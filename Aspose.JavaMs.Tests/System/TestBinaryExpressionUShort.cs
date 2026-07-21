// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2017 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestBinaryExpressionUShort
    {
        [Test]
        public void TestSByteBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(ushorts[i] == sbytes[i], Is.True);
                Assert.That(sbytes[i] == ushorts[i], Is.True);
            }
            Assert.That(ushorts[4] != sbytes[4], Is.True);
            Assert.That(sbytes[4] != ushorts[4], Is.True);
            ushort ushortValue = (ushort)sbytes[4];
            Assert.That(ushortValue == sbytes[4], Is.True);
            Assert.That(sbytes[4] == ushortValue, Is.True);
            //compare expression
            Assert.That(ushortValue > sbytes[0], Is.True);
            Assert.That(ushortValue >= sbytes[1], Is.True);
            Assert.That(ushortValue < sbytes[2], Is.False);
            Assert.That(ushortValue <= sbytes[4], Is.True);
            Assert.That(sbytes[0] > ushortValue, Is.False);
            Assert.That(sbytes[1] >= ushortValue, Is.False);
            Assert.That(sbytes[2] < ushortValue, Is.True);
            Assert.That(sbytes[4] <= ushortValue, Is.True);
        }

        [Test]
        public void TestByteBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(ushorts[i] == bytes[i], Is.True);
                Assert.That(bytes[i] == ushorts[i], Is.True);
            }
            Assert.That(ushorts[4] != bytes[4], Is.True);
            Assert.That(bytes[4] != ushorts[4], Is.True);
            ushort ushortValue = (ushort)bytes[4];
            Assert.That(ushortValue == bytes[4], Is.True);
            Assert.That(bytes[4] == ushortValue, Is.True);
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
        public void TestShortBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(ushorts[i] == shorts[i], Is.True);
                Assert.That(shorts[i] == ushorts[i], Is.True);
            }
            Assert.That(ushorts[4] != shorts[4], Is.True);
            Assert.That(shorts[4] != ushorts[4], Is.True);
            ushort ushortValue = (ushort)shorts[4];
            Assert.That(ushortValue == shorts[4], Is.True);
            Assert.That(shorts[4] == ushortValue, Is.True);
            //compare expression
            Assert.That(ushortValue > shorts[0], Is.True);
            Assert.That(ushortValue >= shorts[1], Is.True);
            Assert.That(ushortValue < shorts[2], Is.False);
            Assert.That(ushortValue <= shorts[4], Is.True);
            Assert.That(shorts[0] > ushortValue, Is.False);
            Assert.That(shorts[1] >= ushortValue, Is.False);
            Assert.That(shorts[2] < ushortValue, Is.True);
            Assert.That(shorts[4] <= ushortValue, Is.True);
        }

        [Test]
        public void TestUShortBinaryExpression()
        {
            //equality expression
            ushort ushortValue = (ushort)shorts[4];
            Assert.That(ushortValue != ushorts[4], Is.True);
            Assert.That(ushorts[4] != ushortValue, Is.True);
            Assert.That(ushortValue == 32767, Is.True);
            ushortValue += 2;
            Assert.That(ushortValue == 32769, Is.True);
            for (int i = 0; i < 5; i++)
            {
                ushortValue = ushorts[i];
                Assert.That(ushortValue == ushorts[i], Is.True);
            }
            //compare expression
            Assert.That(ushortValue > ushorts[0], Is.True);
            Assert.That(ushortValue >= ushorts[1], Is.True);
            Assert.That(ushortValue < ushorts[2], Is.False);
            Assert.That(ushortValue <= ushorts[4], Is.True);
            Assert.That(ushorts[0] > ushortValue, Is.False);
            Assert.That(ushorts[1] >= ushortValue, Is.False);
            Assert.That(ushorts[2] < ushortValue, Is.True);
            Assert.That(ushorts[4] <= ushortValue, Is.True);
        }

        [Test]
        public void TestIntBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(ushorts[i] == ints[i], Is.True);
                Assert.That(ints[i] == ushorts[i], Is.True);
            }
            Assert.That(ushorts[4] != ints[4], Is.True);
            Assert.That(ints[4] != ushorts[4], Is.True);
            int intValue = ushorts[4];
            Assert.That(intValue == ushorts[4], Is.True);
            Assert.That(ushorts[4] == intValue, Is.True);
            //compare expression
            Assert.That(intValue > ushorts[0], Is.True);
            Assert.That(intValue >= ushorts[1], Is.True);
            Assert.That(intValue < ushorts[2], Is.False);
            Assert.That(intValue <= ushorts[4], Is.True);
            Assert.That(ushorts[0] > intValue, Is.False);
            Assert.That(ushorts[1] >= intValue, Is.False);
            Assert.That(ushorts[2] < intValue, Is.True);
            Assert.That(ushorts[4] <= intValue, Is.True);
        }

        [Test]
        public void TestUIntBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(ushorts[i] == uints[i], Is.True);
                Assert.That(uints[i] == ushorts[i], Is.True);
            }
            Assert.That(ushorts[4] != uints[4], Is.True);
            Assert.That(uints[4] != ushorts[4], Is.True);
            uint uintValue = ushorts[4];
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
        public void TestLongBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(ushorts[i] == longs[i], Is.True);
                Assert.That(longs[i] == ushorts[i], Is.True);
            }
            Assert.That(ushorts[4] != longs[4], Is.True);
            Assert.That(longs[4] != ushorts[4], Is.True);
            long longValue = ushorts[4];
            Assert.That(longValue == ushorts[4], Is.True);
            Assert.That(ushorts[4] == longValue, Is.True);
            //compare expression
            Assert.That(longValue > ushorts[0], Is.True);
            Assert.That(longValue >= ushorts[1], Is.True);
            Assert.That(longValue < ushorts[2], Is.False);
            Assert.That(longValue <= ushorts[4], Is.True);
            Assert.That(ushorts[0] > longValue, Is.False);
            Assert.That(ushorts[1] >= longValue, Is.False);
            Assert.That(ushorts[2] < longValue, Is.True);
            Assert.That(ushorts[4] <= longValue, Is.True);
        }

        [Test]
        public void TestFloatBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(ushorts[i] == floats[i], Is.True);
                Assert.That(floats[i] == ushorts[i], Is.True);
            }
            //compare expression
            float floatValue = ushorts[4];
            Assert.That(floatValue > ushorts[0], Is.True);
            Assert.That(floatValue >= ushorts[1], Is.True);
            Assert.That(floatValue < ushorts[2], Is.False);
            Assert.That(floatValue <= ushorts[4], Is.True);
            Assert.That(ushorts[0] > floatValue, Is.False);
            Assert.That(ushorts[1] >= floatValue, Is.False);
            Assert.That(ushorts[2] < floatValue, Is.True);
            Assert.That(ushorts[4] <= floatValue, Is.True);
        }

        [Test]
        public void TestDoubleBinaryExpression()
        {
            //equality expression
            for (int i = 0; i < 4; i++)
            {
                Assert.That(ushorts[i] == doubles[i], Is.True);
                Assert.That(doubles[i] == ushorts[i], Is.True);
            }
            //compare expression
            double doubleValue = ushorts[4];
            Assert.That(doubleValue > ushorts[0], Is.True);
            Assert.That(doubleValue >= ushorts[1], Is.True);
            Assert.That(doubleValue < ushorts[2], Is.False);
            Assert.That(doubleValue <= ushorts[4], Is.True);
            Assert.That(ushorts[0] > doubleValue, Is.False);
            Assert.That(ushorts[1] >= doubleValue, Is.False);
            Assert.That(ushorts[2] < doubleValue, Is.True);
            Assert.That(ushorts[4] <= doubleValue, Is.True);
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
