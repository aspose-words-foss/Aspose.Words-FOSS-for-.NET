// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/11/2004 by Roman Korchagin

using Aspose.JavaAttributes;
using Aspose.Words.RW.Html.Parser;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// I use this interface to allow HtmlTableReader to drive import of HTML tables
    /// and call main HtmlReader when necessary to process content of table cells.
    /// </summary>
    internal interface IHtmlNodeProcessor
    {
        [JavaThrows(true)]
        void ProcessCell(HtmlElementNode cellNode);
    }
}
