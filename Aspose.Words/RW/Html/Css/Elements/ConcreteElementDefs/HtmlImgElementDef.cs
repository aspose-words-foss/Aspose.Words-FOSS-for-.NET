// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/03/2013 by Victor Chebotok

using System;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlImgElementDef : HtmlElementDef
    {
        internal HtmlImgElementDef(CssDocumentMode documentMode)
        {
            mDocumentMode = documentMode;
        }

        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            if (string.Equals(element.GetAttributeValue("type", string.Empty), "hidden", 
                StringComparison.OrdinalIgnoreCase))
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.None));
            }

            // Presentational hints.

            // "align" attribute
            switch (element.GetAttributeValue("align", string.Empty).ToLowerInvariant())
            {
                case "left":
                {
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("float", CssValue.Left));
                    if (mDocumentMode == CssDocumentMode.Quirks)
                    {
                        CssValue threePx = new CssLengthValue(3, CssUnit.Px);
                        cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-right", threePx));
                    }
                    break;
                }
                case "right":
                {
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("float", CssValue.Right));
                    if (mDocumentMode == CssDocumentMode.Quirks)
                    {
                        CssValue threePx = new CssLengthValue(3, CssUnit.Px);
                        cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-left", threePx));
                    }
                    break;
                }
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
                // that align this element's vertical middle with the parent element's baseline.
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

            // "border" attribute
            // As stated by the HTML 5 specification, the following rules only apply when an element is contained 
            // within a hyperlink. But we ignore this rule because all popular browsers ignore this rule too.
            int borderValue = HtmlUtil.ParseNonNegativeInteger(element.GetAttributeValue("border"));
            if (borderValue > 0)
            {
                CssValue borderWidth = new CssLengthValue(borderValue, CssUnit.Px);
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-width", borderWidth));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-width", borderWidth));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-width", borderWidth));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-width", borderWidth));

                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Solid));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Solid));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Solid));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Solid));
            }
        }

        private readonly CssDocumentMode mDocumentMode;
    }
}
