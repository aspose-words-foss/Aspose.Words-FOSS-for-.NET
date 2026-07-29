// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2019 by Ilya Navrotskiy

using System.Collections;
using System.Collections.Generic;
using System.Text;
using Aspose.Words.RW.Markdown.Reader;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// The base class for markdown container blocks.
    /// </summary>
    internal abstract class ContainerBlock : Block, IEnumerable<Block>
    {
        /// <summary>
        /// Writes the block into a model using specified context.
        /// </summary>
        internal override void Write(MarkdownReaderContext context)
        {
            context.Open(this);

            foreach (Block childBlock in mChildBlocks)
                childBlock.Write(context);

            context.Close(this);
        }

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        /// <remarks>
        /// We try to append a specified block to a last opened child block of the container.
        /// If it cannot be appended, then we close that last child block and iterate to
        /// a next last opened child.
        /// </remarks>
        internal override bool TryAppend(Block block)
        {
            Block lastOpenedChildBlock = GetDeepestOpenedChildBlock();
            while (lastOpenedChildBlock != null)
            {
                bool isAppended = lastOpenedChildBlock.TryAppend(block);

                if (lastOpenedChildBlock.Action == BlockAction.Remove)
                    lastOpenedChildBlock.Remove();

                if (block.Action == BlockAction.Exclude)
                    return isAppended;

                if (isAppended)
                    return true;

                lastOpenedChildBlock.Close();

                // EOLs in our markdown tree are used as helper blocks that allow to append other blocks.
                // So, no need to keep closed EOLs in the markdown tree.
                if (lastOpenedChildBlock.Type == BlockType.EndOfLine)
                    lastOpenedChildBlock.Remove();

                lastOpenedChildBlock = GetDeepestOpenedChildBlock();
            }

            return false;
        }

        /// <summary>
        /// Adds child block.
        /// </summary>
        internal Block Add(Block block)
        {
            mChildBlocks.Add(block);
            block.Parent = this;

            return block;
        }

        /// <summary>
        /// Removes child block.
        /// </summary>
        internal void Remove(Block block)
        {
            mChildBlocks.Remove(block);
            block.Parent = null;
        }

        /// <summary>
        /// Closes all child blocks.
        /// </summary>
        internal void CloseChildren()
        {
            Block lastOpenedChildBlock = GetDeepestOpenedChildBlock();
            while (lastOpenedChildBlock != null)
            {
                lastOpenedChildBlock.Close();
                lastOpenedChildBlock = GetDeepestOpenedChildBlock();
            }
        }

        /// <summary>
        /// Closes all child blocks non-including the specified types.
        /// </summary>
        internal void CloseChildrenNonInclusive(params BlockType[] typesToExclude)
        {
            foreach (Block childBlock in mChildBlocks)
            {
                ContainerBlock containerBlock = childBlock as ContainerBlock;
                if (containerBlock != null)
                    containerBlock.CloseChildrenNonInclusive(typesToExclude);

                if (!childBlock.HasType(typesToExclude))
                    childBlock.Close();
            }
        }

        /// <summary>
        /// Returns deepest opened child block of a specified type.
        /// </summary>
        /// <remarks>
        /// If a specified type is <see cref="BlockType.Document"/>, then searches an opened block of any type.
        /// </remarks>
        internal Block GetDeepestOpenedChildBlock(BlockType type = BlockType.Document)
        {
            for (int i = (mChildBlocks.Count - 1); i >= 0; i--)
            {
                Block childBlock = mChildBlocks[i];

                ContainerBlock containerBlock = childBlock as ContainerBlock;
                if (containerBlock != null)
                {
                    // The child block is a container, so first try to search inside it.
                    Block lastOpenedInContainer = containerBlock.GetDeepestOpenedChildBlock(type);
                    if (lastOpenedInContainer != null)
                        return lastOpenedInContainer;
                }

                if (((childBlock.Type == type) || (type == BlockType.Document)) && childBlock.IsOpened)
                    return childBlock;
            }

            return null;
        }

        /// <summary>
        /// Returns deepest opened child block of a specified type.
        /// </summary>
        internal Block GetDeepestOpenedChildBlock(params BlockType [] types)
        {
            for (int i = (mChildBlocks.Count - 1); i >= 0; i--)
            {
                Block childBlock = mChildBlocks[i];

                ContainerBlock containerBlock = childBlock as ContainerBlock;
                if (containerBlock != null)
                {
                    // The child block is a container, so first try to search inside it.
                    Block lastOpenedInContainer = containerBlock.GetDeepestOpenedChildBlock(types);
                    if (lastOpenedInContainer != null)
                        return lastOpenedInContainer;
                }

                if (childBlock.HasType(types) && childBlock.IsOpened)
                    return childBlock;
            }

            return null;
        }

        #region IEnumerable implementation
        public IEnumerator<Block> GetEnumerator()
        {
            return mChildBlocks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

#if DEBUG
        public override string ToString()
        {
            StringBuilder childBlocks = new StringBuilder();
            for (int i = 0; i < Count; i++)
            {
                Block block = mChildBlocks[i];
                string isOpened = (block.IsOpened) ? "opened" : "closed";
                childBlocks.Append(string.Format("({0}:{1})", block.Type, isOpened));
                if (i < (Count - 1))
                    childBlocks.AppendLine(", ");
            }

            return string.Format("{0}{{ChildBlocks:{1}}}", base.ToString(), childBlocks);
        }
#endif

        /// <summary>
        /// Gets child block at a specified position.
        /// </summary>
        internal Block this[int index]
        {
            get { return mChildBlocks[index]; }
        }

        /// <summary>
        /// Gets last child block.
        /// </summary>
        internal Block Last
        {
            get { return (mChildBlocks.Count > 0) ? mChildBlocks[mChildBlocks.Count - 1] : null; }
        }

        /// <summary>
        /// Gets a boolean value indicating whether the container is empty.
        /// </summary>
        internal bool IsEmpty
        {
            get { return (mChildBlocks.Count == 0); }
        }

        /// <summary>
        /// Gets block text.
        /// </summary>
        internal override string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (Block childBlock in mChildBlocks)
                    sb.Append(childBlock.Text);

                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets count of child blocks.
        /// </summary>
        internal int Count
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mChildBlocks.Count; }
        }

        /// <summary>
        /// A collection of child blocks.
        /// </summary>
        private readonly List<Block> mChildBlocks = new List<Block>();
    }
}
