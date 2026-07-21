// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/09/2008 by Konstantin Sidorenko
// 04/02/2016 by Anatoliy Sidorenko

using System;
using System.IO;
using System.Text;
using System.Xml;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Xml
{
    [TestFixture]
    public class TestXmlTextWriterEncoding
    {
        [Test]
        public void TestUtf8()
        {
            Encoding encoding = Encoding.UTF8;
            MemoryStream stream = new MemoryStream(128);
            XmlTextWriter writer = new XmlTextWriter(stream, encoding);

            writer.WriteStartDocument();
            writer.WriteStartElement("start"); //or else stax throws when trying to write end element on writer.Close()
            writer.WriteRaw("123fFЯ");
            writer.Flush();

            //check the bom.
            byte[] bytes = stream.ToArray();
            byte[] preamble = encoding.GetPreamble();
            byte[] bom = new byte[preamble.Length];
            Array.Copy(bytes, 0, bom, 0, bom.Length);
            Assert.That(bom, Is.EqualTo(preamble));

            StreamReader reader = new StreamReader(new MemoryStream(bytes), encoding);
            string declaration = reader.ReadToEnd();
            Assert.That("<?xml version=\"1.0\" encoding=\"utf-8\"?><start>123fFЯ", Is.EqualTo(declaration));

            writer.Close();
            reader.Close();
        }

        [Test]
        [AndroidDelete("Performance issues")]
        public void TestEncodingUtf7()
        {
            Encoding encoding = Encoding.UTF7;
            MemoryStream stream = new MemoryStream(128);
            XmlTextWriter writer = new XmlTextWriter(stream, encoding);

            writer.WriteStartDocument();
            writer.WriteStartElement("start"); //or else stax throws when trying to write end element on writer.Close()
            writer.WriteRaw("123fFЯ");
            writer.Flush();

            //check the bom - XmlTextWriter (or rather backing StreamWriter) doesn't write it for UTF7.
            byte[] bytes = stream.ToArray();
            byte[] preamble = encoding.GetPreamble();
            byte[] bom = new byte[preamble.Length];
            Array.Copy(bytes, 0, bom, 0, bom.Length);
            Assert.That(bom, Is.EqualTo(preamble));

            StreamReader reader = new StreamReader(new MemoryStream(bytes), encoding);
            string declaration = reader.ReadToEnd();
            //JAVA: HACK for Sun jdk1.4, jdk1.5 Charset.encode() Flushing bug:
            //for utf-7 encoding ours XmlTextWriter.Flush() writes additional "\r\n" to the end
            //so the encoder can encode last bytes.
#if JAVA
            Assert.That("<?xml version=\"1.0\" encoding=\"utf-7\"?><start>123fFЯ\r\n", Is.EqualTo(declaration));
#else
            Assert.That("<?xml version=\"1.0\" encoding=\"utf-7\"?><start>123fFЯ", Is.EqualTo(declaration));
#endif

            writer.Close();
            reader.Close();
        }
    }
}
