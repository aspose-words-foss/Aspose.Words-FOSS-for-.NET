// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlColElementDef : HtmlElementDef
    {
        protected override bool CanBeHidden()
        {
            return false;
        }

        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.TableColumn));
            if (element.GetAttributeValue("hidden") != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("visibility", CssValue.Collapse));
            }

            // WORDSNET-12020 Support 'width' attribute of 'col' element in HTML import
            CssValue widthValue = CssValue.ParseLegacyDimension(element.GetAttributeValue("width"));
            if (widthValue != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("width", widthValue));
            }
        }
    }
}
