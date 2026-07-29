// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2013 by Andrey Noskov
using Aspose.Words.Loading;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import
{
    /// <summary>
    /// Tests importing documents from web by URL.
    /// </summary>
    [TestFixture]
    public class TestImportFromWeb
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests load Document by URI, without specifying loading options.
        /// </summary>
        [Test, Ignore("TestImportFromWeb")]
        public void TestLoadDocumentByUrl()
        {
            // I think for testing we can simply read robots.txt it is always there and it is a valid document.
            Document doc = new Document("https://www.aspose.com/robots.txt");
            Assert.That(doc.OriginalLoadFormat, Is.EqualTo(LoadFormat.Text));
        }
    }
}
