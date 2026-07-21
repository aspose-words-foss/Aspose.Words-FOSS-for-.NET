// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2014 by Pavel Gorbunov

using System;
using System.IO;
using System.Text;
using System.Xml;
using Aspose.Xml;
using NUnit.Framework;

namespace Aspose.Tests.Pal
{
    // These tests cases already present in com.aspose.ms.TestXmlTextReader in Java
    [TestFixture]
    public class TestPalXmlUtil
    {
        [Test]
        public void TestReadOuterXmlCharacterEntitiesInAttributes()
        {
            String input = "<start comment=\"&amp; &lt; &gt; \r\n\t.\"></start>";

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.ASCII.GetBytes(input)));

            reader.Read();
            Assert.That(reader.LocalName, Is.EqualTo("start"));
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));

            String outerXml = XmlUtilPal.ReadOuterXml(reader);
            Assert.That(outerXml, Is.EqualTo("<start comment=\"&amp; &lt; &gt; &#xD;&#xA;\t.\"></start>"));
            reader.Close();
        }

        [Test]
        public void TestReadOuterXmlRussianInAttributes()
        {
            String input = "<start comment=\"Русский язык\">" +
                           "</start>";

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));

            reader.Read();
            Assert.That(reader.LocalName, Is.EqualTo("start"));
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));

            String outerXml = XmlUtilPal.ReadOuterXml(reader);
            Assert.That(outerXml, Is.EqualTo(input));
            reader.Close();
        }

        [Test]
        public void TestReadOuterXml()
        {
            String input = "<book genre='novel' ISBN='1-861003-78' pubdate='1987'>\n" +
                           "  <title>Pride And Prejudice</title>\n" +
                           "  <price usd=\"1.23\" eur=\"1.0\" />\n" +
                           "</book>";

            // readOuterXml() changes single quotes to double ones.
            // readOuterXml() put attributes in alphabetical order
            String output = "<book ISBN=\"1-861003-78\" genre=\"novel\" pubdate=\"1987\">\n" +
                           "  <title>Pride And Prejudice</title>\n" +
                           "  <price eur=\"1.0\" usd=\"1.23\" />\n" +
                           "</book>";

            //position on <book> element start.
            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.ASCII.GetBytes(input)));
            reader.Read();
            Assert.That(reader.LocalName, Is.EqualTo("book"));
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
            String outerXml = XmlUtilPal.ReadOuterXml(reader);
            Assert.That(outerXml, Is.EqualTo(output));
            reader.Close();

            //position on whitespaces right after <book> element: outer xml should be empty string.
            reader = new XmlTextReader(new MemoryStream(Encoding.ASCII.GetBytes(input)));
            reader.Read();
            reader.Read();
            Assert.That(reader.LocalName, Is.EqualTo(""));
#if JAVA
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.SignificantWhitespace));
#else
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Whitespace));
#endif
            Assert.That(XmlUtilPal.ReadOuterXml(reader), Is.EqualTo(""));
            reader.Close();

            //position on <title> element.
            reader = new XmlTextReader(new MemoryStream(Encoding.ASCII.GetBytes(input)));
            while ("title" != reader.LocalName)
                reader.Read();
            Assert.That(reader.LocalName, Is.EqualTo("title"));
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
            Assert.That(XmlUtilPal.ReadOuterXml(reader), Is.EqualTo("<title>Pride And Prejudice</title>"));
            reader.Close();

            //position on <price> element.
            reader = new XmlTextReader(new MemoryStream(Encoding.ASCII.GetBytes(input)));
            while ("price" != reader.LocalName)
                reader.Read();
            Assert.That(reader.LocalName, Is.EqualTo("price"));
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
            // readOuterXml() put attributes in alphabetical order
            Assert.That(XmlUtilPal.ReadOuterXml(reader), Is.EqualTo("<price eur=\"1.0\" usd=\"1.23\" />"));
            reader.Close();
        }

        [Test]
        public void TestReadOuterXmlFromString()
        {
            String input = "<book ISBN='1-861003-78' genre='novel' pubdate='1987'>\n" +
                           "  <title>Pride And Prejudice</title>\n" +
                           "  <price eur=\"1.0\" usd=\"1.23\" />\n" +
                           "</book>";

            // readOuterXml() changes single quotes to double ones.
            // readOuterXml() put attributes in alphabetical order
            String output = "<book ISBN=\"1-861003-78\" genre=\"novel\" pubdate=\"1987\">\n" +
                           "  <title>Pride And Prejudice</title>\n" +
                           "  <price eur=\"1.0\" usd=\"1.23\" />\n" +
                           "</book>";

            //position on <book> element start.
            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.ASCII.GetBytes(input)));
            reader.Read();
            Assert.That(reader.LocalName, Is.EqualTo("book"));
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
            String outerXml = XmlUtilPal.ReadOuterXml(reader);
            Assert.That(outerXml, Is.EqualTo(output));
            reader.Close();

            //position on whitespaces right after <book> element: outer xml should be empty string.
            reader = new XmlTextReader(new MemoryStream(Encoding.ASCII.GetBytes(input)));
            reader.Read();
            reader.Read();
            Assert.That(reader.LocalName, Is.EqualTo(""));
