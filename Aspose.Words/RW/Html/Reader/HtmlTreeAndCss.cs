// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/02/2023 by Artem Shabarshin

using System.Collections.Generic;
using Aspose.Words.RW.Html.Css;
using Aspose.Words.RW.Html.Parser;

namespace Aspose.Words.RW.Html.Reader
{
    internal struct HtmlTreeAndCss
    {
        internal HtmlDocument Html { get; set; }

        internal IList<CssStyleRule> StyleRules { get; set; }

        internal IList<CssPageRule> PageRules { get; set; }

        internal IList<CssListRule> ListRules { get; set; }
    }
}
