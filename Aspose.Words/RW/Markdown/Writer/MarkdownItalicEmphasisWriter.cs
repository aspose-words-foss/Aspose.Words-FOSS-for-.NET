// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/06/2020 by Ilya Navrotskiy

using Aspose.Words.RW.Txt.Writer;

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// The class responsible for writing Italic emphasis into Markdown.
    /// </summary>
    internal class MarkdownItalicEmphasisWriter : MarkdownEmphasisWriterBase
    {
        /// <summary>
        /// Initializes a new instance with the specified content lines and IRunAttrSource object.
        /// </summary>
        internal MarkdownItalicEmphasisWriter(TxtContentLines contentLines, IRunAttrSource src)
            : base(contentLines)
        {
            IsEmphasized = (src != null) && InlineHelper.GetBool(src, FontAttr.Italic);
        }

        /// <summary>
        /// Switches delimiter string to underscores.
        /// </summary>
        internal override bool SwitchToUnderscore()
        {
            // The underscore delimiter cannot be set if flanking of opening delimiter is not strong Left.
            if ((OpeningFlanking != FlankingType.Left) || UseHtmlSyntax)
                return false;

            if (mDelimiter != ItalicInlineBlock.UnderscoreDelimiter)
            {
                mDelimiter = ItalicInlineBlock.UnderscoreDelimiter;
                ReplaceOpening(mDelimiter);
            }

            return true;
        }

        /// <summary>
        /// Gets a boolean value indicating whether the emphasis is applied to the specified run source.
        /// </summary>
        protected override bool GetIsEmphasized(IRunAttrSource src)
        {
            return (src != null) && InlineHelper.GetBool(src, FontAttr.Italic);
        }

        /// <summary>
        /// Gets a string value representing delimiter of the emphasis.
        /// </summary>
        protected override string Delimiter
        {
            get
            {
                return (UseHtmlSyntax)
                    ? "<i>"
                    : mDelimiter;
            }
        }

        /// <summary>
        /// Gets a string value representing the emphasis closing delimiter.
        /// </summary>
        protected override string CloseDelimiter
        {
            get
            {
                return (UseHtmlSyntax)
                    ? "</i>"
                    : mDelimiter;
            }
        }

        private string mDelimiter = ItalicInlineBlock.AsteriskDelimiter;
    }
}
