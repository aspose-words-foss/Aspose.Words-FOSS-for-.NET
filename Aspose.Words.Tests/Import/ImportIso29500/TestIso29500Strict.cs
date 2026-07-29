// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/03/2013 by Andrey Noskov
using Aspose.Words.Drawing;
using Aspose.Words.Fonts;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.ImportIso29500
{
    /// <summary>
    /// Test that OOXML ISO/IEC 29500 Strict compliance level files are opened correctly.
    /// </summary>
    [TestFixture]
    public class TestIso29500Strict
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// andrnosk: WORDSNET-7906 Read characterSet attribute (strict OOXML).
        /// </summary>
        [Test]
        public void TestJira7741CharacterSet()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportIso29500S\TestJira7741CharacterSet.docx");

            Assert.That(doc.ComplianceInfo.IsIsoStrict, Is.False);

            FontInfoCollection fontInfos = doc.FontInfos;

            Assert.That(fontInfos[0].Charset, Is.EqualTo(204));
            Assert.That(fontInfos[1].Charset, Is.EqualTo(204));
            Assert.That(fontInfos[2].Charset, Is.EqualTo(0));
            Assert.That(fontInfos[3].Charset, Is.EqualTo(177));
            Assert.That(fontInfos[14].Charset, Is.EqualTo(128));
            Assert.That(fontInfos[16].Charset, Is.EqualTo(129));
            Assert.That(fontInfos[29].Charset, Is.EqualTo(204));
            Assert.That(fontInfos[30].Charset, Is.EqualTo(0));
            Assert.That(fontInfos[62].Charset, Is.EqualTo(177));
            Assert.That(fontInfos[63].Charset, Is.EqualTo(136));
            Assert.That(fontInfos[79].Charset, Is.EqualTo(134));
            Assert.That(fontInfos[124].Charset, Is.EqualTo(134));
            Assert.That(fontInfos[151].Charset, Is.EqualTo(128));
            Assert.That(fontInfos[154].Charset, Is.EqualTo(136));
            Assert.That(fontInfos[161].Charset, Is.EqualTo(134));
            Assert.That(fontInfos[168].Charset, Is.EqualTo(177));
        }

        /// <summary>
        /// andrnosk: WORDSNET-8049 Resolve the problem with the difference 
        /// between Docx RelationshipTypes in strict and transitional OOXML.
        /// </summary>
        [Test]
        public void TestJira8049()
        {
            // Test document was created using MS Word 2013.
            Document doc = TestUtil.Open(@"ImportIso29500S\TestJira8049.docx");
            Assert.That(doc.ComplianceInfo.IsIsoStrict, Is.True);
        }

        /// <summary>
        /// andrnosk: WORDSNET-8241 (DocxNamespaces) Resolve the problem with the difference between namespaces 
        /// in strict and transitional OOXML.
        /// </summary>
        [Test]
        public void TestJira8241()
        {
            // Test document is Strict.
            Document doc = TestUtil.Open(@"ImportIso29500S\TestJira7741DocxNamespaces.docx");
            Assert.That(doc.ComplianceInfo.IsIsoStrict, Is.True);
            Assert.That(doc.GetChildNodes(NodeType.Shape, true).Count, Is.EqualTo(1));

            // This fix allows us correctly read DrawingMLRelatedDataCollection.
            Shape drawingML = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(drawingML.ImageData.HasImage, Is.True);
        }
    }
}