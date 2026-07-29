// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/03/2019 by Ilya Navrotskiy

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aspose.Collections.Generic;
using Aspose.Words.Loading;
using Aspose.Words.RW.Markdown.Reader;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown document.
    /// </summary>
    internal class MarkdownDocument : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MarkdownDocument"/> class.
        /// </summary>
        internal MarkdownDocument()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MarkdownDocument"/> class.
        /// </summary>
        internal MarkdownDocument(MarkdownLoadOptions loadOptions)
        {
            LoadOptions = loadOptions;
        }

        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            return false;
        }

        /// <summary>
        /// Reads a content from a specified reader into the markdown document.
        /// </summary>
        internal void Read(StreamReader reader, LoadingProgressProcessor progressProcessor)
        {
            string txtLine = reader.ReadLine();

            // Note, a markdown block can occupy NOT whole line of a text,
            // so this is a start position of the block inside a current line.
            int start = 0;
            while (txtLine != null)
            {
                // WORDSNET-22892 Adding a Callback event that notifies the progress of loading document.
                if (progressProcessor != null)
                    progressProcessor.Execute(reader.BaseStream);

                mCurParsedBlock = mBlockParser.Parse(txtLine, start);

                // Actually, any line of a text MUST be parsed successfully. Even if it was not parsed
                // as a some special block type, it had to be parsed as a regular paragraph.
                if (mCurParsedBlock == null)
                    throw new InvalidOperationException("Cannot parse a line of text.");

                // Check if the current block falls under some parent list item block, then reparse it appropriately.
                // WORDSNET-26368 Do not reparse Table block as list item. Actually, I think Table block can be
                // inside a list in Markdown. However, it can not be a list item in DOM. So, for a moment just
                // excluded from reparsing.
                ListItemBlock listItemBlock = null;
                if (mCurParsedBlock.Type != BlockType.Table)
                    listItemBlock = TryReparseAsListItem(txtLine, start);

                bool isAppended = TryAppend(mCurParsedBlock);

                 // WORDSNET-26064 Implemented a new load option.
                if ((LoadOptions != null) && LoadOptions.PreserveEmptyLines &&
                    (mCurParsedBlock.Type == BlockType.BlankLine))
                {
                    // IndentedCode allows blank lines by design,
                    // so do not add extra ones in this case.
                    if ((Last == null) || (Last.Type != BlockType.IndentedCode))
                        Add(new ParagraphBlock()).Close();
                }

                // Sometimes while we are trying to append a block there can be a situation
                // when processed block type MUST be excluded from the parser to allow
                // it to be parsed as a some another block type. For example, see SetextHeading.
                if (mCurParsedBlock.Action == BlockAction.Exclude)
                {
                    mBlockParser.Exclude(mCurParsedBlock.Type);
                    continue;
                }

                if (!isAppended)
                    AddBlock(mCurParsedBlock);

                if (listItemBlock != null)
                    listItemBlock.Action = BlockAction.None;

                // Some types might been excluded from a parser (see code above),
                // so initialize it again with the all possible types.
                mBlockParser.InitTypes();

                // Advance start position to a next block.
                start += mCurParsedBlock.Length;
                Debug.Assert(start <= txtLine.Length);

                // Cache footnote definitions to perform a quick resolve later.
                // Note, footnote references are inline level nodes, so they are not defined at this point
                // and will be parsed after call to ParseInlines() below.
                if (mCurParsedBlock.Type == BlockType.FootnoteDefinition)
                    mFootnoteDefinitions.Add((FootnoteDefinitionBlock)mCurParsedBlock);

                // If there is no remaining text in the current line, then read a new one.
                if ((mCurParsedBlock.Type == BlockType.EndOfLine) || (mCurParsedBlock.Type == BlockType.BlankLine))
                {
                    txtLine = reader.ReadLine();
                    start = 0;
                }
            }

            ParseLinkDefinitions();

            // Remove unnecessary last EOL from a tree, if any.
            Block lastBlock = GetDeepestOpenedChildBlock();
            if ((lastBlock != null) && (lastBlock.Type == BlockType.EndOfLine))
                lastBlock.Remove();

            // After all block structures are read, we can parse InlineContainer blocks.
            ParseInlines(this);
        }

        /// <summary>
        /// Adds a specified block to the root of the document.
        /// </summary>
        private void AddBlock(Block block)
        {
            // It is not necessary to keep blank blocks in a markdown tree.
            if ((block.Type != BlockType.BlankLine) && (block.Type != BlockType.EndOfLine))
                Add(block);
        }

        /// <summary>
        /// Parses link reference definition blocks.
        /// </summary>
        private void ParseLinkDefinitions()
        {
            List<InlineContainerBlock> emptyParas = new List<InlineContainerBlock>();
            foreach (Block childBlock in this)
            {
                if ((childBlock.Type == BlockType.Paragraph) || (childBlock.Type == BlockType.SetextHeading))
                {
                    InlineContainerBlock paragraphBlock = (InlineContainerBlock)childBlock;

                     // WORDSNET-26064 We have introduced a new option that allows to preserve blank lines.
                     // So, let's skip paragraphs that are blank lines originally to not remove them later
                     // at the end of parse process.
                    if (paragraphBlock.IsEmpty && (paragraphBlock.ContentLines.Count == 0))
                        continue;

                    StringBuilder linkDefinitionContent = new StringBuilder();
                    LinkDefinitionBlock linkDefinitionBlock = new LinkDefinitionBlock();

                    List<string> contentLines = paragraphBlock.ContentLines;
                    int startContentIndex = 0;
                    int endContentIndex = 0;
                    while (endContentIndex < contentLines.Count)
                    {
                        string linkDefCandidate = string.Format("{0} {1}",
                            linkDefinitionContent,
                            contentLines[endContentIndex]);
                        if (!linkDefinitionBlock.TryParse(linkDefCandidate, 0))
                        {
                            linkDefinitionBlock.Reset();
                            linkDefinitionBlock.TryParse(linkDefinitionContent.ToString(), 0);
                            linkDefinitionContent.Length = 0;
                            if (linkDefinitionBlock.IsValid())
                            {
                                endContentIndex = 0;
                                if (!mLinkDefinitions.ContainsKey(linkDefinitionBlock.Reference))
                                    mLinkDefinitions.Add(linkDefinitionBlock.Reference, linkDefinitionBlock);
                                contentLines.RemoveRange(startContentIndex, endContentIndex - startContentIndex + 1);
                                linkDefinitionBlock = new LinkDefinitionBlock();
                            }
                            else
                                endContentIndex++;

                            startContentIndex = endContentIndex;
                        }
                        else
                        {
                            linkDefinitionContent.AppendLine(contentLines[endContentIndex]);
                            endContentIndex++;
                        }
                    }

                    if (linkDefinitionBlock.IsValid())
                    {
                        endContentIndex--;
                        if (!mLinkDefinitions.ContainsKey(linkDefinitionBlock.Reference))
                            mLinkDefinitions.Add(linkDefinitionBlock.Reference, linkDefinitionBlock);
                        contentLines.RemoveRange(startContentIndex, endContentIndex - startContentIndex + 1);
                    }

                    if ((paragraphBlock.Count == 0) && (paragraphBlock.ContentLines.Count == 0))
                        emptyParas.Add(paragraphBlock);
                }
            }

            foreach (InlineContainerBlock paragraphBlock in emptyParas)
                paragraphBlock.Remove();
        }

        /// <summary>
        /// Parses text inside child InlineContainers onto Inline blocks.
        /// </summary>
        private static void ParseInlines(Block block)
        {
            InlineContainerBlock inlineContainer = block as InlineContainerBlock;
            if (inlineContainer != null)
            {
                inlineContainer.Parse();
            }
            else
            {
                ContainerBlock containerBlock = block as ContainerBlock;
                if (containerBlock != null)
                {
                    foreach (Block childBlock in containerBlock)
                        ParseInlines(childBlock);
                }
            }
        }

        /// <summary>
        /// Tries to reparse the current block as a list item.
        /// </summary>
        /// <returns>
        /// Returns a list item block where re-parsed block should be added,
        /// or <c>null</c> if current block does not fall under any list item.
        /// </returns>
        private ListItemBlock TryReparseAsListItem(string txtLine, int start)
        {
            ListItemBlock listItemBlock =
                (ListItemBlock)GetDeepestOpenedChildBlock(BlockType.BulletListItem, BlockType.OrderedListItem);

            while ((listItemBlock != null) && (listItemBlock.Action != BlockAction.ReparseListItem))
            {
                // In order to fall under the list item inside a Quote, the block must be in Quote too.
                if ((!listItemBlock.IsInQuote || (listItemBlock.GetParent(BlockType.Quote, false) != null))
                    && (mCurParsedBlock.Indentation >= listItemBlock.ListLabelLength))
                {
                    // The block indentation must be greater or equal to
                    // list label length in order to fall under the list item.
                    // Get number of characters to skip in original text for a new block.
                    int skipCount = MarkdownUtil.GetCharsCount(txtLine, start, listItemBlock.ListLabelLength);
                    // Reparse as block inside the list item.
                    mCurParsedBlock = mBlockParser.Parse(txtLine, start + skipCount);
                    if (mCurParsedBlock == null)
                        throw new InvalidOperationException("Cannot parse a line of text inside a list item.");

                    // Adjust an opening indentation sequence.
                    mCurParsedBlock.OpeningIndentation =
                        string.Format("{0}{1}", txtLine.Substring(start, skipCount), mCurParsedBlock.OpeningIndentation);

                    listItemBlock.Action = BlockAction.ReparseListItem;
                    break;
                }

                listItemBlock =
                    (ListItemBlock)listItemBlock.GetPreviousDeepTraversed(true,
                        BlockType.BulletListItem,
                        BlockType.OrderedListItem);
            }

            return listItemBlock;
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.Document; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Block; }
        }

        /// <summary>
        /// Gets Link Definitions.
        /// </summary>
        internal Dictionary<string, LinkDefinitionBlock> LinkDefinitions
        {
            get { return mLinkDefinitions; }
        }

        /// <summary>
        /// Gets Footnote Definitions.
        /// </summary>
        internal HashSetGeneric<FootnoteDefinitionBlock> FootnoteDefinitions
        {
            get { return mFootnoteDefinitions; }
        }

        /// <summary>
        /// Markdown load options.
        /// </summary>
        internal MarkdownLoadOptions LoadOptions { get; }

        private readonly BlockParser mBlockParser = new BlockParser();

        /// <summary>
        /// The currently parsed block.
        /// </summary>
        private Block mCurParsedBlock;

        /// <summary>
        /// The collection of FootnoteDefinitionBlocks in the document.
        /// </summary>
        private readonly HashSetGeneric<FootnoteDefinitionBlock> mFootnoteDefinitions =
            new HashSetGeneric<FootnoteDefinitionBlock>();

        private readonly Dictionary<string, LinkDefinitionBlock> mLinkDefinitions = new Dictionary<string, LinkDefinitionBlock>();
    }
}
