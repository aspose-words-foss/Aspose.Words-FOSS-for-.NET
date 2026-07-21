// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/04/2015 by Ilya Navrotskiy

using Aspose.Bidi;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides static method for reading 'w:dir' and 'w:bdo' elements.
    /// </summary>
    internal static class DocxBidiControlCharReader
    {
        /// <summary>
        /// Reads BiDi control character into a model.
        /// In the model it will be stored as a Run with a corresponding
        /// Unicode BiDi control character <see cref="BidiChars"/>.
        /// </summary>
        internal static void Read(NrxDocumentReaderBase reader, DocxInlineReader inlineReader)
        {
            // Reader should be positioned on either 'dir' or 'bdo' element.
            NrxXmlReader xmlReader = reader.XmlReader;
            string bidiControlCharType = xmlReader.LocalName;
            
            Debug.Assert((bidiControlCharType == "dir") || (bidiControlCharType == "bdo"));

            // Read text direction attribute.
            string bidiTextDirection = xmlReader.ReadAttribute("ltr");
            char bidiChar = BidiControlCharUtil.ToBidiChar(bidiControlCharType, bidiTextDirection);
            
            // Add run with BiDi control character.
            Run run = new Run(reader.Document, bidiChar.ToString(), new RunPr());
            reader.AddChild(run);
            
            // Read children elements.
            xmlReader.MoveToElement();
            while (xmlReader.ReadChild(bidiControlCharType))
                inlineReader.ReadChild(reader);

            // Close 'bdo' or 'dir' element.
            run = new Run(reader.Document, BidiChars.PDF.ToString(), new RunPr());
            reader.AddChild(run);
        }
    }
}
