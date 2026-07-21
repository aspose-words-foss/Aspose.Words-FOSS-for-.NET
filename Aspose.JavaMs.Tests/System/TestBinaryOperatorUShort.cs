// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/03/2017 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

#pragma warning disable CS0675 // Java-specific tests.
namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestBinaryOperatorUShort
    {
        [Test]
        public void TestSByteBinaryOperator()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(sbytes[4] / 2);
            Assert.That(ushortValue + sbytes[1], Is.EqualTo(65));
            Assert.That(ushortValue - sbytes[2], Is.EqualTo(60));
            //arithmetic scaling
            Assert.That(ushortValue * sbytes[1], Is.EqualTo(126));
            Assert.That(ushortValue % sbytes[3], Is.EqualTo(13));
            Assert.That(ushortValue / sbytes[1], Is.EqualTo(31));
            //bitwise
            Assert.That(ushortValue & sbytes[1], Is.EqualTo(2));
#pragma warning disable CS0675
            Assert.That(ushortValue | sbytes[1], Is.EqualTo(63));
#pragma warning restore CS0675
            Assert.That(ushortValue ^ sbytes[3], Is.EqualTo(13));
            //bitwise shift
            Assert.That(ushortValue >> sbytes[1], Is.EqualTo(15));
            Assert.That(ushortValue << sbytes[1], Is.EqualTo(252));
        }

        [Test]
        public void TestByteBinaryOperator()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(bytes[4] / 2);
            Assert.That(ushortValue + bytes[1], Is.EqualTo(129));
            Assert.That(ushortValue - bytes[2], Is.EqualTo(124));
            //arithmetic scaling
            Assert.That(ushortValue * bytes[1], Is.EqualTo(254));
            Assert.That(ushortValue % bytes[3], Is.EqualTo(27));
            Assert.That(ushortValue / bytes[1], Is.EqualTo(63));
            //bitwise
            Assert.That(ushortValue & bytes[1], Is.EqualTo(2));
            Assert.That(ushortValue | bytes[1], Is.EqualTo(127));
            Assert.That(ushortValue ^ bytes[3], Is.EqualTo(77));
            //bitwise shift
            Assert.That(ushortValue >> bytes[1], Is.EqualTo(31));
            Assert.That(ushortValue << bytes[1], Is.EqualTo(508));
        }

        [Test]
        public void TestShortBinaryOperator()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(shorts[4] / 2);
            Assert.That(ushortValue + shorts[1], Is.EqualTo(16385));
            Assert.That(ushortValue - shorts[2], Is.EqualTo(16380));
            //arithmetic scaling
            Assert.That(ushortValue * shorts[1], Is.EqualTo(32766));
            Assert.That(ushortValue % shorts[3], Is.EqualTo(33));
            Assert.That(ushortValue / shorts[1], Is.EqualTo(8191));
            //bitwise
            Assert.That(ushortValue & shorts[1], Is.EqualTo(2));
#pragma warning disable CS0675
            Assert.That(ushortValue | shorts[1], Is.EqualTo(16383));
