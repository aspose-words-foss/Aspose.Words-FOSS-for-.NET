// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

using System;
using System.Collections.Generic;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Tables;
using DataRow = Aspose.Words.Fields.FieldDatabaseDataRow;
using DataTable = Aspose.Words.Fields.FieldDatabaseDataTable;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the DATABASE field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts the results of a database query into a WordprocessingML table.
    /// </remarks>
    public class FieldDatabase : Field, IFieldCodeTokenInfoProvider
    {
        static FieldDatabase()
        {
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Simple1] = StyleIdentifier.TableSimple1;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Simple2] = StyleIdentifier.TableSimple2;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Simple3] = StyleIdentifier.TableSimple3;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Classic1] = StyleIdentifier.TableClassic1;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Classic2] = StyleIdentifier.TableClassic2;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Classic3] = StyleIdentifier.TableClassic3;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Classic4] = StyleIdentifier.TableClassic4;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Colorful1] = StyleIdentifier.TableColorful1;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Colorful2] = StyleIdentifier.TableColorful2;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Colorful3] = StyleIdentifier.TableColorful3;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Columns1] = StyleIdentifier.TableColumns1;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Columns2] = StyleIdentifier.TableColumns2;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Columns3] = StyleIdentifier.TableColumns3;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Columns4] = StyleIdentifier.TableColumns4;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Columns5] = StyleIdentifier.TableColumns5;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Grid1] = StyleIdentifier.TableGrid1;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Grid2] = StyleIdentifier.TableGrid2;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Grid3] = StyleIdentifier.TableGrid3;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Grid4] = StyleIdentifier.TableGrid4;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Grid5] = StyleIdentifier.TableGrid5;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Grid6] = StyleIdentifier.TableGrid6;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Grid7] = StyleIdentifier.TableGrid7;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Grid8] = StyleIdentifier.TableGrid8;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.List1] = StyleIdentifier.TableList1;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.List2] = StyleIdentifier.TableList2;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.List3] = StyleIdentifier.TableList3;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.List4] = StyleIdentifier.TableList4;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.List5] = StyleIdentifier.TableList5;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.List6] = StyleIdentifier.TableList6;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.List7] = StyleIdentifier.TableList7;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.List8] = StyleIdentifier.TableList8;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.ThreeDimensionalFx1] = StyleIdentifier.Table3DEffects1;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.ThreeDimensionalFx2] = StyleIdentifier.Table3DEffects2;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.ThreeDimensionalFx3] = StyleIdentifier.Table3DEffects3;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Contemporary] = StyleIdentifier.TableContemporary;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Elegant] = StyleIdentifier.TableElegant;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Professional] = StyleIdentifier.TableProfessional;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Subtle1] = StyleIdentifier.TableSubtle1;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Subtle2] = StyleIdentifier.TableSubtle2;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Web1] = StyleIdentifier.TableWeb1;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Web2] = StyleIdentifier.TableWeb2;
            gTableFormatToStyleMap[FieldDatabaseTableFormat.Web3] = StyleIdentifier.TableWeb3;
        }

        internal override FieldUpdateAction UpdateCore()
        {
            IFieldDatabaseProvider provider = FetchDocument().FieldOptions.FieldDatabaseProvider;
            if (provider == null)
                return null;

            DataTable queryResult = provider.GetQueryResult(FileName, Connection, Query, this);

            if (queryResult == null)
                return new FieldUpdateActionInsertErrorMessage(this, "Error! Cannot open data source.");

            Table table = BuildTable(queryResult);
            return table != null
                ? new FieldUpdateActionApplyResult(this, new NodeRangeFieldResult(new NodeRange(table, table)), false)
                : new FieldUpdateActionApplyResult(this, string.Empty);
        }

        private Table BuildTable(DataTable dataTable)
        {
            dataTable = TrimRecords(dataTable);
            if (dataTable.Rows.Count == 0)
                return null;

            Table table = new Table(Document);

            if (InsertHeadings)
                InsertHeadingRow(dataTable, table);

            foreach (DataRow row in dataTable.Rows)
                InsertDataRow(row, table);

            FormatTable(table);

            return table;
        }

        private DataTable TrimRecords(DataTable dataTable)
        {
            int firstRecord = FormatterPal.TryParseInt(FirstRecord);
            int lastRecord = FormatterPal.TryParseInt(LastRecord);
            return FieldDatabaseUtils.TrimRecords(dataTable, firstRecord, lastRecord);
        }

        private static void InsertHeadingRow(DataTable dataTable, Table table)
        {
            InsertRow(table, dataTable.ColumnNames);
        }

        private static void InsertDataRow(DataRow row, Table table)
        {
            InsertRow(table, row.Values);
        }

        private static void InsertRow(Table table, string[] cellTexts)
        {
            Row row = new Row(table.Document);
            table.Rows.Add(row);
            foreach (string cellText in cellTexts)
            {
                Cell cell = new Cell(table.Document);
                row.Cells.Add(cell);
                Paragraph paragraph = new Paragraph(cell.Document);
                cell.Paragraphs.Add(paragraph);
                paragraph.Runs.Add(new Run(cell.Document, cellText ?? string.Empty));
            }
        }

        private void FormatTable(Table table)
        {
            FieldDatabaseTableFormat tableFormat = TableFormatCore;
            FieldDatabaseFormatAttributes attributes = FormatAttributesCore;

            TableStyle style = null;
            if (attributes != FieldDatabaseFormatAttributes.None)
            {
                StyleIdentifier styleIdentifier;
                if (gTableFormatToStyleMap.TryGetValue(tableFormat, out styleIdentifier))
                    style = (TableStyle)Document.Styles.GetBySti(styleIdentifier, true);
            }

            FieldDatabaseTableStyleApplier.Apply(table, style, tableFormat, attributes);
        }

        /// <summary>
        /// Gets or sets which attributes of the format are to be applied to the table.
        /// </summary>
        public string FormatAttributes //int
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FormatAttributesSwitch); }
            set { FieldCodeCache.SetSwitchAsInt32(FormatAttributesSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a connection to the data.
        /// </summary>
        public string Connection
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(ConnectionSwitch); }
            set { FieldCodeCache.SetSwitch(ConnectionSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the complete path and file name of the database
        /// </summary>
        public string FileName
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FileNameSwitch); }
            set { FieldCodeCache.SetSwitch(FileNameSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the integral record number of the first data record to insert.
        /// </summary>
        public string FirstRecord //int
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FirstRecordSwitch); }
            set { FieldCodeCache.SetSwitchAsInt32(FirstRecordSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the field names from the database as column headings in
        /// the resulting table.
        /// </summary>
        public bool InsertHeadings
        {
            get { return FieldCodeCache.HasSwitch(InsertHeadingsSwitch); }
            set { FieldCodeCache.SetSwitch(InsertHeadingsSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the format that is to be applied to the result of the database query.
        /// </summary>
        public string TableFormat //int
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(TableFormatSwitch); }
            set { FieldCodeCache.SetSwitchAsInt32(TableFormatSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert data at the beginning of a merge.
        /// </summary>
        public bool InsertOnceOnMailMerge
        {
            get { return FieldCodeCache.HasSwitch(InsertOnceOnMailMergeSwitch); }
            set { FieldCodeCache.SetSwitch(InsertOnceOnMailMergeSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a set of SQL instructions that query the database.
        /// </summary>
        public string Query
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(QuerySwitch); }
            set { FieldCodeCache.SetSwitch(QuerySwitch, value); }
        }

        /// <summary>
        /// Gets or sets the integral record number of the last data record to insert.
        /// </summary>
        public string LastRecord //int
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(LastRecordSwitch); }
            set { FieldCodeCache.SetSwitchAsInt32(LastRecordSwitch, value); }
        }

        private FieldDatabaseFormatAttributes FormatAttributesCore
        {
            get
            {
                int formatAttributes = FormatterPal.TryParseInt(FormatAttributes);
                return formatAttributes == int.MinValue
                    ? FieldDatabaseFormatAttributes.None
                    : (FieldDatabaseFormatAttributes)formatAttributes;
            }
        }

        private FieldDatabaseTableFormat TableFormatCore
        {
            get
            {
                int tableFormat = FormatterPal.TryParseInt(TableFormat);
                return tableFormat == int.MinValue
                    ? FieldDatabaseTableFormat.None
                    : (FieldDatabaseTableFormat)tableFormat;
            }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case InsertHeadingsSwitch:
                case InsertOnceOnMailMergeSwitch:
                {
                    return FieldSwitchType.Flag;
                }
                case FormatAttributesSwitch:
                case ConnectionSwitch:
                case FileNameSwitch:
                case FirstRecordSwitch:
                case TableFormatSwitch:
                case QuerySwitch:
                case LastRecordSwitch:
                {
                    return FieldSwitchType.HasArgument;
                }
                default:
                {
                    return FieldSwitchType.Unknown;
                }
            }
        }

        private const string FormatAttributesSwitch = "\\b";
        private const string ConnectionSwitch = "\\c";
        private const string FileNameSwitch = "\\d";
        private const string FirstRecordSwitch = "\\f";
        private const string InsertHeadingsSwitch = "\\h";
        private const string TableFormatSwitch = "\\l";
        private const string InsertOnceOnMailMergeSwitch = "\\o";
        private const string QuerySwitch = "\\s";
        private const string LastRecordSwitch = "\\t";

        private static readonly IDictionary<FieldDatabaseTableFormat, StyleIdentifier> gTableFormatToStyleMap = new Dictionary<FieldDatabaseTableFormat, StyleIdentifier>();

        private static class FieldDatabaseTableStyleApplier
        {
            static FieldDatabaseTableStyleApplier()
            {
                gMapping.WhenFormatsAreSpecified((int)FieldDatabaseFormatAttributes.Borders)
                    .ThenCopyTableAttrs(TableAttr.BorderTop, TableAttr.BorderLeft, TableAttr.BorderBottom, TableAttr.BorderRight, TableAttr.BorderHorizontal, TableAttr.BorderVertical)
                    .ThenCopyTableAttrs(TableAttr.LeftPadding, TableAttr.TopPadding, TableAttr.BottomPadding, TableAttr.RightPadding)
                    .ThenCopyTableAttrs(TableAttr.LeftIndent)
                    .ThenCopyTableAttrs(TableAttr.CellSpacing)
                    .ThenCopyCellAttrs(CellAttr.BorderTop, CellAttr.BorderLeft, CellAttr.BorderBottom, CellAttr.BorderRight, CellAttr.BorderHorizontal, CellAttr.BorderVertical)
                    .ThenCopyCellAttrs(CellAttr.BorderDiagonalDown, CellAttr.BorderDiagonalUp);

                gMapping.WhenFormatsAreSpecified((int)FieldDatabaseFormatAttributes.Shading)
                    .ThenCopyCellAttrs(CellAttr.Shading);

                gMapping.WhenFormatsAreSpecified((int)FieldDatabaseFormatAttributes.Font)
                    .ThenCopyRunAttrs(FontAttr.Bold, FontAttr.Italic, FontAttr.BoldBi, FontAttr.ItalicBi, FontAttr.AllCaps);

                gMapping.WhenFormatsAreSpecified((int)FieldDatabaseFormatAttributes.Shading, (int)FieldDatabaseFormatAttributes.Font)
                    .ThenCopyRunAttrs(FontAttr.Color);

                gBandsRowsColsCornersOrder = new ConditionalStyleType[]
                {
                    ConditionalStyleType.OddRowBanding,
                    ConditionalStyleType.EvenRowBanding,
                    ConditionalStyleType.OddColumnBanding,
                    ConditionalStyleType.EvenColumnBanding,

                    ConditionalStyleType.FirstRow,
                    ConditionalStyleType.LastRow,
                    ConditionalStyleType.FirstColumn,
                    ConditionalStyleType.LastColumn,

                    ConditionalStyleType.TopLeftCell,
                    ConditionalStyleType.TopRightCell,
                    ConditionalStyleType.BottomLeftCell,
                    ConditionalStyleType.BottomRightCell
                };

                gBandsColsRowsCornersOrder = new ConditionalStyleType[]
                {
                    ConditionalStyleType.OddRowBanding,
                    ConditionalStyleType.EvenRowBanding,
                    ConditionalStyleType.OddColumnBanding,
                    ConditionalStyleType.EvenColumnBanding,

                    ConditionalStyleType.FirstColumn,
                    ConditionalStyleType.LastColumn,
                    ConditionalStyleType.FirstRow,
                    ConditionalStyleType.LastRow,

                    ConditionalStyleType.TopLeftCell,
                    ConditionalStyleType.TopRightCell,
                    ConditionalStyleType.BottomLeftCell,
                    ConditionalStyleType.BottomRightCell
                };
            }

            internal static void Apply(Table table, TableStyle style, FieldDatabaseTableFormat tableFormat, FieldDatabaseFormatAttributes specifiedFormats)
            {
                if ((tableFormat == FieldDatabaseTableFormat.None)
                    || IsFormatSpecified(specifiedFormats, FieldDatabaseFormatAttributes.Borders)
                    || IsFormatSpecified(specifiedFormats, FieldDatabaseFormatAttributes.Shading))
                    ClearTableStyle(table);

                if (IsFormatSpecified(specifiedFormats, FieldDatabaseFormatAttributes.AutoFit))
                {
                    table.PreferredWidth = PreferredWidth.Auto;
                    table.AllowAutoFit = true;
                }
                else
                {
                    table.PreferredWidth = PreferredWidth.FromPercent(100);
                    table.AllowAutoFit = false;
                }

                if (style == null)
                    return;

                table.TablePr.SetAttr(TableAttr.StyleOptions, (int)specifiedFormats);
                table.SetAttrOnAllRows(TableAttr.StyleOptions, (int)specifiedFormats);

                CopyTableStyle(table, style, tableFormat, specifiedFormats);
            }

            private static void ClearTableStyle(Table table)
            {
                table.TablePr.Clear();
                foreach (Row row in table.Rows)
                {
                    row.TablePr.Clear();
                    foreach (Cell cell in row.Cells)
                    {
                        cell.CellPr.Clear();
                        foreach (Paragraph paragraph in cell.Paragraphs)
                        {
                            paragraph.ParaPr.Clear();
                            foreach (Run run in paragraph.Runs)
                                run.RunPr.Clear();
                        }
                    }
                }
            }

            private static void CopyTableStyle(Table table, TableStyle style, FieldDatabaseTableFormat tableFormat, FieldDatabaseFormatAttributes specifiedFormats)
            {
                CopyTableAttrs(new TableStyleAdapter(style), table, specifiedFormats);

                ConditionalStyleType[] tableStyleOverrideTypeOrder = GetTableStyleOverrideTypeOrder(tableFormat);

                foreach (ConditionalStyleType tableStyleOverrideType in tableStyleOverrideTypeOrder)
                {
                    ConditionalStyle conditionalStyle = style.ConditionalStyles[tableStyleOverrideType];
                    if (!conditionalStyle.HasFormatting)
                        continue;

                    ITableStyle tableStyle = new ConditionalStyleAdapter(conditionalStyle);

                    switch (tableStyleOverrideType)
                    {
                        case ConditionalStyleType.FirstColumn:
                        {
                            if (!IsFormatSpecified(specifiedFormats, FieldDatabaseFormatAttributes.FirstColumn))
                                break;

                            foreach (Row row in table.Rows)
                            {
                                int[] keysToIgnore = !MathUtil.IsOdd(row.RowIndex) && IgnoreFirstColumnEvenCellRightBorder(tableFormat)
                                    ? new int[] { CellAttr.BorderRight }
                                    : new int[0];
                                CopyCellAttrs(tableStyle, row.FirstCell, specifiedFormats, keysToIgnore);
                            }
                            break;
                        }
                        case ConditionalStyleType.LastColumn:
                        {
                            if (!IsFormatSpecified(specifiedFormats, FieldDatabaseFormatAttributes.LastColumn))
                                break;

                            foreach (Row row in table.Rows)
                                CopyCellAttrs(tableStyle, row.LastCell, specifiedFormats);
                            break;
                        }
                        case ConditionalStyleType.FirstRow:
                        {
                            if (!IsFormatSpecified(specifiedFormats, FieldDatabaseFormatAttributes.FirstRow))
                                break;

                            CopyRowAttrs(tableStyle, table.FirstRow, specifiedFormats);
                            break;
                        }
                        case ConditionalStyleType.LastRow:
                        {
                            if (!IsFormatSpecified(specifiedFormats, FieldDatabaseFormatAttributes.LastRow))
                                break;

                            CopyRowAttrs(tableStyle, table.LastRow, specifiedFormats);
                            break;
                        }

                        case ConditionalStyleType.TopLeftCell:
                        {
                            if (!IsFormatSpecified(specifiedFormats, GetCornerFormatAttribute(tableFormat, tableStyleOverrideType)))
                                break;

                            CopyCellAttrs(tableStyle, table.FirstRow.FirstCell, specifiedFormats);
                            break;
                        }
                        case ConditionalStyleType.TopRightCell:
                        {
                            if (!IsFormatSpecified(specifiedFormats, GetCornerFormatAttribute(tableFormat, tableStyleOverrideType)))
                                break;

                            CopyCellAttrs(tableStyle, table.FirstRow.LastCell, specifiedFormats);
                            break;
                        }
                        case ConditionalStyleType.BottomLeftCell:
                        {
                            if (!IsFormatSpecified(specifiedFormats, GetCornerFormatAttribute(tableFormat, tableStyleOverrideType)))
                                break;

                            CopyCellAttrs(tableStyle, table.LastRow.FirstCell, specifiedFormats);
                            break;
                        }
                        case ConditionalStyleType.BottomRightCell:
                        {
                            if (!IsFormatSpecified(specifiedFormats, GetCornerFormatAttribute(tableFormat, tableStyleOverrideType)))
                                break;

                            CopyCellAttrs(tableStyle, table.LastRow.LastCell, specifiedFormats);
                            break;
                        }

                        case ConditionalStyleType.OddRowBanding:
                        {
                            int bandSize = style.TablePr.StyleRowBandSize;
                            int rowsCount = table.Rows.Count;
                            for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                                if (IsRowBanded(rowIndex, bandSize, true, rowsCount, specifiedFormats, tableFormat))
                                    CopyRowAttrs(tableStyle, table.Rows[rowIndex], specifiedFormats);
                            break;
                        }
                        case ConditionalStyleType.EvenRowBanding:
                        {
                            int bandSize = style.TablePr.StyleRowBandSize;
                            int rowsCount = table.Rows.Count;
                            for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                                if (IsRowBanded(rowIndex, bandSize, false, rowsCount, specifiedFormats, tableFormat))
                                    CopyRowAttrs(tableStyle, table.Rows[rowIndex], specifiedFormats);
                            break;
                        }
                        case ConditionalStyleType.OddColumnBanding:
                        {
                            foreach (Row row in table.Rows)
                                for (int cellIndex = 0; cellIndex < row.Cells.Count; cellIndex += 2)
                                    CopyCellAttrs(tableStyle, row.Cells[cellIndex], specifiedFormats);
                            break;
                        }
                        case ConditionalStyleType.EvenColumnBanding:
                        {
                            foreach (Row row in table.Rows)
                                for (int cellIndex = 1; cellIndex < row.Cells.Count; cellIndex += 2)
                                    CopyCellAttrs(tableStyle, row.Cells[cellIndex], specifiedFormats);
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            private static bool IgnoreFirstColumnEvenCellRightBorder(FieldDatabaseTableFormat tableFormat)
            {
                switch (tableFormat)
                {
                    case FieldDatabaseTableFormat.ThreeDimensionalFx2:
                    case FieldDatabaseTableFormat.ThreeDimensionalFx3:
                        return true;
                    default:
                        return false;
                }
            }

            private static bool IsRowBanded(int index, int bandSize, bool odd, int rowsCount, FieldDatabaseFormatAttributes specifiedFormats, FieldDatabaseTableFormat tableFormat)
            {
                if ((index == rowsCount - 1) && IsFormatSpecified(specifiedFormats, FieldDatabaseFormatAttributes.LastRow))
                {
                    switch (tableFormat)
                    {
                        case FieldDatabaseTableFormat.List6:
                        case FieldDatabaseTableFormat.List8:
                        case FieldDatabaseTableFormat.Contemporary:
                        case FieldDatabaseTableFormat.ThreeDimensionalFx2:
                        case FieldDatabaseTableFormat.ThreeDimensionalFx3:
                            break;
                        default:
                            return false;
                    }
                }

                return MathUtil.IsOdd((index + bandSize - 1)/ bandSize) == odd;
            }

            private static ConditionalStyleType[] GetTableStyleOverrideTypeOrder(FieldDatabaseTableFormat tableFormat)
            {
                switch (tableFormat)
                {
                    case FieldDatabaseTableFormat.Colorful2:
                    case FieldDatabaseTableFormat.Subtle2:
                        return gBandsColsRowsCornersOrder;
                    default:
                        return gBandsRowsColsCornersOrder;
                }
            }

            private static FieldDatabaseFormatAttributes GetCornerFormatAttribute(FieldDatabaseTableFormat tableFormat, ConditionalStyleType tableStyleOverrideType)
            {
                switch (tableFormat)
                {
                    case FieldDatabaseTableFormat.Columns1:
                    case FieldDatabaseTableFormat.Columns2:
                    case FieldDatabaseTableFormat.Columns3:
                    case FieldDatabaseTableFormat.Grid5:
                    case FieldDatabaseTableFormat.Grid6:
                    case FieldDatabaseTableFormat.Grid7:
                    case FieldDatabaseTableFormat.ThreeDimensionalFx1:
                    case FieldDatabaseTableFormat.ThreeDimensionalFx2:
                    case FieldDatabaseTableFormat.ThreeDimensionalFx3:
                    case FieldDatabaseTableFormat.Subtle2:
                        return GetColumnRelatedCornerFormatAttribute(tableStyleOverrideType);
                    case FieldDatabaseTableFormat.Classic1:
                    case FieldDatabaseTableFormat.Classic2:
                        return GetColumnAndRowRelatedCornerFormatAttribute(tableStyleOverrideType);
                    default:
                        return GetRowRelatedCornerFormatAttribute(tableStyleOverrideType);
                }
            }

            private static FieldDatabaseFormatAttributes GetColumnRelatedCornerFormatAttribute(ConditionalStyleType tableStyleOverrideType)
            {
                switch (tableStyleOverrideType)
                {
                    case ConditionalStyleType.TopLeftCell:
                    case ConditionalStyleType.BottomLeftCell:
                        return FieldDatabaseFormatAttributes.FirstColumn;
                    case ConditionalStyleType.TopRightCell:
                    case ConditionalStyleType.BottomRightCell:
                        return FieldDatabaseFormatAttributes.LastColumn;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static FieldDatabaseFormatAttributes GetRowRelatedCornerFormatAttribute(ConditionalStyleType tableStyleOverrideType)
            {
                switch (tableStyleOverrideType)
                {
                    case ConditionalStyleType.TopLeftCell:
                    case ConditionalStyleType.TopRightCell:
                        return FieldDatabaseFormatAttributes.FirstRow;
                    case ConditionalStyleType.BottomLeftCell:
                    case ConditionalStyleType.BottomRightCell:
                        return FieldDatabaseFormatAttributes.LastRow;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static FieldDatabaseFormatAttributes GetColumnAndRowRelatedCornerFormatAttribute(ConditionalStyleType tableStyleOverrideType)
            {
                switch (tableStyleOverrideType)
                {
                    case ConditionalStyleType.TopRightCell:
                        return FieldDatabaseFormatAttributes.LastColumn;
                    case ConditionalStyleType.BottomLeftCell:
                        return FieldDatabaseFormatAttributes.LastRow;
                    case ConditionalStyleType.TopLeftCell:
                        return FieldDatabaseFormatAttributes.FirstRow;
                    case ConditionalStyleType.BottomRightCell:
                        Debug.Fail("Should not reach this case");
                        return FieldDatabaseFormatAttributes.None;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void CopyTableAttrs(ITableStyle style, Table table, FieldDatabaseFormatAttributes specifiedFormats, params int[] keysToIgnore)
            {
                CopyAttrs(specifiedFormats, table.TablePr, style, AttrPrSource.TablePr, keysToIgnore);

                foreach (Row row in table.Rows)
                {
                    CopyAttrs(specifiedFormats, row.TablePr, style, AttrPrSource.TablePr, keysToIgnore); // table attributes must be specified for all rows. by design.
                    CopyRowAttrs(style, row, specifiedFormats, keysToIgnore);
                }
            }

            private static void CopyRowAttrs(ITableStyle style, Row row, FieldDatabaseFormatAttributes specifiedFormats, params int[] keysToIgnore)
            {
                foreach (Cell cell in row.Cells)
                    CopyCellAttrs(style, cell, specifiedFormats, keysToIgnore);
            }

            private static void CopyCellAttrs(ITableStyle style, Cell cell, FieldDatabaseFormatAttributes specifiedFormats, params int[] keysToIgnore)
            {
                CopyAttrs(specifiedFormats, cell.CellPr, style, AttrPrSource.CellPr, keysToIgnore);

                foreach (Paragraph paragraph in cell.Paragraphs)
                    CopyParagraphAttrs(style, paragraph, specifiedFormats, keysToIgnore);
            }

            private static void CopyParagraphAttrs(ITableStyle style, Paragraph paragraph, FieldDatabaseFormatAttributes specifiedFormats, params int[] keysToIgnore)
            {
                CopyAttrs(specifiedFormats, paragraph.ParagraphBreakRunPr, style, AttrPrSource.RunPr, keysToIgnore);

                foreach (Run run in paragraph.Runs)
                    CopyRunAttrs(style, run, specifiedFormats, keysToIgnore);
            }

            private static void CopyRunAttrs(ITableStyle style, Run run, FieldDatabaseFormatAttributes specifiedFormats, params int[] keysToIgnore)
            {
                CopyAttrs(specifiedFormats, run.RunPr, style, AttrPrSource.RunPr, keysToIgnore);
            }

            private static void CopyAttrs(FieldDatabaseFormatAttributes specifiedFormats, AttrCollection dest, ITableStyle style, AttrPrSource attrPrSource, params int[] keysToIgnore)
            {
                AttrCollection source = style[attrPrSource];
                if (source == null)
                    return;

                foreach (int key in gMapping.GetMappedAttrs(specifiedFormats, attrPrSource))
                {
                    if (ArrayUtil.Contains(keysToIgnore, key))
                        continue;

                    object value = source.GetDirectAttr(key);
                    if (value == null)
                        continue;

                    if (!IsFormatSpecified(specifiedFormats, FieldDatabaseFormatAttributes.Color))
                        value = ApplyGrayScale(value);

                    dest.SetAttr(key, value);
                }
            }

            private static object ApplyGrayScale(object value)
            {
                return ApplyGrayScale(value as DrColor)
                    ?? ApplyGrayScale(value as Shading)
                    ?? ApplyGrayScale(value as Border)
                    ?? value;
            }

            private static DrColor ApplyGrayScale(DrColor value)
            {
                if (value == null)
                    return null;

                if (value.IsGray)
                    return value;

                return IsLight(value)
                    ? new DrColor(value.A, Light, Light, Light)
                    : new DrColor(value.A, Dark, Dark, Dark);
            }

            private static Shading ApplyGrayScale(Shading value)
            {
                if (value == null)
                    return null;

                if (value.Texture == TextureIndex.TextureSolid)
                {
                    if (IsBlackOrWhite(value.BackgroundPatternColorInternal) && IsBlackOrWhite(value.ForegroundPatternColorInternal))
                        return value;

                    Shading result = value.Clone();

                    result.Texture = IsLight(result.BackgroundPatternColorInternal) && IsLight(result.ForegroundPatternColorInternal)
                        ? TextureIndex.Texture30Percent
                        : TextureIndex.Texture60Percent;

                    if (!IsBlackOrWhite(result.BackgroundPatternColorInternal))
                        result.BackgroundPatternColorInternal = DrColor.Black;

                    if (!IsBlackOrWhite(result.ForegroundPatternColorInternal))
                        result.ForegroundPatternColorInternal = DrColor.Black;

                    return result;
                }
                else
                {
                    if (value.BackgroundPatternColorInternal.IsGray &&
                        value.ForegroundPatternColorInternal.IsGray)
                        return value;

                    Shading result = value.Clone();

                    result.BackgroundPatternColorInternal = ApplyGrayScale(result.BackgroundPatternColorInternal);
                    result.ForegroundPatternColorInternal = ApplyGrayScale(result.ForegroundPatternColorInternal);

                    return result;
                }
            }

            private static Border ApplyGrayScale(Border value)
            {
                if (value == null)
                    return null;

                if (value.ColorInternal.IsGray)
                    return value;

                Border result = value.Clone();
                result.ColorInternal = ApplyGrayScale(result.ColorInternal);
                return result;
            }

            private static bool IsBlackOrWhite(DrColor color)
            {
                return color == DrColor.Black || color == DrColor.White;
            }

            private static bool IsLight(DrColor color)
            {
                return color.R >= Light || color.G >= Light || color.B >= Light;
            }

            private static bool IsFormatSpecified(FieldDatabaseFormatAttributes specifiedFormats, FieldDatabaseFormatAttributes format)
            {
                return (specifiedFormats & format) == format;
            }

            private static bool AreAllFormatsSpecified(FieldDatabaseFormatAttributes specifiedFormats, /*FieldDatabaseFormatAttributes*/int[] formats)
            {
                foreach (int i in formats)
                {
                    FieldDatabaseFormatAttributes format = (FieldDatabaseFormatAttributes)i;
                    if (!IsFormatSpecified(specifiedFormats, format))
                        return false;
                }

                return true;
            }

            private const int Light = 192;
            private const int Dark = 128;

            private static readonly StyleMapping gMapping = new StyleMapping();

            private static readonly ConditionalStyleType[] gBandsRowsColsCornersOrder;
            private static readonly ConditionalStyleType[] gBandsColsRowsCornersOrder;

            private enum AttrPrSource
            {
                TablePr,
                CellPr,
                RunPr
            }

            private class StyleMapping
            {
                internal IEnumerable<int> GetMappedAttrs(FieldDatabaseFormatAttributes specifiedFormats, AttrPrSource attrPrSource)
                {
                    IList<int> result = new List<int>();

                    IDictionary</*FieldDatabaseFormatAttributes*/int[], IList<int>> attrPrSourceMapping = GetAttrPrSourceMapping(attrPrSource);
                    foreach (/*FieldDatabaseFormatAttributes*/int[] formats in attrPrSourceMapping.Keys)
                    {
                        if (!AreAllFormatsSpecified(specifiedFormats, formats))
                            continue;

                        foreach (int key in attrPrSourceMapping[formats])
                            result.Add(key);
                    }

                    return result;
                }

                internal StyleMapping WhenFormatsAreSpecified(params /*FieldDatabaseFormatAttributes*/int[] formats)
                {
                    mLastFormats = formats;
                    return this;
                }

                internal StyleMapping ThenCopyTableAttrs(params int[] keys)
                {
                    return MapAttrs(AttrPrSource.TablePr, keys);
                }

                internal StyleMapping ThenCopyCellAttrs(params int[] keys)
                {
                    return MapAttrs(AttrPrSource.CellPr, keys);
                }

                internal StyleMapping ThenCopyRunAttrs(params int[] keys)
                {
                    return MapAttrs(AttrPrSource.RunPr, keys);
                }

                private StyleMapping MapAttrs(AttrPrSource attrPrSource, int[] keys)
                {
                    IDictionary</*FieldDatabaseFormatAttributes*/int[], IList<int>> attrPrSourceMapping = GetAttrPrSourceMapping(attrPrSource);

                    IList<int> mappedKeys;
                    if (!attrPrSourceMapping.TryGetValue(mLastFormats, out mappedKeys))
                    {
                        mappedKeys = new List<int>();
                        attrPrSourceMapping.Add(mLastFormats, mappedKeys);
                    }

                    foreach (int key in keys)
                        mappedKeys.Add(key);

                    return this;
                }

                private IDictionary</*FieldDatabaseFormatAttributes*/int[], IList<int>> GetAttrPrSourceMapping(AttrPrSource attrPrSource)
                {
                    switch (attrPrSource)
                    {
                        case AttrPrSource.TablePr:
                            return mTableAttrsMapping;
                        case AttrPrSource.CellPr:
                            return mCellAttrsMapping;
                        case AttrPrSource.RunPr:
                            return mRunAttrsMapping;
                        default:
                            throw new ArgumentOutOfRangeException("attrPrSource");
                    }
                }

                private /*FieldDatabaseFormatAttributes*/int[] mLastFormats;

                private readonly IDictionary</*FieldDatabaseFormatAttributes*/int[], IList<int>> mTableAttrsMapping = new Dictionary</*FieldDatabaseFormatAttributes*/int[], IList<int>>();
                private readonly IDictionary</*FieldDatabaseFormatAttributes*/int[], IList<int>> mRunAttrsMapping = new Dictionary</*FieldDatabaseFormatAttributes*/int[], IList<int>>();
                private readonly IDictionary</*FieldDatabaseFormatAttributes*/int[], IList<int>> mCellAttrsMapping = new Dictionary</*FieldDatabaseFormatAttributes*/int[], IList<int>>();
            }

            private interface ITableStyle
            {
                AttrCollection this[AttrPrSource attrPrSource] { get; }
            }

            private class TableStyleAdapter : ITableStyle
            {
                private readonly TableStyle mTableStyle;

                internal TableStyleAdapter(TableStyle tableStyle)
                {
                    mTableStyle = tableStyle;
                }

                AttrCollection ITableStyle.this[AttrPrSource attrPrSource]
                {
                    get
                    {
                        switch (attrPrSource)
                        {
                            case AttrPrSource.TablePr:
                                return mTableStyle.TablePr;
                            case AttrPrSource.CellPr:
                                return mTableStyle.CellPr;
                            case AttrPrSource.RunPr:
                                return mTableStyle.RunPr;
                            default:
                                throw new ArgumentOutOfRangeException("attrPrSource");
                        }
                    }
                }
            }

            private class ConditionalStyleAdapter : ITableStyle
            {
                private readonly ConditionalStyle mConditionalStyle;

                internal ConditionalStyleAdapter(ConditionalStyle conditionalStyle)
                {
                    mConditionalStyle = conditionalStyle;
                }

                AttrCollection ITableStyle.this[AttrPrSource attrPrSource]
                {
                    get
                    {
                        switch (attrPrSource)
                        {
                            case AttrPrSource.TablePr:
                                return mConditionalStyle.TablePr;
                            case AttrPrSource.CellPr:
                                return mConditionalStyle.CellPr;
                            case AttrPrSource.RunPr:
                                return mConditionalStyle.RunPr;
                            default:
                                throw new ArgumentOutOfRangeException("attrPrSource");
                        }
                    }
                }
            }
        }
    }
}
