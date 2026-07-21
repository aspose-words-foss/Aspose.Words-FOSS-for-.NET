// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/08/2010 by Konstantin Sidorenko
// 2016/01/20 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestStringFormat
    {
        [Test]
        public void TestHello()
        {
            String hello = String.Format("Hello, {0}!", "World");
            Assert.That(hello, Is.EqualTo("Hello, World!"));
            Assert.That(String.Format("{0},{1},{2}", 1, 2, 3), Is.EqualTo("1,2,3"));
        }

        [Test]
        public void TestEscapes()
        {
            Assert.That(String.Format("Hello, '{0}'!", "World"), Is.EqualTo("Hello, 'World'!"));
            Assert.That(String.Format("{{{0}}},{{{{{1},{2}}}}}}}", 1, 2, 3), Is.EqualTo("{1},{{2,3}}}"));
        }

        [Test]
        public void TestNull()
        {
#if PLAIN_JAVA
            msAssert.areEqual("Hello, ''!", msString.format("Hello, '{0}'!", null));
#endif
            Assert.That(String.Format("{0},{1}", null, null), Is.EqualTo(","));
            Assert.That(String.Format("{0},{1},{2}", null, null, null), Is.EqualTo(",,"));

            Object o1 = null, o2 = null, o3 = null;
            Assert.That(String.Format("Hello, '{0}'!", o1), Is.EqualTo("Hello, ''!"));
            Assert.That(String.Format("{0},{1}", o1, o2), Is.EqualTo(","));
            Assert.That(String.Format("{0},{1},{2}", o1, o2, o3), Is.EqualTo(",,"));
            
        }

        [Test]
        public void TestHexadecimal()
        {
            Assert.That(String.Format("{0:X},{1:X}", 0xABC, 0xCDE42), Is.EqualTo("ABC,CDE42"));
            Assert.That(String.Format("0x{0:X},0x{1:X}", 0xABC, 0xCDE42), Is.EqualTo("0xABC,0xCDE42"));
        }

        [Test]
        public void TestDigits()
        {
            Assert.That(String.Format("{0:X2},{1:X10}", 0xABC, 0xCDE42), Is.EqualTo("ABC,00000CDE42"));
            Assert.That(String.Format("{0:X5},{1:X10}", 0xABC, 0xCDE42), Is.EqualTo("00ABC,00000CDE42"));
            Assert.That(String.Format("{0:d3},{1:D10}", 12345, 12345678), Is.EqualTo("12345,0012345678"));
        }

        [Test]
        public void TestUnsignedDigits()
        {
            byte b = 0x1B;
            ushort ush = 0x32FF;
            uint ui = 0xf2374322U;
            Assert.That(string.Format("{0:X2},{1:X4},{2:X8}", b, ush, ui), Is.EqualTo("1B,32FF,F2374322"));
            Assert.That(string.Format("{0:X2},{1:X4},{2:X8}", 0x1B, 0x32FF, 0xf2374322U), Is.EqualTo("1B,32FF,F2374322"));
        }
    }
}
