// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/05/2018 by Alexander Zhiltsov

using System;
using Aspose.Words.Drawing.Charts;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Charts
{
    /// <summary>
    /// Tests members of the <see cref="AxisBound"/> class.
    /// </summary>
    [TestFixture]
    public class TestAxisBound
    {
        [Test]
        public void TestCreation()
        {
            AxisBound bound = new AxisBound(1d);
            Assert.That(bound.IsAuto, Is.False);
            Assert.That(bound.Value, Is.EqualTo(1d));
            Assert.That(bound.ValueAsDate, Is.EqualTo(new DateTime(1899, 12, 31)));

            DateTime datetime = new DateTime(2018, 5, 25);
            bound = new AxisBound(datetime);
            Assert.That(bound.IsAuto, Is.False);
            Assert.That(bound.Value, Is.EqualTo(datetime.ToOADate()));
            Assert.That(bound.ValueAsDate, Is.EqualTo(datetime));

            Assert.That(new AxisBound().IsAuto, Is.True);
        }

        [Test]
        public void TestEquals()
        {
            AxisBound bound1 = new AxisBound(1d);
            AxisBound bound2 = new AxisBound(1d);
            AxisBound bound3 = new AxisBound(1.00001d);
            Assert.That(bound1.Equals(bound2), Is.True);
            Assert.That(bound2.Equals(bound1), Is.True);
            Assert.That(bound1.Equals(bound3), Is.False);
            Assert.That(bound3.Equals(bound1), Is.False);
            Assert.That(bound1.Equals(new AxisBound()), Is.False);

            bound1 = new AxisBound(DateTime.Today);
            bound2 = new AxisBound(DateTime.Today);
            Assert.That(bound1.Equals(bound2), Is.True);
            Assert.That(bound2.Equals(bound1), Is.True);
            Assert.That(bound1.Equals(new AxisBound()), Is.False);

            bound1 = new AxisBound();
            bound2 = new AxisBound();
            Assert.That(bound1.Equals(bound2), Is.True);
            Assert.That(bound2.Equals(bound1), Is.True);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCreationWithWrongDate()
        {
            new AxisBound(DateTime.MinValue);
        }

        [Test]
        public void TestOverflowAsDate()
        {
            AxisBound bound1 = new AxisBound(10000000);
            Assert.That(bound1.ValueAsDate, Is.EqualTo(DateTime.MinValue));

            AxisBound bound2 = new AxisBound(-10000000);
            Assert.That(bound2.ValueAsDate, Is.EqualTo(DateTime.MinValue));
        }
    }
}
