// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/10/2025 by Alexey Morozov

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Aspose.Common;
using Aspose.JavaAttributes;
using Aspose.Words.Markup;
using Aspose.Words.Saving;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// WORDSNET-28725 InvalidOperationException when calling UpdatePageLayout() after updating CustomXmlPart and changing namespace URI
    /// Fixed per WORDSNET-28747.
    /// </summary>
    [JavaDelete]
    public class Test28725
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


        private static void UpdateCustomXMLPartDataAndSetMapping(Document document)
        {
            foreach (StructuredDocumentTag structuredDocumentTag in document.Range.StructuredDocumentTags)
            {
                if (structuredDocumentTag.XmlMapping.IsMapped)
                {
                    if (structuredDocumentTag.Title == "Comp1")
                    {
                        string componentxml = File.ReadAllText(TestUtil.BuildTestFileName(@"Model\Markup\Test28725 Comp1.xml"), Encoding.UTF8);
                        string namespaceUri = structuredDocumentTag.XmlMapping.CustomXmlPart.Schemas.FirstOrDefault();
                        CustomXmlPart customXmlPart = document.CustomXmlParts.Where(cxp => cxp.Schemas.FirstOrDefault() == namespaceUri).FirstOrDefault();
                        test cxpData = Serializer.Deserialize<test>(customXmlPart.Data);
                        cxpData.Content = componentxml;
                        string xmlns = "test:co:content:000003";
                        customXmlPart.Schemas.RemoveAt(0);
                        customXmlPart.Schemas.Add(xmlns);
                        string xmlString = Serializer.Serialize<test>(cxpData, xmlns);
                        customXmlPart.Data = Encoding.UTF8.GetBytes(xmlString);
                        string updatedXmlns = customXmlPart.Schemas.Count > 0 ? customXmlPart.Schemas.FirstOrDefault() : string.Empty;
                        structuredDocumentTag.XmlMapping.SetMapping(customXmlPart, structuredDocumentTag.XmlMapping.XPath, $"xmlns:ns0='{updatedXmlns}'");
                    }
                    if (structuredDocumentTag.Title == "Comp2")
                    {
                        string componentxml = File.ReadAllText(TestUtil.BuildTestFileName(@"Model\Markup\Test28725 Comp2.xml"), Encoding.UTF8);
                        string namespaceUri = structuredDocumentTag.XmlMapping.CustomXmlPart.Schemas.FirstOrDefault();
                        CustomXmlPart customXmlPart = document.CustomXmlParts.Where(cxp => cxp.Schemas.FirstOrDefault() == namespaceUri).FirstOrDefault();
                        test cxpData = Serializer.Deserialize<test>(customXmlPart.Data);
                        cxpData.Content = componentxml;
                        string xmlns = "test:co:content:000004";
                        customXmlPart.Schemas.RemoveAt(0);
                        customXmlPart.Schemas.Add(xmlns);
                        string xmlString = Serializer.Serialize<test>(cxpData, xmlns);
                        customXmlPart.Data = Encoding.UTF8.GetBytes(xmlString);
                        string updatedXmlns = customXmlPart.Schemas.Count > 0 ? customXmlPart.Schemas.FirstOrDefault() : string.Empty;
                        structuredDocumentTag.XmlMapping.SetMapping(customXmlPart, structuredDocumentTag.XmlMapping.XPath, $"xmlns:ns0='{updatedXmlns}'");
                    }
                }
            }
            // No update page layout in FOSS.
            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.Docx);
            saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;

            // No exception occurred.
            document.Save(@"Model\Markup\Test28725.docx", saveOptions);
        }

        private static class Serializer
        {
            public static string Serialize<T>(test newINPart, string compNs)
            {
                StringBuilder stringBuilder = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NewLineChars = "\r\n";
                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    xmlWriter.WriteStartElement("Test", compNs);
                    xmlWriter.WriteString(newINPart.Content);
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

            private class NamespaceIgnorantXmlTextReader : XmlTextReader
            {
                public NamespaceIgnorantXmlTextReader(TextReader reader) : base(reader) { }
                public NamespaceIgnorantXmlTextReader(Stream reader) : base(reader) { }

                public override string NamespaceURI
                {
                    get { return ""; }
                }
            }
        }

        [Serializable]
        [XmlRoot("Test")]
        public class test
        {
            [XmlText]
            public string Content { get; set; }
        }
    }
}
