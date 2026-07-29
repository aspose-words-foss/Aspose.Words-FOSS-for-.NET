// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/03/2013 by Victor Chebotok

using System;

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlTextareaElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-family",
                new CssFontFamilyPropertyValue(CssValue.Monospace)));

            // Presentational hints.

            // The "cols" and "rows" attributes are not supported, as their processing requires information
            // about the font size of the element.

            if (string.Equals(element.GetAttributeValue("wrap", string.Empty), "off", StringComparison.OrdinalIgnoreCase))
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("white-space", CssValue.Pre));
            }
        }
    }
}
