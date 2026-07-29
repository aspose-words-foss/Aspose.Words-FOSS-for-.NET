// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown ImageDescription opening delimiter.
    /// </summary>
    internal class ImageDescriptionOpeningDelimiter : LinkTextDelimiter
    {
        /// <summary>
        /// Creates a LinkTextOpeningDelimiter object from a delimiter run.
        /// </summary>
        internal ImageDescriptionOpeningDelimiter(DelimiterRun delimiterRun, int offset) : base(delimiterRun, offset)
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

            return ImageDescriptionBlock.IsValid(this, other);
        }

        /// <summary>
        /// Gets type of the delimiter.
        /// </summary>
        internal override DelimiterType Type
        {
            get { return DelimiterType.ImageDescriptionOpening; }
        }

        /// <summary>
        /// Creates a corresponding markdown inline block. 
        /// </summary>
        internal override Block ToBlock()
        {
            return new ImageDescriptionBlock();
        }

        /// <summary>
        /// Gets a corresponding LinkText opening delimiter.
        /// </summary>
        internal LinkTextOpeningDelimiter LinkTextOpening
        {
            get
            {
                Delimiter linkTextOpening = (Delimiter)NextNode;
                if ((linkTextOpening == null) || linkTextOpening.IsNotIncluded)
                    return null;

                if (linkTextOpening.Type != DelimiterType.LinkTextOpening)
                    return null;

                if ((linkTextOpening.Start - 1) != End)
                    return null;

                return (LinkTextOpeningDelimiter)linkTextOpening;
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
        internal const char Character = '!';
    }
}
