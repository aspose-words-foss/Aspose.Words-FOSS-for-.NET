// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2005 by Roman Korchagin


using Aspose.Words.Tables;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Attributes that can be defined for a table row.
    ///
    /// Note the constant values make sure the attributes are written
    /// into a binary file in a specific order and the order is important.
    /// </summary>
    [CppConstexpr]
    internal static class TableAttr
    {
        /// <summary>
        /// int. Table style index.
        /// </summary>
        internal const int Istd = 4005;
        /// <summary>
        /// enum. TableAlignment by itself does not make table floating.
        /// </summary>
        internal const int Alignment = 4010;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int LeftPadding = 4020;
        /// <summary>
        /// bool
        /// </summary>
        internal const int HeadingFormat = 4040;
        internal const int BorderTop = 4050;
        internal const int BorderLeft = 4060;
        internal const int BorderBottom = 4070;
        internal const int BorderRight = 4080;
        internal const int BorderHorizontal = 4090;
        internal const int BorderVertical = 4100;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int RowHeight = 4120;
        /// <summary>
        /// enum. <see cref="TableStyleOptions"/>
        /// </summary>
        internal const int StyleOptions = 4140;
        /// <summary>
        /// enum. Text frame.
        /// </summary>
        internal const int RelativeHorizontalPosition = 4150;
        /// <summary>
        /// enum. Text frame.
        /// </summary>
        internal const int RelativeVerticalPosition = 4160;
        /// <summary>
        /// int, twips. Text frame.
        /// </summary>
        internal const int FrameLeft = 4170;
        /// <summary>
        /// enum. Text frame.
        /// </summary>
        internal const int HorizontalAlignment = 4180;
        /// <summary>
        /// int, twips. Text frame.
        /// </summary>
        internal const int FrameTop = 4190;
        /// <summary>
        /// enum. Text frame.
        /// </summary>
        internal const int VerticalAlignment = 4200;
        /// <summary>
        /// int. Text frame.
        /// </summary>
        internal const int FrameDistanceFromLeft = 4210;
        /// <summary>
        /// int. Text frame.
        /// </summary>
        internal const int FrameDistanceFromTop = 4220;
        /// <summary>
        /// TableWidth
        /// </summary>
        internal const int PreferredWidth = 4230;
        /// <summary>
        /// bool
        /// </summary>
        internal const int AllowAutoFit = 4240;

        /// <summary>
        /// Preferred width before the first cell of a table row.
        /// </summary>
        /// <remarks>
        /// The attribute is used in jagged tables to specify the width of an empty space before the first cell in a row.
        /// This can be interpreted as a preferred width of the missing cells on the left of a row.
        /// As the cells are missing, this is where the preferred width is stored.
        /// The width is preferred in a sense that it may not match the actual width of the missing columns (though it often does).
        /// The value specified in the document must be used when re-calculating table layout.
        /// The value specified in the document can be replaced as a result of table layout re-calculation.
        /// It normally happens with missing values though, or zero values might be removed.
        /// </remarks>
        internal const int WidthBefore = 4250;

        /// <summary>
        /// This attribute is used to access the original "width before" value stored in the document.
        /// </summary>
        /// <remarks>
        /// It is needed for the "new" table grid algorithm implemented by <see cref="TableGridConstructor"/> to work.
        /// NrxTableReader uses some naive technique to set the attribute value from tblGrid column widths specified in the document.
        /// As tblGrid values might be incorrect, the assigned value may prevent correct table layout re-calculation.
        /// So the "new" algorithm uses this attribute to access the original value stored in the document.
        /// Eventually, when all cases are handled by the "new" algorithm, there will be no need to have two attributes.
        /// However, even though right now the "new" algorithm is capable of handling most of the tables,
        /// for auto-fit tables it is only invoked when document layout is constructed,
        /// which does not happen in some popular scenarios, like conversion between flow formats.
        /// </remarks>
        internal const int WidthBeforeOriginal = 4251;

        /// <summary>
        /// Preferred width after the last cell of a table row.
        /// </summary>
        /// <remarks>
        /// The attribute is used in jagged tables to specify the width of an empty space after the last cell in a row.
        /// This can be interpreted as a preferred width of the missing cells on the right of a row.
        /// As the cells are missing, this is where the preferred width is stored.
        /// The width is preferred in a sense that it may not match the actual width of the missing columns (though it often does).
        /// The value specified in the document must be used when re-calculating table layout.
        /// The value specified in the document can be replaced as a result of table layout re-calculation.
        /// It normally happens with missing values though, or zero values might be removed.
        /// </remarks>
        internal const int WidthAfter = 4260;

        /// <summary>
        /// This attribute is used to access the original "width after" value stored in the document.
        /// </summary>
        /// <remarks>
        /// The attribute is used to preserve the original value from the document
        /// in scenarios when the new table layout logic is not applied.
        /// </remarks>
        internal const int WidthAfterOriginal = 4261;

        /// <summary>
        /// int. Text frame.
        /// </summary>
        internal const int FrameDistanceFromRight = 4270;
        /// <summary>
        /// int. Text frame.
        /// </summary>
        internal const int FrameDistanceFromBottom = 4280;

        /// <summary>
        /// Specifies table cell spacing (the spacing between adjacent cells and the edges of the table),
        /// <see cref="PreferredWidth"/>.
        /// </summary>
        /// <remarks>
        /// In MS Word when zero cell spacing is specified, the table appears differently
        /// from the case where cell spacing is not specified at all. Looks like MS Word glitch.
        /// <see cref="Table.AllowCellSpacing"/> controls the presence or missing of the CellSpacing attribute.
        /// </remarks>
        internal const int CellSpacing = 4290;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int TopPadding = 4300;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int BottomPadding = 4310;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int RightPadding = 4320;
        /// <summary>
        /// Shading
        /// </summary>
        internal const int Shading = 4330;
        /// <summary>
        /// int, twips.
        /// Left indent of the table.
        /// </summary>
        internal const int LeftIndent = 4340;
        /// <summary>
        /// bool, Text frame.
        /// </summary>
        internal const int AllowOverlap = 4350;
        /// <summary>
        /// bool.
        /// </summary>
        internal const int AllowBreakAcrossPages = 4360;
        /// <summary>
        /// bool
        /// </summary>
        internal const int Bidi = 4380;
        /// <summary>
        /// int
        /// </summary>
        internal const int RsidTr = 4400;

        /// <summary>
        /// int. Stored in table attributes of a table style.
        /// Default 0. When 0, there is no banding. Valid range in MS Word 0..3, we should enforce too.
        /// </summary>
        internal const int StyleRowBandSize = 4500;
        /// <summary>
        /// int. Stored in table attributes of a table style.
        /// Default 0. When 0, there is no banding. Valid range in MS Word 0..3, we should enforce too.
        /// </summary>
        internal const int StyleColBandSize = 4510;

        internal const int Hidden = 4520;

        /// <summary>
        /// Docx Iso29500 only attribute. It is not applicable to WML, RTF and DOC, so the number is not for writing,
        /// but a unique ID.
        /// </summary>
        internal const int Title = 5000;

        /// <summary>
        /// Docx Iso29500 only attribute. It is not applicable to WML, RTF and DOC, so the number is not for writing,
        /// but a unique ID.
        /// </summary>
        internal const int Description = 5010;

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// HTML related information <see cref="HtmlBlock" /> is applied to this row.
        /// </summary>
        internal const int HtmlBlockId = 5015;

        /// <summary>
        /// HTML block <see cref="HtmlBlock" /> left margin.
        /// </summary>
        internal const int HtmlMarginLeft = 5016;

        /// <summary>
        /// HTML block <see cref="HtmlBlock" /> right margin.
        /// </summary>
        internal const int HtmlMarginRight = 5017;

        /// <summary>
        /// HTML block <see cref="HtmlBlock" /> top margin.
        /// </summary>
        internal const int HtmlMarginTop = 5018;

        /// <summary>
        /// HTML block <see cref="HtmlBlock" /> bottom margin.
        /// </summary>
        internal const int HtmlMarginBottom = 5019;

        /// <summary>
        /// CellPrCollection, note this is stored only temporarily during read and write.
        /// </summary>
        internal const int Sys_Cells = 5100;

        /// <summary>
        /// Do not use anywhere except DOC import/export.
        /// Used to temporarily store a non bidi aware Word 97 attribute that is resolved into a model attribute.
        /// </summary>
        internal const int Sys_Alignment97 = 5101;

        /// <summary>
        /// Do not use anywhere except DOC import/export.
        /// Used to temporarily store first cell position in row when old table definition is read.
        /// Removed after table converted to new style.
        /// </summary>
        internal const int Sys_LeftIndent97 = 5102;

        /// <summary>
        /// Do not use anywhere except DOCX/WML import/export.
        /// Specifies table grid for table.
        /// </summary>
        internal const int Sys_TableGrid = 5103;

        /// <summary>
        /// Do not use anywhere except DOCX/WML import/export.
        /// Specifies grid before for row.
        /// </summary>
        internal const int Sys_GridBefore = 5104;

        /// <summary>
        /// Do not use anywhere except DOCX/WML import/export.
        /// Specifies grid after for row.
        /// </summary>
        internal const int Sys_GridAfter = 5105;

        /// <summary>
        /// Stores the table grid calculated by the new algorithm.
        /// </summary>
        /// <remarks>
        /// The new algorithm currently does not support all possible cases.
        /// State property is set to Applied if the grid was calculated successfully.
        /// </remarks>
        internal const int Sys_CalculatedTableGrid = 5106;

        /// <summary>
        /// Stores the table grid originally read from document.
        /// </summary>
        /// <remarks>
        /// It was introduced by the new table grid calculation algorithm to co-exist with the old algorithm.
        /// Currently, a grid missing in the document may be created, or an existing grid may be updated by the old algorithm.
        /// This logic may break the new algorithm which needs the values as they are stored in the  document,
        /// before any changes are applied or missing values are added.
        /// So this attribute just stores tblGrid read from document before it is changed in any way.
        /// Eventually, when all cases are handled by the "new" algorithm, there will be no need to have two attributes.
        /// </remarks>
        internal const int Sys_TableGridForNewAlgorithm = 5107;

        /// <summary>
        /// Combined with Sys_DxaGapHalf, specifies the location of the horizontal origin of the table relative to the logical left margin.
        /// </summary>
        internal const int Sys_DxaLeft = 5108;

        /// <summary>
        /// Average width, in twips, between the left and right default cell margins for the first cell in the row.
        /// </summary>
        internal const int Sys_DxaGapHalf = 5109;

        /// <summary>
        /// The attribute is used to remember the leftmost row indent for doc and rtf,
        /// as it may be lost when readers reformat the table.
        /// </summary>
        internal const int Sys_LeftIndent97Base = 5110;

        /// <summary>
        /// Do not use anywhere except DOC import/export.
        /// Used to temporarily store a Word 97 bidi table attribute that is resolved into a model attribute.
        /// </summary>
        internal const int Sys_BidiTable97 = 5111;

        /// <summary>
        /// Indicates if the logic that uses older Word 97 attributes was used to compute 'width before' value for doc/rtf.
        /// </summary>
        internal const int Sys_Word97Logic = 5112;
        // ReSharper restore InconsistentNaming
    }
}
