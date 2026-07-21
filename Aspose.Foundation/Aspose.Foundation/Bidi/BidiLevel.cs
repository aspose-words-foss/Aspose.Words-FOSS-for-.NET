// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2014 by Victor Chebotok

namespace Aspose.Bidi
{
    /// <summary>
    /// Embeddings, overrides and isolations that control how text is processed by the Unicode Bidi Algorithm.
    /// </summary>
    /// <remarks>
    /// Items of this enumeration correspond to different combinations of 
    /// the 'unicode-bidi' (see http://www.w3.org/TR/CSS2/visuren.html#propdef-unicode-bidi)
    /// and the 'direction' (see http://www.w3.org/TR/CSS2/visuren.html#propdef-direction) CSS properties.
    /// Only active (non-normal) levels supported by our implementation of the bidirectional algorithm are included
    /// in this enumeration.
    /// </remarks>
    public enum BidiLevel
    {
        /// <summary>
        /// An additional level of left-to-right embedding is opened for the text.
        /// </summary>
        EmbedLtr,
        /// <summary>
        /// An additional level of right-to-left embedding is opened for the text.
        /// </summary>
        EmbedRtl
    }
}
