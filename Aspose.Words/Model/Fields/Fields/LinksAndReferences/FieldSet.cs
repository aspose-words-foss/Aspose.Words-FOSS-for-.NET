// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/08/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the SET field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Assigns new text to a bookmark.
    /// </remarks>
    public class FieldSet : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            return new FieldUpdateActionSetBookmarkValue(this, BookmarkName, BookmarkTextArgument);
        }

        /// <summary>
        /// Gets or sets the name of the bookmark.
        /// </summary>
        public string BookmarkName
        {
            get { return FieldCodeCache.GetArgumentAsString(BookmarkNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(BookmarkNameArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the new text of the bookmark.
        /// </summary>
        public string BookmarkText
        {
            get { return FieldCodeCache.GetArgumentAsString(BookmarkTextArgumentIndex); }
            set { FieldCodeCache.SetArgument(BookmarkTextArgumentIndex, value); }
        }

        private FieldArgument BookmarkTextArgument
        {
            get { return FieldCodeCache.GetArgument(BookmarkTextArgumentIndex); }
        }

        private const int BookmarkNameArgumentIndex = 0;
        private const int BookmarkTextArgumentIndex = 1;
    }
}
