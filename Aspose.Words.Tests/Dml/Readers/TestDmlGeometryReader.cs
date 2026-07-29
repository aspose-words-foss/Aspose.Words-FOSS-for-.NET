// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2010 by Alexey Titov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.Guides;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlGeometryReader"/>.
    /// </summary>
    [TestFixture]
    public class TestDmlGeometryReader
    {
        [Test]
        public void Build_GeometryWithAdjustableValues_GuidesInitialized()
        {
            // Arrange
            string xml =
                "<a:custGeom xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    "<a:avLst>" +
                        "<a:gd name=\"myGuide\" fmla=\"val 2\"/>" +
                    "</a:avLst>" +
                "</a:custGeom>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlGeometry result = DmlGeometryReader.Read(reader, null);
            // Assert
            DmlGuides guides = result.Guides;
            Assert.That(guides.AdjustableValues.Count, Is.EqualTo(1));
            DmlGuide guide = guides.AdjustableValues[0];
            Assert.That(guide.Name, Is.EqualTo("myGuide"));
            Assert.That(guide.Formula is DmlOneArgumentFormula, Is.True);
        }

        [Test]
        public void Build_PresetGeometry_PresetGeometryReaded()
        {
            string xml = "<prstGeom prst=\"moon\"/>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlGeometry result = DmlGeometryReader.Read(reader, null);
            // Assert
            Assert.That(result.PresetName, Is.EqualTo("moon"));
            DmlGuides guides = result.Guides;
            Assert.That(guides.AdjustableValues.Count, Is.EqualTo(1));
            DmlGuide guide = guides.AdjustableValues[0];
            Assert.That(guide.Name, Is.EqualTo("adj"));
            Assert.That(guide.Formula is DmlOneArgumentFormula, Is.True);
        }

        [Test]
        public void Build_PresetGeometryWithAdjustableValues_AdjustableValuesShouldBeReplacedAfterCalculation()
        {
            string xml =
                "<a:prstGeom prst=\"moon\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    "<a:avLst xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                        "<a:gd name=\"myGuide\" fmla=\"val 100\"/>" +
                    "</a:avLst>" +
                "</a:prstGeom>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlGeometry geom = DmlGeometryReader.Read(reader, null);
            DmlGuides guides = geom.Guides;
            guides.Calculate(100, 100);
            double result = guides.GetValue("myGuide");
            // Assert
            Assert.That(result, Is.EqualTo(100));
        }

        [Test]
        public void Build_GeometryWithGuides_GuidesInitialized()
        {
            // Arrange
            string xml =
                "<a:custGeom xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    "<a:gdLst>" +
                        "<a:gd name=\"myGuide\" fmla=\"val 2\"/>" +
                    "</a:gdLst>" +
                "</a:custGeom>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlGeometry result = DmlGeometryReader.Read(reader, null);
            // Assert
            DmlGuides guides = result.Guides;
            Assert.That(guides.Guides.Count, Is.EqualTo(1));
            DmlGuide guide = guides.Guides[0];
            Assert.That(guide.Name, Is.EqualTo("myGuide"));
            Assert.That(guide.Formula is DmlOneArgumentFormula, Is.True);
        }

        [Test]
        public void Build_PathDefined_ShapeHasPath()
        {
            // Arrange
            string xml =
                "<a:custGeom xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    "<a:pathLst>" +
                        "<a:path>" +
                        "</a:path>" +
                    "</a:pathLst>" +
                "</a:custGeom>";

            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlGeometry result = DmlGeometryReader.Read(reader, null);
            // Assert
            Assert.That(result.Paths.Count, Is.EqualTo(1));
            Assert.That(result.Paths[0], IsNot.Null());
        }

        /// <summary>
        /// WORDSNET-14145 Unsupported DML element cxnLst.  
        /// Test checks that reader fills connection sites into the model.
        /// </summary>
        [Test]
        public void Build_GeometryWithSimpleConnectionSite()
        {   
            // Prepare input data.
            string xml =
                "<a:custGeom xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    "<a:avLst/>" +
                    "<a:gdLst/>" +
                    "<a:ahLst/>" +
                    "<a:cxnLst>" +
                        "<a:cxn ang=\"0\">" +
                            "<a:pos x=\"707786\" y=\"507786\"/>" +
                        "</a:cxn>" +
                    "</a:cxnLst>" +
                "</a:custGeom>";

            // Perform reading.
            NrxXmlReader reader = new NrxXmlReader(xml, null);            
            DmlGeometry result = DmlGeometryReader.Read(reader, null);

            // Check results.
            IList<DmlConnectionSite> sites = result.ConnectionSites;
            DmlConnectionSite site = sites[0];

            Assert.That(sites.Count, Is.EqualTo(1));
            Assert.That(site.Angle.String, Is.EqualTo("0"));
            Assert.That(site.Coordinates.X.String, Is.EqualTo("707786"));
            Assert.That(site.Coordinates.Y.String, Is.EqualTo("507786"));
        }

        /// <summary>
        /// Test is concerned with WORDSNET-14145
        /// Checks that connection sites are validated after reading and
        /// invalid sites were skipped while reading.
        /// </summary>
        [Test]
        public void Build_GeometryWithConnectionSiteReferencedToGuide()
        {
            // Prepare input data.
            string xml =                
                "<a:custGeom xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    "<a:avLst>"  +
                        "<a:gd name=\"myXGuide\" fmla=\"val 473660\"/>" +
                        "<a:gd name=\"myAngle\" fmla=\"val 16200000\"/>" +
                    "</a:avLst>" +
                    "<a:gdLst>" +
                        "<a:gd name=\"myYGuide\" fmla=\"val 873660\"/>" +
                    "</a:gdLst>" +
                    "<a:ahLst/>" +
                    "<a:cxnLst>" +
                        "<a:cxn ang=\"MissedAngleGuide\">" +
                            "<a:pos x=\"707786\" y=\"507786\"/>" +
                        "</a:cxn>" +
                        "<a:cxn ang=\"myAngle\">" +
                            "<a:pos x=\"MissaedXGuide\" y=\"507786\"/>" +
                        "</a:cxn>" +
                        "<a:cxn ang=\"0\">" +
                            "<a:pos x=\"myXGuide\" y=\"MissaedYGuide\"/>" +
                        "</a:cxn>" +
                        "<a:cxn ang=\"myAngle\">" +
                            "<a:pos x=\"myXGuide\" y=\"myYGuide\"/>" +
                        "</a:cxn>" +
                    "</a:cxnLst>" +
                "</a:custGeom>";

            // Perform reading.     
            TestWarningCallback warningCallback = new TestWarningCallback();
            NrxXmlReader reader = new NrxXmlReader(xml, null, warningCallback);
            DmlGeometry result = DmlGeometryReader.Read(reader, null);
 
            // Check results.
            IList<DmlConnectionSite> sites = result.ConnectionSites;
            DmlConnectionSite site = sites[0];

            Assert.That(sites.Count, Is.EqualTo(1));
            Assert.That(site.Angle.String, Is.EqualTo("myAngle"));
            Assert.That(site.Coordinates.X.String, Is.EqualTo("myXGuide"));
            Assert.That(site.Coordinates.Y.String, Is.EqualTo("myYGuide"));

            Assert.That(warningCallback.Count, Is.EqualTo(3));
            CheckWarnings(warningCallback);      
        }

        /// <summary>
        /// Test is concerned with WORDSNET-14145
        /// Checks that in case when attributes of the sites and child elements are missed or empty
        /// then such sites were skipped while reading.
        /// </summary>
        [Test]
        public void Build_GeometryWithConnectionSiteMissedAttributes()
        {
            // Prepare input data.
            string xml =
                "<a:custGeom xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    "<a:avLst>" +
                        "<a:gd name=\"myXGuide\" fmla=\"val 473660\"/>" +
                        "<a:gd name=\"myAngle\" fmla=\"val 16200000\"/>" +
                    "</a:avLst>" +
                    "<a:gdLst>" +
                        "<a:gd name=\"myYGuide\" fmla=\"val 873660\"/>" +
                    "</a:gdLst>" +
                    "<a:ahLst/>" +
                    "<a:cxnLst>" +
                        "<a:cxn>" +
                            "<a:pos x=\"707786\" y=\"507786\"/>" +
                        "</a:cxn>" +
                        "<a:cxn ang=\"\">" +
                            "<a:pos x=\"myXGuide\" y=\"507786\"/>" +
                        "</a:cxn>" +
                        "<a:cxn ang=\"0\">" +
                            "<a:pos y=\"507786\"/>" +
                        "</a:cxn>" +
                        "<a:cxn ang=\"myAngle\">" +
                            "<a:pos x=\"\" y=\"myYGuide\"/>" +
                        "</a:cxn>" +
                        "<a:cxn ang=\"0\">" +
                            "<a:pos x=\"707786\" />" +
                        "</a:cxn>" +
                        "<a:cxn ang=\"0\">" +
                            "<a:pos x=\"707786\" y=\"\" z=\"\"/>" +
                        "</a:cxn>" +
                    "</a:cxnLst>" +
                "</a:custGeom>";

            // Perform reading.
            TestWarningCallback warningCallback = new TestWarningCallback();
            NrxXmlReader reader = new NrxXmlReader(xml, null, warningCallback);
            DmlGeometry result = DmlGeometryReader.Read(reader, null);

            // Check results.
            IList<DmlConnectionSite> sites = result.ConnectionSites;
         
            Assert.That(sites.Count, Is.EqualTo(0));
            Assert.That(warningCallback.Count, Is.EqualTo(6));
            CheckWarnings(warningCallback);
        }

        /// <summary>
        /// Checks the registered warnings while geometry reading.
        /// </summary>
        /// <param name="warningCallback">Object with collected warnings.</param>
        private static void CheckWarnings(TestWarningCallback warningCallback)
        {
            for (int i = 0; i < warningCallback.Count; ++i)
            {
                Assert.That(warningCallback[i].Source, Is.EqualTo(WarningSource.DrawingML));
                Assert.That(warningCallback[i].WarningType, Is.EqualTo(WarningType.UnexpectedContent));
                Assert.That(warningCallback[i].Description, Is.EqualTo(WarningStrings.UnacceptableMarkup));
            }   
        }
    }
}
