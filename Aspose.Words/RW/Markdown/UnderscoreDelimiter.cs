// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown Underscore delimiter.
    /// </summary>
    internal class UnderscoreDelimiter : EmphasesDelimiter
    {
        /// <summary>
        /// Creates an UnderscoreDelimiter object from a delimiter run.
        /// </summary>
        public UnderscoreDelimiter(DelimiterRun delimiterRun, int offset) : base(delimiterRun, offset)
        {
        }

        /// <summary>
        /// Gets a boolean value indicating whether the delimiter is valid.
        /// </summary>
        internal override bool IsValid
        {
            get
            {
                // Underscore delimiters are not allowed intraword.
                if (IsIntraword)
                    return false;

                return base.IsValid;
            }
        }

        /// <summary>
        /// Gets type of the delimiter.
        /// </summary>
        internal override DelimiterType Type
        {
            get { return DelimiterType.Underscore; }
        }

        /// <summary>
        /// Gets a corresponding character of the delimiter.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char Character = '_';
    }
}
