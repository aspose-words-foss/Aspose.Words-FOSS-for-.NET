// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2025 by Victor Chebotok

using Aspose.Bidi;
using NUnit.Framework;

namespace Aspose.Foundation.Tests.Tests.Base
{
    [TestFixture]
    public class TestUnicodeUtil
    {
        [Test]
        public void TestReverseString_Null()
        {
            const string source = null;
            string reversed = UnicodeUtil.ReverseString(source);
            Assert.That(reversed, Is.SameAs(source));
        }

        [Test]
        public void TestReverseString_Empty()
        {
            const string source = "";
            string reversed = UnicodeUtil.ReverseString(source);
            Assert.That(reversed, Is.SameAs(source));
        }

        [Test]
        public void TestReverseString_SingleCharacter()
        {
            const string source = "A";
            string reversed = UnicodeUtil.ReverseString(source);
            Assert.That(reversed, Is.SameAs(source));
        }

        [Test]
        public void TestReverseString_NoSurrogatePairs()
        {
            const string source = "ABCDEF";
            string reversed = UnicodeUtil.ReverseString(source);
            Assert.That(reversed, Is.EqualTo("FEDCBA"));
        }

        [Test]
        public void TestReverseString_OneSurrogatePair()
        {
            const string source = Rocket;
            string reversed = UnicodeUtil.ReverseString(source);
            Assert.That(reversed, Is.EqualTo(Rocket));
        }

        [Test]
        public void TestReverseString_TwoSurrogatePairs()
        {
            const string source = Rocket + AceOfSpades;
            string reversed = UnicodeUtil.ReverseString(source);
            Assert.That(reversed, Is.EqualTo(AceOfSpades + Rocket));
        }

        [Test]
        public void TestReverseString_Mixed()
        {
            const string source = Rocket + "ABCD";
            string reversed = UnicodeUtil.ReverseString(source);
            Assert.That(reversed, Is.EqualTo("DCBA" + Rocket));
        }

        [Test]
        public void TestReverseString_MalformedPairs_1()
        {
            const string source = Lead1 + Lead2 + Tail2;
            string reversed = UnicodeUtil.ReverseString(source);
            Assert.That(reversed, Is.EqualTo(Lead2 + Tail2 + Lead1));
        }

        [Test]
        public void TestReverseString_MalformedPairs_2()
        {
            const string source = Lead1 + Tail1 + Tail2;
            string reversed = UnicodeUtil.ReverseString(source);
            Assert.That(reversed, Is.EqualTo(Tail2 + Lead1 + Tail1));
        }

        [Test]
        public void TestReverseString_MalformedPairs_3()
        {
            const string source = Lead1 + Lead2 + Tail2 + Tail1;
            string reversed = UnicodeUtil.ReverseString(source);
            Assert.That(reversed, Is.EqualTo(Tail1 + Lead2 + Tail2 + Lead1));
        }

        [Test]
        public void TestReverseString_MalformedPairs_4()
        {
            const string source = Tail1 + Lead1;
            string reversed = UnicodeUtil.ReverseString(source);
            Assert.That(reversed, Is.EqualTo(Lead1 + Tail1));
        }

        [Test]
        public void TestReverseString_Emojis()
        {
            const string Monkey = "\U0001F648"; // U+1F648 SEE-NO-EVIL MONKEY;

            const string Shrug = "\U0001F937"; // U+1F937 SHRUG
            const string DarkSkin = "\U0001F3FD"; // U+1F3FD EMOJI MODIFIER FITZPATRICK TYPE-4
            const string Zwj = "\u200D"; // U+200D ZERO WIDTH JOINER (ZWJ)
            const string Female = "\u2640"; // U+2640 FEMALE SIGN
            const string Emoji = "\uFE0F"; // U+FE0F VARIATION SELECTOR-16 (VS16) -- emoji variation selector
            const string WomanShrugDarkSkin = Shrug + DarkSkin + Zwj + Female + Emoji;

            const string source = Monkey + WomanShrugDarkSkin;
            string reversed = UnicodeUtil.ReverseString(source);

            // Note that the components of the second emoji are reversed. The method is not intended to correctly preserve
            // the emoji sequences.
            Assert.That(reversed, Is.EqualTo(Emoji + Female + Zwj + DarkSkin + Shrug + Monkey));
        }

        private const string Rocket = "\U0001F680"; // U+1F680 ROCKET
        private const string AceOfSpades = "\U0001F0A1"; // U+1F0A1 PLAYING CARD ACE OF SPADES

        // UTF-16 for U+1F680 ROCKET
        private const string Lead1 = "\uD83D";
        private const string Tail1 = "\uDE80";

        // UTF-16 for U+1F0A1 PLAYING CARD ACE OF SPADES
        private const string Lead2 = "\uD83C";
        private const string Tail2 = "\uDCA1";
    }
}
