// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/03/2023 by Ilya Navrotskiy

using System;
using System.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Saving;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing stroke fill of the Chart.
    /// </summary>
    [TestFixture]
    public class TestStrokeFillChart : TestFillFormatBase
    {
        /// <summary>
        /// Tests Solid fill.
        /// </summary>
        [Test]
        public void TestSolid()
        {
            Document doc = TestUtil.Open(string.Format("{0}TestSolidStrokeFill.docx", TestDir));
            Shape shape = doc.FirstSection.Body.Shapes[0];

            Fill fill = shape.Chart.Series[0].DataPoints[0].Format.Stroke.Fill;

            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(fill.Visible, Is.True);
            Assert.That(fill.Opacity, Is.EqualTo(0.93).Within(0.01));

            CheckColor(Color.FromArgb(0xFF, 0x00, 0x00), fill.ForeColor);
        }

        /// <summary>
        /// Tests Gradient fill.
        /// </summary>
        [Test]
        public void TestGradient()
        {
            Document doc = TestUtil.Open(string.Format("{0}TestGradientStrokeFill.docx", TestDir));
            Shape shape = doc.FirstSection.Body.Shapes[0];

            Fill fill = shape.Chart.Series[0].DataPoints[0].Format.Stroke.Fill;

            Assert.That(fill.FillType, Is.EqualTo(FillType.Gradient));
            Assert.That(fill.Visible, Is.True);
            Assert.That(fill.Opacity, Is.EqualTo(0.91).Within(0.01));

            GradientStopCollection gradients = fill.GradientStops;

            Assert.That(gradients.Count, Is.EqualTo(3));
            CheckGradientStop(gradients[0], Color.White, 0.0, 0.09);
            CheckGradientStop(gradients[1], Color.Red, 0.4, 0.07);
            CheckGradientStop(gradients[2], Color.Yellow, 1.0, 0.12);
        }

        /// <summary>
        /// Tests NoFill.
        /// </summary>
        [Test]
        public void TestNoFill()
        {
            Document doc = TestUtil.Open(string.Format("{0}TestNoFillStroke.docx", TestDir));
            Shape shape = doc.FirstSection.Body.Shapes[0];

            Fill fill = shape.Chart.Series[0].DataPoints[0].Format.Stroke.Fill;

            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(fill.Visible, Is.False);

            CheckColor(Color.Empty, fill.ForeColor);
        }

        /// <summary>
        /// Tests setting Solid fill.
        /// </summary>
        [Test]
        public void TestSetSolid()
        {
            DocumentBuilder builder = CreateDocument(ShapeMarkupLanguage.Dml);
            Chart chart = CreateChart(builder);

            Fill fill = chart.Series[0].DataPoints[0].Format.Stroke.Fill;
            fill.Solid(Color.Red);

            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(fill.Visible, Is.True);
            CheckColor(Color.Red, fill.ForeColor);

            Document doc = builder.Document;
            // Roundtrip the document.
            string outFileName = string.Format("{0}\\SetSolidChartStroke.docx", TestDir);
            doc = TestUtil.SaveOpen(doc, outFileName, SaveOptions.CreateSaveOptions(SaveFormat.Docx), false);

            Shape shape = doc.FirstSection.Body.Shapes[0];
            fill = shape.Chart.Series[0].DataPoints[0].Format.Stroke.Fill;
            Assert.That(fill.Visible, Is.True);
            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));
            CheckColor(Color.Red, fill.ForeColor);
        }

        /// <summary>
        /// Tests setting gradient fill.
        /// </summary>
        [Test]
        public void TestSetGradient()
        {
            DocumentBuilder builder = CreateDocument(ShapeMarkupLanguage.Dml);
            Chart chart = CreateChart(builder);

            Fill fill = chart.Series[0].DataPoints[0].Format.Stroke.Fill;

            fill.OneColorGradient(Color.Red, GradientStyle.Horizontal, GradientVariant.Variant1, 0.3);

            Assert.That(fill.FillType, Is.EqualTo(FillType.Gradient));
            Assert.That(fill.Visible, Is.True);

            GradientStopCollection gradients = fill.GradientStops;
            CheckGradientStop(gradients[0], Color.Red, 0.0, 0.0);
            CheckGradientStop(gradients[1], Color.FromArgb(0x99, 0x00, 0x00), 1.0, 0.0);
            Assert.That(gradients.Count, Is.EqualTo(2));

            Document doc = builder.Document;
            // Roundtrip the document.
            string outFileName = string.Format("{0}\\SetGradientChartStroke.docx", TestDir);
            doc = TestUtil.SaveOpen(doc, outFileName, SaveOptions.CreateSaveOptions(SaveFormat.Docx), false);

            Shape shape = doc.FirstSection.Body.Shapes[0];
            fill = shape.Chart.Series[0].DataPoints[0].Format.Stroke.Fill;
            Assert.That(fill.FillType, Is.EqualTo(FillType.Gradient));
            Assert.That(fill.Visible, Is.True);

            gradients = fill.GradientStops;
            CheckGradientStop(gradients[0], Color.Red, 0.0, 0.0);
            CheckGradientStop(gradients[1], Color.FromArgb(0x99, 0x00, 0x00), 1.0, 0.0);
            Assert.That(gradients.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests <see cref="Fill.Visible"/> property.
        /// </summary>
        [Test]
        public void TestVisible()
        {
            DocumentBuilder builder = CreateDocument(ShapeMarkupLanguage.Dml);
            Chart chart = CreateChart(builder);

            Fill fill = chart.Series[0].DataPoints[0].Format.Stroke.Fill;
            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(fill.Visible, Is.False);

            // Check set true.
            fill.Visible = true;
            Assert.That(fill.Visible, Is.True);
            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));

            Document doc = builder.Document;
            // Roundtrip the document.
            string outFileName = string.Format("{0}\\SetVisibleChartStrokeTrue.docx", TestDir);
            doc = TestUtil.SaveOpen(doc, outFileName, SaveOptions.CreateSaveOptions(SaveFormat.Docx), false);

            // Check Visible is preserved after round-trip.
            Shape shape = doc.FirstSection.Body.Shapes[0];
            fill = shape.Chart.Series[0].DataPoints[0].Format.Stroke.Fill;
            Assert.That(fill.Visible, Is.True);
            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));

            // Make invisible.
            fill.Visible = false;
            Assert.That(fill.Visible, Is.False);
            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));

            // Roundtrip the document.
            outFileName = string.Format("{0}\\SetVisibleChartStrokeFalse.docx", TestDir);
            doc = TestUtil.SaveOpen(doc, outFileName, SaveOptions.CreateSaveOptions(SaveFormat.Docx), false);

            // Check Visible is preserved after round-trip.
            shape = doc.FirstSection.Body.Shapes[0];
            fill = shape.Chart.Series[0].DataPoints[0].Format.Stroke.Fill;
            Assert.That(fill.Visible, Is.False);
            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));
        }

        /// <summary>
        /// Tests <see cref="Fill.Transparency"/> property.
        /// </summary>
        [Test]
        public void TestTransparency()
        {
            DocumentBuilder builder = CreateDocument(ShapeMarkupLanguage.Dml);
            Chart chart = CreateChart(builder);

            Fill fill = chart.Series[0].DataPoints[0].Format.Stroke.Fill;
            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(fill.Transparency, Is.EqualTo(1.0).Within(0.01));

            // Change transparency.
            fill.Transparency = 0.87;
            Assert.That(fill.Transparency, Is.EqualTo(0.87).Within(0.01));

            Document doc = builder.Document;
            // Roundtrip the document.
            string outFileName = string.Format("{0}\\SetTransparencyChartStroke.docx", TestDir);
            doc = TestUtil.SaveOpen(doc, outFileName, SaveOptions.CreateSaveOptions(SaveFormat.Docx), false);

            // Check Transparency is preserved after round-trip.
            Shape shape = doc.FirstSection.Body.Shapes[0];
            fill = shape.Chart.Series[0].DataPoints[0].Format.Stroke.Fill;
            Assert.That(fill.Transparency, Is.EqualTo(0.87).Within(0.01));
        }

        private const string TestDir = @"Model\Charts\Stroke\Fill\";
    }
}
