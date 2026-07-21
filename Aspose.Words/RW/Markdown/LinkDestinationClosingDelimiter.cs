// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown LinkDestination closing delimiter.
    /// </summary>
    internal class LinkDestinationClosingDelimiter : LinkDestinationDelimiter
    {
        /// <summary>
        /// Creates a LinkDestinationClosingDelimiter object from a delimiter run.
        /// </summary>
        internal LinkDestinationClosingDelimiter(DelimiterRun delimiterRun, int offset) : base(delimiterRun, offset)
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
            get { return DelimiterType.LinkDestinationClosing; }
        }

        /// <summary>
        /// Gets a boolean value indicating whether it has a corresponding LinkText.
        /// </summary>
        internal override bool HasLinkText
        {
            get { return (IsLinked && ((LinkDestinationDelimiter)LinkedDelimiter).HasLinkText); }
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
        internal const char Character = ')';
    }
}
