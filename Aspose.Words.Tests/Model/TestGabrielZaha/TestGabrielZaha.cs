// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/07/2022 by Alexey Morozov
#if !NETSTANDARD
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Aspose.Common;
using Aspose.JavaAttributes;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests cases from Gabriel Zaha.
    /// Priority support customer provides massive cases related to XmlMapping, I think it deserves separate section.
    /// </summary>
    [TestFixture]
    [JavaDelete("Not portable to Java Serializable Xml")]
    public class TestGabrielZaha
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [SetUp]
        public void Init()
        {
            RandomUtil.Reset();
        }



        /// <summary>
        /// WORDSNET-23735 Wrong list numbering due to loss and non-use of DurableId attribute values
        /// Checks the entire customer case. There are also tests located in <see cref="TestNumbering"/>.
        /// </summary>
        [Test]
        public void Test23735()
        {
            RandomUtil.Reset();

            const string fileName = @"Model\List\Test23735.docx";
            Document document = TestUtil.Open(fileName);

            foreach (StructuredDocumentTag contentControl in document.GetChildNodes(NodeType.StructuredDocumentTag, true))
            {
                I23735 xmlPart = new I23735();
                string contentControlXml = ExtractContent(contentControl, false);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(contentControlXml);
                xmlPart.C = xmlDocument.OuterXml;

                string xmlns = string.Format("urn:in:{0}", contentControl.Title.Replace(" ", string.Empty));
                string xml = Serializer.Serialize(xmlPart.C, "I", xmlns);
                CustomXmlPart cxp = document.CustomXmlParts.Add(RandomUtil.NewGuid().ToString(), xml);
                contentControl.XmlMapping.SetMapping(cxp, "/ns0:I[1]", string.Format("xmlns:ns0='{0}'", xmlns));
            }

            TestUtil.Save(document, fileName, null, true, GoldLevel.ExportOnly);
        }

        /// <summary>
        /// WORDSNET-23889 Wrong list numbering in SDT bound to custom XML part
        /// This is the customer's case as is. There is also the test <see cref="TestNumbering.Test23889"/>.
        /// </summary>
        [Test]
        public void Test23889()
        {
            RandomUtil.Reset();

            const string fileName = @"Model\List\Test23889.docx";
            Document document = TestUtil.Open(fileName);

            Dictionary<string, string> newPartMappings = new Dictionary<string, string>();
            foreach (CustomXmlPart cxp in document.CustomXmlParts)
            {
                string xmlns = cxp.Schemas.Count > 0 ? cxp.Schemas[0] : null;
                if (string.IsNullOrEmpty(xmlns))
                    continue;

                if (xmlns.StartsWith("urn:in:component"))
                {
                    string componentId = xmlns.Split(':')[3];
                    xmlns = xmlns.Replace(componentId, RandomUtil.NewGuid().ToString());
                    INPart componentDataPart = Serializer.Deserialize<INPart>(cxp.Data);
                    string customXmlPart = Serializer.Serialize(componentDataPart.Content, "INPart", xmlns);

                    cxp.Schemas.RemoveAt(0);
                    cxp.Schemas.Add(xmlns);
                    cxp.Data = Encoding.UTF8.GetBytes(customXmlPart);
                    if (!newPartMappings.ContainsKey(componentId))
                        newPartMappings.Add(componentId, cxp.Id);
                }
            }

            UpdateBinding(document, newPartMappings);

            TestUtil.Save(document, fileName, null, true, GoldLevel.ExportOnly);
        }

        /// <summary>
        /// WORDSNET-24169 Wrong list numbering in SDT bound to custom XML part
        /// This is the customer's case as is. There is also the test <see cref="TestNumbering.Test24169"/>.
        /// </summary>
        [Test]
        public void Test24169()
        {
            const string fileName = @"Model\List\Test24169.docx";
            Document document = TestUtil.Open(fileName);

            foreach (StructuredDocumentTag contentControl in document.GetChildNodes(NodeType.StructuredDocumentTag, true))
            {
                string xmlns = "urn:in:" + contentControl.Title.Replace(" ", string.Empty);
                I xmlPart = new I();
                string componentContentXml = ExtractContent(contentControl, true);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(componentContentXml);
                xmlPart.Content = xmlDocument.OuterXml;
                string customXmlPart = Serializer.Serialize(xmlPart, xmlns);
                CustomXmlPart cxp = document.CustomXmlParts.Add(RandomUtil.NewGuid().ToString(), customXmlPart);
                contentControl.XmlMapping.SetMapping(cxp, "/ns0:I[1]", string.Format("xmlns:ns0='{0}'", xmlns));
            }

            TestUtil.SaveCheckGoldExportOnly(document, fileName);
        }

        /// <summary>
        /// WORDSNET-24169 Wrong list numbering in SDT bound to custom XML part
        /// This is a case similar to the customer's one, but using the <see cref="StructuredDocumentTag.WordOpenXML"/>
        /// property.
        /// </summary>
        [Test]
        public void Test24169WordOpenXML()
        {
            Document document = TestUtil.Open(@"Model\List\Test24169.docx");

            SetMappedContentForSdts(document);

            TestUtil.SaveCheckGoldExportOnly(document, @"Model\List\Test24169WordOpenXML.docx");
        }

        /// <summary>
        /// WORDSNET-24169 Wrong list numbering in SDT bound to custom XML part
        /// Checks the customer's case 3.
        /// </summary>
        [Test]
        public void Test24169Case3()
        {
            Aspose.Words.Loading.LoadOptions loadOptions = new Aspose.Words.Loading.LoadOptions();
            loadOptions.LoadFormat = LoadFormat.Docx;
            loadOptions.Encoding = Encoding.UTF8;
            loadOptions.MswVersion = Aspose.Words.Settings.MsWordVersion.Word2019;

            Document document = TestUtil.Open(@"Model\List\Test24169Case3.docx", loadOptions);
            document.CustomXmlParts.Clear();
            document.CustomDocumentProperties.Clear();
            document.BuiltInDocumentProperties.Clear();
            document.PackageCustomParts.Clear();
            document.RemoveMacros();
            document.UpdateTableLayout();
            document.AutomaticallyUpdateStyles = false;
            document.AttachedTemplate = "";

            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.FlatOpc);
            saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;

            TestUtil.Save(document, @"Model\List\Test24169Case3.xml", saveOptions, true, GoldLevel.ExportOnly);
        }

        /// <summary>
        /// WORDSNET-24169 Wrong list numbering in SDT bound to custom XML part
        /// Checks the customer's case 4.
        /// </summary>
        [Test]
        public void Test24169Case4()
        {
            Aspose.Words.Loading.LoadOptions loadOptions = new Aspose.Words.Loading.LoadOptions();
            loadOptions.LoadFormat = LoadFormat.FlatOpc;
            Document document = TestUtil.Open(@"Model\List\Test24169Case4.xml", loadOptions);
            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.Docx);
            saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;
            TestUtil.Save(document, @"Model\List\Test24169Case4.docx", saveOptions, true, GoldLevel.ExportOnly);
        }


        /// <summary>
        /// WORDSNET-22815 Issues after Set XML Mapping
        /// XmlMapping improved to get closer to Word 2019.
        /// </summary>
        [Test]
        public void Test22815()
        {
            const string testName = @"Model\Markup\Test22815";

            Document document = TestUtil.Open(testName, LoadFormat.Docx);
            foreach (StructuredDocumentTag sdt in document.GetChildNodes(NodeType.StructuredDocumentTag, true))
            {
                CustomXmlPart customXmlPart = null;
                XmlDocument xmlDocument = null;
                string xmlns = string.Empty;
                I_22815 iPart = new I_22815();
                xmlns = "urn:" + sdt.Title;
                iPart.xmlns = xmlns;
                string partString = I_22815.Serialize<I_22815>(iPart);
                customXmlPart = document.CustomXmlParts.Add(Guid.NewGuid().ToString("B"), partString);
                customXmlPart.Schemas.Add(xmlns);
                string componentContentXml = I_22815.GetComponentXML(sdt, document);
                xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(componentContentXml);
                iPart.Content = xmlDocument.OuterXml;

                string outName = string.Format(@"{0} {1}.xml", testName, sdt.Title);
                outName = TestUtil.BuildTestFileName(outName);
                outName = TestUtil.BuildOutFileName(outName, "", SaveFormat.FlatOpc);

                File.WriteAllText(outName, xmlDocument.OuterXml);
                sdt.XmlMapping.SetMapping(customXmlPart, "/ns0:I[1]", string.Format("xmlns:ns0='{0}'", xmlns));
                string xmlString = I_22815.Serialize<I_22815>(iPart);
                customXmlPart.Data = Encoding.UTF8.GetBytes(xmlString);
            }

            document = TestUtil.SaveOpen(document, testName, UnifiedScenario.Docx2DocxNoGold);

            NodeCollection sdts = document.GetChildNodes(NodeType.StructuredDocumentTag, true);
            Assert.That(sdts.Count, Is.EqualTo(2));

            Assert.That(((StructuredDocumentTag)sdts[0]).GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(6));
            Assert.That(((StructuredDocumentTag)sdts[1]).GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(3));
        }

        /// <summary>
        /// WORDSNET-23435 Revisions changed after adding CustomXmlPart.
        /// Checks that setting of mapped content for SDT's does not increase revisions count.
        /// </summary>
        [Test]
        public void Test23435()
        {
            Document doc = TestUtil.Open(@"Model\Revision\Test23435.docx");

            int revisionsCountBefore = doc.Revisions.Groups.Count;
            SetMappedContentForSdts(doc);

            doc = TestUtil.SaveOpen(doc, @"Model\Revision\Test23435", UnifiedScenario.Docx2DocxNoGold);
            // Actually the Word 2019 shows 25 revision groups. However, AW provides 27 revision groups.
            Assert.That(doc.Revisions.Groups.Count, Is.EqualTo(revisionsCountBefore));
        }

        // FOSS: Test24739 (WORDSNET-24739 "bullet missing when mapping XML to SDT") removed. It verified the
        // fix by rendering to PDF and comparing a visual gold; PDF rendering is removed in FOSS. The SDT
        // XML-mapping mechanism itself is covered by TestSdtXmlMapping.

        /// <summary>
        /// WORDSNET-24602 File size increase after mapping
        /// Introduced new property to get minimal mapped document.
        /// </summary>
        [TestCase(@"Model\Markup\Test24602A.docx", 140 * Kb) /* 1.3 before */]
        [TestCase(@"Model\Markup\Test24602B.docx", 440 * Kb  /* 1.5M before */)]
        [TestCase(@"Model\Markup\Test24602C.docx", (int)(1.1 * Mb) /* 18M before */)]
        public void Test24602(string testName, int maxLength)
        {
            Document document = TestUtil.Open(testName);

            CustomXmlPartCollection customXmlParts = document.CustomXmlParts;
            int cxpCount = customXmlParts.Count;
            foreach (StructuredDocumentTag sdt in document.Range.StructuredDocumentTags)
            {
                if (sdt.XmlMapping.IsMapped)
                {
                    sdt.XmlMapping.Delete();
                }
            }
            for (int cxpIndex = cxpCount - 1; cxpIndex > -1; cxpIndex--)
            {
                customXmlParts.RemoveAt(cxpIndex);
            }
            cxpCount = 0;
            foreach (StructuredDocumentTag sdt in document.Range.StructuredDocumentTags)
            {
                cxpCount += 1;
                I componentDataPart = new I();
                componentDataPart.xmlns = $"urn:test{cxpCount}";

                // Use new property to get minimal document.
                string wordOpenXml = sdt.WordOpenXMLMinimal;

                componentDataPart.Content = wordOpenXml;

                string xmlString = Serializer_24602.Serialize<I_24602>(componentDataPart, componentDataPart.xmlns);
                string storeId = Guid.NewGuid().ToString();
                CustomXmlPart cxp = document.CustomXmlParts.Add(storeId, xmlString);
                cxp.Schemas.Add(componentDataPart.xmlns);
                sdt.XmlMapping.SetMapping(cxp, "/ns0:I[1]", string.Empty);
            }
            // No update page layout in FOSS.
            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.Docx);
            saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;
            saveOptions.CompressionLevel = CompressionLevel.Maximum;

            string outFilename = TestUtil.Save(document, testName, saveOptions, false);

            FileInfo fileInfo = new FileInfo(outFilename);
            Assert.That(fileInfo.Length, Is.LessThan(maxLength));
        }

        /// <summary>
        /// WORDSNET-25148 Problem with Captions and References in the first paragraph
        /// Handled duplicate bookmark renaming during XML mapping update.
        /// </summary>
        [Test]
        public void Test25148()
        {
            // Please check both options (WordOpenXML and ExtractContent method)
            // Option 1: Set true to use WordOpenXML Property
            // Option 2: Set false to use ExtractContent method
            bool useWordOpenXML = true;

            Aspose.Words.Loading.LoadOptions loadOptions = new Aspose.Words.Loading.LoadOptions();
            loadOptions.LoadFormat = LoadFormat.Docx;
            loadOptions.MswVersion = Aspose.Words.Settings.MsWordVersion.Word2019;
            Document document = TestUtil.Open(@"Model\Markup\Test25148.docx", loadOptions);
            CustomXmlPartCollection customXmlParts = document.CustomXmlParts;
            int cxpCount = customXmlParts.Count;
            // No update page layout in FOSS.
            foreach (Node node in document.GetChildNodes(NodeType.Any, true))
            {
                if (node.NodeType == NodeType.StructuredDocumentTag)
                {
                    StructuredDocumentTag sdt = (StructuredDocumentTag)node;
                    sdt.XmlMapping.Delete();
                }
                else if (node.NodeType == NodeType.StructuredDocumentTagRangeStart)
                {
                    StructuredDocumentTagRangeStart sdtRange = (StructuredDocumentTagRangeStart)node;
                    sdtRange.XmlMapping.Delete();
                }
            }
            for (int cxpIndex = cxpCount - 1; cxpIndex > -1; cxpIndex--)
            {
                customXmlParts.RemoveAt(cxpIndex);
            }
            cxpCount = 0;
            foreach (Node node1 in document.GetChildNodes(NodeType.Any, true))
            {
                if (node1.NodeType == NodeType.StructuredDocumentTag || node1.NodeType == NodeType.StructuredDocumentTagRangeStart)
                {
                    cxpCount += 1;
                    I componentDataPart = new I();
                    componentDataPart.xmlns = $"urn:test{cxpCount}";
                    string componentContentXml = string.Empty;

                    if (node1.NodeType == NodeType.StructuredDocumentTag)
                    {
                        StructuredDocumentTag sdt = (StructuredDocumentTag)node1;
                        if (useWordOpenXML)
                            componentContentXml = sdt.WordOpenXML;
                        else
                            componentContentXml = GetComponentXML(sdt, document);
                    }
                    else if (node1.NodeType == NodeType.StructuredDocumentTagRangeStart)
                    {
                        StructuredDocumentTagRangeStart sdtRange = (StructuredDocumentTagRangeStart)node1;
                        if (useWordOpenXML)
                            componentContentXml = sdtRange.WordOpenXML;
                        else
                            componentContentXml = GetComponentXML(sdtRange, document);
                    }
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(componentContentXml);
                    componentDataPart.Content = xmlDocument.OuterXml;
                    string xmlString = Serializer_25148.Serialize<I>(componentDataPart, componentDataPart.xmlns);
                    string storeId = Guid.NewGuid().ToString();
                    CustomXmlPart cxp = document.CustomXmlParts.Add(storeId, xmlString);
                    cxp.Schemas.Add(componentDataPart.xmlns);

                    if (node1.NodeType == NodeType.StructuredDocumentTag)
                    {
                        StructuredDocumentTag sdt = (StructuredDocumentTag)node1;
                        sdt.XmlMapping.SetMapping(cxp, "/ns0:I[1]", string.Format("xmlns:ns0='{0}'", componentDataPart.xmlns));
                    }
                    else if (node1.NodeType == NodeType.StructuredDocumentTagRangeStart)
                    {
                        StructuredDocumentTagRangeStart sdtRange = (StructuredDocumentTagRangeStart)node1;
                        sdtRange.XmlMapping.SetMapping(cxp, "/ns0:I[1]", string.Format("xmlns:ns0='{0}'", componentDataPart.xmlns));
                    }
                }
            }
            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.Docx);
            saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;
            document = TestUtil.SaveOpen(document, @"Model\Markup\Test25148.docx", saveOptions, false);

            StructuredDocumentTag tag = (StructuredDocumentTag)document.Range.StructuredDocumentTags[0];

            // Verify that bookmark is located at proper location and has correct name.
            BookmarkStart bkmkStart = (BookmarkStart)tag.PreviousSibling;
            Assert.That(bkmkStart.Bookmark.Name, Is.EqualTo("_Ref129620999"));

            BookmarkEnd bkmkEnd = (BookmarkEnd)((Paragraph)tag.FirstChild).LastChild;
            Assert.That(bkmkStart.Bookmark.BookmarkEnd, Is.EqualTo(bkmkEnd));
        }


        public static string GetComponentXML(Node contentControl, Document document)
        {
            Document componentDoc = new Document();
            componentDoc.EnsureMinimum();
            document.CompatibilityOptions.OptimizeFor(Aspose.Words.Settings.MsWordVersion.Word2019);
            componentDoc.CopyStylesFromTemplate(document);

            if (contentControl.NodeType == NodeType.StructuredDocumentTag)
                ExtractContentFromSdt(contentControl as StructuredDocumentTag, componentDoc, document);
            else if (contentControl.NodeType == NodeType.StructuredDocumentTagRangeStart)
                ExtractContentFromSdtRange(contentControl as StructuredDocumentTagRangeStart, componentDoc, document);

            //ValidateBookmarks(componentDoc);

            componentDoc.CustomXmlParts.Clear();
            componentDoc.CustomDocumentProperties.Clear();
            componentDoc.BuiltInDocumentProperties.Clear();
            componentDoc.PackageCustomParts.Clear();
            componentDoc.RemoveMacros();
            componentDoc.AutomaticallyUpdateStyles = false;
            MemoryStream stream = new MemoryStream();
            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.FlatOpc);
            saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;
            componentDoc.Save(stream, saveOptions);
            string componentContentXml = Encoding.UTF8.GetString(stream.GetBuffer(), 0, stream.GetBuffer().Length);
            string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (componentContentXml.Length > 0 && byteOrderMarkUtf8.Length > 0 && componentContentXml[0] == byteOrderMarkUtf8[0])
                componentContentXml = componentContentXml.Remove(0, byteOrderMarkUtf8.Length);
            Char nullCharacter = '\0';
            componentContentXml = componentContentXml.TrimEnd(nullCharacter);
            return componentContentXml;
        }

        public static void ExtractContentFromSdtRange(StructuredDocumentTagRangeStart contentControl, Document destinationDocument, Document sourceDocument)
        {
            if (contentControl.NodeType == NodeType.StructuredDocumentTagRangeStart)
            {
                destinationDocument.FirstSection.Body.Paragraphs.Clear();
                Body componentBody = destinationDocument.FirstSection.Body;
                StructuredDocumentTagRangeStart componentStart = destinationDocument.ImportNode(contentControl, false, ImportFormatMode.UseDestinationStyles) as StructuredDocumentTagRangeStart;
                componentBody.AppendChild(componentStart);
                Body currentBody = (Body)contentControl.ParentNode;
                foreach (Node node in contentControl.ChildNodes)
                {
                    if (node.ParentNode != currentBody)
                    {
                        currentBody = (Body)node.ParentNode;
                        componentBody = (Body)destinationDocument.ImportNode(currentBody, false, ImportFormatMode.UseDestinationStyles);
                        Node componentSection = destinationDocument.ImportNode(currentBody.ParentSection, false, ImportFormatMode.UseDestinationStyles);
                        destinationDocument.AppendChild(componentSection);
                        destinationDocument.LastSection.AppendChild(componentBody);
                    }

                    Node componentNode = destinationDocument.ImportNode(node, true, ImportFormatMode.UseDestinationStyles);
                    componentBody.AppendChild(componentNode);
                }
                StructuredDocumentTagRangeEnd componentEnd = new StructuredDocumentTagRangeEnd(destinationDocument, ((StructuredDocumentTagRangeStart)componentStart).Id);
                componentBody.AppendChild(componentEnd);
                componentStart.RemoveSelfOnly();
            }

        }

        public static void ExtractContentFromSdt(StructuredDocumentTag contentControl, Document destinationDocument, Document sourceDocument)
        {
            if (contentControl.NodeType == NodeType.StructuredDocumentTag)
            {
                destinationDocument.FirstSection.Body.RemoveAllChildren();
                // Added Workaround for separated comments from the main thread issue
                if (contentControl.GetChildNodes(NodeType.Comment, true).Count > 0 && contentControl.Level == MarkupLevel.Inline)
                {
                    StructuredDocumentTag inlineSDT = (StructuredDocumentTag)destinationDocument.ImportNode(contentControl, true, ImportFormatMode.UseDestinationStyles);
                    if (destinationDocument.LastSection.Body.LastParagraph != null)
                        destinationDocument.LastSection.Body.LastParagraph.AppendChild(inlineSDT);
                    else
                    {
                        Paragraph paragraph = new Paragraph(destinationDocument);
                        paragraph.AppendChild(inlineSDT);
                        destinationDocument.LastSection.Body.AppendChild(paragraph);
                    }
                    inlineSDT.RemoveSelfOnly();
                }
                else
                {
                    for (int index = 0; index < contentControl.GetChildNodes(NodeType.Any, false).Count; index++)
                    {
                        Node node = contentControl.GetChildNodes(NodeType.Any, false)[index];
                        Node importNode = destinationDocument.ImportNode(node, true, ImportFormatMode.UseDestinationStyles);
                        if (importNode.NodeType == NodeType.Cell)
                        {
                            Aspose.Words.Tables.Cell cell = (Aspose.Words.Tables.Cell)importNode;
                            while (cell.GetChildNodes(NodeType.Any, false)[0] != null)
                                destinationDocument.LastSection.Body.AppendChild(cell.GetChild(NodeType.Any, 0, false));
                        }
                        else if (importNode.NodeType == NodeType.StructuredDocumentTag)
                        {
                            StructuredDocumentTag sdt = (StructuredDocumentTag)importNode;
                            if (sdt.Level == MarkupLevel.Cell)
                            {
                                StructuredDocumentTag clonedSDT = new StructuredDocumentTag(sourceDocument, SdtType.RichText, MarkupLevel.Block);
                                StructuredDocumentTag sourceSDT = (StructuredDocumentTag)node;
                                clonedSDT.PlaceholderName = sourceSDT.Placeholder.Name;
                                if (!string.IsNullOrEmpty(sourceSDT.Range.Text))
                                {
                                    clonedSDT.IsShowingPlaceholderText = false;
                                }
                                clonedSDT.RemoveAllChildren();
                                clonedSDT.Title = sourceSDT.Title;
                                clonedSDT.Tag = sourceSDT.Tag;
                                Cell oldCell = sourceSDT.LastChild as Cell;
                                foreach (Node innerNode in oldCell.GetChildNodes(NodeType.Any, false))
                                {
                                    clonedSDT.AppendChild(innerNode);
                                }
                                sourceSDT.Clear();
                                sourceSDT.IsShowingPlaceholderText = false;
                                sourceSDT.RemoveSelfOnly();
                                oldCell.RemoveAllChildren();
                                oldCell.AppendChild(clonedSDT);
                                importNode = destinationDocument.ImportNode(clonedSDT, true, ImportFormatMode.UseDestinationStyles);
                                sdt = (StructuredDocumentTag)importNode;
                            }
                            if (destinationDocument.LastSection.Body.Count == 0 && sdt.Level != MarkupLevel.Block)
                            {
                                Paragraph paragraph = new Paragraph(destinationDocument);
                                paragraph.AppendChild(importNode);
                                destinationDocument.LastSection.Body.AppendChild(paragraph);
                            }
                            else if (destinationDocument.LastSection.Body.LastParagraph != null && sdt.Level == MarkupLevel.Inline)
                                destinationDocument.LastSection.Body.LastParagraph.AppendChild(importNode);
                            else
                                destinationDocument.LastSection.Body.AppendChild(importNode);
                        }
                        else if (importNode.NodeType == NodeType.Run || importNode.NodeType == NodeType.Comment || contentControl.Level == MarkupLevel.Inline)
                        {
                            if (destinationDocument.LastSection.Body.LastParagraph != null)
                                destinationDocument.LastSection.Body.LastParagraph.AppendChild(importNode);
                            else
                            {
                                Paragraph paragraph = new Paragraph(destinationDocument);
                                paragraph.AppendChild(importNode);
                                destinationDocument.LastSection.Body.AppendChild(paragraph);
                            }
                        }
                        else
                            destinationDocument.LastSection.Body.AppendChild(importNode);
                    }
                }
            }
        }

        public class Serializer_25148
        {
            public static string Serialize<T>(I part, string ns)
            {
                string content = string.Empty;
                if (!string.IsNullOrEmpty(part.Content))
                    content = part.Content;
                StringBuilder stringBuilder = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    xmlWriter.WriteStartElement("I", ns);
                    xmlWriter.WriteString(content);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
                return stringBuilder.ToString();
            }

            public static T Deserialize<T>(byte[] xmlBytes)
            {
                using (var stream = new MemoryStream(xmlBytes))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(new NamespaceIgnorantXmlTextReader(stream));
                }
            }
        }

        [Serializable]
        [XmlRoot("I")]
        public class I_24602
        {
            [XmlAttribute]
            public string xmlns { get; set; }
            public string Content { get; set; }
        }


        [Serializable]
        [XmlRoot("C")]
        public class C
        {
            [XmlText]
            public string Content { get; set; }
        }

        static string Serialize(string xmlns, string content) {
            StringBuilder stringBuilder = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings() {
                OmitXmlDeclaration = true,
                Indent = true
            };

            using(XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings)) {
                xmlWriter.WriteStartElement("I", xmlns);
                xmlWriter.WriteString(content);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Sets mapped content for SDT's using <see cref="StructuredDocumentTag.WordOpenXML"/> property of SDT's.
        /// </summary>
        private static void SetMappedContentForSdts(Document doc)
        {
            foreach (StructuredDocumentTag contentControl in doc.GetChildNodes(NodeType.StructuredDocumentTag, true))
            {
                string xmlns = "urn:in:" + contentControl.Title.Replace(" ", string.Empty);
                I xmlPart = new I();
                string componentContentXml = contentControl.WordOpenXML;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(componentContentXml);
                xmlPart.Content = xmlDocument.OuterXml;
                string customXmlPart = Serializer.Serialize(xmlPart, xmlns);
                CustomXmlPart cxp = doc.CustomXmlParts.Add(RandomUtil.NewGuid().ToString(), customXmlPart);
                contentControl.XmlMapping.SetMapping(cxp, "/ns0:I[1]", string.Format("xmlns:ns0='{0}'", xmlns));
            }
        }

        private static string Serialize<T>(I newIPart)
        {
            string ns = newIPart.xmlns;
            string content = string.Empty;
            if (!string.IsNullOrEmpty(newIPart.Content))
                content = newIPart.Content;
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                if (newIPart != null)
                {
                    xmlWriter.WriteStartElement("I", ns);
                    xmlWriter.WriteString(content);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();
                xmlWriter.Close();
            }
            return stringBuilder.ToString();
        }

        private static string ExtractContent(StructuredDocumentTag contentControl,
            bool optimizeForWord2019AndCopyStyles)
        {
            Document importDocument = new Document();

            if (optimizeForWord2019AndCopyStyles)
            {
                importDocument.CompatibilityOptions.OptimizeFor(Aspose.Words.Settings.MsWordVersion.Word2019);
                importDocument.CopyStylesFromTemplate((Document)contentControl.Document);
            }

            importDocument.FirstSection.Body.RemoveAllChildren();
            foreach (Node node in contentControl.GetChildNodes(NodeType.Any, false))
            {
                Node importNode = importDocument.ImportNode(node, true, ImportFormatMode.KeepSourceFormatting);
                if (importNode.NodeType == NodeType.Cell)
                {
                    Cell cell = (Cell)importNode;
                    while (cell.GetChildNodes(NodeType.Any, false)[0] != null)
                        importDocument.LastSection.Body.AppendChild(cell.GetChildNodes(NodeType.Any, false)[0]);
                }
                else if (importNode.NodeType == NodeType.StructuredDocumentTag)
                {
                    StructuredDocumentTag sdt = (StructuredDocumentTag)importNode;
                    if (importDocument.LastSection.Body.Count == 0 && sdt.Level != MarkupLevel.Block)
                    {
                        Paragraph paragraph = new Paragraph(importDocument);
                        paragraph.AppendChild(importNode);
                        importDocument.LastSection.Body.AppendChild(paragraph);
                    }
                    else if (importDocument.LastSection.Body.LastParagraph != null && sdt.Level == MarkupLevel.Inline)
                        importDocument.LastSection.Body.LastParagraph.AppendChild(importNode);
                    else
                        importDocument.LastSection.Body.AppendChild(importNode);
                }
                else if (importNode.NodeType == NodeType.Run || importNode.NodeType == NodeType.Comment ||
                         contentControl.Level == MarkupLevel.Inline)
                {
                    if (importDocument.LastSection.Body.LastParagraph != null)
                    {
                        importDocument.LastSection.Body.LastParagraph.AppendChild(importNode);
                    }
                    else
                    {
                        Paragraph paragraph = new Paragraph(importDocument);
                        paragraph.AppendChild(importNode);
                        importDocument.LastSection.Body.AppendChild(paragraph);
                    }
                }
                else
                {
                    importDocument.LastSection.Body.AppendChild(importNode);
                }
            }

            importDocument.CustomXmlParts.Clear();
            importDocument.CustomDocumentProperties.Clear();
            importDocument.BuiltInDocumentProperties.Clear();
            importDocument.PackageCustomParts.Clear();
            importDocument.RemoveMacros();
            importDocument.UpdateTableLayout();
            importDocument.AutomaticallyUpdateStyles = false;

            MemoryStream stream = new MemoryStream();
            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.FlatOpc);
            saveOptions.SetTestMode();
            saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;
            importDocument.Save(stream, saveOptions);

            string contentControlXml = Encoding.UTF8.GetString(stream.GetBuffer(), 0, stream.GetBuffer().Length);
            string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (contentControlXml.Length > 0 &&
                byteOrderMarkUtf8.Length > 0 &&
                contentControlXml[0] == byteOrderMarkUtf8[0])
            {
                contentControlXml = contentControlXml.Remove(0, byteOrderMarkUtf8.Length);
            }

            return contentControlXml;
        }

        /// <summary>
        /// Represents the customer's code to update SDT contents.
        /// </summary>
        private static void UpdateBinding(Document document, IDictionary<string, string> newPartMappings)
        {
            // Rebind all components to to cloned components
            foreach (StructuredDocumentTag contentControl in document.GetChildNodes(NodeType.StructuredDocumentTag, true))
            {
                if (StringUtil.HasChars(contentControl.XmlMapping.PrefixMappings) &&
                    contentControl.XmlMapping.PrefixMappings.Contains("urn:in:component"))
                {
                    string xmlns = contentControl.XmlMapping.PrefixMappings
                        .Replace("xmlns:ns0", string.Empty)
                        .Replace("'", string.Empty)
                        .Replace(" ", string.Empty)
                        .Replace("=", string.Empty);

                    string componentId = xmlns.Split(':')[3];
                    if (!newPartMappings.ContainsKey(componentId))
                        continue;

                    string storeId = newPartMappings[componentId];
                    CustomXmlPart newPart = document.CustomXmlParts.GetById(storeId);
                    if (newPart != null)
                    {
                        string xmlnsNew = newPart.Schemas.Count > 0 ? newPart.Schemas[0] : null;
                        contentControl.XmlMapping.SetMapping(newPart, contentControl.XmlMapping.XPath,
                            string.Format("xmlns:ns0='{0}'", xmlnsNew));
                    }
                }
            }
        }

        [Serializable]
        [XmlRoot("I")]
        public class I
        {
            [XmlText]
            public string Content { get; set; }
            [XmlAttribute]
            public string xmlns { get; set; }
        }

        [XmlRoot("I")]
        public class I23735
        {
            [XmlText]
            public string C { get; set; }
        }

        [Serializable]
        [XmlRoot("I")]
        public class I_22815
        {
            [XmlText]
            public string Content { get; set; }
            [XmlAttribute]
            public string xmlns { get; set; }
            static void Main(string[] args)
            {
            }

            public static string GetComponentXML(StructuredDocumentTag contentControl, Document document)
            {
                Document componentDoc = new Document();
                componentDoc.CopyStylesFromTemplate(document);
                CreateCXP(componentDoc, contentControl);
                MemoryStream stream = new MemoryStream();
                OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.FlatOpc);
                saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;
                componentDoc.Save(stream, saveOptions);
                string componentContentXml = Encoding.UTF8.GetString(stream.GetBuffer(), 0, stream.GetBuffer().Length);
                string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                if (componentContentXml.Length > 0 && byteOrderMarkUtf8.Length > 0 && componentContentXml[0] == byteOrderMarkUtf8[0])
                    componentContentXml = componentContentXml.Remove(0, byteOrderMarkUtf8.Length);
                return componentContentXml;
            }

            public static void CreateCXP(Document destinationDocument, StructuredDocumentTag contentControl)
            {
                // Handle multi-section structured document tags.
                if (contentControl.NodeType == NodeType.StructuredDocumentTagRangeStart)
                {
                    destinationDocument.FirstSection.Body.Paragraphs.Clear();
                    Body componentBody = destinationDocument.FirstSection.Body;
                    StructuredDocumentTagRangeStart componentStart = destinationDocument.ImportNode(contentControl, false, ImportFormatMode.KeepSourceFormatting) as StructuredDocumentTagRangeStart;
                    componentBody.AppendChild(componentStart);
                    Body currentBody = (Body)contentControl.ParentNode;
                    foreach (Node node in contentControl.GetChildNodes(NodeType.Any, false))
                    {
                        if (node.ParentNode != currentBody)
                        {
                            currentBody = (Body)node.ParentNode;
                            componentBody = (Body)destinationDocument.ImportNode(currentBody, false, ImportFormatMode.KeepSourceFormatting);
                            Node componentSection = destinationDocument.ImportNode(currentBody.ParentSection, false, ImportFormatMode.KeepSourceFormatting);
                            destinationDocument.AppendChild(componentSection);
                            destinationDocument.LastSection.AppendChild(componentBody);
                        }

                        Node componentNode = destinationDocument.ImportNode(node, true, ImportFormatMode.KeepSourceFormatting);
                        componentBody.AppendChild(componentNode);
                    }
                    StructuredDocumentTagRangeEnd componentEnd = new StructuredDocumentTagRangeEnd(destinationDocument, ((StructuredDocumentTagRangeStart)componentStart).Id);
                    componentBody.AppendChild(componentEnd);
                    componentStart.RemoveSelfOnly();
                }
                else if (contentControl.NodeType == NodeType.StructuredDocumentTag)
                {
                    destinationDocument.FirstSection.Body.RemoveAllChildren();
                    // Added Workaround for separated comments from the main thread issue
                    if (contentControl.GetChildNodes(NodeType.Comment, true).Count > 0 && contentControl.Level == MarkupLevel.Inline)
                    {
                        StructuredDocumentTag inlineSDT = (StructuredDocumentTag)destinationDocument.ImportNode(contentControl, true, ImportFormatMode.KeepSourceFormatting);
                        if (destinationDocument.LastSection.Body.LastParagraph != null)
                            destinationDocument.LastSection.Body.LastParagraph.AppendChild(inlineSDT);
                        else
                        {
                            Paragraph paragraph = new Paragraph(destinationDocument);
                            paragraph.AppendChild(inlineSDT);
                            destinationDocument.LastSection.Body.AppendChild(paragraph);
                        }
                        inlineSDT.RemoveSelfOnly();
                    }
                    else
                    {
                        foreach (Node node in contentControl.GetChildNodes(NodeType.Any, false))
                        {
                            Node importNode = destinationDocument.ImportNode(node, true, ImportFormatMode.KeepSourceFormatting);
                            if (importNode.NodeType == NodeType.Cell)
                            {
                                Aspose.Words.Tables.Cell cell = (Aspose.Words.Tables.Cell)importNode;
                                while (cell.GetChildNodes(NodeType.Any, false)[0] != null)
                                    destinationDocument.LastSection.Body.AppendChild(cell.GetChild(NodeType.Any, 0, false));
                            }
                            else if (importNode.NodeType == NodeType.StructuredDocumentTag)
                            {
                                StructuredDocumentTag sdt = (StructuredDocumentTag)importNode;
                                if (destinationDocument.LastSection.Body.Count == 0 && sdt.Level != MarkupLevel.Block)
                                {
                                    Paragraph paragraph = new Paragraph(destinationDocument);
                                    paragraph.AppendChild(importNode);
                                    destinationDocument.LastSection.Body.AppendChild(paragraph);
                                }
                                else if (destinationDocument.LastSection.Body.LastParagraph != null && sdt.Level == MarkupLevel.Inline)
                                    destinationDocument.LastSection.Body.LastParagraph.AppendChild(importNode);
                                else
                                    destinationDocument.LastSection.Body.AppendChild(importNode);
                            }
                            else if (importNode.NodeType == NodeType.Run || importNode.NodeType == NodeType.Comment || contentControl.Level == MarkupLevel.Inline)
                            {
                                if (destinationDocument.LastSection.Body.LastParagraph != null)
                                    destinationDocument.LastSection.Body.LastParagraph.AppendChild(importNode);
                                else
                                {
                                    Paragraph paragraph = new Paragraph(destinationDocument);
                                    paragraph.AppendChild(importNode);
                                    destinationDocument.LastSection.Body.AppendChild(paragraph);
                                }
                            }
                            else
                                destinationDocument.LastSection.Body.AppendChild(importNode);
                        }
                    }

                }
            }

            public static string Serialize<T>(I_22815 newIPart)
            {
                string ns = newIPart.xmlns;
                string content = string.Empty;
                if (!string.IsNullOrEmpty(newIPart.Content))
                    content = newIPart.Content;
                StringBuilder stringBuilder = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    if (newIPart != null)
                    {
                        xmlWriter.WriteStartElement("I", ns);
                        xmlWriter.WriteString(content);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
                return stringBuilder.ToString();
            }
        }

        [XmlRoot]
        public class INPart
        {
            [XmlText]
            public string Content { get; set; }
        }

        private class Serializer
        {
            public static string Serialize(I part, string ns)
            {
                return Serialize(part.Content, "I", ns);
            }

            internal static string Serialize(string content, string name, string ns)
            {
                if (content == null)
                    content = string.Empty;

                StringBuilder stringBuilder = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;

                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    xmlWriter.WriteStartElement(name, ns);
                    xmlWriter.WriteString(content);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
                return stringBuilder.ToString();
            }

            public static T Deserialize<T>(byte[] xmlBytes)
            {
                using (var stream = new MemoryStream(xmlBytes))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(new NamespaceIgnorantXmlTextReader(stream));
                }
            }
        }

        public class Serializer_24602
        {
            public static string Serialize<T>(I part, string ns)
            {
                string content = string.Empty;
                if (!string.IsNullOrEmpty(part.Content))
                    content = part.Content;
                StringBuilder stringBuilder = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    xmlWriter.WriteStartElement("I", ns);
                    xmlWriter.WriteString(content);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
                return stringBuilder.ToString();
            }

            public static T Deserialize<T>(byte[] xmlBytes)
            {
                using (var stream = new MemoryStream(xmlBytes))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(new NamespaceIgnorantXmlTextReader(stream));
                }
            }
        }

        private class NamespaceIgnorantXmlTextReader : XmlTextReader
        {
            public NamespaceIgnorantXmlTextReader(TextReader reader) : base(reader)
            { }
            public NamespaceIgnorantXmlTextReader(Stream reader) : base(reader)
            { }
            public override string NamespaceURI
            {
                get { return ""; }
            }
        }


        private static string Serialize26905<T>(I26905 newINPart, string ns)
        {
            string content = string.Empty;
            if (!string.IsNullOrEmpty(newINPart.C))
                content = newINPart.C;
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.NewLineChars = "\r\n";
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                xmlWriter.WriteStartElement("I", ns);
                xmlWriter.WriteString(content);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();
                xmlWriter.Close();
            }
            return stringBuilder.ToString();
        }


        [XmlRoot]
        public class I26905
        {
            [XmlText]
            public string C { get; set; }
        }

        private const int Kb = 1024;
        private const int Mb = Kb * 1024;
    }
}
#endif
