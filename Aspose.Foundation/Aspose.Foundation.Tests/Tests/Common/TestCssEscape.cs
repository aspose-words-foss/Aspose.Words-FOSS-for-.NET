// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/02/2016 by Victor Chebotok

using Aspose.Common;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Css
{
    /// <summary>
    /// Tests methods of the <see cref="CssEscape"/> class.
    /// </summary>
    [TestFixture]
    public class TestCssEscape
    {
        [Test]
        public void TestIdentifierNull()
        {
            CheckIdentifiers(null, null);
        }

        [Test]
        public void TestIdentifierEmpty()
        {
            CheckIdentifiers("", "");
        }

        [Test]
        public void TestIdentifierLetter()
        {
            CheckIdentifiers("a", "a");
        }

        [Test]
        public void TestIdentifierLetters()
        {
            CheckIdentifiers("abc", "abc");
        }

        [Test]
        public void TestIdentifierDigit()
        {
            CheckIdentifiers("1", @"\31");
        }

        [Test]
        public void TestIdentifierDigits()
        {
            CheckIdentifiers("123", @"\31 23");
        }

        [Test]
        public void TestIdentifierDigitAndLetter()
        {
            CheckIdentifiers("1a", @"\31 a");
        }

        [Test]
        public void TestIdentifierDigitAndLetterNoSpace()
        {
            CheckIdentifiers("1x", @"\31x");
        }

        [Test]
        public void TestIdentifierStartsWithDash()
        {
            CheckIdentifiers("-a", "-a");
        }

        [Test]
        public void TestIdentifierStartsWithDashDigit()
        {
            CheckIdentifiers("-1", @"-\31");
        }

        [Test]
        public void TestIdentifierStartsWithDashNonAscii()
        {
            CheckIdentifiers("-§", "-§");
        }

        [Test]
        public void TestIdentifierDash()
        {
            CheckIdentifiers("-", @"\-");
        }

        [Test]
        public void TestIdentifierDoubleDash()
        {
            CheckIdentifiers("--", @"-\-");
        }

        [Test]
        public void TestIdentifierUnderscore()
        {
            CheckIdentifiers("_", "_");
        }

        [Test]
        public void TestIdentifierDashUnderscore()
        {
            CheckIdentifiers("-_", "-_");
        }

        [Test]
        public void TestIdentifierPunctuation()
        {
            CheckIdentifiers("{a}", @"\{a\}");
        }

        [Test]
        public void TestIdentifierWhitespace()
        {
            CheckIdentifiers("1 a", @"\31\ a");
        }

        [Test]
        public void TestIdentifierNonAscii()
        {
            CheckIdentifiers("§§§", "§§§");
        }

        [Test]
        public void TestIdentifierComplex()
        {
            CheckIdentifiers("-12(a b)§", @"-\31 2\(a\ b\)§");
        }

        [Test]
        public void TestStringEmpty()
        {
            CheckString("", "");
        }

        [Test]
        public void TestStringWhitespace()
        {
            CheckString(" ", " ");
        }

        [Test]
        public void TestStringCrLf()
        {
            CheckString("\r\n", @"\D\A");
            CheckString("\r\nAdorable", @"\D\A Adorable");
            CheckString("\r\nIndifference", @"\D\AIndifference");
        }

        [Test]
        public void TestStringCr()
        {
            CheckString("\r", @"\D");
            CheckString("\r1", @"\D 1");
            CheckString("\rX", @"\DX");
        }

        [Test]
        public void TestStringLineFeed()
        {
            CheckString("\n", @"\A");
            CheckString("\n2", @"\A 2");
            CheckString("\nG", @"\AG");
        }

        [Test]
        public void TestStringFormFeed()
        {
            CheckString("\f", @"\C");
            CheckString("\fC", @"\C C");
            CheckString("\fR", @"\CR");
        }

        [Test]
        public void TestStringQuote()
        {
            CheckDoubleQuotedString("\"", "\\\"");
            CheckSingleQuotedString("'", "\\'");
        }

        [Test]
        public void TestStringBackslash()
        {
            CheckString("\\", "\\\\");
        }

        [Test]
        public void TestStringComplex()
        {
            const string originalStringValue = @"public class foo
{
private int x;

foo(int x) {
  this.x = x;
}
}";
            const string escapedStringValue = @"public class foo\D\A{\D\Aprivate int x;\D\A\D\A foo(int x) {\D\A   this.x = x;\D\A}\D\A}";
            CheckString(originalStringValue, escapedStringValue);
        }

        [TestCase("identifier")]
        [TestCase("-identifier")]
        [TestCase("_identifier")]
        [TestCase("а3")]
        [TestCase("-а3")]
        [TestCase("_3")]
        [TestCase("宋体")]
        public void TestIsValidIdentifier(string identifier)
        {
            Assert.That(CssEscape.IsValidIdentifier(identifier), Is.True);
        }

        [TestCase("font name")]
        [TestCase("(parameter)")]
        [TestCase("'quoted_text'")]
        [TestCase("\"quoted_text\"")]
        [TestCase("100")]
        [TestCase("3pt")]
        [TestCase("-3pt")]
        [TestCase("one,two")]
        public void TestIsInvalidIdentifier(string identifier)
        {
            Assert.That(CssEscape.IsValidIdentifier(identifier), Is.False);
        }

        private static void CheckIdentifiers(string originalUnescaped, string expectedEscaped)
        {
            string actualEscaped = CssEscape.EscapeIdentifier(originalUnescaped);
            Assert.That(actualEscaped, Is.EqualTo(expectedEscaped));
        }

        private static void CheckSingleQuotedString(string originalUnescaped, string expectedEscaped)
        {
            string actualEscaped = CssEscape.EscapeSingleQuotedString(originalUnescaped);
            Assert.That(actualEscaped, Is.EqualTo(expectedEscaped));
        }

        private static void CheckDoubleQuotedString(string originalUnescaped, string expectedEscaped)
        {
            string actualEscaped = CssEscape.EscapeDoubleQuotedString(originalUnescaped);
            Assert.That(actualEscaped, Is.EqualTo(expectedEscaped));
        }

        private static void CheckString(string originalUnescaped, string expectedEscaped)
        {
            CheckSingleQuotedString(originalUnescaped, expectedEscaped);
            CheckDoubleQuotedString(originalUnescaped, expectedEscaped);
        }
    }
}
