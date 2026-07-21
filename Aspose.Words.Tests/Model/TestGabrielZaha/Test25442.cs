// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/07/2023 by amorozov

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Aspose.Common;
using Aspose.JavaAttributes;
using Aspose.Words.Markup;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// WORDSNET-25442 The style changed after xmlns modification
    /// Improved XML mapping.
    /// </summary>
    [JavaDelete]
    public class Test25442
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
            Aspose.Words.Loading.LoadOptions loadOptions = new Aspose.Words.Loading.LoadOptions();
            loadOptions.LoadFormat = LoadFormat.Docx;
            loadOptions.MswVersion = MsWordVersion.Word2019;
            Document document = TestUtil.Open(@"Model\Markup\Test25442.docx", loadOptions);
            IDictionary<string, string> newPartMappings = new Dictionary<string, string>();
            foreach (CustomXmlPart cxp in document.CustomXmlParts)
            {
                string xmlns = cxp.Schemas.FirstOrDefault();
                if (string.IsNullOrEmpty(xmlns))
                    continue;
                if (xmlns.StartsWith("urn:test1"))
                {
                    string componentID = xmlns.Split(':')[1];
                    xmlns = xmlns.Replace(componentID, "test2");
                    I iPart = Deserialize<I>(cxp.Data);
                    string customXmlPart = Serialize<I>(iPart, xmlns);
                    cxp.Schemas.RemoveAt(0);
                    cxp.Schemas.Add(xmlns);
                    cxp.Data = Encoding.UTF8.GetBytes(customXmlPart);
                    if (!newPartMappings.ContainsKey(componentID))
                        newPartMappings.Add(componentID, cxp.Id);
                }
            }
            UpdateBinding(document, newPartMappings);
            // No update page layout in FOSS.
            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.Docx);
            saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;

            document = TestUtil.SaveOpen(document, @"Model\Markup\Test25442.docx", saveOptions, false);

            Story header = document.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary];
            Assert.That(header.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));

            Paragraph para = header.FirstParagraph;
            Assert.That(para.ParaPr[ParaAttr.Istd], Is.EqualTo(0x47));
            Assert.That(para.ParaPr[ParaAttr.BorderBottom], IsNot.Null());
        }

        public static void UpdateBinding(Document document, IDictionary<string, string> newPartMappings)
        {
            foreach (StructuredDocumentTag contentControl in document.GetChildNodes(NodeType.StructuredDocumentTag, true))
            {
                if (!string.IsNullOrEmpty(contentControl.XmlMapping.PrefixMappings) && contentControl.XmlMapping.PrefixMappings.Contains("urn:test1"))
                {
                    string xmlns = contentControl.XmlMapping.PrefixMappings.Replace("xmlns:ns0", string.Empty).Replace("'", string.Empty).Replace(" ", string.Empty).Replace("=", string.Empty);
                    string componetID = xmlns.Split(':')[1];
                    if (!newPartMappings.ContainsKey(componetID))
                        continue;
                    string storeID = newPartMappings[componetID];
                    CustomXmlPart newPart = document.CustomXmlParts.OfType<CustomXmlPart>().Where(x => storeID.Equals(x.Id)).FirstOrDefault();
                    if (newPart != null)
                    {
                        contentControl.XmlMapping.SetMapping(newPart, contentControl.XmlMapping.XPath, string.Empty);
                    }
                }
            }
        }

        public static string Serialize<T>(I newINPart, string ns)
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

        public static T Deserialize<T>(byte[] xmlBytes)
        {
            using (var stream = new MemoryStream(xmlBytes))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(new NamespaceIgnorantXmlTextReader(stream));
            }
        }

        public class NamespaceIgnorantXmlTextReader : XmlTextReader
        {
            public NamespaceIgnorantXmlTextReader(Stream reader) : base(reader)
            { }
            public override string NamespaceURI
            {
                get { return ""; }
            }
        }

        [XmlRoot]
        public class I
        {
            [XmlText]
            public string C { get; set; }
        }
    }
}
