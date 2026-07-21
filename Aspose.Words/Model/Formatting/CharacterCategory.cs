// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/03/2007 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// A run in Microsoft Word can have up to four fonts assigned to it.
    /// Different characters in a single run can use different fonts depending on their Unicode character code.
    /// This enum identifies character category (that allows to select the proper font to use).
    /// http://en.wikipedia.org/wiki/Mapping_of_Unicode_characters
    ///
    /// This enum is also used as the Font Type Hint value.
    /// Font Type Hint specifies the font type which shall be used to format any ambiguous characters in a run.
    ///
    /// An interesting note in the RTF specification:
    /// South Asian (complex script) runs of text share associated properties with right-to-left runs,
    /// but they are considered left-to-right.
    /// </summary>
    internal enum CharacterCategory
    {
        /// <summary>
        /// Font used for characters that do not fall into any of the above ranges.
        /// Also known as High ASCII.
        /// </summary>
        Other = 0,
        /// <summary>
        /// Font used for the East Asian characters.
        /// Also known as East Asian.
        /// </summary>
        FarEast = 1,
        /// <summary>
        /// Font used for the Complex Script characters.
        /// </summary>
        ComplexScript = 2,
        /// <summary>
        /// Font used for the 0..127 characters.
        /// </summary>
        /// <remarks>This value is not used as <see cref="RunPr.CharacterCategoryHint"/>,
        /// may be it's better to create separate enum for character hints.</remarks>
        Ascii
    }
}
