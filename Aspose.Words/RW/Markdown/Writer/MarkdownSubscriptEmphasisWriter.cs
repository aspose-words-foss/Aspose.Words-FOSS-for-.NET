// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2023 by Vadim Saltykov

using Aspose.Words.RW.Txt.Writer;

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// The class responsible for writing Subscript emphasis into Markdown.
    /// </summary>
    internal class MarkdownSubscriptEmphasisWriter : MarkdownEmphasisWriterBase
    {
        /// <summary>
        /// Initializes a new instance with the specified content lines and IRunAttrSource object.
        /// </summary>
        internal MarkdownSubscriptEmphasisWriter(TxtContentLines contentLines, IRunAttrSource src)
            : base(contentLines)
        {
            IsEmphasized = HasSubscriptAttr(src);
        }

        /// <summary>
        /// Gets a boolean value indicating whether the emphasis is applied to the specified run source.
        /// </summary>
        protected override bool GetIsEmphasized(IRunAttrSource src)
        {
            return HasSubscriptAttr(src);
        }

        /// <summary>
        /// Returns True if the corresponding source contains Subscript direct attribute.
        /// </summary>
        private static bool HasSubscriptAttr(IRunAttrSource src)
        {
            if (src == null)
                return false;

            object value = src.GetDirectRunAttr(FontAttr.VerticalAlignment);
            return (value != null) && ((RunVerticalAlignment)value == RunVerticalAlignment.Subscript);
        }

        /// <summary>
        /// Gets a string value representing delimiter of the emphasis.
        /// </summary>
        protected override string Delimiter
        {
            get { return "<sub>"; }
        }

        /// <summary>
        /// Gets a string value representing the emphasis closing delimiter.
        /// </summary>
        protected override string CloseDelimiter
        {
            get { return "</sub>"; }
        }
    }
}
