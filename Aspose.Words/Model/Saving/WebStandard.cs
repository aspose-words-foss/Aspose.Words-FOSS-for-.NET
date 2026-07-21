// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/10/2016 by Alexey Butalov

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Indicates the version of the HTML standard and DOCTYPE declaration which are used when saving a document to
    /// HTML-based formats.
    /// </summary>
    internal enum WebStandard
    {
        /// <summary>
        /// Saves the document in compliance with the XHTML 1.0 Transitional standard. 
        /// Omits DOCTYPE declaration, that triggers the quirks mode when rendering the output HTML document
        /// in modern browsers.
        /// </summary>
        XhtmlTransitionalQuirks,

        /// <summary>
        /// Saves the document in compliance with the XHTML 1.0 Transitional standard.
        /// Writes the corresponding DOCTYPE declaration.
        /// </summary>
        XhtmlTransitional,

        /// <summary>
        /// Saves the document in compliance with the HTML 5 standard.
        /// </summary>
        Html5,

        /// <summary>
        /// Saves the document in compliance with the Epub standard.
        /// </summary>
        Epub
    }
}
