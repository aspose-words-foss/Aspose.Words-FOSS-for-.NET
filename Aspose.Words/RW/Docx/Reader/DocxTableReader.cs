// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/08/2007 by Vladimir Averkin

using Aspose.Collections;
using Aspose.Words.Drawing;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.Tables;


namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides static method for reading 'w:tbl' element.
    /// </summary>
    internal static class DocxTableReader
    {
        /// <summary>
        /// Reads 'w:tbl' element.
        /// </summary>
        internal static void Read(NrxDocumentReaderBase reader)
        {
            reader.AddAndPushContainer(new Table(reader.Document));

            TablePr tablePr = new TablePr();
            // WORDSNET-9225 This property should be 'true' by default.
            tablePr.AllowAutoFit = true;

            Table table = (Table)reader.CurContainer;
            reader.FlushPendingMarkup(table);

            ReadChildren(reader, tablePr);

            reader.PopContainer(NodeType.Table);

            // WORDSNET-27404 Perform correction of floating table alignment.
            // AM. I do not quite understand MS Word logic here and made fix as narrow as possible.
            if((tablePr.Alignment == TableAlignment.Center) && tablePr.Contains(TableAttr.FrameLeft))
                table.SetAttrOnAllRowsNoOverride(TableAttr.HorizontalAlignment, HorizontalAlignment.Center);

            if (!reader.LoadOptions.SkipFormatting)
                reader.FixMissingTableNormalIfNeeded(table);
        }

        internal static void ReadChildren(NrxDocumentReaderBase reader, TablePr tablePr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            xmlReader.MoveToElement();
            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
                ReadChild(reader, tablePr);
        }

        /// <summary>
        /// Refactored into a separate internal method to allow reading contents of Row level CustomXmlMarkup elements.
        /// </summary>
        internal static void ReadChild(NrxDocumentReaderBase reader, TablePr tablePr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            switch (xmlReader.LocalName)
            {
                case "tblPr":
                    DocxTablePrReader.ReadTblPr(reader, tablePr);
                    break;
                case "tblGrid":
                    if (reader.LoadOptions.SkipFormatting)
                    {
                        xmlReader.IgnoreElementNoWarn();
                        break;
                    }

                    // AM. If we read this element I think current container must be Table.
                    NrxTableGrid.ReadTblGrid(xmlReader, ((Table)reader.CurContainer).TablePr, complianceInfo);
                    break;
                case "p":
                    ReadParagraph(reader);
                    break;
                case "tr":
                    DocxRowReader.Read(reader, tablePr);
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
                    DocxSdtReader sdtReader = DocxReaderFactory.CreateRowLevelSdtReader(tablePr);
                    sdtReader.Read(reader);
                    break;
                case "customXml":
                    CompositeNode container = reader.CurContainer;
                    int childCount = container.Count;

                    DocxCustomXmlMarkupReader customXmlReader = DocxReaderFactory.CreateRowLevelCustomXmlReader(tablePr);
                    customXmlReader.Read(reader);

                    // WORDSNET-18395 If a table contains an empty customXml element on row level, MS Word divides the table
                    // into two ones and inserts a paragraph between.
                    if ((container == reader.CurContainer) && (childCount == container.Count))
                    {
                        Table curTable = (Table)reader.CurContainer;

                        reader.PopContainer(NodeType.Table);
                        reader.AddAndPushContainer(new Paragraph(reader.Document));
                        reader.PopContainer(NodeType.Paragraph);

                        Table newTable = new Table(reader.Document);
                        CloneTableAttributes(curTable, newTable);

                        reader.AddAndPushContainer(newTable);
                    }
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
                default:
                    // DD: there seem to be other elements that we dont support, e.g. formulas, that require this ReadChildren.
                    ReadChildren(reader, tablePr);
                    break;
            }
        }

        /// <summary>
        /// Resiliently reads paragraph element occurred at row level.
        /// </summary>
        private static void ReadParagraph(NrxDocumentReaderBase reader)
        {
            // Break current table.
            reader.PopContainer(NodeType.Table);

            // Resiliently reads paragraph.
            DocxParaReader paraReader =
                new DocxParaReader(new DocxParaPrReader(new DocxRunPrReader(), new DocxSectPrReader()),
                    new DocxInlineReader(new DocxHyperlinkReader(), new DocxSmartTagReader()));
            paraReader.Read(reader);

            // And start new table.
            reader.AddAndPushContainer(new Table(reader.Document));
        }

        private static void CloneTableAttributes(Table srcTable, Table dstTable)
        {
            const int tableGridKey = TableAttr.Sys_TableGrid;
            const int tableGridKey2 = TableAttr.Sys_TableGridForNewAlgorithm;


            // AM. This should be done by ExpandTo but TableGrid is not IComplexAttr and
            // have to be cloned manually.
            object srcTableGrid = srcTable.TablePr[tableGridKey];
            if (srcTableGrid != null)
            {
                IntList dstTableGrid = new IntList();
                dstTableGrid.AddRange((IntList)srcTableGrid);
                dstTable.TablePr.SetAttr(tableGridKey, dstTableGrid);
            }

            object srcTableGrid2 = srcTable.TablePr[tableGridKey2];
            if (srcTableGrid2 != null)
            {
                dstTable.TablePr.SetAttr(tableGridKey2, ((IComplexAttr)srcTableGrid2).DeepCloneComplexAttr());
            }
        }
    }
}
