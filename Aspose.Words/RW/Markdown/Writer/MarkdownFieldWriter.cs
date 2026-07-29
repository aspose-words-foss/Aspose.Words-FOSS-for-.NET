// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/11/2021 by Ilya Navrotskiy

using System.Text;
using Aspose.Fonts;
using Aspose.JavaAttributes;
using Aspose.Words.Fields;
using Aspose.Words.RW.Txt.Writer;

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// The class responsible for writing Fields into markdown.
    /// </summary>
    internal class MarkdownFieldWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownFieldWriter" /> class.
        /// </summary>
        internal MarkdownFieldWriter(FieldStart fieldStart, MarkdownWriter writer)
        {
            ArgumentUtil.CheckNotNull(fieldStart, "FieldStart");

            FieldStart = fieldStart;

            mEmphasesWriter = new MarkdownEmphasesWriter(
                mFieldResult,
                null,
                writer);
        }

        /// <summary>
        /// Creates instance of <see cref="MarkdownFieldWriter"/> object.
        /// </summary>
        internal static MarkdownFieldWriter Create(FieldStart fieldStart, MarkdownLinkDefinitionWriter linkDefinitionWriter, MarkdownWriter writer)
        {
            switch (fieldStart.FieldType)
            {
                case FieldType.FieldRef:
                case FieldType.FieldHyperlink:
                {
                    return new MarkdownHyperlinkWriter(fieldStart, linkDefinitionWriter, writer);
                }

                case FieldType.FieldPageRef:
                {
                    // Simple check for nested ref fields to avoid escaping in the child field.
                    return (writer.FieldWriter != null) && IsRefField(writer.FieldWriter.FieldStart)
                        ? new MarkdownFieldWriter(fieldStart, writer)
                        : new MarkdownHyperlinkWriter(fieldStart, linkDefinitionWriter, writer);
                }

                default:
                    return new MarkdownFieldWriter(fieldStart, writer);
            }
        }

        /// <summary>
        /// Processes a FieldSeparator.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void OnFieldSeparator(FieldSeparator fieldSeparator)
        {
            IsFieldResult = true;

            // WORDSNET-23165 By default, lets ignore text in FieldCode part.
            mFieldResult.Clear();
        }

        /// <summary>
        /// Processes a specified FieldEnd.
        /// </summary>
        internal string OnFieldEnd(FieldEnd fieldEnd)
        {
            AppendNextSiblingEmphases(null);
            return Text;
        }

        /// <summary>
        /// Processes a specified run.
        /// </summary>
        [JavaThrows(true)]
        internal void OnRun(Run run)
        {
            Debug.Assert(run != null);

            string text = TxtWriterBase.GetText(run);
            if (IsFieldResult)
                mEmphasesWriter.AppendEmphases(run);
            AppendText(text);
        }

        /// <summary>
        /// Returns true, if a specified <see cref="FieldStart"/> type is one of the field reference types.
        /// </summary>
        private static bool IsRefField(FieldStart fieldStart)
        {
            return (fieldStart.FieldType == FieldType.FieldRef) ||
                   (fieldStart.FieldType == FieldType.FieldHyperlink) ||
                   (fieldStart.FieldType == FieldType.FieldPageRef);
        }

        /// <summary>
        /// Appends a specified text to the FieldResult builder.
        /// </summary>
        private void AppendText(string text)
        {
            if (text == null)
                return;

            foreach (char c in text)
            {
                if (IsFieldResult && !char.IsWhiteSpace(c))
                    mEmphasesWriter.FlushPendingOpeningEmphases();

                FieldResult.Append(FontUtil.UnicodeToSymbol(c));
            }
        }

        /// <summary>
        /// Flushes current <see cref="Text"/>.
        /// </summary>
        internal string FlushText()
        {
            string text = OnFieldEnd(null);
            FieldResult.Length = 0;

            return text;
        }

        /// <summary>
        /// Appends a specified text to a FieldResult.
        /// </summary>
        internal void AppendToFieldResult(string text)
        {
            FieldResult.Append(text);
        }

        /// <summary>
        /// Appends emphases for a next sibling IInline node of a specified FieldChar.
        /// </summary>
        private void AppendNextSiblingEmphases(FieldChar fieldChar)
        {
            IInline emphasisSrc = (fieldChar == null)
                ? null
                : (IInline)fieldChar.NextSiblingOfTypes(NodeType.Run, NodeType.Footnote);

            mEmphasesWriter.AppendEmphases(emphasisSrc);
        }

        /// <summary>
        /// Gets a FieldResult builder.
        /// </summary>
        /// <remarks>
        /// Field result in Hyperlink contains DisplayText of the link.
        /// </remarks>
        protected StringBuilder FieldResult
        {
            get { return mFieldResult.CurrentLine; }
        }

        /// <summary>
        /// Gets a string that is ready for writing into markdown.
        /// </summary>
        protected virtual string Text
        {
            get { return FieldResult.ToString(); }
        }

        /// <summary>
        /// Gets a boolean value indicating either we are inside FieldResult for a moment.
        /// </summary>
        protected bool IsFieldResult { get; private set; }

        /// <summary>
        /// Gets a FieldStart object.
        /// </summary>
        private FieldStart FieldStart { get; set; }

        private readonly MarkdownEmphasesWriter mEmphasesWriter;
        private readonly TxtContentLines mFieldResult = new TxtContentLines();
    }
}
