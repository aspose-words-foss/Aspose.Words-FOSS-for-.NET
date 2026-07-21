// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2013 by Vadim Saltykov

using System.IO;
using Aspose.Crypto;
using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import
{
    /// <summary>
    /// Tests importing documents from Azure storage services.
    /// </summary>
    [TestFixture]
    public class TestImportFromAzure
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WORDSNET-25139 Image is not loaded properly if the document is loaded from LazyLoadingReadOnlyStream.
        /// The check for complete reading of data from LazyLoadingReadOnlyStream has been added.
        /// </summary>
        /// <remarks>
        /// LazyLoadingReadOnlyStream does not return all the data at once if their size exceeds 4Mb.
        /// ZipEntry.GetCompressedBytes() did not check the stream for full reading, which led to the fact that
        /// the buffer that was further used as the basis for all OpcPackagePart streams was filled partially in some cases.
        /// </remarks>
        [Test]
        public void Test25139()
        {
            using (LazyLoadingReadOnlyStreamMock stream =
                new LazyLoadingReadOnlyStreamMock(TestUtil.BuildTestFileName(@"ImportDocx\Test25139.docx")))
            {
                Document doc = new Document(stream);
                Shape shape = doc.FirstSection.Body.Shapes[0];
                // Checks the hash of the problematic shape.
                byte[] hash = HashUtil.ComputeHash(DigestAlgorithm.MD5, shape.ImageData.ImageBytes);
                Assert.That(StringUtil.BytesToHex(hash).ToLower(), Is.EqualTo("cf06235e0dd07e87e21875582e390cde"));
            }
        }
    }

    /// <summary>
    /// Helper class that emulates LazyLoadingReadOnlyStream behavior in terms of chunk reading of a file located in Azure Blob storage.
    /// </summary>
    internal class LazyLoadingReadOnlyStreamMock : FileStream
    {
        [JavaAttributes.JavaThrows(true)]
        public LazyLoadingReadOnlyStreamMock(string testFile)
            : base(testFile, FileMode.Open)
        {
        }

        [JavaAttributes.JavaConvertCheckedExceptions]
        public override int Read(byte[] buffer, int offset, int count)
        {
            int bufferSize = System.Math.Min(count, DefaultAzureBlobBufferSize);
            return base.Read(buffer, offset, bufferSize);
        }

        private const int DefaultAzureBlobBufferSize = 4 * 1024 * 1024;
    }
}
