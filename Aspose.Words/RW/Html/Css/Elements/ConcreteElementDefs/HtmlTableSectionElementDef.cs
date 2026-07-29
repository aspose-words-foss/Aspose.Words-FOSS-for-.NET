// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/03/2013 by Victor Chebotok

using System;

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlTableSectionElementDef : HtmlElementDef
    {
        protected override bool CanBeHidden()
        {
            return false;
        }

        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("vertical-align", CssValue.Middle));

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-color", CssValue.Inherit));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-color", CssValue.Inherit));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-color", CssValue.Inherit));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-color", CssValue.Inherit));

            switch (element.ElementName)
            {
                case "thead":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.TableHeaderGroup));
                    break;
                case "tbody":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.TableRowGroup));
                    break;
                case "tfoot":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.TableFooterGroup));
                    break;
                default:
                    Debug.Assert(false, "Unknown table's section.");
                    break;
            }

            if (element.GetAttributeValue("hidden") != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("visibility", CssValue.Collapse));
            }

            // Presentational hints.

            // The "background" attribute is not supported.

            TranslateAlignAttributeToCss(element, cssDeclarations, true);
            TranslateVAlignAttributeToCss(element, cssDeclarations);
            TranslateBGColorAttributeToCss(element, cssDeclarations);

            // Parent TABLE's "rules" attribute
            IElementProvider parent = element.GetParentElement();
            if ((parent != null) && (parent.ElementName == "table"))
            {
                if (string.Equals(parent.GetAttributeValue("rules", string.Empty), "groups",
                    StringComparison.OrdinalIgnoreCase))
                {
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-width", CssValue.OnePx));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-width", CssValue.OnePx));

                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Solid));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Solid));
                }
            }
        }
    }
}
