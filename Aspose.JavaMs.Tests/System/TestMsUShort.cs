// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/03/2017 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

#pragma warning disable CS0675 // Java-specific tests.
namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestMsUShort
    {
        [Test]
        public void TestUnaryOperators()
        {
            ushort ushortValue = (ushort)65525;
            ushort ushortValue2 = (ushort)10;
            Assert.That(+ushortValue, Is.EqualTo(65525));
            Assert.That(-ushortValue, Is.EqualTo(-65525));
            Assert.That(ushortValue++, Is.EqualTo(65525));
            Assert.That(++ushortValue, Is.EqualTo(65527));
            Assert.That(ushortValue--, Is.EqualTo(65527));
            Assert.That(--ushortValue, Is.EqualTo(65525));
            Assert.That(~ushortValue, Is.EqualTo(-65526));
            Assert.That(~ushortValue2, Is.EqualTo(-11));
        }

        [Test]
        public void TestUnaryBitwiseOperator()
        {
            ushort maxValue = ushort.MaxValue;
            ushort minValue = ushort.MinValue;

            Assert.That(~maxValue, Is.EqualTo(-65536));
            Assert.That(~minValue, Is.EqualTo(-1));

            Assert.That(~(maxValue + 1), Is.EqualTo(-65537));
            Assert.That(~(maxValue - 1), Is.EqualTo(-65535));

            Assert.That(~(minValue + 1), Is.EqualTo(-2));
            Assert.That(~(minValue - 1), Is.EqualTo(0));

            Assert.That(~ushorts[0], Is.EqualTo(-2));
            Assert.That(~ushorts[1], Is.EqualTo(-3));
            Assert.That(~ushorts[2], Is.EqualTo(-4));
            Assert.That(~ushorts[3], Is.EqualTo(-51));
        }

        [Test]
        public void TestBinaryBitwiseOperator()
        {
            ushort ushortValue = (ushort)65525;
            ushort ushortValue2 = (ushort)11;

            Assert.That(ushortValue & 0x0FFF, Is.EqualTo(4085));
            Assert.That(ushortValue & ushortValue2, Is.EqualTo(1));

            Assert.That(ushortValue | 0x0FFF, Is.EqualTo(65535));
            Assert.That(ushortValue | ushortValue2, Is.EqualTo(65535));

            Assert.That(ushortValue ^ 0x0FFF, Is.EqualTo(61450));
            Assert.That(ushortValue ^ ushortValue2, Is.EqualTo(65534));

            Assert.That(ushortValue >> 14, Is.EqualTo(3));
            Assert.That(ushortValue2 << 12, Is.EqualTo(45056));
        }

        [Test]
        public void TestIncrementOperators()
        {
            ushort ushortValue = ushorts[4];
            ushortValue++;
            Assert.That(ushortValue, Is.EqualTo((ushort)0));
            ushortValue--;
            Assert.That(ushortValue, Is.EqualTo((ushort)65535));
            ++ushortValue;
            Assert.That(ushortValue, Is.EqualTo((ushort)0));
            --ushortValue;
            Assert.That(ushortValue, Is.EqualTo((ushort)65535));
            ushortValue = (ushort)shorts[4];
            ushortValue++;
            Assert.That(ushortValue, Is.EqualTo((ushort)32768));
            ushortValue--;
            Assert.That(ushortValue, Is.EqualTo((ushort)32767));
            ++ushortValue;
            Assert.That(ushortValue, Is.EqualTo((ushort)32768));
            --ushortValue;
            Assert.That(ushortValue, Is.EqualTo((ushort)32767));
        }

        [Test]
        public void TestShiftOperators()
        {
            ushort ushortValue = ushorts[4];
            Assert.That(ushortValue << 4, Is.EqualTo(1048560));
            Assert.That(ushortValue << 8, Is.EqualTo(16776960));
            Assert.That(ushortValue << 16, Is.EqualTo(-65536));
            Assert.That(ushortValue << 24, Is.EqualTo(-16777216));
            Assert.That(ushortValue << 32, Is.EqualTo(65535));
            Assert.That(ushortValue << 40, Is.EqualTo(16776960));
            Assert.That(ushortValue << 48, Is.EqualTo(-65536));
            Assert.That(ushortValue << 56, Is.EqualTo(-16777216));
            Assert.That(ushortValue >> 4, Is.EqualTo(4095));

            Assert.That(ushortValue << sbytes[1], Is.EqualTo(262140));
            Assert.That(ushortValue << bytes[1], Is.EqualTo(262140));
            Assert.That(ushortValue << shorts[1], Is.EqualTo(262140));
            Assert.That(ushortValue << ushorts[1], Is.EqualTo(262140));
            Assert.That(ushortValue << ints[1], Is.EqualTo(262140));

            Assert.That(ushortValue >> sbytes[1], Is.EqualTo(16383));
            Assert.That(ushortValue >> bytes[1], Is.EqualTo(16383));
            Assert.That(ushortValue >> shorts[1], Is.EqualTo(16383));
            Assert.That(ushortValue >> ushorts[1], Is.EqualTo(16383));
            Assert.That(ushortValue >> ints[1], Is.EqualTo(16383));
        }

        [Test]
        public void TestAssignment()
        {
            ushort ushortValue = (ushort)sbytes[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("3"));
            ushortValue = bytes[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("3"));
            ushortValue = (ushort)shorts[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("3"));
            ushortValue = ushorts[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("3"));
            ushortValue = (ushort)ints[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("3"));
            ushortValue = (ushort)uints[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("3"));
            ushortValue = (ushort)longs[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("3"));
            ushortValue = (ushort)floats[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("3"));
            ushortValue = (ushort)doubles[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("3"));
        }

        [Test]
        public void TestSByteCompoundAssignment()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(sbytes[4] / 2);
            ushortValue += (ushort)sbytes[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("65"));
            ushortValue -= (ushort)sbytes[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("62"));
            //arithmetic scaling
            ushortValue *= (ushort)sbytes[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("124"));
            ushortValue %= (ushort)sbytes[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("24"));
            ushortValue /= (ushort)sbytes[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("12"));
            //bitwise
            ushortValue &= (ushort)sbytes[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("0"));
#pragma warning disable CS0675
            ushortValue |= (ushort)sbytes[1];
#pragma warning restore CS0675
            Assert.That(ushortValue.ToString(), Is.EqualTo("2"));
            ushortValue ^= (ushort)sbytes[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            ushortValue >>= (ushort)sbytes[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("12"));
            ushortValue <<= (ushort)sbytes[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestByteCompoundAssignment()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(bytes[4] / 2);
            ushortValue += bytes[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("129"));
            ushortValue -= bytes[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("126"));
            //arithmetic scaling
            ushortValue *= bytes[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("252"));
            ushortValue %= bytes[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("2"));
            ushortValue /= bytes[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("1"));
            //bitwise
            ushortValue &= bytes[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("0"));
            ushortValue |= bytes[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("2"));
            ushortValue ^= bytes[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            ushortValue >>= bytes[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("12"));
            ushortValue <<= bytes[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestShortCompoundAssignment()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(shorts[4] / 2);
            ushortValue += (ushort)shorts[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("16385"));
            ushortValue -= (ushort)shorts[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("16382"));
            //arithmetic scaling
            ushortValue *= (ushort)shorts[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("32764"));
            ushortValue %= (ushort)shorts[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("14"));
            ushortValue /= (ushort)shorts[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("7"));
            //bitwise
            ushortValue &= (ushort)shorts[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("2"));
            ushortValue |= (ushort)shorts[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("2"));
            ushortValue ^= (ushort)shorts[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            ushortValue >>= shorts[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("12"));
            ushortValue <<= shorts[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestUShortCompoundAssignment()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(ushorts[4] / 2);
            ushortValue += ushorts[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("32769"));
            ushortValue -= ushorts[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("32766"));
            //arithmetic scaling
            ushortValue *= ushorts[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("65532"));
            ushortValue %= ushorts[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("32"));
            ushortValue /= ushorts[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("16"));
            //bitwise
            ushortValue &= ushorts[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("0"));
            ushortValue |= ushorts[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("2"));
            ushortValue ^= ushorts[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            ushortValue >>= ushorts[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("12"));
            ushortValue <<= ushorts[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestIntCompoundAssignment()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(ints[4] / 2);
            ushortValue += (ushort)ints[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("1"));
            ushortValue -= (ushort)ints[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("65534"));
            //arithmetic scaling
            ushortValue *= (ushort)ints[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("65532"));
            ushortValue %= (ushort)ints[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("32"));
            ushortValue /= (ushort)ints[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("16"));
            //bitwise
            ushortValue &= (ushort)ints[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("0"));
            ushortValue |= (ushort)ints[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("2"));
            ushortValue ^= (ushort)ints[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            ushortValue >>= ints[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("12"));
            ushortValue <<= ints[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestUIntCompoundAssignment()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(uints[4] / 2);
            ushortValue += (ushort)uints[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("1"));
            ushortValue -= (ushort)uints[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("65534"));
            //arithmetic scaling
            ushortValue *= (ushort)uints[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("65532"));
            ushortValue %= (ushort)uints[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("32"));
            ushortValue /= (ushort)uints[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("16"));
            //bitwise
            ushortValue &= (ushort)uints[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("0"));
            ushortValue |= (ushort)uints[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("2"));
            ushortValue ^= (ushort)uints[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            ushortValue >>= (int)uints[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("12"));
            ushortValue <<= (int)uints[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestLongCompoundAssignment()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(longs[4] / 2);
            ushortValue += (ushort)longs[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("1"));
            ushortValue -= (ushort)longs[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("65534"));
            //arithmetic scaling
            ushortValue *= (ushort)longs[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("65532"));
            ushortValue %= (ushort)longs[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("32"));
            ushortValue /= (ushort)longs[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("16"));
            //bitwise
            ushortValue &= (ushort)longs[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("0"));
            ushortValue |= (ushort)longs[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("2"));
            ushortValue ^= (ushort)longs[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            ushortValue >>= (int)longs[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("12"));
            ushortValue <<= (int)longs[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestFloatCompoundAssignment()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(SByte.MaxValue);
            ushortValue += (ushort)floats[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("129"));
            ushortValue -= (ushort)floats[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("126"));
            //arithmetic scaling
            ushortValue *= (ushort)floats[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("252"));
            ushortValue %= (ushort)floats[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("2"));
            ushortValue /= (ushort)floats[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("1"));
            //bitwise
            ushortValue &= (ushort)floats[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("0"));
            ushortValue |= (ushort)floats[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("2"));
            ushortValue ^= (ushort)floats[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            ushortValue >>= (int)floats[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("12"));
            ushortValue <<= (int)floats[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestDoubleCompoundAssignment()
        {
            //arithmetic addition
            ushort ushortValue = (ushort)(SByte.MaxValue);
            ushortValue += (ushort)doubles[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("129"));
            ushortValue -= (ushort)doubles[2];
            Assert.That(ushortValue.ToString(), Is.EqualTo("126"));
            //arithmetic scaling
            ushortValue *= (ushort)doubles[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("252"));
            ushortValue %= (ushort)doubles[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("2"));
            ushortValue /= (ushort)doubles[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("1"));
            //bitwise
            ushortValue &= (ushort)doubles[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("0"));
            ushortValue |= (ushort)doubles[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("2"));
            ushortValue ^= (ushort)doubles[3];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            ushortValue >>= (int)doubles[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("12"));
            ushortValue <<= (int)doubles[1];
            Assert.That(ushortValue.ToString(), Is.EqualTo("48"));
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
#pragma warning restore CS0675
