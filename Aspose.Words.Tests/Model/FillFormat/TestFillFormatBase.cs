// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/12/2019 by Ilya Navrotskiy

using System.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The base class for testing FillFormat object.
    /// </summary>
    public class TestFillFormatBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Creates test document with a specified markup language.
        /// </summary>
        internal static DocumentBuilder CreateDocument(ShapeMarkupLanguage markupLanguage)
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);

            if (markupLanguage == ShapeMarkupLanguage.Dml)
            {
                doc.CompatibilityOptions.OptimizeFor(MsWordVersion.Word2010);
                OoxmlComplianceInfo.MarkAsHasDrawingExtensions(doc);
            }

            DocumentBuilder builder = new DocumentBuilder(doc);
            return builder;
        }

        /// <summary>
        /// Opens test document with a specified markup language.
        /// </summary>
        internal static Document Open(string fileName, ShapeMarkupLanguage markupLanguage)
        {
            string testFilePath = GetTestPath(fileName, markupLanguage);
            LoadFormat lf = MarkupLanguageToLoadFormat(markupLanguage);

            return TestUtil.Open(testFilePath, lf);
        }

        /// <summary>
        /// Returns first fillable node in a specified node.
        /// </summary>
        internal static Node GetFirstFillable(Node rootNode)
        {
            NodeMatcher matcher = new NodeTypeMatcher(new NodeType[] {NodeType.Run, NodeType.Shape});
            return NodeUtil.FindChildOrSelf(rootNode, matcher);
        }

        /// <summary>
        /// Returns FillFormat object of the first fillable node in a specified node.
        /// </summary>
        internal static Fill GetFill(Node rootNode)
        {
            Node node = GetFirstFillable(rootNode);
            return (node.NodeType == NodeType.Run) ? ((Run)node).Font.Fill : ((Shape)node).Fill;
        }

        /// <summary>
        /// Creates SaveOptions by a specified markup language.
        /// </summary>
        internal static SaveOptions CreateSaveOptions(ShapeMarkupLanguage markupLanguage)
        {
            SaveFormat sf = MarkupLanguageToSaveFormat(markupLanguage);
            return SaveOptions.CreateSaveOptions(sf);
        }

        /// <summary>
        /// Converts markup language to extension.
        /// </summary>
        protected static string MarkupLanguageToExtension(ShapeMarkupLanguage markupLanguage)
        {
            LoadFormat lf = MarkupLanguageToLoadFormat(markupLanguage);
            return FileFormatUtil.LoadFormatToExtension(lf);
        }

        /// <summary>
        /// Creates a column chart.
        /// </summary>
        protected static Chart CreateChart(DocumentBuilder builder)
        {
            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart = shape.Chart;
            ChartSeriesCollection series = chart.Series;

            // Delete default generated series.
            series.Clear();

            string[] categories = new string[] { "Category 1", "Category 2" };

            series.Add("Series 1", categories, new double[] { 1, 2 });
            series.Add("Series 2", categories, new double[] { 3, 4 });

            return chart;
        }

        /// <summary>
        /// Checks color with diagnostic message.
        /// </summary>
        protected static void CheckColor(Color expectedColor, Color actualColor, string msg = "")
        {
            int expectedValue = expectedColor.ToArgb();
            int actualValue = actualColor.ToArgb();
            Assert.That(actualValue, Is.EqualTo(expectedValue), string.Format(                "Expected: 0x{0:X8}, Actual: 0x{1:X8}, {2}", expectedValue, actualValue, msg));
        }

        /// <summary>
        /// Checks gradient stop.
        /// </summary>
        protected static void CheckGradientStop(
            GradientStop gradientStop,
            Color expectedColor,
            double expectedPosition)
        {
            CheckGradientStop(gradientStop, expectedColor, expectedPosition, 0.0);
        }

        /// <summary>
        /// Checks gradient stop.
        /// </summary>
        protected static void CheckGradientStop(
            GradientStop gradientStop,
            Color expectedColor,
            double expectedPosition,
            double expectedTransparency)
        {
            CheckColor(expectedColor, gradientStop.Color);
            Assert.That(gradientStop.Position, Is.EqualTo(expectedPosition).Within(0.01));
            Assert.That(gradientStop.Transparency, Is.EqualTo(expectedTransparency).Within(0.01));
        }

        /// <summary>
        /// Returns true, if a specified gradient style represents linear gradient.
        /// </summary>
        private static bool IsLinearGradient(GradientStyle style)
        {
            return (style == GradientStyle.Horizontal) ||
                   (style == GradientStyle.Vertical) ||
                   (style == GradientStyle.DiagonalDown) ||
                   (style == GradientStyle.DiagonalUp);
        }

        /// <summary>
        /// Returns expected angle degrees for a specified gradient style and variant.
        /// </summary>
        private static double GetExpectedAngle(GradientStyle style, GradientVariant variant)
        {
            switch (style)
            {
                case GradientStyle.Horizontal:
                    return gExpectedHorizontalAngles[(int)variant - 1];
                case GradientStyle.Vertical:
                    return gExpectedVerticalAngles[(int)variant - 1];
                case GradientStyle.DiagonalUp:
                    return gExpectedDiagonalUpAngles[(int)variant - 1];
                case GradientStyle.DiagonalDown:
                    return gExpectedDiagonalDownAngles[(int)variant - 1];
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Checks gradient angle of all shapes in a specified document.
        /// </summary>
        protected static void CheckGradientAngleShape(Document doc)
        {
            foreach (Shape shape in doc.FirstSection.Body.Shapes)
            {
                GradientStyle style = shape.Fill.GradientStyle;
                GradientVariant variant = shape.Fill.GradientVariant;

                double expectedAngle = GetExpectedAngle(style, variant);
                Assert.That(shape.Fill.GradientAngle, Is.EqualTo(expectedAngle), string.Format("[{0}, {1}]", style, variant));

                // Set some arbitrary angle.
                const double angle = 137.0;
                shape.Fill.GradientAngle = angle;

                // The angle is only available in linear gradients.
                expectedAngle = IsLinearGradient(style) ? angle : 0.0;
                Assert.That(shape.Fill.GradientAngle, Is.EqualTo(expectedAngle), string.Format("[{0}, {1}]", style, variant));
            }
        }

        /// <summary>
        /// Checks gradient angle of all styles and variants in text of a specified document.
        /// </summary>
        protected static void CheckGradientAngleText(Document doc)
        {
            GradientStyle[] styles = new GradientStyle[]
            {
                GradientStyle.Horizontal, GradientStyle.Vertical, GradientStyle.DiagonalDown, GradientStyle.DiagonalUp,
                GradientStyle.FromCenter, GradientStyle.FromCorner
            };
            foreach (GradientStyle style in styles)
            {
                for (int variant = 1; variant <= 4; variant++)
                {
                    string runText = string.Format("...{0}_{1}...", style.ToString(), variant);
                    Run run = TestUtil.GetRunWithText(doc, runText);

                    double expectedAngle = GetExpectedAngle(style, (GradientVariant)variant);
                    Assert.That(run.Font.Fill.GradientAngle, Is.EqualTo(expectedAngle), runText);

                    // Set some arbitrary angle.
                    const double angle = 137.0;
                    run.Font.Fill.GradientAngle = angle;

                    // The angle is only available in linear gradients.
                    expectedAngle = IsLinearGradient(style) ? angle : 0.0;
                    Assert.That(run.Font.Fill.GradientAngle, Is.EqualTo(expectedAngle), runText);
                }
            }
        }

        /// <summary>
        /// Converts markup language to LoadFormat.
        /// </summary>
        private static LoadFormat MarkupLanguageToLoadFormat(ShapeMarkupLanguage markupLanguage)
        {
            // FOSS We do not have DOC. But we have DOCX with compatibility VML shapes, so load DOCX in any case.
            return (markupLanguage == ShapeMarkupLanguage.Dml) ? LoadFormat.Docx : LoadFormat.Docx;
        }

        /// <summary>
        /// Converts markup language to SaveFormat.
        /// </summary>
        private static SaveFormat MarkupLanguageToSaveFormat(ShapeMarkupLanguage markupLanguage)
        {
            // FOSS We do not have DOC. But we have DOCX with compatibility VML shapes, so load DOCX in any case.
            return (markupLanguage == ShapeMarkupLanguage.Dml) ? SaveFormat.Docx : SaveFormat.Docx;
        }

        /// <summary>
        /// Gets full test path by a specified test file name and markup language.
        /// </summary>
        private static string GetTestPath(string fileName, ShapeMarkupLanguage markupLanguage)
        {
            return string.Format(@"Model\FillFormat\{0}\{1}", MarkupLanguageToString(markupLanguage), fileName);
        }

        /// <summary>
        /// Converts a specified markup language to a string.
        /// </summary>
        private static string MarkupLanguageToString(ShapeMarkupLanguage markupLanguage)
        {
            return (markupLanguage == ShapeMarkupLanguage.Dml) ? "Dml" : "Vml";
        }

        private static readonly double[] gExpectedHorizontalAngles = new[] { 90.0, 270.0, 90.0, 270.0 };
        private static readonly double[] gExpectedVerticalAngles = new[] { 0.0, 180.0, 0.0, 180.0 };
        private static readonly double[] gExpectedDiagonalDownAngles = new[] { 135.0, 315.0, 135.0, 315.0 };
        private static readonly double[] gExpectedDiagonalUpAngles = new[] { 45.0, 225.0, 45.0, 225.0 };
    }
}
