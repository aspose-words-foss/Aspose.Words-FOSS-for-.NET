// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.IO;
using Aspose.JavaAttributes;
using Aspose.TestFx;
using Aspose.Words.Fields;
using Aspose.Words.Fonts;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Test document-wide features.
    /// </summary>
    [TestFixture]
    public class TestDocument
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void TestOpenFileNotFound()
        {
            new Document(@"C:\Dummy.doc");
        }


        // FOSS: TestAttachedTemplate was removed. It read attached-template metadata from .doc
        // inputs and round-tripped through .doc, all removed in FOSS. The AttachedTemplate
        // get/set + OOXML round-trip is still covered by TestJira2177.


        /// <summary>
        /// WORDSNET-2177 Consider adding “Automatically update document styles” option.
        /// andrnosk: Document.LinkStyles option is exposed to public API.
        /// The styles in the document will be updated to match the styles in the
        /// attached template each time the document is opened using MSWord.
        /// </summary>
        [Test]
        public void TestJira2177()
        {
            const string templateName = @"C:\MyPath\MyTemplate.dot";
            Document doc = new Document();
            doc.AttachedTemplate = templateName;

            Assert.That(doc.AutomaticallyUpdateStyles, Is.False);
            doc.AutomaticallyUpdateStyles = true;

            doc = TestUtil.SaveOpen(doc, @"Model\Document\TestJira2177", UnifiedScenario.Docx2DocxNoGold);

            Assert.That(doc.AttachedTemplate, Is.EqualTo(templateName));
            Assert.That(doc.AutomaticallyUpdateStyles, Is.True);
        }

        private static void CheckTemplateDoc(
            string srcTemplateName,
            string dstTemplateName,
            string dstDocumentName,
            LoadFormat templateFormatExpected,
            LoadFormat documentFormatExpected)
        {
            Document doc = TestUtil.Open(@"Model\Document\" + srcTemplateName);
            Assert.That(doc.OriginalLoadFormat, Is.EqualTo(templateFormatExpected));

            // Test save as a template.
            // Open in MS Word 97 and try Save As. Check saving to other types is disabled there.
            doc = TestUtil.SaveOpen(doc, @"Model\Document\" + dstTemplateName);
            Assert.That(doc.OriginalLoadFormat, Is.EqualTo(templateFormatExpected));

            // Test save as a normal document.
            // Open in MS Word 97 and try Save As. Check saving to other types is enabled there.
            doc = TestUtil.SaveOpen(doc, @"Model\Document\" + dstDocumentName);
            Assert.That(doc.OriginalLoadFormat, Is.EqualTo(documentFormatExpected));
        }

        [Test]
        public void TestCloneSectionAndContents()
        {
            Document doc = new Document();

            Section section = new Section(doc);
            section.PageSetup.SectionStart = SectionStart.OddPage;

            Body body = new Body(doc);
            section.AppendChild(body);

            Paragraph para = new Paragraph(doc);
            para.ParagraphFormat.LeftIndent = 10;
            body.AppendChild(para);

            Run run = new Run(doc, "Hello");
            run.Font.Bold = true;
            para.AppendChild(run);

            Section newSection = (Section)section.Clone(true);

            //Verify it was deep cloned.
            Assert.That(section.SectPr != newSection.SectPr, Is.True);    //Reference is not the same.
            Assert.That(newSection.PageSetup.SectionStart, Is.EqualTo(SectionStart.OddPage));    //But data is copied.

            Paragraph newPara = newSection.Body.FirstParagraph;
            Assert.That(newPara.ParaPr != para.ParaPr, Is.True);
            Assert.That(newPara.ParagraphFormat.LeftIndent, Is.EqualTo(10d));

            Run newRun = (Run)newPara.GetChildNodes(NodeType.Any, false)[0];
            Assert.That(newRun.ParentNode, Is.EqualTo(newPara), "Parent is not set properly.");
            Assert.That(newRun.RunPr != run.RunPr, Is.True);
            Assert.That(newRun.GetText(), Is.EqualTo("Hello"));
            Assert.That(newRun.Font.Bold, Is.EqualTo(true));
        }

        [Test]
        public void TestCloneFontInfo()
        {
            Document srcDoc = new Document();
            Assert.That(srcDoc.FontInfos.Count, Is.EqualTo(3));

            Document dstDoc = srcDoc.Clone();
            dstDoc.FontInfos.Merge(new FontInfo("TestFont"));
            //Check that FontInfo was copied from the original
            Assert.That(dstDoc.FontInfos.Count, Is.EqualTo(4));

            //Check that it was actually copied.
            Assert.That(srcDoc.FontInfos.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// When cloning a document, the event handlers of the new document should be null.
        /// </summary>
        [Test]
        public void TestCloneWithHandlers()
        {
            Document srcDoc = new Document();
            HandleNodeInserted handler = new HandleNodeInserted();
            srcDoc.NodeChangingCallback = handler;

            Document dstDoc = srcDoc.Clone();

            Assert.That(handler.EventCount, Is.EqualTo(0));
            srcDoc.AppendChild(new Section(srcDoc));
            Assert.That(handler.EventCount, Is.EqualTo(1));

            dstDoc.AppendChild(new Section(dstDoc));
            //Only original node should have raised the event, but not the copied node.
            Assert.That(handler.EventCount, Is.EqualTo(1));
        }

        private class HandleNodeInserted : INodeChangingCallback
        {
            void INodeChangingCallback.NodeInserting(NodeChangingArgs e)
            {
                // Do nothing.
            }

            void INodeChangingCallback.NodeInserted(NodeChangingArgs e)
            {
                ++EventCount;
            }

            void INodeChangingCallback.NodeRemoving(NodeChangingArgs e)
            {
                // Do nothing.
            }

            void INodeChangingCallback.NodeRemoved(NodeChangingArgs e)
            {
                // Do nothing.
            }

            public int EventCount { get; private set; }
        }

        /// <summary>
        /// WORDSNET-612 Throws "Attempted to copy a by reference attribute '16'.
        /// </summary>
        [Test]
        public void TestCloneAfterSave()
        {
            Document doc = new Document();
            MemoryStream stream = new MemoryStream();
            doc.Save(stream, SaveFormat.Docx);   // FOSS: was SaveFormat.Doc; format is incidental to the clone-after-save check.

            //This used to throw because writing injected attributes that were not possible to clone.
            Document newDoc = doc.Clone();
            Assert.That(newDoc, IsNot.Null());
        }

        /// <summary>
        /// Make sure the stream is not closed after writing the document.
        /// </summary>
        [Test]
        public void TestStreamNotClosed()
        {
            // FOSS: only OOXML, Text and Markdown writers survive — Doc/Rtf/Odt/Html/WordML/Mhtml were removed.
            DoTestStreamNotClosed(SaveFormat.Text);
            DoTestStreamNotClosed(SaveFormat.Docx);
            DoTestStreamNotClosed(SaveFormat.Docm);
            DoTestStreamNotClosed(SaveFormat.Markdown);
        }

        private static void DoTestStreamNotClosed(SaveFormat saveFormat)
        {
            Document doc = new Document();
            MemoryStream stream = new MemoryStream();
            doc.Save(stream, saveFormat);

            // Make sure the position is at the end of the stream.
            Assert.That(stream.Length, Is.EqualTo(stream.Position));

            // Make sure the stream is not closed by trying to move by the stream.
            stream.Position = 0;
            Assert.That(stream.Position, Is.EqualTo(0L));
        }

        [Test]
        public void TestBlank()
        {
            Document doc = new Document();
            Assert.That(doc.GetText(), Is.EqualTo("\x000c"));
            TestUtil.Save(doc, @"Model\Document\TestBlank.docx");   // FOSS: was .doc.
        }

        /// <summary>
        /// A customer reported that saving and opening via memory stream does not work.
        /// </summary>
        [Test]
        public void TestOpenSaveByteArray()
        {
            Document srcDoc = new Document();

            MemoryStream streamOut = new MemoryStream();
            srcDoc.Save(streamOut, SaveFormat.Docx);   // FOSS: was SaveFormat.Doc.

            byte[] docBytes = streamOut.ToArray();
            MemoryStream streamIn = new MemoryStream(docBytes);

            Document dstDoc = new Document(streamIn);

            Assert.That(dstDoc.GetText(), Is.EqualTo("\x000c"));
        }


        [Test]
        public void TestSaveWithoutExtensionWithOptions()
        {
            string fileNameA = TestUtil.GetInTestOutPath(@"Model\Document\TestSaveWithoutExtensionA Out");
            string fileNameB = TestUtil.GetInTestOutPath(@"Model\Document\TestSaveWithoutExtensionB Out.");

            File.Delete(fileNameA);
            File.Delete(fileNameB);

            Document doc = new Document();
            doc.Save(fileNameA, new OoxmlSaveOptions());
            doc.Save(fileNameB, new OoxmlSaveOptions());

            Assert.That(File.Exists(fileNameA), Is.True);
            Assert.That(File.Exists(fileNameB), Is.True);
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "Invalid save format requested.")]
        public void TestSaveWithoutExtensionWithoutOptions()
        {
            Document doc = new Document();
            doc.Save(TestUtil.GetInTestOutPath(@"Model\Document\TestSaveWithoutExtensionAuto Out"));
        }

        // FOSS: TestNoLastSectionBreak was removed. IsIgnoreLastSectionBreak is legacy
        // binary-.doc/.rtf behavior; both inputs are removed formats with no OOXML analogue.

        [Test]
        [CodePorting.Translator.Cs2Cpp.CppSkipEntity("C++ doesn't support Pdf2Word")]
        [JavaDelete("WORDSJAVA-2366: Autoportable Pdf2Word for Java.")]
        public void TestFormatPdf()
        {
            using (new GlobalLock("TestDetectFileFormat.pdf"))
            {
                // FOSS: PDF loading (Pdf2Word) was removed, so loading a PDF now throws.
                Assert.That(() => TestUtil.Open(@"Model\Document\TestDetectFileFormat.pdf"),
                    Throws.TypeOf<NotSupportedException>());
            }
        }






        /// <summary>
        /// WORDSNET-16234 FontInfoCollection.EmbedSystemFonts default value is incorrect.
        /// The problem is resource file blank.doc has this property set to true.
        /// Should reset it to false after blank.doc is loaded upon creating new document.
        /// </summary>
        [Test]
        public void TestJira16234()
        {
            Document doc = new Document();

            Assert.That(doc.FontInfos.EmbedSystemFonts, Is.False);
            Assert.That(doc.DocPr.EmbedSystemFonts, Is.False);
        }

        /// <summary>
        /// WORDSNET-13875 Spell check disabled after document save.
        /// Checks that appropriate DocPr members are set when public properties are used.
        /// </summary>
        [Test]
        public void Test13875()
        {
            Document doc = new Document();

            // Checking ShowGrammaticalErrors.
            doc.DocPr.ProofStateGrammar = Settings.ProofState.Dirty;
            Assert.That(doc.GrammarChecked, Is.True);
            doc.GrammarChecked = false;
            Assert.That(Settings.ProofState.None, Is.EqualTo(doc.DocPr.ProofStateGrammar));

            // Checking ShowSpellingErrors.
            doc.DocPr.ProofStateSpelling = Settings.ProofState.Dirty;
            Assert.That(doc.SpellingChecked, Is.True);
            doc.SpellingChecked = false;
            Assert.That(Settings.ProofState.None, Is.EqualTo(doc.DocPr.ProofStateSpelling));
        }



        // FOSS: Test28870 was removed. CompObj is a binary-.doc OLE structure and the test
        // both loads and saves .doc, all removed in FOSS.
    }
}
