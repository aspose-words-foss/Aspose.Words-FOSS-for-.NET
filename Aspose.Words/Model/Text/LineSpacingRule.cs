// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Specifies line spacing values for a paragraph.
    /// </summary>
    [CppEnumEnableMetadata]
    public enum LineSpacingRule
    {
        /// <summary>
        /// The line spacing can be greater than or equal to, but never less than,
        /// the value specified in the <see cref="ParagraphFormat.LineSpacing"/> property.
        /// </summary>
        AtLeast,
        /// <summary>
        /// The line spacing never changes from the value specified in the
        /// <see cref="ParagraphFormat.LineSpacing"/> property,
        /// even if a larger font is used within the paragraph.
        /// </summary>
        Exactly,
        /// <summary>
        /// The line spacing is specified in the <see cref="ParagraphFormat.LineSpacing"/>
        /// property as the number of lines. One line equals 12 points.
        /// </summary>
        Multiple
    }
}
