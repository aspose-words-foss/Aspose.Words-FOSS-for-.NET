// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Allows to specify a level of markdown block.
    /// </summary>
    internal enum MarkdownBlockLevel
    {
        /// <summary>
        /// Specifies level 'None' of the markdown block.
        /// </summary>
        /// <remarks>
        /// The blocks with this level typically are not actually stored in a markdown tree.
        /// </remarks>
        None,

        /// <summary>
        /// Specifies level 'Inline' of the markdown block.
        /// </summary>
        /// <remarks>
        /// The blocks with this level are allowed to be child of Leaf-level or other Inline-level blocks.
        /// </remarks>
        Inline,


        /// <summary>
        /// Specifies level 'Leaf' of the markdown block.
        /// </summary>
        /// <remarks>
        /// The Leaf blocks are the blocks that cannot contain Block-level or other Leaf-level blocks.
        /// </remarks>
        Leaf,

        /// <summary>
        /// Specifies level 'Block' of the markdown block.
        /// </summary>
        /// <remarks>
        /// The Block blocks are the blocks that can contain Leaf-level or other Block-level blocks.
        /// </remarks>
        Block
    }
}
