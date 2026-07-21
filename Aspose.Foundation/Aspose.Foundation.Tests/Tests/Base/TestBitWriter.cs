// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/05/2011 by Alexey Titov

using Aspose.IO;
using NUnit.Framework; 

namespace Aspose.Tests.Base
{
    [TestFixture]
    public class TestBitWriter
    {
        [Test]
        public void WriteOneInCurrentBit_1stAndLastBitInByte()
        {
            // Arrange
            byte[] bytes = new byte[1];
            BitWriter writer = new BitWriter(bytes);
            // Act
            writer.WriteOneInCurrentBit();
            for (int i = 0; i < 7; i++)
            {
                writer.MoveToNextBit();
            }
            writer.WriteOneInCurrentBit();
            writer.Flush();
            // Assert
            Assert.That(bytes[0], Is.EqualTo(129));
        }

        [Test]
        public void MoveToNextBit_OverByteBoundary()
        {
            // Arrange
            byte[] bytes = new byte[2];
            BitWriter writer = new BitWriter(bytes);
            // Act
            writer.WriteOneInCurrentBit();
            for (int i = 0; i < 9; i++)
            {
                writer.MoveToNextBit();
            }
            writer.WriteOneInCurrentBit();
            writer.Flush();
            // Assert
            Assert.That(bytes[0], Is.EqualTo(128));
            Assert.That(bytes[1], Is.EqualTo(64));
        }

        [Test]
        public void MoveToNextByte()
        {
            // Arrange
            byte[] bytes = new byte[2];
            BitWriter writer = new BitWriter(bytes);
            // Act
            // Write 1st byte
            writer.WriteOneInCurrentBit();
            for (int i = 0; i < 3; i++)
            {
                writer.MoveToNextBit();
            }
            // Write 2nd byte
            writer.ByteIndex = 1;
            writer.MoveToNextBit();
            writer.WriteOneInCurrentBit();
            writer.Flush();
            // Assert
            Assert.That(bytes[0], Is.EqualTo(128));
            Assert.That(bytes[1], Is.EqualTo(64));
        }
    }
}