// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/11/2007 by Roman Korchagin

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides methods to read "inline" DOCX elements. Inline elements are those that normally
    /// occur inside a paragraph. But in DOCX the same set of elements can occur inside hyperlink,
    /// fldSimple, smartTag and few other elements, therefore this common code is factored out here.
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal class DocxInlineReader : NrxInlineReaderBase
    {
        internal DocxInlineReader(DocxHyperlinkReader hyperlinkReader,
            DocxSmartTagReader smartTagReader)
        {
            Debug.Assert(hyperlinkReader != null);
            Debug.Assert(smartTagReader != null);
            mHyperlinkReader = hyperlinkReader;
            mSmartTagReader = smartTagReader;
        }

        /// <summary>
        /// Reads all children of the current element.
        /// The current element is supposed to a container for inline elements such as hyperlink, fldSimple etc.
        /// Reads all children of the current element.
        /// </summary>
        internal override void ReadChildren(NrxDocumentReaderBase reader, RunPr runPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            xmlReader.MoveToElement();
            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
                ReadChild(reader, runPr);
        }

        /// <summary>
        /// Reads one current element. Factored into a separate method to make reading of child
        /// of paragraph elements possible since pPr is not an inline element and is read in the
        /// paragraph reader.
        /// </summary>
        protected override void ReadChild(NrxDocumentReaderBase reader, RunPr runPr)
        {
            runPr = runPr != null
                ? runPr.Clone()
                : new RunPr();

            NrxXmlReader xmlReader = reader.XmlReader;
            switch (xmlReader.LocalName)
            {
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
                case "subDoc":
                {
                    string relId = reader.XmlReader.ReadAttribute("id", "");
                    string docFileName = ((Document)reader.Document).OriginalFileName;
                    string target = UriUtil.UnescapeHref(reader.GetRelationshipTarget(relId));
                    reader.AddChild(new SubDocument(reader.Document, UriUtil.GetAbsoluteFileName(docFileName, target)));
                    break;
                }
                case "r":
                {
                    DocxReaderFactory.RunReader.Read(reader, runPr);
                    break;
                }
                case "hyperlink":
                    mHyperlinkReader.Read(reader);
                    break;
                case "smartTag":
                    mSmartTagReader.Read(reader);
                    break;
                case "del":
                    mAnnotationReader.ReadInlineDeletion(reader);
                    break;
                case "ins":
                    mAnnotationReader.ReadInlineInsertion(reader);
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
                case "moveFrom":
                    mAnnotationReader.ReadInlineMove(reader, false);
                    break;
                case "moveTo":
                    mAnnotationReader.ReadInlineMove(reader, true);
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
                    DocxSdtReader sdtReader = DocxReaderFactory.CreateInlineLevelSdtReader();
                    sdtReader.Read(reader);
                    break;
                case "customXml":
                    DocxCustomXmlMarkupReader customXmlReader = DocxReaderFactory.CreateInlineLevelCustomXmlReader();
                    customXmlReader.Read(reader);
                    break;
                case "oMath":
                    DocxMathReader.ReadOMath(reader);
                    break;
                case "oMathPara":
                    DocxMathReader.ReadOMathPara(reader);
                    break;
                case "altChunk":
                    DocxAltChunkReader.Read(reader);
                    break;
                // WORDSNET-11281 numPr element was not loaded if it is in p element
                case "numPr":
                    if (reader.CurContainer.NodeType == NodeType.Paragraph)
                    {
                        ParaPr paraPr = ((Paragraph)reader.CurContainer).ParaPr;
                        DocxReaderFactory.ParaPrReader.ReadNumPrWithCollectingListId(reader, paraPr);
                    }
                    break;
                case "fldData":
                    NrxFldCharReaderBase.ReadFieldData(reader);
                    break;
                case "bdo":
                case "dir":
                    DocxBidiControlCharReader.Read(reader, this);
                    break;
                default:
                {
                    // WORDSNET-20919 There is drawing element in 'p'. Fixed by reading text level elements here.
                    if (!DocxReaderFactory.RunReader.ReadTextLevelElement(reader, runPr))
                        // DD: Left for resiliency, per spec it seems that we read all inlines that are out there.
                        ReadChildren(reader);
                    break;
                }
            }
        }

        internal void SetAnnotationReader(DocxAnnotationReader annotationReader)
        {
            Debug.Assert(annotationReader != null);
            mAnnotationReader = annotationReader;
        }

        private DocxAnnotationReader mAnnotationReader;
        private readonly DocxHyperlinkReader mHyperlinkReader;
        private readonly DocxSmartTagReader mSmartTagReader;
    }
}
