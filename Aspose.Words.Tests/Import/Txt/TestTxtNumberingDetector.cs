// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2012 by Alexey Butalov
using Aspose.Words.RW.Txt.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Txt
{
    /// <summary>
    /// Tests how TxtNumberingDetector detects numbering in various text.
    /// </summary>
    [TestFixture]
    public class TestTxtNumberingDetector
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
        }

        [Test]
        public void TestDetectForEmptySource()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("", false);
            Assert.That(numbering, Is.Null);
        }

        [Test]
        public void TestDetectLowerAlphaRightBracket()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("b) some text", false);
            Assert.That(numbering, IsNot.Null());
            Assert.That(numbering.Text, Is.EqualTo("b)"));
            Assert.That(numbering.Numbers.Length, Is.EqualTo(1));
            Assert.That(numbering.Numbers[0], Is.EqualTo("b"));
        }

        [Test]
        public void TestDetectLowerAlphaDot()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("c. some text", false);
            Assert.That(numbering, IsNot.Null());
            Assert.That(numbering.Text, Is.EqualTo("c."));
            Assert.That(numbering.Numbers.Length, Is.EqualTo(1));
            Assert.That(numbering.Numbers[0], Is.EqualTo("c"));
        }


        [Test]
        public void TestDetectUpperAlphaRightBracket()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("D) some text", false);
            Assert.That(numbering, IsNot.Null());
            Assert.That(numbering.Text, Is.EqualTo("D)"));
            Assert.That(numbering.Numbers.Length, Is.EqualTo(1));
            Assert.That(numbering.Numbers[0], Is.EqualTo("D"));
        }

        [Test]
        public void TestDetectUpperAlphaDot()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("E. some text", false);
            Assert.That(numbering, IsNot.Null());
            Assert.That(numbering.Text, Is.EqualTo("E."));
            Assert.That(numbering.Numbers.Length, Is.EqualTo(1));
            Assert.That(numbering.Numbers[0], Is.EqualTo("E"));
        }

        [Test]
        public void TestDetectFalse()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("AB. some text", false);
            Assert.That(numbering, Is.Null);
        }

        [Test]
        public void TestDetectAsterisk()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("* some text", false);
            Assert.That(numbering, IsNot.Null());
            Assert.That(numbering.Text, Is.EqualTo("*"));
            Assert.That(numbering.Numbers.Length, Is.EqualTo(1));
            Assert.That(numbering.Numbers[0], Is.EqualTo("*"));
        }

        [Test]
        public void TestDetectDash()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("- some text", false);
            Assert.That(numbering, IsNot.Null());
            Assert.That(numbering.Text, Is.EqualTo("-"));
            Assert.That(numbering.Numbers.Length, Is.EqualTo(1));
            Assert.That(numbering.Numbers[0], Is.EqualTo("-"));
        }

        [Test]
        public void TestDetectArabicDotLevel1()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("3. some text", false);
            Assert.That(numbering, IsNot.Null());
            Assert.That(numbering.Text, Is.EqualTo("3."));
            Assert.That(numbering.Numbers.Length, Is.EqualTo(1));
            Assert.That(numbering.Numbers[0], Is.EqualTo("3"));
        }

        [Test]
        public void TestDetectArabicDotZerro()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("0. some text", false);
            Assert.That(numbering, Is.Null);
        }

        [Test]
        public void TestDetectArabicNumberRightBracket()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("1) some text", false);
            Assert.That(numbering, IsNot.Null());
            Assert.That(numbering.Text, Is.EqualTo("1)"));
            Assert.That(numbering.Numbers.Length, Is.EqualTo(1));
            Assert.That(numbering.Numbers[0], Is.EqualTo("1"));
        }

        [Test]
        public void TestDetectArabicDotLevel3()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("2.1.31. some text", false);
            Assert.That(numbering, IsNot.Null());
            Assert.That(numbering.Text, Is.EqualTo("2.1.31."));
            Assert.That(numbering.Numbers.Length, Is.EqualTo(3));
            Assert.That(numbering.Numbers[0], Is.EqualTo("2"));
            Assert.That(numbering.Numbers[1], Is.EqualTo("1"));
            Assert.That(numbering.Numbers[2], Is.EqualTo("31"));
        }

        [Test]
        public void TestDetectRomanLowercaseDot()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("mmdc. some text", false);
            Assert.That(numbering, IsNot.Null());
            Assert.That(numbering.Text, Is.EqualTo("mmdc."));
            Assert.That(numbering.Numbers.Length, Is.EqualTo(1));
            Assert.That(numbering.Numbers[0], Is.EqualTo("mmdc"));
        }

        [Test]
        public void TestDetectRomanLowercaseRightBracket()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("xxiv) some text", false);
            Assert.That(numbering, IsNot.Null());
            Assert.That(numbering.Text, Is.EqualTo("xxiv)"));
            Assert.That(numbering.Numbers.Length, Is.EqualTo(1));
            Assert.That(numbering.Numbers[0], Is.EqualTo("xxiv"));
        }

        [Test]
        public void TestDetectRomanUppercaseDot()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("XXIII. some text", false);
            Assert.That(numbering, IsNot.Null());
            Assert.That(numbering.Text, Is.EqualTo("XXIII."));
            Assert.That(numbering.Numbers.Length, Is.EqualTo(1));
            Assert.That(numbering.Numbers[0], Is.EqualTo("XXIII"));
        }

        [Test]
        public void TestDetectRomanUppercaseRightBracket()
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect("MXVII) some text", false);
            Assert.That(numbering, IsNot.Null());
            Assert.That(numbering.Text, Is.EqualTo("MXVII)"));
            Assert.That(numbering.Numbers.Length, Is.EqualTo(1));
            Assert.That(numbering.Numbers[0], Is.EqualTo("MXVII"));
        }
    }
}
