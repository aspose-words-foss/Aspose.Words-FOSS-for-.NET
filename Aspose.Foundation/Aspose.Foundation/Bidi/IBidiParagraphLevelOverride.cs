// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

using System.Collections.Generic;
using Aspose.JavaAttributes;

namespace Aspose.Bidi
{
    /// <summary>
    /// Represents BIDI paragraph embedding level override for paragraphs created during
    /// single <see cref="BidiParagraph.SplitStringToParagraphs(string)"/> method call.
    /// </summary>
    public interface IBidiParagraphLevelOverride
    {
        /// <summary>
        /// When implemented, returns embedding level to apply to the given BIDI paragraph.
        /// </summary>
        /// <remarks>
        /// Your implementation should return <b>-1</b> to indicate that BIDI standart algorithm of paragraph
        /// embedding level definition must be used.
        /// </remarks>
        /// <param name="previousParagraphs">
        /// Previously created paragraphs during current <see cref="BidiParagraph.SplitStringToParagraphs(string)"/> call.
        /// Although it is possible, your implementation must not change contents of this list as if it was read-only.
        /// </param>
        /// <param name="paragraphText">Text of the given paragraph.</param>
        /// <returns>Embedding level to apply to the given paragraph.</returns>
        [JavaThrows(true)]
        int GetEmbeddingLevel(IList<BidiParagraph> previousParagraphs, string paragraphText);
    }
}
