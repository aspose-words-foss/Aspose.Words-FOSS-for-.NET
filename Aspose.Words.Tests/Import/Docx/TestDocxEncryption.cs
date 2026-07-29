// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using Aspose.Words.Loading;
using Aspose.Words.Saving;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Docx
{
    /// <summary>
    /// Tests related to DOCX encryption.
    /// </summary>
    [TestFixture]
    public class TestDocxEncryption
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestEncrypted()
        {
            string fileName = TestUtil.BuildTestFileName(@"ImportDocx\Encryption\TestEncrypted.docx");

            FileFormatInfo info = FileFormatUtil.DetectFileFormat(fileName);
            Assert.That(info.LoadFormat, Is.EqualTo(LoadFormat.Docx));
            Assert.That(info.IsEncrypted, Is.EqualTo(true));

            Document doc = new Document(fileName, new LoadOptions("workshare"));

            Assert.That(doc.GetText(), Is.EqualTo("Hello World from an encrypted document!\x000c"));
        }

        /// <summary>
        /// WORDSNET-8935 “FileCorruptedException” exception occurs when try to open encrypted DOCX file.
        /// Fixed to throw a proper exception.
        /// </summary>
        [Test, ExpectedException(typeof(IncorrectPasswordException))]
        public void TestEncryptedWrongPassword()
        {
            new Document(TestUtil.BuildTestFileName(@"ImportDocx\Encryption\TestEncrypted.docx"), new LoadOptions(new string('A', 100)));
        }

        [Test, ExpectedException(typeof(IncorrectPasswordException))]
        public void TestEncryptedEmptyPassword()
        {
            new Document(TestUtil.BuildTestFileName(@"ImportDocx\Encryption\TestEncrypted.docx"), new LoadOptions(""));
        }

        [Test, ExpectedException(typeof(IncorrectPasswordException))]
        public void TestEncryptedNullPassword()
        {
            new Document(TestUtil.BuildTestFileName(@"ImportDocx\Encryption\TestEncrypted.docx"), new LoadOptions((string)null));
        }

        /// <summary>
        /// Although the user specifies a wrong load format we still load the document okay.
        /// </summary>
        [Test]
        public void TestEncryptedWrongLoadFormat()
        {
            Document doc = new Document(
                TestUtil.BuildTestFileName(@"ImportDocx\Encryption\TestEncrypted.docx"), new LoadOptions("workshare"));

            Assert.That(doc.OriginalLoadFormat, Is.EqualTo(LoadFormat.Docx));
            Assert.That(doc.GetText(), Is.EqualTo("Hello World from an encrypted document!\x000c"));
        }

        /// <summary>
        /// The document is an encrypted DOCX, but contains a stream of a DOC document that was
        /// causing wrong detection. Fixed.
        /// Passwords to open it are:
        /// pass1: Libertybell
        /// pass2: GreekTragedy
        /// </summary>
        [Test]
        public void TestEncryptedWithWordDocumentStream()
        {
            Document doc = new Document(
                TestUtil.BuildTestFileName(@"ImportDocx\Encryption\TestEncryptedWithWordDocumentStream.docx"),
                new LoadOptions("Libertybell"));

            Assert.That(doc.GetChildNodes(NodeType.Paragraph, true).Count, Is.EqualTo(57));
        }

        [Test]
        public void TestAgileEncryption()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Encryption\TestAgileEncryption.docx", new LoadOptions("123"));
            // insane check
            Assert.That(doc.GetText(), Is.EqualTo("test\f"));

            TestUtil.Open(@"ImportDocx\Encryption\TestAgileEncryptionBigger.docx", new LoadOptions("password"));
        }

        [Test]
        [ExpectedException(typeof(IncorrectPasswordException))]
        public void TestAgileEncryptionBadPassword()
        {
            TestUtil.Open(@"ImportDocx\Encryption\TestAgileEncryption.docx", new LoadOptions("1234"));
        }

        [Test]
        [ExpectedException(typeof(FileCorruptedException))]
        public void TestAgileEncryptionIntegrity()
        {
            TestUtil.Open(@"ImportDocx\Encryption\TestAgileEncryption Modified.docx", new LoadOptions("123"));
        }

        /// <summary>
        /// Encrypt with password and open with wrong password.
        /// </summary>
        [Test]
        [ExpectedException(typeof(IncorrectPasswordException))]
        public void TestEncryptionWrongPassword()
        {
            Document doc = TestUtil.Open(BaseTestFile, new LoadOptions("workshare"));
            SaveOpen(doc, BaseTestFile, "password", "wrongpassword", SaveFormat.Docx);
        }

        /// <summary>
        /// Encrypt with password and with the same password.
        /// </summary>
        [Test]
        public void TestEncryptionTheSamePassword()
        {
            Document doc = TestUtil.Open(BaseTestFile, new LoadOptions("workshare"));
            doc = SaveOpen(doc, BaseTestFile, "amorozov", "amorozov", SaveFormat.Docx);
            Assert.That(doc.GetText(), Is.EqualTo("Hello World from an encrypted document!\x000c"));
        }

        /// <summary>
        /// Encrypt with password and with the same password bigger file.
        /// </summary>
        [Test]
        public void TestEncryptionAgileToStandart()
        {
            const string testfile = @"ImportDocx\Encryption\TestAgileEncryptionBigger.docx";

            Document doc = TestUtil.Open(testfile, new LoadOptions("password"));
            doc = SaveOpen(doc, testfile, "amorozov", "amorozov", SaveFormat.Docx);
            Assert.That(doc.GetNodeById("1.0.0").GetText().StartsWith("Штаты, небольшой городок"), Is.True);
        }

        /// <summary>
        /// Encrypt with password and with the same password bigger file.
        /// </summary>
        [Test]
        public void TestEncryptionFlatOpc()
        {
            const string testfile = @"ImportDocx\Encryption\TestFlatOpc.xml";

            Document doc = TestUtil.Open(testfile, new LoadOptions("test"));
            doc = SaveOpen(doc, testfile, "amorozov", "amorozov", SaveFormat.FlatOpc);
            Assert.That(doc.GetText(), Is.EqualTo("Test Encrypted FlatOPC.\f"));
        }

        /// <summary>
        /// WORDSNET-7901 FileFormatInfo.LoadFormat return Docx for password protected docm.
        /// andrnosk: The reason of the problem is recursion which we are using to read encrypted documents.
        /// On the first stage when an OOXML document is encrypted, it is not possible to ascertained whether it is an Excel,
        /// Word or PowerPoint document without decrypting it first so we just return DOCX.
        /// But on the next stage if LoadPassword is specified we can get the original format but in current code
        /// we just replace this real format with Docx.
        /// Fixed by removing the code which ovveride LoadFormat.
        /// </summary>
        [Test]
        public void TestJira7901()
        {
            string fileName = TestUtil.BuildTestFileName(@"ImportDocx\Encryption\TestJira7901.docm");

            // While an OOXML document is encrypted, it is not possible to ascertained whether it is
            // an Excel, Word Docx, Docm or PowerPoint document without decrypting it first so we just return DOCX.
            FileFormatInfo fileFormatInfo = FileFormatUtil.DetectFileFormat(fileName);
            Assert.That(fileFormatInfo.IsEncrypted, Is.True);
            Assert.That(fileFormatInfo.LoadFormat, Is.EqualTo(LoadFormat.Docx));

            LoadOptions loadOption = new LoadOptions();
            loadOption.Password = "test";

            Document doc = TestUtil.Open(fileName, loadOption);

            // Now the document is decrypted and we can get the correct original Load Format.
            Assert.That(doc.OriginalLoadFormat, Is.EqualTo(LoadFormat.Docm));
        }

        /// <summary>
        /// WORDSNET-7964 Load Office 2013 encrypted document into Aspose.Words DOM
        /// Added support for SHA512 hash algorithm used in Office2013.
        ///
        /// On Java this requires Java Cryptography Extensions (JCE) in order to pass.
        /// You need to download it from http://www.oracle.com/technetwork/java/javasebusiness/downloads/java-archive-downloads-java-plat-419418.html#jce_policy-1.5.0-oth-JPR
        /// At the moment we are running unit tests on Java 5 and therefore JCE should be for Java 5.
        ///
        /// You need then to copy the JAR files into C:\Program Files (x86)\Java\jdk1.5.0_22\jre\lib\security
        /// </summary>
        [Test]
        public void TestJira7964()
        {
            LoadOptions lo = new LoadOptions("open");
            Document doc = TestUtil.Open(@"ImportDocx\Encryption\TestJira7964.docx", lo);

            Assert.That(doc.GetText(), Is.EqualTo("test\f"));
        }

        /// <summary>
        /// WORDSNET-9654 Aspose.Words.FileCorruptedException is thrown while loading MS Word 2013 document
        /// PKE hash algorithm is different.
        /// </summary>
        [Test]
        public void TestJira9654()
        {
            LoadOptions lo = new LoadOptions("pass");
            Document doc = TestUtil.Open(@"ImportDocx\Encryption\TestJira9654.docx", lo);

            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(4));
        }


        /// <summary>
        /// WORDSNET-17059 Aspose.Words.UnsupportedFileFormatException is thrown while loading DOCX
        /// Document is encrypted with ECMA376 encryption but DataSpaces stream is missing.
        /// </summary>
        [Test]
        public void TestJira17059()
        {
            LoadOptions lo = new LoadOptions();
            lo.Password = "123456";

            Document doc = TestUtil.Open(@"ImportDocx\Encryption\TestJira17059.docx", lo);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// WORDSNET-19272 Aspose.Words throws an error upon loading Apple Pages Documents
        /// "Primary" stream instead of "\0x006Primary".
        /// </summary>
        [Test]
        public void Test19272()
        {
            LoadOptions lo = new LoadOptions();
            lo.Password = "test123";

            Document doc = TestUtil.Open(@"ImportDocx\Encryption\Test19272.docx", lo);
            Assert.That(doc.FirstSection.Body.FirstParagraph.FirstRun.Text, Is.EqualTo("This is a test Word document from Pages app"));
        }

        /// <summary>
        /// WORDSNET-22837 Loading encrypted word File Corrupted Exception
        /// Version stream is missing and Primary stream has invalid transform values.
        /// It seems that Word does not verify these fields at all.
        /// </summary>
        [Test]
        public void Test22837()
        {
            LoadOptions lo = new LoadOptions();
            lo.LoadFormat = LoadFormat.Docx;
            lo.Password = "112233";

            Document doc = TestUtil.Open(@"ImportDocx\Encryption\Test22837.docx", lo);
            Assert.That(doc.GetText(), Is.EqualTo("37040619890809102X开房记录\f"));
        }

        /// <summary>
        /// Saves file encrypted with given password and opens it again using the same password.
        /// </summary>
        private static Document SaveOpen(Document doc, string filename, string savePassword, string loadPassword, SaveFormat sf)
        {
            string outfile = TestUtil.BuildOutFileName(TestUtil.BuildTestFileName(filename), "", sf);

            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions();
            saveOptions.Password = savePassword;
            TestUtil.Save(doc, outfile, saveOptions, false);
            return TestUtil.Open(outfile, new LoadOptions(loadPassword));
        }

        private const string BaseTestFile = @"ImportDocx\Encryption\TestEncrypted.docx";

    }
}
