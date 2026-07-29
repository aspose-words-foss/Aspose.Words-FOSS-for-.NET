// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/11/2019 by Ilya Navrotskiy

using System.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing Transparency property of the FillFormat object.
    /// </summary>
    [TestFixture]
    public class TestFillFormatTransparency : TestFillFormatBase
    {
        /// <summary>
        /// Tests Transparency of the various Blip fills.
        /// </summary>
        /// <remarks> These fills behave the same in DML and VML. </remarks>
        [TestCase("Patterned")]
        [TestCase("PresetTextured")]
        [TestCase("UserPicture")]
        [TestCase("UserTextured")]
        public void TestBlipTransparency(string testName)
        {
            const double expectedOriginalValue = 0.37d;

            // DML
            TestFillTransparency(testName, ShapeMarkupLanguage.Dml, expectedOriginalValue, 0.0);
            TestFillTransparency(testName, ShapeMarkupLanguage.Dml, expectedOriginalValue, 1.0);
            TestFillTransparency(testName, ShapeMarkupLanguage.Dml, expectedOriginalValue, 0.52);
            // VML
            TestFillTransparency(testName, ShapeMarkupLanguage.Vml, expectedOriginalValue, 0.0);
            TestFillTransparency(testName, ShapeMarkupLanguage.Vml, expectedOriginalValue, 1.0);
            TestFillTransparency(testName, ShapeMarkupLanguage.Vml, expectedOriginalValue, 0.52);
        }

        /// <summary>
        /// Tests Transparency of the various one-colored DML fills.
        /// </summary>
        [TestCase("TextSolid", 0.39)]
        [TestCase("TextOneColorGradient", 0.65)]
        [TestCase("TextNoFill", 1.0)]
        [TestCase("Solid", 0.37)]
        [TestCase("NoFill", 1.0)]
        public void TestDmlTransparency(string testName, double expectedOriginalValue)
        {
            TestFillTransparency(testName, ShapeMarkupLanguage.Dml, expectedOriginalValue, 0.0);
            TestFillTransparency(testName, ShapeMarkupLanguage.Dml, expectedOriginalValue, 1.0);
            TestFillTransparency(testName, ShapeMarkupLanguage.Dml, expectedOriginalValue, 0.52);
        }

        /// <summary>
        /// Tests Transparency of the various gradient fills.
        /// It has additional check for each gradient stop, as transparency setter should change them all.
        /// </summary>
        [TestCase("TextTwoColorGradient", 0.16)]
        [TestCase("TextPresetGradient", 0.39)]
        [TestCase("OneColorGradient", 0.37)]
        [TestCase("TwoColorGradient", 0.37)]
        [TestCase("PresetGradient", 0.37)]
        public void TestDmlGradientTransparency(string testName, double expectedOriginalValue)
        {
            Document doc = TestFillTransparency(testName, ShapeMarkupLanguage.Dml, expectedOriginalValue, 0.0);
            CheckGradientStops(doc, 0.0);

            doc = TestFillTransparency(testName, ShapeMarkupLanguage.Dml, expectedOriginalValue, 1.0);
            CheckGradientStops(doc, 1.0);

            doc = TestFillTransparency(testName, ShapeMarkupLanguage.Dml, expectedOriginalValue, 0.52);
            CheckGradientStops(doc, 0.52);
        }

        /// <summary>
        /// Tests Transparency of the various VML shapes fills.
        /// </summary>
        [TestCase("OneColorGradient", 0.37)]
        [TestCase("PresetGradient", 0.37)]
        [TestCase("TwoColorGradient", 0.37)]
        [TestCase("Solid", 0.37)]
        [TestCase("NoFill", 0)]
        public void TestVmlShapeTransparency(string testName, double expectedOriginalValue)
        {
            TestFillTransparency(testName, ShapeMarkupLanguage.Vml, expectedOriginalValue, 0.0);
            TestFillTransparency(testName, ShapeMarkupLanguage.Vml, expectedOriginalValue, 1.0);
            TestFillTransparency(testName, ShapeMarkupLanguage.Vml, expectedOriginalValue, 0.52);
        }

        /// <summary>
        /// Tests Transparency when there is no Alpha in the color of the fill.
        /// </summary>
        [TestCase("Patterned")]
        [TestCase("OneColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("Solid")]
        [TestCase("NoFill")]
        [TestCase("TextSolid")]
        [TestCase("TextOneColorGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextPresetGradient")]
        [TestCase("TextNoFill")]
        public void TestTransparencyNoAlpha(string testName)
        {
            Document doc = Open(testName, ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);

            // Apply color without Alpha (full opaque).
            fill.ForeColor = Color.Green;
            Assert.That(fill.Transparency, Is.EqualTo(0.0).Within(0.01));
        }

        /// <summary>
        /// Checks gradient stops.
        /// </summary>
        private static void CheckGradientStops(Node node, double expectedTransparency)
        {
            Node fillableNode = GetFirstFillable(node);

            DmlGradientFill fill;
            double expectedAlpha;
            if (fillableNode.NodeType == NodeType.Shape)
            {
                Shape shape = (Shape)fillableNode;
                fill = (DmlGradientFill)((IDmlCommonShapePrSource)shape.DmlNode).Fill;
                // Shapes keep opacity instead of transparency.
                expectedAlpha = 1 - expectedTransparency;
            }
            else
            {
                Run run = (Run)fillableNode;
                fill = (DmlGradientFill)run.RunPr[FontAttr.EffectFill];
                expectedAlpha = expectedTransparency;
            }

            foreach (DmlGradientStop gradientStop in fill.GradientStops)
                Assert.That(gradientStop.Color.Alpha.Value, Is.EqualTo(expectedAlpha).Within(0.001));
        }

        /// <summary>
        /// Tests Transparency of the first fillable object in a document at a specified path.
        /// </summary>
        private static Document TestFillTransparency(string testFileNameWithoutExtension, ShapeMarkupLanguage markupLanguage,
            double originalValue, double desiredValue)
        {
            return TestFillTransparency(testFileNameWithoutExtension, markupLanguage, originalValue, desiredValue, desiredValue);
        }

        /// <summary>
        /// Tests Transparency of the first fillable object in a document at a specified path.
        /// </summary>
        private static Document TestFillTransparency(string testFileNameWithoutExtension, ShapeMarkupLanguage markupLanguage,
            double originalValue, double desiredValue, double actualValue)
        {
            Document doc = Open(testFileNameWithoutExtension, markupLanguage);
            return TestFillTransparency(doc, testFileNameWithoutExtension, markupLanguage,
                originalValue, desiredValue, actualValue);
        }

        /// <summary>
        /// Tests Transparency of the first fillable object in a specified document.
        /// </summary>
        private static Document TestFillTransparency(Document doc, string testFileNameWithoutExtension,
            ShapeMarkupLanguage markupLanguage, double originalValue, double desiredValue, double actualValue)
        {
            Fill fill = GetFill(doc);
            Assert.That(fill.Transparency, Is.EqualTo(originalValue).Within(0.005));

            bool isNoFill = !fill.Visible && (fill.FillType == FillType.Solid);

            // Set desired Value.
            fill.Transparency = desiredValue;
            Assert.That(fill.Transparency, Is.EqualTo(desiredValue).Within(0.005));

            // WORDSNET-25216 Need to specify color for a DML pattern fill otherwise it will not be written to
            // the output document.
            if (isNoFill && (fill.FillType == FillType.Patterned) && (markupLanguage == ShapeMarkupLanguage.Dml))
            {
                fill.ForeColor = Color.Black;
                fill.Transparency = desiredValue;
            }

            string ext = MarkupLanguageToExtension(markupLanguage);
            string outFileName = string.Format("FillFormat\\{0}_Transparency_{1}{2}",
                testFileNameWithoutExtension, desiredValue, ext);

            // Roundtrip the document.
            doc = TestUtil.SaveOpen(doc, outFileName, CreateSaveOptions(markupLanguage), false);

            fill = GetFill(doc);
            Assert.That(fill.Transparency, Is.EqualTo(actualValue).Within(0.005));

            return doc;
        }
    }
}
