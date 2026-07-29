// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/03/2017 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestMsByte
    {
        [Test]
        public void TestUnaryOperators()
        {
            byte byteValue = (byte)245;
            byte byteValue2 = (byte)10;
            Assert.That(+byteValue, Is.EqualTo(245));
            Assert.That(-byteValue, Is.EqualTo(-245));
            Assert.That(byteValue++, Is.EqualTo(245));
            Assert.That(++byteValue, Is.EqualTo(247));
            Assert.That(byteValue--, Is.EqualTo(247));
            Assert.That(--byteValue, Is.EqualTo(245));
            Assert.That(~byteValue, Is.EqualTo(-246));
            Assert.That(~byteValue2, Is.EqualTo(-11));
        }

        [Test]
        public void TestUnaryBitwiseOperator()
        {
            byte maxValue = byte.MaxValue;
            byte minValue = byte.MinValue;

            Assert.That(~maxValue, Is.EqualTo(-256));
            Assert.That(~minValue, Is.EqualTo(-1));

            Assert.That(~(maxValue + 1), Is.EqualTo(-257));
            Assert.That(~(maxValue - 1), Is.EqualTo(-255));

            Assert.That(~(minValue + 1), Is.EqualTo(-2));
            Assert.That(~(minValue - 1), Is.EqualTo(0));

            Assert.That(~bytes[0], Is.EqualTo(-2));
            Assert.That(~bytes[1], Is.EqualTo(-3));
            Assert.That(~bytes[2], Is.EqualTo(-4));
            Assert.That(~bytes[3], Is.EqualTo(-51));
        }

        [Test]
        public void TestBinaryBitwiseOperator()
        {
            byte byteValue = (byte)245;
            byte byteValue2 = (byte)11;

            Assert.That(byteValue & 0x0F, Is.EqualTo(5));
            Assert.That(byteValue & byteValue2, Is.EqualTo(1));

            Assert.That(byteValue | 0x0F, Is.EqualTo(255));
            Assert.That(byteValue | byteValue2, Is.EqualTo(255));

            Assert.That(byteValue ^ 0x0F, Is.EqualTo(250));
            Assert.That(byteValue ^ byteValue2, Is.EqualTo(254));

            Assert.That(byteValue >> 1, Is.EqualTo(122));
            Assert.That(byteValue2 << 1, Is.EqualTo(22));
        }

        [Test]
        public void TestIncrementOperators()
        {
            byte byteValue = bytes[4];
            byteValue++;
            Assert.That(byteValue, Is.EqualTo(0));
            byteValue--;
            Assert.That(byteValue, Is.EqualTo(255));
            ++byteValue;
            Assert.That(byteValue, Is.EqualTo(0));
            --byteValue;
            Assert.That(byteValue, Is.EqualTo(255));
            byteValue = (byte)sbytes[4];
            byteValue++;
            Assert.That(byteValue, Is.EqualTo(128));
            byteValue--;
            Assert.That(byteValue, Is.EqualTo(127));
            ++byteValue;
            Assert.That(byteValue, Is.EqualTo(128));
            --byteValue;
            Assert.That(byteValue, Is.EqualTo(127));
        }

        [Test]
        public void TestShiftOperators()
        {
            byte byteValue = bytes[4];
            Assert.That(byteValue << 4, Is.EqualTo(4080));
            Assert.That(byteValue << 8, Is.EqualTo(65280));
            Assert.That(byteValue << 16, Is.EqualTo(16711680));
            Assert.That(byteValue << 24, Is.EqualTo(-16777216));
            Assert.That(byteValue << 32, Is.EqualTo(255));
            Assert.That(byteValue << 40, Is.EqualTo(65280));
            Assert.That(byteValue << 48, Is.EqualTo(16711680));
            Assert.That(byteValue << 56, Is.EqualTo(-16777216));
            Assert.That(byteValue >> 4, Is.EqualTo(15));

            Assert.That(byteValue << sbytes[1], Is.EqualTo(1020));
            Assert.That(byteValue << bytes[1], Is.EqualTo(1020));
            Assert.That(byteValue << shorts[1], Is.EqualTo(1020));
            Assert.That(byteValue << ushorts[1], Is.EqualTo(1020));
            Assert.That(byteValue << ints[1], Is.EqualTo(1020));

            Assert.That(byteValue >> sbytes[1], Is.EqualTo(63));
            Assert.That(byteValue >> bytes[1], Is.EqualTo(63));
            Assert.That(byteValue >> shorts[1], Is.EqualTo(63));
            Assert.That(byteValue >> ushorts[1], Is.EqualTo(63));
            Assert.That(byteValue >> ints[1], Is.EqualTo(63));
        }

        [Test]
        public void TestAssignment()
        {
            byte byteValue = (byte)sbytes[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("3"));
            byteValue = bytes[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("3"));
            byteValue = (byte)shorts[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("3"));
            byteValue = (byte)ushorts[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("3"));
            byteValue = (byte)ints[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("3"));
            byteValue = (byte)uints[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("3"));
            byteValue = (byte)longs[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("3"));
            byteValue = (byte)floats[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("3"));
            byteValue = (byte)doubles[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("3"));
        }

        [Test]
        public void TestSByteCompoundAssignment()
        {
            //arithmetic addition
            byte byteValue = (byte)(sbytes[4] / 2);
            byteValue += (byte)sbytes[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("65"));
            byteValue -= (byte)sbytes[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("62"));
            //arithmetic scaling
            byteValue *= (byte)sbytes[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("124"));
            byteValue %= (byte)sbytes[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("24"));
            byteValue /= (byte)sbytes[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("12"));
            //bitwise
            byteValue &= (byte)sbytes[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("0"));
            byteValue |= (byte)sbytes[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue ^= (byte)sbytes[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            byteValue >>= (byte)sbytes[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("12"));
            byteValue <<= (byte)sbytes[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestByteCompoundAssignment()
        {
            //arithmetic addition
            byte byteValue = (byte)(bytes[4] / 2);
            byteValue += bytes[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("129"));
            byteValue -= bytes[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("126"));
            //arithmetic scaling
            byteValue *= bytes[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("252"));
            byteValue %= bytes[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue /= bytes[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("1"));
            //bitwise
            byteValue &= bytes[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("0"));
            byteValue |= bytes[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue ^= bytes[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            byteValue >>= bytes[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("12"));
            byteValue <<= bytes[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestShortCompoundAssignment()
        {
            //arithmetic addition
            byte byteValue = (byte)(shorts[4] / 2);
            byteValue += (byte)shorts[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("1"));
            byteValue -= (byte)shorts[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("254"));
            //arithmetic scaling
            byteValue *= (byte)shorts[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("252"));
            byteValue %= (byte)shorts[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue /= (byte)shorts[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("1"));
            //bitwise
            byteValue &= (byte)shorts[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("0"));
            byteValue |= (byte)shorts[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue ^= (byte)shorts[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            byteValue >>= shorts[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("12"));
            byteValue <<= shorts[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestUShortCompoundAssignment()
        {
            //arithmetic addition
            byte byteValue = (byte)(ushorts[4] / 2);
            byteValue += (byte)ushorts[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("1"));
            byteValue -= (byte)ushorts[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("254"));
            //arithmetic scaling
            byteValue *= (byte)ushorts[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("252"));
            byteValue %= (byte)ushorts[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue /= (byte)ushorts[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("1"));
            //bitwise
            byteValue &= (byte)ushorts[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("0"));
            byteValue |= (byte)ushorts[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue ^= (byte)ushorts[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            byteValue >>= ushorts[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("12"));
            byteValue <<= ushorts[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestIntCompoundAssignment()
        {
            //arithmetic addition
            byte byteValue = (byte)(ints[4] / 2);
            byteValue += (byte)ints[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("1"));
            byteValue -= (byte)ints[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("254"));
            //arithmetic scaling
            byteValue *= (byte)ints[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("252"));
            byteValue %= (byte)ints[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue /= (byte)ints[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("1"));
            //bitwise
            byteValue &= (byte)ints[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("0"));
            byteValue |= (byte)ints[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue ^= (byte)ints[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            byteValue >>= ints[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("12"));
            byteValue <<= ints[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestUIntCompoundAssignment()
        {
            //arithmetic addition
            byte byteValue = (byte)(uints[4] / 2);
            byteValue += (byte)uints[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("1"));
            byteValue -= (byte)uints[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("254"));
            //arithmetic scaling
            byteValue *= (byte)uints[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("252"));
            byteValue %= (byte)uints[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue /= (byte)uints[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("1"));
            //bitwise
            byteValue &= (byte)uints[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("0"));
            byteValue |= (byte)uints[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue ^= (byte)uints[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            byteValue >>= (int)uints[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("12"));
            byteValue <<= (int)uints[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestLongCompoundAssignment()
        {
            //arithmetic addition
            byte byteValue = (byte)(longs[4] / 2);
            byteValue += (byte)longs[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("1"));
            byteValue -= (byte)longs[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("254"));
            //arithmetic scaling
            byteValue *= (byte)longs[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("252"));
            byteValue %= (byte)longs[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue /= (byte)longs[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("1"));
            //bitwise
            byteValue &= (byte)longs[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("0"));
            byteValue |= (byte)longs[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue ^= (byte)longs[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            byteValue >>= (int)longs[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("12"));
            byteValue <<= (int)longs[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestFloatCompoundAssignment()
        {
            //arithmetic addition
            byte byteValue = (byte)(SByte.MaxValue);
            byteValue += (byte)floats[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("129"));
            byteValue -= (byte)floats[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("126"));
            //arithmetic scaling
            byteValue *= (byte)floats[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("252"));
            byteValue %= (byte)floats[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue /= (byte)floats[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("1"));
            //bitwise
            byteValue &= (byte)floats[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("0"));
            byteValue |= (byte)floats[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue ^= (byte)floats[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            byteValue >>= (int)floats[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("12"));
            byteValue <<= (int)floats[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestDoubleCompoundAssignment()
        {
            //arithmetic addition
            byte byteValue = (byte)(SByte.MaxValue);
            byteValue += (byte)doubles[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("129"));
            byteValue -= (byte)doubles[2];
            Assert.That(byteValue.ToString(), Is.EqualTo("126"));
            //arithmetic scaling
            byteValue *= (byte)doubles[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("252"));
            byteValue %= (byte)doubles[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue /= (byte)doubles[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("1"));
            //bitwise
            byteValue &= (byte)doubles[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("0"));
            byteValue |= (byte)doubles[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("2"));
            byteValue ^= (byte)doubles[3];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            byteValue >>= (int)doubles[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("12"));
            byteValue <<= (int)doubles[1];
            Assert.That(byteValue.ToString(), Is.EqualTo("48"));
        }

        private readonly sbyte[] sbytes = { 1, 2, 3, 50, SByte.MaxValue};
        private readonly byte[] bytes = { 1, 2, 3, 50, Byte.MaxValue};
        private readonly short[] shorts = { 1, 2, 3, 50, Int16.MaxValue};
        private readonly ushort[] ushorts = { 1, 2, 3, 50, UInt16.MaxValue};
        private readonly int[] ints = { 1, 2, 3, 50, Int32.MaxValue};
        private readonly uint[] uints = { 1, 2, 3, 50, UInt32.MaxValue };
        private readonly long[] longs = { 1L, 2L, 3L, 50, Int64.MaxValue};
        private readonly float[] floats = { 1.0F, 2.0F, 3.0F, 50};
        private readonly double[] doubles = { 1.0, 2.0, 3.0, 50};
    }
}
