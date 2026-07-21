// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/04/2024 by Alexey Morozov

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Aspose.Common;
using Aspose.JavaAttributes;
using Aspose.Words.Markup;
using Aspose.Words.Settings;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// WORDSNET-26636 Continuation of WORDSNET-26174: Table Border Lost for Inline Component
    /// Improved conditions to detect multiline mapped document properly.
    /// </summary>
    [JavaDelete("XmlSerialiser not work in Java")]
    public class Test26636
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
            loadOptions.MswVersion = MsWordVersion.Word2019;
            Document document = TestUtil.Open(@"Model\Markup\Test26636.docx", loadOptions);
            foreach (CustomXmlPart cxp in document.CustomXmlParts)
            {
                string xmlns = cxp.Schemas.FirstOrDefault();
                string xmlString = string.Empty;
                if (string.IsNullOrEmpty(xmlns))
                    continue;
                if (xmlns.Contains("urn:test"))
                {
                    I componentDataPart = Serializer.Deserialize<I>(cxp.Data);
                    if (xmlns.Equals("urn:test1"))
                    {
                        componentDataPart.Content = File.ReadAllText(TestUtil.BuildTestFileName(@"Model\Markup\Test26636 component1.xml"), Encoding.UTF8);
                        xmlString = Serializer.Serialize<I>(componentDataPart, "urn:test1");
                    }
                    else
                    {
                        componentDataPart.Content = File.ReadAllText(TestUtil.BuildTestFileName(@"Model\Markup\Test26636 component2.xml"), Encoding.UTF8);
                        xmlString = Serializer.Serialize<I>(componentDataPart, "urn:test2");
                    }


                    cxp.Data = Encoding.UTF8.GetBytes(xmlString);
                }
            }

            document = TestUtil.SaveOpen(document, @"Model\Markup\Test26636.docx");

            foreach (StructuredDocumentTag sdt in document.Range.StructuredDocumentTags)
            {
                Assert.That(sdt.Level, Is.EqualTo(MarkupLevel.Block));
                Assert.That(sdt.FirstChild.NodeType, Is.EqualTo(NodeType.Paragraph));
                Assert.That(sdt.FirstChild.NextSibling.NodeType, Is.EqualTo(NodeType.Table));
            }
        }

        [Serializable]
        [XmlRoot("I")]
        public class I
        {
            [XmlAttribute]
            public string xmlns { get; set; }
            [XmlText]
            public string Content { get; set; }
        }

        public class Serializer
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

        public class NamespaceIgnorantXmlTextReader : XmlTextReader
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
    }
}
