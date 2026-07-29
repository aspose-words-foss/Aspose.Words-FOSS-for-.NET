// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/02/2013 by Victor Chebotok

using Aspose.Words.Lists;

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlULElementDef : HtmlElementDef
    {
        internal HtmlULElementDef(bool applyFormattingAsMsWord)
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

            int nestingLevel = 0;
            const int maxNestingLevel = 2;
            IElementProvider parent = element.GetParentElement();
            while ((parent != null) && (nestingLevel < maxNestingLevel))
            {
                if ((parent.ElementName == "ol") ||
                    (parent.ElementName == "ul") ||
                    (parent.ElementName == "dir") ||
                    (parent.ElementName == "menu"))
                {
                    ++nestingLevel;
                }
                parent = parent.GetParentElement();
            }

            CssValue listStyleTypeValue;
            switch (nestingLevel)
            {
                case 0:
                    listStyleTypeValue = CssValue.Disc;
                    break;
                case 1:
                    listStyleTypeValue = CssValue.Circle;
                    break;
                default:
                    listStyleTypeValue = CssValue.Square;
                    break;
            }
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", listStyleTypeValue));

            // Presentational hints.

            // "type" attribute
            switch (element.GetAttributeValue("type", string.Empty).ToLowerInvariant())
            {
                case "disc":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.Disc));
                    break;
                case "circle":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.Circle));
                    break;
                case "square":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.Square));
                    break;
                default:
                    // Other 'type' attribute values are ignored.
                    break;
            }
        }

        private readonly bool mApplyFormattingAsMsWord;
    }
}
