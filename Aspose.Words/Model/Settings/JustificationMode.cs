// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Specifies the character spacing adjustment for a document.
    /// The default value is <c>Expand</c>.
    /// </summary>
    /// <dev>
    /// In HTML, this is equivalent to setting text-justify-trim on the BODY element.
    /// </dev>
    public enum JustificationMode
    {
        /// <summary>
        /// Do not compress character spacing.
        /// </summary>
        Expand = 0,

        /// <summary>
        /// Compress character spacing.
        /// </summary>
        Compress = 1,

        /// <summary>
        /// Compress, using rules of the kana syllabaries, Hiragana and Katakana.
        /// </summary>
        CompressKana = 2
    }
}
