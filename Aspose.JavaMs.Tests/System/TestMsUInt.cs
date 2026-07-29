// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/03/2017 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

#pragma warning disable CS0675 // Java-specific tests.
namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestMsUInt
    {
        [Test]
        public void TestUnaryOperators()
        {
            uint uintValue = 4294967285U;
            uint uintValue2 = 10U;
            Assert.That(4294967285 == +uintValue, Is.True);
            Assert.That(-4294967285 == -uintValue, Is.True);
            Assert.That(4294967285 == uintValue++, Is.True);
            Assert.That(4294967287 == ++uintValue, Is.True);
            Assert.That(4294967287 == uintValue--, Is.True);
            Assert.That(4294967285 == --uintValue, Is.True);
            Assert.That(10 == ~uintValue, Is.True);
            Assert.That(4294967285 == ~uintValue2, Is.True);
        }

        [Test]
        public void TestUnaryBitwiseOperator()
        {
            uint maxValue = uint.MaxValue;
            uint minValue = uint.MinValue;

            Assert.That(~maxValue, Is.EqualTo(0));
            Assert.That(~minValue, Is.EqualTo(4294967295));

            Assert.That(~(maxValue + 1), Is.EqualTo(4294967295));
            Assert.That(~(maxValue - 1), Is.EqualTo(1));

            Assert.That(~(minValue + 1), Is.EqualTo(4294967294));
            Assert.That(~(minValue - 1), Is.EqualTo(0));

            Assert.That(~uints[0], Is.EqualTo(4294967294));
            Assert.That(~uints[1], Is.EqualTo(4294967293));
            Assert.That(~uints[2], Is.EqualTo(4294967292));
            Assert.That(~uints[3], Is.EqualTo(4294967245));
        }

        [Test]
        public void TestBinaryBitwiseOperator()
        {
            uint uintValue = 4294967285U;
            uint uintValue2 = 11U;

            Assert.That(uintValue & 0x0F0F0F0F, Is.EqualTo(252645125));
            Assert.That(uintValue & uintValue2, Is.EqualTo(1));

            Assert.That(uintValue | 0x0F0F0F0F, Is.EqualTo(4294967295));
            Assert.That(uintValue | uintValue2, Is.EqualTo(4294967295));

            Assert.That(uintValue ^ 0x0F0F0F0F, Is.EqualTo(4042322170));
            Assert.That(uintValue ^ uintValue2, Is.EqualTo(4294967294));

            Assert.That(uintValue >> 30, Is.EqualTo(3));
            Assert.That(uintValue2 << 30, Is.EqualTo(3221225472));
        }

        [Test]
        public void TestIncrementOperators()
        {
            uint uintValue = uints[4];
            uintValue++;
            Assert.That(uintValue, Is.EqualTo(0));
            uintValue--;
            Assert.That(uintValue, Is.EqualTo(4294967295));
            ++uintValue;
            Assert.That(uintValue, Is.EqualTo(0));
            --uintValue;
            Assert.That(uintValue, Is.EqualTo(4294967295));
            uintValue = (uint)ints[4];
            uintValue++;
            Assert.That(uintValue, Is.EqualTo(2147483648));
            uintValue--;
            Assert.That(uintValue, Is.EqualTo(2147483647));
            ++uintValue;
            Assert.That(uintValue, Is.EqualTo(2147483648));
            --uintValue;
            Assert.That(uintValue, Is.EqualTo(2147483647));
        }

        [Test]
        public void TestShiftOperators()
        {
            uint uintValue = uints[4];
            Assert.That(uintValue << 4, Is.EqualTo(4294967280));
            Assert.That(uintValue << 8, Is.EqualTo(4294967040));
            Assert.That(uintValue << 16, Is.EqualTo(4294901760));
            Assert.That(uintValue << 24, Is.EqualTo(4278190080));
            Assert.That(uintValue << 32, Is.EqualTo(4294967295));
            Assert.That(uintValue << 40, Is.EqualTo(4294967040));
            Assert.That(uintValue << 48, Is.EqualTo(4294901760));
            Assert.That(uintValue << 56, Is.EqualTo(4278190080));
            Assert.That(uintValue >> 4, Is.EqualTo(268435455));
            
            Assert.That(uintValue << sbytes[1], Is.EqualTo(4294967292));
            Assert.That(uintValue << bytes[1], Is.EqualTo(4294967292));
            Assert.That(uintValue << shorts[1], Is.EqualTo(4294967292));
            Assert.That(uintValue << ushorts[1], Is.EqualTo(4294967292));
            Assert.That(uintValue << ints[1], Is.EqualTo(4294967292));

            Assert.That(uintValue >> sbytes[1], Is.EqualTo(1073741823));
            Assert.That(uintValue >> bytes[1], Is.EqualTo(1073741823));
            Assert.That(uintValue >> shorts[1], Is.EqualTo(1073741823));
            Assert.That(uintValue >> ushorts[1], Is.EqualTo(1073741823));
            Assert.That(uintValue >> ints[1], Is.EqualTo(1073741823));
        }

        [Test]
        public void TestAssignment()
        {
            uint uintValue = (uint)sbytes[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("3"));
            uintValue = bytes[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("3"));
            uintValue = (uint)shorts[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("3"));
            uintValue = ushorts[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("3"));
            uintValue = (uint)ints[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("3"));
            uintValue = uints[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("3"));
            uintValue = (uint)longs[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("3"));
            uintValue = (uint)floats[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("3"));
            uintValue = (uint)doubles[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("3"));
        }

        [Test]
        public void TestSByteCompoundAssignment()
        {
            //arithmetic addition
            uint uintValue = (uint)(sbytes[4] / 2);
            uintValue += (uint)sbytes[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("65"));
            uintValue -= (uint)sbytes[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("62"));
            //arithmetic scaling
            uintValue *= (uint)sbytes[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("124"));
            uintValue %= (uint)sbytes[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("24"));
            uintValue /= (uint)sbytes[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("12"));
            //bitwise
            uintValue &= (uint)sbytes[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("0"));
#pragma warning disable CS0675
            uintValue |= (uint)sbytes[1];
#pragma warning restore CS0675
            Assert.That(uintValue.ToString(), Is.EqualTo("2"));
            uintValue ^= (uint)sbytes[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            uintValue >>= sbytes[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("12"));
            uintValue <<= sbytes[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestByteCompoundAssignment()
        {
            //arithmetic addition
            uint uintValue = (uint)(bytes[4] / 2);
            uintValue += bytes[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("129"));
            uintValue -= bytes[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("126"));
            //arithmetic scaling
            uintValue *= bytes[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("252"));
            uintValue %= bytes[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("2"));
            uintValue /= bytes[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("1"));
            //bitwise
            uintValue &= bytes[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("0"));
            uintValue |= bytes[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("2"));
            uintValue ^= bytes[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            uintValue >>= bytes[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("12"));
            uintValue <<= bytes[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestShortCompoundAssignment()
        {
            //arithmetic addition
            uint uintValue = (uint)(shorts[4] / 2);
            uintValue += (uint)shorts[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("16385"));
            uintValue -= (uint)shorts[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("16382"));
            //arithmetic scaling
            uintValue *= (uint)shorts[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("32764"));
            uintValue %= (uint)shorts[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("14"));
            uintValue /= (uint)shorts[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("7"));
            //bitwise
            uintValue &= (uint)shorts[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("2"));
#pragma warning disable CS0675
            uintValue |= (uint)shorts[1];
#pragma warning restore CS0675
            Assert.That(uintValue.ToString(), Is.EqualTo("2"));
            uintValue ^= (uint)shorts[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            uintValue >>= shorts[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("12"));
            uintValue <<= shorts[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestUShortCompoundAssignment()
        {
            //arithmetic addition
            uint uintValue = (uint)(ushorts[4] / 2);
            uintValue += ushorts[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("32769"));
            uintValue -= ushorts[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("32766"));
            //arithmetic scaling
            uintValue *= ushorts[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("65532"));
            uintValue %= ushorts[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("32"));
            uintValue /= ushorts[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("16"));
            //bitwise
            uintValue &= ushorts[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("0"));
            uintValue |= ushorts[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("2"));
            uintValue ^= ushorts[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            uintValue >>= ushorts[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("12"));
            uintValue <<= ushorts[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestIntCompoundAssignment()
        {
            //arithmetic addition
            uint uintValue = (uint)(ints[4] / 2);
            uintValue += (uint)ints[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("1073741825"));
            uintValue -= (uint)ints[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("1073741822"));
            //arithmetic scaling
            uintValue *= (uint)ints[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("2147483644"));
            uintValue %= (uint)ints[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("44"));
            uintValue /= (uint)ints[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("22"));
            //bitwise
            uintValue &= (uint)ints[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("2"));
            uintValue |= (uint)ints[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("2"));
            uintValue ^= (uint)ints[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            uintValue >>= ints[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("12"));
            uintValue <<= ints[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestUIntCompoundAssignment()
        {
            //arithmetic addition
            uint uintValue = (uints[4] / 2);
            uintValue += uints[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("2147483649"));
            uintValue -= uints[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("2147483646"));
            //arithmetic scaling
            uintValue *= uints[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("4294967292"));
            uintValue %= uints[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("42"));
            uintValue /= uints[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("21"));
            //bitwise
            uintValue &= uints[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("0"));
            uintValue |= uints[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("2"));
            uintValue ^= uints[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            uintValue >>= (int)uints[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("12"));
            uintValue <<= (int)uints[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestLongCompoundAssignment()
        {
            //arithmetic addition
            uint uintValue = (uint)(longs[4] / 2);
            uintValue += (uint)longs[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("1"));
            uintValue -= (uint)longs[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("4294967294"));
            //arithmetic scaling
            uintValue *= (uint)longs[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("4294967292"));
            uintValue %= (uint)longs[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("42"));
            uintValue /= (uint)longs[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("21"));
            //bitwise
            uintValue &= (uint)longs[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("0"));
            uintValue |= (uint)longs[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("2"));
            uintValue ^= (uint)longs[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            uintValue >>= (int)longs[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("12"));
            uintValue <<= (int)longs[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestFloatCompoundAssignment()
        {
            //arithmetic addition
            uint uintValue = (uint)(SByte.MaxValue);
            uintValue += (uint)floats[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("129"));
            uintValue -= (uint)floats[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("126"));
            //arithmetic scaling
            uintValue *= (uint)floats[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("252"));
            uintValue %= (uint)floats[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("2"));
            uintValue /= (uint)floats[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("1"));
            //bitwise
            uintValue &= (uint)floats[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("0"));
            uintValue |= (uint)floats[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("2"));
            uintValue ^= (uint)floats[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            uintValue >>= (int)floats[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("12"));
            uintValue <<= (int)floats[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
        }

        [Test]
        public void TestDoubleCompoundAssignment()
        {
            //arithmetic addition
            uint uintValue = (uint)(SByte.MaxValue);
            uintValue += (uint)doubles[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("129"));
            uintValue -= (uint)doubles[2];
            Assert.That(uintValue.ToString(), Is.EqualTo("126"));
            //arithmetic scaling
            uintValue *= (uint)doubles[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("252"));
            uintValue %= (uint)doubles[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("2"));
            uintValue /= (uint)doubles[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("1"));
            //bitwise
            uintValue &= (uint)doubles[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("0"));
            uintValue |= (uint)doubles[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("2"));
            uintValue ^= (uint)doubles[3];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
            //bitwise shift
            uintValue >>= (int)doubles[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("12"));
            uintValue <<= (int)doubles[1];
            Assert.That(uintValue.ToString(), Is.EqualTo("48"));
        }

        private readonly sbyte[] sbytes = { 1, 2, 3, 50, SByte.MaxValue };
        private readonly byte[] bytes = { 1, 2, 3, 50, Byte.MaxValue };
        private readonly short[] shorts = { 1, 2, 3, 50, Int16.MaxValue };
        private readonly ushort[] ushorts = { 1, 2, 3, 50, UInt16.MaxValue };
        private readonly int[] ints = { 1, 2, 3, 50, Int32.MaxValue };
        private readonly uint[] uints = { 1, 2, 3, 50, UInt32.MaxValue, UInt32.MinValue };
        private readonly long[] longs = { 1L, 2L, 3L, 50, Int64.MaxValue };
        private readonly float[] floats = { 1.0F, 2.0F, 3.0F, 50 };
        private readonly double[] doubles = { 1.0, 2.0, 3.0, 50 };
    }
}
#pragma warning restore CS0675
