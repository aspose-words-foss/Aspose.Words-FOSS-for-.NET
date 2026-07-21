// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown Emphases delimiter.
    /// </summary>
    internal abstract class EmphasesDelimiterBase : Delimiter
    {
        /// <summary>
        /// Creates a EmphasesDelimiter object from a delimiter run.
        /// </summary>
        protected EmphasesDelimiterBase(DelimiterRun delimiterRun, int offset) : base(delimiterRun, offset, MaxLength)
        {
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter can be Opening for a specified one.
        /// </summary>
        internal override bool CanBeOpeningFor(Delimiter other)
        {
            if (IsNotIncluded)
                return false;

            if (Type != other.Type)
                return false;

            return ((FlankingType == FlankingType.Left) || (FlankingType == FlankingType.Both));
        }

        /// <summary>
        /// Gets a boolean value indicating whether the delimiter is valid.
        /// </summary>
        internal override bool IsValid
        {
            get { return (FlankingType != FlankingType.None); }
        }

        /// <summary>
        /// Gets a priority of the delimiter.
        /// </summary>
        internal override DelimiterPriority Priority
        {
            get { return DelimiterPriority.Emphases; }
        }

        /// <summary>
        /// Gets a priority to unlink inside linked delimiters.
        /// </summary>
        /// <remarks>
        /// This is a priority of a linked delimiter below which there can not be other linked delimiters inside.
        /// </remarks>
        internal override DelimiterPriority UnlinkPriority
        {
            get { return DelimiterPriority.Lowest; }
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter can be a Closing.
        /// </summary>
        protected override bool CanBeClosing
        {
            get
            {
                if (IsNotIncluded)
                    return false;

                return ((FlankingType == FlankingType.Right) || (FlankingType == FlankingType.Both));
            }
        }

        /// <summary>
        /// Gets an integer value representing a maximum allowed length of the delimiter.
        /// </summary>
        private const int MaxLength = int.MaxValue;
    }
}
