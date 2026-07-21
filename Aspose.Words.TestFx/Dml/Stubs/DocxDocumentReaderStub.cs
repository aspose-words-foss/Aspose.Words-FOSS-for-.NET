// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2018 by Alexey Butalov

using System;
using System.IO;
using Aspose.IO;
using Aspose.OpcPackaging;
using Aspose.Words.Loading;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.Tests.Dml
{
    internal class DocxDocumentReaderStub : DocxDocumentReaderBase
    {
        internal DocxDocumentReaderStub(string xml)
            : this(xml, null)
        {
        }

        internal DocxDocumentReaderStub(string xml,
            IWarningCallback warningCallback)
            : base(new OpcPackage(),
                GetDocxDocumentReaderStub(),
                new Document(),
                new LoadOptions(),
                new OoxmlComplianceInfo(),
                DocxReaderFactory.StylesReader,
                DocxReaderFactory.StoryReader,
                DocxReaderFactory.NumberingReader,
                DocxReaderFactory.SectPrReader)
        {
            MemoryStream stream = new MemoryStream();
            StreamUtil.WriteAnsiStringToStream(xml, stream);
            stream.Position = 0;

            DocxXmlReader xmlReader = new DocxXmlReader(stream, warningCallback, new OoxmlComplianceInfo());
            PushPartReader(xmlReader);
        }

        internal DocxDocumentReaderStub(Stream stream)
            : base(new OpcPackage(),
                GetDocxDocumentReaderStub(),
                new Document(),
                new LoadOptions(),
                new OoxmlComplianceInfo(),
                DocxReaderFactory.StylesReader,
                DocxReaderFactory.StoryReader,
                DocxReaderFactory.NumberingReader,
                DocxReaderFactory.SectPrReader)
        {
            DocxXmlReader xmlReader = new DocxXmlReader(stream, new OoxmlComplianceInfo());
            PushPartReader(xmlReader);
        }

        protected override void DoRead()
        {
            throw new NotImplementedException();
        }

        public override string GetRelationshipTarget(string relId)
        {
            return string.Format("reltaget/{0}", relId);
        }

        internal override bool IsEquationXmlReader
        {
            get { return false; }
        }

        public override byte[] GetBinData(string relId)
        {
            return new byte[0];
        }

        private static OpcPackagePart GetDocxDocumentReaderStub()
        {
            OpcPackagePart documentPartStub = new OpcPackagePart("document.xml", "test");
            documentPartStub.Stream = new MemoryStream();
            StreamUtil.WriteAnsiStringToStream("<document />", documentPartStub.Stream);
            documentPartStub.Stream.Position = 0;
            return documentPartStub;
        }
    }
}
