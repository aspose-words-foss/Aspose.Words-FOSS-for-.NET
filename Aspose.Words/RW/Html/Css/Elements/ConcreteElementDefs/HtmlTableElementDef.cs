// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/03/2013 by Victor Chebotok

using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlTableElementDef : HtmlElementDef
    {
        internal HtmlTableElementDef(CssDocumentMode documentMode)
        {
            mDocumentMode = documentMode;
        }

        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            cssDeclarations.AddOrReplace(gDefaultStyle);

            // Presentational hints.

            // The "background" attribute is not supported.

            // "align" attribute
            switch (element.GetAttributeValue("align", string.Empty).ToLowerInvariant())
            {
                case "left":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("float", CssValue.Left));
                    break;
                case "right":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("float", CssValue.Right));
                    break;
                case "center":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-left", CssValue.Auto));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-right", CssValue.Auto));
                    break;
                default:
                    // Other 'align' attribute values are ignored.
                    break;
            }

            // "rules" attribute
            switch (element.GetAttributeValue("rules", string.Empty).ToLowerInvariant())
            {
                case "none":
                case "groups":
                case "rows":
                case "cols":
                case "all":
                {
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-collapse", CssValue.Collapse));

                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-color", CssValue.Black));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-color", CssValue.Black));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-color", CssValue.Black));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-color", CssValue.Black));
                    break;
                }
                default:
                    // Other 'rules' attribute values are ignored.
                    break;
            }

            // "border" attribute
            string borderValue = element.GetAttributeValue("border");
            if ((borderValue != null) && (HtmlUtil.ParseNonNegativeInteger(borderValue) != 0))
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Outset));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Outset));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Outset));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Outset));
            }
            if (borderValue != null)
            {
                CssValue borderWidthValue = CssValue.ParseLegacyPixelLength(borderValue);
                if (borderWidthValue == null)
                    borderWidthValue = CssValue.OnePx;

                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-width", borderWidthValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-width", borderWidthValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-width", borderWidthValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-width", borderWidthValue));
            }

            // "frame" attribute
            switch (element.GetAttributeValue("frame", string.Empty).ToLowerInvariant())
            {
                case "void":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Hidden));
                    break;
                case "above":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Outset));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Hidden));
                    break;
                case "below":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Outset));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Hidden));
                    break;
                case "hsides":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Outset));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Outset));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Hidden));
                    break;
                case "lhs":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Outset));
                    break;
                case "rhs":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Outset));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Hidden));
                    break;
                case "vsides":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Outset));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Hidden));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Outset));
                    break;
                case "box":
                case "border":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Outset));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Outset));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Outset));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Outset));
                    break;
                default:
                    // Other 'frame' attribute values are ignored.
                    break;
            }
            switch (element.GetAttributeValue("frame", string.Empty).ToLowerInvariant())
            {
                case "void":
                case "above":
                case "below":
                case "hsides":
                case "lhs":
                case "rhs":
                case "vsides":
                case "box":
                case "border":
                {
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-color", CssValue.Black));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-color", CssValue.Black));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-color", CssValue.Black));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-color", CssValue.Black));
                    break;
                }
                default:
                    // Other 'frame' attribute values are ignored.
                    break;
            }

            // "cellpadding" attribute
            CssValue cellpaddingValue = CssValue.ParseLegacyPixelLength(element.GetAttributeValue("cellpadding"));
            if (cellpaddingValue != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("padding-top", cellpaddingValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("padding-right", cellpaddingValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("padding-bottom", cellpaddingValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("padding-left", cellpaddingValue));
            }

            // "cellspacing" attribute
            CssValue cellspacingValue = CssValue.ParseLegacyPixelLength(element.GetAttributeValue("cellspacing"));
            if (cellspacingValue != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-spacing", cellspacingValue));
            }

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

            TranslateHeightAttributeToCss(element, cssDeclarations);
            TranslateWidthAttributeToCss(element, cssDeclarations);
            TranslateBGColorAttributeToCss(element, cssDeclarations);

            // "bordercolor" attribute
            CssValue bordercolorValue = CssValue.ParseLegacyColor(element.GetAttributeValue("bordercolor"));
            if (bordercolorValue != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-color", bordercolorValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-color", bordercolorValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-color", bordercolorValue));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-color", bordercolorValue));
            }

            if (mDocumentMode == CssDocumentMode.Quirks)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("line-height", CssValue.Initial));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("white-space", CssValue.Initial));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("text-align", CssValue.Initial));
            }
        }

        protected override void ApplyOverridableFontStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            if (mDocumentMode == CssDocumentMode.Quirks)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-weight", CssValue.Initial));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-style", CssValue.Initial));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-variant", CssValue.Initial));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-size", CssValue.Initial));
            }
        }

        private readonly CssDocumentMode mDocumentMode;


        private static readonly CssDeclarationCollection gDefaultStyle = new CssDeclarationCollection(
            new CssSpecifiedDeclaration("padding-top", CssValue.OnePx),
            new CssSpecifiedDeclaration("padding-right", CssValue.OnePx),
            new CssSpecifiedDeclaration("padding-bottom", CssValue.OnePx),
            new CssSpecifiedDeclaration("padding-left", CssValue.OnePx),

            new CssSpecifiedDeclaration("display", CssValue.Table),
            new CssSpecifiedDeclaration("border-spacing", new CssLengthValue(2, CssUnit.Px)),
            new CssSpecifiedDeclaration("border-collapse", CssValue.Separate),

            new CssSpecifiedDeclaration("border-top-color", CssValue.Gray),
            new CssSpecifiedDeclaration("border-right-color", CssValue.Gray),
            new CssSpecifiedDeclaration("border-bottom-color", CssValue.Gray),
            new CssSpecifiedDeclaration("border-left-color", CssValue.Gray));
    }
}
