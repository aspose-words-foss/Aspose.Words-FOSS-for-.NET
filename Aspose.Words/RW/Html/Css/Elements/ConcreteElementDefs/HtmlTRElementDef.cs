// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2013 by Victor Chebotok

using System;

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlTRElementDef : HtmlElementDef
    {
        protected override bool CanBeHidden()
        {
            return false;
        }

        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.TableRow));

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-color", CssValue.Inherit));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-color", CssValue.Inherit));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-color", CssValue.Inherit));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-color", CssValue.Inherit));

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("vertical-align", CssValue.Inherit));

            if (element.GetAttributeValue("hidden") != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("visibility", CssValue.Collapse));
            }

            // Presentational hints.

            // The "background" attribute is not supported.

            TranslateAlignAttributeToCss(element, cssDeclarations, true);
            TranslateVAlignAttributeToCss(element, cssDeclarations);

            // "height" attribute
            // Read HTML row height. Normally, there is no such attribute, but we do write it
            // (like Microsoft Word) for last row to support misaligned columns.
            string heightStr = element.GetAttributeValue("height");
            if (StringUtil.HasChars(heightStr))
            {
                CssValue heightValue = CssValue.ParseLegacyDimension(heightStr);
                if (heightValue == null)
                    heightValue = CssValue.ParseLegacyPixelLength(heightStr);
                if (heightValue != null)
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("height", heightValue));
            }

            TranslateBGColorAttributeToCss(element, cssDeclarations);

            #region Parent TABLE's attributes

            // Find the parent TABLE element.
            IElementProvider parent = element.GetParentElement();
            if ((parent != null) && ((parent.ElementName == "thead") || (parent.ElementName == "tbody") || (parent.ElementName == "tfoot")))
            {
                parent = parent.GetParentElement();
            }

            // Process parent TABLE's attributes.
            if ((parent != null) && (parent.ElementName == "table"))
            {
                // "rules" attribute
                if (string.Equals(parent.GetAttributeValue("rules", string.Empty), "rows", StringComparison.OrdinalIgnoreCase))
                {
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-width", CssValue.OnePx));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-width", CssValue.OnePx));

                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Solid));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Solid));
                }
            }

            #endregion
        }
    }
}
