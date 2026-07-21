// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/06/2006 by Roman Korchagin

namespace Aspose.Words.Lists
{
    /// <summary>
    /// Specifies the character that separates the list label from the text of the paragraph.
    /// </summary>
    /// <remarks>
    /// <p>Used as a value for the <see cref="ListLevel.TrailingCharacter"/> property.</p>
    /// </remarks>
    public enum ListTrailingCharacter
    {
        /// <summary>
        /// A tab character is placed between the list label and text of the paragraph.
        /// </summary>
        Tab = 0,
        /// <summary>
        /// A space character is placed between the list label and text of the paragraph.
        /// </summary>
        Space = 1,
        /// <summary>
        /// There is no separator character between the list label and text of the paragraph.
        /// </summary>
        Nothing = 2
    }
}
