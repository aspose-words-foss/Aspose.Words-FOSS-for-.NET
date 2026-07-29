// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/03/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies a token <see cref="ResultEnumerator"/> that may be encountered during result enumeration.
    /// </summary>
    internal enum ResultToken
    {
        /// <summary>
        /// The token is undefined.
        /// </summary>
        Undefined,
        /// <summary>
        /// Letter, digit or one of special characters.
        /// </summary>
        WordCharacter,
        /// <summary>
        /// The ' ' character, other whitespace seem to be considered a separator.
        /// </summary>
        Space,
        /// <summary>
        /// Any other character.
        /// </summary>
        Separator,
        /// <summary>
        /// A paragraph break.
        /// </summary>
        PargaraphBreak,
        /// <summary>
        /// A table
        /// </summary>
        Table
    }
}
