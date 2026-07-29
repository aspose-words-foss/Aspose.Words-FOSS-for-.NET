// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlSupElementDef : HtmlElementDef
    {
        internal HtmlSupElementDef(bool applyFormattingAsMsWord)
        {
            mApplyFormattingAsMsWord = applyFormattingAsMsWord;
        }

        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("vertical-align", CssValue.Super));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("line-height", CssValue.Normal));
        }

        protected override void ApplyOverridableFontStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            if (!mApplyFormattingAsMsWord)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-size", CssValue.Smaller));
            }
        }

        private readonly bool mApplyFormattingAsMsWord;
    }
}
