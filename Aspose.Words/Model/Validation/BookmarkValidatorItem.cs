// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/01/2007 by Roman Korchagin

using Aspose.Words.BuildingBlocks;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// A data holder. Keeps info about both bookmark start and bookmark end nodes during document validation.
    /// Not a struct because stored in a hashtable as a value, it requires a by reference object.
    /// </summary>
    internal class BookmarkValidatorItem
    {
        internal BookmarkValidatorItem(BookmarkStart start, StoryType storyType)
        {
            Debug.Assert(start != null);

            BookmarkStart = start;
            BookmarkStoryType = storyType;
        }

        /// <summary>
        /// Returns true, if there is only BookmarkStart in this instance.
        /// </summary>
        internal bool HasStartOnly
        {
            get
            {
                if (BookmarkEnd != null)
                    return false;

                // WORDSNET-16397 Check BookmarkStart is not removed.
                if (BookmarkStart.ParentNode == null)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Obtains bookmark validation key.
        /// </summary>
        internal static string GetKey(IBookmarkNode bookmarkNode)
        {
            Debug.Assert(bookmarkNode != null);

            Node bookmark = (Node)bookmarkNode;
            GlossaryDocument glossary = bookmark.Document as GlossaryDocument;

            if (glossary == null)
                return bookmarkNode.Name;

            CompositeNode buildingBlock = bookmark.GetAncestor(NodeType.BuildingBlock);
            string buildingBlockId = buildingBlock != null
                ? buildingBlock.GetHashCode().ToString()
                : string.Empty;

            const string keyPattern = "{0};{1}";

            return StringUtil.HasChars(buildingBlockId)
                ? string.Format(keyPattern, buildingBlockId, bookmarkNode.Name)
                : bookmarkNode.Name;
        }

        internal BookmarkStart BookmarkStart { get; }
        internal BookmarkEnd BookmarkEnd { get; set; }
        internal StoryType BookmarkStoryType { get; }
    }
}
