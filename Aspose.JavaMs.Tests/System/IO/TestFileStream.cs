// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/07/2005 by Roman Korchagin
// 2016/06/21 by Anatoliy Sidorenko

using System.IO;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.IO
{
    [TestFixture]
    public class TestFileStream
    {
        [Test]
        public void TestWrite()
        {
            // Write to TestOut rather than TestData, the test only needs a scratch file.
            string testFilePath = TestFxUtil.GetInTestOutPath("MsTestWrite.doc");
            TestFxUtil.EnsureDirectoryForFileExists(testFilePath);

            using (Stream dst = new FileStream(testFilePath, FileMode.Create))
            {

                //Test properties
                Assert.That(dst.Position, Is.EqualTo(0));
                Assert.That(dst.Length, Is.EqualTo(0));

                //Write data
                byte[] b = { 1, 2, 3, 4 };
                dst.Write(b, 0, b.Length);
                Assert.That(dst.Position, Is.EqualTo(4));
                Assert.That(dst.Length, Is.EqualTo(4));

                //Check it was written.
                dst.Position = 0;
                byte[] x = new byte[4];
                dst.Read(x, 0, 4);
                Assert.That(x[0], Is.EqualTo(1));
                Assert.That(x[1], Is.EqualTo(2));
                Assert.That(x[2], Is.EqualTo(3));
                Assert.That(x[3], Is.EqualTo(4));
            }
        }
    }
}
