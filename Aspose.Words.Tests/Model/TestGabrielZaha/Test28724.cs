// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/10/2025 by Alexey Morozov

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
    /// WORDSNET-28724 InvalidOperationException when saving DOCX after updating CustomXmlPart
    /// Made quick resilience for the case.
    /// </summary>
    [JavaDelete]
    public class Test28724
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

        [Test]
        public void Test()
        {
            Loading.LoadOptions loadOptions = new Loading.LoadOptions();
            loadOptions.LoadFormat = LoadFormat.Docx;
            loadOptions.MswVersion = Settings.MsWordVersion.Word2019;

            Document document = TestUtil.Open(@"Model\Markup\Test28724.docx", loadOptions);
            UpdateCustomXMLPartData(document);
        }

        private static void UpdateCustomXMLPartData(Document document)
        {
            string componentxml =
                File.ReadAllText(TestUtil.BuildTestFileName(@"Model\Markup\Test28724 test.xml"), Encoding.UTF8);
            foreach (StructuredDocumentTag structuredDocumentTag in document.Range.StructuredDocumentTags)
            {

                if (structuredDocumentTag.Title == "Comp1")
                {
                    string namespaceUri = structuredDocumentTag.XmlMapping.CustomXmlPart.Schemas.FirstOrDefault();
                    CustomXmlPart customXmlPart = document.CustomXmlParts.FirstOrDefault(cxp => cxp.Schemas.FirstOrDefault() == namespaceUri);
                    test cxpData = Serializer.Deserialize<test>(customXmlPart.Data);
                    cxpData.Content = componentxml;
                    string xmlString = Serializer.Serialize<test>(cxpData, namespaceUri);
                    customXmlPart.Data = Encoding.UTF8.GetBytes(xmlString);
                }
            }

            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.Docx);
            saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;

            // Test no exception occured.
            TestUtil.Save(document, @"Model\Markup\Test28724.docx", saveOptions, false);
        }

        private class NamespaceIgnorantXmlTextReader : XmlTextReader
        {
            public NamespaceIgnorantXmlTextReader(TextReader reader)
                : base(reader)
            {
            }

            public NamespaceIgnorantXmlTextReader(Stream reader)
                : base(reader)
            {
            }

            public override string NamespaceURI
            {
                get { return ""; }
            }
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
