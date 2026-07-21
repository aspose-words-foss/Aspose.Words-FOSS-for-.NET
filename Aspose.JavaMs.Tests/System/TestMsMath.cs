// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2010 by Konstantin Sidorenko
// 13/01/2016 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestMsMath
    {
        [Test]
        public void TestRound()
        {
            Assert.That(0d, Is.EqualTo(Math.Round(0d, 0, MidpointRounding.AwayFromZero)));
            Assert.That(-1d, Is.EqualTo(Math.Round(-0.5d, 0, MidpointRounding.AwayFromZero)));
            Assert.That(1d, Is.EqualTo(Math.Round(0.5d, 0, MidpointRounding.AwayFromZero)));
            Assert.That(0.2d, Is.EqualTo(Math.Round(0.19999d, 2, MidpointRounding.AwayFromZero)));
            Assert.That(-0.2d, Is.EqualTo(Math.Round(-0.19999d, 2, MidpointRounding.AwayFromZero)));
            Assert.That(1.5d, Is.EqualTo(Math.Round(1.5d, 2, MidpointRounding.AwayFromZero)));
            Assert.That(-1.5d, Is.EqualTo(Math.Round(-1.5d, 2, MidpointRounding.AwayFromZero)));
            Assert.That(2d, Is.EqualTo(Math.Round(1.5d, 0, MidpointRounding.AwayFromZero)));
            Assert.That(-2d, Is.EqualTo(Math.Round(-1.5d, 0, MidpointRounding.AwayFromZero)));
            Assert.That(2.4d, Is.EqualTo(Math.Round(2.4d, 1, MidpointRounding.AwayFromZero)));
            Assert.That(-2.4d, Is.EqualTo(Math.Round(-2.4d, 1, MidpointRounding.AwayFromZero)));
            Assert.That(2d, Is.EqualTo(Math.Round(2.4d, 0, MidpointRounding.AwayFromZero)));
            Assert.That(-2d, Is.EqualTo(Math.Round(-2.4d, 0, MidpointRounding.AwayFromZero)));
        }

        [Test]
        public void TestRoundInfinity()
        {
            double roundPositive = Math.Round(double.PositiveInfinity);
            double roundNegative = Math.Round(double.NegativeInfinity);

            Assert.That(double.PositiveInfinity, Is.EqualTo(Math.Round(double.PositiveInfinity)));
            Assert.That(double.NegativeInfinity, Is.EqualTo(Math.Round(double.NegativeInfinity)));
            Assert.That(double.NaN, Is.EqualTo(Math.Round(double.NaN)));
        }

        [Test]
        public void TestMathPrecession()
        {
            Assert.That(0, Is.EqualTo(MathUtil.GetPrecision(0.0)));
            Assert.That(0, Is.EqualTo(MathUtil.GetPrecision(5.0)));
            Assert.That(-16, Is.EqualTo(MathUtil.GetPrecision(1.0E-15)));
            Assert.That(-21, Is.EqualTo(MathUtil.GetPrecision(1.0E-20)));
        }
    }
}
