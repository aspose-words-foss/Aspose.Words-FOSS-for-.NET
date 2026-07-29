// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2012 by Konstantin Sidorenko
// 04/05/2016 by Anatoliy Sidorenko

using System.Drawing;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Drawing
{
    [TestFixture]
    public class TestPointF
    {
        [Test]
        public void TestCtor()
        {
            TestCtor(-758, -6545);
            TestCtor(22348, -6545);
            TestCtor(0, -6545);
            TestCtor(-6545, 0);
            TestCtor(0, 0);
            TestCtor(0, float.NaN);
            TestCtor(float.NaN, 0);
            TestCtor(float.NegativeInfinity, 0);
            TestCtor(0, float.NegativeInfinity);
            TestCtor(float.PositiveInfinity, 0);
            TestCtor(0, float.PositiveInfinity);
            TestCtor(float.MinValue, 0);
            TestCtor(0, float.MinValue);
            TestCtor(float.MaxValue, 0);
            TestCtor(0, float.MaxValue);
            TestCtor(float.MaxValue, float.MaxValue);
            TestCtor(float.MinValue, float.MinValue);
            TestCtor(float.MaxValue, float.MinValue);
            TestCtor(float.MinValue, float.MaxValue);
        }

        private void TestCtor(float x, float y)
        {
            PointF point = new PointF(x, y);
            Assert.That(x, Is.EqualTo(point.X));
            Assert.That(y, Is.EqualTo(point.Y));
        }
    }
}
