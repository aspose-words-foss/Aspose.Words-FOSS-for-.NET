// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown AutoLink opening delimiter.
    /// </summary>
    internal class AutoLinkOpeningDelimiter : AutoLinkDelimiter
    {
        /// <summary>
        /// Creates an AutoLinkOpeningDelimiter object from a delimiter run.
        /// </summary>
        internal AutoLinkOpeningDelimiter(DelimiterRun delimiterRun, int offset) : base(delimiterRun, offset)
        {
        }

        /// <summary>
        /// Gets a delimiter that can be an Opening for this delimiter.
        /// </summary>
        internal override Delimiter GetPotentialOpeningBackward()
        {
            return null;
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter can be Opening for a specified one. 
        /// </summary>
        internal override bool CanBeOpeningFor(Delimiter other)
        {
            if (IsNotIncluded)
                return false;

            if (other.Type != DelimiterType.AutoLinkClosing)
                return false;

            return AutolinkInlineBlock.IsValid(this, other);
        }

        /// <summary>
        /// Gets type of the delimiter.
        /// </summary>
        internal override DelimiterType Type
        {
            get { return DelimiterType.AutoLinkOpening; }
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
        internal const char Character = '<';
    }
}
