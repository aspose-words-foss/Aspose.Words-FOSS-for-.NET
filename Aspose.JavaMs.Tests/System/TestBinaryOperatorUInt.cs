// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/03/2017 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

#pragma warning disable CS0675 // Java-specific tests.

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestBinaryOperatorUInt
    {
        [Test]
        public void TestSByteBinaryOperator()
        {
            //arithmetic addition
            uint uintValue = (uint)(sbytes[4] / 2);
            Assert.That(uintValue + sbytes[1], Is.EqualTo(65));
            Assert.That(uintValue - sbytes[2], Is.EqualTo(60));
            //arithmetic scaling
            Assert.That(uintValue * sbytes[1], Is.EqualTo(126));
            Assert.That(uintValue % sbytes[3], Is.EqualTo(13));
            Assert.That(uintValue / sbytes[1], Is.EqualTo(31));
            //bitwise
            Assert.That(uintValue & sbytes[1], Is.EqualTo(2));
            Assert.That(uintValue | sbytes[1], Is.EqualTo(63));
            Assert.That(uintValue ^ sbytes[3], Is.EqualTo(13));
            //bitwise shift
            Assert.That(uintValue >> sbytes[1], Is.EqualTo(15));
            Assert.That(uintValue << sbytes[1], Is.EqualTo(252));
        }

        [Test]
        public void TestByteBinaryOperator()
        {
            //arithmetic addition
            uint uintValue = (uint)(bytes[4] / 2);
            Assert.That(uintValue + bytes[1], Is.EqualTo(129));
            Assert.That(uintValue - bytes[2], Is.EqualTo(124));
            //arithmetic scaling
            Assert.That(uintValue * bytes[1], Is.EqualTo(254));
            Assert.That(uintValue % bytes[3], Is.EqualTo(27));
            Assert.That(uintValue / bytes[1], Is.EqualTo(63));
            //bitwise
            Assert.That(uintValue & bytes[1], Is.EqualTo(2));
            Assert.That(uintValue | bytes[1], Is.EqualTo(127));
            Assert.That(uintValue ^ bytes[3], Is.EqualTo(77));
            //bitwise shift
            Assert.That(uintValue >> bytes[1], Is.EqualTo(31));
            Assert.That(uintValue << bytes[1], Is.EqualTo(508));
        }

        [Test]
        public void TestShortBinaryOperator()
        {
            //arithmetic addition
            uint uintValue = (uint)(shorts[4] / 2);
            Assert.That(uintValue + shorts[1], Is.EqualTo(16385));
            Assert.That(uintValue - shorts[2], Is.EqualTo(16380));
            //arithmetic scaling
            Assert.That(uintValue * shorts[1], Is.EqualTo(32766));
            Assert.That(uintValue % shorts[3], Is.EqualTo(33));
            Assert.That(uintValue / shorts[1], Is.EqualTo(8191));
            //bitwise
            Assert.That(uintValue & shorts[1], Is.EqualTo(2));
            Assert.That(uintValue | shorts[1], Is.EqualTo(16383));
            Assert.That(uintValue ^ shorts[3], Is.EqualTo(16333));
            //bitwise shift
            Assert.That(uintValue >> shorts[1], Is.EqualTo(4095));
            Assert.That(uintValue << shorts[1], Is.EqualTo(65532));
        }

        [Test]
        public void TestUShortBinaryOperator()
        {
            //arithmetic addition
            uint uintValue = (uint)(ushorts[4] / 2);
            Assert.That(uintValue + ushorts[1], Is.EqualTo(32769));
            Assert.That(uintValue - ushorts[2], Is.EqualTo(32764));
            //arithmetic scaling
            Assert.That(uintValue * ushorts[1], Is.EqualTo(65534));
            Assert.That(uintValue % ushorts[3], Is.EqualTo(17));
            Assert.That(uintValue / ushorts[1], Is.EqualTo(16383));
            //bitwise
            Assert.That(uintValue & ushorts[1], Is.EqualTo(2));
            Assert.That(uintValue | ushorts[1], Is.EqualTo(32767));
            Assert.That(uintValue ^ ushorts[3], Is.EqualTo(32717));
            //bitwise shift
            Assert.That(uintValue >> ushorts[1], Is.EqualTo(8191));
            Assert.That(uintValue << ushorts[1], Is.EqualTo(131068));
        }

        [Test]
        public void TestIntBinaryOperator()
        {
            //arithmetic addition
            uint uintValue = (uint)(ints[4] / 2);
            Assert.That(uintValue + ints[1], Is.EqualTo(1073741825));
            Assert.That(uintValue - ints[2], Is.EqualTo(1073741820));
            //arithmetic scaling
            Assert.That(uintValue * ints[1], Is.EqualTo(2147483646));
            Assert.That(uintValue % ints[3], Is.EqualTo(23));
            Assert.That(uintValue / ints[1], Is.EqualTo(536870911));
            //bitwise
            Assert.That(uintValue & ints[1], Is.EqualTo(2));
            Assert.That(uintValue | ints[1], Is.EqualTo(1073741823));
            Assert.That(uintValue ^ ints[3], Is.EqualTo(1073741773));
            //bitwise shift
            Assert.That(uintValue >> ints[1], Is.EqualTo(268435455));
            Assert.That(uintValue << ints[1], Is.EqualTo(4294967292));
        }

        [Test]
        public void TestUIntBinaryOperator()
        {
            //arithmetic addition
            uint uintValue = (uint)(uints[4] / 2);
            Assert.That(uintValue + uints[1], Is.EqualTo(2147483649));
            Assert.That(uintValue - uints[2], Is.EqualTo(2147483644));
            //arithmetic scaling
            Assert.That(uintValue * uints[1], Is.EqualTo(4294967294));
            Assert.That(uintValue % uints[3], Is.EqualTo(47));
            Assert.That(uintValue / uints[1], Is.EqualTo(1073741823));
            //bitwise
            Assert.That(uintValue & uints[1], Is.EqualTo(2));
            Assert.That(uintValue | uints[1], Is.EqualTo(2147483647));
            Assert.That(uintValue ^ uints[3], Is.EqualTo(2147483597));
        }

        [Test]
        public void TestLongBinaryOperator()
        {
            //arithmetic addition
            uint uintValue = (uint)(longs[4] / 2);
            Assert.That(uintValue + longs[1], Is.EqualTo(4294967297L));
            Assert.That(uintValue - longs[2], Is.EqualTo(4294967292L));
            //arithmetic scaling
            Assert.That(uintValue * longs[1], Is.EqualTo(8589934590));
            Assert.That(uintValue % longs[3], Is.EqualTo(45));
            Assert.That(uintValue / longs[1], Is.EqualTo(2147483647));
            //bitwise
            Assert.That(uintValue & longs[1], Is.EqualTo(2));
            Assert.That(uintValue | longs[1], Is.EqualTo(4294967295L));
            Assert.That(uintValue ^ longs[3], Is.EqualTo(4294967245L));
        }

        [Test]
        public void TestFloatBinaryOperator()
        {
            //arithmetic addition
            uint uintValue = (uint)(Int16.MaxValue);
            Assert.That(uintValue + floats[1], Is.EqualTo(32769.0f));
            Assert.That(uintValue - floats[2], Is.EqualTo(32764.0f));
            //arithmetic scaling
            Assert.That(uintValue * floats[1], Is.EqualTo(65534.0f));
            Assert.That(uintValue % floats[3], Is.EqualTo(17.0f));
            Assert.That(uintValue / floats[1], Is.EqualTo(16383.5f));
        }

        [Test]
        public void TestDoubleBinaryOperator()
        {
            //arithmetic addition
            uint uintValue = (uint)(Int16.MaxValue);
            Assert.That(uintValue + doubles[1], Is.EqualTo(32769.0d));
            Assert.That(uintValue - doubles[2], Is.EqualTo(32764.0d));
            //arithmetic scaling
            Assert.That(uintValue * doubles[1], Is.EqualTo(65534.0d));
            Assert.That(uintValue % doubles[3], Is.EqualTo(17.0d));
            Assert.That(uintValue / doubles[1], Is.EqualTo(16383.5d));
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
