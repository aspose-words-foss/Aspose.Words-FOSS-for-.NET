// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown AutoLink closing delimiter.
    /// </summary>
    internal class AutoLinkClosingDelimiter : AutoLinkDelimiter
    {
        /// <summary>
        /// Creates an AutoLinkClosingDelimiter object from a delimiter run.
        /// </summary>
        internal AutoLinkClosingDelimiter(DelimiterRun delimiterRun, int offset) : base(delimiterRun, offset)
        {
        }

        /// <summary>
        /// Gets a delimiter that can be an Opening for this delimiter.
        /// </summary>
        internal override Delimiter GetPotentialOpeningBackward()
        {
            Delimiter curDelimiter = (Delimiter)PrevNode;
            while (curDelimiter != null)
            {
                // Skip removed delimiters.
                if (IsNotIncluded)
                {
                    curDelimiter = (Delimiter)curDelimiter.PrevNode;
                    continue;
                }

                // WORDSNET-20703 There cannot be nested AutoLinks by spec, so check it and exit if found.     
                if (curDelimiter.Type == DelimiterType.AutoLinkClosing)
                    return null;

                // Jump over linked delimiters with the same or greater priority.
                if ((curDelimiter.IsLinked && curDelimiter.IsClosing) && (Priority <= curDelimiter.Priority))
                {
                    Delimiter linkedDelimiter = curDelimiter.LinkedDelimiter;
                    while (curDelimiter != linkedDelimiter)
                    {
                        // There cannot be nested AutoLinks by spec, so check it and exit if found.     
                        if (curDelimiter.Type == DelimiterType.AutoLinkClosing)
                            return null;

                        curDelimiter = (Delimiter)curDelimiter.PrevNode;
                    }
                }
                else
                {
                    if (curDelimiter.CanBeOpeningFor(this))
                        return curDelimiter;
                }

                curDelimiter = (Delimiter)curDelimiter.PrevNode;
            }

            return null;
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
            get { return DelimiterType.AutoLinkClosing; }
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
        internal const char Character = '>';
    }
}
