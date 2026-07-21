// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/07/2005 by Roman Korchagin
// 2016/06/20 by Anatoliy Sidorenko

using System.IO;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.IO
{
    [TestFixture]
    public class TestMemoryStream
    {
        /// <summary>
        /// Not really much testing because MemoryStream is directly ported from the Rotor source.
        /// </summary>
        [Test]
        public void TestResizableReadWrite()
        {
            MemoryStream s = new MemoryStream();

            //Test properties
            Assert.That(s.Length, Is.EqualTo(0));
            Assert.That(s.Position, Is.EqualTo(0));

            byte[] b = { 1, 2, 3, 4 };
            s.Write(b, 0, b.Length);

            //Test data was added
            Assert.That(s.Length, Is.EqualTo(4));
            Assert.That(s.Position, Is.EqualTo(4));

            //Test change position
            s.Position = 0;
            Assert.That(s.Position, Is.EqualTo(0));

            //Test read data back
            byte[] x = new byte[10];
            Assert.That(s.Read(x, 0, 10), Is.EqualTo(4));
            Assert.That(x[0], Is.EqualTo(1));
            Assert.That(x[0], Is.EqualTo(1));
            Assert.That(x[1], Is.EqualTo(2));
            Assert.That(x[2], Is.EqualTo(3));
            Assert.That(x[3], Is.EqualTo(4));

            //Check read past end returns zero bytes.
            Assert.That(s.Read(x, 0, 10), Is.EqualTo(0));
        }
    }
}
