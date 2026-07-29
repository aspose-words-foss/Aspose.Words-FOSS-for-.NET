// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Specifies text alignment in a paragraph.
    /// </summary>
    [CppEnumEnableMetadata]
    public enum ParagraphAlignment
    {
        /// <summary>
        /// Text is aligned to the left.
        /// </summary>
        Left = 0,

        /// <summary>
        /// Text is centered horizontally.
        /// </summary>
        Center = 1,

        /// <summary>
        /// Text is aligned to the right.
        /// </summary>
        Right = 2,

        /// <summary>
        /// Text is aligned to both left and right.
        /// </summary>
        Justify = 3,

        /// <summary>
        /// Text is evenly distributed.
        /// </summary>
        Distributed = 4,

        /// <summary>
        /// Arabic only. Kashida length for text is extended to a medium length determined by the consumer.
        /// </summary>
        ArabicMediumKashida = 5,

        /// <summary>
        /// Arabic only. Kashida length for text is extended to its widest possible length.
        /// </summary>
        ArabicHighKashida = 7,

        /// <summary>
        /// Arabic only. Kashida length for text is extended to a slightly longer length.
        /// </summary>
        ArabicLowKashida = 8,

        /// <summary>
        /// Thai only. Text is justified with an optimization for Thai.
        /// </summary>
        ThaiDistributed = 9,

        /// <summary>
        /// The only Math element in a line, aligned as 'Centered As Group'.
        /// </summary>
        MathElementCenterAsGroup = 10
    }
}
