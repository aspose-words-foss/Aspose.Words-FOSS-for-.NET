// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/10/2008 by Konstantin Sidorenko
// 2016/02/01 by Anatoliy Sidorenko

using System;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Xml
{
    [TestFixture]
    public class TestXmlTextReader
    {
        [Test]
        public void TestMoveToContent()
        {
            string[] inputs =
            {
                "<price>123.4</price>",
                "<!-- some test Comment --><?processing\n  instruction?><price>123.4</price>",
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><price>123.4</price>",
            };

            foreach (string inputStr in inputs)
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(inputStr));
                XmlTextReader reader = new XmlTextReader(stream);

                Assert.That(reader.MoveToContent(), Is.EqualTo(XmlNodeType.Element));
                Assert.That(reader.LocalName, Is.EqualTo("price"));
                Assert.That(reader.ReadString(), Is.EqualTo("123.4"));

                reader.Close();
            }
        }

        [Test]
        public void TestIsEmptyElement()
        {
            string input = "<empty/>";
            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));


            Assert.That(reader.Read(), Is.True);
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
            Assert.That(reader.LocalName, Is.EqualTo("empty"));

            //JAVA: IsEmptyElement property is not applicable for java's XMLStreareader since StAX parser
            //internally converts empty Elements to ordinary non-empty Elements.

            reader.Close();
        }

        [Test]
        public void TestIsEmptyElementException()
        {
            string input = "<empty/>";
            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));

            reader.Read();
            Assert.That(reader.IsEmptyElement, Is.True);
            reader.Close();
        }

        [Test]
        public void TestLocalName()
        {
            string input = "<book xmlns:bk='urn:samples'>\n" +
                           "  <title>Pride And Prejudice</title>\n" +
                           "  <bk:genre>novel</bk:genre>\n" +
                           "</book>\n";
            string output = "<book><title><bk:genre> The namespace URI is urn:samples";

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
            StringBuilder sb = new StringBuilder();
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    string prefix = reader.Prefix;
                    if (string.IsNullOrEmpty(prefix))
                        sb.Append("<").Append(reader.LocalName).Append(">");
                    else
                    {
                        sb.Append("<").Append(reader.Prefix).Append(":").Append(reader.LocalName).Append(">");
                        sb.Append(" The namespace URI is ").Append(reader.NamespaceURI);
                    }
                }
            }

            Assert.That(sb.ToString(), Is.EqualTo(output));
            reader.Close();
        }

        [Test]
        public void TestLocalNameAll()
        {
            string input = "<book xmlns:bk='urn:samples'>\n" +
                           "  <title>Pride And Prejudice</title>\n" +
                           "  <bk:genre>novel</bk:genre>\n" +
                           "</book>\n";

            //Note: MS hasn't END_DOCUMENT node type.
            string outputMS = "<Element:book><Whitespace:>" +
                              "<Element:title><Text:><EndElement:title><Whitespace:>" +
                              "<Element:genre><Text:><EndElement:genre><Whitespace:>" +
                              "<EndElement:book><Whitespace:>";

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
            StringBuilder sb = new StringBuilder();
            while (reader.Read())
                sb.Append("<").Append(reader.NodeType.ToString()).Append(":").
                        Append(reader.LocalName).Append(">");

#if JAVA
            string output = "<START_ELEMENT:book><SPACE:>" +
                            "<START_ELEMENT:title><CHARACTERS:><END_ELEMENT:title><SPACE:>" +
                            "<START_ELEMENT:genre><CHARACTERS:><END_ELEMENT:genre><SPACE:>" +
                            "<END_ELEMENT:book><END_DOCUMENT:>";

            Assert.That(output, Is.EqualTo(sb.ToString()));
#else
            Assert.That(outputMS, Is.EqualTo(sb.ToString()));
#endif
            reader.Close();
        }

        [Test]
        public void TestNameSpaceURIAndPrefix()
        {
            string input = "<book xmlns:bk='urn:samples'>\n" +
                           "  <title>Pride And Prejudice</title>\n" +
                           "  <bk:genre>novel</bk:genre>\n" +
                           "</book>\n";

            //Note: MS hasn't END_DOCUMENT node type.
            string outputMS = "<Element:book::><Whitespace:::>" +
                              "<Element:title::><Text:::><EndElement:title::><Whitespace:::>" +
                              "<Element:genre:bk:urn:samples><Text:::><EndElement:genre:bk:urn:samples><Whitespace:::>" +
                              "<EndElement:book::><Whitespace:::>";

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));

            StringBuilder sb = new StringBuilder();
            while (reader.Read())
                AppendNodeWithPrefixAndNamespace(sb, reader);

