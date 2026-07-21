// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlEmbedElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // Presentational hints.

            // "align" attribute
            switch (element.GetAttributeValue("align", string.Empty).ToLowerInvariant())
            {
                case "left":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("float", CssValue.Left));
                    break;
                case "right":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("float", CssValue.Right));
                    break;
                case "top":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("vertical-align", CssValue.Top));
                    break;
                case "baseline":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("vertical-align", CssValue.Baseline));
                    break;
                case "texttop":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("vertical-align", CssValue.TextTop));
                    break;
                // "center" and "middle" values should be mapped to special CSS property values
                // that align IFRAME's vertical middle with the parent element's baseline.
                // Consider implementing that special CSS value (for example, "-aw-baseline-middle"),
                // if the CSS "middle" value causes trouble.
                case "center":
                case "middle":
                case "absmiddle":
                case "abscenter":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("vertical-align", CssValue.Middle));
                    break;
                case "bottom":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("vertical-align", CssValue.Bottom));
                    break;
                default:
                    // Other 'align' attribute values are ignored.
                    break;
            }

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
