// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlH4ElementDef : HtmlElementDef
    {
        internal HtmlH4ElementDef(bool applyFormattingAsMsWord)
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
            CssValue fontSize = mApplyFormattingAsMsWord
                ? new CssLengthValue(12, CssUnit.Pt)
                : new CssLengthValue(1, CssUnit.Em);
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-size", fontSize));

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-weight", CssValue.Bold));
        }

        private readonly bool mApplyFormattingAsMsWord;
    }
}
