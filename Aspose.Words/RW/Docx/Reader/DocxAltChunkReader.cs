// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2012 by Andrey Noskov

using System.Collections.Generic;
using System.IO;
using Aspose.OpcPackaging;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Lists;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Factories;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides static method for reading altChunk element.
    /// </summary>
    internal class DocxAltChunkReader
    {
        /// <summary>
        /// Reads an altChunk element (17.17.2.1).
        /// </summary>
        internal static void Read(NrxDocumentReaderBase reader)
        {
            Debug.Assert(reader.IsDocx);

            string altChunkId = "";
            ImportFormatMode importFormatMode = ImportFormatMode.UseDestinationStyles;

            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "id":
                        altChunkId = xmlReader.Value;
                        break;
                    default:
                        xmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Docx, xmlReader.LocalName);
                        break;
                }
            }

            while (xmlReader.ReadChild("altChunk"))
            {
                switch (xmlReader.LocalName)
                {
                    case "altChunk":
                        // WORDSNET-11573 Nested altChunk element.
                        Read(reader);
                        return;
                    case "altChunkPr":
                        importFormatMode = ReadAltChunkPr(xmlReader, importFormatMode);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            AddAltChunk((DocxDocumentReaderBase)reader, altChunkId, importFormatMode);
        }

        /// <summary>
        /// Read altChunkPr element (17.17.2.2)
        /// Within ISO/IEC 29500, only one property (matchSrc) is specified for altChunkPr element.
        /// </summary>
        private static ImportFormatMode ReadAltChunkPr(NrxXmlReader xmlReader, ImportFormatMode importFormatMode)
        {
            while (xmlReader.ReadChild("altChunkPr"))
            {
                switch (xmlReader.LocalName)
                {
                    // Read matchSrc - Keep Source Formatting on Import (17.17.2.3)
                    case "matchSrc":
                        importFormatMode = xmlReader.ReadBoolVal()
                                               ? ImportFormatMode.KeepSourceFormatting
                                               : ImportFormatMode.UseDestinationStyles;
                        break;
                    default:
                        xmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Docx, xmlReader.LocalName);
                        break;
                }
            }
            return importFormatMode;
        }

        /// <summary>
        /// Create and add AltChunk object to AltChunk collection.
        /// </summary>
        private static void AddAltChunk(DocxDocumentReaderBase reader, string altChunkId, ImportFormatMode importFormatMode)
        {
            string target = reader.GetRelationshipTarget(altChunkId);

            OpcPackagePart imagePart = reader.GetPartByName(target);
            if (imagePart == null)
                return;

            byte[] altChunkData = imagePart.GetAsMemoryStream().ToArray();

            // Detect load format of altChunk content. If load format unknown just ignore.
            FileFormatDetector detector = new FileFormatDetector();
            FileFormatInfo info = detector.Detect(new MemoryStream(altChunkData));
            if (info.LoadFormat == LoadFormat.Unknown)
            {
                return;
            }

            // Reset loading callback which used for the main document.
            // Don't want to affect main document progress while "AltChunk" loading.
            LoadOptions lo = reader.LoadOptions.Clone();
            lo.ProgressCallback = null;

            // Get AltChunk document.
            Document altChunkDocument = LoadAltChunkDocument(altChunkData, info.LoadFormat, reader, lo, importFormatMode);

            if (altChunkDocument.OriginalLoadFormat == LoadFormat.Rtf)
            {
                // WORDSNET-28163 Handle RTF altchunk tables.
                // AM. Don't really know if it is a good place for this code, maybe we need to add internal load options
                // to handle this case directly in RTF reader.
                MsWordVersionCore altchunkVersion = altChunkDocument.CompatibilityOptions.MswVersion;
                MsWordVersionCore targetVersion = reader.Document.FetchDocument().CompatibilityOptions.MswVersion;

                if ((altchunkVersion == MsWordVersionCore.Unspecified) && (targetVersion >= MsWordVersionCore.Word2013))
                    HandleRtfTables(altChunkDocument);
            }

            NodeType containerNodeType = NodeType.Paragraph;

            Node referenceNode = reader.CurContainer.LastChild;

            // WORDSNET-13130 Page break position is changed after re-saving Docx.
            // Flash a run with a page break into the first paragraph of altChunk document. If the first node in body
            // is not a paragraph, create a new paragraph with a page break and insert it into main document.
            FlushRun(altChunkDocument, reader);

            // If CurContainer is Paragraph, altChunk is inserted on inline level and must be processed in special way.
            bool isInline = (reader.CurContainer.NodeType == NodeType.Paragraph);

            // WORDSNET-11945 If CurContainer is StructuredDocumentTag placed on an inline level,
            // i.e. inside a paragraph. This means that it can host only inline content inside.
            if (reader.CurContainer.NodeType == NodeType.StructuredDocumentTag)
            {
                containerNodeType = NodeType.StructuredDocumentTag;

                StructuredDocumentTag structuredDocumentTag = (StructuredDocumentTag)reader.CurContainer;
                isInline = (structuredDocumentTag.ParentNode.NodeType == NodeType.Paragraph);
            }

            CompositeNode referenceContainer;

            CompositeNode curContainer = reader.CurContainer;

            // If it is inline altChunk, we should close current paragraph/SDT and reopen it after altChunk referenceContainer.
            if (isInline)
                reader.PopContainer(containerNodeType);

            // Create, add and push paragraph or SDT container to insert an altChunk content before it.
            switch (containerNodeType)
            {
                case NodeType.StructuredDocumentTag:
                    referenceContainer = new StructuredDocumentTag(reader.Document, MarkupLevel.Inline);
                    break;
                default:
                    referenceContainer = new Paragraph(reader.Document);
                    break;
            }

            reader.AddAndPushContainer(referenceContainer);
            // WORDSNET-16659 Flush SDT to the "altchunk" reference node to preserve SDT content.
            // Note: Reference node will be removed after markup will be resolved.
            if (referenceContainer.NodeType == NodeType.Paragraph)
            {
                reader.FlushPendingMarkup(referenceContainer);
                // WORDSNET-17050 Update node, which represents end of the SDT markup.
                reader.XmlReader.PossibleSdtEnd = (Paragraph)referenceContainer;
            }

            // Reopen container if altChunk is on inline level.
            if (isInline)
            {
                reader.PopContainer(containerNodeType);
                reader.AddAndPushContainer((CompositeNode)curContainer.Clone(false));
            }

            // Pop altChunk referenceParagraph if altChunk is on block level.
            if (!isInline)
                reader.PopContainer(containerNodeType);

            DocxAltChunk altChunk = new DocxAltChunk(referenceContainer, referenceNode, altChunkDocument, importFormatMode, isInline);

            // Add AltChunk object to AltChunk collection.
            reader.AddAltChunk(altChunk);
        }

        /// <summary>
        /// Loads an altChunk document.
        /// </summary>
        /// <param name="altChunkData">A byte array to load the altChunk document from.</param>
        /// <param name="altChunkFormat">Detected <see cref="LoadFormat"/> of the altChunk document.</param>
        /// <param name="docxReader">"Docx" document reader.</param>
        /// <param name="targetLoadOptions">The LoadOptions of the target Document.</param>
        /// <param name="importFormatMode">The mode in which the altChunk will be inserted into the document.</param>
        /// <returns>A document loaded from the altChunk data. </returns>
        private static Document LoadAltChunkDocument(
            byte[] altChunkData,
            LoadFormat altChunkFormat,
            DocxDocumentReaderBase docxReader,
            LoadOptions targetLoadOptions,
            ImportFormatMode importFormatMode)
        {
            MemoryStream documentStream = new MemoryStream(altChunkData);
            DocumentBase targetDocument = docxReader.Document;

            // Import of HTML-based altChunks require special processing in order to make sure the result looks close
            // to what is produced by MS Word.
            if ((altChunkFormat == LoadFormat.Html) || (altChunkFormat == LoadFormat.Mhtml))
            {
                Document altChunkDocument = new Document(DocumentCtorMode.EmptyDocumentNode);

                // WORDSNET-23604 Update initial value of the list definition identifier to preserve numbering from the altChunk.
                List<DocxAltChunk> altChunks = docxReader.AltChunks;
                if (altChunks.Count > 0)
                {
                    // WORDSNET-23844 Try to find a previous non-empty list definitions collection from altChunks.
                    List<ListDef> listDefs = null;
                    for (int i = altChunks.Count - 1; i >= 0; --i)
                    {
                        listDefs = altChunks[i].AltChunkDocument.Lists.ListDefs;
                        if (listDefs.Count > 0)
                            break;
                    }

                    listDefs.Sort();
                    int maxListDefId = listDefs.Count > 0 ? listDefs[listDefs.Count - 1].ListDefId : 0;
                    altChunkDocument.Lists.SetMaxListDefId(maxListDefId);
                }

                // WORDSNET-23496 Copy all HTML blocks to a new altChunk document to
                // take into account IDs of already created HTML blocks.
                CopyHtmlBlocks(targetDocument, altChunkDocument);
                foreach (DocxAltChunk altChunk in altChunks)
                {
                    CopyHtmlBlocks(altChunk.AltChunkDocument, altChunkDocument);
                }

                if (importFormatMode == ImportFormatMode.UseDestinationStyles)
                {
                    // Make sure the altChunk document has the same base formatting as the target document.
                    // This step is important for correct splitting of imported formatting into style and direct parts.
                    targetDocument.Styles.DefaultParaPr.CopyTo(altChunkDocument.Styles.DefaultParaPr);
                    targetDocument.Styles.DefaultRunPr.CopyTo(altChunkDocument.Styles.DefaultRunPr);
                }

                // The HTML reader should stick to MS Word behavior when importing HTML altChunks.
                HtmlLoadOptions htmlLoadOptions = new HtmlLoadOptions(targetLoadOptions);
                htmlLoadOptions.ApplyFormattingAsMsWord = true;

                // WORDSNET-20364 Copy the MS Word version to the document imported from HTML or use the MS Word version
                // from load options if the original document's MS Word version is unspecified.
                Document mainDocument = (targetDocument.NodeType == NodeType.Document)
                    ? (Document)targetDocument
                    : ((GlossaryDocument)targetDocument).MainDocument;
                if (mainDocument.CompatibilityOptions.MswVersion != MsWordVersionCore.Unspecified)
                {
                    altChunkDocument.CompatibilityOptions.MswVersion = mainDocument.CompatibilityOptions.MswVersion;
                }
                else
                {
                    altChunkDocument.CompatibilityOptions.OptimizeFor(targetLoadOptions.MswVersion);
                }

                altChunkDocument.Load(documentStream, htmlLoadOptions);
                return altChunkDocument;
            }

            LoadOptions loadOptions = new LoadOptions(targetLoadOptions);

            // All other formats don't require any special processing.
            return new Document(documentStream, loadOptions, false);
        }

        private static void CopyHtmlBlocks(
            DocumentBase source,
            DocumentBase destination)
        {
            destination.HtmlBlockCollection.Add(source.HtmlBlockCollection);
        }

        /// <summary>
        /// Updates document tables to mimic MS Word behaviour.
        /// </summary>
        private static void HandleRtfTables(Document doc)
        {
            foreach (Section section in doc.Sections)
            foreach (IStory story in section)
            foreach (Table table in story.Tables)
            {
                table.SetAttrOnAllRows(TableAttr.StyleOptions, TableStyleOptions.Default2003);
                table.SetAttrOnAllRows(TableAttr.LeftIndent, -5);
            }
        }

        /// <summary>
        /// Flush a run with text joined in RunTextBuilder.
        /// </summary>
        /// <param name="altChunkDoc">AltChunk document</param>
        /// <param name="reader">Reader</param>
        private static void FlushRun(Document altChunkDoc, NrxDocumentReaderBase reader)
        {
            if (reader.XmlReader.RunTextBuilder.Length == 0)
                return;

            Node fChild = altChunkDoc.FirstSection.Body.FirstNonAnnotationChild;
            Paragraph altChunkPar = (fChild.NodeType == NodeType.Paragraph) ? (Paragraph)fChild : null;

            if (null != altChunkPar)
            {
                // Flush a run with a page break into the first paragraph of altChunk document.
                string runText = reader.XmlReader.RunTextBuilder.ToString();
                Run run = new Run(altChunkDoc, runText, new RunPr());
                altChunkPar.PrependChild(run);

                // Reset run text builder.
                reader.XmlReader.RunTextBuilder.Length = 0;
            }
            else
            {
                // Create a new paragraph with a page break and insert it into main document.
                ((DocxDocumentReaderBase)reader).FlushRun(reader.Document);
            }
        }
    }
}
