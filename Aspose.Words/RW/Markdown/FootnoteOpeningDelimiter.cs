// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2021 by Mikhail Nepreteamov

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown Footnote opening delimiter.
    /// </summary>
    internal class FootnoteOpeningDelimiter : LinkTextDelimiter
    {
        /// <summary>
        /// Creates a FootnoteOpeningDelimiter object from a delimiter run.
        /// </summary>
        internal FootnoteOpeningDelimiter(DelimiterRun delimiterRun, int offset) : base(delimiterRun, offset)
        {
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter can be Opening for a specified one.
        /// </summary>
        internal override bool CanBeOpeningFor(Delimiter other)
        {
            return ((other != null) && (other.Type == DelimiterType.LinkTextClosing) && !IsNotIncluded &&
                FootnoteReferenceBlock.IsValid(this, other));
        }

        /// <summary>
        /// Creates a corresponding markdown inline block.
        /// </summary>
        internal override Block ToBlock()
        {
            return new FootnoteReferenceBlock();
        }

        /// <summary>
        /// Gets sibling LinkText opening delimiter just before this Footnote delimiter,
        /// or <c>null</c> if there is no such delimiter.
        /// </summary>
        internal LinkTextOpeningDelimiter LinkTextOpening
        {
            get
            {
                Delimiter linkTextOpening = (Delimiter)PrevNode;

                if ((linkTextOpening == null) || linkTextOpening.IsNotIncluded)
                    return null;

                if (linkTextOpening.Type != DelimiterType.LinkTextOpening)
                    return null;

                if ((linkTextOpening.Start + 1) != End)
                    return null;

                return (LinkTextOpeningDelimiter)linkTextOpening;
            }
        }

        /// <summary>
        /// Gets type of the delimiter.
        /// </summary>
        internal override DelimiterType Type
        {
            get { return DelimiterType.FootnoteOpening; }
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter can be a Closing.
        /// </summary>
        protected override bool CanBeClosing
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a corresponding character of the delimiter.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char Character = '^';
    }
}
