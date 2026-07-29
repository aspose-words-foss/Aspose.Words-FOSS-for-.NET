// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/02/2017 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Number
{
    [TestFixture]
    public class TestFormatUShort
    {
        [Test]
        public void TestBasic()
        {
            ushort ushortValue = 0x1F;
            Assert.That(ushortValue.ToString(), Is.EqualTo("31"));
            Assert.That(ushortValue.ToString("D2"), Is.EqualTo("31"));
            Assert.That(ushortValue.ToString("d2"), Is.EqualTo("31"));
            Assert.That(ushortValue.ToString("D1"), Is.EqualTo("31"));
            Assert.That(ushortValue.ToString("d1"), Is.EqualTo("31"));
            Assert.That(ushortValue.ToString("X"), Is.EqualTo("1F"));
            Assert.That(ushortValue.ToString("x"), Is.EqualTo("1f"));
            Assert.That(ushortValue.ToString("X1"), Is.EqualTo("1F"));
            Assert.That(ushortValue.ToString("x1"), Is.EqualTo("1f"));
            ushortValue = 0x99F1;
            Assert.That(ushortValue.ToString(), Is.EqualTo("39409"));
            Assert.That(ushortValue.ToString("D2"), Is.EqualTo("39409"));
            Assert.That(ushortValue.ToString("d2"), Is.EqualTo("39409"));
            Assert.That(ushortValue.ToString("D1"), Is.EqualTo("39409"));
            Assert.That(ushortValue.ToString("d1"), Is.EqualTo("39409"));
            Assert.That(ushortValue.ToString("X"), Is.EqualTo("99F1"));
            Assert.That(ushortValue.ToString("x"), Is.EqualTo("99f1"));
            Assert.That(ushortValue.ToString("X1"), Is.EqualTo("99F1"));
            Assert.That(ushortValue.ToString("x1"), Is.EqualTo("99f1"));
        }

        [Test]
        public void TestZeroPad()
        {
            ushort ushortValue = 31;
            Assert.That(ushortValue.ToString("D5"), Is.EqualTo("00031"));
            Assert.That(ushortValue.ToString("d5"), Is.EqualTo("00031"));

            Assert.That(ushortValue.ToString("X5"), Is.EqualTo("0001F"));
            Assert.That(ushortValue.ToString("x5"), Is.EqualTo("0001f"));
        }

        [Test]
        public void TestUShortArray()
        {
            Assert.That(ushortArray[3].ToString(), Is.EqualTo("99"));
            Assert.That(ushortArray[4].ToString("d5"), Is.EqualTo("65534"));

            Assert.That(ushortArray[2].ToString("X2"), Is.EqualTo("09"));
            Assert.That(ushortArray[4].ToString("x"), Is.EqualTo("fffe"));
        }

        [Test]
        public void TestBorder()
        {
            Assert.That(ushortArray[0].ToString(), Is.EqualTo("65535"));
            Assert.That(ushortArray[1].ToString(), Is.EqualTo("0"));
            Assert.That(ushortArray[2].ToString(), Is.EqualTo("9"));
            Assert.That(ushortArray[3].ToString(), Is.EqualTo("99"));
            Assert.That(ushortArray[4].ToString(), Is.EqualTo("65534"));
            Assert.That(ushortArray[5].ToString(), Is.EqualTo("65525"));
            Assert.That(ushortArray[6].ToString(), Is.EqualTo("65435"));
            Assert.That(ushortArray[7].ToString(), Is.EqualTo("0"));
            Assert.That(ushortArray[8].ToString(), Is.EqualTo("1"));
            Assert.That(ushortArray[9].ToString(), Is.EqualTo("10"));
            Assert.That(ushortArray[10].ToString(), Is.EqualTo("100"));
            Assert.That(ushortArray[11].ToString(), Is.EqualTo("65535"));
            Assert.That(ushortArray[12].ToString(), Is.EqualTo("65526"));
            Assert.That(ushortArray[13].ToString(), Is.EqualTo("65436"));
        }

        [Test]
        public void TestMax()
        {
            Assert.That(maxValue.ToString(), Is.EqualTo("65535"));
            Assert.That(maxValue.ToString("D"), Is.EqualTo("65535"));
            Assert.That(maxValue.ToString("d"), Is.EqualTo("65535"));

            Assert.That(maxValue.ToString("D1"), Is.EqualTo("65535"));
            Assert.That(maxValue.ToString("d2"), Is.EqualTo("65535"));
            Assert.That(maxValue.ToString("D5"), Is.EqualTo("65535"));

            Assert.That(maxValue.ToString("X"), Is.EqualTo("FFFF"));
            Assert.That(maxValue.ToString("x"), Is.EqualTo("ffff"));
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

        private static ushort maxValue = UInt16.MaxValue;
        private static ushort minValue = UInt16.MinValue;

        private static ushort[] ushortArray =
        {
            maxValue,
            (ushort)(maxValue + 1),
            (ushort)(maxValue + 10),
            (ushort)(maxValue + 100),
            (ushort)(maxValue - 1),
            (ushort)(maxValue - 10),
            (ushort)(maxValue - 100),
            minValue,
            (ushort)(minValue + 1),
            (ushort)(minValue + 10),
            (ushort)(minValue + 100),
            (ushort)(minValue - 1),
            (ushort)(minValue - 10),
            (ushort)(minValue - 100)
        };
    }
}