// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/03/2019 by Ilya Navrotskiy

using Aspose.Collections;

namespace Aspose.Words.RW.Markdown.Reader
{
    /// <summary>
    /// The class for parsing a text line into the <see cref="Block"/> object.
    /// </summary>
    internal class BlockParser
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal BlockParser()
        {
            InitTypes();
        }

        /// <summary>
        /// Initializes block collection with all possible block types.
        /// </summary>
        internal void InitTypes()
        {
            // If all possible types are already persisted in a collection, then nothing to do.
            if (mBlockTypes.Count == gAllBlockTypes.Length)
                return;

            mBlockTypes.Clear();
            foreach (BlockType type in gAllBlockTypes)
                mBlockTypes.Add((int)type);
        }

        /// <summary>
        /// Excludes specified type from a collection of block types,
        /// so that this type will not participate in a block parsing.
        /// </summary>
        internal void Exclude(BlockType type)
        {
            mBlockTypes.Remove((int) type);
        }

        /// <summary>
        /// Parses a text line starting from a specified position to a Block object.
        /// </summary>
        internal Block Parse(string txtLine, int start)
        {
            for (int i = 0; i < mBlockTypes.Count; i++)
            {
                Block block = TryParse(txtLine, start, (BlockType)mBlockTypes[i]);
                if (block != null)
                    return block;
            }

            return null;
        }

        /// <summary>
        /// Tries to parse a line of text starting from a specified position as a specified block type.
        /// </summary>
        /// <returns>
        /// Parsed block of a specified type, or <c>null</c> if cannot parse.
        /// </returns>
        private Block TryParse(string txtLine, int start, BlockType type)
        {
            Block block = mBlocks[(int)type];
            if (block == null)
                block = MarkdownBlockFactory.Create(type);

            if (block.TryParse(txtLine, start))
            {
                mBlocks[(int)type] = null;
                return block;
            }

            mBlocks[(int)type] = block;
            return null;
        }

        // A collection of blocks which we have tried to recognize, but failed.
        // So we can use it for next parsing line.
        private readonly IntToObjDictionary<Block> mBlocks = new IntToObjDictionary<Block>();

        /// <summary>
        /// A collection of block types that will participate in a parsing process.
        /// </summary>
        /// <remarks>
        /// During the parsing process this collection may be altered when we have determined some line of text
        /// as a particular block type, but it turned out later, that this identification was wrong. So we must exclude
        /// that type from the collection to allow parser to try the line of text as some another block type.
        /// </remarks>
        /// <dev>IN. Forced to use IntList instead of generic List due to Java auto-porting problems.</dev>
        private readonly IntList mBlockTypes = new IntList();

        /// <summary>
        /// All possible block types.
        /// </summary>
        /// <remarks>
        /// The order of block types is IMPORTANT:
        /// - Table should be below Quote and above Lists,
        /// - Setext must be above HorizontalRule,
        /// - HorizontalRule must be above BulletListItem,
        /// - BlankLine, EndOfLine and Paragraph should be at the very end, and they should go in this order.
        /// </remarks>
        private static readonly BlockType[] gAllBlockTypes =
        {
            BlockType.Quote,
            BlockType.Table,
            BlockType.AtxHeading,
            BlockType.SetextHeading,
            BlockType.HorizontalRule,
            BlockType.BulletListItem,
            BlockType.OrderedListItem,
            BlockType.IndentedCode,
            BlockType.FencedCode,
            BlockType.FootnoteDefinition,
            BlockType.BlankLine,
            BlockType.EndOfLine,
            BlockType.Paragraph
        };
    }
}
