// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/09/2016 by Alexey Butalov

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Indicates the version of HTML is used when saving the document to <see cref="SaveFormat.Html"/> and 
    /// <see cref="SaveFormat.Mhtml"/> formats.
    /// </summary>
    public enum HtmlVersion
    {
        /// <summary>
        /// Saves the document in compliance with the XHTML 1.0 Transitional standard. 
        /// </summary>
        /// <remarks>
        /// <p>Aspose.Words aims to output XHTML according to the XHTML 1.0 Transitional standard, 
        /// but the output will not always validate against the DTD. Some structures inside a Microsoft Word 
        /// document are hard or impossible to map to a document that will validate against the XHTML schema. 
        /// For example, XHTML does not allow nested lists (UL cannot be nested inside another UL element), 
        /// but in Microsoft Word document multilevel lists occur quite often.</p>
        /// </remarks>
        Xhtml = 0,
        /// <summary>
        /// Saves the document in compliance with the HTML 5 standard. 
        /// </summary>
        Html5 = 1
    }
}
