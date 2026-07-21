// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/02/2013 by Victor Chebotok

using Aspose.Words.Lists;

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlOLElementDef : HtmlElementDef
    {
        internal HtmlOLElementDef(bool applyFormattingAsMsWord)
        {
            mApplyFormattingAsMsWord = applyFormattingAsMsWord;
        }

        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.Block));

            // We use MS Word-specific list indent in our user agent.
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("-aw-padding-start", new CssLengthValue(ListLevel.LeftIndent, CssUnit.Pt)));

            if (mApplyFormattingAsMsWord)
            {
                return;
            }

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.Decimal));

            // Presentational hints.

            // "type" attribute
            switch (element.GetAttributeValue("type"))
            {
                case "1":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.Decimal));
                    break;
                case "a":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.LowerAlpha));
                    break;
                case "A":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.UpperAlpha));
                    break;
                case "i":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.LowerRoman));
                    break;
                case "I":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.UpperRoman));
                    break;
                default:
                    // Other 'type' attribute values are ignored.
                    break;
            }
        }

        private readonly bool mApplyFormattingAsMsWord;
    }
}
