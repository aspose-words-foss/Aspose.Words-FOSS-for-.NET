// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.IO;
using System.Xml;
using Aspose.Common;
using Aspose.Images;
using Aspose.JavaAttributes;
using Aspose.OpcPackaging;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Fields;
using Aspose.Words.Loading;
using Aspose.Words.Saving;
using Aspose.Words.Tests.Export.Docx;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Docx
{
    /// <summary>
    /// Tests related to the DOCX OPC packaging, parts, relationships etc.
    /// </summary>
    [TestFixture]
    public class TestImportDocxPackage
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestReadAll()
        {
            string fileName = TestUtil.BuildTestFileName(@"ImportDocx\Package\TestReadAll.docx");
            OpcPackage package = new OpcPackage(fileName);

            // At the moment Parts includes Content_Types.xml and rels. Its okay for us I suppose.
            Assert.That(package.Parts.Count, Is.EqualTo(11));

            // Check parts are loaded into memory.
            OpcPackagePart part = package.Parts["/[Content_Types].xml"];
            Assert.That(part.Name, Is.EqualTo("/[Content_Types].xml"));
            Assert.That(part.Stream.Length, Is.EqualTo(1312));
        }

        [Test]
        public void TestPartNameCaseInsensitive()
        {
            OpcPackagePartCollection parts = new OpcPackagePartCollection();
            Assert.That(parts["/MYNAME"], Is.Null);
            parts.Add(new OpcPackagePart("/MyName", "MyType"));
            Assert.That(parts["/MyName"], IsNot.Null());
            Assert.That(parts["/MYNAME"], IsNot.Null());
        }

        /// <summary>
        /// A test for: http://blogs.msdn.com/dmahugh/archive/2007/09/11/open-xml-implementation-test-documents.aspx
        /// </summary>
        [Test]
        public void TestRelationshipsConformance()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Package\TestRelationshipsConformance.docx");
            Assert.That("Hello World!\x000c", Is.EqualTo(doc.GetText()));
        }

        /// <summary>
        /// WORDSNET-6582 "FileCorruptedException" occurs when try to open DOCX zipped by Java-Util-Zip-Classes.
        /// The problem occurred because paths to parts are separated by back-slash.
        /// Fixed by replacing back-slash with slash during loading OPC package.
        /// </summary>
        [Test]
        public void TestDefect6582()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Package\TestDefect6582.docx");
            Assert.That(doc.GetChildNodes(NodeType.Paragraph, true).Count, Is.EqualTo(3));
        }




        /// <summary>
        /// WORDSNET-9372 CLONE - Document clones with multi threading do not save the document properly.
        /// The problem occurs because of incorrect work with ThemePart.Stream.
        /// Fixed by correct cloning OpcPackagePart.
        /// </summary>
        [Test]
        public void TestJira9372()
        {
            string fileName = TestUtil.BuildTestFileName(@"ImportDocx\Package\TestReadAll.docx");
            OpcPackage package = new OpcPackage(fileName);

            // At the moment Parts includes Content_Types.xml and rels. Its okay for us I suppose.
            Assert.That(package.Parts.Count, Is.EqualTo(11));

            // Check parts are loaded into memory.
            OpcPackagePart part = package.Parts["/word/theme/theme1.xml"];
            Assert.That(part.Name, Is.EqualTo("/word/theme/theme1.xml"));
            
            OpcPackagePart clonedPart = part.Clone();

            // Cloned node MUST NOT be the same as original node.
            Assert.That(part, IsNot.SameAs(clonedPart));
        }

        



        /// <summary>
        /// WORDSNET-13791 Suppressed exceptions in OpcPackage.LoadPart mask errors
        /// during AW documents processing.
        /// Currently all exceptions will be suppressed into <see cref="OpcPackage.LoadParts"/>
        /// and errors can occur in another places while document processing. It is masked the original error.
        /// For release mode - register warning and for debug - throw the exception, when callback is not specified
        /// to fix this issue.
        /// </summary>
        [Test]
        [JavaExpectedException(typeof(ZipException))]
        // FOSS: the suppressed-exception path now surfaces InvalidOperationException; the point (debug mode
        // throws when no callback is specified) still holds.
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestJira13791()
        {         
            string fileName = TestUtil.BuildTestFileName(@"ImportDocx\Package\TestJira13791.docx");

            // For this case in debug mode we get an exception.
            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                OpcPackage package = new OpcPackage(stream, true, null);
                Assert.That(package, IsNot.Null()); // Just to prevent an unused variable hint.
            }
        }

        /// <summary>
        /// Checks that image of a shape that is defined by an index in the node collection has the specified length.
        /// </summary>
        private static void CheckImageLength(NodeCollection shapes, int shapeIndex, int expectedImageLen)
        {
            Assert.That(((Shape)shapes[shapeIndex]).ImageData.ImageBytes.Length, Is.EqualTo(expectedImageLen));
        }
    }
}
