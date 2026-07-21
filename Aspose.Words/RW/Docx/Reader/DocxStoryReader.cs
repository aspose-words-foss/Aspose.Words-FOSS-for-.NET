// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/08/2007 by Vladimir Averkin

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides method for reading story-level elements, 
    /// i.e any element that can contain paragraphs or tables.
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal class DocxStoryReader : NrxStoryReaderBase
    {
        internal DocxStoryReader(DocxParaReader paraReader)
        {
            Debug.Assert(paraReader != null);
            mParaReader = paraReader;
        }

        /// <summary>
        /// Reads all children of the current element into the specified node. 
        /// </summary>
        /// <param name="reader">DOCX parser used to read the document.</param>
        /// <param name="story">Node where to read the story to.</param>
        internal override void Read(NrxDocumentReaderBase reader, CompositeNode story)
        {
            reader.ReadStory(story);
        }

        /// <summary>
        /// Reads all children of the current element into the current container.
        /// The current element is supposed to be a story element such as comment, footnote, header/footer or a shape.
        /// The children are supposed to be block-level elements.
        /// </summary>
        internal override void ReadChildren(NrxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            xmlReader.MoveToElement();
            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
                ReadChild(reader);
        }

        /// <summary>
        /// Reads one story child element (paragraph, table, bookmark start or end) and appends 
        /// to the current container in the model builder.
        /// </summary>
        internal override void ReadChild(NrxDocumentReaderBase reader)
        {
            bool isSectionBreak = false;

            NrxXmlReader xmlReader = reader.XmlReader;
            switch (xmlReader.LocalName)
            {
                case "p":
                    isSectionBreak = mParaReader.Read(reader);
                    break;
                case "tbl":
                    DocxTableReader.Read(reader);
                    break;
                case "bookmarkStart":
                    DocxBookmarkReader.ReadStart(reader);
                    break;
                case "bookmarkEnd":
                    DocxBookmarkReader.ReadEnd(reader);
                    break;
                case "commentRangeStart":
                    DocxCommentsReader.ReadCommentRangeStart(reader);
                    break;
                case "commentRangeEnd":
                    DocxCommentsReader.ReadCommentRangeEnd(reader);
                    break;
                case "permStart":
                    NrxEditableRangesReader.ReadEditableRangeStart(reader);
                    break;
                case "permEnd":
                    NrxEditableRangesReader.ReadEditableRangeEnd(reader);
                    break;
                case "moveFromRangeStart":
                    DocxAnnotationReader.ReadMoveRangeStart(reader, true);
                    break;
                case "moveFromRangeEnd":
                    DocxAnnotationReader.ReadMoveRangeEnd(reader, true);
                    break;
                case "moveToRangeStart":
                    DocxAnnotationReader.ReadMoveRangeStart(reader, false);
                    break;
                case "moveToRangeEnd":
                    DocxAnnotationReader.ReadMoveRangeEnd(reader, false);
                    break;
                case "customXmlDelRangeStart":
                case "customXmlInsRangeStart":
                    DocxAnnotationReader.ReadCustomXmlEditRevision(reader);
                    break;
                case "customXmlMoveFromRangeStart":
                case "customXmlMoveToRangeStart":
                    DocxAnnotationReader.ReadCustomXmlMoveRevision(reader);
                    break;
                case "customXmlDelRangeEnd":
                case "customXmlInsRangeEnd":
                case "customXmlMoveFromRangeEnd":
                case "customXmlMoveToRangeEnd":
                    DocxAnnotationReader.ClearCustomXmlRevision(reader);
                    break;
                case "sdt":
                    DocxSdtReader sdtReader = DocxReaderFactory.CreateBlockLevelSdtReader();
                    sdtReader.Read(reader);
                    break;
                case "customXml":
                    DocxCustomXmlMarkupReader customXmlReader = DocxReaderFactory.CreateBlockLevelCustomXmlReader();
                    customXmlReader.Read(reader);
                    break;
                case "altChunk":
                    DocxAltChunkReader.Read(reader);
                    break;
                // WORDSNET-10436 <w:br> element inside body element. Read it and when new paragraph will be read, insert it there, as MSW does.
                case "br":
                    xmlReader.RunTextBuilder.Append(NrxRunReaderBase.ReadBreak(xmlReader, new RunPr()));
                    break;
                // WORDSNET-13091 The w:pict element is directly in the w:body element. Cover it with a new paragraph and
                // run like MS Word does.
                case "pict":
                // WORDSNET-22916 A w:drawing element is located directly in a w:body element. Fix it in the same way as
                // for 'pict'.
                case "drawing":
                    CompositeNode savedContainer = null;
                    if (!reader.CurContainer.CanInsert(new Run(reader.Document)))
                    {
                        // Create dummy container and read into it.
                        savedContainer = reader.CurContainer;
                        reader.CurContainer = new Paragraph(reader.Document);
                    }

                    DocxReaderFactory.RunReader.ReadTextLevelElement(reader, new RunPr());
                    reader.AddPendingInline(reader.CurContainer.FirstChild, savedContainer);

                    if (savedContainer != null)
                    {
                        // Postpone read inline node and restore container.
                        reader.CurContainer = savedContainer;
                    }

                    break;
                case "sectPr":
                    // WORDSNET-17627 w:sectPr element is directly in w:document element.
                    reader.ReadGlobalSectPr(false);
                    break;
                default:
                    // DD: there seem to be other elements that we dont support, e.g. formulas, that require this ReadChildren.
                    ReadChildren(reader);
                    break;
            }

            if (isSectionBreak)
            {
                if(reader.CurContainer.NodeType == NodeType.Body)
                    reader.StartNewSection();
                else
                    reader.WarnUnexpected(WarningStrings.UnexpectedSectionBreak);
            }
        }

        private readonly DocxParaReader mParaReader;
    }
}
