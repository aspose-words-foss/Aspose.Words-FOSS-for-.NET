// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/05/2016 by Anton Savko

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how Aspose.Words exports OfficeMath to HTML, MHTML and EPUB.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum HtmlOfficeMathOutputMode
    {
        /// <summary>
        /// OfficeMath is converted to HTML as image specified by &lt;img&gt; tag.
        /// </summary>
        Image = 0,
        /// <summary>
        /// OfficeMath is converted to HTML using MathML.
        /// </summary>
        MathML = 1,
        /// <summary>
        /// OfficeMath is converted to HTML as sequence of runs specified by &lt;span&gt; tags.
        /// </summary>
        Text = 2
    }
}
