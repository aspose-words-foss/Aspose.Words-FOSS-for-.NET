// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/03/2024 by Ilya Navrotskiy

using System;
using System.Drawing;
using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests glow formatting.
    /// </summary>
    [TestFixture]
    public class TestGlowFormat
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WORDSNET-26670 Add support to get Glow effect of a Shape.
        /// Tests all glow properties getters and setters in DML shape.
        /// </summary>
        [Test]
        public void Test26670Dml()
        {
            Document doc = TestUtil.Open(@"Model\Shape\GlowFormat\Test26670Dml.docx");

            GlowFormat glow = doc.FirstSection.Body.Shapes[0].Glow;

            // Test all getters.
            TestUtil.CheckColor(Color.FromArgb(0x73FF6F00), glow.Color);
            Assert.That(glow.Transparency, Is.EqualTo(0.55));
            Assert.That(glow.Radius, Is.EqualTo(18.3));

            // Test setters.
            glow.Color = Color.OrangeRed;
            TestUtil.CheckColor(Color.FromArgb(unchecked((int)0xFFFF4500)), glow.Color);

            glow.Transparency = 0.37;
            Assert.That(glow.Transparency, Is.EqualTo(0.37));

            glow.Radius = 40.8;
            Assert.That(glow.Radius, Is.EqualTo(40.8));

            // Roundtrip and check again.
            doc = TestUtil.SaveOpen(doc, @"Model\Shape\GlowFormat\Test26670Dml.docx");
            glow = doc.FirstSection.Body.Shapes[0].Glow;
            TestUtil.CheckColor(Color.FromArgb(unchecked((int)0xA1FF4500)), glow.Color);
            Assert.That(glow.Transparency, Is.EqualTo(0.37));
            Assert.That(glow.Radius, Is.EqualTo(40.8));
        }

        /// <summary>
        /// Relates to WORDSNET-26670.
        /// Tests expected exception in VML shape.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Object doesn't support this action.")]
        public void Test26670VmlColor()
        {
            // FOSS: Doc reader removed; input converted to .docx. Word keeps the legacy shape as VML, which
            // still doesn't support Glow, so accessing Glow.Color throws as before.
            Document doc = TestUtil.Open(@"Model\Shape\GlowFormat\Test26670Vml.docx");

            Shape shape = doc.FirstSection.Body.Shapes[0];
            Assert.That(shape.Glow.Color, Is.Null);
        }

        /// <summary>
        /// Relates to WORDSNET-26670.
        /// Tests remove glow.
        /// </summary>
        [Test]
        public void TestRemove()
        {
            Document doc = TestUtil.Open(@"Model\Shape\GlowFormat\TestRemove.docx");

            Shape shape = doc.FirstSection.Body.Shapes[0];
            GlowFormat glow = shape.Glow;
            TestUtil.CheckColor(Color.FromArgb(unchecked((int)0xD9FA8072)), glow.Color);
            Assert.That(glow.Transparency, Is.EqualTo(0.15).Within(0.000001));
            Assert.That(glow.Radius, Is.EqualTo(30));

            glow.Remove();
            // Check properties and then roundtrip to ensure that
            // access to non-existent glow does not create it.
            TestUtil.CheckColor(Color.Black, shape.Glow.Color);
            Assert.That(shape.Glow.Transparency, Is.EqualTo(0.0));
            Assert.That(shape.Glow.Radius, Is.EqualTo(0.0));

            // Roundtrip and check again.
            doc = TestUtil.SaveOpen(doc, @"Model\Shape\GlowFormat\TestRemove.docx");
            shape = doc.FirstSection.Body.Shapes[0];
            TestUtil.CheckColor(Color.Black, shape.Glow.Color);
            Assert.That(shape.Glow.Transparency, Is.EqualTo(0.0));
            Assert.That(shape.Glow.Radius, Is.EqualTo(0.0));
        }

    }
}