#if JAVA
            string output = "<START_ELEMENT:book::><SPACE:::>" +
                            "<START_ELEMENT:title::><CHARACTERS:::><END_ELEMENT:title::><SPACE:::>" +
                            "<START_ELEMENT:genre:bk:urn:samples><CHARACTERS:::><END_ELEMENT:genre:bk:urn:samples><SPACE:::>" +
                            "<END_ELEMENT:book::><END_DOCUMENT:::>";

            Assert.That(output, Is.EqualTo(sb.ToString()));
#else
            Assert.That(outputMS, Is.EqualTo(sb.ToString()));
#endif
            reader.Close();
        }

        [Test]
        public void TestValue()
        {
            string input = "<?xml version=\"1.0\"?>\n" +
                           "<!-- This is a sample XML document -->\n" +
                           "<!DOCTYPE Items [<!ENTITY number \"123\">]>\n" +
                           "<Items attr=\"value\">\n" +
                           "  <Item>Test with an entity: &number;</Item>\n" +
                           "  <Item>test with a child Element <more/> stuff</Item>\n" +
                           "  <Item>test with a CDATA section <![CDATA[<456>]]> def</Item>\n" +
                           "  <Item>Test with an char entity: &#65;</Item>\n" +
                           "  <!-- Fourteen chars in this Element.-->\n" +
                           "  <Item>1234567890ABCD</Item>\n" +
                           "</Items>\n";

            //Note: MS hasn't END_DOCUMENT node type.
            //Java skips XmlDeclaration and DOCTYPE Elements.
            string outputMS = "<XmlDeclaration:xml:version=\"1.0\"><Whitespace::\n>" +
                              "<Comment:: This is a sample XML document ><Whitespace::\n>" +
                              "<DocumentType:Items:<!ENTITY number \"123\">><Whitespace::\n>" +
                              "<Element:Items:><Whitespace::\n" +
                              "  ><Element:Item:><Text::Test with an entity: ><EntityReference:number:><EndElement:Item:><Whitespace::\n" +
                              "  ><Element:Item:><Text::test with a child Element ><Element:more:><Text:: stuff><EndElement:Item:><Whitespace::\n" +
                              "  ><Element:Item:><Text::test with a CDATA section ><CDATA::<456>><Text:: def><EndElement:Item:><Whitespace::\n" +
                              "  ><Element:Item:><Text::Test with an char entity: A><EndElement:Item:><Whitespace::\n" +
                              "  ><Comment:: Fourteen chars in this Element.><Whitespace::\n" +
                              "  ><Element:Item:><Text::1234567890ABCD><EndElement:Item:><Whitespace::\n" +
                              "><EndElement:Items:><Whitespace::\n>";

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
            StringBuilder sb = new StringBuilder();
            while (reader.Read())
                AppendNode(sb, reader);

#if JAVA
            string output = "<COMMENT:: This is a sample XML document ><DTD::<!ENTITY number \"123\">><START_ELEMENT:Items:><SPACE::\n" +
                              "  ><START_ELEMENT:Item:><CHARACTERS::Test with an entity: ><ENTITY_REFERENCE:number:123><END_ELEMENT:Item:><SPACE::\n" +
                              "  ><START_ELEMENT:Item:><CHARACTERS::test with a child Element ><START_ELEMENT:more:><END_ELEMENT:more:><CHARACTERS:: stuff><END_ELEMENT:Item:><SPACE::\n" +
                              "  ><START_ELEMENT:Item:><CHARACTERS::test with a CDATA section <456> def><END_ELEMENT:Item:><SPACE::\n" +
                              "  ><START_ELEMENT:Item:><CHARACTERS::Test with an char entity: A><END_ELEMENT:Item:><SPACE::\n" +
                              "  ><COMMENT:: Fourteen chars in this Element.><SPACE::\n" +
                              "  ><START_ELEMENT:Item:><CHARACTERS::1234567890ABCD><END_ELEMENT:Item:><SPACE::\n" +
                            "><END_ELEMENT:Items:><END_DOCUMENT::>";

            Assert.That(output, Is.EqualTo(sb.ToString()));
#else
            Assert.That(outputMS, Is.EqualTo(sb.ToString()));
#endif
            reader.Close();
        }


        [Test]
        public void TestNonLatinNamesAndValues()
        {
            string input = "<?xml version=\"1.0\"?>\n" +
                           "<!-- Это пример XML документа с русскими символами -->\n" +
                           "<Items атрибут=\"значение\">\n" +
                           "  <Элемент>просто текст</Элемент>\n" +
                           "</Items>\n";

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));

            reader.Read();
