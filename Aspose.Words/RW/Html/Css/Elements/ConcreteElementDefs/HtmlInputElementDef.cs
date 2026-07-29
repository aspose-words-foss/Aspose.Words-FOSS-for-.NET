// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/03/2013 by Victor Chebotok

using System;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlInputElementDef : HtmlElementDef
    {
        internal HtmlInputElementDef(CssDocumentMode documentMode)
        {
            mDocumentMode = documentMode;
        }

        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.

            if (string.Equals(element.GetAttributeValue("type", string.Empty), "hidden", StringComparison.OrdinalIgnoreCase))
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.None));
            }


            // Presentational hints.


            // The "size" attribute is not supported at the moment, as its processing requires information
            // about element's font.

            if (string.Equals(element.GetAttributeValue("type", string.Empty), "image",
                StringComparison.OrdinalIgnoreCase))
            {
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
                // within a hyperlink. I hope, checking that parent element is an anchor is enough.
                IElementProvider parent = element.GetParentElement();
                if ((parent != null) && (parent.ElementName == "a"))
                {
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
            }

            if (mDocumentMode == CssDocumentMode.Quirks)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("box-sizing", CssValue.BorderBox));
            }
        }

        protected override void ApplyOverridableFontStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-family",
                new CssFontFamilyPropertyValue(new CssIdentifierValue("sans-serif"))));
        }

        private readonly CssDocumentMode mDocumentMode;
    }
}
