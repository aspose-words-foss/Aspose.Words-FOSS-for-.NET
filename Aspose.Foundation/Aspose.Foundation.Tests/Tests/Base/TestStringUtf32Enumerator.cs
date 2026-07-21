// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2012 by Konstantin Kornilov

using System.Text;
using Aspose.Bidi;
using Aspose.Collections;
using NUnit.Framework;

namespace Aspose.Tests.Base
{
    [TestFixture]
    public class TestStringUtf32Enumerator
    {
        [Test]
        public void TestEnumerateUtf32EmptyString()
        {
            StringUtf32Enumerator enumerator = new StringUtf32Enumerator("");
            Assert.That(enumerator.MoveNext(), Is.False);
            Assert.That(enumerator.MoveNext(), Is.False);
        }

        [Test]
        public void TestEnumerateUtf32NullString()
        {
            StringUtf32Enumerator enumerator = new StringUtf32Enumerator(null);
            Assert.That(enumerator.MoveNext(), Is.False);
            Assert.That(enumerator.MoveNext(), Is.False);
        }

        [Test]
        public void TestEnumerateUtf32SimpleString()
        {
            TestEnumerateUtf32('a', 'b', 'c');
        }

        [Test]
        public void TestEnumerateUtf32SupplementaryString()
        {
            const int char1 = 0x1F680; // U+1F680 ROCKET.
            const int char2 = 0x1F681; // U+1F681 HELICOPTER.
            const int char3 = 0x1F682; // U+1F682 STEAM LOCOMOTIVE.

            TestEnumerateUtf32(char1, char2, char3);
        }

        [Test]
        public void TestEnumerateUtf32MixedString()
        {
            const int char1 = 0x1F680; // U+1F680 ROCKET.
            const int char2 = 0x1F681; // U+1F681 HELICOPTER.
            const int char3 = 0x1F682; // U+1F682 STEAM LOCOMOTIVE.

            TestEnumerateUtf32('a', char1, 'b', char2, 'c', char3, 'd');
        }

        [Test]
        public void TestInvalidSurrogates()
        {
            const int supplementaryChar = 0x1F680; // U+1F680 ROCKET.
            char leadSurrogate = UnicodeUtil.ConvertFromUtf32(supplementaryChar)[0];
            char tailSurrogate = UnicodeUtil.ConvertFromUtf32(supplementaryChar)[1];
            const char normalChar = 'A';
            
            TestEnumerateUtf32(leadSurrogate);
            TestEnumerateUtf32(tailSurrogate);
            TestEnumerateUtf32(leadSurrogate, leadSurrogate);
            TestEnumerateUtf32(tailSurrogate, tailSurrogate);
            TestEnumerateUtf32(tailSurrogate, leadSurrogate);
            TestEnumerateUtf32(tailSurrogate, normalChar, leadSurrogate);
            TestEnumerateUtf32(leadSurrogate, normalChar, tailSurrogate);
            TestEnumerateUtf32(normalChar, leadSurrogate, leadSurrogate, normalChar);
            TestEnumerateUtf32(normalChar, supplementaryChar, normalChar);
            TestEnumerateUtf32(tailSurrogate, leadSurrogate, normalChar);
            TestEnumerateUtf32(leadSurrogate, supplementaryChar, tailSurrogate);
        }

        private static void TestEnumerateUtf32(params int[] utf32Characters)
        {
            // Build a test string from the provided characters.
            StringBuilder testStringBuilder = new StringBuilder();
            IntList offsets = new IntList();
            foreach (int utf32Character in utf32Characters)
            {
                offsets.Add(testStringBuilder.Length);
                UnicodeUtil.AppendUtf32(testStringBuilder, utf32Character);
            }

            // Test character enumeration.
            StringUtf32Enumerator enumerator = new StringUtf32Enumerator(testStringBuilder.ToString());
            for (int i = 0; i < utf32Characters.Length; i++)
            {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(utf32Characters[i]));
                Assert.That(enumerator.Current, Is.EqualTo(utf32Characters[i]));
                Assert.That(enumerator.Offset, Is.EqualTo(offsets[i]));
            }

            // The enumerator has reached the end of the sequence and it cannot return any more characters.
            // Double check is intentional here.
            Assert.That(enumerator.MoveNext(), Is.False);
            Assert.That(enumerator.MoveNext(), Is.False);
        }
    }
}
