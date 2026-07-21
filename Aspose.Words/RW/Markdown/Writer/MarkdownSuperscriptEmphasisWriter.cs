// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2023 by Vadim Saltykov

using Aspose.Words.RW.Txt.Writer;

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// The class responsible for writing Superscript emphasis into Markdown.
    /// </summary>
    internal class MarkdownSuperscriptEmphasisWriter : MarkdownEmphasisWriterBase
    {
        /// <summary>
        /// Initializes a new instance with the specified content lines and IRunAttrSource object.
        /// </summary>
        internal MarkdownSuperscriptEmphasisWriter(TxtContentLines contentLines, IRunAttrSource src)
            : base(contentLines)
        {
            IsEmphasized = HasSuperscriptAttr(src);
        }

        /// <summary>
        /// Gets a boolean value indicating whether the emphasis is applied to the specified run source.
        /// </summary>
        protected override bool GetIsEmphasized(IRunAttrSource src)
        {
            return HasSuperscriptAttr(src);
        }

        /// <summary>
        /// Returns True if the corresponding source contains Superscript direct attribute.
        /// </summary>
        private static bool HasSuperscriptAttr(IRunAttrSource src)
        {
            if (src == null)
                return false;

            object value = src.GetDirectRunAttr(FontAttr.VerticalAlignment);
            return (value != null) && ((RunVerticalAlignment)value == RunVerticalAlignment.Superscript);
        }

        /// <summary>
        /// Gets a string value representing delimiter of the emphasis.
        /// </summary>
        protected override string Delimiter
        {
            get { return "<sup>"; }
        }

        /// <summary>
        /// Gets a string value representing the emphasis closing delimiter.
        /// </summary>
        protected override string CloseDelimiter
        {
            get { return "</sup>"; }
        }
    }
}
