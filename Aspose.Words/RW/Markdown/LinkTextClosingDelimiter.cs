// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown LinkText closing delimiter.
    /// </summary>
    internal class LinkTextClosingDelimiter : LinkTextDelimiter
    {
        /// <summary>
        /// Creates a LinkTextClosingDelimiter object from a delimiter run.
        /// </summary>
        internal LinkTextClosingDelimiter(DelimiterRun delimiterRun, int offset) : base(delimiterRun, offset)
        {
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter can be Opening for a specified one.
        /// </summary>
        internal override bool CanBeOpeningFor(Delimiter other)
        {
            return false;
        }

        /// <summary>
        /// Gets type of the delimiter.
        /// </summary>
        internal override DelimiterType Type
        {
            get { return DelimiterType.LinkTextClosing; }
        }


        /// <summary>
        /// Gets a boolean value indicating whether it has a corresponding LinkDestination.
        /// </summary>
        internal bool HasLinkDestination
        {
            get
            {
                if ((NextNode == null) || NextNode.IsNotIncluded)
                    return false;

                Delimiter nextDelimiter = (Delimiter)NextNode;
                if ((nextDelimiter.Type != DelimiterType.LinkDestinationOpening) || !nextDelimiter.IsLinked)
                    return false;

                return ((nextDelimiter.Start - 1) == End);
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether it has a corresponding reference LinkText.
        /// </summary>
        internal bool HasReferenceLinkText
        {
            get
            {
                if ((NextNode == null) || NextNode.IsNotIncluded)
                    return false;

                Delimiter nextDelimiter = (Delimiter)NextNode;
                if ((nextDelimiter.Type != DelimiterType.LinkTextOpening) || !nextDelimiter.IsLinked)
                    return false;

                return ((nextDelimiter.Start - 1) == End);
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether it has a corresponding collapsed LinkText.
        /// </summary>
        internal bool HasCollapsedLinkText
        {
            get
            {
                if ((PrevNode == null) || PrevNode.IsNotIncluded || (PrevNode.PrevNode == null))
                    return false;

                Delimiter prevDelimiter = (Delimiter)PrevNode.PrevNode;
                if ((prevDelimiter.Type != DelimiterType.LinkTextClosing) || !prevDelimiter.IsLinked)
                    return false;

                return ((prevDelimiter.End + 2) == End);
            }
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter can be a Closing.
        /// </summary>
        protected override bool CanBeClosing
        {
            get { return !IsNotIncluded; }
        }

        /// <summary>
        /// Gets a corresponding character of the delimiter.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char Character = ']';
    }
}
