// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/12/2016 by Alexey Butalov

using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides method for parsing 'w:tc' element.
    /// NOTE: This class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal class DocxCellReader : NrxCellReaderBase
    {
        internal DocxCellReader(DocxStoryReader storyReader,
            DocxParaPrReader paraPrReader,
            DocxCellPrReader cellPrReader) :
            base(storyReader, paraPrReader, cellPrReader)
        {
        }
    }
}