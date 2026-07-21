// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/08/2013 by Roman Korchagin

using System;
using System.Drawing;
using System.Globalization;
using Aspose.Drawing;
using NUnit.Framework;

namespace Aspose.Tests.Base
{
    [TestFixture]
    public class TestMathUtil
    {
        [TestCase(0.0, 0.0, "0.0")]
        [TestCase(-0.0, 0.0, "0.0")]
        [TestCase(5.0, 5.0, "5.0")]
        [TestCase(-5.0, -5.0, "-5.0")]
        [TestCase(-9.99, -9.99, "-10.0")]
        [TestCase(5.97, 5.97, "6.0")]
        [TestCase(9.99, 9.99, "10.0")]
        [TestCase(12345.6789, 12345.6789, "12345.7")]
        public void TestNormalizeZeroDouble(double input, double expected, string formatted)
        {
            Assert.That(MathUtil.NormalizeZero(input), Is.EqualTo(expected));
            Assert.That(MathUtil.NormalizeZero(input).ToString("F1", CultureInfo.InvariantCulture), Is.EqualTo(formatted));
        }

        [TestCase(0.0f, 0.0f, "0.0")]
        [TestCase(-0.0f, 0.0f, "0.0")]
        [TestCase(5.0f, 5.0f, "5.0")]
        [TestCase(-5.0f, -5.0f, "-5.0")]
        [TestCase(-9.99f, -9.99f, "-10.0")]
        [TestCase(5.97f, 5.97f, "6.0")]
        [TestCase(9.99f, 9.99f, "10.0")]
        [TestCase(12345.6789f, 12345.6789f, "12345.7")]
        public void TestNormalizeZeroFloat(float input, float expected, string formatted)
        {
            Assert.That(MathUtil.NormalizeZero(input), Is.EqualTo(expected));
            Assert.That(MathUtil.NormalizeZero(input).ToString("F1", CultureInfo.InvariantCulture), Is.EqualTo(formatted));
        }

#if NETSTANDARD
        // Starting with .NET Framework 4.5, the formatting of the number -0.0 is 
        // preserved when converting it to a string. In earlier versions, -0.0 was 
        // formatted as "0.0".
        [Test]
        public void TestSignedZeroDefaultString()
        {
            Assert.That(((double)(-0.0)).ToString("F1", CultureInfo.InvariantCulture), Is.EqualTo("-0.0"));
            Assert.That(((float)(-0.0)).ToString("F1", CultureInfo.InvariantCulture), Is.EqualTo("-0.0"));
        }
#endif

        /// <summary>
        /// Tests <see cref="MathUtil.IsZero(double)"/> method.
        /// </summary>
        [Test]
        public void TestIsZeroDouble()
        {
            Assert.That(MathUtil.IsZero((double)0), Is.True); // casting for C++
            Assert.That(MathUtil.IsZero(0.0d), Is.True);
            Assert.That(MathUtil.IsZero(double.Epsilon / 2), Is.True);
            Assert.That(MathUtil.IsZero(-double.Epsilon / 2), Is.True);
            Assert.That(MathUtil.IsZero((double)-1), Is.False);  // casting for C++
            Assert.That(MathUtil.IsZero(double.Epsilon * 2), Is.False);
        }

        /// <summary>
        /// Tests <see cref="MathUtil.AreEqual(double,double)"/> method.
        /// </summary>
        [Test]
        public void TestDoubleAreEqual()
        {
            // This is tolerance used by default in this method. 
            const double tolerance = 1e-10;

            TestDoubleAreEqualCore(123456.789, tolerance);
            TestDoubleAreEqualCore(-123456.789, tolerance);
            TestDoubleAreEqualCore(double.Epsilon, tolerance);
            TestDoubleAreEqualCore(0.0d, tolerance);
        }

