// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/09/2019 by Ilya Navrotskiy

using System.Text;
using Aspose.Words.RW.Markdown.Reader;

namespace Aspose.Words.RW.Markdown
{
    internal abstract class CodeBlockBase : InlineContainerBlock
    {
        /// <summary>
        /// Parses text in content lines onto child Inline blocks.
        /// </summary>
        internal override void Parse()
        {
            string inlineText = GetInlineText();
            InlineBlock inlineBlock = new InlineBlock(inlineText);
            if (!TryAppend(inlineBlock))
                Add(inlineBlock);

            // No need to keep text lines in a memory after they were parsed as inlines.
            ContentLines.Clear();
            RemoveAllParts();
        }

        /// <summary>
        /// Writes the block into a model using specified context.
        /// </summary>
        internal override void Write(MarkdownReaderContext context)
        {
            context.Open(this);
            context.Builder.Write(Text);
            context.Close(this);
        }

        /// <summary>
        /// Gets a plain text from the content lines of the inline container.
        /// </summary>
        protected override string GetInlineText()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ContentLines.Count; i++)
            {
                sb.Append(ContentLines[i]);

                // Append EOL to each line except of very last one.
                if (i < ContentLines.Count - 1)
                    sb.Append(LineSeparator);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Leaf; }
        }
    }
}