#if !JAVA
            reader.Read();//characters(whitespaces)
            reader.Read();
#endif
            Assert.That(XmlNodeType.Comment, Is.EqualTo(reader.NodeType));
            Assert.That(" Это пример XML документа с русскими символами ", Is.EqualTo(reader.Value));

#if !JAVA
            reader.Read();//characters(whitespaces)
#endif
            reader.Read();
            Assert.That(XmlNodeType.Element, Is.EqualTo(reader.NodeType));
            Assert.That("Items", Is.EqualTo(reader.LocalName));

            reader.MoveToFirstAttribute();
            Assert.That("атрибут", Is.EqualTo(reader.LocalName));
            Assert.That("значение", Is.EqualTo(reader.Value));

            reader.Read();//characters(whitespaces)
            reader.Read();
            Assert.That(XmlNodeType.Element, Is.EqualTo(reader.NodeType));
            Assert.That("Элемент", Is.EqualTo(reader.LocalName));

            reader.Read();
            Assert.That(XmlNodeType.Text, Is.EqualTo(reader.NodeType));
            Assert.That("просто текст", Is.EqualTo(reader.Value));

            reader.Read();//</Элемент>
            reader.Read();//characters(whitespaces)
            reader.Read();
            Assert.That(XmlNodeType.EndElement, Is.EqualTo(reader.NodeType));
            Assert.That("Items", Is.EqualTo(reader.LocalName));

            reader.Close();
        }

        [Test]
        public void TestXmlReaderMoveToNextAttribute()
        {
            string input = "<book genre='novel' ISBN='1-861003-78' pubdate='1987'>\n" +
                           "  <title>Pride And Prejudice</title>\n" +
                           "  <pice usd=\"1.23\" eur=\"1.0\" />\n" +
                           "</book>";
            string outputMS = "<Element:book:><Attribute:genre:novel><Attribute:ISBN:1-861003-78><Attribute:pubdate:1987><Whitespace::\n" +
                              "  ><Element:title:><Text::Pride And Prejudice><EndElement:title:><Whitespace::\n" +
                              "  ><Element:pice:><Attribute:usd:1.23><Attribute:eur:1.0><Whitespace::\n" +
                              "><EndElement:book:>";

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
            StringBuilder sb = new StringBuilder();

            while (reader.Read())
            {
                AppendNode(sb, reader);
                while (reader.MoveToNextAttribute())
                    AppendNode(sb, reader);
            }

#if JAVA
            string output = "<START_ELEMENT:book:><ATTRIBUTE:genre:novel><ATTRIBUTE:ISBN:1-861003-78><ATTRIBUTE:pubdate:1987><SPACE::\n" +
                            "  ><START_ELEMENT:title:><CHARACTERS::Pride And Prejudice><END_ELEMENT:title:><SPACE::\n" +
                            "  ><START_ELEMENT:pice:><ATTRIBUTE:usd:1.23><ATTRIBUTE:eur:1.0><END_ELEMENT:pice:><SPACE::\n" +
                            "><END_ELEMENT:book:><END_DOCUMENT::>";

            Assert.That(output, Is.EqualTo(sb.ToString()));
#else
            Assert.That(outputMS, Is.EqualTo(sb.ToString()));
#endif
            reader.Close();
        }

        [Test]
        public void TestXmlReaderNonLatinAttribute()
        {
            string input = "<book genre='роман' ISBN='1-861003-78' pubdate='1987'>\n" +
                           "  <title>Преступление и Наказание</title>\n" +
                           "  <pice руб=\"123\" eur=\"1.0\" />\n" +
                           "</book>";
            string outputMS = "<Element:book:><Attribute:genre:роман><Attribute:ISBN:1-861003-78><Attribute:pubdate:1987><Whitespace::\n" +
                              "  ><Element:title:><Text::Преступление и Наказание><EndElement:title:><Whitespace::\n" +
                              "  ><Element:pice:><Attribute:руб:123><Attribute:eur:1.0><Whitespace::\n" +
                              "><EndElement:book:>";

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
            StringBuilder sb = new StringBuilder();

            while (reader.Read())
            {
                AppendNode(sb, reader);
                while (reader.MoveToNextAttribute())
                    AppendNode(sb, reader);
            }

#if JAVA
            string output = "<START_ELEMENT:book:><ATTRIBUTE:genre:роман><ATTRIBUTE:ISBN:1-861003-78><ATTRIBUTE:pubdate:1987><SPACE::\n" +
                            "  ><START_ELEMENT:title:><CHARACTERS::Преступление и Наказание><END_ELEMENT:title:><SPACE::\n" +
                            "  ><START_ELEMENT:pice:><ATTRIBUTE:руб:123><ATTRIBUTE:eur:1.0><END_ELEMENT:pice:><SPACE::\n" +
                            "><END_ELEMENT:book:><END_DOCUMENT::>";

            Assert.That(output, Is.EqualTo(sb.ToString()));
#else
            Assert.That(outputMS, Is.EqualTo(sb.ToString()));
#endif
            reader.Close();
        }

        [Test]
        public void TestHasAttributes()
        {
            string input = "<book genre='novel' ISBN='1-861003-78' pubdate='1987'>\n" +
                           "  <title>Pride And Prejudice</title>\n" +
                           "  <pice usd=\"1.23\" eur=\"1.0\" />\n" +
                           "</book>";
            string outputMS =
                    "<Element:book:>True<Attribute:genre:novel>True<Attribute:ISBN:1-861003-78>True<Attribute:pubdate:1987>True<Whitespace::\n" +
                    "  >False<Element:title:>False<Text::Pride And Prejudice>False<EndElement:title:>False<Whitespace::\n" +
                    "  >False<Element:pice:>True<Attribute:usd:1.23>True<Attribute:eur:1.0>True<Whitespace::\n" +
                    ">False<EndElement:book:>False";

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
            StringBuilder sb = new StringBuilder();

            while (reader.Read())
            {
                AppendNode(sb, reader);
                sb.Append(reader.HasAttributes);
                while (reader.MoveToNextAttribute())
                {
                    AppendNode(sb, reader);
                    sb.Append(reader.HasAttributes);
                }
            }
#if JAVA
            string output =
                    "<START_ELEMENT:book:>true<ATTRIBUTE:genre:novel>true<ATTRIBUTE:ISBN:1-861003-78>true<ATTRIBUTE:pubdate:1987>true<SPACE::\n" +
                    "  >false<START_ELEMENT:title:>false<CHARACTERS::Pride And Prejudice>false<END_ELEMENT:title:>false<SPACE::\n" +
                    "  >false<START_ELEMENT:pice:>true<ATTRIBUTE:usd:1.23>true<ATTRIBUTE:eur:1.0>true<END_ELEMENT:pice:>false<SPACE::\n" +
                    ">false<END_ELEMENT:book:>false<END_DOCUMENT::>false";

            Assert.That(output, Is.EqualTo(sb.ToString()));
#else
            Assert.That(outputMS, Is.EqualTo(sb.ToString()));
#endif
            reader.Close();
        }

        [Test]
        public void TestCharacterEntitiesInText()
        {
            string input = "<start Comment=\"Just a plain string with entities:\">" +
                          //note: 0xC, 0x1, 0x7, etc. is illegal character entities in xml 1.0 (and in java) but legal in .Net.
                          //		              "&amp; &lt; &gt; \" ' \r &#xC; &#x1; &#x7; ." +
                          "&amp; &lt; &gt; \" ' \r ." +
                          "</start>";

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));

            reader.Read();
            Assert.That("start", Is.EqualTo(reader.LocalName));
            Assert.That(XmlNodeType.Element, Is.EqualTo(reader.NodeType));

            reader.Read();
            Assert.That(XmlNodeType.Text, Is.EqualTo(reader.NodeType));
            string Text = reader.ReadString();
            Assert.That("& < > \" ' \r .", Is.EqualTo(Text));

            reader.Close();
        }

        [Test]
        public void TestCharacterEntitiesInAttributes()
        {
            string input = "<start Comment=\"&amp; &lt; &gt; \r.\">" +
                          "</start>";

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));

            reader.Read();
            Assert.That("start", Is.EqualTo(reader.LocalName));
            Assert.That(XmlNodeType.Element, Is.EqualTo(reader.NodeType));

            reader.MoveToFirstAttribute();
            Assert.That("Comment", Is.EqualTo(reader.LocalName));
            string value = reader.Value;
            // \r in the attribute value is replaced by a space due the normalization.
            // Normalization is always true in java reader because of Xml standard.
            // WORDSJAVA-1069 Tab inside a list number in TOC are converted to space.
            // Now we disable normalization according to .NET behaviour.
            Assert.That("& < > \r.", Is.EqualTo(value));

            reader.Close();
        }

        [Test]
        public void TestReadOuterXmlCharacterEntitiesInText()
        {
            string input = "<start Comment=\"Just a plain string with entities:\">" +
                          //note: 0xC, 0x1, 0x7, etc. is illegal character entities in xml 1.0 (and in java) but legal in .Net.
                          //		              "&amp; &lt; &gt; \" ' \r &#xC; &#x1; &#x7; ." +
                          "&amp; &lt; &gt; \" ' \r ." +
                          "</start>";

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));

            reader.Read();
            Assert.That("start", Is.EqualTo(reader.LocalName));
            Assert.That(XmlNodeType.Element, Is.EqualTo(reader.NodeType));

            string outerXml = reader.ReadOuterXml();
            Assert.That(input, Is.EqualTo(outerXml));
            reader.Close();
        }

        [Test]
        public void TestWhiteSpaces()
        {
            string input = "<pict>\r\n" +
                    "\t<scriptText> \r\n" +
                    "\r\n" +
                    "cnnad_createAd...\r\n" +
                    "\r\n" +
                    "</scriptText>\r\n" +
                    "</pict>";
            string outputMS =
                    "<Element:pict:><Whitespace::\r\n" +
                            "\t><Element:scriptText:><Text:: \r\n" +
                            "\r\n" +
                            "cnnad_createAd...\r\n" +
                            "\r\n" +
                            "><EndElement:scriptText:><Whitespace::\r\n" +
                            "><EndElement:pict:>";

            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
            StringBuilder sb = new StringBuilder();

            while (reader.Read())
                AppendNode(sb, reader);

#if JAVA
            string output =
                    "<START_ELEMENT:pict:><SPACE::\r\n" +
                            "\t><START_ELEMENT:scriptText:><CHARACTERS:: \r\n" +
                            "\r\n" +
                            "cnnad_createAd...\r\n" +
                            "\r\n" +
                            "><END_ELEMENT:scriptText:><SPACE::\r\n" +
                            "><END_ELEMENT:pict:><END_DOCUMENT::>";

            Assert.That(output, Is.EqualTo(sb.ToString()));
#else
            Assert.That(outputMS, Is.EqualTo(sb.ToString()));
#endif
            reader.Close();
        }

        [Test]
        public void TestReadInnerXml()
        {
            string firstBookTitleInnerText = "The Handmaid's Tale";
            string firstBookInnerText = "\n" +
                                        "    <title>" + firstBookTitleInnerText + "</title>\n" +
                                        "    <price>19.95</price>\n" +
                                        "  ";
            string bookstoreInnerText = "\n" +
                                        "  <book genre=\"novel\" ISBN=\"10-861003-324\">" +
                                        firstBookInnerText +
                                        "</book>\n" +
                                        "  <book genre=\"novel\" ISBN=\"1-861001-57-5\">\n" +
                                        "    <title>Pride And Prejudice</title>\n" +
                                        "    <price>24.95</price>\n" +
                                        "  </book>\n";

            string input = "<!--sample XML fragment-->\n" +
                           "<bookstore>" +
                           bookstoreInnerText +
                           "</bookstore>\n" +
                           "\n";

            //position on Comments right at the begin: inner xml should be empty string.
            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
            reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
            reader.Namespaces = false;
#if NET40
            reader.DtdProcessing = DtdProcessing.Prohibit;
#endif

            reader.Read();
            Assert.That("", Is.EqualTo(reader.LocalName));
            Assert.That(XmlNodeType.Comment, Is.EqualTo(reader.NodeType));
            Assert.That("", Is.EqualTo(reader.ReadInnerXml()));
            reader.Close();

            //position on <bookstore> Element start.
            reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
            while (!"bookstore".Equals(reader.LocalName))
                reader.Read();
            Assert.That("bookstore", Is.EqualTo(reader.LocalName));
            Assert.That(XmlNodeType.Element, Is.EqualTo(reader.NodeType));
            Assert.That(bookstoreInnerText, Is.EqualTo(reader.ReadInnerXml()));
            reader.Close();

            //position on the first <book> Element start.
            reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
            while (!"book".Equals(reader.LocalName))
                reader.Read();
            Assert.That("book", Is.EqualTo(reader.LocalName));
            Assert.That(XmlNodeType.Element, Is.EqualTo(reader.NodeType));
            Assert.That(firstBookInnerText, Is.EqualTo(reader.ReadInnerXml()));
            reader.Close();

            //position on the first <book>'s <title> Element start.
            reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
            while (!"title".Equals(reader.LocalName))
                reader.Read();
            Assert.That("title", Is.EqualTo(reader.LocalName));
            Assert.That(XmlNodeType.Element, Is.EqualTo(reader.NodeType));
            Assert.That(firstBookTitleInnerText, Is.EqualTo(reader.ReadInnerXml()));
            reader.Close();
        }


        private void AppendNode(StringBuilder sb, XmlTextReader reader)
        {
            sb.Append("<")
                    .Append(reader.NodeType.ToString())
                    .Append(":").Append(reader.LocalName)
                    .Append(":").Append(reader.Value)
                    .Append(">");
        }

        private static void AppendNodeWithPrefixAndNamespace(StringBuilder sb, XmlTextReader reader)
        {
            sb.Append("<").Append(reader.NodeType.ToString())
                    .Append(":").Append(reader.LocalName)
                    .Append(":").Append(reader.Prefix)
                    .Append(":").Append(reader.NamespaceURI)
                    .Append(">");
        }

        [Test]
        public void TestTabs()
        {
            string xml = "<node attr=\"\t0\" />";
            //xml = xml.replace("\t", "&#x9;");
            XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(xml)));
            reader.Namespaces = false;
