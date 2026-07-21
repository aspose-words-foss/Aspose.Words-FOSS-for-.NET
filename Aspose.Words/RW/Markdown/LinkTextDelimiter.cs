// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown LinkText delimiter.
    /// </summary>
    internal abstract class LinkTextDelimiter : Delimiter
    {
        /// <summary>
        /// Creates a LinkTextDelimiter object from a delimiter run.
        /// </summary>
        protected LinkTextDelimiter(DelimiterRun delimiterRun, int offset) : base(delimiterRun, offset, 1)
        {
        }

        /// <summary>
        /// Creates a corresponding markdown inline block.
        /// </summary>
        internal override Block ToBlock()
        {
            return new LinkTextBlock();
        }

        /// <summary>
        /// Gets a priority of the delimiter.
        /// </summary>
        internal override DelimiterPriority Priority
        {
            get
            {
                return DelimiterPriority.LinkText;
            }
        }

        /// <summary>
        /// Gets a priority to unlink inside linked delimiters.
        /// </summary>
        /// <remarks>
        /// This is a priority of a linked delimiter below which there can be no other linked delimiters inside.
        /// </remarks>
        internal override DelimiterPriority UnlinkPriority
        {
            get { return DelimiterPriority.Highest; }
        }

        /// <summary>
        /// Gets an integer value representing a maximum allowed length of the delimiter in a linked state.
        /// </summary>
        protected override int MaxLinkedLength
        {
            get { return 1; }
        }
    }
}
