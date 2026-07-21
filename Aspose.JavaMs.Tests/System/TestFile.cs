// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/01/2007 by Konstantin Sidorenko
// 2015/12/30 by Anatoliy Sidorenko


using System.IO;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestFile
    {
        [Test]
        public void TestFileExist()
        {
            Assert.That(false, Is.EqualTo(File.Exists("")));
            Assert.That(false, Is.EqualTo(File.Exists(null)));

            Assert.That(false, Is.EqualTo(File.Exists(TestEnvironment.GetRawRoot() + "\\words-net\\words-net")));
            Assert.That(true, Is.EqualTo(File.Exists(TestEnvironment.GetRawRoot() + "\\words-net\\words-net\\Aspose.Words.sln")));
            Assert.That(true, Is.EqualTo(File.Exists(TestEnvironment.GetRawRoot() + "/words-net/words-net/Aspose.Words.sln")));
        }
    }
}
