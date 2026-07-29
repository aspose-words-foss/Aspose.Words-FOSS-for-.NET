// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using Aspose.Words.Properties;
using Aspose.Words.Settings;
using Aspose.Words.Tests.Export.Docx;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for document protection.
    /// </summary>
    [TestFixture]
    public class TestProtection : UnifiedTestsBase
    {
        [Test]
        public void TestLegacyProtectionHashExample()
        {
            Assert.That(DocumentProtection.CalculateLegacyHash("Example"), Is.EqualTo(0x64ceed7e));
        }

        [Test]
        public void TestLegacyProtectionHashNull()
        {
            Assert.That(DocumentProtection.CalculateLegacyHash(null), Is.EqualTo(0));
        }

        [Test]
        public void TestLegacyProtectionHashEmpty()
        {
            Assert.That(DocumentProtection.CalculateLegacyHash(""), Is.EqualTo(0));
        }

        [Test]
        public void TestLegacyProtectionHashLong()
        {
            int hash1 = DocumentProtection.CalculateLegacyHash("This is a very long password.");
            int hash2 = DocumentProtection.CalculateLegacyHash("This is a very long password, but even longer than the previous one.");
            Assert.That(hash2, Is.EqualTo(hash1));
            Assert.That(hash1, Is.EqualTo(0x46ffeeff));
        }

        /// <summary>
        /// WORDSNET-9724 Document.Unprotect(String) doesn't remove protection from document.
        /// andrnosk: The problem occurs because CalculateDocumentProtectionHash method always uses default values for 
        /// algorithm, spin count and salt parameters, but in this case we have to use values specified inside 
        /// document to calculate correct hash.
        /// </summary>
        [Test]
        public void TestJira9724()
        {
            Document doc = TestUtil.Open(@"Model\Protection\TestJira9724.docx");
            Assert.That(ProtectionType.AllowOnlyFormFields, Is.EqualTo(doc.ProtectionType));
            doc.Unprotect("12345");
            Assert.That(ProtectionType.NoProtection, Is.EqualTo(doc.ProtectionType));
        }
        

        /// <summary>
        /// WORDSNET-7893 Consider adding a VerifiyPassword method.
        /// Implemented password validation for write-protected documents.
        /// Note, that Word 2013 stores hash with SHA-512 algorithm and other Word versions use SHA-1.
        /// </summary>
        [Test]
        public void TestJira7893()
        {
            // Check Word2013 write-protected document.
            Document doc = TestUtil.Open(@"Model\Protection\TestJira7893Word2013.docx");
            WriteProtection writeProtection = doc.WriteProtection;
            Assert.That(writeProtection.ValidatePassword("!Qaz2wsx"), Is.True);
            ValidateBadPasswords(writeProtection);

            // Check Word2010 write-protected document.
            doc = TestUtil.Open(@"Model\Protection\TestJira7893Word2010.docx");
            writeProtection = doc.WriteProtection;
            Assert.That(writeProtection.ValidatePassword("!Qaz2wsx"), Is.True);
            ValidateBadPasswords(writeProtection);

            // Check unprotected document.
            doc = new Document();
            ValidateBadPasswords(doc.WriteProtection);
        }



        /// <summary>
        /// The password is "test".
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCommentsOnly(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Protection\TestProtectionCommentsOnly", lf, sf);

            Assert.That(doc.ProtectionType, Is.EqualTo(ProtectionType.AllowOnlyComments));

            switch (TestUtil.GetUnifiedScenario(lf, sf))
            {
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Doc2Docx:
                case UnifiedScenario.Docx2DocxNoGold:
                    Assert.That(doc.BuiltInDocumentProperties.Security, Is.EqualTo(DocumentSecurity.ReadOnlyExceptAnnotations));
                    break;
                default:
                    Assert.That(doc.BuiltInDocumentProperties.Security, Is.EqualTo(DocumentSecurity.None));
                    break;
            }

            DocumentProtection prot = doc.DocPr.DocumentProtection;
            PasswordHash hash = prot.PasswordHash;
            switch (TestUtil.GetUnifiedScenario(lf, sf))
            {
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Wml2WmlNoGold:
                case UnifiedScenario.Doc2Wml:
                    // RK In these cases only "old" password hash exists.
                    Assert.That(prot.LegacyHash, Is.EqualTo(0x1FC6CBEB));
                    Assert.That(hash.Hash, Is.EqualTo(null));
                    Assert.That(hash.Salt, Is.EqualTo(null));
                    Assert.That(hash.CryptAlgorithmSid, Is.EqualTo(0));
                    Assert.That(hash.CryptSpinCount, Is.EqualTo(0));
                    break;
                case UnifiedScenario.Doc2Docx:
                case UnifiedScenario.Docx2DocxNoGold:
                case UnifiedScenario.Rtf2RtfNoGold:
                    // RK In these cases only "new" password hash exists.
                    Assert.That(prot.LegacyHash, Is.EqualTo(0));
                    Assert.That(hash.Hash.Length, Is.EqualTo(20));
                    Assert.That(hash.Salt.Length, Is.EqualTo(16));
                    Assert.That(hash.CryptAlgorithmSid, Is.EqualTo(4));
                    // "ms" files created using Word 2007 SP2 have a higher spin count value.
                    Assert.That(hash.CryptSpinCount, Is.EqualTo((lf == LoadFormat.Doc) ? 50000 : 100000));
                    break;
                case UnifiedScenario.Doc2Rtf:
                    // RK Our RTF save-open supports both old and new password hashes.
                    Assert.That(prot.LegacyHash, Is.EqualTo(0x1FC6CBEB));
                    Assert.That(hash.Hash.Length, Is.EqualTo(20));
                    break;
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestNoProtection(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Protection\TestProtectionNone", lf, sf);
            Assert.That(doc.ProtectionType, Is.EqualTo(ProtectionType.NoProtection));
        }

        /// <summary>
        /// No password on this document.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestProtectionFormFields(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Protection\TestProtectionFormFieldsOnly", lf, sf);
            Assert.That(doc.ProtectionType, Is.EqualTo(ProtectionType.AllowOnlyFormFields));
            Assert.That(doc.BuiltInDocumentProperties.Security, Is.EqualTo(DocumentSecurity.None));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestProtectGeneratePassword(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Protection\TestProtectionNone", lf, sf);

            // This is the "legacy" way of protecting with an unknown password because we didn't know the hash algorithm.
            doc.Protect(ProtectionType.AllowOnlyFormFields);

            doc = TestUtil.SaveOpen(doc, @"Model\Protection\TestProtectGeneratePassword", lf, sf);

            // Check the protection is now set with a password (albeit unknown).
            Assert.That(doc.ProtectionType, Is.EqualTo(ProtectionType.AllowOnlyFormFields));
            Assert.That(doc.BuiltInDocumentProperties.Security, Is.EqualTo(DocumentSecurity.None));

            DocumentProtection prot = doc.DocPr.DocumentProtection;
            PasswordHash hash = prot.PasswordHash;
            switch (TestUtil.GetUnifiedScenario(lf, sf))
            {
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Doc2Wml:
                case UnifiedScenario.Wml2WmlNoGold:
                    // RK When document is opened/saved in these formats, only "old" password hash exists.
                    Assert.That(prot.LegacyHash == 0x56AE3201, Is.True);
                    break;
                case UnifiedScenario.Doc2Docx:
                case UnifiedScenario.Doc2Rtf:
                case UnifiedScenario.Docx2DocxNoGold:
                case UnifiedScenario.Rtf2RtfNoGold:
                    // RK If the file is opened/save in these formats, only "new" password hash exists.
                    Assert.That(hash.Hash.Length, Is.EqualTo(20));
                    break;
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }

        /// <summary>
        /// Changing protection type should not change existing protection password.
        /// The password is "test".
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestProtectKeepPassword(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Protection\TestProtectionCommentsOnly", lf, sf);

            PasswordHash hashBefore = doc.DocPr.DocumentProtection.PasswordHash;
            
            doc.Protect(ProtectionType.AllowOnlyFormFields);

            doc = TestUtil.SaveOpen(doc, @"Model\Protection\TestProtectKeepPassword", lf, sf);
            
            Assert.That(doc.ProtectionType, Is.EqualTo(ProtectionType.AllowOnlyFormFields));
            Assert.That(doc.BuiltInDocumentProperties.Security, Is.EqualTo(DocumentSecurity.None));

            // Check the original password was kept.
            DocumentProtection prot = doc.DocPr.DocumentProtection;
            PasswordHash hash = prot.PasswordHash;
            switch (TestUtil.GetUnifiedScenario(lf, sf))
            {
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Wml2WmlNoGold:
                case UnifiedScenario.Doc2Wml:
                    // RK We have the legacy hash only, check it.
                    Assert.That(prot.LegacyHash, Is.EqualTo(0x1fc6cbeb));
                    Assert.That(hash.Hash, Is.EqualTo(null));
                    break;
                case UnifiedScenario.Doc2Docx:
                case UnifiedScenario.Doc2Rtf:
                case UnifiedScenario.Docx2DocxNoGold:
                case UnifiedScenario.Rtf2RtfNoGold:
                    // RK Check the ooxml hash.
                    Assert.That(hash.CryptAlgorithmSid, Is.EqualTo(4));
                    // "ms" files created using Word 2007 SP2 have a higher spin count value.
                    Assert.That(hash.CryptSpinCount, Is.EqualTo((lf == LoadFormat.Doc) ? 50000 : 100000));  
                    Assert.That(hash != hashBefore, Is.True);
                    ArrayUtil.CheckArraysEqual(hashBefore.Salt, hash.Salt);
                    ArrayUtil.CheckArraysEqual(hashBefore.Hash, hash.Hash);
                    break;
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }

        /// <summary>
        /// WORDSNET-4278 Support protection password export in RTF.
        /// WORDSNET-3613 Support proper protection password export in DOCX.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestProtectSetPassword(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            doc.Protect(ProtectionType.AllowOnlyFormFields, "test");
            doc = TestUtil.SaveOpen(doc, @"Model\Protection\TestProtectSetPassword", lf, sf);

            Assert.That(doc.ProtectionType, Is.EqualTo(ProtectionType.AllowOnlyFormFields));

            // Check the password was set.
            DocumentProtection prot = doc.DocPr.DocumentProtection;
            PasswordHash hash = prot.PasswordHash;
            switch (sf)
            {
                case SaveFormat.Doc:
                case SaveFormat.WordML:
                    // RK Check the legacy hash.
                    Assert.That(prot.LegacyHash, Is.EqualTo(0x1fc6cbeb));
                    Assert.That(hash.Hash, Is.EqualTo(null));
                    break;
                case SaveFormat.Docx:
                case SaveFormat.Rtf:
                    // RK Check the new hash.
                    Assert.That(hash.CryptAlgorithmSid, Is.EqualTo(4));
                    Assert.That(hash.CryptSpinCount, Is.EqualTo(50000));
                    Assert.That(Convert.ToBase64String(hash.Hash), Is.EqualTo("wZko1pqoSiQdGEuyf0evzPwm9AE="));
                    Assert.That(Convert.ToBase64String(hash.Salt), Is.EqualTo("hIqOl99kIr/+N0hyovEq7Q=="));
                    break;
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestProtectNoPassword(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            
            // Verify we allow empty string.
            doc.Protect(ProtectionType.AllowOnlyFormFields, "");
            Assert.That(doc.DocPr.DocumentProtection.LegacyHash, Is.EqualTo(0));
            Assert.That(doc.DocPr.DocumentProtection.PasswordHash.Hash, Is.EqualTo(null));

            // Verify we allow null.
            doc.Protect(ProtectionType.AllowOnlyFormFields, null);
            Assert.That(doc.DocPr.DocumentProtection.LegacyHash, Is.EqualTo(0));
            Assert.That(doc.DocPr.DocumentProtection.PasswordHash.Hash, Is.EqualTo(null));

            // Check after save open.
            doc = TestUtil.SaveOpen(doc, @"Model\Protection\TestProtectNoPassword", lf, sf);
            Assert.That(doc.DocPr.DocumentProtection.LegacyHash, Is.EqualTo(0));
            Assert.That(doc.DocPr.DocumentProtection.PasswordHash.Hash, Is.EqualTo(null));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestReadOnlyRecommended(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            Assert.That(doc.DocPr.WriteProtection.ReadOnlyRecommended, Is.EqualTo(false));

            doc = TestUtil.OpenSaveOpen(@"Model\Protection\TestReadOnlyRecommended", lf, sf);

            switch (TestUtil.GetUnifiedScenario(lf, sf))
            {
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Doc2Docx:
                case UnifiedScenario.Doc2Rtf:
                case UnifiedScenario.Docx2DocxNoGold:
                case UnifiedScenario.Rtf2RtfNoGold:
                    // RK For RTF although the property is documented and written, MS Word seems to ignore it.
                    Assert.That(doc.DocPr.WriteProtection.ReadOnlyRecommended, Is.EqualTo(true));
                    break;
                case UnifiedScenario.Doc2Wml:
                case UnifiedScenario.Wml2WmlNoGold:
                    // RK WordML does not have this property.
                    Assert.That(doc.DocPr.WriteProtection.ReadOnlyRecommended, Is.EqualTo(false));
                    break;
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }

        /// <summary>
        /// Also known as write protection password.
        /// The password is "test".
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSharingPassword(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Protection\TestSharingPassword", lf, sf);

            WriteProtection prot = doc.DocPr.WriteProtection;
            switch (TestUtil.GetUnifiedScenario(lf, sf))
            {
                case UnifiedScenario.Doc2Doc:
                    // RK This password is actually stored in DOC.
                    Assert.That(prot.GetPassword(), Is.EqualTo("test"));
                    Assert.That(prot.PasswordHash.IsEmpty, Is.True);
                    break;
                case UnifiedScenario.Doc2Docx:
                case UnifiedScenario.Doc2Rtf:
                case UnifiedScenario.Docx2DocxNoGold:
                case UnifiedScenario.Rtf2RtfNoGold:
                    Assert.That(prot.GetPassword(), Is.EqualTo(""));
                    Assert.That(prot.PasswordHash.IsEmpty, Is.False);
                    break;
                case UnifiedScenario.Doc2Wml:
                case UnifiedScenario.Wml2WmlNoGold:
                    // RK WordML does not have this feature.
                    Assert.That(prot.GetPassword(), Is.EqualTo(""));
                    Assert.That(prot.PasswordHash.IsEmpty, Is.True);
                    break;
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestUnprotectWithPassword(LoadFormat lf, SaveFormat sf)
        {
            const string correctPassword = "MyPassword1234567890";
            const string wrongPassword = "ThisIsIncorrectPassword";

            Document doc = new Document();

            // Protect document using correct password.
            doc.Protect(ProtectionType.ReadOnly, correctPassword);
            Assert.That(doc.ProtectionType, Is.EqualTo(ProtectionType.ReadOnly));

            // Try unprotecting with wrong password.
            Assert.That(doc.Unprotect(wrongPassword), Is.False);
            Assert.That(doc.ProtectionType, Is.EqualTo(ProtectionType.ReadOnly));

            // Try unprotecting with correct password.
            Assert.That(doc.Unprotect(correctPassword), Is.True);
            Assert.That(doc.ProtectionType, Is.EqualTo(ProtectionType.NoProtection));
        }

        /// <summary>
        /// WORDSNET-20831 Section.ProtectedForForms returns true for all sections in DOCX format even when a section is not protected.
        /// SectAttr.Unlocked is true when the section is not protected with forms,
        /// but in DOCX/WML formProt = 0 (false) when the section is not protected (see 2.6.6 in OOXML spec). 
        /// So the correct value of SectAttr.Unlocked should be !formProt.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefect20831(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Protection\TestDefect20831", lf, sf);

            // The document is protect with AllowOnlyFormFields protection type.
            Assert.That(doc.ProtectionType, Is.EqualTo(ProtectionType.AllowOnlyFormFields));

            // There are 3 sections in the document, the second is not protected.
            Assert.That(doc.Sections[0].ProtectedForForms, Is.True);
            Assert.That(doc.Sections[1].ProtectedForForms, Is.False);
            Assert.That(doc.Sections[2].ProtectedForForms, Is.True);
        }

        /// <summary>
        /// Validates write-protection password against obviously wrong, 'empty' and 'null' password strings.
        /// </summary>
        private static void ValidateBadPasswords(WriteProtection writeProtection)
        {
            Assert.That(writeProtection.ValidatePassword("wrong_password"), Is.False);
            Assert.That(writeProtection.ValidatePassword(""), Is.False);
            Assert.That(writeProtection.ValidatePassword(null), Is.False);
        }

        // FOSS: TestJira12137 removed — it validated a legacy plain (unhashed) write-protection password
        // in a binary .doc; DOC/DOT load and that legacy password path are gone.

        /// <summary>
        /// WORDSNET-12495 Password protected document cannot be unlocked anymore after editing with aspose
        /// RsaAES should be written in cryptProviderType element for SHA-512 protection.
        /// </summary>
        [Test]
        public void TestJira12495()
        {
            const string testFile = @"Model\Protection\TestJira12495.docx";

            Document doc = TestUtil.Open(testFile);
            string outFile = TestUtil.Save(doc, testFile);

            Assert.That(TestUtil.IsWrittenToOpcPackage(outFile, @"/word/settings.xml", "rsaAES"), Is.True);
        }





        /// <summary>
        /// WORDSNET-22989 Document can be unprotected with an empty password.
        /// An empty password shouldn't unprotect the document.
        /// </summary>
        [Test]
        public void Test26170()
        {
            Document doc = TestUtil.Open(@"Model\Protection\Test26170.docx");

            // Trying to unprotect with an empty password.
            Assert.That(doc.Unprotect(""), Is.EqualTo(false));
            Assert.That(ProtectionType.AllowOnlyFormFields, Is.EqualTo(doc.ProtectionType));

            // Trying to unprotect with correct password.
            Assert.That(doc.Unprotect("test"), Is.EqualTo(true));
            Assert.That(ProtectionType.NoProtection, Is.EqualTo(doc.ProtectionType));
        }
    }
}
