// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/04/2024 by Vyacheslav Deryushev

using Aspose.Words.Drawing;
using Aspose.Words.Saving;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests adjustment raw values.
    /// </summary>
    [TestFixture]
    public class TestAdjustments
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }




        /// <summary>
        /// WORDSNET-27018 Incorrect count of adjustments after setting adjust to 0.
        /// We should not exclude zero values from the collection.
        /// </summary>
        [Test]
        public void Test27018()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = new Shape(doc, ShapeType.Arrow);
            shape.Width = 100;
            shape.Height = 100;
            shape.Adjustments[0].Value = 0;

            builder.MoveToDocumentStart();
            builder.InsertNode(shape);

            OoxmlSaveOptions ooxmlSaveOptions = new OoxmlSaveOptions();
            ooxmlSaveOptions.Compliance = OoxmlCompliance.Ecma376_2006;

            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Adjustments\Test27018.docx", ooxmlSaveOptions);

            shape = (Shape)doc.GetChildNodes(NodeType.Shape, true)[0];
            AdjustmentCollection adjustments = shape.Adjustments;

            Assert.That(adjustments[0].Value, Is.EqualTo(0));
            Assert.That(adjustments[1].Value, Is.EqualTo(5400));
            Assert.That(adjustments.Count, Is.EqualTo(2));
        }
    }
}
