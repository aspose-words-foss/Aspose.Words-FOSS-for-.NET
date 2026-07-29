// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/07/2015 by Edward Voronov

using System;

namespace Aspose.Words.Notes
{
    /// <summary>
    /// Provides footnote utility functions.
    /// </summary>
    internal static class FootnoteUtil
    {
        /// <summary>
        /// Returns "reference" <see cref="StyleIdentifier"/> corresponding to a <see cref="Footnote"/> type.
        /// </summary>
        internal static StyleIdentifier GetFootnoteReferenceStyleIdentifier(Footnote footnote)
        {
            return GetFootnoteReferenceStyleIdentifier(footnote.FootnoteType);
        }

        /// <summary>
        /// Returns "reference" <see cref="StyleIdentifier"/> corresponding to a <see cref="FootnoteType"/>.
        /// </summary>
        internal static StyleIdentifier GetFootnoteReferenceStyleIdentifier(FootnoteType footnoteType)
        {
            switch (footnoteType)
            {
                case FootnoteType.Footnote:
                    return StyleIdentifier.FootnoteReference;
                case FootnoteType.Endnote:
                    return StyleIdentifier.EndnoteReference;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns "text" <see cref="StyleIdentifier"/> corresponding to a <see cref="Footnote"/> type.
        /// </summary>
        internal static StyleIdentifier GetFootnoteTextStyleIdentifier(Footnote footnote)
        {
            return GetFootnoteTextStyleIdentifier(footnote.FootnoteType);
        }

        /// <summary>
        /// Returns "text" <see cref="StyleIdentifier"/> corresponding to a <see cref="FootnoteType"/>.
        /// </summary>
        internal static StyleIdentifier GetFootnoteTextStyleIdentifier(FootnoteType footnoteType)
        {
            switch (footnoteType)
            {
                case FootnoteType.Footnote:
                    return StyleIdentifier.FootnoteText;
                case FootnoteType.Endnote:
                    return StyleIdentifier.EndnoteText;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
