// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words.Notes
{
    /// <summary>
    /// Specifies whether this is a footnote or an endnote.
    /// </summary>
    /// <remarks>
    /// <para>Both footnotes and endnotes are represented by objects by the <see cref="Footnote"/>
    /// class. Use <see cref="Aspose.Words.Notes.Footnote.FootnoteType"/> to distinguish between footnotes 
    /// and endnotes.</para>
    ///
    /// <seealso cref="Footnote"/>
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum FootnoteType
    {
        /// <summary>
        /// The object is a footnote.
        /// </summary>
        Footnote = 0,
        /// <summary>
        /// The object is an endnote.
        /// </summary>
        Endnote = 1,
    }
}
