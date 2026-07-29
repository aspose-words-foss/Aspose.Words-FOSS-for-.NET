// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/12/2006 by Konstantin Sidorenko
// 2016/02/02 by Anatoliy Sidorenko

using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Xml
{
    /// <summary>
    /// Note UTF8 BOM (or, rather, utf8 Bom bytes decoded by utf8 encoding:) on the start of the _each first_ test string.
    ///
    /// Quotation from XmlTextWriter ctor:
    /// .Net writes the BOM only if sees the Stream begin, but we can't check
    /// whether it is begin or not for java's OutputStream - so we always write the BOM
    /// during XmlTextWriter initialization.
    /// </summary>
    [TestFixture]
    public class TestXmlTextWriter
    {
        /// <summary>
        ///  Checks start document declaration options, xml version and encoding.
        /// </summary>
        [Test]
        public void TestStartDocument()
        {
            MemoryStream streams = new MemoryStream(128);
            Encoding encoding = Encoding.UTF8;
            XmlTextWriter writer = new XmlTextWriter(streams, encoding);

            writer.WriteStartDocument();
            writer.Flush();
            string declaration = encoding.GetString(streams.ToArray());
            Assert.That("\uFEFF<?xml version=\"1.0\" encoding=\"utf-8\"?>", Is.EqualTo(declaration));

            streams.Close();
            streams = new MemoryStream(128);
            encoding = Encoding.UTF8;
            writer = new XmlTextWriter(streams, encoding);

            writer.WriteStartDocument(false);
            writer.Flush();
            declaration = encoding.GetString(streams.ToArray());
            Assert.That("\uFEFF<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?>", Is.EqualTo(declaration));

            streams.Close();
            streams = new MemoryStream(128);
            encoding = Encoding.UTF8;
            writer = new XmlTextWriter(streams, encoding);

            writer.WriteStartDocument(true);
            writer.Flush();
            declaration = encoding.GetString(streams.ToArray());
            Assert.That("\uFEFF<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>", Is.EqualTo(declaration));

            //Woodstox throws during closing if no tag started (when auto elements closing enabled) - let's add something.
            writer.WriteStartElement("html");

            writer.Close();
            streams.Close();
        }

        [Test]
        public void TestWriteDocType()
        {
            MemoryStream streams = new MemoryStream(128);
            Encoding encoding = Encoding.UTF8;
            XmlTextWriter writer = new XmlTextWriter(streams, encoding);

            writer.WriteDocType(
                "html",
                "-//W3C//DTD XHTML 1.0 Transitional//EN",
                "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd",
                null);

            writer.Flush();
            string dtd = encoding.GetString(streams.ToArray());

            Assert.That("\uFEFF<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">", Is.EqualTo(dtd));

            //Woodstox throws during closing if no tag started (when auto elements closing enabled) - let's add something.
            writer.WriteStartElement("html");

            writer.Close();
            streams.Close();
        }

        [Test]
        public void TestWriteStartElement()
        {
            MemoryStream streams = new MemoryStream(128);
            Encoding encoding = Encoding.UTF8;
            XmlTextWriter writer = new XmlTextWriter(streams, encoding);

            writer.WriteStartElement("html");
            writer.WriteAttributeString("xmlns", "http://www.w3.org/1999/xhtml");
            writer.WriteStartElement("head");

            writer.Flush();
            string start = encoding.GetString(streams.ToArray());
            Assert.That("\uFEFF<html xmlns=\"http://www.w3.org/1999/xhtml\"><head", Is.EqualTo(start));

            writer.Close();
            streams.Close();
        }

        [Test]
        public void TestWriteEndElements()
        {
            MemoryStream streams = new MemoryStream(128);
            Encoding encoding = Encoding.UTF8;
            XmlTextWriter writer = new XmlTextWriter(streams, encoding);

            writer.WriteStartElement("html");
            writer.WriteStartElement("head");
            writer.WriteStartElement("meta");
            writer.WriteEndElement(); // meta
            writer.WriteStartElement("body");

            writer.WriteEndElement(); // body
            writer.WriteEndElement(); // head
            writer.WriteEndElement(); // html

            writer.Flush();
            string start = encoding.GetString(streams.ToArray());
            //JAVA: Woodstox parser can write automatic empty elements
            Assert.That("\uFEFF<html><head><meta /><body /></head></html>", Is.EqualTo(start));

            writer.Close();
            streams.Close();
        }

        [Test]
        public void TestWriteFullEndElements()
        {
            MemoryStream streams = new MemoryStream(128);
            Encoding encoding = Encoding.UTF8;
            XmlTextWriter writer = new XmlTextWriter(streams, encoding);

            writer.WriteStartElement("html");
            writer.WriteStartElement("head");
            writer.WriteStartElement("meta");
            writer.WriteFullEndElement(); // meta
            writer.WriteStartElement("body");

            writer.WriteFullEndElement(); // body
            writer.WriteFullEndElement(); // head
            writer.WriteFullEndElement(); // html

            writer.Flush();
            string start = encoding.GetString(streams.ToArray());
            Assert.That("\uFEFF<html><head><meta></meta><body></body></head></html>", Is.EqualTo(start));

            writer.Close();
            streams.Close();
        }

        [Test]
        public void TestWriteEndDocument()
        {
            MemoryStream streams = new MemoryStream(128);
            Encoding encoding = Encoding.UTF8;
            XmlTextWriter writer = new XmlTextWriter(streams, encoding);

            writer.WriteStartDocument();
            writer.WriteStartElement("html");
            writer.WriteStartElement("head");
            writer.WriteStartElement("meta");
            writer.WriteEndElement(); // meta
            writer.WriteStartElement("body");

            writer.WriteEndDocument(); // closes all open tags

            writer.Flush();
            string start = encoding.GetString(streams.ToArray());
            //note: Woodstox closes "body" with full end tag.
#if JAVA
            Assert.That("\uFEFF<?xml version=\"1.0\" encoding=\"utf-8\"?><html><head><meta /><body></body></head></html>", Is.EqualTo(start));
#else
            Assert.That("\uFEFF<?xml version=\"1.0\" encoding=\"utf-8\"?><html><head><meta /><body /></head></html>", Is.EqualTo(start));
#endif

            writer.Close();
            streams.Close();
        }

        [Test]
        public void TestWriteString()
        {
            MemoryStream streams = new MemoryStream(128);
            Encoding encoding = Encoding.UTF8;
            XmlTextWriter writer = new XmlTextWriter(streams, encoding);

            writer.WriteStartElement("start");
            writer.WriteString("Just a plain string.");
            writer.WriteString(" A string with entities: ");
            writer.WriteString("& < > ");
            writer.WriteString("\" ' \r ");
            writer.WriteString("\f \u0001 \u0007 .");
            writer.WriteEndElement();

            writer.Flush();
            string output = encoding.GetString(streams.ToArray());
            Assert.That("\uFEFF<start>Just a plain string. A string with entities: &amp; &lt; &gt; \" ' \r &#xC; &#x1; &#x7; .</start>", Is.EqualTo(output));

            writer.Close();
            streams.Close();
        }

        [Test]
        public void TestWriteRussianString()
        {
            MemoryStream streams = new MemoryStream(128);
            Encoding encoding = Encoding.UTF8;
            XmlTextWriter writer = new XmlTextWriter(streams, encoding);

            writer.WriteStartElement("start");
            writer.WriteString("Русский текст.");
            writer.WriteEndElement();

            writer.Flush();
            string output = encoding.GetString(streams.ToArray());
            Assert.That("\uFEFF<start>Русский текст.</start>", Is.EqualTo(output));

            writer.Close();
            streams.Close();
        }

        [Test]
        public void TestWriteAttributeString()
        {
            MemoryStream streams = new MemoryStream(128);
            Encoding encoding = Encoding.UTF8;
            XmlTextWriter writer = new XmlTextWriter(streams, encoding);

            writer.WriteStartElement("html");
            writer.WriteAttributeString("xmlns", "http://www.w3.org/1999/xhtml");
            writer.WriteStartElement("head");
            writer.WriteStartElement("meta");
            writer.WriteAttributeString("http-equiv", "Content-Type");
            writer.WriteAttributeString("content", "text/html; charset=utf-8");
            writer.WriteEndElement(); // meta
            writer.WriteStartElement("body");
            writer
                .WriteAttributeString("attributeWithEntities", "<attribute>with\"entities\"and\'a'postrofes\'");
            writer.WriteAttributeString("att", "<>");

            writer.Flush();
            string start = encoding.GetString(streams.ToArray());

            Assert.That("\uFEFF<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
                                       "<head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />" +
                                       "<body attributeWithEntities=\"&lt;attribute&gt;with&quot;entities&quot;and'a'postrofes'\" att=\"&lt;&gt;\"", Is.EqualTo(start));

            writer.Close();
            streams.Close();
        }

        [Test]
        public void TestWriteRussianAttributeString()
        {
            MemoryStream streams = new MemoryStream(128);
            Encoding encoding = Encoding.UTF8;
            XmlTextWriter writer = new XmlTextWriter(streams, encoding);

            writer.WriteStartElement("html");
            writer.WriteAttributeString("xmlns", "http://www.w3.org/1999/xhtml");
            writer.WriteStartElement("head");
            writer.WriteStartElement("meta");
            writer.WriteAttributeString("http-equiv", "Content-Type");
            writer.WriteAttributeString("content", "text/html; charset=utf-8");
            writer.WriteEndElement(); // meta
            writer.WriteStartElement("body");
            writer.WriteAttributeString("attributeWithRussian", "Русский текст.");

            writer.Flush();
            string start = encoding.GetString(streams.ToArray());

            Assert.That("\uFEFF<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
                                       "<head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />" +
                                       "<body attributeWithRussian=\"Русский текст.\"", Is.EqualTo(start));

            writer.Close();
            streams.Close();
        }

        [Test]
        public void TestWriteAttributeWithCharacterEntities()
        {
            MemoryStream streams = new MemoryStream(128);
            Encoding encoding = Encoding.UTF8;
            XmlTextWriter writer = new XmlTextWriter(streams, encoding);

            writer.WriteStartElement("html");
            string entitiesStr = "& < > \" ' \r \n \t \f \u0001 \u0007 .";
            writer.WriteAttributeString("entities", entitiesStr);
            writer.WriteEndElement();

            writer.Flush();
            string output = encoding.GetString(streams.ToArray());
#if JAVA
            string expected =
                "\uFEFF<html entities=\"&amp; &lt; &gt; &quot; ' &#xD; &#xA; &#x9; &#xC; &#x1; &#x7; .\" />";
#else
            string expected =
                "\uFEFF<html entities=\"&amp; &lt; &gt; &quot; ' &#xD; &#xA; \t &#xC; &#x1; &#x7; .\" />";
#endif

            writer.Close();
            streams.Close();
            Assert.That(expected, Is.EqualTo(output));
        }

        [Test]
        public void TestWriteRaw()
        {
            MemoryStream streams = new MemoryStream(128);
            Encoding encoding = Encoding.UTF8;
            XmlTextWriter writer = new XmlTextWriter(streams, encoding);

            writer.WriteStartElement("html");
            string entitiesStr = "A string with entities: & < > \" ' \r \f \u0001 \u0007 .";
            writer.WriteRaw(entitiesStr);

            writer.Flush();
            string output = encoding.GetString(streams.ToArray());
            Assert.That("\uFEFF<html>" + entitiesStr, Is.EqualTo(output));

            writer.Close();
            streams.Close();
        }
    }
}
