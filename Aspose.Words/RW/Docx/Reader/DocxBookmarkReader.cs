// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/09/2007 by Vladimir Averkin

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides static method for parsing 'w:bookmarkStart' and 'w:bookmarkEnd' nodes.
    /// These nodes can be met in all elements that are story containers, and also paragraph, 
    /// table and row elements.
    /// </summary>
    internal static class DocxBookmarkReader
    {
        /// <summary>
        /// Reads 'w:bookmarkStart' node.
        /// </summary>
        internal static void ReadStart(NrxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            NrxAnnotation annotation = new NrxAnnotation(xmlReader);
            if (!annotation.HasId)
                return;

            // RK Changed this to set instead of add so bookmarks with duplicate IDs do not cause exceptions.
            reader.IdToBookmarkName[annotation.Id] = annotation.Name;
            BookmarkStart bookmarkStart = new BookmarkStart(reader.Document, annotation.Name);
            bookmarkStart.DisplacedBy = annotation.DisplacedBy;

            if (annotation.ColFirst >= 0 && annotation.ColLast >= 0)
            {
                bookmarkStart.FirstColumn = annotation.ColFirst;
                bookmarkStart.LastColumn = annotation.ColLast;
            }

            reader.AddCrossStructureAnnotation(bookmarkStart);
        }

        /// <summary>
        /// Reads 'w:bookmarkEnd' node.
        /// </summary>
        internal static void ReadEnd(NrxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            NrxAnnotation annotation = new NrxAnnotation(xmlReader);
            if (!annotation.HasId)
                return;

            string name = reader.IdToBookmarkName.GetValueOrNull(annotation.Id);
            // If 'name is null then BookmarkStart node for this BookmarkEnd node is missing.
            if (name != null)
            {
                BookmarkEnd bookmarkEnd = new BookmarkEnd(reader.Document, name);
                bookmarkEnd.DisplacedBy = annotation.DisplacedBy;
                reader.AddCrossStructureAnnotation(bookmarkEnd);

                // WORDSNET-4063 RK There are two bookmarks with the same id in this file. 
                // MS Word reads it okay so should we. We now remove the id-to-name mapping once 
                // bookmark end was found, it will allow the second bookmark to be processed okay.
                reader.IdToBookmarkName.Remove(annotation.Id);
            }
        }
    }
}
