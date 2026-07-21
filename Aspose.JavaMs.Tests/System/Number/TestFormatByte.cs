// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/02/2017 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Number
{
    [TestFixture]
    public class TestFormatByte
    {
        [Test]
        public void TestBasic()
        {
            byte byteValue = 0x1F;
            Assert.That(byteValue.ToString(), Is.EqualTo("31"));
            Assert.That(byteValue.ToString("D2"), Is.EqualTo("31"));
            Assert.That(byteValue.ToString("d2"), Is.EqualTo("31"));
            Assert.That(byteValue.ToString("D1"), Is.EqualTo("31"));
            Assert.That(byteValue.ToString("d1"), Is.EqualTo("31"));
            Assert.That(byteValue.ToString("X"), Is.EqualTo("1F"));
            Assert.That(byteValue.ToString("x"), Is.EqualTo("1f"));
            Assert.That(byteValue.ToString("X1"), Is.EqualTo("1F"));
            Assert.That(byteValue.ToString("x1"), Is.EqualTo("1f"));
            Assert.That(byteValue.ToString("G2"), Is.EqualTo("31"));
            Assert.That(byteValue.ToString("g2"), Is.EqualTo("31"));
            byteValue = 0xF1;
            Assert.That(byteValue.ToString(), Is.EqualTo("241"));
            Assert.That(byteValue.ToString("D2"), Is.EqualTo("241"));
            Assert.That(byteValue.ToString("d2"), Is.EqualTo("241"));
            Assert.That(byteValue.ToString("D1"), Is.EqualTo("241"));
            Assert.That(byteValue.ToString("d1"), Is.EqualTo("241"));
            Assert.That(byteValue.ToString("X"), Is.EqualTo("F1"));
            Assert.That(byteValue.ToString("x"), Is.EqualTo("f1"));
            Assert.That(byteValue.ToString("X1"), Is.EqualTo("F1"));
            Assert.That(byteValue.ToString("x1"), Is.EqualTo("f1"));
            Assert.That(byteValue.ToString("G3"), Is.EqualTo("241"));
            Assert.That(byteValue.ToString("g3"), Is.EqualTo("241"));
        }

        [Test]
        public void TestZeroPad()
        {
            byte byteValue = 31;
            Assert.That(byteValue.ToString("D5"), Is.EqualTo("00031"));
            Assert.That(byteValue.ToString("d5"), Is.EqualTo("00031"));

            Assert.That(byteValue.ToString("X5"), Is.EqualTo("0001F"));
            Assert.That(byteValue.ToString("x5"), Is.EqualTo("0001f"));
        }

        [Test]
        public void TestByteArray()
        {
            Assert.That(byteArray[3].ToString(), Is.EqualTo("99"));
            Assert.That(byteArray[4].ToString("d5"), Is.EqualTo("00254"));

            Assert.That(byteArray[2].ToString("X2"), Is.EqualTo("09"));
            Assert.That(byteArray[4].ToString("x"), Is.EqualTo("fe"));
        }

        [Test]
        public void TestBorder()
        {
            Assert.That(byteArray[0].ToString(), Is.EqualTo("255"));
            Assert.That(byteArray[1].ToString(), Is.EqualTo("0"));
            Assert.That(byteArray[2].ToString(), Is.EqualTo("9"));
            Assert.That(byteArray[3].ToString(), Is.EqualTo("99"));
            Assert.That(byteArray[4].ToString(), Is.EqualTo("254"));
            Assert.That(byteArray[5].ToString(), Is.EqualTo("245"));
            Assert.That(byteArray[6].ToString(), Is.EqualTo("155"));
            Assert.That(byteArray[7].ToString(), Is.EqualTo("0"));
            Assert.That(byteArray[8].ToString(), Is.EqualTo("1"));
            Assert.That(byteArray[9].ToString(), Is.EqualTo("10"));
            Assert.That(byteArray[10].ToString(), Is.EqualTo("100"));
            Assert.That(byteArray[11].ToString(), Is.EqualTo("255"));
            Assert.That(byteArray[12].ToString(), Is.EqualTo("246"));
            Assert.That(byteArray[13].ToString(), Is.EqualTo("156"));
        }

        [Test]
        public void TestMax()
        {
            Assert.That(maxValue.ToString(), Is.EqualTo("255"));
            Assert.That(maxValue.ToString("D"), Is.EqualTo("255"));
            Assert.That(maxValue.ToString("d"), Is.EqualTo("255"));
            Assert.That(maxValue.ToString("G"), Is.EqualTo("255"));
            Assert.That(maxValue.ToString("g"), Is.EqualTo("255"));

            Assert.That(maxValue.ToString("D1"), Is.EqualTo("255"));
            Assert.That(maxValue.ToString("d2"), Is.EqualTo("255"));
            Assert.That(maxValue.ToString("D5"), Is.EqualTo("00255"));

            Assert.That(maxValue.ToString("X"), Is.EqualTo("FF"));
            Assert.That(maxValue.ToString("x"), Is.EqualTo("ff"));
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

        private static byte maxValue = Byte.MaxValue;
        private static byte minValue = Byte.MinValue;

        private static byte[] byteArray =
        {
            maxValue,
            (byte)(maxValue + 1),
            (byte)(maxValue + 10),
            (byte)(maxValue + 100),
            (byte)(maxValue - 1),
            (byte)(maxValue - 10),
            (byte)(maxValue - 100),
            minValue,
            (byte)(minValue + 1),
            (byte)(minValue + 10),
            (byte)(minValue + 100),
            (byte)(minValue - 1),
            (byte)(minValue - 10),
            (byte)(minValue - 100)
        };
    }
}