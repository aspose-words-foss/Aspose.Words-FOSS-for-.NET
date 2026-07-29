// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/02/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlCaptionElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.TableCaption));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("text-align", CssValue.Center));

            // Presentational hints.
            switch (element.GetAttributeValue("align", string.Empty).ToLowerInvariant())
            {
                case "top":
                case "left":
                case "right":
                    // Modern browsers accept "left" and "right" values, but treat them as "top": the caption is placed
                    // before the table, and the caption contents are centered.
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("caption-side", CssValue.Top));
                    break;
                case "bottom":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("caption-side", CssValue.Bottom));
                    break;
                default:
                    // Other 'align' attribute values are ignored.
                    break;
            }
        }
    }
}