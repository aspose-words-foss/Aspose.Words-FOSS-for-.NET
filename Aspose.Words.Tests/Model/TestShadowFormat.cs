// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/05/2022 by Vadim Saltykov

using System;
using System.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Saving;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests shadow formatting.
    /// </summary>
    [TestFixture]
    public class TestShadowFormat
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WORDSNET-23877 Provide API to remove the shape shadows.
        /// DML shape case.
        /// </summary>
        [Test]
        public void Test23877Dml()
        {
            Document doc = TestUtil.Open(@"Model\Shape\ShadowFormat\TestShadowDML.docx");

            Shape shape = doc.FirstSection.Body.Shapes[0];

            Assert.That(shape.ShadowFormat.Visible, Is.True);
            Assert.That(shape.DmlShape.Effects[DmlShapeEffectType.OuterShadow], IsNot.Null());

            shape.ShadowFormat.Clear();

            Assert.That(shape.ShadowFormat.Visible, Is.False);
            Assert.That(shape.DmlShape.Effects, Is.Null);
        }

        // FOSS: Test23877Vml removed — loads a VML .doc and asserts VML-specific shadow
        // representation (ShapeAttr.ShadowOn/ShadowType/ShadowOpacity). The Doc reader is
        // removed and VML shapes have no equivalent in the DML path. DML shadow removal
        // stays covered by Test23877Dml.





        /// <summary>
        /// WORDSNET-26555 Add support to get DML effects of a Shape.
        /// Implemented <see cref="ShadowFormat.Color"/>.
        /// </summary>
        [TestCase("DML.docx")] // FOSS: VML .doc case removed (Doc reader gone; VML shadow is DOC-specific).
        public void Test26555(string fileExt)
        {
            string fileName = string.Format(@"Model\Shape\ShadowFormat\Test26555{0}", fileExt);
            Document doc = TestUtil.Open(fileName);

            Shape shape = doc.FirstSection.Body.Shapes[0];
            TestUtil.CheckColor(Color.FromArgb(0x66FF0000), shape.ShadowFormat.Color);

            doc = TestUtil.SaveOpen(doc, fileName);

            shape = doc.FirstSection.Body.Shapes[0];
            TestUtil.CheckColor(Color.FromArgb(0x66FF0000), shape.ShadowFormat.Color);

            // Check also color in cleared shadow.
            shape.ShadowFormat.Clear();
            TestUtil.CheckColor(Color.Black, shape.ShadowFormat.Color);
        }

        /// <summary>
        /// WORDSNET-28384 Provide API to set Shadow Color on Shape
        /// Tests the new public Color property.
        /// </summary>
        [TestCase("DML.docx")] // FOSS: VML .doc case removed (Doc reader gone; VML shadow is DOC-specific).
        public void Test28384Color(string fileExt)
        {
            string fileName = string.Format(@"Model\Shape\ShadowFormat\Test28384{0}", fileExt);
            Document doc = TestUtil.Open(fileName);
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            TestUtil.CheckColor(Color.Black, shape.ShadowFormat.Color);

            shape.ShadowFormat.Color = Color.Red;
            TestUtil.CheckColor(Color.Red, shape.ShadowFormat.Color);

            shape.ShadowFormat.Color = Color.Empty;
            TestUtil.CheckColor(Color.Empty, shape.ShadowFormat.Color);
        }

        /// <summary>
        /// Relates to WORDSNET-28384.
        /// Tests the new public Transparency property.
        /// </summary>
        [TestCase("DML.docx")] // FOSS: VML .doc case removed (Doc reader gone; VML shadow is DOC-specific).
        public void Test28384Transparency(string fileExt)
        {
            string fileName = string.Format(@"Model\Shape\ShadowFormat\Test28384{0}", fileExt);
            Document doc = TestUtil.Open(fileName);
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            Assert.That(shape.ShadowFormat.Transparency, Is.EqualTo(0.0).Within(0.01));
            shape.ShadowFormat.Transparency = 0.5;
            Assert.That(shape.ShadowFormat.Transparency, Is.EqualTo(0.5).Within(0.01));
        }

        /// <summary>
        /// Relates to WORDSNET-28384.
        /// Tests that exception is thrown when applying value to Transparency out of [0, 1] range.
        /// </summary>
        [TestCase("DML.docx", -0.3), ExpectedException(typeof(ArgumentException), ExpectedMessage = "Expected a value between 0 and 1.\r\nParameter name: Shadow.Transparency")]
        [TestCase("DML.docx", 1.3)]
        // FOSS: VML .doc cases removed (Doc reader gone).
        public void Test28384TransparencyInvalid(string fileExt, double transparency)
        {
            string fileName = string.Format(@"Model\Shape\ShadowFormat\Test28384{0}", fileExt);
            Document doc = TestUtil.Open(fileName);
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            shape.ShadowFormat.Transparency = transparency;
        }

        /// <summary>
        /// Relates to WORDSNET-28384.
        /// Tests stability of changing the shadow type to the same value.
        /// </summary>
        [TestCase("DML.docx")] // FOSS: VML .doc case removed (Doc reader gone; VML shadow is DOC-specific).
        public void Test28384ResetTypeResilience(string fileExt)
        {
            string fileName = string.Format(@"Model\Shape\ShadowFormat\Test28384{0}", fileExt);
            Document doc = TestUtil.Open(fileName);
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            shape.ShadowFormat.Type = ShadowType.Shadow21;
            shape.ShadowFormat.Color = Color.Red;
            shape.ShadowFormat.Transparency = 0.5;

            // Checks that setting the same shadow type does not reset the other properties.
            shape.ShadowFormat.Type = ShadowType.Shadow21;
            TestUtil.CheckColor(Color.FromArgb(128, 255, 0, 0), shape.ShadowFormat.Color);
            Assert.That(shape.ShadowFormat.Transparency, Is.EqualTo(0.5).Within(0.01));
        }

    }
}
