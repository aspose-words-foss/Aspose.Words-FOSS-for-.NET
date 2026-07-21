// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/11/2016 by Hasanov

using System.IO;
using Aspose.Words.Drawing.Core;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    [TestFixture]
    public class TestImageDataCore
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WORDSNET-14428 The ImageDataCore.GetIcoImageBytes(Stream stream, int imageIndex)
        /// incorrectly positions the stream reader before calling ImageUtil.PrependBmpHeader.
        /// ihasanov: The problem occurred because of not setting to offset when ico-file contains multiple entries.
        /// </summary>
        [Test]
        public void TestJira14428()
        {
            AssertIcoFile(TestUtil.BuildTestFileName(@"Model\Drawing\Core\icon_01.ico"), 2);
            AssertIcoFile(TestUtil.BuildTestFileName(@"Model\Drawing\Core\icon_02.ico"), 1);
            AssertIcoFile(TestUtil.BuildTestFileName(@"Model\Drawing\Core\icon_03.ico"), 2);
            AssertIcoFile(TestUtil.BuildTestFileName(@"Model\Drawing\Core\icon_04.ico"), 2);
            AssertIcoFile(TestUtil.BuildTestFileName(@"Model\Drawing\Core\icon_05.ico"), 4);
        }

        /// <summary>
        /// WORDSNET-18086 An exception occurred because AW tried to load PNG images stored in .ICO files as BMP images.
        [Test]
        public void TestJira18086()
        {
            AssertIcoFile(TestUtil.BuildTestFileName(@"Model\Drawing\Core\pngIcon.ico"), 1);
        }

        private static void AssertIcoFile(string icoFile, int entries)
        {
            for (int entryIdx = 0; entryIdx < entries; entryIdx++)
            {
                using (FileStream fileStream = new FileStream(icoFile, FileMode.Open, FileAccess.Read))
                {
                    Assert.That(TestUtil.ReadAllBytes(icoFile), IsNot.Null()); // check if ICO file has been read ok
                    Assert.That(ImageDataCore.GetIcoImageBytes(fileStream, entryIdx), IsNot.Null()); // check each ICO's image entry
                }
            }
        }
    }
}
