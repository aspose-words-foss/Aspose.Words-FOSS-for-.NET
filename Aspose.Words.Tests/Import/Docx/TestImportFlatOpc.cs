// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System.IO;
using System.Text;
using Aspose.IO;
using Aspose.OpcPackaging;
using Aspose.TestFx;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Docx
{
    /// <summary>
    /// Miscelaneous import FlatOpc tests. Usually for strange files received from customers.
    /// </summary>
    [TestFixture]
    public class TestImportFlatOpc
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }



        /// <summary>
        /// WORDSNET-19626 Bookmark which is a direct child of Body is missed upon loading.
        /// The problem occurred because we lost inline content at the end of the document 
        /// if inline content is direct child of body. In this particular case it is bookmark at the end of the document.
        /// Fixed by adding this content to the last paragraph of Body once we finished reading Body.
        /// </summary>
        [Test]
        public void TestDefect19626()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportFlatOpc\TestDefect19626.fopc");
            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(9));
            Assert.That(doc.Range.Bookmarks["EndRangeHere"], IsNot.Null());
        }



        /// <summary>
        /// WORDSNET-17034 The source WordML is improperly uploaded.
        /// Missing mso-application tag.
        /// </summary>
        [Test]
        public void TestJira17034()
        {
            Document doc = TestUtil.OpenFromZip(@"ImportFlatOpc\TestJira17034.xml.zip");
            Assert.That(doc.FirstSection.Body.Tables.Count, Is.EqualTo(11));
        }
    }
}
