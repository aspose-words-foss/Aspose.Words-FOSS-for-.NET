// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/06/2011 by Dmitry Matveenko

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Stores data for a TOC entry.
    /// </summary>
    /// <remarks>
    /// Extracted from TocEntryExtractor as it turned out that ParagraphTocEntry constructor uses all fields from here.
    /// </remarks>
    internal class ParagraphTocEntryInfo
    {
        private ParagraphTocEntryInfo()
            : this(1)
        {
            IsEmptyTocEntry = true;
        }

        internal ParagraphTocEntryInfo(int level)
            : this(level, null, null)
        {
        }

        internal ParagraphTocEntryInfo(int level, Node firstNode, Node lastNode)
            : this(level, firstNode, lastNode, false)
        {
        }

        internal ParagraphTocEntryInfo(int level, Node firstNode, Node lastNode, bool isLinkedStyleTocEntry)
        {
            Level = level;
            FirstChild = firstNode;
            LastChild = lastNode;
            IsLinkedStyleTocEntry = isLinkedStyleTocEntry;
        }

        internal int Level { get; }

        internal Node FirstChild { get; }

        internal Node LastChild { get; }

        internal bool IsEmptyTocEntry { get; }

        internal bool IsLinkedStyleTocEntry { get; }

        internal static readonly ParagraphTocEntryInfo EmptyTocEntryInfoInstance = new ParagraphTocEntryInfo();
    }
}
