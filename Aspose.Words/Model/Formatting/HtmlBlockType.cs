// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/02/2013 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Specifies type for <see cref="HtmlBlock" /> element.
    /// </summary>
    internal enum HtmlBlockType
    {
        /// <summary>
        /// Specifies that HtmlBlock represents an HTML div element. 
        /// </summary>
        Div = 0,

        /// <summary>
        /// Specifies that HtmlBlock represents an HTML blockquote element. 
        /// This element shall specify that this container shall be written out using the blockquote element 
        /// if this document is subsequently saved as HTML.
        /// </summary>
        BlockQuote = 1,

        /// <summary>
        /// Specifies that HtmlBlock represents formatting properties on the HTML body element. 
        /// This element shall specify that the properties specified by this container shall be written 
        /// out onto the body element if this document is subsequently saved as HTML.
        /// </summary>
        Body = 2
    }
}
