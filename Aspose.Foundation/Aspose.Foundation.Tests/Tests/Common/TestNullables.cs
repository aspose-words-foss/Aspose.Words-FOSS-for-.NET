// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/06/2020 by Edward Voronov

using System;
using Aspose.Common;
using NUnit.Framework;

namespace Aspose.Foundation.Tests.Tests.Common
{
    [TestFixture]
    public class TestNullables
    {
        [Test]
        public void TestHasNotValue()
        {
            NullableInt32 nullable = NullableInt32.Null;

            Assert.That(nullable.HasValue, Is.False);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "NullableInt32 doesn't have a value.")]
        public void TestValueThrowsException()
        {
            NullableInt32 nullable = NullableInt32.Null;

            int value = nullable.Value;
        }

        [Test]
        [TestCaseSource("TestCasesForNullables")]
        public void TestHasValue(int value)
        {
            NullableInt32 nullable = new NullableInt32(value);

            Assert.That(nullable.HasValue, Is.True);
        }

        [Test]
        [TestCaseSource("TestCasesForNullables")]
        public void TestValue(int value)
        {
            NullableInt32 nullable = new NullableInt32(value);

            Assert.That(nullable.Value, Is.EqualTo(value));
        }

        [Test]
        [TestCaseSource("TestCasesForNullables")]
        public void TestGetDefinedValue(int value)
        {
            NullableInt32 nullable = new NullableInt32(value);

            Assert.That(nullable.GetValueOrDefault(), Is.EqualTo(value));
        }

        [Test]
        public void TestGetDefaultValue()
        {
            NullableInt32 nullable = NullableInt32.Null;

            Assert.That(nullable.GetValueOrDefault(), Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource("TestCasesForNullables")]
        public void TestGetCustomDefaultValue(int value)
        {
            NullableInt32 nullable = NullableInt32.Null;

            Assert.That(nullable.GetValueOrDefault(value), Is.EqualTo(value));
        }

        [Test]
        [TestCaseSource("TestCasesForNullables")]
        public void TestEqualsToPrimitive(int value)
        {
            NullableInt32 nullable = new NullableInt32(value);

            Assert.That(nullable.Equals(value), Is.True);
        }

        [Test]
        [TestCaseSource("TestCasesForNullables")]
        public void TestEqualsToNullable(int value)
        {
            NullableInt32 nullable = new NullableInt32(value);
            NullableInt32 otherNullable = new NullableInt32(value);

            Assert.That(nullable.Equals(otherNullable), Is.True);
            Assert.That(otherNullable.Equals(nullable), Is.True);
        }

        [Test]
        public void TestNullEqualsToNull()
        {
            NullableInt32 nullable = NullableInt32.Null;
            NullableInt32 otherNullable = NullableInt32.Null;

            Assert.That(nullable.Equals(otherNullable), Is.True);
            Assert.That(otherNullable.Equals(nullable), Is.True);
        }

        [Test]
        [TestCaseSource("TestCasesForNullables")]
        public void TestNotEqualsToPrimitive(int value)
        {
            NullableInt32 nullable = new NullableInt32(value);

            Assert.That(nullable.Equals(7), Is.False);
        }

        [Test]
        [TestCaseSource("TestCasesForNullables")]
        public void TestNotEqualsToNullable(int value)
        {
            NullableInt32 nullable = new NullableInt32(value);
            NullableInt32 otherNullable = new NullableInt32(7);

            Assert.That(nullable.Equals(otherNullable), Is.False);
            Assert.That(otherNullable.Equals(nullable), Is.False);
        }

        [Test]
        [TestCaseSource("TestCasesForNullables")]
        public void TestNullNotEqualsToNotNull(int value)
        {
            NullableInt32 nullable = new NullableInt32(value);
            NullableInt32 otherNullable = NullableInt32.Null;

            Assert.That(nullable.Equals(otherNullable), Is.False);
            Assert.That(otherNullable.Equals(nullable), Is.False);
        }

        [Test]
        [TestCaseSource("TestCasesForNullables")]
        public void TestHashCode(int value)
        {
            NullableInt32 nullable = new NullableInt32(value);
            NullableInt32 otherNullable = new NullableInt32(value);

            Assert.That(nullable.GetHashCode() == otherNullable.GetHashCode(), Is.True);
        }

        [Test]
        public void TestNullHashCode()
        {
            NullableInt32 nullable = NullableInt32.Null;
            NullableInt32 otherNullable = NullableInt32.Null;

            Assert.That(nullable.GetHashCode() == otherNullable.GetHashCode(), Is.True);
        }

        public static readonly int[] TestCasesForNullables = { int.MinValue, int.MaxValue, 0, 95, -32 };
    }
}
