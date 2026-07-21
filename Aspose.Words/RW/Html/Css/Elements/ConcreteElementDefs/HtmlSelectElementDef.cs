// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/03/2017 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlSelectElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(
            IHtmlElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations)
        {
            // Empty method.
        }

        protected override void ApplyOverridableFontStyles(
            IHtmlElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations)
        {
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-family", new CssFontFamilyPropertyValue(
                new CssIdentifierValue("sans-serif"))));
        }
    }
}
