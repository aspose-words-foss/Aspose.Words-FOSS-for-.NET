// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/02/2017 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Number
{
    [TestFixture]
    public class TestFormatUInt
    {
        [Test]
        public void TestBasic()
        {
            uint uintValue = 0x1F;
            Assert.That(uintValue.ToString(), Is.EqualTo("31"));
            Assert.That(uintValue.ToString("D2"), Is.EqualTo("31"));
            Assert.That(uintValue.ToString("d2"), Is.EqualTo("31"));
            Assert.That(uintValue.ToString("D1"), Is.EqualTo("31"));
            Assert.That(uintValue.ToString("d1"), Is.EqualTo("31"));
            Assert.That(uintValue.ToString("X"), Is.EqualTo("1F"));
            Assert.That(uintValue.ToString("x"), Is.EqualTo("1f"));
            Assert.That(uintValue.ToString("X1"), Is.EqualTo("1F"));
            Assert.That(uintValue.ToString("x1"), Is.EqualTo("1f"));
            uintValue = 0x99FFFFFF;
            Assert.That(uintValue.ToString(), Is.EqualTo("2583691263"));
            Assert.That(uintValue.ToString("D2"), Is.EqualTo("2583691263"));
            Assert.That(uintValue.ToString("d2"), Is.EqualTo("2583691263"));
            Assert.That(uintValue.ToString("D1"), Is.EqualTo("2583691263"));
            Assert.That(uintValue.ToString("d1"), Is.EqualTo("2583691263"));
            Assert.That(uintValue.ToString("X"), Is.EqualTo("99FFFFFF"));
            Assert.That(uintValue.ToString("x"), Is.EqualTo("99ffffff"));
            Assert.That(uintValue.ToString("X1"), Is.EqualTo("99FFFFFF"));
            Assert.That(uintValue.ToString("x1"), Is.EqualTo("99ffffff"));
        }

        [Test]
        public void TestZeroPad()
        {
            uint uintValue = 31;
            Assert.That(uintValue.ToString("D5"), Is.EqualTo("00031"));
            Assert.That(uintValue.ToString("d5"), Is.EqualTo("00031"));

            Assert.That(uintValue.ToString("X5"), Is.EqualTo("0001F"));
            Assert.That(uintValue.ToString("x5"), Is.EqualTo("0001f"));
        }

        [Test]
        public void TestUIntArray()
        {
            Assert.That(uintArray[3].ToString(), Is.EqualTo("99"));
            Assert.That(uintArray[4].ToString("d5"), Is.EqualTo("4294967294"));

            Assert.That(uintArray[2].ToString("X2"), Is.EqualTo("09"));
            Assert.That(uintArray[4].ToString("x"), Is.EqualTo("fffffffe"));
        }

        [Test]
        public void TestBorder()
        {
            Assert.That(uintArray[0].ToString(), Is.EqualTo("4294967295"));
            Assert.That(uintArray[1].ToString(), Is.EqualTo("0"));
            Assert.That(uintArray[2].ToString(), Is.EqualTo("9"));
            Assert.That(uintArray[3].ToString(), Is.EqualTo("99"));
            Assert.That(uintArray[4].ToString(), Is.EqualTo("4294967294"));
            Assert.That(uintArray[5].ToString(), Is.EqualTo("4294967285"));
            Assert.That(uintArray[6].ToString(), Is.EqualTo("4294967195"));
            Assert.That(uintArray[7].ToString(), Is.EqualTo("0"));
            Assert.That(uintArray[8].ToString(), Is.EqualTo("1"));
            Assert.That(uintArray[9].ToString(), Is.EqualTo("10"));
            Assert.That(uintArray[10].ToString(), Is.EqualTo("100"));
            Assert.That(uintArray[11].ToString(), Is.EqualTo("4294967295"));
            Assert.That(uintArray[12].ToString(), Is.EqualTo("4294967286"));
            Assert.That(uintArray[13].ToString(), Is.EqualTo("4294967196"));
        }

        [Test]
        public void TestMax()
        {
            Assert.That(maxValue.ToString(), Is.EqualTo("4294967295"));
            Assert.That(maxValue.ToString("D"), Is.EqualTo("4294967295"));
            Assert.That(maxValue.ToString("d"), Is.EqualTo("4294967295"));

            Assert.That(maxValue.ToString("D1"), Is.EqualTo("4294967295"));
            Assert.That(maxValue.ToString("d2"), Is.EqualTo("4294967295"));
            Assert.That(maxValue.ToString("D5"), Is.EqualTo("4294967295"));

            Assert.That(maxValue.ToString("X"), Is.EqualTo("FFFFFFFF"));
            Assert.That(maxValue.ToString("x"), Is.EqualTo("ffffffff"));
        }

        [Test]
        public void TestMin()
        {
            Assert.That(minValue.ToString(), Is.EqualTo("0"));
            Assert.That(minValue.ToString("D"), Is.EqualTo("0"));
            Assert.That(minValue.ToString("d"), Is.EqualTo("0"));
            Assert.That(minValue.ToString("G"), Is.EqualTo("0"));
            Assert.That(minValue.ToString("g"), Is.EqualTo("0"));

            Assert.That(minValue.ToString("D1"), Is.EqualTo("0"));
            Assert.That(minValue.ToString("d2"), Is.EqualTo("00"));
            Assert.That(minValue.ToString("D5"), Is.EqualTo("00000"));

            Assert.That(minValue.ToString("X"), Is.EqualTo("0"));
            Assert.That(minValue.ToString("x"), Is.EqualTo("0"));
        }

        private static uint maxValue = UInt32.MaxValue;
        private static uint minValue = UInt32.MinValue;

        private static uint[] uintArray =
        {
            maxValue,
            (uint)(maxValue + 1),
            (uint)(maxValue + 10),
            (uint)(maxValue + 100),
            (uint)(maxValue - 1),
            (uint)(maxValue - 10),
            (uint)(maxValue - 100),
            minValue,
            (uint)(minValue + 1),
            (uint)(minValue + 10),
            (uint)(minValue + 100),
            (uint)(minValue - 1),
            (uint)(minValue - 10),
            (uint)(minValue - 100)
        };
    }
}