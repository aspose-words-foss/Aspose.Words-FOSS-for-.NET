// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/29/2014 by Alexey Noskov

using System.Xml;
using Aspose.Collections;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Diagrams;
using Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Saving;
using Aspose.Words.Tests.Export.Docx;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    [TestFixture, Ignore("TestDmlDiagrams")] // todo: remove explicit when writing diagrams implemented.
    [AndroidDelete]
    public class TestDmlDiagrams
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }












        [Test]
        public void TestListDiagrams()
        {
            TestUtil.OpenSaveOpen(@"Model\Diagrams\TestListDiagrams.docx");
        }




        [Test]
        public void TestRelationshipDiagrams()
        {
            TestUtil.OpenSaveOpen(@"Model\Diagrams\TestRelationshipDiagrams.docx");
        }



        [Test]
        public void TestDiagramMath()
        {
            TestUtil.OpenSaveOpen(@"Model\Diagrams\TestDiagramMath.docx");
        }



        /// <summary>
        /// WORDSNET-12476 Shape's hyperlink is removed after re-saving Docx.
        /// The 'dgm:extLst' element was not supported for diagram point.
        /// </summary>
        [Test]
        public void TestJira12476()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Diagrams\TestJira12476", UnifiedScenario.Docx2DocxNoGold);
            DmlDiagram diagram = (DmlDiagram)doc.FirstSection.Body.Shapes[0].DmlNode;

            StringToObjDictionary<DmlExtension> extensions = diagram.Data.PointList[1].Extensions;
            Assert.That(extensions.Count, Is.EqualTo(2));

            DmlExtension extension0 = extensions["{A40237B7-FDA0-4F09-8148-C483321AD2D9}"];
            Assert.That(extension0.NvPr, Is.Null);
            Assert.That(extension0.XmlDoc, IsNot.Null());

            DmlExtension extension1 = extensions["{E40237B7-FDA0-4F09-8148-C483321AD2D9}"];
            DmlNvDrawingProperties nvPr = extension1.NvPr;
            Assert.That(nvPr, IsNot.Null());
            Assert.That(extension1.XmlDoc, Is.Null);
            Assert.That(nvPr.HlinkClick.Id, Is.EqualTo(@"http://www.rossmanith.com"));
            Assert.That(nvPr.HlinkHover.Id, Is.EqualTo(@"http://www.hlinkhover.com"));
        }

        /// <summary>
        /// WORDSNET-13319 Corrupt output when using Aspose to save MS Word document containing SmartArt.
        /// The problem occurs because DmlPointsTextSpacing values was written incorrectly.
        /// </summary>
        [Test]
        public void TestJira13319()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Diagrams\TestJira13319", UnifiedScenario.Docx2DocxNoGold);
            DmlDiagram diagram = (DmlDiagram)doc.FirstSection.Body.Shapes[0].DmlNode;
            DmlDiagramPoint dmlDiagramPoint = diagram.Data.PointList[7];
            DmlParagraph dmlParagraph = (DmlParagraph)dmlDiagramPoint.TextBody.Paragraphs[3];
            Assert.That(((DmlPointsTextSpacing)dmlParagraph.Properties.SpaceAfter).Value.Value, Is.EqualTo(336));
        }

        /// <summary>
        /// WORDSNET-14129 The recolorImg extension of the dataModel element is lost after re-saving a document.
        /// Fixed by adding mechanism to read/write data model extensions.
        /// </summary>
        [Test]
        public void TestJira14129()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Diagrams\TestJira14129", UnifiedScenario.Docx2DocxNoGold);
            DocxExportContext context = new DocxExportContext(doc, "word/diagrams/data1.xml", OoxmlComplianceCore.IsoTransitional);

            XmlElement parent = (XmlElement)context.GetXmlNode(string.Format("//a:ext[@uri = \"{0}\"]", DmlExtensionUri.RecolorImg.ToUpper()));
            Assert.That(parent, IsNot.Null());

            XmlElement node = (XmlElement)parent.FirstChild;
            Assert.That(node, IsNot.Null());

            string xPath = "//dgm14:recolorImg";

            Assert.That(node, IsNot.Null(), string.Format("Cannot find the node '{0}'.", xPath));
            Assert.That(node.Attributes[0].Value, Is.EqualTo("1"), string.Format("Value of the node '{0}' is wrong.", xPath));
        }

        /// <summary>
        /// WORDSNET-14137 Effect list is not supported in diagram background.
        /// Fixed by adding mechanism to read/write data model effect list.
        /// </summary>
        [Test]
        public void TestJira14137()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\TestJira13384", UnifiedScenario.Docx2DocxNoGold);

            Shape node = (Shape)doc.FirstSection.Body.GetChild(NodeType.Shape, 2, true);
            DmlShapeEffectsCollection effectsCollection = ((DmlDiagram)node.GraphicData).Data.Background.Effects;
            Assert.That(effectsCollection.Count, Is.EqualTo(1));
            DmlShapeEffect dmlShapeEffect = effectsCollection[DmlShapeEffectType.OuterShadow];
            Assert.That(dmlShapeEffect, IsNot.Null());
        }
    }
}
