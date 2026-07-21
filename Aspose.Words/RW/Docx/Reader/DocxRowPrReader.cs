// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/08/2007 by Vladimir Averkin

using System;
using Aspose.Words.Nrx;
using Aspose.Words.Revisions;
using Aspose.Words.RW.Nrx;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides static method for reading row properties from different document parts.
    /// </summary>
    internal static class DocxRowPrReader
    {
        /// <summary>
        /// Reads 'w:trPr' element from the specified reader. Reader should be positioned to element start.
        /// Returns a w:gridBefore value if encountered it.
        /// </summary>
        internal static void Read(NrxDocumentReaderBase reader, TablePr tablePr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;

            if (reader.LoadOptions.SkipFormatting)
            {
                xmlReader.IgnoreElementNoWarn();
                return;
            }

            while (xmlReader.ReadChild("trPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "cnfStyle":
                        // This is a calculated "helper" value, ignore.
                        xmlReader.IgnoreElementNoWarn();
                        break;
                    case "divId":
                        tablePr.SetAttr(TableAttr.HtmlBlockId, xmlReader.ReadIntVal());
                        break;
                    case "wBefore":
                    {
                        PreferredWidth widthBefore = xmlReader.ReadLength(complianceInfo);
                        tablePr.SetAttr(TableAttr.WidthBefore, widthBefore);
                        tablePr.SetAttr(TableAttr.WidthBeforeOriginal, widthBefore);
                        break;
                    }
                    case "wAfter":
                    {
                        PreferredWidth widthAfter = xmlReader.ReadLength(complianceInfo);
                        tablePr.SetAttr(TableAttr.WidthAfter, widthAfter);
                        tablePr.SetAttr(TableAttr.WidthAfterOriginal, widthAfter);
                        break;
                    }
                    case "gridBefore":
                        tablePr.SetAttr(TableAttr.Sys_GridBefore, xmlReader.ReadIntVal());
                        break;
                    case "gridAfter":
                        tablePr.SetAttr(TableAttr.Sys_GridAfter, xmlReader.ReadIntVal());
                        break;
                    case "cantSplit":
                        tablePr.SetAttr(TableAttr.AllowBreakAcrossPages, !xmlReader.ReadBoolVal());
                        break;
                    case "trHeight":
                        ReadHeight(tablePr, xmlReader, complianceInfo);
                        break;
                    case "tblHeader":
                        tablePr.SetAttr(TableAttr.HeadingFormat, xmlReader.ReadBoolVal());
                        break;
                    case "tblCellSpacing":
                        {
                            PreferredWidth cellSpacing = xmlReader.ReadCellSpacing(complianceInfo);
                            if (cellSpacing != null)
                                tablePr.SetAttr(TableAttr.CellSpacing, cellSpacing);
                            break;
                        }
                    case "jc":
                        tablePr.Alignment = NrxTableEnum.XmlToTableAlignment(xmlReader.ReadVal(), complianceInfo);
                        break;
                    case "trPrChange":
                        // RK I'm not sure this is correct, but lets keep it for now.
                        // WORDSNET-9223 When there is changes in row formatting (trPr), we should read trPrChange
                        // when we read tblPrChange instead, nodes after trPrChange can be ignored. This causes problems during opening
                        DocxAnnotationReader.ReadRowPrFormatRevision(reader, tablePr);
                        break;
                    case "del":
                        DocxAnnotationReader.ReadEditRevision(reader, tablePr, EditRevisionType.Deletion);
                        break;
                    case "ins":
                        DocxAnnotationReader.ReadEditRevision(reader, tablePr, EditRevisionType.Insertion);
                        break;
                    case "hidden":
                        tablePr.SetAttr(TableAttr.Hidden, xmlReader.ReadBoolVal());
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadHeight(TablePr tablePr, NrxXmlReader xmlReader, OoxmlComplianceInfo complianceInfo)
        {
            HeightRule hRule = HeightRule.AtLeast;
            int val = 0;

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "hRule":
                        hRule = NrxTableEnum.XmlToHeightRule(xmlReader.Value);
                        break;
                    case "val":
                        // WORDSNET-8026 Word ignores value with leading space.
                        // Limit fix to this element for a while.
                        if(!xmlReader.Value.StartsWith(" ", StringComparison.Ordinal))
                        {
                            val = xmlReader.GetValueAsTwips(complianceInfo);
                        }
                        break;
                    default:
                    {
                        string message = String.Format(WarningStrings.UnexpectedAttribute, xmlReader.LocalName);
                        xmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Docx, message);
                        break;
                    }
                }
            }

            tablePr.SetAttr(TableAttr.RowHeight, new Height(hRule, val));
        }
    }
}
