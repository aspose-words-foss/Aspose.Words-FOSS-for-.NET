// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2017 by Alexey Butalov

using Aspose.Words.Notes;
using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides methods for reading run of text in DOCX.
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal class DocxRunReader : NrxRunReaderBase
    {
        internal DocxRunReader(NrxStoryReaderBase storyReader, 
            DocxRunPrReader runPrReader, 
            DocxParaReader paraReader,
            DocxFldCharReader fldCharReader,
            NrxFldSimpleReader fldSimpleReader,
            DocxHyperlinkReader hyperlinkReader,
            DocxAnnotationReader annotationReader,
            INrxVmlReader vmlReader) : 
            base(storyReader, runPrReader, paraReader, fldCharReader, fldSimpleReader, hyperlinkReader, vmlReader)
        {
            Debug.Assert(annotationReader != null);
            mAnnotationReader = annotationReader;
        }

        protected override bool ReadFormatSpecific(string localName, NrxDocumentReaderBase reader, RunPr runPr)
        {
            switch (localName)
            {
                case "altChunk":
                    DocxAltChunkReader.Read(reader);
                    return true;
                case "del":
                    // WORDSNET-10596 there are revision 'del' elements inside math <m:r>.
                    mAnnotationReader.ReadMathRunPrRevision(reader, runPr, true);
                    return true;
                case "ins":
                    // WORDSNET-10596 there are revision 'ins' elements inside math <m:r>.
                    mAnnotationReader.ReadMathRunPrRevision(reader, runPr, false);
                    return true;
                default:
                    return false;
            }
        }

        protected override void ReadTable(NrxDocumentReaderBase reader)
        {
            DocxTableReader.Read(reader);
        }

        protected override Footnote GetFootnote(NrxDocumentReaderBase reader, 
            int id, FootnoteType footnoteType, RunPr runPr)
        {
            return reader.GetFootnoteById(footnoteType, id);
        }

        protected override Comment GetComment(NrxDocumentReaderBase reader, RunPr runPr)
        {
            int id = reader.XmlReader.ReadIntAttribute("id", 0);
            return reader.GetCommentById(id);
        }

        private readonly DocxAnnotationReader mAnnotationReader;
    }
}
