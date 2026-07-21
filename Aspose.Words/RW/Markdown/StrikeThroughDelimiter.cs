// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown StrikeThrough delimiter.
    /// </summary>
    internal class StrikeThroughDelimiter : EmphasesDelimiterBase
    {
        /// <summary>
        /// Creates a StrikeThroughDelimiter object from a delimiter run.
        /// </summary>
        public StrikeThroughDelimiter(DelimiterRun delimiterRun, int offset) : base(delimiterRun, offset)
        {
        }

        /// <summary>
        /// Creates a corresponding markdown inline block. 
        /// </summary>
        internal override Block ToBlock()
        {
            return new StrikethroughInlineBlock();
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter can be Opening for a specified one. 
        /// </summary>
        internal override bool CanBeOpeningFor(Delimiter other)
        {
            // The length of Strikethrough delimiter cannot be less than 2.
            if ((Length < 2) || (other.Length < 2))
                return false;

            return base.CanBeOpeningFor(other);
        }

        /// <summary>
        /// Gets type of the delimiter.
        /// </summary>
        internal override DelimiterType Type
        {
            get { return DelimiterType.StrikeThrough; }
        }

        /// <summary>
        /// Gets an integer value representing a maximum allowed length of the delimiter in a linked state.
        /// </summary>
        protected override int MaxLinkedLength
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets a corresponding character of the delimiter.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char Character = '~';
    }
}
