// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/03/2013 by Victor Chebotok

using System;

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlColgroupElementDef : HtmlElementDef
    {
        protected override bool CanBeHidden()
        {
            return false;
        }

        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.TableColumnGroup));
            if (element.GetAttributeValue("hidden") != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("visibility", CssValue.Collapse));
            }

            // Presentational hints.

            // Parent TABLE's "rules" attribute
            IElementProvider parent = element.GetParentElement();
            if ((parent != null) && (parent.ElementName == "table"))
            {
                if (string.Equals(parent.GetAttributeValue("rules", string.Empty), "groups", StringComparison.OrdinalIgnoreCase))
                {
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-width", CssValue.OnePx));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-width", CssValue.OnePx));

                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Solid));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Solid));
                }
            }
        }
    }
}
