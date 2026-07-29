// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/01/2017 by Alexey Butalov

using System.IO;
using Aspose.Words.DigitalSignatures;
using Aspose.Words.Loading;

namespace Aspose.Words.RW.Docx.Reader
{
    internal class OpenXmlDocumentReader : IDocumentReader
    {
        internal OpenXmlDocumentReader(Stream stream, LoadOptions loadOptions, FileFormatInfo fileFormatInfo, Document document)
        {
            // alexnosk: Do not assert if fileFormatInfo is null, this means use specified LoadFormat explicitly without detection.
            // If code fails to read file it will fallback to file format detection. 
            Debug.Assert(loadOptions != null);
            Debug.Assert(stream != null);
            Debug.Assert(document != null);
            mLoadOptions = loadOptions;
            mStream = stream;
            mFileFormatInfo = fileFormatInfo;
            mDocument = document;
        }

        public void Read()
        {
            Debug.Assert ((mFileFormatInfo == null) || !mFileFormatInfo.IsEncrypted);

            mDocument.ComplianceInfo = new OoxmlComplianceInfo();
            DocxReader reader = new DocxReader(mStream, mDocument, mLoadOptions);
            reader.Read();
        }

        public bool IsEncrypted
        {
            get { return (mFileFormatInfo != null) && mFileFormatInfo.IsEncrypted; }
        }

        public Stream Decrypt()
        {
            return DigitalSignatureUtil.DecryptFileSystem(mFileFormatInfo.FileSystem, mLoadOptions.Password);
        }

        private readonly LoadOptions mLoadOptions;
        private readonly Stream mStream;
        private readonly FileFormatInfo mFileFormatInfo;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Document mDocument;
    }
}
