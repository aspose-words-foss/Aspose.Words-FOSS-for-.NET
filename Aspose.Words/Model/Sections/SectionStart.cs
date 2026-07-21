// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// The type of break at the beginning of the section.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum SectionStart
    {
        /// <summary>
        /// The new section starts on the same page as the previous section.
        /// </summary>
        Continuous = 0,
        /// <summary>
        /// The section starts from a new column.
        /// </summary>
        NewColumn = 1,
        /// <summary>
        /// The section starts from a new page.
        /// </summary>
        NewPage = 2,
        /// <summary>
        /// The section starts on a new even page.
        /// </summary>
        EvenPage = 3,
        /// <summary>
        /// The section starts on a new odd page.
        /// </summary>
        OddPage = 4
    }
}
