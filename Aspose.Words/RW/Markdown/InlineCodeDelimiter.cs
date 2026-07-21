// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown InlineCode delimiter.
    /// </summary>
    internal class InlineCodeDelimiter : Delimiter
    {
        /// <summary>
        /// Creates an InlineCodeDelimiter object from a delimiter run.
        /// </summary>
        public InlineCodeDelimiter(DelimiterRun delimiterRun, int offset) : base(delimiterRun, offset, MaxLength)
        {
        }

        /// <summary>
        /// Creates a corresponding markdown inline block.
        /// </summary>
        internal override Block ToBlock()
        {
            return new InlineCodeBlock(Length);
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter can be Opening for a specified one.
        /// </summary>
        internal override bool CanBeOpeningFor(Delimiter other)
        {
            if (Type != other.Type)
                return false;

            if (IsNotIncluded)
                return false;

            // The lengths of opening and closing sequences of InlineCode delimiters must be equal.
            if (Length != other.Length)
                return false;

            // The opening of the InlineCode block must be unescaped.
            return (!IsEscaped);
        }

        /// <summary>
        /// Gets type of the delimiter.
        /// </summary>
        internal override DelimiterType Type
        {
            get { return DelimiterType.InlineCode; }
        }

        /// <summary>
        /// Gets a priority of the delimiter.
        /// </summary>
        internal override DelimiterPriority Priority
        {
            get { return DelimiterPriority.InlineCode; }
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
        /// Gets a boolean value indicating the delimiter can be a Closing.
        /// </summary>
        protected override bool CanBeClosing
        {
            get { return !IsNotIncluded; }
        }

        /// <summary>
        /// Gets an integer value representing a maximum allowed length of the delimiter in a linked state.
        /// </summary>
        protected override int MaxLinkedLength
        {
            get { return int.MaxValue; }
        }

        /// <summary>
        /// Gets a corresponding character of the delimiter.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char Character = '`';

        /// <summary>
        /// Gets an integer value representing a maximum allowed length of the delimiter.
        /// </summary>
        private const int MaxLength = int.MaxValue;
    }
}
