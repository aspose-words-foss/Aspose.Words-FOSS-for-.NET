// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown LinkDestination opening delimiter.
    /// </summary>
    internal class LinkDestinationOpeningDelimiter : LinkDestinationDelimiter
    {
        /// <summary>
        /// Creates a LinkDestinationOpeningDelimiter object from a delimiter run.
        /// </summary>
        internal LinkDestinationOpeningDelimiter(DelimiterRun delimiterRun, int offset) : base(delimiterRun, offset)
        {
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter can be Opening for a specified one. 
        /// </summary>
        internal override bool CanBeOpeningFor(Delimiter other)
        {
            if (other.Type != DelimiterType.LinkDestinationClosing)
                return false;

            if (IsLinked)
                return false;

            if (!LinkDestinationBlock.IsValid(this, other))
                return false;

            Delimiter linkTextClosing = GetLinkTextClosing();
            if (linkTextClosing == null)
                return false;

            if (linkTextClosing.IsLinked)
                return true;

            // Try to Link temporary this LinkDestination to raise priority of the
            // corresponding LinkText and check either it can be linked as well.
            Link(other);
            Delimiter linkTextOpening = linkTextClosing.GetPotentialOpeningBackward();
            UnLink();

            return (linkTextOpening != null);
        }

        /// <summary>
        /// Gets type of the delimiter.
        /// </summary>
        internal override DelimiterType Type
        {
            get { return DelimiterType.LinkDestinationOpening; }
        }

        /// <summary>
        /// Gets a boolean value indicating whether it has a corresponding linked LinkText.
        /// </summary>
        internal override bool HasLinkText
        {
            get
            {
                Delimiter linkTextClosing = GetLinkTextClosing();
                return ((linkTextClosing != null) && (linkTextClosing.IsLinked));
            }
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
        internal const char Character = '(';
    }
}
