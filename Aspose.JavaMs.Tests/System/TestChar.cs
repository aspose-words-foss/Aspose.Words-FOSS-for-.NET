// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/01/2007 by Konstantin Sidorenko
// 2015/12/21 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestChar
    {
        [Test]
        public void TestCharIsWhiteSpace()
        {
            Assert.That(true, Is.EqualTo(Char.IsWhiteSpace(' ')));
            Assert.That(true, Is.EqualTo(Char.IsWhiteSpace('\u0009')));
            Assert.That(true, Is.EqualTo(Char.IsWhiteSpace('\n')));
            Assert.That(true, Is.EqualTo(Char.IsWhiteSpace('\u000b')));
            Assert.That(true, Is.EqualTo(Char.IsWhiteSpace('\f')));
            Assert.That(true, Is.EqualTo(Char.IsWhiteSpace('\r')));
            Assert.That(true, Is.EqualTo(Char.IsWhiteSpace('\u0085')));
            Assert.That(true, Is.EqualTo(Char.IsWhiteSpace('\u2028')));
            Assert.That(true, Is.EqualTo(Char.IsWhiteSpace('\u2029')));
        }

        [Test]
        public void TestToUpperInvariant()
        {
            Assert.That(Char.ToUpperInvariant(MicroSign), Is.EqualTo(MicroSign));
            Assert.That(Char.ToUpperInvariant('h'), Is.EqualTo('H'));
        }

        [Test]
        public void TestToLowerInvariant()
        {
            Assert.That(Char.ToLowerInvariant(MicroSign), Is.EqualTo(MicroSign));
            Assert.That(Char.ToLowerInvariant('H'), Is.EqualTo('h'));
        }

        // J1678: micro sign char stays the same while converting to upper or to lower invariant in .Net.
        private const char MicroSign = '\u00b5';
    }
}
