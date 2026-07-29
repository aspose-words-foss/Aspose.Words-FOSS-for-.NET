// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/04/2019 by Ilya Navrotskiy

using System;
using System.Drawing;
using System.Text;
using Aspose.Collections;
using Aspose.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Lists;
using Aspose.Words.Notes;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Markdown.Reader
{
    /// <summary>
    /// The class that helps to read a Markdown document into the model.
    /// </summary>
    internal class MarkdownReaderContext
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal MarkdownReaderContext(Document document)
        {
            Debug.Assert(document != null);

            mBuilder = new DocumentBuilder(document);

            // Disable bold and italic explicitly to exclude affecting from a parent styles.
            mBuilder.Bold = false;
            mBuilder.Italic = false;
            mBuilder.Font.StrikeThrough = false;

            // Set defaults for cute looking.
            ParaPr defaultParaPr = document.Styles.DefaultParaPr;
            defaultParaPr.SpaceAfter = 160;

            RunPr defaultRunPr = document.Styles.DefaultRunPr;
            defaultRunPr.Name = "Calibri";
            defaultRunPr.Size = 22;
            defaultRunPr.SizeBi = 22;

            // Lets Normal inherit font size from the defaults.
            Style normalStyle = mBuilder.Document.Styles.GetByName("Normal", true);
            normalStyle.RunPr.Remove(FontAttr.Size);
            normalStyle.RunPr.Remove(FontAttr.SizeBi);
        }

        /// <summary>
        /// Opens block in the context.
        /// </summary>
        internal void Open(Block block)
        {
            switch (block.Type)
            {
                // WORDSNET-27290 Implemented loading of HTML inlines.
                case BlockType.HtmlTag:
                {
                    HtmlTagBlock htmlTagBlock = (HtmlTagBlock)block;

                    // WORDSNET-28605 Implemented loading HTML tables (and actually other HTML tags).
                    if (IsInHtml || !OpenHtmlInline(htmlTagBlock))
                    {
                        string tagName = htmlTagBlock.TagName;
                        if (gKnownHtmlTagNames.Contains(tagName))
                        {
                            if (!htmlTagBlock.IsSelfClosing)
                                IncHtmlTagsStartCount(tagName);
                            mHtmlBuilder.Append(htmlTagBlock.RawText);
                        }
                        else
                        {
                            string text = htmlTagBlock.RawText.Replace(MarkdownUtil.SoftLineBreakChar, ' ');
                            mBuilder.Write(text);
                        }
                    }

                    break;
                }
                case BlockType.Quote:
                {
                    // We MUST separate sibling Quotes with an empty paragraph. Otherwise, we will not be able to understand
                    // they are different Quotes in our model.
                    if ((block.Parent.Type == BlockType.Document) &&
                        (mLastClosedBlock != null) && (mLastClosedBlock.Type == BlockType.Quote))
                    {
                        mBuilder.Writeln();
                        mBuilder.ParagraphFormat.Style = FetchStyle("Normal", StyleType.Paragraph);
                    }
                    break;
                }
                case BlockType.Paragraph:
                case BlockType.IndentedCode:
                case BlockType.HorizontalRule:
                case BlockType.FencedCode:
                case BlockType.SetextHeading:
                case BlockType.AtxHeading:
                {
                    if (mFootnote != null)
                        StartFootnoteParagraph();
                    break;
                }
                case BlockType.Table:
                {
                    // Separate sibling Tables with an empty paragraph.
                    if (IsTable(mLastClosedBlock))
                        mBuilder.Writeln();

                    // WORDSNET-26451 Reset style to 'Normal' for each newly started table.
                    mBuilder.ParagraphFormat.Style = FetchStyle("Normal", StyleType.Paragraph);
                    break;
                }
                case BlockType.Cell:
                {
                    Cell cell = mBuilder.InsertCell();
                    ParaPr paraPr = cell.FirstParagraph.ParaPr;
                    paraPr.Alignment = ((CellBlock)block).Alignment;
                    // There cannot be lists in a markdown table.
                    RemoveList(paraPr);
                    break;
                }
                case BlockType.InlineCode:
                {
                    string styleName = GetInlineCodeStyleName((InlineCodeBlock)block);
                    Style style = FetchStyle(styleName, StyleType.Character);
                    mBuilder.Font.Style = style;
                    DecorateInlineCode(style);
                    break;
                }
                case BlockType.LinkText:
                {
                    LinkDestinationBlock linkDestinationBlock = ((LinkTextBlock)block).LinkDestinationBlock;
                    if (linkDestinationBlock != null)
                    {
                        mBuilder.Font.Style = mBuilder.Document.Styles.GetBySti(StyleIdentifier.Hyperlink, true);
                        FieldCodeHyperlink fieldCode = new FieldCodeHyperlink(
                            UriUtil.GetAddress(linkDestinationBlock.Uri),
                            UriUtil.GetSubAddress(linkDestinationBlock.Uri),
                            linkDestinationBlock.Title,
                            string.Empty,
                            ((LinkTextBlock)block).DefinitionLabel,
                            false);
                        mBuilder.StartHyperlink(fieldCode);
                    }
                    break;
                }
                case BlockType.Autolink:
                {
                    mBuilder.Font.Style = mBuilder.Document.Styles.GetBySti(StyleIdentifier.Hyperlink, true);
                    break;
                }
                case BlockType.BoldInline:
                {
                    mBuilder.Bold = true;
                    mBoldCount++;
                    break;
                }
                case BlockType.ItalicInline:
                {
                    mBuilder.Italic = true;
                    mItalicCount++;
                    break;
                }
                case BlockType.Strikethrough:
                {
                    mBuilder.Font.StrikeThrough = true;
                    mStrikethroughCount++;
                    break;
                }
                case BlockType.Underline:
                {
                    mBuilder.Font.Underline = Underline.Single;
                    mUnderlineCount++;
                    break;
                }
                case BlockType.FootnoteReference:
                {
                    mFootnote = mBuilder.InsertFootnote(FootnoteType.Footnote, "", null);
                    // Now we need to write definition paragraphs of the footnote.
                    // Thus save formatting of the current parent paragraph of the footnote
                    // to restore it after all definitions are written and we will continue
                    // to write the interrupted parent paragraph of the footnote.
                    SaveCurrentFormatting();
                    // Clear current formatting in order to avoid its influence on definition paragraphs.
                    ClearFormatting();
                    break;
                }
                default:
                    break;
            }

            mLastOpenedBlock = block;
        }

        /// <summary>
        /// Closes block in the context.
        /// </summary>
        internal void Close(Block block)
        {
            switch (block.Type)
            {
                case BlockType.Document:
                {
                    // Flush possible pending raw HTML.
                    if (mHtmlBuilder.Length > 0)
                    {
                        mBuilder.InsertHtml(mHtmlBuilder.ToString());
                        mHtmlBuilder.Length = 0;
                    }

                    // When we create new Document we ensure there is at least one paragraph.
                    // So, if this is still empty and there is another paragraph before,
                    // we should remove this redundant paragraph created along with the Document.
                    Paragraph lastParagraph = mBuilder.Document.LastSection.Body.LastChild as Paragraph;
                    if ((lastParagraph != null) && !lastParagraph.HasChildNodes)
                    {
                        Node prevNode = lastParagraph.PreviousSibling;
                        if (prevNode != null && prevNode.NodeType == NodeType.Paragraph)
                            lastParagraph.Remove();
                    }
                    break;
                }
                case BlockType.Quote:
                {
                    // Set Normal style back when Quote is closed.
                    mBuilder.ParagraphFormat.Style = FetchStyle("Normal", StyleType.Paragraph);
                    break;
                }
                case BlockType.Paragraph:
                case BlockType.HorizontalRule:
                case BlockType.AtxHeading:
                case BlockType.SetextHeading:
                case BlockType.IndentedCode:
                case BlockType.FencedCode:
                {
                    if (mLastOpenedBlock != null)
                    {
                        // Check previous opened is not a HTML <p> or <h0>-<h9>.
                        HtmlTagBlock htmlBlock = mLastOpenedBlock as HtmlTagBlock;
                        if (htmlBlock == null || !htmlBlock.IsParaOrHeading)
                            ApplyFormatting(mBuilder.CurrentParagraph.ParaPr, block);
                    }

                    StartNewPara();
                    break;
                }
                case BlockType.Table:
                {
                    mBuilder.EndTable();
                    // There cannot be lists in a markdown table, so lets remove it
                    // to avoid influences of possible previous list.
                    RemoveList(mBuilder.CurrentParagraph.ParaPr);
                    break;
                }
                case BlockType.Row:
                {
                    RowBlock row = (RowBlock)block;
                    TableBlock parentTable = (TableBlock)row.GetParent(BlockType.Table);
                    Debug.Assert(parentTable != null);
                    // The spec says that if there are a number of cells fewer than the number of cells
                    // in the header row, empty cells are inserted.
                    for (int i = row.Count; i < parentTable.ColumnsCount; i++)
                    {
                        Cell cell = mBuilder.InsertCell();
                        cell.FirstParagraph.ParaPr.Alignment = ((CellBlock)parentTable.DelimiterRow[i]).Alignment;
                    }

                    mBuilder.EndRow();
                    break;
                }
                case BlockType.LinkText:
                {
                    if (((LinkTextBlock)block).LinkDestinationBlock != null)
                    {
                        mBuilder.EndHyperlink();
                        mBuilder.Font.Style = mBuilder.Document.Styles.GetBySti(StyleIdentifier.DefaultParagraphFont, true);
                    }
                    break;
                }
                case BlockType.Autolink:
                case BlockType.InlineCode:
                {
                    mBuilder.Font.Style = mBuilder.Document.Styles.GetBySti(StyleIdentifier.DefaultParagraphFont, true);
                    break;
                }
                case BlockType.BoldInline:
                {
                    mBoldCount--;
                    mBuilder.Bold = (mBoldCount > 0);
                    break;
                }
                case BlockType.ItalicInline:
                {
                    mItalicCount--;
                    mBuilder.Italic = (mItalicCount > 0);
                    break;
                }
                case BlockType.Strikethrough:
                {
                    mStrikethroughCount--;
                    mBuilder.Font.StrikeThrough = (mStrikethroughCount > 0);
                    break;
                }
                case BlockType.Underline:
                {
                    mUnderlineCount--;
                    if (mUnderlineCount == 0)
                        mBuilder.Font.Underline = Underline.None;
                    break;
                }

                case BlockType.FootnoteReference:
                {
                    Debug.Assert(mFootnote != null);

                    mBuilder.MoveTo(mFootnote.ParentParagraph);
                    mFootnote = null;
                    // We wrote footnote definition paragraphs and ready to continue to write the rest
                    // of interrupted parent paragraph of the footnote. So, restore its formatting.
                    RestoreFormatting();
                    break;
                }

                // WORDSNET-27290 Implemented loading of HTML inlines.
                case BlockType.HtmlTag:
                {
                    HtmlTagBlock htmlTagBlock = (HtmlTagBlock)block;

                    // WORDSNET-28605 Implemented loading HTML tables.
                    if (IsInHtml)
                        mHtmlBuilder.Append(htmlTagBlock.RawText);
                    else if (!CloseHtmlInline(htmlTagBlock))
                        mBuilder.Write(htmlTagBlock.RawText);

                    DecHtmlTagsStartCount(htmlTagBlock.TagName);

                    if (HtmlTagsStartedCount == 0 && (mHtmlBuilder.Length > 0))
                    {
                        mBuilder.InsertHtml(mHtmlBuilder.ToString(), HtmlInsertOptions.RemoveLastEmptyParagraph);
                        mHtmlBuilder.Length = 0;
                    }

                    break;
                }

                default:
                    break;
            }

            mLastClosedBlock = block;
        }

        /// <summary>
        /// Writes a specified text either directly to <see cref="mBuilder"/> or
        /// to <see cref="mHtmlBuilder"/> for writing it later when raw HTML block will end.
        /// </summary>
        internal void WriteText(string text)
        {
            HtmlTagBlock htmlTagBlock = mLastOpenedBlock as HtmlTagBlock;
            if (IsInHtml && htmlTagBlock != null)
            {
                switch (htmlTagBlock.TagName)
                {
                    // Don't write whitespace characters located between some HTML tags.
                    // This can be also SoftLineBreaks.
                    case "tr":
                    case "table":
                    {
                        if (!string.IsNullOrEmpty(text) && text.Trim().Length > 0)
                            mBuilder.Write(text);
                        break;
                    }

                    case "td":
                    {
                        // If 'td' block is still opened, then we can write text into it.
                        // Otherwise, write text directly to the document Builder.
                        if (IsInHtmlCell)
                            mHtmlBuilder.Append(text);
                        else if (!string.IsNullOrEmpty(text) && text.Trim().Length > 0)
                            mBuilder.Write(text);

                        break;
                    }

                    default:
                    {
                        // As we inside raw HTML block, then append text to HTML StringBuilder.
                        mHtmlBuilder.Append(text);
                        break;
                    }
                }
            }
            else
            {
                mBuilder.Write(text);
            }
        }

        /// <summary>
        /// Opens <see cref="HtmlTagBlock"/>.
        /// </summary>
        /// <remarks>
        /// Sometimes it is better to process HTML tags directly, without using <see cref="DocumentBuilder.InsertHtml(string)"/>.
        /// For, example, it allows us to set proper 'InlineCode' style name if encounter 'code' HTML tag.
        /// And it works faster, then InsertHtml().
        /// </remarks>
        private bool OpenHtmlInline(HtmlTagBlock block)
        {
            switch (block.TagName)
            {
                case "p":
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                {
                    // h1-h6 HTML elements are inline and must be at least inside InlineContainer block.
                    Debug.Assert(mLastOpenedBlock != null);
                    // If this is not a very first child, then we need to cut current Paragraph and start a new one.
                    if (!block.IsFirstChild)
                    {
                        HtmlTagBlock lastClosedBlock = mLastClosedBlock as HtmlTagBlock;
                        if ((lastClosedBlock == null) || !lastClosedBlock.IsParaOrHeading)
                        {
                            ApplyFormatting(mBuilder.CurrentParagraph.ParaPr, mLastOpenedBlock);

                            // There is ending paragraph already in just inserted Cell node,
                            // so no need to start a new paragraph in this case.
                            if ((mLastOpenedBlock.Type != BlockType.HtmlTag) || !((HtmlTagBlock)mLastOpenedBlock).IsCell)
                                StartNewPara();
                        }
                    }

                    ApplyFormatting(mBuilder.CurrentParagraph.ParaPr, block);
                    break;
                }

                case "i":
                {
                    mBuilder.Italic = true;
                    mItalicCount++;
                    break;
                }

                case "sup":
                {
                    mBuilder.Font.Superscript = true;
                    mSuperscriptCount++;
                    break;
                }

                case "sub":
                {
                    mBuilder.Font.Subscript = true;
                    mSubscriptCount++;
                    break;
                }

                case "strong":
                case "b":
                {
                    mBuilder.Bold = true;
                    mBoldCount++;
                    break;
                }

                case "u":
                {
                    mBuilder.Font.Underline = Underline.Single;
                    mUnderlineCount++;
                    break;
                }

                case "strike":
                case "del":
                case "s":
                {
                    mBuilder.Font.StrikeThrough = true;
                    mStrikethroughCount++;
                    break;
                }

                case "br":
                {
                    ApplyFormatting(mBuilder.CurrentParagraph.ParaPr, block);
                    StartNewPara();
                    break;
                }

                case "code":
                {
                    Style style = FetchStyle(MarkdownUtil.InlineCodeStyleName, StyleType.Character);
                    mBuilder.Font.Style = style;
                    DecorateInlineCode(style);
                    break;
                }

                default:
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Closes <see cref="HtmlTagBlock"/>.
        /// </summary>
        /// See some details in <see cref="OpenHtmlInline"/>.
        private bool CloseHtmlInline(HtmlTagBlock block)
        {
            switch (block.TagName)
            {
                case "p":
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                {
                    if (!block.IsLastChild)
                        StartNewPara();
                    break;
                }

                case "i":
                {
                    mItalicCount--;
                    mBuilder.Italic = (mItalicCount > 0);
                    break;
                }

                case "b":
                case "strong":
                {
                    mBoldCount--;
                    mBuilder.Bold = (mBoldCount > 0);
                    break;
                }

                case "sup":
                {
                    mSuperscriptCount--;
                    mBuilder.Font.Superscript = (mSuperscriptCount > 0);
                    break;
                }

                case "sub":
                {
                    mSubscriptCount--;
                    mBuilder.Font.Subscript = (mSubscriptCount > 0);
                    break;
                }

                case "u":
                {
                    mUnderlineCount--;
                    if (mUnderlineCount == 0)
                        mBuilder.Font.Underline = Underline.None;
                    break;
                }

                case "strike":
                case "del":
                case "s":
                {
                    mStrikethroughCount--;
                    mBuilder.Font.StrikeThrough = (mStrikethroughCount > 0);
                    break;
                }

                case "br":
                {
                    ApplyFormatting(mBuilder.CurrentParagraph.ParaPr, block);
                    StartNewPara();
                    break;
                }

                case "code":
                {
                    mBuilder.Font.Style = mBuilder.Document.Styles.GetBySti(StyleIdentifier.DefaultParagraphFont, true);
                    break;
                }

                default:
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Applies formatting to a specified <see cref="ParaPr"/> by a specified <see cref="Block"/>.
        /// </summary>
        private void ApplyFormatting(ParaPr paraPr, Block block)
        {
            Style style = GetStyle(block);
            Debug.Assert(style != null);

            paraPr[ParaAttr.Istd] = style.Istd;

            // IN. There is slightly complicated behavior for HorizontalRules in specification editor at
            // https://spec.commonmark.org/dingus/ (see TestBulletListK and TestBulletListF). It applies an
            // empty bullet to a first horizontal rule in a list and moves down real non-empty bullet to a
            // first non-horizontal rule paragraph inside the same list. However, it is not obvious behavior
            // that is not somehow explained in the specification. As it very complicates code and seems is not
            // very frequent case, lets postpone this for a while.
            ApplyList(paraPr, block);
        }

        /// <summary>
        /// Starts a new paragraph.
        /// </summary>
        private void StartNewPara()
        {
            // To close paragraph insert paragraph break.
            if (mFootnote == null)
                mBuilder.Writeln();
        }

        /// <summary>
        /// Gets a style for a specified block.
        /// </summary>
        private Style GetStyle(Block block)
        {
            Style style = null;

            Block prevBlock = null;
            Block curBlock = block;

            // This a hashcode for the current chain of blocks.
            int curHashCode = 0;

            bool canSkipBlock = false;

            while ((curBlock != null) && (curBlock.Type != BlockType.Document))
            {
                Style baseStyle = style;

                curHashCode = GetHashCode(curBlock, curHashCode);

                style = mBlocksChainHashcodeToStyle[curHashCode];
                if (style == null)
                {
                    switch (curBlock.Type)
                    {
                        case BlockType.Paragraph:
                        {
                            if (IsParaOrHeading(prevBlock))
                                canSkipBlock = true;
                            break;
                        }

                        case BlockType.HtmlTag:
                        {
                            if (IsParaOrHeading(prevBlock) && ((HtmlTagBlock)block).IsParaOrHeading)
                                canSkipBlock = true;
                            break;
                        }

                        case BlockType.SetextHeading:
                        {
                            // For setext heading we want a corresponding Heading style to be the base style.
                            AtxHeadingBlock atxHeadingBlock = new AtxHeadingBlock();
                            atxHeadingBlock.Level = ((HeadingBlock)curBlock).Level;
                            baseStyle = GetStyle(atxHeadingBlock);
                            break;
                        }

                        case BlockType.OrderedListItem:
                        case BlockType.BulletListItem:
                        {
                            if (prevBlock != null)
                            {
                                // We can omit list style for this block if this is a terminal list (contains only a Leaf block)
                                // or its child non-leaf block is not first in this list.
                                if ((prevBlock.BlockLevel == MarkdownBlockLevel.Leaf) || !prevBlock.IsFirstChild)
                                    canSkipBlock = true;
                            }
                            break;
                        }
                        default:
                            break;
                    }

                    if (canSkipBlock)
                    {
                        style = baseStyle;
                        canSkipBlock = false;
                    }
                    else
                    {
                        style = GetExistingStyle(curBlock, baseStyle);

                        // If style is not found in an existing collection, then create a new one.
                        if (style == null)
                            style = CreateStyle(curBlock, baseStyle, curHashCode);
                    }
                }

                prevBlock = curBlock;
                curBlock = curBlock.Parent;
            }

            return style;
        }

        /// <summary>
        /// Returns true, if a specified block represents paragraph or heading.
        /// </summary>
        private static bool IsParaOrHeading(Block block)
        {
            if (block == null)
                return false;

            if (block.Type == BlockType.Paragraph)
                return true;

            if ((block.Type == BlockType.HtmlTag) && ((HtmlTagBlock)block).IsParaOrHeading)
                return true;

            return false;
        }

        /// <summary>
        /// Returns true, if a specified block represents a table.
        /// </summary>
        private static bool IsTable(Block block)
        {
            if (block == null)
                return false;

            if (block.Type == BlockType.Table)
                return true;

            if ((block.Type == BlockType.HtmlTag) && ((HtmlTagBlock)block).IsTable)
                return true;

            return false;
        }

        /// <summary>
        /// Creates a style for the specified block with the specified base style and hash code.
        /// </summary>
        private Style CreateStyle(Block block, Style baseStyle, int hashCode)
        {
            string styleName = GetStyleName(block);
            Style style = FetchStyle(styleName, StyleType.Paragraph);
            if ((style.StyleIdentifier != StyleIdentifier.Normal) && baseStyle != null)
                style.BasedOnIstd = baseStyle.Istd;

            Decorate(style, block);
            mBlocksChainHashcodeToStyle.Add(hashCode, style);

            return style;
        }

        /// <summary>
        /// Gets hash code for a specified block.
        /// </summary>
        /// <remarks>
        /// This method allows to produce hash codes for the chains of blocks and
        /// allows to compare them effectively for building corresponding hierarchies of styles.
        /// </remarks>
        private static int GetHashCode(Block block, int initialHashCodeValue)
        {
            unchecked
            {
                int hashCode;
                switch (block.Type)
                {
                    case BlockType.AtxHeading:
                    case BlockType.SetextHeading:
                    {
                        hashCode = (int)block.Type;
                        hashCode = (hashCode * 397) ^ ((HeadingBlock)block).Level;
                        break;
                    }

                    case BlockType.HtmlTag:
                    {
                        hashCode = (int)block.Type;
                        hashCode = (hashCode * 397) ^ ((HtmlTagBlock)block).HeadingTagLevel;
                        break;
                    }

                    case BlockType.BulletListItem:
                    case BlockType.OrderedListItem:
                    {
                        ListItemBlock listItemBlock = (ListItemBlock)block;
                        hashCode = (int)block.Type;
                        hashCode = (hashCode * 397) ^ listItemBlock.StartAt;
                        hashCode = (hashCode * 397) ^ (int)listItemBlock.Marker;
                        hashCode = (hashCode * 397) ^ listItemBlock.GetLevel();
                        break;
                    }
                    case BlockType.FencedCode:
                    {
                        hashCode = (int)block.Type;
                        hashCode = (hashCode * 397) ^ ((FencedCodeBlock)block).Info.GetHashCode();
                        break;
                    }
                    case BlockType.HorizontalRule:
                        hashCode = (int)BlockType.Paragraph;
                        break;
                    default:
                        hashCode = (int)block.Type;
                        break;
                }

                return (initialHashCodeValue * 397) ^ hashCode;
            }
        }

        /// <summary>
        /// Fetches style with a specified name.
        /// </summary>
        private Style FetchStyle(string name, StyleType styleType)
        {
            // Try get an existing style.
            Style style = mBuilder.Document.Styles.GetByName(name, true);

            // Create a new style.
            if (style == null)
                style = mBuilder.Document.Styles.Add(styleType, name);

            return style;
        }

        /// <summary>
        /// Returns style name for a specified block.
        /// </summary>
        private string GetStyleName(Block block)
        {
            string styleName;

            switch (block.Type)
            {
                case BlockType.Quote:
                case BlockType.BulletListItem:
                case BlockType.OrderedListItem:
                case BlockType.FootnoteDefinition:
                {
                    styleName = GetUniqueStyleName(block);
                    break;
                }
                case BlockType.AtxHeading:
                {
                    styleName = string.Format("{0} {1}", MarkdownUtil.HeadingStyleName, ((HeadingBlock)block).Level);
                    break;
                }
                case BlockType.HtmlTag:
                {
                    int level = ((HtmlTagBlock)block).HeadingTagLevel;
                    styleName = level == -1 ? "Normal" : string.Format("{0} {1}", MarkdownUtil.HeadingStyleName, level);
                    break;
                }
                case BlockType.SetextHeading:
                {
                    styleName = string.Format("{0}{1}", MarkdownUtil.SetextHeadingStyleName, ((HeadingBlock)block).Level);
                    break;
                }
                case BlockType.IndentedCode:
                {
                    styleName = MarkdownUtil.IndentedCodeStyleName;
                    break;
                }
                case BlockType.FencedCode:
                {
                    string infoString = ((FencedCodeBlock)block).Info;
                    styleName = string.Format("{0}{1}{2}",
                        MarkdownUtil.FencedCodeStyleName, (infoString.Length > 0) ? "." : "", infoString);
                    break;
                }
                default:
                    styleName = "Normal";
                    break;
            }

            return styleName;
        }

        /// <summary>
        /// Gets unique style name for a specified Block-level block.
        /// </summary>
        private string GetUniqueStyleName(Block block)
        {
            Debug.Assert(block.BlockLevel == MarkdownBlockLevel.Block);

            string baseStyleName;
            switch (block.Type)
            {
                case BlockType.Quote:
                    baseStyleName = MarkdownUtil.QuoteStyleName;
                    break;
                case BlockType.BulletListItem:
                case BlockType.OrderedListItem:
                    baseStyleName = MarkdownUtil.ListStyleName;
                    break;
                case BlockType.FootnoteDefinition:
                    baseStyleName = MarkdownUtil.FootnoteStyleName;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unexpected block type: {0}", block.Type));
            }

            string uniqueStyleName;
            int usedCount = mUsedBlockStyles[baseStyleName];
            if (StringToIntDictionary.IsNullSubstitute(usedCount))
            {
                uniqueStyleName = baseStyleName;
                usedCount = 1;
            }
            else
            {
                uniqueStyleName = string.Format("{0}{1}", baseStyleName, usedCount);
                usedCount++;
            }

            mUsedBlockStyles[baseStyleName] = usedCount;
            return uniqueStyleName;
        }

        /// <summary>
        /// Gets style with a specified base style from an existing collection.
        /// </summary>
        private Style GetExistingStyle(Block block, Style baseStyle)
        {
            int listLevel = -1;
            string styleName;

            switch (block.Type)
            {
                case BlockType.Quote:
                {
                    styleName = MarkdownUtil.QuoteStyleName;
                    break;
                }
                case BlockType.BulletListItem:
                case BlockType.OrderedListItem:
                {
                    styleName = MarkdownUtil.ListStyleName;
                    listLevel = ((ListItemBlock)block).GetLevel();
                    break;
                }
                default:
                    return null;
            }

            return GetExistingStyle(block, baseStyle, styleName, listLevel);
        }

        /// <summary>
        /// Gets existing style with a specified name, base style and list level for a specified block.
        /// </summary>
        private Style GetExistingStyle(Block block, Style baseStyle, string styleName, int listLevel)
        {
            Debug.Assert(StringUtil.HasChars(styleName));

            foreach (Style existingStyle in mBlocksChainHashcodeToStyle.Values)
            {
                if ((existingStyle.Name.StartsWith(styleName, StringComparison.Ordinal) &&
                    (existingStyle.GetBaseStyle() == baseStyle)))
                {
                    List list = existingStyle.GetListInternal();

                    // Block and style has no list, so it is suitable.
                    if ((listLevel == -1) && (list == null))
                        return existingStyle;

                    // Both: block and style has list. Check they are suitable.
                    if ((listLevel != -1) && (list != null))
                    {
                        int styleListLevel = (int)existingStyle.ParaPr.FetchAttr(ParaAttr.ListLevel);
                        if (((styleListLevel == listLevel) || (block.Type == BlockType.BulletListItem)) &&
                            IsSuitableList(list, (ListItemBlock)block))
                            return existingStyle;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Starts a new paragraph inside Footnote.
        /// </summary>
        private void StartFootnoteParagraph()
        {
            Paragraph footnotePara;
            // The very first paragraph of Footnote definition was auto-created along with the Footnote
            // and originally contains only the footnote's marker with rest of text empty.
            // So, we just re-use it.
            // Note, even if this is a completely empty paragraph, it will have at least 2 children:
            // footnote Reference and space char after it as separator.
            if ((mFootnote.Paragraphs.Count == 1) && (mFootnote.Paragraphs[0].GetChildNodes(NodeType.Any, false).Count == 1))
            {
                footnotePara = mFootnote.Paragraphs[0];
            }
            else
            {
                footnotePara = new Paragraph(mBuilder.Document);
                mFootnote.Paragraphs.Add(footnotePara);
            }

            mBuilder.MoveTo(footnotePara);

            // Write one space character as a separator between Footnote marker and text for a very first line.
            if (mFootnote.Paragraphs.Count == 1)
                mBuilder.Write(" ");
        }

        /// <summary>
        /// Gets InlineCode style name.
        /// </summary>
        private static string GetInlineCodeStyleName(InlineCodeBlock block)
        {
            string delimiterLength = (block.DelimiterLength > 1) ? string.Format(".{0}", block.DelimiterLength) : "";
            return string.Format("{0}{1}", MarkdownUtil.InlineCodeStyleName, delimiterLength);
        }

        /// <summary>
        /// Decorates a specified style.
        /// </summary>
        private void Decorate(Style style, Block block)
        {
            switch (block.Type)
            {
                case BlockType.BulletListItem:
                    DecorateBulletList(style, (BulletListItemBlock)block);
                    break;
                case BlockType.OrderedListItem:
                    DecorateOrderedList(style, (OrderedListItemBlock)block);
                    break;
                case BlockType.Quote:
                    DecorateQuote(style);
                    break;
                case BlockType.IndentedCode:
                case BlockType.FencedCode:
                    DecorateCode(style);
                    break;
                case BlockType.InlineCode:
                    DecorateInlineCode(style);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Decorates BulletList.
        /// </summary>
        private void DecorateBulletList(Style style, BulletListItemBlock block)
        {
            List list = FetchBulletList(block.Marker);
            style.ParaPr.ListId = list.ListId;
            style.ParaPr.ListLevel = block.GetLevel();
        }

        /// <summary>
        /// Decorates OrderedList.
        /// </summary>
        private void DecorateOrderedList(Style style, OrderedListItemBlock block)
        {
            ApplyList(style.ParaPr, block);
        }

        /// <summary>
        /// Decorates Quote.
        /// </summary>
        private static void DecorateQuote(Style style)
        {
            style.ParaPr.BorderLeft = new Border(LineStyle.Single, 18, DrColor.FromArgb(0x9F, 0x9F, 0x9F));
        }

        /// <summary>
        /// Decorates InlineCode.
        /// </summary>
        private static void DecorateInlineCode(Style style)
        {
            style.RunPr.Name = "Consolas";
            style.RunPr.Color = DrColor.FromArgb(0xC7, 0x25, 0x4E);
            style.RunPr.HighlightColor = DrColor.FromArgb(0xF9, 0xF2, 0xF4);
            style.RunPr.Size = 20;
            style.RunPr.SizeBi = 20;
        }

        /// <summary>
        /// Decorates Code (Fenced or Indented).
        /// </summary>
        private static void DecorateCode(Style style)
        {
            Shading shading = new Shading();
            shading.ForegroundPatternColor = Color.FromArgb(0xE2, 0xE2, 0xE2);
            shading.Texture = TextureIndex.TextureSolid;
            style.ParaPr.Shading = shading;

            style.RunPr.Name = "Consolas";
            style.RunPr.Size = 20;
            style.RunPr.SizeBi = 20;
        }

        /// <summary>
        /// Gets a boolean value indicating a specified list can be used for a specified list item block.
        /// </summary>
        private bool IsSuitableList(List list, ListItemBlock listItemBlock)
        {
            if (list == null)
                return false;

            // There cannot be different containers for the same list.
            if ((listItemBlock.Type == BlockType.OrderedListItem) &&
                (listItemBlock.GetListContainer() != mListContainers[list.ListId]))
                return false;

            // For a bullet list item, the marker (and other info) should be obtained from the very first level.
            int level = (listItemBlock.Type == BlockType.OrderedListItem) ? listItemBlock.GetLevel() : 0;
            ListLevel listLevel = list.ListLevels[level];

            // Check the level was not initialized yet.
            if (listLevel.NumberFormat.Length == 0)
                return true;

            ListMarker marker = MarkdownUtil.ToListMarker(listLevel.NumberFormat[listLevel.NumberFormat.Length -1]);
            if (listItemBlock.Marker != marker)
                return false;

            // For the very first block in level it is also important to start at the value specified for this level.
            if ((listItemBlock.Type == BlockType.OrderedListItem) &&
                (listItemBlock.IsLevelStart && (listLevel.StartAt != listItemBlock.StartAt)))
                return false;

            return true;
        }

        /// <summary>
        /// Applies list to a specified paraPr in accordance with a specified block.
        /// </summary>
        private void ApplyList(ParaPr paraPr, Block block)
        {
            ListItemBlock listItemBlock = block as ListItemBlock;
            if (listItemBlock == null)
                listItemBlock = (ListItemBlock)block.GetParent(BlockType.BulletListItem, BlockType.OrderedListItem);

            if (listItemBlock != null)
                SetList(paraPr, block, listItemBlock);
            else
                RemoveList(paraPr);
        }

        /// <summary>
        /// Sets list to a specified paraPr by a specified block in accordance with a specified list item block.
        /// </summary>
        private void SetList(ParaPr paraPr, Block block, ListItemBlock listItemBlock)
        {
            Block b = block;
            if ((block.Type == BlockType.HtmlTag) && ((HtmlTagBlock)block).IsParaOrHeading)
            {
                b = block.GetParent(BlockType.Paragraph);
                Debug.Assert(b != null);
            }

            // Try to mimic non-visible list marker of the spec editor in Word by writing
            // the blank number format for leaf blocks. It seems no matter which number style to
            // use in this case, as it is blank, so always apply a bullet list.
            ListMarker marker = (((b.Parent != listItemBlock) || !b.IsFirstChild) && (b != listItemBlock))
                ? ListMarker.None
                : listItemBlock.Marker;

            List list = ((marker == ListMarker.Dot) || (marker == ListMarker.Parenthesis))
                ? FetchOrderedList(listItemBlock)
                : FetchBulletList(marker);

            int listLevel = listItemBlock.GetLevel();
            if (marker != ListMarker.None)
                mLevelLists[listLevel] = list;

            paraPr[ParaAttr.ListId] = list.ListId;
            paraPr[ParaAttr.ListLevel] = listLevel;
            }

        /// <summary>
        /// Removes list from a specified ParaPr and map of ListContainers.
        /// </summary>
        private void RemoveList(ParaPr paraPr)
            {
                int listId = (int)paraPr.FetchAttr(ParaAttr.ListId);
                if (listId != 0)
                    mListContainers.Remove(listId);

                paraPr.Remove(ParaAttr.ListId);
                paraPr.Remove(ParaAttr.ListLevel);
            }

        /// <summary>
        /// Gets ordered list by a specified list item block.
        /// </summary>
        private List GetOrderedList(ListItemBlock listItemBlock)
        {
            Debug.Assert(listItemBlock.Type == BlockType.OrderedListItem);

            int listLevel = listItemBlock.GetLevel();

            // Check list for the listItemBlock level was already created.
            List list = mLevelLists[listLevel];

            // If it was not created for that level, then check previous level.
            if ((list == null) && (listLevel > 0))
                list = mLevelLists[listLevel - 1];

            // Check the list is suitable for the specified list item block.
            if (!IsSuitableList(list, listItemBlock))
                return null;

            return list;
        }

        /// <summary>
        /// Fetches ordered list by a specified list item block.
        /// </summary>
        private List FetchOrderedList(ListItemBlock listItemBlock)
        {
            Debug.Assert(listItemBlock.Type == BlockType.OrderedListItem);

            List list = GetOrderedList(listItemBlock);
            if (list == null)
            {
                list = mBuilder.Document.Lists.AddEmpty(ListType.HybridMultiLevel);
                mListContainers.Add(list.ListId, listItemBlock.GetListContainer());
            }

            int level = listItemBlock.GetLevel();
            ListLevel listLevel = list.ListLevels[level];
            if (listLevel.NumberFormat.Length == 0)
                InitOrderedListLevel(list, level, listItemBlock);

            return list;
        }

        /// <summary>
        /// Gets bullet list by a specified marker.
        /// </summary>
        private List GetBulletList(ListMarker marker)
        {
            List list = BulletMarkerToList[(int)marker];
            return (IntToObjDictionary<List>.IsNullSubstitute(list)) ? null : list;
        }

        /// <summary>
        /// Fetches bullet list by a specified marker.
        /// </summary>
        private List FetchBulletList(ListMarker marker)
        {
            List list = GetBulletList(marker);
            if (list == null)
            {
                list = CreateBulletList(marker);
                BulletMarkerToList.Add((int)marker, list);
            }

            return list;
        }

        /// <summary>
        /// Creates a bullet list corresponded to a specified marker.
        /// </summary>
        private List CreateBulletList(ListMarker marker)
        {
            List list = ListFactory.CreateList(mBuilder.Document.Lists, ListTemplate.BulletDefault);
            list.ListLevels[0].RunPr.SetSymbolFonts(ListFactory.CourierFont);

            string bullet = MarkdownUtil.ListMarkerToChar(marker).ToString();
            list.ListLevels[0].NumberFormat = bullet;

            if (marker == ListMarker.None)
            {
                for (int i = 1; i < 9; i++)
                    list.ListLevels[i].NumberFormat = bullet;
            }

            return list;
        }

        /// <summary>
        /// Initializes an ordered list level in accordance with a specified list item block.
        /// </summary>
        private static void InitOrderedListLevel(List list, int level, ListItemBlock listItemBlock)
        {
            Debug.Assert(listItemBlock.Type == BlockType.OrderedListItem);

            int levelIndent = 720 * (level + 1);
            string numberFormat = string.Format("{0}{1}", (char)level, MarkdownUtil.ListMarkerToChar(listItemBlock.Marker));
            ListLevelInitializer.InitNumberListLevel(list, level,
                NumberStyle.Arabic, numberFormat, ListLevelAlignment.Left, levelIndent, levelIndent, -360);

            list.ListLevels[level].StartAt = listItemBlock.StartAt;
        }

        /// <summary>
        /// Saves current Builder formatting and emphases counters.
        /// </summary>
        private void SaveCurrentFormatting()
        {
            mBuilder.PushFont();
            mBuilder.PushParaPr();

            mSavedBoldCount = mBoldCount;
            mSavedItalicCount = mItalicCount;
            mSavedStrikethroughCount = mStrikethroughCount;
            mSavedUnderlineCount = mUnderlineCount;
            mSavedSuperscriptCount = mSuperscriptCount;
            mSavedSubscriptCount = mSubscriptCount;
        }

        /// <summary>
        /// Restores saved Builder formatting and emphases counters.
        /// </summary>
        private void RestoreFormatting()
        {
            mBuilder.PopFont();
            mBuilder.PopParaPr();

            mBoldCount = mSavedBoldCount;
            mItalicCount = mSavedItalicCount;
            mStrikethroughCount = mSavedStrikethroughCount;
            mUnderlineCount = mSavedUnderlineCount;
            mSuperscriptCount = mSavedSuperscriptCount;
            mSubscriptCount = mSavedSubscriptCount;
        }

        /// <summary>
        /// Clears current Builder formatting and all emphases counters.
        /// </summary>
        private void ClearFormatting()
        {
            mBuilder.ClearFont();
            mBuilder.CurrentParagraph.ParaPr.Clear();

            mBoldCount = 0;
            mItalicCount = 0;
            mStrikethroughCount = 0;
            mUnderlineCount = 0;
        }

        /// <summary>
        /// Increments number of started HTML tags with a specified name in <see cref="mHtmlTagStarts"/>.
        /// </summary>
        private void IncHtmlTagsStartCount(string name)
        {
            AddToHtmlTagsStartCount(name, 1);
        }

        /// <summary>
        /// Decrements number of started HTML tags with a specified name in <see cref="mHtmlTagStarts"/>.
        /// </summary>
        private void DecHtmlTagsStartCount(string name)
        {
            AddToHtmlTagsStartCount(name, -1);
        }

        /// <summary>
        /// Adds a specified value to a number of started HTML tags with a specified name in <see cref="mHtmlTagStarts"/>.
        /// </summary>
        private void AddToHtmlTagsStartCount(string name, int value)
        {
            int count = mHtmlTagStarts[name];
            if (StringToIntDictionary.IsNullSubstitute(count))
                count = 0;

            if (count <= 0 && value < 0)
                return;

            mHtmlTagStarts[name] = count + value;
        }

        /// <summary>
        /// Gets related DocumentBuilder object.
        /// </summary>
        internal DocumentBuilder Builder
        {
            get { return mBuilder; }
        }

        /// <summary>
        /// Gets map of bullet markers to corresponding lists.
        /// </summary>
        private IntToObjDictionary<List> BulletMarkerToList
        {
            get
            {
                if (mBulletMarkerToList == null)
                    mBulletMarkerToList = new IntToObjDictionary<List>(4);

                return mBulletMarkerToList;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether we are now in raw HTML.
        /// </summary>
        private bool IsInHtml
        {
            get { return HtmlTagsStartedCount > 0; }
        }

        /// <summary>
        /// Gets a boolean value indicating whether we are inside HTML cell.
        /// </summary>
        private bool IsInHtmlCell
        {
            get
            {
                HtmlTagBlock start = mLastOpenedBlock as HtmlTagBlock;
                if (start == null)
                    return false;

                if (start.TagName != "td")
                    return false;

                HtmlTagBlock end = mLastClosedBlock as HtmlTagBlock;
                if (end == null)
                    return true;

                if (end.TagName != "td")
                    return true;

                // Actually HTML tag pair can be even in another Paragraph. But for a while let's just check
                // this simple case with common parent.
                Block nextSibling = start.NextSibling;
                while (nextSibling != null)
                {
                    // If </td> is after <td>, then this <td> is closed. Then we are not inside a cell.
                    if (end == nextSibling)
                        return false;

                    nextSibling = nextSibling.NextSibling;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets integer value representing number of times HTML tags are started at the moment.
        /// </summary>
        private int HtmlTagsStartedCount
        {
            get
            {
                int count = 0;
                foreach (string tagName in mHtmlTagStarts.Keys)
                    count += mHtmlTagStarts[tagName];

                return count;
            }
        }

        /// <summary>
        /// Static Ctor.
        /// </summary>
        static MarkdownReaderContext()
        {
            gKnownHtmlTagNames.Add("p");
            gKnownHtmlTagNames.Add("h1");
            gKnownHtmlTagNames.Add("h2");
            gKnownHtmlTagNames.Add("h3");
            gKnownHtmlTagNames.Add("h4");
            gKnownHtmlTagNames.Add("h5");
            gKnownHtmlTagNames.Add("h6");
            gKnownHtmlTagNames.Add("i");
            gKnownHtmlTagNames.Add("b");
            gKnownHtmlTagNames.Add("u");
            gKnownHtmlTagNames.Add("sup");
            gKnownHtmlTagNames.Add("sub");
            gKnownHtmlTagNames.Add("strong");
            gKnownHtmlTagNames.Add("strike");
            gKnownHtmlTagNames.Add("del");
            gKnownHtmlTagNames.Add("s");
            gKnownHtmlTagNames.Add("br");
            gKnownHtmlTagNames.Add("table");
            gKnownHtmlTagNames.Add("tr");
            gKnownHtmlTagNames.Add("td");
            gKnownHtmlTagNames.Add("code");
            gKnownHtmlTagNames.Add("html");
            gKnownHtmlTagNames.Add("head");
            gKnownHtmlTagNames.Add("meta");
            gKnownHtmlTagNames.Add("title");
            gKnownHtmlTagNames.Add("div");
            gKnownHtmlTagNames.Add("span");
            gKnownHtmlTagNames.Add("body");
        }

        /// <summary>
        /// The last opened block.
        /// </summary>
        private Block mLastOpenedBlock;

        /// <summary>
        /// The last closed block.
        /// </summary>
        private Block mLastClosedBlock;

        private readonly DocumentBuilder mBuilder;
        private IntToObjDictionary<List> mBulletMarkerToList;

        /// <summary>
        /// Maps hashcode of block chains (block hierarchies) to a Style.
        /// </summary>
        private readonly IntToObjDictionary<Style> mBlocksChainHashcodeToStyle = new IntToObjDictionary<Style>();

        /// <summary>
        /// Keeps last List applied to a corresponding level.
        /// </summary>
        private readonly List[] mLevelLists = new List[9];

        /// <summary>
        /// Maps Lists to ContainerBlocks they are belonging.
        /// </summary>
        private readonly IntToObjDictionary<ContainerBlock> mListContainers = new IntToObjDictionary<ContainerBlock>();

        /// <summary>
        /// All Block styles should have unique ID.
        /// </summary>
        /// <remarks>
        /// For a moment there can be only 3 types of Block level blocks.
        /// The key: style name
        /// The value: the number of times the style is used (i.e. current style index)
        /// </remarks>
        private readonly StringToIntDictionary mUsedBlockStyles = new StringToIntDictionary(3);

        private int mBoldCount;
        private int mItalicCount;
        private int mStrikethroughCount;
        private int mUnderlineCount;
        private int mSuperscriptCount;
        private int mSubscriptCount;

        /// <summary>
        /// The currently started Footnote.
        /// </summary>
        /// <returns> If it is not inside a Footnote, then <c>null</c>, otherwise current Footnote node. </returns>
        private Footnote mFootnote;

        private int mSavedBoldCount;
        private int mSavedItalicCount;
        private int mSavedStrikethroughCount;
        private int mSavedUnderlineCount;
        private int mSavedSuperscriptCount;
        private int mSavedSubscriptCount;

        /// <summary>
        /// Accumulates raw HTML until all started HTML tags are not ended and then inserts it into a Document.
        /// </summary>
        private readonly StringBuilder mHtmlBuilder = new StringBuilder();

        /// <summary>
        /// Keeps number of times particular HTML tag is currently started.
        /// Key: tag name.
        /// Value: count.
        /// </summary>
        private readonly StringToIntDictionary mHtmlTagStarts = new StringToIntDictionary();

        /// <summary>
        /// Keeps known (that will be actually processed as HTML and not as simple text) HTML tags.
        /// </summary>
        private static readonly CaseInsensitiveHashSet gKnownHtmlTagNames = new CaseInsensitiveHashSet();
    }
}
