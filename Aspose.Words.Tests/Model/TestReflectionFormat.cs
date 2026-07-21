// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/03/2024 by Ilya Navrotskiy

using System;
using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests reflection formatting.
    /// </summary>
    [TestFixture]
    public class TestReflectionFormat
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WORDSNET-26671 Add support to get Reflection effect of a Shape.
        /// Tests all reflection properties getters and setters in DML shape.
        /// </summary>
        [Test]
        public void Test26671Dml()
        {
            Document doc = TestUtil.Open(@"Model\Shape\ReflectionFormat\Test26671Dml.docx");

            ReflectionFormat reflection = doc.FirstSection.Body.Shapes[0].Reflection;

            // Test all getters.
            Assert.That(reflection.Transparency, Is.EqualTo(0.55));
            Assert.That(reflection.Size, Is.EqualTo(0.26));
            Assert.That(reflection.Blur, Is.EqualTo(4.7));
            Assert.That(reflection.Distance, Is.EqualTo(12.7));

            // Test setters.
            reflection.Transparency = 0.37;
            Assert.That(reflection.Transparency, Is.EqualTo(0.37));

            reflection.Size = 0.48;
            Assert.That(reflection.Size, Is.EqualTo(0.48));

            reflection.Blur = 17.5;
            Assert.That(reflection.Blur, Is.EqualTo(17.5));

            reflection.Distance = 9.2;
            Assert.That(reflection.Distance, Is.EqualTo(9.2));

            // Roundtrip and check again.
            doc = TestUtil.SaveOpen(doc, @"Model\Shape\ReflectionFormat\Test26671Dml.docx");
            reflection = doc.FirstSection.Body.Shapes[0].Reflection;
            Assert.That(reflection.Transparency, Is.EqualTo(0.37));
            Assert.That(reflection.Size, Is.EqualTo(0.48));
            Assert.That(reflection.Blur, Is.EqualTo(17.5));
            Assert.That(reflection.Distance, Is.EqualTo(9.2));
        }

        // FOSS: Test26671Vml removed — loads a VML .doc to assert that Reflection is unsupported
        // on VML shapes. The Doc reader is removed, so the .doc can no longer load. The DML path
        // stays covered by Test26671Dml.

        /// <summary>
        /// Relates to WORDSNET-26671.
        /// Tests remove reflection.
        /// </summary>
        [Test]
        public void TestRemove()
        {
            Document doc = TestUtil.Open(@"Model\Shape\ReflectionFormat\TestRemove.docx");

            Shape shape = doc.FirstSection.Body.Shapes[0];
            ReflectionFormat reflection = shape.Reflection;
            Assert.That(reflection.Transparency, Is.EqualTo(0.55).Within(0.000001));
            Assert.That(reflection.Size, Is.EqualTo(0.68).Within(0.000001));
            Assert.That(reflection.Blur, Is.EqualTo(14));
            Assert.That(reflection.Distance, Is.EqualTo(6));

            reflection.Remove();
            // Check properties and then roundtrip to ensure that
            // access to non-existent reflection does not create it.
            Assert.That(shape.Reflection.Transparency, Is.EqualTo(0.0));
            Assert.That(shape.Reflection.Distance, Is.EqualTo(0.0));
            Assert.That(shape.Reflection.Size, Is.EqualTo(0.0));
            Assert.That(shape.Reflection.Blur, Is.EqualTo(0.0));

            // Roundtrip and check again.
            doc = TestUtil.SaveOpen(doc, @"Model\Shape\ReflectionFormat\TestRemove.docx");
            shape = doc.FirstSection.Body.Shapes[0];
            Assert.That(shape.Reflection.Transparency, Is.EqualTo(0.0));
            Assert.That(shape.Reflection.Distance, Is.EqualTo(0.0));
            Assert.That(shape.Reflection.Size, Is.EqualTo(0.0));
            Assert.That(shape.Reflection.Blur, Is.EqualTo(0.0));
        }

    }
}
