// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlBRElementDef : HtmlElementDef
    {
        internal HtmlBRElementDef(bool applyFormattingAsMsWord)
        {
            mApplyFormattingAsMsWord = applyFormattingAsMsWord;
        }

        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // Presentational hints.

            // "clear" attribute
            // MS Word processes the "clear" attribute and the "clear" CSS property in a non-standard way. In order to mimic
            // this behavior, we process these values at a higher level.
            if (!mApplyFormattingAsMsWord)
            {
                switch (element.GetAttributeValue("clear", string.Empty).ToLowerInvariant())
                {
                    case "left":
                        cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("clear", CssValue.Left));
                        break;
                    case "right":
                        cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("clear", CssValue.Right));
                        break;
                    case "all":
                    case "both":
                        cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("clear", CssValue.Both));
                        break;
                    default:
                        // Other 'clear' attribute values are ignored.
                        break;
                }
            }
        }

        private readonly bool mApplyFormattingAsMsWord;
    }
}
