// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/07/2010 by Roman Korchagin

using System.Text;
using Aspose.Ss;
using Aspose.Words.DigitalSignatures;

namespace Aspose.Words
{
    /// <summary>
    /// Contains data returned by <see cref="FileFormatUtil"/> document format detection methods.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/detect-file-format-and-check-format-compatibility/">Detect File Format and Check Format Compatibility</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>You do not create instances of this class directly. Objects of this class are returned by
    /// <see cref="FileFormatUtil.DetectFileFormat(System.IO.Stream)"/> methods.</para>
    /// </remarks>
    public class FileFormatInfo
    {
        /// <summary>
        /// No public ctor.
        /// </summary>
        internal FileFormatInfo()
        {
        }

        /// <summary>
        /// Gets the detected document format.
        /// </summary>
        /// <remarks>
        /// <para>When an OOXML document is encrypted, it is not possible to ascertain whether it is
        /// an Excel, Word or PowerPoint document without decrypting it first so for an encrypted OOXML
        /// document this property will always return <see cref="Aspose.Words.LoadFormat.Docx"/>.</para>
        ///
        /// <seealso cref="IsEncrypted"/>
        /// </remarks>
        public LoadFormat LoadFormat
        {
            get { return mLoadFormat; }
        }

        /// <summary>
        /// Returns <c>true</c> if the document is encrypted and requires a password to open.
        /// </summary>
        /// <remarks>
        /// <para>This property exists to help you sort documents that are encrypted from those that are not.
        /// If you attempt to load an encrypted document using Aspose.Words without supplying a password an
        /// exception will be thrown. You can use this property to detect whether a document requires a password
        /// and take some action before loading a document, for example, prompt the user for a password. </para>
        ///
        /// <seealso cref="LoadFormat"/>
        /// </remarks>
        public bool IsEncrypted
        {
            get { return mIsEncrypted; }
        }

        /// <summary>
        /// Returns <c>true</c> if this document contains a digital signature.
        /// This property merely informs that a digital signature is present on a document,
        /// but it does not  specify whether the signature is valid or not.
        /// </summary>
        /// <remarks>
        /// <para>This property exists to help you sort documents that are digitally signed from those that are not.
        /// If you use Aspose.Words to modify and save a document that is digitally signed, then the digital signature will
        /// be lost. This is by design because a digital signature exists to guard the authenticity of a document.
        /// Using this property you can detect digitally signed documents before processing them in the same way as normal
        /// documents and take some action to avoid losing the digital signature, for example notify the user.
        /// </para>
        /// </remarks>
        public bool HasDigitalSignature
        {
            get { return mHasDigitalSignature; }
        }

        /// <summary>
        /// Returns <c>true</c> if this document contains a VBA macros.
        /// </summary>
        public bool HasMacros { get; internal set; }

        /// <summary>
        /// Gets the detected encoding if applicable to the current document format.
        /// At the moment detects encoding only for HTML documents.
        /// </summary>
        public Encoding Encoding
        {
            get { return mEncoding; }
        }

        /// <summary>
        /// Signing algorithm used in this file. Valid only if <see cref="HasDigitalSignature" /> is <c>true</c>.
        /// </summary>
        internal DigitalSignatureType DigitalSignatureType
        {
            get { return mDigitalSignatureType; }
            set { mDigitalSignatureType = value; }
        }

        /// <summary>
        /// Indicates that the file is a MS Word template document.
        /// </summary>
        internal bool IsDocumentTemplate
        {
            get
            {
                switch (mLoadFormat)
                {
                    case LoadFormat.Dotx:
                    case LoadFormat.Dot:
                    case LoadFormat.Dotm:
                    case LoadFormat.FlatOpcTemplate:
                    case LoadFormat.FlatOpcTemplateMacroEnabled:
                        return true;

                    default:
                        return false;
                }
            }
        }

        internal void SetLoadFormat(LoadFormat loadFormat)
        {
            mLoadFormat = loadFormat;
        }

        internal void SetIsEncrypted(bool isEncrypted)
        {
            mIsEncrypted = isEncrypted;
        }

        internal void SetIsDigitalSignaturePresent(bool isDigitalSignaturePresent)
        {
            mHasDigitalSignature = isDigitalSignaturePresent;
        }

        internal void SetEncoding(Encoding encoding)
        {
            mEncoding = encoding;
        }

        /// <summary>
        /// Gets the structured storage file system if the document is structured storage; <c>null</c> otherwise.
        ///
        /// RK This does not look like a very good idea to pass the Structured Storage File System here in the FileFormatInfo class.
        /// Was it done for speed optimization or for what purpose? I think it creates some bad dependencies. Should be investigated.
        /// </summary>
        internal FileSystem FileSystem
        {
            get { return mFileSystem; }
            set { mFileSystem = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether file contains characters belonging to RTL scripts.
        /// </summary>
        internal bool HasRtlScript
        {
            get { return mHasRtlScript; }
            set { mHasRtlScript = value; }
        }

        /// <summary>
        /// Returns guessed <see cref="ApplicationType"/> where the document was created.
        /// </summary>
        internal ApplicationType Application
        {
            get { return mApplication; }
            set { mApplication = value; }
        }

        /// <summary>
        /// Indicates that document is embedded into WordDocument stream.
        /// </summary>
        internal bool EmbeddedInWordDocument = false;

        private LoadFormat mLoadFormat = LoadFormat.Unknown;
        private bool mIsEncrypted;
        private bool mHasDigitalSignature;
        private Encoding mEncoding;
        private bool mHasRtlScript;
        private DigitalSignatureType mDigitalSignatureType = DigitalSignatureType.Unknown;
        private FileSystem mFileSystem;
        private ApplicationType mApplication = ApplicationType.Unknown;
    }
}
