// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/01/2017 by Alexey Butalov

using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Helps to read fldChar from DOCX document. 
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal class DocxFldCharReader : NrxFldCharReaderBase
    {
        protected override bool ReadFormatSpecificAttribute(string attrName, NrxDocumentReaderBase reader, RunPr runPr)
        {
            switch (attrName)
            {
                case "ffData":
                    DocxFfDataReader.Read(reader.XmlReader, reader.ComplianceInfo);
                    return true;
                default:
                    return false;
            }
        }
    }
}