        private static void TestDoubleAreEqualCore(double testValue1, double tolerance)
        {
            Assert.That(MathUtil.AreEqual(testValue1, testValue1), Is.True);
            Assert.That(MathUtil.AreEqual(testValue1, (testValue1 - tolerance / 2)), Is.True);
            Assert.That(MathUtil.AreEqual(testValue1, (testValue1 + tolerance / 2)), Is.True);
            Assert.That(MathUtil.AreEqual(testValue1, (testValue1 - tolerance)), Is.False);
            Assert.That(MathUtil.AreEqual(testValue1, (testValue1 + tolerance)), Is.False);
        }

        [Test]
        public void TestPolyline()
        {
            Point[] points = new Point[] { new Point(100, 50), new Point(200, 70), new Point(300, 150), new Point(400, 120) };

            int y = MathUtil.GetPolylineY(30, points);
            Assert.That(y, Is.EqualTo(0));

            y = MathUtil.GetPolylineY(450, points);
            Assert.That(y, Is.EqualTo(105));

            y = MathUtil.GetPolylineY(100, points);
            Assert.That(y, Is.EqualTo(50));

            y = MathUtil.GetPolylineY(300, points);
            Assert.That(y, Is.EqualTo(150));

            y = MathUtil.GetPolylineY(400, points);
            Assert.That(y, Is.EqualTo(120));

            y = MathUtil.GetPolylineY(150, points);
            Assert.That(y, Is.EqualTo(60));

            y = MathUtil.GetPolylineY(350, points);
            Assert.That(y, Is.EqualTo(135));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPolyline2()
        {
            Point[] points = new Point[] { new Point(100, 50), new Point(200, 70), new Point(200, 150), new Point(300, 120) };

            int y = MathUtil.GetPolylineY(300, points);
            Assert.That(y, Is.EqualTo(0));
        }

        [Test]
        public void TestRoundingDivision()
        {
            Assert.That(MathUtil.Divide(4, 2), Is.EqualTo(2));
            Assert.That(MathUtil.Divide(3, 2), Is.EqualTo(2));
            Assert.That(MathUtil.Divide(5, 2), Is.EqualTo(3));
            Assert.That(MathUtil.Divide(499, 1000), Is.EqualTo(0));
            Assert.That(MathUtil.Divide(499, 999), Is.EqualTo(0));
            Assert.That(MathUtil.Divide(500, 1000), Is.EqualTo(1));
        }

        /// <summary>
        /// Test case from metafile in TestJira12446.
        /// </summary>
        [Test]
        public void TestDecomposeMatrixBug()
        {
            DrMatrix matrix = new DrMatrix(9.5229E-17f, 1.138952f, -1.5521f, 6.974E-17f, 975.2505f, 431.4603f);
            float[] components = matrix.DecomposeMatrix();
            Assert.That(components[0], Is.EqualTo(1.138952f));
            Assert.That(components[1], Is.EqualTo(1.5521f));
            Assert.That(components[3], Is.EqualTo(90f));
            Assert.That(components[4], Is.EqualTo(975.2505f));
            Assert.That(components[5], Is.EqualTo(431.4603f));
        }

        [Test]
        [TestCase(3, 6)]
        [TestCase(4, 24)]
        [TestCase(5, 120)]
        [TestCase(11, 39916800)]
        [TestCase(12, 479001600)]
        public void TestFectorial(int n, int result)
        {
            Assert.That(MathUtil.Factorial(n), Is.EqualTo(result));
        }

        /// <summary>
        /// WORDSJAVA-2352 '(int)double.PositiveInfinity' returns int.MinValue on .Net. 
        /// '(int)double.MaxValue', '(int)double.NaN' - the same.
        /// </summary>
        [Test]
        public void TestDoubleToInt()
        {
            Assert.That(MathUtil.DoubleToInt(double.PositiveInfinity), Is.EqualTo(int.MinValue));
            Assert.That(MathUtil.DoubleToInt(double.NegativeInfinity), Is.EqualTo(int.MinValue));

            Assert.That(MathUtil.DoubleToInt(double.MaxValue), Is.EqualTo(int.MinValue));
            Assert.That(MathUtil.DoubleToInt(double.MinValue), Is.EqualTo(int.MinValue));

            Assert.That(MathUtil.DoubleToInt(double.NaN), Is.EqualTo(int.MinValue));
        }
    }
}
