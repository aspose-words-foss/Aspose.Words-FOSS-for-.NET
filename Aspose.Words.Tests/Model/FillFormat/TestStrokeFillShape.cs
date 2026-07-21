// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2023 by Ilya Navrotskiy

using System;
using System.Drawing;
using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing stroke fill of the Shape.
    /// </summary>
    [TestFixture]
    public class TestStrokeFillShape : TestFillFormatBase
    {
        /// <summary>
        /// Tests Solid fill.
        /// </summary>
        [TestCase("TestSolidStrokeFill.docx")]
        public void TestSolid(string testName)
        {
            Document doc = TestUtil.Open(string.Format("{0}{1}", TestDir, testName));
            Shape shape = doc.FirstSection.Body.Shapes[0];

            Fill fill = shape.Stroke.Fill;

            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(fill.Visible, Is.True);
            Assert.That(fill.Opacity, Is.EqualTo(0.93).Within(0.01));

            CheckColor(Color.FromArgb(0xFF, 0x00, 0x00), fill.ForeColor);
        }

        /// <summary>
        /// Tests Gradient fill (DML).
        /// </summary>
        [Test]
        public void TestGradientDml()
        {
            Document doc = TestUtil.Open(string.Format("{0}TestGradientStrokeFill.docx", TestDir));
            Shape shape = doc.FirstSection.Body.Shapes[0];

            Fill fill = shape.Stroke.Fill;

            Assert.That(fill.FillType, Is.EqualTo(FillType.Gradient));
            Assert.That(fill.Visible, Is.True);
            Assert.That(fill.Opacity, Is.EqualTo(0.93).Within(0.01));

            GradientStopCollection gradients = fill.GradientStops;

            Assert.That(gradients.Count, Is.EqualTo(5));
            CheckGradientStop(gradients[0], Color.FromArgb(0xFF, 0x99, 0x00), 0.37, 0.07);
            CheckGradientStop(gradients[1], Color.FromArgb(0xFF, 0xFF, 0xFF), 0.0, 0.0);
            CheckGradientStop(gradients[2], Color.FromArgb(0x99, 0xCC, 0xFF), 0.74, 0.0);
            CheckGradientStop(gradients[3], Color.FromArgb(0x00, 0x00, 0xFF), 0.83, 0.37);
            CheckGradientStop(gradients[4], Color.FromArgb(0xFF, 0xFF, 0x00), 1.0, 0.00);
        }

        /// <summary>
        /// Tests NoFill.
        /// </summary>
        [TestCase("TestNoFillStroke.docx")] // DML
        public void TestNoFill(string testName)
        {
            Document doc = TestUtil.Open(string.Format("{0}{1}", TestDir, testName));
            Shape shape = doc.FirstSection.Body.Shapes[0];

            Fill fill = shape.Stroke.Fill;

            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(fill.Visible, Is.False);

            CheckColor(Color.Empty, fill.ForeColor);
        }

        private const string TestDir = @"Model\Shape\Stroke\Fill\";
    }
}