#if JAVA
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.SignificantWhitespace));
#else
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Whitespace));
#endif
            Assert.That(XmlUtilPal.ReadOuterXml(reader), Is.EqualTo(""));
            reader.Close();

            //position on <title> element.
            reader = new XmlTextReader(new MemoryStream(Encoding.ASCII.GetBytes(input)));
            while ("title" != reader.LocalName)
                reader.Read();
            Assert.That(reader.LocalName, Is.EqualTo("title"));
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
            Assert.That(XmlUtilPal.ReadOuterXml(reader), Is.EqualTo("<title>Pride And Prejudice</title>"));
            reader.Close();

            //position on <price> element.
            reader = new XmlTextReader(new MemoryStream(Encoding.ASCII.GetBytes(input)));
            while ("price" != reader.LocalName)
                reader.Read();
            Assert.That(reader.LocalName, Is.EqualTo("price"));
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
            // readOuterXml() put attributes in alphabetical order
            Assert.That(XmlUtilPal.ReadOuterXml(reader), Is.EqualTo("<price eur=\"1.0\" usd=\"1.23\" />"));
            reader.Close();
        }

        [Test]
        public void TestReadOuterXmlWithXmlns()
        {
            String bodyNode = "<w:body>\n" +
                              "\t\t<a:graphic xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">\n" +
                              "\t\t\t<a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas\">\n" +
                              "\t\t\t<w10:blip r:embed=\"rId8\" cstate=\"print\" />\n" +
                              "\t\t\t\t<lc:lockedCanvas xmlns:lc=\"http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas\">\n" +
                              "\t\t\t\t\t<a:nvGrpSpPr>\n" +
                              "\t\t\t\t\t\t<a:cNvPr id=\"0\" name=\"\" /> \n" +
                              "\t\t\t\t\t\t<a:cNvGrpSpPr /> \n" +
                              "\t\t\t\t\t</a:nvGrpSpPr>\n" +
                              "\t\t\t\t</lc:lockedCanvas>\n" +
                              "\t\t\t<w10:blip r:embed=\"rId8\" cstate=\"print\" />\n" +
                              "\t\t\t</a:graphicData>\n" +
                              "\t\t</a:graphic>\n" +
                              "\t</w:body>";
            String documentNode = "<w:document xmlns:w10=\"urn:schemas-microsoft-com:office:word\" " +
                                    "xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\" " +
                                    "xmlns:wne=\"http://schemas.microsoft.com/office/word/2006/wordml\" " +
                                    "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">\n" +
                                  "\t" + bodyNode + "\n" +
                                  "</w:document>";
            String input = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?> \n" + documentNode;

            // readOuterXml() adds xmlns defined before current xml piece AND used inside the piece.
            // readOuterXml() put attributes in alphabetical order
            // xmlns added to node starting from which defined namespace used, for instance (see below):
            // xmlns:w added to w:body, xmlns:r added to a:blip.
            String bodyNodeOuter = "<w:body xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">\n" +
                                   "\t\t<a:graphic xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">\n" +
                                   "\t\t\t<a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas\">\n" +
                                   "\t\t\t<w10:blip cstate=\"print\" r:embed=\"rId8\" " +
                                        "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" " +
                                        "xmlns:w10=\"urn:schemas-microsoft-com:office:word\" />\n" +
                                   "\t\t\t\t<lc:lockedCanvas xmlns:lc=\"http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas\">\n" +
                                   "\t\t\t\t\t<a:nvGrpSpPr>\n" +
                                   "\t\t\t\t\t\t<a:cNvPr id=\"0\" name=\"\" /> \n" +
                                   "\t\t\t\t\t\t<a:cNvGrpSpPr /> \n" +
                                   "\t\t\t\t\t</a:nvGrpSpPr>\n" +
                                   "\t\t\t\t</lc:lockedCanvas>\n" +
                                   "\t\t\t<w10:blip cstate=\"print\" r:embed=\"rId8\" " +
                                        "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" " +
                                        "xmlns:w10=\"urn:schemas-microsoft-com:office:word\" />\n" +
                                   "\t\t\t</a:graphicData>\n" +
                                   "\t\t</a:graphic>\n" +
                                   "\t</w:body>";

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.ASCII.GetBytes(input)));
            while ("body" != reader.LocalName)
                reader.Read();
            //check prerequisites
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
            Assert.That(reader.Prefix, Is.EqualTo("w"));
            Assert.That(reader.LocalName, Is.EqualTo("body"));
            //read outer xml
            Assert.That(XmlUtilPal.ReadOuterXml(reader), Is.EqualTo(bodyNodeOuter));
            //check cursor properly positioned after outer xml was read.
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Whitespace)); // TEXT in Java
            Assert.That(reader.LocalName, Is.EqualTo(""));
            reader.Read();
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.EndElement));
            Assert.That(reader.LocalName, Is.EqualTo("document"));
        }

        [Test]
        public void TestReadOuterXmlWithDefaultNamespace()
        {
            String documentNode = "<w:document " +
                                    "xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\" " +
                                    "xmlns:wne=\"http://schemas.microsoft.com/office/word/2006/wordml\" " +
                                    "xmlns=\"urn:schemas-microsoft-com:office:word\">\n" +
                                  "\t<w:body>\n" +
                                  "\t\t<a:graphic xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">\n" +
                                  "\t\t\t<a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas\">\n" +
                                  "\t\t\t\t<lc:lockedCanvas xmlns:lc=\"http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas\">\n" +
                                  "\t\t\t\t\t<a:nvGrpSpPr>\n" +
                                  "\t\t\t\t\t\t<a:cNvPr id=\"0\" name=\"\" /> \n" +
                                  "\t\t\t\t\t\t<a:cNvGrpSpPr /> \n" +
                                  "\t\t\t\t\t</a:nvGrpSpPr>\n" +
                                  "\t\t\t\t</lc:lockedCanvas>\n" +
                                  "\t\t\t</a:graphicData>\n" +
                                  "\t\t</a:graphic>\n" +
                                  "\t</w:body>\n" +
                                  "</w:document>";
            String outerXml = "<w:document " +
                                "xmlns=\"urn:schemas-microsoft-com:office:word\" " +
                                "xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\" " +
                                "xmlns:wne=\"http://schemas.microsoft.com/office/word/2006/wordml\">\n" +
                              "\t<w:body>\n" +
                              "\t\t<a:graphic xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">\n" +
                              "\t\t\t<a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas\">\n" +
                              "\t\t\t\t<lc:lockedCanvas xmlns:lc=\"http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas\">\n" +
                              "\t\t\t\t\t<a:nvGrpSpPr>\n" +
                              "\t\t\t\t\t\t<a:cNvPr id=\"0\" name=\"\" /> \n" +
                              "\t\t\t\t\t\t<a:cNvGrpSpPr /> \n" +
                              "\t\t\t\t\t</a:nvGrpSpPr>\n" +
                              "\t\t\t\t</lc:lockedCanvas>\n" +
                              "\t\t\t</a:graphicData>\n" +
                              "\t\t</a:graphic>\n" +
                              "\t</w:body>\n" +
                              "</w:document>";
            String input = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?> \n" + documentNode;

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.ASCII.GetBytes(input)));
            while ("document" != reader.LocalName)
                reader.Read();
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
            Assert.That(reader.Prefix, Is.EqualTo("w"));
            Assert.That(reader.LocalName, Is.EqualTo("document"));
            Assert.That(XmlUtilPal.ReadOuterXml(reader), Is.EqualTo(outerXml));
        }

        /// <summary>
        /// WORDSNET-14030 ArgumentOutOfRangeException occurred if a CDATA section is encountered on reading
        /// outer XML. Now support of CDATA XML sections is implemented in the XmlUtilPal.ReadOuterXml method.
        /// </summary>
        [Test]
        public void TestReadOuterXmlWithCdata()
        {
            string input = "<MyXml><![CDATA[ cdata stuff ]]></MyXml>";
            // JAVA StAX parser with coalescing setting (our default config) reports CDATA as CHARACTERS.
            // As a result - no exception in original 14030 issue.
            string output =
#if JAVA
                "<MyXml> cdata stuff </MyXml>";
#else
                input;
#endif

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.ASCII.GetBytes(input)));

            reader.Read();
            Assert.That(reader.LocalName, Is.EqualTo("MyXml"));
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));

            string outerXml = XmlUtilPal.ReadOuterXml(reader);
            Assert.That(outerXml, Is.EqualTo(output));

            reader.Close();
        }
    }
}
