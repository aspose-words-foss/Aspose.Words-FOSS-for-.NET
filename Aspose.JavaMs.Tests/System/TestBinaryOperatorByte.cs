// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/03/2017 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

#pragma warning disable CS0675 // Java-specific tests.

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestBinaryOperatorByte
    {
        [Test]
        public void TestSByteBinaryOperator()
        {
            //arithmetic addition
            byte byteValue = (byte)(sbytes[4] / 2);
            Assert.That(byteValue + sbytes[1], Is.EqualTo(65));
            Assert.That(byteValue - sbytes[2], Is.EqualTo(60));
            //arithmetic scaling
            Assert.That(byteValue * sbytes[1], Is.EqualTo(126));
            Assert.That(byteValue % sbytes[3], Is.EqualTo(13));
            Assert.That(byteValue / sbytes[1], Is.EqualTo(31));
            //bitwise
            Assert.That(byteValue & sbytes[1], Is.EqualTo(2));
            Assert.That(byteValue | sbytes[1], Is.EqualTo(63));
            Assert.That(byteValue ^ sbytes[3], Is.EqualTo(13));
            //bitwise shift
            Assert.That(byteValue >> sbytes[1], Is.EqualTo(15));
            Assert.That(byteValue << sbytes[1], Is.EqualTo(252));
        }

        [Test]
        public void TestByteBinaryOperator()
        {
            //arithmetic addition
            byte byteValue = (byte)(bytes[4] / 2);
            Assert.That(byteValue + bytes[1], Is.EqualTo(129));
            Assert.That(byteValue - bytes[2], Is.EqualTo(124));
            //arithmetic scaling
            Assert.That(byteValue * bytes[1], Is.EqualTo(254));
            Assert.That(byteValue % bytes[3], Is.EqualTo(27));
            Assert.That(byteValue / bytes[1], Is.EqualTo(63));
            //bitwise
            Assert.That(byteValue & bytes[1], Is.EqualTo(2));
            Assert.That(byteValue | bytes[1], Is.EqualTo(127));
            Assert.That(byteValue ^ bytes[3], Is.EqualTo(77));
            //bitwise shift
            Assert.That(byteValue >> bytes[1], Is.EqualTo(31));
            Assert.That(byteValue << bytes[1], Is.EqualTo(508));
        }

        [Test]
        public void TestShortBinaryOperator()
        {
            //arithmetic addition
            byte byteValue = (byte)(shorts[4] / 2);
            Assert.That(byteValue + shorts[1], Is.EqualTo(257));
            Assert.That(byteValue - shorts[2], Is.EqualTo(252));
            //arithmetic scaling
            Assert.That(byteValue * shorts[1], Is.EqualTo(510));
            Assert.That(byteValue % shorts[3], Is.EqualTo(5));
            Assert.That(byteValue / shorts[1], Is.EqualTo(127));
            //bitwise
            Assert.That(byteValue & shorts[1], Is.EqualTo(2));
            Assert.That(byteValue | shorts[1], Is.EqualTo(255));
            Assert.That(byteValue ^ shorts[3], Is.EqualTo(205));
            //bitwise shift
            Assert.That(byteValue >> shorts[1], Is.EqualTo(63));
            Assert.That(byteValue << shorts[1], Is.EqualTo(1020));
        }

        [Test]
        public void TestUShortBinaryOperator()
        {
            //arithmetic addition
            byte byteValue = (byte)(ushorts[4] / 2);
            Assert.That(byteValue + ushorts[1], Is.EqualTo(257));
            Assert.That(byteValue - ushorts[2], Is.EqualTo(252));
            //arithmetic scaling
            Assert.That(byteValue * ushorts[1], Is.EqualTo(510));
            Assert.That(byteValue % ushorts[3], Is.EqualTo(5));
            Assert.That(byteValue / ushorts[1], Is.EqualTo(127));
            //bitwise
            Assert.That(byteValue & ushorts[1], Is.EqualTo(2));
            Assert.That(byteValue | ushorts[1], Is.EqualTo(255));
            Assert.That(byteValue ^ ushorts[3], Is.EqualTo(205));
            //bitwise shift
            Assert.That(byteValue >> ushorts[1], Is.EqualTo(63));
            Assert.That(byteValue << ushorts[1], Is.EqualTo(1020));
        }

        [Test]
        public void TestIntBinaryOperator()
        {
            //arithmetic addition
            byte byteValue = (byte)(ints[4] / 2);
            Assert.That(byteValue + ints[1], Is.EqualTo(257));
            Assert.That(byteValue - ints[2], Is.EqualTo(252));
            //arithmetic scaling
            Assert.That(byteValue * ints[1], Is.EqualTo(510));
            Assert.That(byteValue % ints[3], Is.EqualTo(5));
            Assert.That(byteValue / ints[1], Is.EqualTo(127));
            //bitwise
            Assert.That(byteValue & ints[1], Is.EqualTo(2));
            Assert.That(byteValue | ints[1], Is.EqualTo(255));
            Assert.That(byteValue ^ ints[3], Is.EqualTo(205));
            //bitwise shift
            Assert.That(byteValue >> ints[1], Is.EqualTo(63));
            Assert.That(byteValue << ints[1], Is.EqualTo(1020));
        }

        [Test]
        public void TestUIntBinaryOperator()
        {
            //arithmetic addition
            byte byteValue = (byte)(uints[4] / 2);
            Assert.That(byteValue + uints[1], Is.EqualTo(257));
            Assert.That(byteValue - uints[2], Is.EqualTo(252L));
            //arithmetic scaling
            Assert.That(byteValue * uints[1], Is.EqualTo(510));
            Assert.That(byteValue % uints[3], Is.EqualTo(5L));
            Assert.That(byteValue / uints[1], Is.EqualTo(127L));
            //bitwise
            Assert.That(byteValue & uints[1], Is.EqualTo(2));
            Assert.That(byteValue | uints[1], Is.EqualTo(255L));
            Assert.That(byteValue ^ uints[3], Is.EqualTo(205L));
        }

        [Test]
        public void TestLongBinaryOperator()
        {
            //arithmetic addition
            byte byteValue = (byte)(longs[4] / 2);
            Assert.That(byteValue + longs[1], Is.EqualTo(257));
            Assert.That(byteValue - longs[2], Is.EqualTo(252));
            //arithmetic scaling
            Assert.That(byteValue * longs[1], Is.EqualTo(510));
            Assert.That(byteValue % longs[3], Is.EqualTo(5));
            Assert.That(byteValue / longs[1], Is.EqualTo(127));
            //bitwise
            Assert.That(byteValue & longs[1], Is.EqualTo(2));
            Assert.That(byteValue | longs[1], Is.EqualTo(255));
            Assert.That(byteValue ^ longs[3], Is.EqualTo(205));
        }

        [Test]
        public void TestFloatBinaryOperator()
        {
            //arithmetic addition
            byte byteValue = (byte)(SByte.MaxValue);
            Assert.That(byteValue + floats[1], Is.EqualTo(129.0f));
            Assert.That(byteValue - floats[2], Is.EqualTo(124.0f));
            //arithmetic scaling
            Assert.That(byteValue * floats[1], Is.EqualTo(254.0f));
            Assert.That(byteValue % floats[3], Is.EqualTo(27.0f));
            Assert.That(byteValue / floats[1], Is.EqualTo(63.5f));
        }

        [Test]
        public void TestDoubleBinaryOperator()
        {
            //arithmetic addition
            byte byteValue = (byte)(SByte.MaxValue);
            Assert.That(byteValue + doubles[1], Is.EqualTo(129.0d));
            Assert.That(byteValue - doubles[2], Is.EqualTo(124.0d));
            //arithmetic scaling
            Assert.That(byteValue * doubles[1], Is.EqualTo(254.0d));
            Assert.That(byteValue % doubles[3], Is.EqualTo(27.0d));
            Assert.That(byteValue / doubles[1], Is.EqualTo(63.5d));
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

#pragma warning restore CS0675
