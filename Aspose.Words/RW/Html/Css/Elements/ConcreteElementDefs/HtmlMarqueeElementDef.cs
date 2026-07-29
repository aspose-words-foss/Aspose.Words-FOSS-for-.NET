// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlMarqueeElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // Presentational hints.

            TranslateBGColorAttributeToCss(element, cssDeclarations);
            TranslateHeightAttributeToCss(element, cssDeclarations);
            TranslateWidthAttributeToCss(element, cssDeclarations);

            // "hspace" attribute
            CssValue hspaceValue = CssValue.ParseLegacyDimension(element.GetAttributeValue("hspace"));
            if (hspaceValue != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-left", hspaceValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-right", hspaceValue));
            }

            // "vspace" attribute
            CssValue vspaceValue = CssValue.ParseLegacyDimension(element.GetAttributeValue("vspace"));
            if (vspaceValue != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-top", vspaceValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-bottom", vspaceValue));
            }
        }
    }
}
