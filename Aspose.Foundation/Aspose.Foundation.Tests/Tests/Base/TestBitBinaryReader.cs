// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2011 by Konstantin Kornilov
using System;
using System.IO;
using Aspose.IO;
using NUnit.Framework;

namespace Aspose.Tests.Base
{
    [TestFixture]
    public class TestBitBinaryReader
    {
        [Test]
        public void TestReadBit()
        {
            VerifyResultBitArray(
                new byte[] {0x35},
                new bool[] {false, false, true, true, false, true, false, true});

            VerifyResultBitArray(
                new byte[] {0xF2, 0x01},
                new bool[]
                    {
                        true, true, true, true, false, false, true, false,
                        false, false, false, false, false, false, false, true
                    });
        }

        private void VerifyResultBitArray(byte[] data, bool[] expectedResultWithMsbOrdering)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                // Verify MSB ordering.
                BitBinaryReader msbReader = new BitBinaryReader(stream, true);
                for (int i = 0; i < expectedResultWithMsbOrdering.Length; i++)
                {
                    Assert.That(msbReader.ReadBit(), Is.EqualTo(expectedResultWithMsbOrdering[i]));
                }

                // Verify LSB ordering.
                stream.Position = 0;
                BitBinaryReader lsbReader = new BitBinaryReader(stream, false);
                Assert.That(expectedResultWithMsbOrdering.Length%8, Is.EqualTo(0));
                for (int i = 0; i < expectedResultWithMsbOrdering.Length/8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        Assert.That(lsbReader.ReadBit(), Is.EqualTo(expectedResultWithMsbOrdering[8*i + 8 - j - 1]));
                    }
            }
        }

        [Test]
        public void TestReadValueMsb()
        {
            byte[] data = new byte[] {0x04, 0x03, 0x02, 0x01};
            using (MemoryStream stream = new MemoryStream(data))
            {
                BitBinaryReader msbReader = new BitBinaryReader(stream, true);
                Assert.That(msbReader.ReadUnsignedValue(4), Is.EqualTo(0x0));
                Assert.That(msbReader.ReadUnsignedValue(8), Is.EqualTo(0x40));
                Assert.That(msbReader.ReadUnsignedValue(20), Is.EqualTo(0x30201));
            }
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestReadValueLsb()
        {
            byte[] data = new byte[] {0x04, 0x03, 0x02, 0x01};
            using (MemoryStream stream = new MemoryStream(data))
            {
                BitBinaryReader lsbReader = new BitBinaryReader(stream, false);
                lsbReader.ReadUnsignedValue(4);
                lsbReader.ReadUnsignedValue(8);
                lsbReader.ReadUnsignedValue(20);
            }
        }
    }
}