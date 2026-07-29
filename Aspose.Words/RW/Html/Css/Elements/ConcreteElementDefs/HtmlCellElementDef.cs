// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/03/2013 by Victor Chebotok

using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlCellElementDef : HtmlElementDef
    {
        internal HtmlCellElementDef(CssDocumentMode documentMode)
        {
            mDocumentMode = documentMode;
        }

        protected override bool CanBeHidden()
        {
            return false;
        }

        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            cssDeclarations.AddOrReplace(gDefaultStyle);

            if (element.GetAttributeValue("hidden") != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("visibility", CssValue.Collapse));
            }

            if (element.ElementName == "th")
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-weight", CssValue.Bold));
            }

            // Presentational hints.

            // The "background" attribute is not supported.
            ProcessParentTableAttributes(element, cssDeclarations);
            ProcessCellAttributes(element, cssDeclarations);
        }

        private static readonly CssValue gGrayColor = CssValue.Gray;

        private static readonly CssDeclarationCollection gDefaultStyle = new CssDeclarationCollection(
            new CssSpecifiedDeclaration("display", CssValue.TableCell),

            // WORDSNET-14473 Although the HTML 5 specification requires that by default table cells (td and th) be gray 
            // (see https://www.w3.org/TR/html/rendering.html#tables), modern browsers don't apply this user agent style.
            // These styles removed from here.

            new CssSpecifiedDeclaration("padding-top", CssValue.OnePx),
            new CssSpecifiedDeclaration("padding-right", CssValue.OnePx),
            new CssSpecifiedDeclaration("padding-bottom", CssValue.OnePx),
            new CssSpecifiedDeclaration("padding-left", CssValue.OnePx),

            new CssSpecifiedDeclaration("vertical-align", CssValue.Inherit));

        /// <summary>
        /// Processes parent TABLE's attributes.
        /// </summary>
        private static void ProcessParentTableAttributes(
            IHtmlElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations)
        {
            IElementProvider parentTable = GetParentTableElement(element);
            if (parentTable == null)
                return;

            // "border" attribute
            string borderValue = parentTable.GetAttributeValue("border");
            if ((borderValue != null) && (HtmlUtil.ParseNonNegativeInteger(borderValue) != 0))
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-width", CssValue.OnePx));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-width", CssValue.OnePx));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-width", CssValue.OnePx));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-width", CssValue.OnePx));

                // WORDSNET-14473 Table with "border" attribute has gray border color by default.
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-color", gGrayColor));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-color", gGrayColor));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-color", gGrayColor));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-color", gGrayColor));

                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Inset));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Inset));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Inset));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Inset));
            }

            // "rules" attribute
            string rulesValue = parentTable.GetAttributeValue("rules", string.Empty).ToLowerInvariant();
            switch (rulesValue)
            {
                case "none":
                case "groups":
                case "rows":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-width", CssValue.OnePx));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-width", CssValue.OnePx));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-width", CssValue.OnePx));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-width", CssValue.OnePx));

                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.None));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.None));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.None));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.None));
                    break;
                case "cols":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-width", CssValue.OnePx));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-width", CssValue.OnePx));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-width", CssValue.OnePx));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-width", CssValue.OnePx));

                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.None));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Solid));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.None));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Solid));
                    break;
                case "all":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-width", CssValue.OnePx));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-width", CssValue.OnePx));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-width", CssValue.OnePx));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-width", CssValue.OnePx));

                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Solid));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Solid));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Solid));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Solid));
                    break;
                default:
                    // Other 'rules' attribute values are ignored.
                    break;
            }

            // "bordercolor" attribute
            CssValue bordercolorValue = CssValue.ParseLegacyColor(parentTable.GetAttributeValue("bordercolor"));

            if ((rulesValue == "none") ||
                (rulesValue == "groups") ||
                (rulesValue == "rows") ||
                (rulesValue == "cols") ||
                (rulesValue == "all"))
            {
                if (bordercolorValue == null)
                    bordercolorValue = CssValue.Black;

                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-color", bordercolorValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-color", bordercolorValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-color", bordercolorValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-color", bordercolorValue));

            }
            else if (bordercolorValue != null)
            {
                // WORDSNET-27791 Although the CSS standard doesn't state this, tests show that table cells inherit
                // 'border-xxx-color' properties from their parent elements in Google Chrome.
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-color", CssValue.Inherit));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-color", CssValue.Inherit));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-color", CssValue.Inherit));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-color", CssValue.Inherit));
            }

            // "cellpadding" attribute
            CssValue cellpaddingValue = CssValue.ParseLegacyPixelLength(parentTable.GetAttributeValue("cellpadding"));
            if (cellpaddingValue != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("padding-top", cellpaddingValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("padding-right", cellpaddingValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("padding-bottom", cellpaddingValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("padding-left", cellpaddingValue));
            }

            // "nowrap" attribute
            if (element.GetAttributeValue("nowrap") != null)
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("white-space", CssValue.Nowrap));
        }

        /// <summary>
        /// Finds the parent TABLE element.
        /// </summary>
        private static IElementProvider GetParentTableElement(IElementProvider element)
        {
            IElementProvider parent = element.GetParentElement();
            if ((parent != null) && (parent.ElementName == "tr"))
            {
                parent = parent.GetParentElement();
                if ((parent != null) && ((parent.ElementName == "thead") || (parent.ElementName == "tbody") || (parent.ElementName == "tfoot")))
                {
                    parent = parent.GetParentElement();
                }
            }

            return ((parent != null) && (parent.ElementName == "table"))
                       ? parent
                       : null;
        }

        private void ProcessCellAttributes(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            TranslateVAlignAttributeToCss(element, cssDeclarations);
            TranslateBGColorAttributeToCss(element, cssDeclarations);
            TranslateAlignAttributeToCss(element, cssDeclarations, true);
            TranslateHeightAttributeToCss(element, cssDeclarations);

            // "width" attribute
            CssValue widthValue = CssValue.ParseLegacyDimension(element.GetAttributeValue("width"));
            if (widthValue != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("width", widthValue));
            }

            // "nowrap" attribute
            if (element.GetAttributeValue("nowrap") != null)
            {
                if ((mDocumentMode == CssDocumentMode.Quirks) && (widthValue != null) &&
                    (widthValue.ValueType == CssValueType.Length))
                {
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("white-space", CssValue.Normal));
                }
                else
                {
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("white-space", CssValue.Nowrap));
                }
            }
        }

        private readonly CssDocumentMode mDocumentMode;
    }
}