#pragma warning restore CS0675
            Assert.That(ushortValue ^ shorts[3], Is.EqualTo(16333));
            //bitwise shift
            Assert.That(ushortValue >> shorts[1], Is.EqualTo(4095));
            Assert.That(ushortValue << shorts[1], Is.EqualTo(65532));
        }

        [Test]
        public void TestUShortBinaryOperator()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(ushorts[4] / 2);
            Assert.That(ushortValue + ushorts[1], Is.EqualTo(32769));
            Assert.That(ushortValue - ushorts[2], Is.EqualTo(32764));
            //arithmetic scaling
            Assert.That(ushortValue * ushorts[1], Is.EqualTo(65534));
            Assert.That(ushortValue % ushorts[3], Is.EqualTo(17));
            Assert.That(ushortValue / ushorts[1], Is.EqualTo(16383));
            //bitwise
            Assert.That(ushortValue & ushorts[1], Is.EqualTo(2));
            Assert.That(ushortValue | ushorts[1], Is.EqualTo(32767));
            Assert.That(ushortValue ^ ushorts[3], Is.EqualTo(32717));
            //bitwise shift
            Assert.That(ushortValue >> ushorts[1], Is.EqualTo(8191));
            Assert.That(ushortValue << ushorts[1], Is.EqualTo(131068));
        }

        [Test]
        public void TestIntBinaryOperator()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(ints[4] / 2);
            Assert.That(ushortValue + ints[1], Is.EqualTo(65537));
            Assert.That(ushortValue - ints[2], Is.EqualTo(65532));
            //arithmetic scaling
            Assert.That(ushortValue * ints[1], Is.EqualTo(131070));
            Assert.That(ushortValue % ints[3], Is.EqualTo(35));
            Assert.That(ushortValue / ints[1], Is.EqualTo(32767));
            //bitwise
            Assert.That(ushortValue & ints[1], Is.EqualTo(2));
            Assert.That(ushortValue | ints[1], Is.EqualTo(65535));
            Assert.That(ushortValue ^ ints[3], Is.EqualTo(65485));
            //bitwise shift
            Assert.That(ushortValue >> ints[1], Is.EqualTo(16383));
            Assert.That(ushortValue << ints[1], Is.EqualTo(262140));
        }

        [Test]
        public void TestUIntBinaryOperator()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(uints[4] / 2);
            Assert.That(ushortValue + uints[1], Is.EqualTo(65537));
            Assert.That(ushortValue - uints[2], Is.EqualTo(65532));
            //arithmetic scaling
            Assert.That(ushortValue * uints[1], Is.EqualTo(131070));
            Assert.That(ushortValue % uints[3], Is.EqualTo(35));
            Assert.That(ushortValue / uints[1], Is.EqualTo(32767));
            //bitwise
            Assert.That(ushortValue & uints[1], Is.EqualTo(2));
            Assert.That(ushortValue | uints[1], Is.EqualTo(65535));
            Assert.That(ushortValue ^ uints[3], Is.EqualTo(65485));
        }

        [Test]
        public void TestLongBinaryOperator()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(longs[4] / 2);
            Assert.That(ushortValue + longs[1], Is.EqualTo(65537));
            Assert.That(ushortValue - longs[2], Is.EqualTo(65532));
            //arithmetic scaling
            Assert.That(ushortValue * longs[1], Is.EqualTo(131070));
            Assert.That(ushortValue % longs[3], Is.EqualTo(35));
            Assert.That(ushortValue / longs[1], Is.EqualTo(32767));
            //bitwise
            Assert.That(ushortValue & longs[1], Is.EqualTo(2));
            Assert.That(ushortValue | longs[1], Is.EqualTo(65535));
            Assert.That(ushortValue ^ longs[3], Is.EqualTo(65485));
        }

        [Test]
        public void TestFloatBinaryOperator()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(Int16.MaxValue);
            Assert.That(ushortValue + floats[1], Is.EqualTo(32769.0f));
            Assert.That(ushortValue - floats[2], Is.EqualTo(32764.0f));
            //arithmetic scaling
            Assert.That(ushortValue * floats[1], Is.EqualTo(65534.0f));
            Assert.That(ushortValue % floats[3], Is.EqualTo(17.0f));
            Assert.That(ushortValue / floats[1], Is.EqualTo(16383.5f));
        }

        [Test]
        public void TestDoubleBinaryOperator()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(Int16.MaxValue);
            Assert.That(ushortValue + doubles[1], Is.EqualTo(32769.0d));
            Assert.That(ushortValue - doubles[2], Is.EqualTo(32764.0d));
            //arithmetic scaling
            Assert.That(ushortValue * doubles[1], Is.EqualTo(65534.0d));
            Assert.That(ushortValue % doubles[3], Is.EqualTo(17.0d));
            Assert.That(ushortValue / doubles[1], Is.EqualTo(16383.5d));
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
