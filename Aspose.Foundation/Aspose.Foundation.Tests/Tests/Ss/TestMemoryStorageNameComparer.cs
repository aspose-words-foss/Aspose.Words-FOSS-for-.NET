// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2014 by Pavel Gorbunov

using System.Collections.Generic;
using Aspose.Ss;
using NUnit.Framework;

namespace Aspose.Tests.Ss
{
    [TestFixture]
    public class TestMemoryStorageNameComparer
    {
        [Test]
        public void TestBasic()
        {
            IComparer<string> comparer = new MemoryStorageNameComparer();

            Assert.That(comparer.Compare("1Table", "1Table"), Is.EqualTo(0));
            Assert.That(comparer.Compare("1table", "1Table"), Is.EqualTo(0));
            Assert.That(comparer.Compare("Item", "Properties"), Is.EqualTo(-1));
            
            // String.Compare() incorrectly compares some special characters like 0x01, 0x03 and 0x05 that occur in stream names.
            Assert.That(comparer.Compare("\x0001CompObj", "\x0001COMPObj"), Is.EqualTo(0));
            Assert.That(comparer.Compare("\x0001CompObj", "\x0003CompObj"), Is.EqualTo(-2));
            Assert.That(comparer.Compare("\x0001CompObj", "MsoDataStore"), Is.EqualTo(-1));
            Assert.That(comparer.Compare("\x0001\x0003\x0005", "\x0001\x0005\x0003"), Is.EqualTo(-2));
            
            // The underscore character is treated as greater than all alpha (latin only) chars.
            Assert.That(comparer.Compare("_a", "a_"), Is.EqualTo(30));

            // "ß".ToUpper() gives "ß" in .NET, whereas "ß".ToUpper() gives "SS" in Java,
            // so we refused from String.ToUpper()
            Assert.That(comparer.Compare("TßÄÒXÓÛRPEKÁÅÑÛÝCEUGÇÐ==", "TÌÛNÇØ25ÐÄÎÄOÝEOXÏÊNQQ=="), Is.EqualTo(19));
            Assert.That(comparer.Compare("ÄÄßBJIÏÚ×UÒFVÓJYVÝÎÍSQ==", "ÄÐÆPIÑÒÒÝÄKHÐÑ2XTU01ÉÀ=="), Is.EqualTo(-12));
            Assert.That(comparer.Compare("ÆÃÂCNÈ1ÒTÔOÐÅØ21ÙXRJßÐ==", "XÉÁÇÌÐÕÛCÔÖØÀFLÇÁÀWSÆÀ=="), Is.EqualTo(110));
            Assert.That(comparer.Compare("ËØKOMÐGØYEW2ÝWFÛOWÉÎÉQ==", "ËØKOMÐGØYEW2ÝWFÛOWÉÎÉQ=="), Is.EqualTo(0));
            Assert.That(comparer.Compare("ÌBÄY2ÂGFÜEÆWÛLÒLÆZÒG3À==", "ÌÌÆGNHÜYÂU0OBÀÀÅÚÌHKÊÀ=="), Is.EqualTo(-138));
            Assert.That(comparer.Compare("ÙÌQÆÆJÚÎÀÔ0ßÚTOEÛÁÎEÌÀ==", "ÛWTÃSÈUCÌUÆÖÜYDÀÑÁÙIÛQ=="), Is.EqualTo(-2));
        }
    }
}
