// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/05/2017 by Anatoliy Sidorenko

using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Number
{
    [TestFixture]
    public class TestFormatShort
    {
        [Test]
        public void TestBasic()
        {
            short shortValue = 0x1FFF;
            Assert.That(shortValue.ToString(), Is.EqualTo("8191"));
            Assert.That(shortValue.ToString("D2"), Is.EqualTo("8191"));
            Assert.That(shortValue.ToString("d2"), Is.EqualTo("8191"));
            Assert.That(shortValue.ToString("D1"), Is.EqualTo("8191"));
            Assert.That(shortValue.ToString("d1"), Is.EqualTo("8191"));
            Assert.That(shortValue.ToString("X"), Is.EqualTo("1FFF"));
            Assert.That(shortValue.ToString("x"), Is.EqualTo("1fff"));
            Assert.That(shortValue.ToString("X1"), Is.EqualTo("1FFF"));
            Assert.That(shortValue.ToString("x1"), Is.EqualTo("1fff"));
            Assert.That(shortValue.ToString("G"), Is.EqualTo("8191"));
            Assert.That(shortValue.ToString("g"), Is.EqualTo("8191"));
        }

        [Test]
        public void TestMinusSign()
        {
            short shortValue = -3156;
            Assert.That(shortValue.ToString("D2"), Is.EqualTo("-3156"));
            Assert.That(shortValue.ToString("d2"), Is.EqualTo("-3156"));
            Assert.That(shortValue.ToString("D1"), Is.EqualTo("-3156"));
            Assert.That(shortValue.ToString("d1"), Is.EqualTo("-3156"));
            Assert.That(shortValue.ToString("D"), Is.EqualTo("-3156"));
            Assert.That(shortValue.ToString("d"), Is.EqualTo("-3156"));
            Assert.That(shortValue.ToString("G"), Is.EqualTo("-3156"));
            Assert.That(shortValue.ToString("g"), Is.EqualTo("-3156"));
        }

        [Test]
        public void TestZeroPad()
        {
            short shortValue = 3156;
            short shortValue1 = -3156;
            Assert.That(shortValue.ToString("D5"), Is.EqualTo("03156"));
            Assert.That(shortValue.ToString("d5"), Is.EqualTo("03156"));
            Assert.That(shortValue1.ToString("d5"), Is.EqualTo("-03156"));

            Assert.That(shortValue.ToString("D10"), Is.EqualTo("0000003156"));
            Assert.That(shortValue.ToString("d10"), Is.EqualTo("0000003156"));
            Assert.That(shortValue1.ToString("d10"), Is.EqualTo("-0000003156"));

            Assert.That(shortValue.ToString("X5"), Is.EqualTo("00C54"));
            Assert.That(shortValue.ToString("x5"), Is.EqualTo("00c54"));
            Assert.That(shortValue1.ToString("x5"), Is.EqualTo("0f3ac"));
        }

        [Test]
        public void TestShortArray()
        {
            Assert.That(shortArray[3].ToString(), Is.EqualTo("-32669"));
            Assert.That(shortArray[4].ToString("d7"), Is.EqualTo("0032766"));

            Assert.That(shortArray[2].ToString("X2"), Is.EqualTo("8009"));
            Assert.That(shortArray[4].ToString("x"), Is.EqualTo("7ffe"));
        }

        [Test]
        public void TestBorder()
        {
            Assert.That(shortArray[0].ToString(), Is.EqualTo("32767"));
            Assert.That(shortArray[1].ToString(), Is.EqualTo("-32768"));
            Assert.That(shortArray[2].ToString(), Is.EqualTo("-32759"));
            Assert.That(shortArray[3].ToString(), Is.EqualTo("-32669"));
            Assert.That(shortArray[4].ToString(), Is.EqualTo("32766"));
            Assert.That(shortArray[5].ToString(), Is.EqualTo("32757"));
            Assert.That(shortArray[6].ToString(), Is.EqualTo("32667"));
            Assert.That(shortArray[7].ToString(), Is.EqualTo("-32768"));
            Assert.That(shortArray[8].ToString(), Is.EqualTo("-32767"));
            Assert.That(shortArray[9].ToString(), Is.EqualTo("-32758"));
            Assert.That(shortArray[10].ToString(), Is.EqualTo("-32668"));
            Assert.That(shortArray[11].ToString(), Is.EqualTo("32767"));
            Assert.That(shortArray[12].ToString(), Is.EqualTo("32758"));
            Assert.That(shortArray[13].ToString(), Is.EqualTo("32668"));
        }

        [Test]
        public void TestMax()
        {
            Assert.That(maxValue.ToString(), Is.EqualTo("32767"));
            Assert.That(maxValue.ToString("D"), Is.EqualTo("32767"));
            Assert.That(maxValue.ToString("d"), Is.EqualTo("32767"));
            Assert.That(maxValue.ToString("G"), Is.EqualTo("32767"));
            Assert.That(maxValue.ToString("g"), Is.EqualTo("32767"));

            Assert.That(maxValue.ToString("D1"), Is.EqualTo("32767"));
            Assert.That(maxValue.ToString("d2"), Is.EqualTo("32767"));
            Assert.That(maxValue.ToString("D7"), Is.EqualTo("0032767"));

            Assert.That(maxValue.ToString("X"), Is.EqualTo("7FFF"));
            Assert.That(maxValue.ToString("x"), Is.EqualTo("7fff"));
        }

        [Test]
        public void TestMin()
        {
            Assert.That(minValue.ToString(), Is.EqualTo("-32768"));
            Assert.That(minValue.ToString("D"), Is.EqualTo("-32768"));
            Assert.That(minValue.ToString("d"), Is.EqualTo("-32768"));
            Assert.That(minValue.ToString("G"), Is.EqualTo("-32768"));
            Assert.That(minValue.ToString("g"), Is.EqualTo("-32768"));

            Assert.That(minValue.ToString("D1"), Is.EqualTo("-32768"));
            Assert.That(minValue.ToString("d2"), Is.EqualTo("-32768"));
            Assert.That(minValue.ToString("D7"), Is.EqualTo("-0032768"));

            Assert.That(minValue.ToString("X"), Is.EqualTo("8000"));
            Assert.That(minValue.ToString("x"), Is.EqualTo("8000"));
        }

        private static short maxValue = short.MaxValue;
        private static short minValue = short.MinValue;

        private static short[] shortArray =
        {
            maxValue,
            (short)(maxValue + 1),
            (short)(maxValue + 10),
            (short)(maxValue + 100),
            (short)(maxValue - 1),
            (short)(maxValue - 10),
            (short)(maxValue - 100),
            minValue,
            (short)(minValue + 1),
            (short)(minValue + 10),
            (short)(minValue + 100),
            (short)(minValue - 1),
            (short)(minValue - 10),
            (short)(minValue - 100)
        };
    }
}