#if NET40
            reader.DtdProcessing = DtdProcessing.Prohibit;
#endif
            Assert.That(reader.Read(), Is.True);
            Assert.That(reader.MoveToFirstAttribute(), Is.True);
            Assert.That("\t0", Is.EqualTo(reader.Value));
        }

        [Test]
        public void TestJiraJ1719()
        {
            String xml =
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                            "<w:hdr xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\" " +
                            "xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">" +
                            "<w:p>" +
                            "<w:r>" +
                            "<w:drawing>" +
                            "<wp:inline distT=\"0\" distR=\"114300\" distL=\"114300\" distB=\"0\">" +
                            "<a:graphic>" +
                            "<a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/picture\">" +
                            "</a:graphicData>" +
                            "</a:graphic>" +
                            "</wp:inline>" +
                            "</w:drawing>" +
                            "</w:r>" +
                            "</w:hdr>";

            String message = null;

            try
            {
                XmlTextReader reader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(xml)));
                while (reader.Read())
                {
                }
            }
            catch (Exception e)
            {
                // .Net: XmlException, Java: XMLStreamException
                message = e.Message;
            }

            // Neutral message is "Undeclared namespace prefix \"a\"" - but the message can be localized when system locale is non-english.
            // It is also can be single quotes '' instead of double quotes "" on different system locales.
            Assert.That(message.Contains("\"a\"") || message.Contains("'a'"), Is.True, message);
        }
    }
}
