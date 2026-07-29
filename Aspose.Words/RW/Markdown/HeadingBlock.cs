// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// The base class for markdown heading blocks.
    /// </summary>
    internal abstract class HeadingBlock : InlineContainerBlock
    {
        /// <summary>
        /// Tries to append a specified block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            // Only inline blocks can be appended into a heading.
            if (block.BlockLevel == MarkdownBlockLevel.Inline)
                return base.TryAppend(block);

            return false;
        }
        
        /// <summary>
        /// Gets or sets a heading level.
        /// </summary>
        internal int Level
        {
            get { return mLevel; }
            set { mLevel = value; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Leaf; }
        }

#if DEBUG
        public override string ToString()
        {
            return string.Format("{0}, Level={1}", base.ToString(), Level);
        }
#endif
        /// <summary>
        /// A heading level.
        /// </summary>
        private int mLevel;
    }
}
