// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/03/2010 by Denis Darkin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Allows to specify the version of HTML output of any web page 
    /// produced by this document. 
    /// </summary>
    internal enum WebTarget
    {
        /// <summary>
        /// If document does not contain any definition, this one is default.
        /// </summary>
        None,
        
        /// <summary>
        /// W3C XHTML 1.0 + CSS 1
        /// </summary>
        XhtmlPlusCss1,
        
        /// <summary>
        /// W3C HTML 4.01 + CSS 1
        /// </summary>
        Html4PlusCss1,
        
        /// <summary>
        /// W3C XHTML 1.0 + CSS 2
        /// </summary>
        XhtmlPlusCss2,
        
        /// <summary>
        /// W3C HTML 4.01 + CSS 2
        /// </summary>
        Html4PlusCss2
    }
}
