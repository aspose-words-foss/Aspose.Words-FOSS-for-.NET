// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/12/2016 by Alexey Butalov

using Aspose.JavaAttributes;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Provides methods for parsing 'w:p' element.
    /// NOTE: Derived classes should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal abstract class NrxParaReaderBase
    {
        [JavaThrows(true)]
        internal abstract bool Read(NrxDocumentReaderBase reader);
    }
}
