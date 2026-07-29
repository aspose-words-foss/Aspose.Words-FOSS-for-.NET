// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/01/2014 by Alexey Butalov

using Aspose.Words.Revisions;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal class DocxCellPrReader : NrxCellPrReaderBase
    {
        internal DocxCellPrReader(DocxParaReader paraReader)
            : base(paraReader)
        {
        }

        protected override bool ReadFormatSpecific(string localName, NrxDocumentReaderBase reader, CellPr cellPr)
        {
            switch (localName)
            {
                case "tcPrChange": //aml:annotation
                    DocxAnnotationReader.ReadCellPrRevision(reader, cellPr);
                    return true;
                case "cellIns":
                    DocxAnnotationReader.ReadEditRevision(reader, cellPr, EditRevisionType.Insertion);
                    return true;
                case "cellDel":
                    DocxAnnotationReader.ReadEditRevision(reader, cellPr, EditRevisionType.Deletion);
                    return true;
                default:
                    return false;
            }
        }
    }
}