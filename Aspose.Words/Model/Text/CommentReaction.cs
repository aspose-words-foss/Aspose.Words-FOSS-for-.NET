// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2024 by Alexander Zhiltsov

namespace Aspose.Words
{
    /// <summary>
    /// Represents a 'reaction' element of the CT_CommentReaction complex type defined in §2.1.3.1 of [MS-OREACTXML]
    /// that specifies information for a single reaction type.
    /// </summary>
    internal class CommentReaction
    {
        internal CommentReaction()
        {
            ReactionInfos = new CommentReactionInfoCollection();
        }

        /// <summary>
        /// Specifies the type of a reaction.
        /// </summary>
        /// <remarks>
        /// Values must be greater than 0 and less than 2147483648. The value 1 represents a Like (Thumbs-Up).
        /// </remarks>
        internal int ReactionType { get; set; }

        /// <summary>
        /// Specifies information about the reaction users and time of the reactions.
        /// </summary>
        /// <remarks>
        /// The collection should contain records with unique <see cref="CommentReactionInfo.UserId"/>.
        /// </remarks>
        internal CommentReactionInfoCollection ReactionInfos { get; set; }
    }
}
