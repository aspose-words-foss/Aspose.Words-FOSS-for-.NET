// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2016 by Alexey Butalov

using Aspose.JavaAttributes;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Provides methods for reading run properties from different document parts.
    /// NOTE: Derived classes should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal abstract class NrxRunPrReaderBase
    {
        /// <summary>
        /// Reads 'w:rPr' element from the specified reader. 
        /// Reader should be positioned to element start.
        /// </summary>
        [JavaThrows(true)]
        internal abstract void Read(NrxDocumentReaderBase reader, RunPr runPr);

        /// <summary>
        /// Reads an element that can occur as a child of an w:rPr element or m:rPr element in case of Office Math.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="runPr"></param>
        /// <param name="resiliency">Indicates that element occurred in inappropriate location.</param>
        [JavaThrows(true)]
        internal abstract void ReadChild(NrxDocumentReaderBase reader, RunPr runPr, bool resiliency);

        internal abstract void ReadFonts(NrxXmlReader xmlReader, RunPr runPr);
    }
}
