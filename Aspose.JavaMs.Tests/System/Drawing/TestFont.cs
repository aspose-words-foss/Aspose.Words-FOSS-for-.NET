// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/08/2016 by Anatoliy Sidorenko
#if !NETSTANDARD
using System.Drawing;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Drawing
{
    [TestFixture]
    [AndroidDelete("Font doesn't work the same way on Android")]
    public class TestFont
    {

        [Test]
        public void TestCtor()
        {
            TestCtor("Arial", 8.0f);
            TestCtor("Times New Roman", 8.1f);
            TestCtor("Arial", 8.0f, FontStyle.Bold);
            TestCtor("Times New Roman", 8.1f, FontStyle.Bold|FontStyle.Italic);
        }

        private void TestCtor(string fontName, float sizeInPoints)
        {
            Font testFont = new Font(fontName, sizeInPoints);
            Assert.That(fontName, Is.EqualTo(testFont.Name));
            Assert.That(sizeInPoints, Is.EqualTo(testFont.SizeInPoints));
        }

        private void TestCtor(string fontName, float sizeInPoints, FontStyle fontStyle)
        {
            Font testFont = new Font(fontName, sizeInPoints, fontStyle);
            Assert.That(fontName, Is.EqualTo(testFont.Name));
            Assert.That(sizeInPoints, Is.EqualTo(testFont.SizeInPoints));
            Assert.That(fontStyle, Is.EqualTo(testFont.Style));
        }
    }
}
#endif
