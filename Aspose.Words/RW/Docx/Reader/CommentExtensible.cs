// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/09/2020 by Alexander Zhiltsov

using System;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Utility class to store comment information related to commentsIds and commentsExtensible document parts.
    /// </summary>
    internal class CommentExtensible
    {
        /// <summary>
        /// Ctor to create instance of this class.
        /// </summary>
        internal CommentExtensible(int paraId, int durableId)
        {
            ParaId = paraId;
            DurableId = durableId;
        }

        /// <summary>
        /// Paragraph Id of last paragraph of the comment.
        /// </summary>
        internal int ParaId { get; set; }

        /// <summary>
        /// Comment durable identifier.
        /// </summary>
        internal int DurableId { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time that the comment was made.
        /// </summary>
        internal DateTime UtcDateTime
        {
            get { return mUtcDateTime; }
            set { mUtcDateTime = value; }
        }

        /// <summary>
        /// When <c>true</c> specifies that the comment is a follow-up.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// The attribute MUST NOT be present on comments that are replies as specified by the paraIdParent attribute
        /// of an associated commentEx element.
        /// When the property is <c>true</c>, the Office 365 version of Word ignores the content of the comment. Other
        /// versions of Word treat the comment as a regular comment.
        /// </remarks>
        internal bool IsIntelligentPlaceholder { get; set; }

        /// <summary>
        /// Gets or sets a collection of reactions to the comment.
        /// </summary>
        internal CommentReactionCollection Reactions { get; set; }

        private DateTime mUtcDateTime = DateTime.MinValue; // Init for Java.
    }
}
