// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2019 by Ilya Navrotskiy

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Aspose.Words.Loading;
using Aspose.Words.RW.Html.Parser;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// The base class for containers of markdown Inline blocks.
    /// </summary>
    internal abstract class InlineContainerBlock : ContainerBlock
    {
        /// <summary>
        /// Parses text in content lines onto child Inline blocks.
        /// </summary>
        internal virtual void Parse()
        {
            string inlineText = GetInlineText();
            LinkedList delimiters = GetDelimiters(GetDelimiterRuns(inlineText, LoadOptions));

            Link(delimiters);
            ProcessDelimiters(delimiters, inlineText);

            // No need to keep text lines in a memory after they were parsed as inlines.
            ContentLines.Clear();
            RemoveAllParts();
        }

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            switch (block.Type)
            {
                case BlockType.IndentedCode:
                case BlockType.Paragraph:
                {
                    // If block being appended is inside a Quote, then we might close some of the
                    // parent Quotes to count their nesting level. As we encounter a non-quote
                    // block that means Quote was already accounted, we MUST open all closed
                    // Quotes to allow append into them.
                    OpenParentQuotes();

                    // We can merge contents (so called lazy-continuation).
                    ContentLines.Add(block.ContentLine);
                    return true;
                }
                case BlockType.EndOfLine:
                {
                    // An EOL does not interrupt anything.
                    Add(block);
                    return true;
                }
                default:
                    return base.TryAppend(block);
            }
        }

        /// <summary>
        /// Opens all parent Quote blocks.
        /// </summary>
        protected void OpenParentQuotes()
        {
            OpenParents(BlockType.Quote);
        }

        /// <summary>
        /// Appends inline block with a specified text to this container.
        /// </summary>
        private void AppendInline(string text)
        {
            if (!StringUtil.HasChars(text))
                return;

            Block parentBlock = GetDeepestOpenedChildBlock();
            if (parentBlock == null)
                parentBlock = this;

            // IN: Not sure, this switch block should be here. Looks like it
            // will be more appropriate to process it finally in Block.Write().
            string textToAppend;
            switch (parentBlock.Type)
            {
                case BlockType.IndentedCode:
                case BlockType.FencedCode:
                case BlockType.InlineCode:
                {
                    textToAppend = text;
                    break;
                }

                case BlockType.LinkText:
                {
                    ((LinkTextBlock)parentBlock).SetRawText(text);
                    textToAppend = MarkdownUtil.UnescapeMarkupSymbols(text);

                    break;
                }

                case BlockType.ImageDescription:
                {
                    ((ImageDescriptionBlock)parentBlock).SetRawText(text);
                    textToAppend = MarkdownUtil.UnescapeMarkupSymbols(text);

                    break;
                }

                case BlockType.LinkDestination:
                {
                    textToAppend = text.Replace(MarkdownUtil.SoftLineBreakChar, ControlChar.LineBreakChar);
                    break;
                }

                default:
                {
                    string parsedText = ParseHardLineBreaks(text);
                    textToAppend = MarkdownUtil.UnescapeMarkupSymbols(parsedText);

                    break;
                }
            }

            // Finally, append inline block with a non-empty text.
            if (textToAppend.Length == 0)
                return;

            // Don't split to HTML inlines for these block types.
            if (parentBlock.Type == BlockType.IndentedCode ||
                parentBlock.Type == BlockType.FencedCode ||
                parentBlock.Type == BlockType.InlineCode ||
                parentBlock.Type == BlockType.LinkDestination ||
                parentBlock.Type == BlockType.Autolink)
            {

                InlineBlock inlineBlock = new InlineBlock(textToAppend);
                if (!TryAppend(inlineBlock))
                    Add(inlineBlock);
            }
            else
            {
                // WORDSNET-27290 Implemented loading of raw HTML blocks.
                List<Block> inlineBlocks = GetInlines(textToAppend);
                foreach (Block inlineBlock in inlineBlocks)
                {
                    if (!TryAppend(inlineBlock))
                        Add(inlineBlock);
                }
            }
        }

        /// <summary>
        /// Returns a collection of inline blocks (<see cref="InlineBlock"/> or <see cref="HtmlTagBlock"/>)
        /// parsed from a specified string.
        /// </summary>
        private static List<Block> GetInlines(string text)
        {
            List<Block> inlineBlocks = new List<Block>();

            HtmlTokenizer tokenizer = new HtmlTokenizer(text, new Features());

            // Check there is no any valid HTML tokens, but there is non-empty string is passed.
            HtmlToken token = tokenizer.NextToken(false);
            if (token.Type == HtmlTokenType.EndOfFile)
            {
                inlineBlocks.Add(new InlineBlock(text));
                return inlineBlocks;
            }

            // Process HTML tokens.
            while (token.Type != HtmlTokenType.EndOfFile)
            {
                switch (token.Type)
                {
                    case HtmlTokenType.Tag:
                    {
                        HtmlTagToken tagToken = (HtmlTagToken)token;

                        // We need this because HtmlToken doesn't preserve arguments order after it is parsed.
                        // In this case we cannot just restore text of html tag and forced to fall back to
                        // general inline block. For correct <http> tag we assume that there should be exactly
                        // one attribute.
                        if (tagToken.Name.StartsWith("http", StringComparison.OrdinalIgnoreCase) && (tagToken.Attributes.Count != 1))
                        {
                            inlineBlocks.Add(new InlineBlock(text));
                        }
                        else
                        {
                            inlineBlocks.Add(new HtmlTagBlock((HtmlTagToken)token));
                        }

                        break;
                    }
                    case HtmlTokenType.Text:
                    {
                        inlineBlocks.Add(new InlineBlock(((HtmlTextToken)token).Text));
                        break;
                    }
                }

                token = tokenizer.NextToken(false);
            }

            return inlineBlocks;
        }

        /// <summary>
        /// Gets a plain text from the content lines of the inline container.
        /// </summary>
        protected virtual string GetInlineText()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ContentLines.Count; i++)
            {
                string line = ContentLines[i];
                // Do not append an empty line.
                if (line.Length > 0)
                {
                    sb.Append(line);

                    // Append EOL to each line except of very last one.
                    if (i < (ContentLines.Count - 1))
                        sb.Append(MarkdownUtil.SoftLineBreakChar);
                }
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Returns index of hard line break in a specified string, or -1 if not found.
        /// </summary>
        private static int IndexOfHardLineBreak(string txtLine)
        {
            if (!StringUtil.HasChars(txtLine))
                return -1;

            int index = txtLine.Length - 1;

            // Check case with backslash at the end of line.
            if ((txtLine[index] == '\\') && !MarkdownUtil.IsEscaped(txtLine, index))
                return index;

            // Check case with 2 or more spaces at the end of line.
            while (index >= 0 && txtLine[index] == ' ')
                index--;

            return (txtLine.Length - index) > 2 ? (index + 1) : -1;
        }

        /// <summary>
        /// Gets a collection of the <see cref="DelimiterRun"/> objects from a specified text.
        /// </summary>
        private static List<DelimiterRun> GetDelimiterRuns(string text, MarkdownLoadOptions loadOptions)
        {
            List<DelimiterRun> delimiterRuns = new List<DelimiterRun>();

            int start = 0;
            while (start < text.Length)
            {
                DelimiterRun delimiterRun = new DelimiterRun(loadOptions);
                if (delimiterRun.TryParse(text, start))
                {
                    delimiterRuns.Add(delimiterRun);
                    start += delimiterRun.Length;
                }
                else
                {
                    // If delimiter was not parsed successfully due to it is escaped,
                    // then try to skip escaped character. Otherwise, there are no more
                    // delimiter runs, so we should stop to search.
                    if (delimiterRun.IsEscaped)
                        start = delimiterRun.Start + 1;
                    else
                        break;
                }
            }

            return delimiterRuns;
        }

        /// <summary>
        /// Gets a doubly linked list of the <see cref="Delimiter"/> objects
        /// from a specified collection of delimiter runs.
        /// </summary>
        private static LinkedList GetDelimiters(List<DelimiterRun> delimiterRuns)
        {
            LinkedList delimiters = new LinkedList();

            foreach (DelimiterRun delimiterRun in delimiterRuns)
            {
                foreach (Delimiter delimiter in delimiterRun.Delimiters)
                {
                    if (delimiter.IsValid)
                    {
                        delimiters.Append(delimiter);
                        delimiters.MoveObjectSafe(delimiter);
                    }
                }
            }

            return delimiters;
        }

        /// <summary>
        /// Links delimiters in a specified doubly linked list so
        /// that they form a pair of Opening and Closing delimiters.
        /// </summary>
        private void Link(LinkedList delimiters)
        {
            delimiters.Reset();
            while (delimiters.MoveNext())
            {
                Delimiter closingDelimiter = ((Delimiter)delimiters.Current).GetPotentialClosingForward();
                if (closingDelimiter != null)
                {
                    delimiters.MoveObject(closingDelimiter);

                    Delimiter openingDelimiter = closingDelimiter.GetPotentialOpeningBackward();
                    if (CanBeLinked(openingDelimiter, closingDelimiter))
                    {
                        Link(openingDelimiter, closingDelimiter, delimiters);
                        // The InlineLink (or Image) is a complex object that consists of two parts:
                        // LinkText (or ImageDescription in case of it is an Image) and a corresponding LinkDestination.
                        if (openingDelimiter.Type == DelimiterType.LinkDestinationOpening ||
                            openingDelimiter.Type == DelimiterType.LinkTextOpening)
                            LinkLinkText(openingDelimiter, delimiters);
                    }
                }
            }
        }

        /// <summary>
        /// Links LinkText (or ImageDescription) delimiters corresponded to a specified LinkDestination.
        /// </summary>
        private static void LinkLinkText(Delimiter linkOpening, LinkedList delimiters)
        {
            Debug.Assert(linkOpening.IsLinked);

            Delimiter linkTextClosing = linkOpening.GetLinkTextClosing();
            if (linkTextClosing == null)
                return;

            Delimiter linkTextOpening = (linkTextClosing.IsLinked)
                ? (linkTextClosing.LinkedDelimiter.Type == DelimiterType.FootnoteOpening)
                    ? ((FootnoteOpeningDelimiter)linkTextClosing.LinkedDelimiter).LinkTextOpening
                    : linkTextClosing.LinkedDelimiter
                : linkTextClosing.GetPotentialOpeningBackward();

            if (linkTextOpening == null)
                return;

            Link(linkTextOpening, linkTextClosing, delimiters);

            // Now we need to link delimiters inside the just linked LinkText.
            Delimiter start = (Delimiter)linkTextOpening.NextNode;
            // In case of link text is actually an ImageDescription,
            // we have to skip the LinkText delimiter immediately following it.
            if (linkTextOpening.Type == DelimiterType.ImageDescriptionOpening)
                start = (Delimiter)start.NextNode;

            while (start != linkTextClosing)
            {
                if (!start.IsNotIncluded)
                {
                    // Find closing delimiter located before the end of link text.
                    Delimiter closing = start.GetPotentialClosingForward();
                    if ((closing == null) || !closing.IsBefore(linkTextClosing))
                        break;

                    // Find opening delimiter located after the stat of link text.
                    Delimiter opening = closing.GetPotentialOpeningBackward();
                    if ((opening != null) && linkTextOpening.IsBefore(opening))
                    {
                        Link(opening, closing, delimiters);
                        // The InlineLink (or Image) is a complex object that consists of two parts:
                        // LinkText (or ImageDescription in case of it is an Image) and a corresponding LinkDestination.
                        if (opening.Type == DelimiterType.LinkDestinationOpening)
                            LinkLinkText((LinkDestinationOpeningDelimiter)opening, delimiters);

                        start = (Delimiter)closing.NextNode;
                    }
                    else
                    {
                        start = (Delimiter)start.NextNode;
                    }
                }
            }
        }

        /// <summary>
        /// Links specified delimiters.
        /// </summary>
        private static void Link(Delimiter openingDelimiter, Delimiter closingDelimiter, LinkedList delimiters)
        {
            // When we link delimiters into a pair, their lengths might been different,
            // so there can be a delimiter that is remained from one of those paired delimiters.
            Delimiter remainedDelimiter = closingDelimiter.Link(openingDelimiter);

            if (remainedDelimiter != null)
            {
                if (remainedDelimiter.End < openingDelimiter.Start)
                {
                    // If delimiter is remained from the Opening, then insert it before it.
                    delimiters.MoveObjectSafe(openingDelimiter);
                    delimiters.Insert(remainedDelimiter);
                }
                else
                {
                    // If delimiter is remained after the Closing, then insert it after it.
                    delimiters.MoveObjectSafe(closingDelimiter);
                    delimiters.Append(remainedDelimiter);
                }

                delimiters.MoveObjectSafe(closingDelimiter);
            }

            // Unlink previously linked delimiters inside the new linked pair in accordance with their priority.
            UnLink(openingDelimiter, closingDelimiter);

            // Each type of delimiter has its maximum allowed length when it is in a linked state.
            openingDelimiter.Split(delimiters);
        }

        /// <summary>
        /// Unlinks linked delimiters between specified ones in accordance with their priority.
        /// </summary>
        private static void UnLink(Delimiter a, Delimiter b)
        {
            Delimiter start = a.IsBefore(b) ? a : b;
            Delimiter end = a.IsBefore(b) ? b : a;

            // We skip range bounds, so move to next node immediately.
            start = (Delimiter)start.NextNode;
            while (start != end)
            {
                if (start.IsLinked && (start.Priority < a.UnlinkPriority))
                    start.UnLink();

                start = (Delimiter)start.NextNode;
            }
        }

        // IN: This probbaly should be done in Delimiter class.
        /// <summary>
        /// Returns true if the specified opening and closing delimiters can be linked.
        /// </summary>
        private bool CanBeLinked(Delimiter openingDelimiter, Delimiter closingDelimiter)
        {
            if ((openingDelimiter == null) || (closingDelimiter == null))
                return false;

            if (closingDelimiter.Type == DelimiterType.AutoLinkClosing)
            {
                if (!AutolinkInlineBlock.IsValid(openingDelimiter, closingDelimiter))
                    return false;
            }

            if (closingDelimiter.Type == DelimiterType.LinkTextClosing)
            {
                switch (openingDelimiter.Type)
                {
                    case DelimiterType.ImageDescriptionOpening:
                        return CanBeLinkedImageDescriptionDelimiters(openingDelimiter as ImageDescriptionOpeningDelimiter,
                            closingDelimiter as LinkTextClosingDelimiter);
                    case DelimiterType.FootnoteOpening:
                        return true;
                    case DelimiterType.LinkTextOpening:
                        return CanBeLinkedLinkTextDelimiters(openingDelimiter as LinkTextOpeningDelimiter,
                            closingDelimiter as LinkTextClosingDelimiter);
                    default:
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if the specified opening and closing LinkText delimiters can be linked.
        /// </summary>
        private bool CanBeLinkedLinkTextDelimiters(LinkTextOpeningDelimiter linkTextOpen, LinkTextClosingDelimiter linkTextClose)
        {
            if ((linkTextOpen == null) || (linkTextClose == null))
                return false;

            if (linkTextClose.HasLinkDestination || linkTextClose.HasReferenceLinkText || linkTextClose.HasCollapsedLinkText)
                return true;

            string linkText = (linkTextOpen.Start < linkTextClose.End)
                ? linkTextOpen.Text.Substring(linkTextOpen.Start + 1, linkTextClose.End - linkTextOpen.Start - 1)
                    .ToUpper(CultureInfo.InvariantCulture)
                : string.Empty;

            return Document.LinkDefinitions.ContainsKey(linkText);
        }

        /// <summary>
        /// Returns true if the specified opening ImageDescription and closing LinkText delimiters can be linked.
        /// </summary>
        private bool CanBeLinkedImageDescriptionDelimiters(ImageDescriptionOpeningDelimiter imageDescOpen, LinkTextClosingDelimiter linkTextClose)
        {
            if ((imageDescOpen == null) || (linkTextClose == null))
                return false;

            if (linkTextClose.HasLinkDestination)
                return true;

            string linkText = (imageDescOpen.Start < linkTextClose.End)
                ? imageDescOpen.Text.Substring(imageDescOpen.Start + 2, linkTextClose.End - imageDescOpen.Start - 2)
                    .ToUpper(CultureInfo.InvariantCulture)
                : string.Empty;

            return Document.LinkDefinitions.ContainsKey(linkText);
        }

        /// <summary>
        /// Processes delimiters in a specified text by converting them to a concrete inline blocks.
        /// </summary>
        private void ProcessDelimiters(LinkedList delimiters, string text)
        {
            delimiters.Reset();

            Delimiter prevLinkedDelimiter = null;
            string inlineText;

            Stack<Block> inlineContainers = new Stack<Block>();
            Stack<ReferenceBlock> unresolvedLinks = new Stack<ReferenceBlock>();

            while (delimiters.MoveNext())
            {
                Delimiter curLinkedDelimiter = (Delimiter)delimiters.Current;
                if (curLinkedDelimiter.IsLinked)
                {
                    // The text of ImageDescription is actually resides inside a corresponding LinkText.
                    if ((prevLinkedDelimiter != null) && (prevLinkedDelimiter.Type == DelimiterType.ImageDescriptionOpening))
                        prevLinkedDelimiter = ((ImageDescriptionOpeningDelimiter)prevLinkedDelimiter).LinkTextOpening;

                    // The text before FootnoteReference is actually resides before the corresponding LinkText.
                    inlineText = GetSubstring(text, prevLinkedDelimiter,
                        (curLinkedDelimiter.Type == DelimiterType.FootnoteOpening)
                            ? ((FootnoteOpeningDelimiter)curLinkedDelimiter).LinkTextOpening
                            : curLinkedDelimiter);

                    // Append text located before current opening delimiter.
                    AppendInline(MarkdownUtil.UnEscape(inlineText).ToString());

                    if (curLinkedDelimiter.IsOpening)
                    {
                        Block inlineContainer = curLinkedDelimiter.ToBlock();
                        inlineContainers.Push(inlineContainer);
                        if (!TryAppend(inlineContainer))
                            Add(inlineContainer);
                    }
                    else
                    {
                        // Normally, we cannot encounter closing delimiter before its opening.
                        Debug.Assert(inlineContainers.Count > 0);

                        Block inlineContainer = inlineContainers.Pop();
                        inlineContainer.Close();

                        // Resolve LinkDefinitions.
                        if ((inlineContainer.Type == BlockType.LinkText) || (inlineContainer.Type == BlockType.ImageDescription))
                            if (((ReferenceBlock)inlineContainer).Resolve(Document.LinkDefinitions, unresolvedLinks))
                                inlineContainer.Remove();

                        // Resolve FootnoteReference.
                        if (inlineContainer.Type == BlockType.FootnoteReference)
                            ((FootnoteReferenceBlock)inlineContainer).Resolve(Document.FootnoteDefinitions);
                    }

                    prevLinkedDelimiter = curLinkedDelimiter;
                }
            }

            while (unresolvedLinks.Count > 0)
            {
                ReferenceBlock linkTextBlock = unresolvedLinks.Pop();
                if (!linkTextBlock.HasLinkDestination())
                    linkTextBlock.ExpandInlineLinkText();
            }

            // Process a last piece of text.
            inlineText = GetSubstring(text, prevLinkedDelimiter, null);
            AppendInline(MarkdownUtil.UnEscape(inlineText).ToString());
        }

        /// <summary>
        /// Gets text between specified delimiters.
        /// </summary>
        private static string GetSubstring(string text, Delimiter delimiterBefore, Delimiter delimiterAfter)
        {
            int startIdx = (delimiterBefore == null) ? 0 : delimiterBefore.End + 1;
            int length = ((delimiterAfter == null) ? text.Length : delimiterAfter.Start) - startIdx;

            if (length == 0)
                return "";

            if (length == text.Length)
                return text;

            return text.Substring(startIdx, length);
        }

        /// <summary>
        /// Parses a specified text to a HardLineBreaks
        /// (<see cref="MarkdownUtil.HardLineBreakSpacesChar"/> and
        /// <see cref="MarkdownUtil.HardLineBreakSlashChar"/>).
        /// </summary>
        private static string ParseHardLineBreaks(string text)
        {
            StringBuilder sb = new StringBuilder();

            int startIndex = 0;
            int softLineBreakIndex = text.IndexOf(MarkdownUtil.SoftLineBreakChar);
            while (softLineBreakIndex != -1)
            {
                string line = text.Substring(startIndex, softLineBreakIndex - startIndex);
                if (line.Length > 0)
                {
                    int hardLineBreakIndex = IndexOfHardLineBreak(line);
                    if (hardLineBreakIndex != -1)
                    {
                        sb.Append(line.Substring(0, hardLineBreakIndex));
                        sb.Append(line[hardLineBreakIndex] == '\\'
                            ? MarkdownUtil.HardLineBreakSlashChar
                            : MarkdownUtil.HardLineBreakSpacesChar);
                    }
                    else
                    {
                        sb.Append(line);
                        sb.Append(MarkdownUtil.SoftLineBreakChar);
                    }
                }
                else
                {
                    sb.Append(MarkdownUtil.SoftLineBreakChar);
                }

                startIndex = softLineBreakIndex + 1;
                softLineBreakIndex = text.IndexOf(MarkdownUtil.SoftLineBreakChar, startIndex);
            }

            sb.Append(text.Substring(startIndex));
            return sb.ToString();
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder contentLines = new StringBuilder(ContentLines.Count);
            foreach (string contentLine in ContentLines)
            {
                contentLines.Append("{" + contentLine + "}");
            }

            return string.Format("{0}, Lines: {1}", base.ToString(), contentLines);
        }
#endif

        /// <summary>
        /// Gets content lines.
        /// </summary>
        internal List<string> ContentLines
        {
            get { return mContentLines; }
        }

        /// <summary>
        /// Gets <see cref="MarkdownLoadOptions"/>.
        /// </summary>
        private MarkdownLoadOptions LoadOptions
        {
            get { return (Document == null) ? null : Document.LoadOptions; }
        }

        private readonly List<string> mContentLines = new List<string>();

        /// <summary>
        /// The line separator to insert after each content line in a plain text.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char LineSeparator = ControlChar.LineBreakChar;
    }
}
