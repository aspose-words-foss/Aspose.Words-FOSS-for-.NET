// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown LinkText opening delimiter.
    /// </summary>
    internal class LinkTextOpeningDelimiter : LinkTextDelimiter
    {
        /// <summary>
        /// Creates a LinkTextOpeningDelimiter object from a delimiter run.
        /// </summary>
        internal LinkTextOpeningDelimiter(DelimiterRun delimiterRun, int offset) : base(delimiterRun, offset)
        {
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter can be Opening for a specified one. 
        /// </summary>
        internal override bool CanBeOpeningFor(Delimiter other)
        {
            if (other.Type != DelimiterType.LinkTextClosing)
                return false;

            if (IsNotIncluded)
                return false;

            if (IsImageDescription)
                return false;

            // There cannot be nested links by spec.
            if (MarkdownUtil.GetLinkedInRange(DelimiterType.LinkTextOpening, this, other) != null)
                return false;

            return LinkTextBlock.IsValid(this, other);
        }
        
        /// <summary>
        /// Gets type of the delimiter.
        /// </summary>
        internal override DelimiterType Type
        {
            get { return DelimiterType.LinkTextOpening; }
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter can be a Closing.
        /// </summary>
        protected override bool CanBeClosing
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a boolean value indicating whether this LinkText is actually a part of ImageDescription.
        /// </summary>
        private bool IsImageDescription
        {
            get
            {
                if ((PrevNode == null) || PrevNode.IsNotIncluded)
                    return false;

                Delimiter prevDelimiter = (Delimiter)PrevNode;
                if (prevDelimiter.Type != DelimiterType.ImageDescriptionOpening)
                    return false;

                return ((prevDelimiter.End + 1) == Start);
            }
        }
        
        /// <summary>
        /// Gets a corresponding character of the delimiter.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char Character = '[';
    }
}
