// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown Emphases delimiter.
    /// </summary>
    internal abstract class EmphasesDelimiter : EmphasesDelimiterBase
    {
        /// <summary>
        /// Creates a EmphasesDelimiter object from a delimiter run.
        /// </summary>
        protected EmphasesDelimiter(DelimiterRun delimiterRun, int offset) : base(delimiterRun, offset)
        {
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter can be Opening for a specified one. 
        /// </summary>
        internal override bool CanBeOpeningFor(Delimiter other)
        {
            if (!base.CanBeOpeningFor(other))
                return false;

            // If one of the delimiters can both open and close emphasis, then the sum of the lengths of 
            // the delimiter runs containing the opening and closing delimiters must not be a multiple 
            // of 3 (rule #9 of the spec). 
            return (((FlankingType != FlankingType.Both) && (other.FlankingType != FlankingType.Both)) ||
                    (((Length + other.Length) % 3) != 0));
        }

        /// <summary>
        /// Creates a corresponding markdown inline block. 
        /// </summary>
        internal override Block ToBlock()
        {
            return (Length == 1) ? (Block)new ItalicInlineBlock() : new BoldInlineBlock();
        }

        /// <summary>
        /// Gets an integer value representing a maximum allowed length of the delimiter in a linked state.
        /// </summary>
        protected override int MaxLinkedLength
        {
            get { return 2; }
        }
    }
}
