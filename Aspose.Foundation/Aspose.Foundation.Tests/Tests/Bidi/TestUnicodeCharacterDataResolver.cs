// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/04/2016 by Alexander Zhiltsov

using Aspose.Bidi;
using NUnit.Framework;

namespace Aspose.Tests.Bidi
{
    /// <summary>
    /// Tests the <see cref="UnicodeCharacterDataResolver"/> class.
    /// </summary>
    [TestFixture]
    public class TestUnicodeCharacterDataResolver
    {
        /// <summary>
        /// Tests that the compose map is initialized well.
        /// </summary>
        [TestCase("\u0020\u0308", '\u00A8')]
        [TestCase("\u22B5\u0338", '\u22ED')]
        [TestCase("\u0031\u0030", '\u2469')]
        [TestCase("\u062C\u0644\u0020\u062C\u0644\u0627\u0644\u0647", '\uFDFB')]
        [TestCase("\u25CB", '\uFFEE')]
        public void TestComposeMap(string key, char expectedValue)
        {
            Assert.That(UnicodeCharacterDataResolver.Compose(key), Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that the decompose map is initialized well.
        /// </summary>
        [TestCase('\u00A8', "\u0020\u0308")]
        [TestCase('\u22ED', "\u22B5\u0338")]
        [TestCase('\u2469', "\u0031\u0030")]
        [TestCase('\uFDFB', "\u062C\u0644\u0020\u062C\u0644\u0627\u0644\u0647")]
        [TestCase('\uFFEE', "\u25CB")]
        public void TestDecomposeMap(char key, string expectedValue)
        {
            Assert.That(UnicodeCharacterDataResolver.GetUnicodeDecompositionMapping(key), Is.EqualTo(expectedValue));
        }
    }
}
