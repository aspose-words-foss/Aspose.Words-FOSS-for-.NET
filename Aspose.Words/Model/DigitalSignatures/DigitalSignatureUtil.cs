// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2013 by Alexey Morozov

using System;
using System.IO;
using System.Reflection;
using Aspose.Crypto;
using Aspose.JavaAttributes;
using Aspose.OpcPackaging;
using Aspose.Ss;
using Aspose.Words.RW.Factories;
using Aspose.Words.RW.OfficeCrypto;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.DigitalSignatures
{
    /// <summary>
    /// Provides methods for signing document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-digital-signatures/">Work with Digital Signatures</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>Since digital signature works with file content rather than Document Object Model these methods are put into a separate class.</para>
    /// <para>Supported formats are:
    /// <see cref= "LoadFormat.Doc"/>,
    /// <see cref= "LoadFormat.Dot"/>,
    /// <see cref= "LoadFormat.Docx"/>,
    /// <see cref= "LoadFormat.Dotx"/>,
    /// <see cref= "LoadFormat.Docm"/>,
    /// <see cref= "LoadFormat.Dotm"/>,
    /// <see cref= "LoadFormat.Odt"/>,
    /// <see cref= "LoadFormat.Ott"/>.</para>
    /// </remarks>
    public static class DigitalSignatureUtil
    {
        /// <summary>
        /// Signs source document using given <see cref="CertificateHolder"/> and <see cref="SignOptions"/>
        /// with digital signature and writes signed document to destination stream.
        /// <p>Supported formats are:
        /// <see cref= "LoadFormat.Doc"/>,
        /// <see cref= "LoadFormat.Dot"/>,
        /// <see cref= "LoadFormat.Docx"/>,
        /// <see cref= "LoadFormat.Dotx"/>,
        /// <see cref= "LoadFormat.Docm"/>,
        /// <see cref= "LoadFormat.Dotm"/>,
        /// <see cref= "LoadFormat.Odt"/>,
        /// <see cref= "LoadFormat.Ott"/>.</p>
        /// <p><b>Output will be written to the start of stream and stream size will be updated with content length.</b></p>
        /// </summary>
        /// <param name="srcStream">The stream which contains the document to sign.</param>
        /// <param name="dstStream">The stream that signed document will be written to.</param>
        /// <param name="certHolder"><see cref="CertificateHolder"/> object with certificate that used to sign file.
        /// <ms>The certificate in holder MUST contain private keys and have the X509KeyStorageFlags.Exportable flag set.</ms>
        /// <cpp>The certificate in holder MUST contain private keys.</cpp></param>
        /// <param name="signOptions"><see cref="SignOptions"/> object with various signing options.</param>
        /// <javaName>void sign(java.io.InputStream srcStream,java.io.OutputStream dstStream,com.aspose.words.CertificateHolder certHolder,com.aspose.words.SignOptions signOptions)</javaName>
        [JavaUseSecondApiChangeMap("dstStream")]
        public static void Sign([CppIOStreamWrapper(IOStreamType.IStream)]Stream srcStream,[CppIOStreamWrapper(IOStreamType.OStream)] Stream dstStream, CertificateHolder certHolder, SignOptions signOptions)
        {
            ArgumentUtil.CheckNotNull(srcStream, "srcStream");
            ArgumentUtil.CheckNotNull(dstStream, "dstStream");
            ArgumentUtil.CheckNotNull(certHolder, "certHolder");

            SignStreams(srcStream, dstStream, certHolder, signOptions);

            // WORDSJAVA-2079 internal MemoryStream should be flushed to public java.io.OutputStream.
            dstStream.Flush();
        }

        private static void SignStreams(Stream srcStream, Stream dstStream, CertificateHolder certHolder, SignOptions signOptions)
        {
            DigitalSignature signature = new DigitalSignature(certHolder);

            if (signOptions != null)
                signOptions.ApplyTo(signature);

            // FOSS

            FileFormatDetector detector = new FileFormatDetector();
            FileFormatInfo formatInfo = detector.Detect(srcStream);

            // Choose appropriate signer.
            switch (formatInfo.LoadFormat)
            {
                case LoadFormat.Doc:
                case LoadFormat.Dot:
                    throw new NotSupportedException("FOSS");

                case LoadFormat.Docx:
                case LoadFormat.Dotx:
                case LoadFormat.Docm:
                case LoadFormat.Dotm:
                    throw new NotSupportedException("FOSS");

                // WORDSNET-25914 Added case for OTT format.
                case LoadFormat.Ott:
                case LoadFormat.Odt:
                    throw new NotSupportedException("FOSS");
                default:
                    throw new InvalidOperationException(NotSupportedByFileFormat);
            }
        }

        /// <summary>
        /// Signs source document using given <see cref="CertificateHolder"/> and <see cref="SignOptions"/>
        /// with digital signature and writes signed document to destination file.
        /// <p>Supported formats are:
        /// <see cref= "LoadFormat.Doc"/>,
        /// <see cref= "LoadFormat.Dot"/>,
        /// <see cref= "LoadFormat.Docx"/>,
        /// <see cref= "LoadFormat.Dotx"/>,
        /// <see cref= "LoadFormat.Docm"/>,
        /// <see cref= "LoadFormat.Dotm"/>,
        /// <see cref= "LoadFormat.Odt"/>,
        /// <see cref= "LoadFormat.Ott"/>.</p>
        /// </summary>
        /// <param name="srcFileName">The file name of the document to sign.</param>
        /// <param name="dstFileName">The file name of the signed document output.</param>
        /// <param name="certHolder"><see cref="CertificateHolder"/> object with certificate that used to sign file.
        /// <ms>The certificate in holder MUST contain private keys and have the X509KeyStorageFlags.Exportable flag set.</ms>
        /// <cpp>The certificate in holder MUST contain private keys.</cpp></param>
        /// <param name="signOptions"><see cref="SignOptions"/> object with various signing options.</param>
        public static void Sign(string srcFileName, string dstFileName, CertificateHolder certHolder, SignOptions signOptions)
        {
            ArgumentUtil.CheckHasChars(srcFileName, "srcFileName");
            ArgumentUtil.CheckHasChars(dstFileName, "dstFileName");

            // Open source stream with read/write sharing because it can be the same file passed as source and destination.
            using (FileStream srcStream = File.Open(srcFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // The same reason we OpenOrCreate output file.
                using (FileStream dstStream = File.Open(dstFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                    Sign(srcStream, dstStream, certHolder, signOptions);
            }
        }

        /// <summary>
        /// Signs source document using given <see cref="CertificateHolder"/> with digital signature
        /// and writes signed document to destination stream.
        /// <p>Supported formats are:
        /// <see cref= "LoadFormat.Doc"/>,
        /// <see cref= "LoadFormat.Dot"/>,
        /// <see cref= "LoadFormat.Docx"/>,
        /// <see cref= "LoadFormat.Dotx"/>,
        /// <see cref= "LoadFormat.Docm"/>,
        /// <see cref= "LoadFormat.Dotm"/>,
        /// <see cref= "LoadFormat.Odt"/>,
        /// <see cref= "LoadFormat.Ott"/>.</p>
        /// <p><b>Output will be written to the start of stream and stream size will be updated with content length.</b></p>
        /// </summary>
        /// <param name="srcStream">The stream which contains the document to sign.</param>
        /// <param name="dstStream">The stream that signed document will be written to.</param>
        /// <param name="certHolder"><see cref="CertificateHolder"/> object with certificate that used to sign file.
        /// <ms>The certificate in holder MUST contain private keys and have the X509KeyStorageFlags.Exportable flag set.</ms>
        /// <cpp>The certificate in holder MUST contain private keys.</cpp></param>
        /// <javaName>void sign(java.io.InputStream srcStream,java.io.OutputStream dstStream,com.aspose.words.CertificateHolder certHolder)</javaName>
        [JavaUseSecondApiChangeMap("dstStream")]
        public static void Sign([CppIOStreamWrapper(IOStreamType.IStream)]Stream srcStream, [CppIOStreamWrapper(IOStreamType.OStream)]Stream dstStream, CertificateHolder certHolder)
         {
             Sign(srcStream, dstStream, certHolder, null);
         }

        /// <summary>
        /// Signs source document using given <see cref="CertificateHolder"/> with digital signature
        /// and writes signed document to destination file.
        /// <p>Supported formats are:
        /// <see cref= "LoadFormat.Doc"/>,
        /// <see cref= "LoadFormat.Dot"/>,
        /// <see cref= "LoadFormat.Docx"/>,
        /// <see cref= "LoadFormat.Dotx"/>,
        /// <see cref= "LoadFormat.Docm"/>,
        /// <see cref= "LoadFormat.Dotm"/>,
        /// <see cref= "LoadFormat.Odt"/>,
        /// <see cref= "LoadFormat.Ott"/>.</p>
        /// </summary>
        /// <param name="srcFileName">The file name of the document to sign.</param>
        /// <param name="dstFileName">The file name of the signed document output.</param>
        /// <param name="certHolder"><see cref="CertificateHolder"/> object with certificate that used to sign file.
        /// <ms>The certificate in holder MUST contain private keys and have the X509KeyStorageFlags.Exportable flag set.</ms>
        /// <cpp>The certificate in holder MUST contain private keys.</cpp></param>
        public static void Sign(string srcFileName, string dstFileName, CertificateHolder certHolder)
        {
            Sign(srcFileName, dstFileName, certHolder, null);
        }

        /// <summary>
        /// Removes all digital signatures from source file and writes unsigned file to destination file.
        /// <p>The following formats are compatible for digital signature removal:
        /// <see cref= "LoadFormat.Doc"/>,
        /// <see cref= "LoadFormat.Dot"/>,
        /// <see cref= "LoadFormat.Docx"/>,
        /// <see cref= "LoadFormat.Dotx"/>,
        /// <see cref= "LoadFormat.Docm"/>,
        /// <see cref= "LoadFormat.Dotm"/>,
        /// <see cref= "LoadFormat.Odt"/>,
        /// <see cref= "LoadFormat.Ott"/>.</p>
        /// </summary>
        public static void RemoveAllSignatures(string srcFileName, string dstFileName)
        {
            // Open source stream with read/write sharing because it can be the same file passed as source and destination.
            using (FileStream srcStream = File.Open(srcFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // The same reason we OpenOrCreate output file.
                using (FileStream dstStream = File.Open(dstFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                    RemoveAllSignatures(srcStream, dstStream);
            }
        }

        /// <summary>
        /// Removes all digital signatures from document in source stream and writes unsigned document to destination stream.
        /// <p><b>Output will be written to the start of stream and stream size will be updated with content length.</b></p>
        /// <p>The following formats are compatible for digital signature removal:
        /// <see cref= "LoadFormat.Doc"/>,
        /// <see cref= "LoadFormat.Dot"/>,
        /// <see cref= "LoadFormat.Docx"/>,
        /// <see cref= "LoadFormat.Dotx"/>,
        /// <see cref= "LoadFormat.Docm"/>,
        /// <see cref= "LoadFormat.Dotm"/>,
        /// <see cref= "LoadFormat.Odt"/>,
        /// <see cref= "LoadFormat.Ott"/>.</p>
        /// </summary>
        /// <javaName>void removeAllSignatures(java.io.InputStream srcStream,java.io.OutputStream dstStream)</javaName>
        [JavaUseSecondApiChangeMap("dstStream")]
        public static void RemoveAllSignatures([CppIOStreamWrapper(IOStreamType.IStream)]Stream srcStream, [CppIOStreamWrapper(IOStreamType.OStream)]Stream dstStream)
        {
            FileFormatDetector detector = new FileFormatDetector();
            FileFormatInfo formatInfo = detector.Detect(srcStream);

            switch (formatInfo.LoadFormat)
            {
                case LoadFormat.Doc:
                case LoadFormat.Dot:
                    {
                        FileSystem fs = new FileSystem(srcStream);

                        fs.Root.Remove(OfficeCryptoNames.XmlDsigSignatureStorageName);
                        fs.Root.Remove(OfficeCryptoNames.CryptoApiSignatureStreamName);

                        dstStream.Position = 0;

                        // Save file system to destination stream.
                        fs.Save(dstStream);

                        // Update stream length.
                        dstStream.SetLength(dstStream.Position);
                        break;
                    }

                case LoadFormat.Docx:
                case LoadFormat.Dotx:
                case LoadFormat.Docm:
                case LoadFormat.Dotm:
                    {
                        OpcPackage package = new OpcPackage(srcStream);
                        // Look for existing signatures.
                        OpcPackagePart originPart = package.GetPartByRelationshipType(null, OpcRelationshipType.DigitalSignatureOrigin);

                        if (originPart != null)
                        {
                            foreach (OpcRelationship rel in originPart.Rels)
                            {
                                string signaturePartName = originPart.GetRelatedPartName(rel);
                                package.Parts.Remove(signaturePartName);
                            }

                            package.Parts.Remove(originPart.Name);
                            OpcRelationship originPartRel = package.Rels.GetFirstByType(OpcRelationshipType.DigitalSignatureOrigin);
                            package.Rels.Remove(originPartRel.Id);
                            package.UpdateRelationshipsAndContentTypes();
                        }

                        dstStream.Position = 0;

                        // Save package to destination stream.
                        package.Save(dstStream);

                        dstStream.SetLength(dstStream.Position);
                        break;
                    }

                // WORDSNET-25043 Implemented signature removing in ODT.
                // WORDSNET-25411 Added case for OTT.
                case LoadFormat.Odt:
                case LoadFormat.Ott:
                    throw new NotSupportedException("FOSS");
                default:
                    throw new InvalidOperationException(NotSupportedByFileFormat);
            }
        }

        /// <summary>
        /// Loads digital signatures from document.
        /// </summary>
        /// <param name="fileName">Path to the document.</param>
        /// <returns>Collection of digital signatures. Returns empty collection if file is not signed.</returns>
        public static DigitalSignatureCollection LoadSignatures(string fileName)
        {
            using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                return LoadSignatures(stream);
        }

        /// <summary>
        /// Loads digital signatures from document using stream.
        /// </summary>
        /// <param name="stream">Stream with the document.</param>
        /// <returns>Collection of digital signatures. Returns empty collection if file is not signed.</returns>
        /// <javaName>com.aspose.words.DigitalSignatureCollection loadSignatures(java.io.InputStream stream)</javaName>
        public static DigitalSignatureCollection LoadSignatures([CppIOStreamWrapper(IOStreamType.IStream)]Stream stream)
        {
            FileFormatDetector detector = new FileFormatDetector();
            FileFormatInfo formatInfo = detector.Detect(stream);

            switch (formatInfo.LoadFormat)
            {
                case LoadFormat.Doc:
                case LoadFormat.Dot:
                case LoadFormat.Docx:
                case LoadFormat.Dotx:
                case LoadFormat.Dotm:
                case LoadFormat.Docm:
                case LoadFormat.Odt:
                case LoadFormat.Ott:
                    // Simplify the task for a while and use existing code.
                    Document doc = new Document(stream, null, false);
                    return doc.DigitalSignatures;

                default:
                    throw new InvalidOperationException(NotSupportedByFileFormat);
            }
        }

        /// <summary>
        /// Decrypts <see cref="FileSystem"/> using specified password.
        /// </summary>
        internal static MemoryStream DecryptFileSystem(FileSystem fileSystem, string password)
        {
            DataSpaces dataSpaces = new DataSpaces(fileSystem);
            if (dataSpaces.IsEncryptedEcma376())
            {
                MemoryStream decryptedStream = dataSpaces.Decrypt(password);

                // SPEED After we decrypted we don't need these and we null them to remove
                // all references to the structured storage so it can go to garbage.
                // Although resharper grays this code out, I think it helps with memory.
                dataSpaces = null;
                fileSystem = null;

                return decryptedStream;
            }
            else
                throw new UnsupportedFileFormatException("Unknown file format.");
        }

        /// <summary>
        /// Returns a string with a digest method.
        /// </summary>
        internal static string GetDigestMethod(DigestAlgorithm digestAlgorithm)
        {
            switch (digestAlgorithm)
            {
                case DigestAlgorithm.Sha256:
                    return "http://www.w3.org/2001/04/xmlenc#sha256";
                default:
                    return "http://www.w3.org/2000/09/xmldsig#sha1";
            }
        }

        /// <summary>
        /// Loads digital signatures from XPS document.
        /// </summary>
        private static DigitalSignatureCollection LoadFromXps(Stream stream)
        {
            stream.Position = 0;
            OpcPackage package = new OpcPackage(stream);

            DigitalSignatureCollection signatures = new DigitalSignatureCollection();

            OpcPackagePart signatureOriginPart = package.GetPartByRelationshipType(null, OpcRelationshipType.DigitalSignatureOrigin);
            if (signatureOriginPart != null)
                foreach (OpcRelationship rel in signatureOriginPart.Rels)
                {
                    string signaturePartName = signatureOriginPart.GetRelatedPartName(rel);
                    OpcPackagePart signaturePart = package.FetchPartByName(signaturePartName);

                    OpcPackagePart certificatePart =
                        package.GetPartByRelationshipType(signaturePart, OpcRelationshipType.DigitalSignatureCertificate);


                    CertificateHolderInternal cert = Crypto.CryptoUtilPal.CreateHolder(
                        certificatePart.GetAsMemoryStream().ToArray());

                    XmlDsigReader.Read(signaturePart.Stream, cert, new OpcReferenceResolver(package), signatures, null);
                }

            return signatures;
        }

        private const string NotSupportedByFileFormat = "Signing feature is not supported by this file format.";
    }
}
