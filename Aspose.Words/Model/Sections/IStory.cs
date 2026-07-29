// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/02/2024 by Edward Voronov

using Aspose.Words.Tables;

namespace Aspose.Words
{
    /// <summary>
    /// Represents element that contains block-level nodes.
    /// </summary>
    internal interface IStory
    {
        /// <summary>
        /// Returns the type of the story.
        /// </summary>
        StoryType StoryType { get; }

        /// <summary>
        /// Gets the first paragraph in the story.
        /// </summary>
        Paragraph FirstParagraph { get; }

        /// <summary>
        /// Gets the last paragraph in the story.
        /// </summary>
        Paragraph LastParagraph { get; }

        /// <summary>
        /// Gets a collection of paragraphs that are immediate children of the story.
        /// </summary>
        ParagraphCollection Paragraphs { get; }

        /// <summary>
        /// Gets a collection of tables that are immediate children of the story.
        /// </summary>
        TableCollection Tables { get; }
    }
}
