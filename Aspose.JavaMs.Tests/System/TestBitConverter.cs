// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2017 by Konstantin Sidorenko

using System;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestBitConverter
    {
        [Test]
        public void TestGetBytesFromInt()
        {
            Assert.That(BitConverter.GetBytes(0), Is.EqualTo(new byte[] {0, 0, 0, 0}));
            Assert.That(BitConverter.GetBytes(1), Is.EqualTo(new byte[] {0x01, 0x00, 0x00, 0x00}));
            Assert.That(BitConverter.GetBytes(100), Is.EqualTo(new byte[] {0x64, 0x00, 0x00, 0x00}));
            Assert.That(BitConverter.GetBytes(1000), Is.EqualTo(new byte[] {0xE8, 0x03, 0x00, 0x00}));
            Assert.That(BitConverter.GetBytes(1000000), Is.EqualTo(new byte[] {0x40, 0x42, 0x0F, 0x00}));
            Assert.That(BitConverter.GetBytes(1000000000), Is.EqualTo(new byte[] {0x00, 0xCA, 0x9A, 0x3B}));
            Assert.That(BitConverter.GetBytes(int.MaxValue - 1), Is.EqualTo(new byte[] {0xFE, 0xFF, 0xFF, 0x7F}));
            Assert.That(BitConverter.GetBytes(int.MaxValue), Is.EqualTo(new byte[] {0xFF, 0xFF, 0xFF, 0x7F}));
            Assert.That(BitConverter.GetBytes(int.MinValue), Is.EqualTo(new byte[] {0x00, 0x00, 0x00, 0x80}));
            Assert.That(BitConverter.GetBytes(int.MinValue + 1), Is.EqualTo(new byte[] {0x01, 0x00, 0x00, 0x80}));
            Assert.That(BitConverter.GetBytes(-1000000000), Is.EqualTo(new byte[] {0x00, 0x36, 0x65, 0xC4}));
            Assert.That(BitConverter.GetBytes(-1000000), Is.EqualTo(new byte[] {0xC0, 0xBD, 0xF0, 0xFF}));
            Assert.That(BitConverter.GetBytes(-1000), Is.EqualTo(new byte[] {0x18, 0xFC, 0xFF, 0xFF}));
            Assert.That(BitConverter.GetBytes(-100), Is.EqualTo(new byte[] {0x9C, 0xFF, 0xFF, 0xFF}));
            Assert.That(BitConverter.GetBytes(-1), Is.EqualTo(new byte[] {0xFF, 0xFF, 0xFF, 0xFF}));
        }

        [Test]
        public void TestGetBytesFromLong()
        {
            Assert.That(BitConverter.GetBytes(0L), Is.EqualTo(new byte[] {0, 0, 0, 0, 0, 0, 0, 0}));
            Assert.That(BitConverter.GetBytes(1L), Is.EqualTo(new byte[] {0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}));
            Assert.That(BitConverter.GetBytes(100L), Is.EqualTo(new byte[] {0x64, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}));
            Assert.That(BitConverter.GetBytes(1000L), Is.EqualTo(new byte[] {0xE8, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}));
            Assert.That(BitConverter.GetBytes(1000000L), Is.EqualTo(new byte[] {0x40, 0x42, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00}));
            Assert.That(BitConverter.GetBytes(1000000000L), Is.EqualTo(new byte[] {0x00, 0xCA, 0x9A, 0x3B, 0x00, 0x00, 0x00, 0x00}));
            Assert.That(BitConverter.GetBytes(1000000000000L), Is.EqualTo(new byte[] {0x00, 0x10, 0xA5, 0xD4, 0xE8, 0x00, 0x00, 0x00}));
            Assert.That(BitConverter.GetBytes(1000000000000000L), Is.EqualTo(new byte[] {0x00, 0x80, 0xC6, 0xA4, 0x7E, 0x8D, 0x03, 0x00}));
            Assert.That(BitConverter.GetBytes(1000000000000000000L), Is.EqualTo(new byte[] {0x00, 0x00, 0x64, 0xA7, 0xB3, 0xB6, 0xE0, 0x0D}));
            Assert.That(BitConverter.GetBytes(long.MaxValue - 1), Is.EqualTo(new byte[] {0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F}));
            Assert.That(BitConverter.GetBytes(long.MaxValue), Is.EqualTo(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F}));
            Assert.That(BitConverter.GetBytes(long.MinValue), Is.EqualTo(new byte[] {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80}));
            Assert.That(BitConverter.GetBytes(long.MinValue + 1), Is.EqualTo(new byte[] {0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80}));
            Assert.That(BitConverter.GetBytes(-1000000000000000000L), Is.EqualTo(new byte[] {0x00, 0x00, 0x9C, 0x58, 0x4C, 0x49, 0x1F, 0xF2}));
            Assert.That(BitConverter.GetBytes(-1000000000000000L), Is.EqualTo(new byte[] {0x00, 0x80, 0x39, 0x5B, 0x81, 0x72, 0xFC, 0xFF}));
            Assert.That(BitConverter.GetBytes(-1000000000000L), Is.EqualTo(new byte[] {0x00, 0xF0, 0x5A, 0x2B, 0x17, 0xFF, 0xFF, 0xFF}));
            Assert.That(BitConverter.GetBytes(-1000000000L), Is.EqualTo(new byte[] {0x00, 0x36, 0x65, 0xC4, 0xFF, 0xFF, 0xFF, 0xFF}));
            Assert.That(BitConverter.GetBytes(-1000000L), Is.EqualTo(new byte[] {0xC0, 0xBD, 0xF0, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF}));
            Assert.That(BitConverter.GetBytes(-1000L), Is.EqualTo(new byte[] {0x18, 0xFC, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF}));
            Assert.That(BitConverter.GetBytes(-100L), Is.EqualTo(new byte[] {0x9C, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF}));
            Assert.That(BitConverter.GetBytes(-1L), Is.EqualTo(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF}));
        }

        [Test]
        public void TestGetBytesFromFloat()
        {
            Assert.That(BitConverter.GetBytes(0f), Is.EqualTo(new byte[] { 0x00, 0x00, 0x00, 0x00 }));
            Assert.That(BitConverter.GetBytes(0.1f), Is.EqualTo(new byte[] { 0xCD, 0xCC, 0xCC, 0x3D }));
            Assert.That(BitConverter.GetBytes(0.001f), Is.EqualTo(new byte[] { 0x6F, 0x12, 0x83, 0x3A }));
            Assert.That(BitConverter.GetBytes(0.000001f), Is.EqualTo(new byte[] { 0xBD, 0x37, 0x86, 0x35 }));
            Assert.That(BitConverter.GetBytes(0.000000001f), Is.EqualTo(new byte[] { 0x5F, 0x70, 0x89, 0x30 }));
            Assert.That(BitConverter.GetBytes(0.000000000001f), Is.EqualTo(new byte[] { 0xCC, 0xBC, 0x8C, 0x2B }));
            Assert.That(BitConverter.GetBytes(0.000000000000001f), Is.EqualTo(new byte[] { 0x7D, 0x1D, 0x90, 0x26 }));
            Assert.That(BitConverter.GetBytes(0.000000000000000001f), Is.EqualTo(new byte[] { 0xEF, 0x92, 0x93, 0x21 }));
            Assert.That(BitConverter.GetBytes(0.000000000000000000001f), Is.EqualTo(new byte[] { 0xA0, 0x1D, 0x97, 0x1C }));
            Assert.That(BitConverter.GetBytes(0.000000000000000000000001f), Is.EqualTo(new byte[] { 0x15, 0xBE, 0x9A, 0x17 }));
            Assert.That(BitConverter.GetBytes(0.000000000000000000000000001f), Is.EqualTo(new byte[] { 0xD2, 0x74, 0x9E, 0x12 }));
            Assert.That(BitConverter.GetBytes(0.000000000000000000000000000001f), Is.EqualTo(new byte[] { 0x60, 0x42, 0xA2, 0x0D }));
            Assert.That(BitConverter.GetBytes(0.000000000000000000000000000000001f), Is.EqualTo(new byte[] { 0x4C, 0x27, 0xA6, 0x08 }));
            Assert.That(BitConverter.GetBytes(0.000000000000000000000000000000000001f), Is.EqualTo(new byte[] { 0x25, 0x24, 0xAA, 0x03 }));
            Assert.That(BitConverter.GetBytes(0.000000000000000000000000000000000000001f), Is.EqualTo(new byte[] { 0x98, 0xE3, 0x0A, 0x00 }));
            Assert.That(BitConverter.GetBytes(0.000000000000000000000000000000000000000001f), Is.EqualTo(new byte[] { 0xCA, 0x02, 0x00, 0x00 }));
            Assert.That(BitConverter.GetBytes(0.000000000000000000000000000000000000000000001f), Is.EqualTo(new byte[] { 0x01, 0x00, 0x00, 0x00 }));
            // This literal is too small for float. But .Net accepts it:) and BitConverter returns just zeroes instead of overflow.
            // Java doesn't accept the literal - syntax error, so we can't check what BitConverter will return in tis case:).
            // 0.0000000000000000000000000000000000000000000001f
#if !JAVA
            Assert.That(BitConverter.GetBytes(0.0000000000000000000000000000000000000000000001f), Is.EqualTo(new byte[] { 0x00, 0x00, 0x00, 0x00 }));
#endif
            Assert.That(BitConverter.GetBytes(1f), Is.EqualTo(new byte[] { 0x00, 0x00, 0x80, 0x3F }));
            Assert.That(BitConverter.GetBytes(1000f), Is.EqualTo(new byte[] { 0x00, 0x00, 0x7A, 0x44 }));
            Assert.That(BitConverter.GetBytes(-1000f), Is.EqualTo(new byte[] { 0x00, 0x00, 0x7A, 0xC4 }));
            Assert.That(BitConverter.GetBytes(float.MaxValue), Is.EqualTo(new byte[] { 0xFF, 0xFF, 0x7F, 0x7F }));
            Assert.That(BitConverter.GetBytes(float.MinValue), Is.EqualTo(new byte[] { 0xFF, 0xFF, 0x7F, 0xFF }));
        }
    }
}