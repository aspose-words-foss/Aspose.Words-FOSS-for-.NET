// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aspose.Words.Drawing;
using Aspose.Words.Saving;
using Aspose.Words.Vba;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for macros in documents.
    /// </summary>
    [TestFixture]
    public class TestVba : UnifiedTestsBase
    {
        /// <summary>
        /// This is needed because we have to deal with DOCM files instead of DOCX.
        /// </summary>
        private static UnifiedScenario GetDocxMacroAdjustedScenario(LoadFormat lf, SaveFormat sf)
        {
            return TestUtil.GetUnifiedScenario(
                (lf == LoadFormat.Docx) ? LoadFormat.Docm : lf,
                (sf == SaveFormat.Docx) ? SaveFormat.Docm : sf);
        }



        /// <summary>
        /// Performs a check for a digital signature to be preserved.
        /// </summary>
        private static void TestDigitalSignatureCore(
            LoadFormat lf,
            SaveFormat sf,
            string shortFileNameWithoutExtension,
            int sigLength,
            int sig0,
            int sig1,
            int sig8,
            int sig9)
        {
            UnifiedScenario scenario = GetDocxMacroAdjustedScenario(lf, sf);

            Document doc = TestUtil.OpenSaveOpen(shortFileNameWithoutExtension, scenario);

            // RK This switch is needed because RTF format does not have VBA.
            switch (scenario)
            {
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Doc2Docm:
                case UnifiedScenario.Doc2Wml:
                case UnifiedScenario.Docm2DocmNoGold:
                case UnifiedScenario.Wml2WmlNoGold:
                    Assert.That(doc.VbaProject, IsNot.Null());
                    byte[] vbaSignature = doc.VbaProject.Signature;
                    Assert.That(vbaSignature, IsNot.Null());
                    // This is the length of the signature with the 8 byte header.
                    Assert.That(vbaSignature.Length, Is.EqualTo(sigLength));
                    // Start of the header.
                    Assert.That(vbaSignature[0], Is.EqualTo(sig0));
                    Assert.That(vbaSignature[1], Is.EqualTo(sig1));
                    // Start of the signature.
                    Assert.That(vbaSignature[8], Is.EqualTo(sig8));
                    Assert.That(vbaSignature[9], Is.EqualTo(sig9));
                    break;
                case UnifiedScenario.Doc2Rtf:
                case UnifiedScenario.Rtf2RtfNoGold:
                    Assert.That(doc.VbaProject, Is.Null);
                    break;
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }

        /// <summary>
        /// Checks how macro names displayed in the Macros window are preserved.
        /// Also checks the document events.
        /// Also checks how macros can be programmatically removed.
        ///
        /// WORDSNET-829 Document OnOpen event no longer fires when opened and saved.
        /// Really obscure and undocumented behavior from MS Word, cannot solve quickly.
        ///
        /// WORDSNET-9658 Macros are no longer visible in the Macros window.
        /// The problem was that vbaData.xml was not processed.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestMacroNames(LoadFormat lf, SaveFormat sf)
        {
            UnifiedScenario scenario = GetDocxMacroAdjustedScenario(lf, sf);

            Document doc = TestUtil.OpenSaveOpen(@"Model\Vba\TestMacroNames", scenario);

            // RK This switch is needed because RTF format does not have VBA.
            switch (scenario)
            {
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Doc2Docm:
                case UnifiedScenario.Doc2Wml:
                case UnifiedScenario.Docm2DocmNoGold:
                case UnifiedScenario.Wml2WmlNoGold:
                {
                    Assert.That(doc.VbaProject, IsNot.Null());
                    IList<string> vbaMacroNames = doc.VbaProject.MacroNames;
                    Assert.That(vbaMacroNames.Count, Is.EqualTo(1));
                    Assert.That(vbaMacroNames[0], Is.EqualTo("Project.Module1.MyTestMacro"));

                    Assert.That(doc.VbaDocumentEvents, Is.EqualTo(VbaDocumentEvents.Open | VbaDocumentEvents.XmlAfterInsert));
                    break;
                }
                case UnifiedScenario.Doc2Rtf:
                case UnifiedScenario.Rtf2RtfNoGold:
                {
                    Assert.That(doc.VbaProject, Is.Null);
                    Assert.That((int)doc.VbaDocumentEvents, Is.EqualTo(0));
                    break;
                }
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }

            doc.RemoveMacros();

            // When you open this document in MS Word, press Alt+F11 and check there is no vba project.
            // There is no Macros in this document now, it should be possible to save as DOCX, not DOCM.
            TestUtil.SaveOpen(doc, @"Model\Vba\TestRemoveVba", TestUtil.GetUnifiedScenario(lf, sf));

            // Check the macros and digital signature were removed.
            Assert.That(doc.HasMacros, Is.False);
            Assert.That(doc.VbaProject, Is.Null);
            Assert.That((int)doc.VbaDocumentEvents, Is.EqualTo(0));
        }


        private static void CheckAcd(Document doc, int acdIndex, FixedCommandIdentifier fci, object arg)
        {
            AllocatedCommand acd = (AllocatedCommand)doc.AllocatedCommands[acdIndex];
            Assert.That(acd.FciBasedOn, Is.EqualTo(fci));

            if (arg is byte[])
                ArrayUtil.CheckArraysEqual((byte[])arg, acd.ArgValue);
            else if (arg is string)
                Assert.That(Encoding.Unicode.GetString(acd.ArgValue), Is.EqualTo(arg));
        }

        /// <summary>
        /// Tests how keymap customizations work.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestKeyMap(LoadFormat lf, SaveFormat sf)
        {
            UnifiedScenario scenario = GetDocxMacroAdjustedScenario(lf, sf);

            Document doc = TestUtil.OpenSaveOpen(@"Model\Vba\TestKeyMap", scenario);

            // RK This switch is needed because RTF format does not have customizations.
            switch (scenario)
            {
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Doc2Docm:
                case UnifiedScenario.Doc2Wml:
                case UnifiedScenario.Docm2DocmNoGold:
                case UnifiedScenario.Wml2WmlNoGold:
                {
                    Assert.That(doc.AttachedToolbars, Is.EqualTo(null));

                    Assert.That(doc.KeyMaps.Count, Is.EqualTo(6));
                    CheckKeyMap(doc, 0, KeyMapType.Mask, 0x0241, 0, 0, 0, 0, null);
                    CheckKeyMap(doc, 1, KeyMapType.AllocatedCommand, 0x0242, 0, 0, 0, 0, null);
                    CheckKeyMap(doc, 2, KeyMapType.AllocatedCommand, 0x0248, 0x0031, 1, 0, 0, null);
                    CheckKeyMap(doc, 3, KeyMapType.Macro, 0x024d, 0, 0, 0, 0, "PROJECT.NEWMACROS.MACRO1");
                    CheckKeyMap(doc, 4, KeyMapType.InsertCharacter, 0x0254, 0x004d, 0, 0x2122, 0, null);
                    CheckKeyMap(doc, 5, KeyMapType.FixedCommand, 0x0341, 0, 0, 0, 0x00ed, null);

                    // Check acds
                    Assert.That(doc.AllocatedCommands.Count, Is.EqualTo(2));
                    CheckAcd(doc, 0, FixedCommandIdentifier.ApplyAutoTextName, "Best regards,");
                    CheckAcd(doc, 1, FixedCommandIdentifier.ApplyStyleName, new byte[] { 1, 0, 0, 0, 1, 0 }); // Heading 1
                    break;
                }
                case UnifiedScenario.Doc2Rtf:
                case UnifiedScenario.Rtf2RtfNoGold:
                {
                    // Check RTF has nothing of that.
                    Assert.That(doc.AttachedToolbars, Is.Null);
                    Assert.That(doc.AllocatedCommands, Is.Null);
                    Assert.That(doc.VbaProject, Is.Null);
                    break;
                }
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }

        /// <summary>
        /// Verifies ability to modify macro source code.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestModifyMacroSourceCode(LoadFormat lf, SaveFormat sf)
        {
            const string newSourceCode = "Test change source code";

            if ((lf == LoadFormat.Rtf) || (sf == SaveFormat.Rtf))
                return;

            LoadFormat loadFormat = (lf == LoadFormat.Docx) ? LoadFormat.Docm : lf;

            Document doc = TestUtil.Open(@"Model\Vba\TestMacroNames", loadFormat);
            VbaProject project = doc.VbaProject;

            Assert.That(project.Modules.Count, Is.EqualTo(2));
            Assert.That(project.Modules[0].Name, Is.EqualTo("ThisDocument"));
            Assert.That(project.Modules[1].Name, Is.EqualTo("Module1"));

            // Verify initial VbaProject.
            string m0SourceCode = project.Modules[0].SourceCode;
            string m1SourceCode = project.Modules[1].SourceCode;
            Assert.That(m0SourceCode.Contains("Attribute VB_GlobalNameSpace"), Is.True);
            Assert.That(m1SourceCode.Contains("Hello from normal macro!"), Is.True);

            // Set a new source code.
            project.Modules[1].SourceCode = newSourceCode;

            UnifiedScenario scenario = GetDocxMacroAdjustedScenario(lf, sf) | UnifiedScenario.NoGold;
            doc = TestUtil.SaveOpen(doc, @"Model\Vba\TestModifyMacro", scenario);
            project = doc.VbaProject;

            // Verify modified source code was changed, whereas other project parts remains the same.
            Assert.That(project.Modules.Count, Is.EqualTo(2));
            Assert.That(project.Modules[0].Name, Is.EqualTo("ThisDocument"));
            Assert.That(project.Modules[1].Name, Is.EqualTo("Module1"));
            Assert.That(project.Modules[0].SourceCode, Is.EqualTo(m0SourceCode));

            Assert.That(project.Modules[1].SourceCode, Is.EqualTo(newSourceCode));
        }

        private static void CheckKeyMap(
            Document doc,
            int keyMapIndex,
            KeyMapType type,
            int kcmPrimary,
            int kcmSecondary,
            int acdIndex,
            int characterCode,
            int fixedCommandIdentifier,
            string macroName)
        {
            KeyMap keyMap = (KeyMap)doc.KeyMaps[keyMapIndex];
            Assert.That(keyMap.KeymapType, Is.EqualTo(type));
            Assert.That(keyMap.KcmPrimary, Is.EqualTo(kcmPrimary));
            Assert.That(keyMap.KcmSecondary, Is.EqualTo(kcmSecondary));
            Assert.That(keyMap.AllocatedCommandIndex, Is.EqualTo(acdIndex));
            Assert.That(keyMap.CharacterCode, Is.EqualTo(characterCode));
            Assert.That((int)keyMap.FixedCommandIdentifier, Is.EqualTo(fixedCommandIdentifier));
            Assert.That(keyMap.MacroName, Is.EqualTo(macroName));
        }

        /// <summary>
        /// WORDSNET-19530 NullReferenceException occurs during saving document to DOC/DOCX
        /// wne:argValue is not specified in the DOCX file and leaves the value null and then that crashes on write to DOC or DOCX.
        /// Fixed code to leave empty byte array instead of null.
        /// </summary>
        [Test]
        public void TestAllocatedCommandCtor()
        {
            AllocatedCommand cmd = new AllocatedCommand();
            Assert.That(cmd.ArgValue.Length, Is.EqualTo(0));    // This must not be null.
        }

        /// <summary>
        /// WORDSNET-3714 Consider adding an ability to read macros from documents.
        /// Added basic VBA project reader.
        /// </summary>
        [Test]
        public void TestVbaProject()
        {
            Document doc = TestUtil.Open(@"Model\Vba\Test3714.docm");

            VbaProject project = doc.VbaProject;

            Assert.That(project.Modules.Count, Is.EqualTo(3));

            Assert.That(project.Modules[0].Name, Is.EqualTo("ThisDocument"));
            Assert.That(project.Modules[1].Name, Is.EqualTo("NewMacros"));
            Assert.That(project.Modules[2].Name, Is.EqualTo("Module1"));

            Assert.That(project.Modules[0].SourceCode.Contains("Attribute VB_GlobalNameSpace"), Is.True);
            Assert.That(project.Modules[1].SourceCode.Contains("Selection.HomeKey Unit:=wdLine, Extend:=wdExtend"), Is.True);
            Assert.That(project.Modules[2].SourceCode.Contains("MacroInModule11 Macro"), Is.True);
        }

        /// <summary>
        /// [MS-OVBA] 3.2.1 No Compression Example.
        /// </summary>
        [Test]
        public void TestNoCompressionDecompression()
        {
            const string inputText = "abcdefghijklmnopqrstuv.";
            const string expectedCompressedOutput =
            "01 19 B0 00 61 62 63 64 65 66 67 68 00 69 6A 6B 6C 6D 6E 6F 70 00 71 72 73 74 75 76 2E";
            const string expectedDecompressedOutput =
            "61 62 63 64 65 66 67 68 69 6A 6B 6C 6D 6E 6F 70 71 72 73 74 75 76 2E";

            TestCompressionDecompression(inputText, expectedCompressedOutput, expectedDecompressedOutput);
        }

        /// <summary>
        /// [MS-OVBA] 3.2.2 Normal Compression Example.
        /// </summary>
        [Test]
        public void TestNormalCompressionDecompression()
        {
            const string inputText = "#aaabcdefaaaaghijaaaaaklaaamnopqaaaaaaaaaaaarstuvwxyzaaa";
            const string expectedCompressedOutput =
            "01 2F B0 00 23 61 61 61 62 63 64 65 82 66 00 70 61 67 68 69 6A 01 38 08 61 6B 6C 00 20 6D 6E 6F" +
            "70 06 71 02 70 04 00 72 73 74 75 76 10 77 78 79 7A 00 2C";
            const string expectedDecompressedOutput =
            "23 61 61 61 62 63 64 65 66 61 61 61 61 67 68 69 6A 61 61 61 61 61 6B 6C 61 61 61 6D 6E 6F 70 71" +
            "61 61 61 61 61 61 61 61 61 61 61 61 72 73 74 75 76 77 78 79 7A 61 61 61";

            TestCompressionDecompression(inputText, expectedCompressedOutput, expectedDecompressedOutput);
        }

        /// <summary>
        /// [MS-OVBA] 3.2.3 Maximum Compression Example.
        /// </summary>
        [Test]
        public void TestMaximumCompressionDecompression()
        {
            const string inputText = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            const string expectedCompressedOutput = "01 03 B0 02 61 45 00";
            const string expectedDecompressedOutput =
            "61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 " +
            "61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 61 " +
            "61 61 61 61 61 61 61 61 61";

            TestCompressionDecompression(inputText, expectedCompressedOutput, expectedDecompressedOutput);
        }

        /// <summary>
        ///  WORDSNET-19153 Improve a VBAProject class to be able to read all project content
        ///  into the document model.
        /// </summary>
        [Test]
        public void Test19153()
        {
            string testName = @"Model\Vba\Test3714";
            Document doc = TestUtil.Open(testName+ ".docm");

            VerifyVba3714(doc);

            doc.VbaProject.MarkModified();
            doc = TestUtil.SaveOpen(doc, testName,
                TestUtil.GetUnifiedScenario(LoadFormat.Docm, SaveFormat.Docm) | UnifiedScenario.NoGold);

            VerifyVba3714(doc);
        }

        /// <summary>
        /// Verifies creation, export and import of a new empty VbaProject.
        /// </summary>
        [Test]
        public void Test19153_CreateNewEmptyProject()
        {
            Document doc = new Document();

            VbaProject project = new VbaProject();
            doc.VbaProject = project;

            VerifyNewEmptyProject(doc);

            string testName = @"Model\Vba\TestEmptyNewProject";
            doc = TestUtil.SaveOpen(doc, testName,
                TestUtil.GetUnifiedScenario(LoadFormat.Docm, SaveFormat.Docm) | UnifiedScenario.NoGold);

            VerifyNewEmptyProject(doc);
        }

        /// <summary>
        /// Verifies creation, export and import of a new VbaProject with adding a model.
        /// </summary>
        [Test]
        public void Test19153_CreateNewProject()
        {
            Document doc = new Document();

            VbaProject project = new VbaProject();
            project.Name = "AsposeProject";
            doc.VbaProject = project;

            VbaModule module = new VbaModule("AsposeModule");
            module.SourceCode = "New source code";

            doc.VbaProject.Modules.Add(module);

            VerifyCreatedNewProject(doc);

            string testName = @"Model\Vba\TestNewProject";
            doc = TestUtil.SaveOpen(doc, testName,
                TestUtil.GetUnifiedScenario(LoadFormat.Docm, SaveFormat.Docm) | UnifiedScenario.NoGold);

            VerifyCreatedNewProject(doc);
        }

        /// <summary>
        /// Verifies remove module from the VbaModuleCollection.
        /// </summary>
        [Test]
        public void Test19153_ModuleCollectionRemove()
        {
            Document doc = new Document();

            VbaProject project = new VbaProject();
            project.Name = "AsposeProject";
            doc.VbaProject = project;

            VbaModule module = new VbaModule("AsposeModule");

            doc.VbaProject.Modules.Add(module);
            Assert.That(project.Modules.Count, Is.EqualTo(2));
            Assert.That(project.Modules["AsposeModule"], IsNot.Null());

            doc.VbaProject.Modules.Remove(module);
            Assert.That(project.Modules.Count, Is.EqualTo(1));
            Assert.That(project.Modules["AsposeModule"], Is.Null);
        }

        private void VerifyNewEmptyProject(Document doc)
        {
            VbaProject p = doc.VbaProject;
            Assert.That(p.Name, Is.EqualTo("Project"));

            Assert.That(p.CodePage, Is.EqualTo(1251));
            Assert.That(p.SysKind, Is.EqualTo(1));

            Assert.That(p.Lcid, Is.EqualTo(0x00000409));
            Assert.That(p.LcidInvoke, Is.EqualTo(0x00000409));

            Assert.That(p.Description, Is.EqualTo(string.Empty));

            Assert.That(p.VersionMajor, Is.EqualTo(1));
            Assert.That(p.VersionMinor, Is.EqualTo(1));

            VbaModuleCollection modules = p.Modules;
            Assert.That(modules.Count, Is.EqualTo(1));

            VbaModule module = modules[0];

            Assert.That(module.Name, Is.EqualTo("ThisDocument"));
            Assert.That(module.Type, Is.EqualTo(VbaModuleType.DocumentModule));
            Assert.That(module.HelpContext, Is.EqualTo(0));
            Assert.That(module.DocString, Is.EqualTo(string.Empty));
        }

        private void VerifyCreatedNewProject(Document doc)
        {
            VbaProject p = doc.VbaProject;
            Assert.That(p.Name, Is.EqualTo("AsposeProject"));

            Assert.That(p.CodePage, Is.EqualTo(1251));
            Assert.That(p.SysKind, Is.EqualTo(1));

            Assert.That(p.Lcid, Is.EqualTo(0x00000409));
            Assert.That(p.LcidInvoke, Is.EqualTo(0x00000409));

            Assert.That(p.Description, Is.EqualTo(string.Empty));

            Assert.That(p.VersionMajor, Is.EqualTo(1));
            Assert.That(p.VersionMinor, Is.EqualTo(1));

            VbaModuleCollection modules = p.Modules;
            Assert.That(modules.Count, Is.EqualTo(2));

            VbaModule module = modules[0];

            Assert.That(module.Name, Is.EqualTo("ThisDocument"));
            Assert.That(module.Type, Is.EqualTo(VbaModuleType.DocumentModule));
            Assert.That(module.HelpContext, Is.EqualTo(0));
            Assert.That(module.DocString, Is.EqualTo(string.Empty));

            module = modules[1];
            Assert.That(module.Name, Is.EqualTo("AsposeModule"));
            Assert.That(module.SourceCode, Is.EqualTo("New source code"));
            Assert.That(module.Type, Is.EqualTo(VbaModuleType.ProceduralModule));
            Assert.That(module.HelpContext, Is.EqualTo(0));
            Assert.That(module.DocString, Is.EqualTo(string.Empty));
        }


        /// <summary>
        /// WORDSNET-19556 Added a feature to clone VbaPorject.
        /// </summary>
        [Test]
        public void Test19556()
        {
            Document sourceDoc = TestUtil.Open(@"Model\Vba\Test3714.docm");

            VbaProject sourceProject = sourceDoc.VbaProject;

            Document destDoc = new Document();
            VbaProject destProject = sourceProject.Clone();
            destDoc.VbaProject = destProject;

            Assert.That(ReferenceEquals(sourceProject, destProject), Is.False);
            Assert.That(ReferenceEquals(sourceProject.Modules, destProject.Modules), Is.False);
            Assert.That(ReferenceEquals(sourceProject.References, destProject.References), Is.False);

            destDoc = TestUtil.SaveOpen(destDoc, @"Model\Vba\Test19556",
                TestUtil.GetUnifiedScenario(LoadFormat.Docm, SaveFormat.Docm) | UnifiedScenario.NoGold);

            VerifyVba3714(destDoc);
        }

        /// <summary>
        /// WORDSNET-15697 VBAProject password protection was lost after changes in this project.
        /// We must preserve several project parameters in a stream "PROJECT" to preserve password protection.
        /// </summary>
        [Test]
        public void Test15697()
        {
            Document doc = TestUtil.Open(@"Model\Vba\Test15697.docm");

            VbaModule module = doc.VbaProject.Modules[1];
            module.SourceCode = module.SourceCode.Replace("Hello", "Test15697");

            doc.VbaProject.MarkModified();

            doc = TestUtil.SaveOpen(doc, @"Model\Vba\Test15697", TestUtil.GetUnifiedScenario(LoadFormat.Docm, SaveFormat.Docm));

            VbaProject project = doc.VbaProject;

            // Get a "PROJECT" stream as a text to verify the needed parameters.
            MemoryStream stream = project.Storage.FetchStream(VbaRecord.ProjectStreamName);
            string stringProj = Encoding.GetEncoding(project.CodePage).GetString(stream.ToArray());

            // To preserve password protection these four parameters must be preserved:
            Assert.That(stringProj.Contains("ID=\"{00000000-0000-0000-0000-000000000000}\""), Is.True);
            Assert.That(stringProj.Contains("CMG=\"848628B6283E2C3E2C3A303A30\""), Is.True);
            Assert.That(stringProj.Contains("DPB=\"080AA4ABC1ABC1543FACC1E396A1EBDAC59C863691AD483DB71A1082E7C25048C7C73E2E\""), Is.True);
            Assert.That(stringProj.Contains("GC=\"8C8E20BE204321432143\""), Is.True);

            // Lets check modified source code.
            Assert.That(project.Modules[1].SourceCode.Contains("Test15697"), Is.True);
        }

        /// <summary>
        /// WORDSNET-21071 Cannot access to indexer for VbaReferenceCollection.
        /// The indexer should be exposed publicly.
        /// </summary>
        [Test]
        [CodePorting.Translator.Cs2Cpp.CppSkipEntity("Test doesn't make sense in C++")]
        [JavaAttributes.JavaDelete("Strange test. No need in Java.")]
        public void Test21071()
        {
            Assert.That(typeof(VbaReferenceCollection).GetProperty("Item"), IsNot.Null());
        }




        /// <summary>
        /// WORDSNET-12444 Support an API to check if VBAcode is signed.
        /// <see cref="VbaProject.IsSigned"/> was implemented.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestVbaProjectIsSigned(LoadFormat lf, SaveFormat sf)
        {
            string testName = @"Model\Vba\Test12444";
            if ((lf == LoadFormat.Rtf) || (sf == SaveFormat.Rtf))
                return;
            LoadFormat loadFormat = (lf == LoadFormat.Docx) ? LoadFormat.Docm : lf;
            SaveFormat saveFormat = (sf == SaveFormat.Docx) ? SaveFormat.Docm : sf;

            Document doc = TestUtil.Open(testName, loadFormat);

            // Load signed document, verify project is signed.
            VbaProject project = doc.VbaProject;
            Assert.That(project.IsSigned, Is.True);

            // Modify project property and verify project is not signed anymore.
            project.Name = "New Name";
            Assert.That(project.IsSigned, Is.False);

            doc = TestUtil.SaveOpen(doc, testName, TestUtil.GetUnifiedScenario(loadFormat, saveFormat) | UnifiedScenario.NoGold);
            Assert.That(doc.VbaProject.IsSigned, Is.False);
        }

        /// <summary>
        /// WORDSNET-25332 Accents are lost when VBA source code is written to document.
        /// Added setter for VbaProject.CodePage property.
        /// </summary>
        [TestCase("ãç", 860)]
        [TestCase("фй", 1251)]
        public void Test25332(string text, int codePage)
        {
            string sourceCode= string.Format(
                "Private Sub Test()\r\n    Dim test As String\r\n    Set test = \"{0}\"\r\nEnd Sub", text);

            Document doc = new Document();
            doc.VbaProject = new VbaProject();

            // Explicitly set Portuguese code page.
            doc.VbaProject.CodePage = codePage;

            VbaModule module =  new VbaModule();
            module.Name = "Test";
            doc.VbaProject.Modules.Add(module);
            module.SourceCode = sourceCode;

            doc = TestUtil.SaveOpen(doc, SaveFormat.Docm);

            Assert.That(doc.VbaProject.Modules["Test"].SourceCode, Is.EqualTo(sourceCode));
        }

        /// <summary>
        /// #WORDSNET-26750 Investigate whether it is possible to check if the document has macros without loading document into the model.
        /// Added macros presence detection to FileFormatUtil class.
        /// </summary>
        [TestCase(@"Model\Vba\Test12444.doc", true)]
        [TestCase(@"Model\Vba\Test3714.docm", true)]
        [TestCase(@"ImportDocx\Test18712.docx", false)]
        public void Test26750(string fileName, bool hasMacros)
        {
            FileFormatInfo fi = FileFormatUtil.DetectFileFormat(TestUtil.BuildTestFileName(fileName));
            Assert.That(fi.HasMacros, Is.EqualTo(hasMacros));
        }

        /// <summary>
        /// WORDSNET-26289 Consider providing VbaProject.IsProtected property.
        /// Added public property VbaProject.IsProtected.
        /// </summary>
        [TestCase(@"Model\Vba\Test15697.docm", true)]
        public void Test26289(string fileName, bool isProtected)
        {
            Document doc = TestUtil.Open(fileName);
            Assert.That(doc.VbaProject.IsProtected, Is.EqualTo(isProtected));
        }


        /// <summary>
        /// Compression and decompression of [MS-OVBA] examples.
        /// Core test for WORDSNET-18697
        /// </summary>
        private static void TestCompressionDecompression(string inputText, string expectedCompressedOutput, string expectedDecompressedOutput)
        {
            // Compression.
            MemoryStream inputStream = new MemoryStream(Encoding.ASCII.GetBytes(inputText));
            MemoryStream outputStream = new MemoryStream();

            RleCompressor.Compress(inputStream, outputStream);

            Assert.That(StringUtil.BytesToHex(outputStream.ToArray()), Is.EqualTo(expectedCompressedOutput.Replace(" ", "")));

            // Decompression.
            outputStream.Position = 0;
            MemoryStream decompressedStream = new MemoryStream();

            RleCompressor.Decompress(outputStream, decompressedStream);

            Assert.That(StringUtil.BytesToHex(decompressedStream.ToArray()), Is.EqualTo(expectedDecompressedOutput.Replace(" ", "")));
            Assert.That(Encoding.ASCII.GetString(decompressedStream.ToArray()), Is.EqualTo(inputText));
        }

        private static void VerifyVba3714(Document doc)
        {
            VbaProject p = doc.VbaProject;
            Assert.That(p.Name, Is.EqualTo("MyProject"));
            Assert.That(p.CodePage, Is.EqualTo(1251));
            Assert.That(p.SysKind, Is.EqualTo(1));

            Assert.That(p.Lcid, Is.EqualTo(1033));
            Assert.That(p.LcidInvoke, Is.EqualTo(1033));

            Assert.That(p.Description, Is.EqualTo("Описание проекта"));
            Assert.That(p.HelpFilePath1, Is.EqualTo(string.Empty));
            Assert.That(p.HelpFilePath2, Is.EqualTo(string.Empty));

            Assert.That(p.HelpContext, Is.EqualTo(666));
            Assert.That(p.LibFlags, Is.EqualTo(0));
            Assert.That(p.VersionMajor, Is.EqualTo(1588943535));
            Assert.That(p.VersionMinor, Is.EqualTo(6));

            Assert.That(p.Constants, Is.EqualTo(string.Empty));

            VbaReferenceCollection references = p.References;
            Assert.That(references.Count, Is.EqualTo(3));

            Assert.That(references[0].Type, Is.EqualTo(VbaReferenceType.Registered));
            Assert.That(references[0].Name, Is.EqualTo("stdole"));
            Assert.That(((VbaReferenceRegistered)references[0]).LibId, Is.EqualTo("*\\G{00020430-0000-0000-C000-000000000046}#2.0#0#C:\\Windows\\SysWOW64\\"+
                "stdole2.tlb#OLE Automation"));

            Assert.That(references[1].Type, Is.EqualTo(VbaReferenceType.Project));
            Assert.That(references[1].Name, Is.EqualTo("Normal"));
            VbaReferenceProject referenceProject = (VbaReferenceProject)references[1];
            Assert.That(referenceProject.LibIdAbsolute, Is.EqualTo("*\\CNormal"));
            Assert.That(referenceProject.LibIdRelative, Is.EqualTo("*\\CNormal"));
            Assert.That(referenceProject.MajorVersion, Is.EqualTo(1588889804));
            Assert.That(referenceProject.MinorVersion, Is.EqualTo(13));

            Assert.That(references[2].Name, Is.EqualTo("Office"));
            Assert.That(references[2].Type, Is.EqualTo(VbaReferenceType.Registered));
            Assert.That(((VbaReferenceRegistered)references[2]).LibId, Is.EqualTo(@"*\G{2DF8D04C-5BFA-101B-BDE5-00AA0044DE52}#2.0#0#C:\Program Files (x86)\Common "+
                            @"Files\Microsoft Shared\OFFICE15\MSO.DLL#Microsoft Office 15.0 Object Library"));

            VbaModuleCollection modules = p.Modules;
            Assert.That(modules.Count, Is.EqualTo(3));

            VbaModule module = modules[0];
            Assert.That(module.Name, Is.EqualTo("ThisDocument"));
            Assert.That(module.SourceCode, Is.EqualTo("Attribute VB_Name = \"ThisDocument\"\r\n" +
                "Attribute VB_Base = \"1Normal.ThisDocument\"\r\n" +
                "Attribute VB_GlobalNameSpace = False\r\n" +
                "Attribute VB_Creatable = False\r\n" +
                "Attribute VB_PredeclaredId = True\r\n" +
                "Attribute VB_Exposed = True\r\n" +
                "Attribute VB_TemplateDerived = True\r\n" +
                "Attribute VB_Customizable = True\r\n"));
            Assert.That(module.Type, Is.EqualTo(VbaModuleType.DocumentModule));
            Assert.That(module.HelpContext, Is.EqualTo(0));
            Assert.That(module.DocString, Is.EqualTo(string.Empty));

            module = modules[1];
            Assert.That(module.Name, Is.EqualTo("NewMacros"));
            Assert.That(module.SourceCode, Is.EqualTo("Attribute VB_Name = \"NewMacros\"\r\n" +
                "Sub Macro1()\r\n'\r\n" +
                "' Macro1 Macro\r\n" +
                "    Selection.TypeText Text:=\"test\"\r\n" +
                "    Selection.HomeKey Unit:=wdLine, Extend:=wdExtend\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.Underline = wdUnderlineSingle\r\n\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n\r\n" +
                "    Selection.TypeText Text:=\"test\"\r\n" +
                "    Selection.HomeKey Unit:=wdLine, Extend:=wdExtend\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.Underline = wdUnderlineSingle\r\n" +
                "    \r\n    \r\n" +
                "        Selection.TypeText Text:=\"test\"\r\n" +
                "    Selection.HomeKey Unit:=wdLine, Extend:=wdExtend\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.Underline = wdUnderlineSingle\r\n    \r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.Underline = wdUnderlineSingle\r\n\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n\r\n" +
                "    Selection.TypeText Text:=\"test\"\r\n" +
                "    Selection.HomeKey Unit:=wdLine, Extend:=wdExtend\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.Underline = wdUnderlineSingle\r\n    \r\n    \r\n" +
                "        Selection.TypeText Text:=\"test\"\r\n" +
                "    Selection.HomeKey Unit:=wdLine, Extend:=wdExtend\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n    \r\n" +
                "        Selection.TypeText Text:=\"test\"\r\n" +
                "    Selection.HomeKey Unit:=wdLine, Extend:=wdExtend\r\n" +
                "    Selection.Font.Underline = wdUnderlineSingle\r\n    \r\n" +
                "        Selection.TypeText Text:=\"test\"\r\n" +
                "    Selection.HomeKey Unit:=wdLine, Extend:=wdExtend\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.UnderlineColor = wdColorAutomatic\r\n" +
                "    Selection.Font.Underline = wdUnderlineSingle\r\n\r\nEnd Sub\r\n"));

            Assert.That(module.Type, Is.EqualTo(VbaModuleType.ProceduralModule));
            Assert.That(module.HelpContext, Is.EqualTo(0));
            Assert.That(module.DocString, Is.EqualTo(string.Empty));

            module = modules[2];
            Assert.That(module.Name, Is.EqualTo("Module1"));
            Assert.That(module.SourceCode, Is.EqualTo("Attribute VB_Name = \"Module1\"\r\n" +
                "Sub MacroInModule11()\r\n'\r\n" +
                "' MacroInModule11 Macro\r\n" +
                "    Selection.TypeText Text:=\"Module1\"\r\n\r\n" +
                "End Sub\r\n"));
            Assert.That(module.Type, Is.EqualTo(VbaModuleType.ProceduralModule));
            Assert.That(module.HelpContext, Is.EqualTo(0));
            Assert.That(module.DocString, Is.EqualTo(string.Empty));
        }
    }
}
