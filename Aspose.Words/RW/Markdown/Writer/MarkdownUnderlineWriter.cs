// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/11/2023 by Ilya Navrotskiy

using Aspose.Words.RW.Txt.Writer;

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// The class responsible for writing Underline emphasis into Markdown.
    /// </summary>
    internal class MarkdownUnderlineWriter : MarkdownEmphasisWriterBase
    {
        /// <summary>
        /// Initializes a new instance with the specified content lines and IRunAttrSource object.
        /// </summary>
        internal MarkdownUnderlineWriter(TxtContentLines contentLines, IRunAttrSource src)
            : base(contentLines)
        {
            IsEmphasized = (src != null) && (Underline)InlineHelper.FetchAttr(src, FontAttr.Underline) != Underline.None;
        }

        /// <summary>
        /// Gets a boolean value indicating whether the emphasis is applied to the specified run source.
        /// </summary>
        protected override bool GetIsEmphasized(IRunAttrSource src)
        {
            return (src != null) && (Underline)InlineHelper.FetchAttr(src, FontAttr.Underline) != Underline.None;
        }

        /// <summary>
        /// Gets a string value representing delimiter of the emphasis.
        /// </summary>
        protected override string Delimiter
        {
            get
            {
                return (UseHtmlSyntax)
                    ? "<u>"
                    : UnderlineInlineBlock.Delimiter;
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
                    ? "</u>"
                    : UnderlineInlineBlock.Delimiter;
            }
        }
    }
}
