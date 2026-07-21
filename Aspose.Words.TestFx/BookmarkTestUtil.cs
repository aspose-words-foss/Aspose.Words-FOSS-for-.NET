// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/12/2018 by Alexey Butalov

namespace Aspose.Words.Tests
{
    public static class BookmarkTestUtil
    {
        /// <summary>
        /// Creates document with a specified number of bookmarks.
        /// </summary>
        public static Document CreateDocumentWithBookmarks(int bookmarksCount)
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());

            for (int i = 0; i < bookmarksCount; i++)
            {
                string bookmarkName = string.Format("bookmark{0}", i);
                InsertBookmark(builder, bookmarkName);
            }

            return builder.Document;
        }

        /// <summary>
        /// Appends new bookmark to the document.
        /// </summary>
        public static void AppendBookmark(Document doc, string name)
        {
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();

            InsertBookmark(builder, name);
        }

        /// <summary>
        /// Inserts new bookmark into the document.
        /// </summary>
        private static void InsertBookmark(DocumentBuilder builder, string name)
        {
            builder.StartBookmark(name);
            builder.Write(name);
            builder.EndBookmark(name);
            builder.Writeln();
        }
    }
}
