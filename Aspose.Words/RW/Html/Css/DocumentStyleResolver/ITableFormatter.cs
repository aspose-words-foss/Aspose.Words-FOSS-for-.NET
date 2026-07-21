// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/03/2025 by Nikolay Sezganov

using Aspose.JavaAttributes;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Defines the minimal functionality required for reading HTML and MsoHtml tables.
    /// </summary>
    internal interface ITableFormatter
    {
        [JavaThrows(true)]
        void ToTable(
            Table table,
            int topCaptionCount,
            int bottomCaptionCount,
            CssBorder htmlTableFirstRowFirstCellLeftBorder,
            CssBorder htmlTableFirstRowLastCellRightBorder);
    }
}
