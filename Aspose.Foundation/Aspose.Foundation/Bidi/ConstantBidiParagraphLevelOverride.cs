// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

using System.Collections.Generic;

namespace Aspose.Bidi
{
    /// <summary>
    /// Represents BIDI paragraph embedding level override by constant value for all paragraphs created during
    /// single <see cref="BidiParagraph.SplitStringToParagraphs(string)"/> method call.
    /// </summary>
    public sealed class ConstantBidiParagraphLevelOverride : IBidiParagraphLevelOverride
    {
        /// <summary>
        /// Ctor hidden from using outside.
        /// </summary>
        private ConstantBidiParagraphLevelOverride(int embeddingLevel)
        {
            mEmbeddingLevel = embeddingLevel;
        }

        /// <summary>
        /// Returns one of the multiton instances of this class depending on the specified BIDI embedding level.
        /// </summary>
        public static IBidiParagraphLevelOverride GetInstance(bool isRtlEmbedding)
        {
            return (isRtlEmbedding ? gRtlInstance : gLtrInstance);
        }

        int IBidiParagraphLevelOverride.GetEmbeddingLevel(IList<BidiParagraph> previousParagraphs, string paragraphText)
        {
            return mEmbeddingLevel;
        }

        private readonly int mEmbeddingLevel;

        // Multiton instances.
        private static readonly IBidiParagraphLevelOverride gLtrInstance = new ConstantBidiParagraphLevelOverride(0);
        private static readonly IBidiParagraphLevelOverride gRtlInstance = new ConstantBidiParagraphLevelOverride(1);
    }
}
