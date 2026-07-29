// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlH1ElementDef : HtmlElementDef
    {
        internal HtmlH1ElementDef(bool applyFormattingAsMsWord)
        {
            mApplyFormattingAsMsWord = applyFormattingAsMsWord;
        }

        protected override void ApplyStyles(
            IHtmlElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.Block));

            // Presentational hints.
            TranslateAlignAttributeToCss(element, cssDeclarations, false);
        }

        protected override void ApplyOverridableFontStyles(
            IHtmlElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations)
        {
            CssLengthValue fontSizeValue = mApplyFormattingAsMsWord
                ? new CssLengthValue(24, CssUnit.Pt)
                : GetHtmlFontSize(element);
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-size", fontSizeValue));

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-weight", CssValue.Bold));
        }

        private static CssLengthValue GetHtmlFontSize(IHtmlElementProvider element)
        {
            int nestingLevel = 0;
            const int maxNestingLevel = 5;
            IElementProvider parent = element.GetParentElement();
            while ((parent != null) && (nestingLevel < maxNestingLevel))
            {
                if ((parent.ElementName == "article") ||
                    (parent.ElementName == "aside") ||
                    (parent.ElementName == "nav") ||
                    (parent.ElementName == "section"))
                {
                    ++nestingLevel;
                }
                parent = parent.GetParentElement();
            }

            CssLengthValue fontSizeValue;
            switch (nestingLevel)
            {
                case 0:
                    fontSizeValue = new CssLengthValue(2, CssUnit.Em);
                    break;
                case 1:
                    fontSizeValue = new CssLengthValue(1.5, CssUnit.Em);
                    break;
                case 2:
                    fontSizeValue = new CssLengthValue(1.17, CssUnit.Em);
                    break;
                case 3:
                    fontSizeValue = new CssLengthValue(1, CssUnit.Em);
                    break;
                case 4:
                    fontSizeValue = new CssLengthValue(0.83, CssUnit.Em);
                    break;
                default:
                    fontSizeValue = new CssLengthValue(0.67, CssUnit.Em);
                    break;
            }

            return fontSizeValue;
        }

        private readonly bool mApplyFormattingAsMsWord;
    }
}
