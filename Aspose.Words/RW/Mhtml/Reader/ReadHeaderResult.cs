// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2016 by Nikolay Sezganov

namespace Aspose.Words.RW.Mhtml.Reader
{
    /// <summary>
    /// Specifies result type of header reading in <see cref="MhtmlFormatDetector"/> .
    /// </summary>
    internal enum ReadHeaderResult
    {
        /// <summary>
        /// Result is successful.
        /// </summary>
        Success,
        /// <summary>
        /// Result contains anything with illegal chars. 
        /// </summary>
        ContainsIllegalChars,
        /// <summary>
        /// Result contains only line break chars. 
        /// </summary>
        EmptyLine
    }
}