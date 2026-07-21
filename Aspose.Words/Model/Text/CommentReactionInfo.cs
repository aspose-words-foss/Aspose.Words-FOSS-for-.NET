// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2024 by Alexander Zhiltsov

using System;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a 'reactionInfo' element of the CT_CommentReactionInfo complex type defined in §2.1.3.2 of
    /// [MS-OREACTXML] that specifies the user who added the reaction and the time when the reaction was added.
    /// </summary>
    internal class CommentReactionInfo
    {
        /// <summary>
        /// Specifies the provider issued ID for the user.
        /// </summary>
        internal string UserId { get; set; }

        /// <summary>
        /// Specifies the display name of the user.
        /// </summary>
        internal string UserName { get; set; }

        /// <summary>
        /// Specifies the provider that produced the <see cref="UserId"/>.
        /// </summary>
        internal string UserProvider { get; set; }

        /// <summary>
        /// Specifies date information when the reaction was added, the value is defined to be in the UTC time zone.
        /// </summary>
        internal DateTime DateTimeUtc { get; set; }
    }
}
