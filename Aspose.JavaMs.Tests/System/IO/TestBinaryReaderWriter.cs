// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/07/2005 by Roman Korchagin
// 2016/06/20 by Anatoliy Sidorenko

using System.IO;
using System.Text;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.IO
{
    [TestFixture]
    public class TestBinaryReaderWriter
    {
        [Test]
        public void TestReadChars()
        {
            byte[] unicodeBytes = { 84, 0, 101, 0, 115, 0, 116, 0, 32, 0, 83, 0, 116, 0, 114, 0, 105, 0, 110, 0, 103, 0, 32, 0, 34, 4, 53, 4, 65, 4, 66, 4, 62, 4, 50, 4, 48, 4, 79, 4, 32, 0, 65, 4, 66, 4, 64, 4, 62, 4, 71, 4, 58, 4, 48, 4, 32, 0, 49, 0, 50, 0, 51, 0, 32, 0, 47, 0, 110, 0, 46, 0 };
            string testStrUnicode = "Test String Тестовая строчка 123 /n.";
            string testStrAscii = "T\u0000e\u0000s\u0000t\u0000 \u0000";

            MemoryStream stream = new MemoryStream(unicodeBytes);
            BinaryReader reader = new BinaryReader(stream, Encoding.Unicode);

            //reading in "main" (setted by ctor) encoding
            char[] chars = reader.ReadChars(11);
            char[] testChars = testStrUnicode.Substring(0, 11).ToCharArray();
            Assert.That(ArrayUtil.IsArrayEqual(chars, testChars), Is.True);
            Assert.That(11 * 2, Is.EqualTo(reader.BaseStream.Position));

            //one-short reading in another encoding
            reader = new BinaryReader(stream, Encoding.ASCII);
            reader.BaseStream.Position = 0;
            chars = reader.ReadChars(10);
            testChars = testStrAscii.ToCharArray();
            Assert.That(ArrayUtil.IsArrayEqual(chars, testChars), Is.True);
            Assert.That(10, Is.EqualTo(reader.BaseStream.Position));

            //again main encoding
            reader = new BinaryReader(stream, Encoding.Unicode);
            reader.BaseStream.Position = 10;
            chars = reader.ReadChars(15);
            testChars = testStrUnicode.Substring(5, 15).ToCharArray();
            Assert.That(ArrayUtil.IsArrayEqual(chars, testChars), Is.True);
        }

        [Test]
        public void TestBinaryWriter()
        {
            Stream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write((byte)1);
            writer.Write((byte)0);

            writer.Write((short)1234);
            writer.Write((short)-1234);

            writer.Write(123456);
            writer.Write(-123456);

            writer.Write((ushort)0xF123);
            writer.Write((uint)0xF1234567);

            //Check length ang go back to read the stuff
            Assert.That(stream.Length, Is.EqualTo(20));
            stream.Position = 0;

            BinaryReader reader = new BinaryReader(stream);

            Assert.That(reader.ReadByte(), Is.EqualTo(1));
            Assert.That((byte)reader.ReadByte(), Is.EqualTo(0));

            Assert.That(reader.ReadInt16(), Is.EqualTo(1234));
            Assert.That(reader.ReadInt16(), Is.EqualTo(-1234));

            Assert.That(reader.ReadInt32(), Is.EqualTo(123456));
            Assert.That(reader.ReadInt32(), Is.EqualTo(-123456));

            Assert.That(reader.ReadUInt16(), Is.EqualTo((ushort)0xF123));
            Assert.That(reader.ReadUInt32(), Is.EqualTo((uint)0xF1234567));
        }
    }
}
