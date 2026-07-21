// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2017 by Alexey Butalov

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides methods for reading paragraph properties in DOCX.
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal class DocxParaPrReader : NrxParaPrReaderBase
    {
        internal DocxParaPrReader(DocxRunPrReader runPrReader,
            DocxSectPrReader sectPrReader) : 
            base(runPrReader, sectPrReader)
        {
        }

        protected override bool ReadFormatSpecific(string localName, 
            NrxDocumentReaderBase reader, ParaPr paraPr, RunPr runPr)
        {
            switch (localName)
            {
                case "pPrChange":
                    mAnnotationReader.ReadParaPrRevision(reader, paraPr, runPr);
                    return true;
                default:
                    return false;
            }
        }

        protected override bool ReadFormatSpecificNumPr(string localName, NrxXmlReader xmlReader, ParaPr paraPr)
        {
            switch (localName)
            {
                case "ins":
                    DocxAnnotationReader.ReadNumberingInsertion(xmlReader, paraPr);
                    return true;
                case "numberingChange":
                    DocxAnnotationReader.ReadNumberingChange(xmlReader, paraPr);
                    return true;
                default:
                    return false;
            }
        }

        internal void SetAnnotationReader(DocxAnnotationReader annotationReader)
        {
            Debug.Assert(annotationReader != null);
            mAnnotationReader = annotationReader;
        }

        private DocxAnnotationReader mAnnotationReader;
    }
}