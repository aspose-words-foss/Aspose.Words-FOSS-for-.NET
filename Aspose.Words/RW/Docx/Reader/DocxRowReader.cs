// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/08/2007 by Vladimir Averkin

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides static method for parsing 'w:tr' element.
    /// </summary>
    internal static class DocxRowReader
    {
        /// <summary>
        /// Reads 'w:tr' element.
        /// </summary>
        internal static void Read(NrxDocumentReaderBase reader, TablePr tablePr)
        {
            TablePr rowPr = tablePr.Clone();
            Row row = new Row(reader.Document, rowPr);
            reader.AddAndPushContainer(row);

            // Read attributes.
            if (!reader.LoadOptions.SkipFormatting)
            {
                NrxXmlReader xmlReader = reader.XmlReader;
                while (xmlReader.MoveToNextAttribute())
                {
                    // The reading code below is not full because not all rsid attributes are in the model now.
                    switch (xmlReader.LocalName)
                    {
                        case "rsidTr":
                        {
                            int rsid = NrxXmlUtil.TryHexToInt(xmlReader.Value);
                            if (rsid != int.MinValue)
                                rowPr.SetAttr(TableAttr.RsidTr, rsid);
                            break;
                        }

                        case "paraId":
                            row.ParaId = NrxXmlUtil.TryHexToInt(xmlReader.Value); 
                            break;

                        case "textId":
                            row.TextId = NrxXmlUtil.TryHexToInt(xmlReader.Value); 
                            break;

                        default:
                            break;
                    }
                }
            }

            ReadChildren(reader, rowPr);
            reader.FlushPendingParagraphs(row);

            // WORDSNET-19201 Flush formatting collected outside 'tr' element.
            if (reader.GlobalRowPr.Count > 0)
            {
                reader.GlobalRowPr.ExpandTo(rowPr);
                reader.GlobalRowPr.Clear();
            }

            reader.PopContainer(NodeType.Row);

            if (!reader.LoadOptions.SkipFormatting)
                NrxTableGrid.TableGridToCellWidths(row, reader.TablesWithMissedTableGrid);
        }

        internal static void ReadChildren(NrxDocumentReaderBase reader, TablePr rowPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            xmlReader.MoveToElement();
            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
                ReadChild(reader, rowPr);
        }

        /// <summary>
        /// Refactored into a separate internal method to allow reading contents of Row level CustomXmlMarkup elements. 
        /// </summary>
        internal static void ReadChild(NrxDocumentReaderBase reader, TablePr rowPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            switch (xmlReader.LocalName)
            {
                case "tblPrEx":
                    DocxTablePrReader.ReadTblPrEx(reader, rowPr);
                    break;
                case "trPr":
                    DocxRowPrReader.Read(reader, rowPr);
                    break;
                case "tc":
                {
                    DocxReaderFactory.CellReader.Read(reader);

                    // WORDSNET-24470 Flush pending paragraphs to a first occurred cell.
                    if ((reader.CurContainer != null) && (reader.CurContainer.NodeType == NodeType.Row))
                        reader.FlushPendingParagraphs(((Row)reader.CurContainer).LastCell);
                    break;
                }
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
                {
                    DocxSdtReader sdtReader = DocxReaderFactory.CreateCellLevelSdtReader(rowPr);
                    sdtReader.Read(reader);
                    break;
                }
                case "customXml":
                {
                    DocxCustomXmlMarkupReader customXmlReader = DocxReaderFactory.CreateCellLevelCustomXmlReader(rowPr);
                    customXmlReader.Read(reader);
                    break;
                }
                case "p":
                {
                    // Resiliency.
                    ReadPendingParagraphs(reader);
                    break;
                }
                default:
                    // DD: there seem to be other elements that we dont support, e.g. formulas, that require this ReadChildren.
                    ReadChildren(reader, rowPr);
                    break;
            }
        }

        private static void ReadPendingParagraphs(NrxDocumentReaderBase reader)
        {
            // Read unexpected paragraph from the cell level to the temporary container.
            Cell tmpCell = new Cell(reader.Document);
            reader.AddAndPushContainer(tmpCell);

            DocxReaderFactory.StoryReader.ReadChild(reader);

            reader.PopContainer(NodeType.Cell);

            // Just the one paragraph expected.
            Debug.Assert(tmpCell.FirstParagraph == tmpCell.LastParagraph);
            reader.AddPendingParagraph(tmpCell.FirstParagraph);
            tmpCell.Remove();
        }
    }
}
