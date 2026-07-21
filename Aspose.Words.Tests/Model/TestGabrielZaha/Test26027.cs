// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/10/2023 by Alexey Morozov

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
    /// WORDSNET-26027 (fixed) - Paragraph formatting changed after updating CXP
    /// XML Mapping logic has been changed.
    /// Tested on Microsoft® Word 2019 MSO (Version 2309 Build 16.0.16827.20166) 32-bit.
    /// </summary>
    [JavaDelete]
    public class Test26027
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
            loadOptions.MswVersion = Aspose.Words.Settings.MsWordVersion.Word2019;
            Document document = TestUtil.Open(@"Model\Markup\Test26027.docx", loadOptions);

            foreach (CustomXmlPart cxp in document.CustomXmlParts)
            {
                if (cxp.Schemas.Count() > 0 && cxp.Schemas.FirstOrDefault().Equals("urn:test1", StringComparison.OrdinalIgnoreCase))
                {
                    I xmlPart = Deserialize<I>(cxp.Data);
                    string content = File.ReadAllText(TestUtil.BuildTestFileName(@"Model\Markup\Test26027 component.xml"));
                    xmlPart.C = content;
                    string customXmlPart = Serialize<I>(xmlPart, cxp.Schemas.First());
                    cxp.Data = Encoding.UTF8.GetBytes(customXmlPart);
                }
            }
            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.Docx);
            saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;

            document = TestUtil.SaveOpen(document, @"Model\Markup\Test26027.docx", saveOptions, false);

            Paragraph para = document.FirstSection.Body.Tables[0].Rows[1].FirstCell.LastParagraph;

            Assert.That(para.GetText(), Is.EqualTo("|Test format and Size libero, sit amet commodo magna eros quis urna|.\u0007"));

            Assert.That(para.ParaPr[ParaAttr.Istd], Is.Null);
            Assert.That(para.ParaPr[ParaAttr.SpaceBefore], Is.Null);
            Assert.That(para.ParaPr[ParaAttr.SpaceAfter], Is.EqualTo(250));
            Assert.That(para.ParaPr[ParaAttr.LineSpacing], Is.EqualTo(new LineSpacing(300, LineSpacingRule.AtLeast)));

            Assert.That(para.ParagraphBreakRunPr[FontAttr.Istd], Is.EqualTo(0x29));
            Assert.That(para.ParagraphBreakRunPr[FontAttr.SizeBi], Is.EqualTo(22));
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

    [XmlRoot]
    public class I
    {
        [XmlText]
        public string C { get; set; }
        [XmlAttribute]
        public string xmlns { get; set; }
    }
}
