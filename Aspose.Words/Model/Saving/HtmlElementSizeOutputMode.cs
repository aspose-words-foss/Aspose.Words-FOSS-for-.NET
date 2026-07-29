// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/03/2011 by Viktor Sazhaev

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how Aspose.Words exports element widths and heights to HTML, MHTML and EPUB.
    /// </summary>
    /// <seealso cref="HtmlSaveOptions.TableWidthOutputMode"/>
    public enum HtmlElementSizeOutputMode
    {
        /// <summary>
        /// All element sizes, both in absolute and relative units, specified in the document are exported.
        /// </summary>
        All = 0,
        /// <summary>
        /// Element sizes are exported only if they are specified in relative units in the document. 
        /// Fixed sizes are not exported in this mode. Visual agents will calculate missing sizes to make 
        /// document layout more natural.
        /// </summary>
        RelativeOnly = 1,
        /// <summary>
        /// Element sizes are not exported. Visual agents will build layout automatically according to relationship between elements.
        /// </summary>
        None = 2
    }
}
