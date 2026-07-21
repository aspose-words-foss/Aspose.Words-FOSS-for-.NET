// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2017 by Anatoliy Sidorenko

using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Number
{
    [TestFixture]
    public class TestFormatSByte
    {
        [Test]
        public void TestBasic()
        {
            sbyte sbyteValue = 0x1F;
            Assert.That(sbyteValue.ToString(), Is.EqualTo("31"));
            Assert.That(sbyteValue.ToString("D2"), Is.EqualTo("31"));
            Assert.That(sbyteValue.ToString("d2"), Is.EqualTo("31"));
            Assert.That(sbyteValue.ToString("D1"), Is.EqualTo("31"));
            Assert.That(sbyteValue.ToString("d1"), Is.EqualTo("31"));
            Assert.That(sbyteValue.ToString("X"), Is.EqualTo("1F"));
            Assert.That(sbyteValue.ToString("x"), Is.EqualTo("1f"));
            Assert.That(sbyteValue.ToString("X1"), Is.EqualTo("1F"));
            Assert.That(sbyteValue.ToString("x1"), Is.EqualTo("1f"));
            Assert.That(sbyteValue.ToString("G"), Is.EqualTo("31"));
            Assert.That(sbyteValue.ToString("g"), Is.EqualTo("31"));
        }

        [Test]
        public void TestMinusSign()
        {
            sbyte sbyteValue = -31;
            Assert.That(sbyteValue.ToString("D2"), Is.EqualTo("-31"));
            Assert.That(sbyteValue.ToString("d2"), Is.EqualTo("-31"));
            Assert.That(sbyteValue.ToString("D1"), Is.EqualTo("-31"));
            Assert.That(sbyteValue.ToString("d1"), Is.EqualTo("-31"));
            Assert.That(sbyteValue.ToString("D"), Is.EqualTo("-31"));
            Assert.That(sbyteValue.ToString("d"), Is.EqualTo("-31"));
            Assert.That(sbyteValue.ToString("G"), Is.EqualTo("-31"));
            Assert.That(sbyteValue.ToString("g"), Is.EqualTo("-31"));
        }

        [Test]
        public void TestZeroPad()
        {
            sbyte sbyteValue = 31;
            sbyte sbyteValue1 = -31;
            Assert.That(sbyteValue.ToString("D5"), Is.EqualTo("00031"));
            Assert.That(sbyteValue.ToString("d5"), Is.EqualTo("00031"));
            Assert.That(sbyteValue1.ToString("d5"), Is.EqualTo("-00031"));

            Assert.That(sbyteValue.ToString("D10"), Is.EqualTo("0000000031"));
            Assert.That(sbyteValue.ToString("d10"), Is.EqualTo("0000000031"));
            Assert.That(sbyteValue1.ToString("d10"), Is.EqualTo("-0000000031"));

            Assert.That(sbyteValue.ToString("X5"), Is.EqualTo("0001F"));
            Assert.That(sbyteValue.ToString("x5"), Is.EqualTo("0001f"));
            Assert.That(sbyteValue1.ToString("x5"), Is.EqualTo("000e1"));
        }

        [Test]
        public void TestSByteArray()
        {
            Assert.That(sbyteArray[3].ToString(), Is.EqualTo("-29"));
            Assert.That(sbyteArray[4].ToString("d5"), Is.EqualTo("00126"));

            Assert.That(sbyteArray[2].ToString("X2"), Is.EqualTo("89"));
            Assert.That(sbyteArray[4].ToString("x"), Is.EqualTo("7e"));
        }

        [Test]
        public void TestBorder()
        {
            Assert.That(sbyteArray[0].ToString(), Is.EqualTo("127"));
            Assert.That(sbyteArray[1].ToString(), Is.EqualTo("-128"));
            Assert.That(sbyteArray[2].ToString(), Is.EqualTo("-119"));
            Assert.That(sbyteArray[3].ToString(), Is.EqualTo("-29"));
            Assert.That(sbyteArray[4].ToString(), Is.EqualTo("126"));
            Assert.That(sbyteArray[5].ToString(), Is.EqualTo("117"));
            Assert.That(sbyteArray[6].ToString(), Is.EqualTo("27"));
            Assert.That(sbyteArray[7].ToString(), Is.EqualTo("-128"));
            Assert.That(sbyteArray[8].ToString(), Is.EqualTo("-127"));
            Assert.That(sbyteArray[9].ToString(), Is.EqualTo("-118"));
            Assert.That(sbyteArray[10].ToString(), Is.EqualTo("-28"));
            Assert.That(sbyteArray[11].ToString(), Is.EqualTo("127"));
            Assert.That(sbyteArray[12].ToString(), Is.EqualTo("118"));
            Assert.That(sbyteArray[13].ToString(), Is.EqualTo("28"));
        }

        [Test]
        public void TestMax()
        {
            Assert.That(maxValue.ToString(), Is.EqualTo("127"));
            Assert.That(maxValue.ToString("D"), Is.EqualTo("127"));
            Assert.That(maxValue.ToString("d"), Is.EqualTo("127"));
            Assert.That(maxValue.ToString("G"), Is.EqualTo("127"));
            Assert.That(maxValue.ToString("g"), Is.EqualTo("127"));

            Assert.That(maxValue.ToString("D1"), Is.EqualTo("127"));
            Assert.That(maxValue.ToString("d2"), Is.EqualTo("127"));
            Assert.That(maxValue.ToString("D5"), Is.EqualTo("00127"));

            Assert.That(maxValue.ToString("X"), Is.EqualTo("7F"));
            Assert.That(maxValue.ToString("x"), Is.EqualTo("7f"));
        }

        [Test]
        public void TestMin()
        {
            Assert.That(minValue.ToString(), Is.EqualTo("-128"));
            Assert.That(minValue.ToString("D"), Is.EqualTo("-128"));
            Assert.That(minValue.ToString("d"), Is.EqualTo("-128"));
            Assert.That(minValue.ToString("G"), Is.EqualTo("-128"));
            Assert.That(minValue.ToString("g"), Is.EqualTo("-128"));

            Assert.That(minValue.ToString("D1"), Is.EqualTo("-128"));
            Assert.That(minValue.ToString("d2"), Is.EqualTo("-128"));
            Assert.That(minValue.ToString("D5"), Is.EqualTo("-00128"));

            Assert.That(minValue.ToString("X"), Is.EqualTo("80"));
            Assert.That(minValue.ToString("x"), Is.EqualTo("80"));
        }

        private static sbyte maxValue = sbyte.MaxValue;
        private static sbyte minValue = sbyte.MinValue;

        private static sbyte[] sbyteArray =
        {
            maxValue,
            (sbyte)(maxValue + 1),
            (sbyte)(maxValue + 10),
            (sbyte)(maxValue + 100),
            (sbyte)(maxValue - 1),
            (sbyte)(maxValue - 10),
            (sbyte)(maxValue - 100),
            minValue,
            (sbyte)(minValue + 1),
            (sbyte)(minValue + 10),
            (sbyte)(minValue + 100),
            (sbyte)(minValue - 1),
            (sbyte)(minValue - 10),
            (sbyte)(minValue - 100)
        };
    }
}
