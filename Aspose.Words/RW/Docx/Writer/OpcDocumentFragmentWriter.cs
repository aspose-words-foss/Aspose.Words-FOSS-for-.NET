// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/10/2021 by Dmitry Sokolov

using System;
using System.IO;
using Aspose.Collections.Generic;
using Aspose.Words.Markup;
using Aspose.Words.Revisions;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using Aspose.Words.Validation;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Represents a class to write a document's fragment to <see cref="SaveFormat.FlatOpc"/>.
    /// </summary>
    internal class OpcDocumentFragmentWriter : IDocumentFragmentWriter
    {
        internal OpcDocumentFragmentWriter() : this(false)
        {
        }

        /// <summary>
        /// Special constructor to write minimal document.
        /// </summary>
        /// <remarks>
        /// See WORDSNET-24602 for details.
        /// </remarks>
        internal OpcDocumentFragmentWriter(bool writeMinimalDocument) : this(new OoxmlSaveOptions(SaveFormat.FlatOpc))
        {
            mWriteMinimalDocument = writeMinimalDocument;
        }

        internal OpcDocumentFragmentWriter(OoxmlSaveOptions saveOptions)
        {
            mSaveOptions = saveOptions;

            if (gTestMode)
                mSaveOptions.SetTestMode();
        }

        #region IDocumentFragmentWriter

        /// <summary>
        /// Saves just an individual node (document fragment) into the <see cref="SaveFormat.FlatOpc"/> format.
        /// </summary>
        /// <param name="node">A node to save.</param>
        /// <returns>String that represents node content in the <see cref="SaveFormat.FlatOpc"/> format.</returns>
        /// <remarks>Supported nodes:<br/>
        /// <see cref="NodeType.StructuredDocumentTag"/><br/>
        /// <see cref="NodeType.StructuredDocumentTagRangeStart"/>
        /// </remarks>
        public string SaveToString(Node node)
        {
            Debug.Assert(node != null);

            Document srcDoc = node.Document as Document;
            // Skip nodes from glossary for a while.
            // It is necessary to take in attention 3 documents in some way and prepare the result.
            bool insertContent = srcDoc != null;

            // Attempt use the main document for nodes in glossary causes heavy cloning operation in some
            // cases, for example "TestJira7611.docx". For the "TestJira7611.docx" iterations through all SDT
            // in the glossary may take few hours. And some attempts failed with out of memory exception.
            // So, decided to use just a blank document for a while.
            Document dstDoc = insertContent ? (Document)srcDoc.Document.Clone(false) : new Document();

            if (insertContent)
                ImportNodeContent(node, dstDoc);

            dstDoc.WarningCallback = null;
            dstDoc.BuiltInDocumentProperties.Clear();
            // WORDSNET-23347 At this point all SDTs are updated
            // and we can remove corresponding CustomXmlParts to mimic Word.
            dstDoc.CustomXmlParts.Clear();

            if (mWriteMinimalDocument)
            {
                CleanupOptions options = new CleanupOptions() { UnusedBuiltinStyles = true };
                dstDoc.Cleanup(options);
                dstDoc.DocPr.Rsids.Clear();
                // WORDSNET-28539 Remove the document protection in WriteMinimalDocument mode.
                dstDoc.DocPr.DocumentProtection.Unprotect();
                dstDoc.SetThemeInternal(null);

                RunPrValidator runPrCollector = new RunPrValidator();
                ISetGeneric<string> usedFonts = new HashSetGeneric<string>();
                runPrCollector.Execute(dstDoc, usedFonts);
                dstDoc.FontInfos.UpdateToUsedFonts(usedFonts);

                dstDoc.Variables.Clear();
                dstDoc.JoinRunsWithSameFormatting();
                RemoveRsids(dstDoc);

                mSaveOptions.SetBuiltInThemeIfNull = false;
                mSaveOptions.WriteLatentStyles = false;
            }

            // WORDSNET-27447 Remove macros when save WordOpenXML content.
            dstDoc.RemoveMacros();

            return DocumentToString(dstDoc);
        }

        #endregion IDocumentFragmentWriter

        /// <summary>
        /// Sets Test flag. This flag is used to make behavior of the class deterministic for test purposes.
        /// Call this method before running any tests which can be connected to methods of this class.
        /// </summary>
        public static void SetTestMode()
        {
            gTestMode = true;
        }

        /// <summary>
        /// Imports node content depending from the node type.
        /// </summary>
        private void ImportNodeContent(Node node, Document dstDoc)
        {
            Document srcDoc = node.Document as Document;
            NodeImporter importer = new NodeImporter(srcDoc, dstDoc, ImportFormatMode.UseDestinationStyles);

            switch (node.NodeType)
            {
                case NodeType.StructuredDocumentTag:
                    ImportSdt((StructuredDocumentTag)node, importer);
                    break;
                case NodeType.StructuredDocumentTagRangeStart:
                    ImportSdtRange((StructuredDocumentTagRangeStart)node, importer);
                    break;
                default:
                    // Skip other nodes for a while and return just empty document.
                    break;
            }

            // WORDSNET-23887 The Word inserts start of bookmarks to the result when bookmarks in the destination are inconsistent.
            FixMissingBookmarksStarts(node, dstDoc, importer);
        }

        private static void FixMissingBookmarksStarts(Node node, Document dstDoc, NodeImporter importer)
        {
            // Skip for a while cases for row and cell level SDTs. For example, currently logic does not insert bookmarks
            // from cell level. See Test23887CellLevel for details.
            if (!node.IsComposite || ((node.NodeLevel != NodeLevel.Block) && (node.NodeLevel != NodeLevel.Inline)))
                return;

            BookmarkCache dstBookmarkCache = null;
            BookmarkCache srcBookmarkCache = null;

            try
            {
                dstBookmarkCache = new BookmarkCache(dstDoc);
                srcBookmarkCache = new BookmarkCache((Document)node.Document);
                // Looks like the Word flushes "starts" to the begging of the first paragraph.
                Paragraph dstBookmarkParent = (Paragraph)dstDoc.GetChild(NodeType.Paragraph, 0, true);
                if (dstBookmarkParent == null)
                    return;

                foreach (BookmarkEnd end in ((CompositeNode)node).GetChildNodes(NodeType.BookmarkEnd, true))
                {
                    Bookmark srcBookmark = srcBookmarkCache[end.Name];
                    if (srcBookmark == null) // i.e. "start" not found in the source.
                        continue;

                    Bookmark dstBookmark = dstBookmarkCache[srcBookmark.Name];
                    if (dstBookmark != null) // i.e. current bookmark has "start" and "end".
                        continue;

                    // "end" should be imported from the source and there is not a "start" in the destination.
                    // At the same time the source contains "start". So, import and append "start" to the
                    // destination as the Word does.

                    BookmarkStart dstStart = (BookmarkStart)importer.ImportNode(srcBookmark.BookmarkStart, false);
                    dstBookmarkParent.PrependChild(dstStart);

                    // I.e. start + end here must exist.
                    dstBookmark = dstBookmarkCache[dstStart.Name];
                    Debug.Assert(dstBookmark != null);

                    // Looks like the Word resets this bookmark property.
                    dstBookmark.BookmarkEnd.DisplacedBy = DisplacedByType.Unspecified;
                    dstBookmark.BookmarkStart.DisplacedBy = DisplacedByType.Unspecified;
                }
            }
            finally
            {
                // Release resources and unsubscribe from events.
                if (dstBookmarkCache != null)
                    dstBookmarkCache.Clear();

                if (srcBookmarkCache != null)
                    srcBookmarkCache.Clear();
            }
        }

        /// <summary>
        /// Imports SDT node.
        /// </summary>
        private void ImportSdt(StructuredDocumentTag sdt, NodeImporter importer)
        {
            if (!sdt.HasChildNodes)
                return;

            Document dstDoc = importer.Context.DstDoc as Document;
            Debug.Assert(dstDoc != null);

            StructuredDocumentTag dstSdt = (StructuredDocumentTag)importer.ImportNode(sdt, true);
            Section srcSect = (Section)sdt.GetAncestor(NodeType.Section);
            if (srcSect != null)
            {
                Section dstSect = (Section)importer.ImportNode(srcSect, false);
                dstDoc.Sections.Add(dstSect);
            }

            DocumentBuilder builder = new DocumentBuilder(dstDoc);
            builder.MoveToDocumentEnd();

            switch (dstSdt.Level)
            {
                case MarkupLevel.Block:
                    InsertBlockLevelSdt(builder, dstSdt);
                    break;
                case MarkupLevel.Inline:
                    InsertInlineLevelSdt(builder, dstSdt);
                    break;
                case MarkupLevel.Cell:
                    InsertCellLevelSdt(builder, importer, sdt, dstSdt);
                    break;
                case MarkupLevel.Row:
                    InsertRowLevelSdt(builder, importer, sdt, dstSdt);
                    break;
                default:
                    throw new InvalidOperationException("Unexpected SDT level.");
            }
        }

        /// <summary>
        /// Imports SDT range.
        /// </summary>
        private void ImportSdtRange(StructuredDocumentTagRangeStart rangeStart, NodeImporter importer)
        {
            Debug.Assert(rangeStart != null);

            if (rangeStart.ParentNode == null)
                throw new InvalidOperationException("Structured document tag range start is not descendant of a document.");

            StructuredDocumentTagRangeEnd rangeEnd = rangeStart.RangeEnd;
            if (rangeEnd == null)
                throw new InvalidOperationException("Structured document tag range end was not found in the document.");

            Document dstDoc = importer.Context.DstDoc as Document;
            Debug.Assert(dstDoc != null);

            Section endSection = (rangeEnd.ParentNode != null) ? ((Body)rangeEnd.ParentNode).ParentSection : null;
            Section startSection = (rangeStart.ParentNode != null) ? ((Body)rangeStart.ParentNode).ParentSection : null;

            for (Section curSect = startSection; curSect != endSection.NextSibling && curSect != null; curSect = (Section)curSect.NextSibling)
            {
                if (ReferenceEquals(curSect, startSection))
                {
                    dstDoc.EnsureMinimum();
                    dstDoc.FirstSection.Body.Remove();
                }

                CopySection(curSect, importer, rangeStart);
            }
        }

        private static void CopySection(
            Section srcSect,
            NodeImporter importer,
            StructuredDocumentTagRangeStart rangeStart)
        {
            Section importedSect;
            Document dstDoc = (Document)importer.Context.DstDoc;

            bool isFirstSection = ReferenceEquals(((Body)rangeStart.ParentNode).ParentSection, srcSect);
            bool isLastSection = ReferenceEquals(((Body)rangeStart.RangeEnd.ParentNode).ParentSection, srcSect);

            if (isLastSection)
            {
                importedSect = dstDoc.LastSection;
            }
            else
            {
                importedSect = (Section)importer.ImportNode(srcSect, !isFirstSection);
                dstDoc.InsertBefore(importedSect, dstDoc.LastSection);
            }

            if (!isLastSection && !isFirstSection)
                return;

            // Copy headers/footers.
            foreach(HeaderFooter headerFooter in srcSect.GetChildNodes(NodeType.HeaderFooter, false))
            {
                HeaderFooter importedHeaderFooter = (HeaderFooter)importer.ImportNode(headerFooter, true);
                importedSect.AppendChild(importedHeaderFooter);
            }

            // Copy body content.
            Node importedNode = importer.ImportNode(srcSect.Body, false);
            importedSect.AppendChild(importedNode);

            CopyBodyChildren(
                isFirstSection ? rangeStart.NextSibling : srcSect.Body.FirstChild,
                rangeStart.RangeEnd,
                importer,
                importedSect);
        }

        private static void CopyBodyChildren(Node from, Node to, NodeImporter importer, Section dstSect)
        {
            for (Node nodeToImport = from; nodeToImport != null && nodeToImport != to; nodeToImport = nodeToImport.NextSibling)
            {
                Node importedNode = importer.ImportNode(nodeToImport, true);
                dstSect.Body.AppendChild(importedNode);
            }
        }

        private string DocumentToString(Document doc)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamReader sr = new StreamReader(ms))
                {
                    doc.Save(ms, mSaveOptions);

                    ms.Position = 0;
                    return sr.ReadToEnd();
                }
            }
        }

        private void InsertInlineLevelSdt(DocumentBuilder builder, StructuredDocumentTag dstSdt)
        {
            builder.InsertNode(dstSdt);

            bool isUpdateXmlMapping =
                mSaveOptions.UpdateXmlMapping && dstSdt.XmlMapping.ChecksumNotEqual(new XmlMappingContext());

            dstSdt.RemoveSelfOnly(isUpdateXmlMapping);
        }

        private void InsertBlockLevelSdt(DocumentBuilder builder, StructuredDocumentTag dstSdt)
        {
            builder.CurrentStory.AppendChild(dstSdt);
            builder.CurrentParagraph.Remove();

            bool isUpdateXmlMapping =
                mSaveOptions.UpdateXmlMapping && dstSdt.XmlMapping.ChecksumNotEqual(new XmlMappingContext());

            dstSdt.RemoveSelfOnly(isUpdateXmlMapping);
        }

        /// <summary>
        /// Inserts cell level SDT content to the destination.
        /// </summary>
        private static void InsertCellLevelSdt(
            DocumentBuilder builder,
            NodeImporter importer,
            StructuredDocumentTag sourceSdt,
            StructuredDocumentTag dstSdt)
        {
            // Skip SDT updating for this level. Looks like cell level SDT can not be mapped to the XML.
            // Skip updating of nested SDT's for a while.

            // Word moves cell content to the story when SDT contains only one cell.
            Cell firstCell = (Cell)dstSdt.GetChild(NodeType.Cell, 0, true);
            if (ReferenceEquals(firstCell, firstCell.ParentNode.LastChild))
            {
                // Actually between cell and imported SDT may be other SDT's. And we should to import
                // these SDT's, change SDT's levels and remove cell. Skip this logic for a while.
                // WORDSNET-23220 Iterate through nodes in reverse order.
                for (Node cellChild = firstCell.LastChild; cellChild != null; cellChild = firstCell.LastChild)
                    builder.CurrentStory.PrependChild(cellChild);

                builder.CurrentParagraph.Remove();
            }
            else
            {
                // Import parent row and table.
                Row srcRow = (Row)sourceSdt.ParentNode;
                Row dstRow = (Row)importer.ImportNode(sourceSdt.ParentNode, false);

                Table dstTbl = (Table)importer.ImportNode(srcRow.ParentTable, false);
                dstTbl.AppendChild(dstRow);

                // Import SDT cells and append these cells to the destination table.
                for (Node childCell = dstSdt.LastChild; childCell != null; childCell = dstSdt.LastChild)
                    dstRow.PrependChild(childCell);

                builder.CurrentStory.PrependChild(dstTbl);
            }
        }

        private static void InsertRowLevelSdt(
            DocumentBuilder builder,
            NodeImporter importer,
            StructuredDocumentTag sourceSdt,
            StructuredDocumentTag dstSdt)
        {
            // Skip SDT updating for this level. Looks like row level SDT can not be mapped to the XML.
            Table srcTbl = (Table)sourceSdt.GetAncestor(NodeType.Table);
            Table dstTbl = (Table)importer.ImportNode(srcTbl, false);

            for (Node childRow = dstSdt.LastChild; childRow != null; childRow = dstSdt.LastChild)
                dstTbl.PrependChild(childRow);

            // Already there is paragraph in the end of the document. Insert content before the paragraph.
            builder.CurrentStory.PrependChild(dstTbl);
        }

        /// <summary>
        /// Removes all rsid related attributes.
        /// </summary>
        private static void RemoveRsids(Document doc)
        {
            foreach (Section section in doc.Sections)
            {
                foreach (Paragraph para in section.Body.Paragraphs)
                {
                    foreach (Run run in para.Runs)
                    {
                        run.RunPr.Remove(FontAttr.RsidR);
                        run.RunPr.Remove(FontAttr.RsidRPr);
                    }

                    para.ParaPr.Remove(ParaAttr.RsidP);

                    para.ParagraphBreakRunPr.Remove(FontAttr.RsidR);
                    para.ParagraphBreakRunPr.Remove(FontAttr.RsidRPr);
                }

                section.SectPr.Remove(SectAttr.Rsid);
            }
        }

        private readonly OoxmlSaveOptions mSaveOptions;

        /// <summary>
        /// Controls whether we write stripped-down document that excludes any non-content-related parts.
        /// </summary>
        private readonly bool mWriteMinimalDocument;

        private static bool gTestMode;
    }
}
