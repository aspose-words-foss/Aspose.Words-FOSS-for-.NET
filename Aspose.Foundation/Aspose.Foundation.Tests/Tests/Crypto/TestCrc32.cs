// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/05/2024 by Denis Panov

using System.Text;
using Aspose.Crypto;
using NUnit.Framework;

namespace Aspose.Tests.Crypto
{
    public class TestCrc32
    {
        [TestCase(3, 11, 0xA0D118A0)]
        [TestCase(9, 5, 0xDEF4CFE9)]
        [TestCase(70, 6, 0xF373B43B)]
        public void TestCrcForPart(int index, int count, uint expected)
        {
            uint result = Crc32.ComputeCRC(gBuffer, index, count);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestCrcByteArray()
        {
            uint result = Crc32.ComputeCRC(gBuffer);
            Assert.That(result, Is.EqualTo(0xCBE25E99));
        }

        [Test]
        public void TestCrcEmptyArray()
        {
            uint result = Crc32.ComputeCRC(new byte[] { });
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void TestCrcString()
        {
            byte[] bytesOfString = Encoding.ASCII.GetBytes("Test string");
            uint result = Crc32.ComputeCRC(bytesOfString);
            Assert.That(result, Is.EqualTo(0x95DB9A92));
        }

        static byte[] gBuffer = new byte[] {
            0x49, 0x74, 0x20, 0x69, 0x73, 0x20, 0x61, 0x20, 0x6D, 0x69, 0x73, 0x74, 0x61, 0x6B, 0x65, 0x20, 0x74, 0x6F, 0x20,
            0x74, 0x68, 0x69, 0x6E, 0x6B, 0x20, 0x79, 0x6F, 0x75, 0x20, 0x63, 0x61, 0x6E, 0x20, 0x73, 0x6F, 0x6C, 0x76, 0x65,
            0x20, 0x61, 0x6E, 0x79, 0x20, 0x6D, 0x61, 0x6A, 0x6F, 0x72, 0x20, 0x70, 0x72, 0x6F, 0x62, 0x6C, 0x65, 0x6D, 0x73,
            0x20, 0x6A, 0x75, 0x73, 0x74, 0x20, 0x77, 0x69, 0x74, 0x68, 0x20, 0x70, 0x6F, 0x74, 0x61, 0x74, 0x6F, 0x65, 0x73
        };
    }
}
