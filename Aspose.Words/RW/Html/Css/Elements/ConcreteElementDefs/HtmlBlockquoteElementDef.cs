// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlBlockquoteElementDef : HtmlElementDef
    {
        internal HtmlBlockquoteElementDef(bool applyFormattingAsMsWord)
        {
            CssDeclarationCollectionBuilder defaultStyleBuilder = new CssDeclarationCollectionBuilder();

            defaultStyleBuilder.Add(new CssSpecifiedDeclaration("display", CssValue.Block));

            if (applyFormattingAsMsWord)
            {
                // MS Word uses the following default margins for `blockquote` elements.

                CssValue horizontalMargin = new CssLengthValue(36, CssUnit.Pt);
                defaultStyleBuilder.Add(new CssSpecifiedDeclaration("margin-left", horizontalMargin));
                defaultStyleBuilder.Add(new CssSpecifiedDeclaration("margin-right", horizontalMargin));

                CssValue verticalMargin = new CssLengthValue(5, CssUnit.Pt);
                defaultStyleBuilder.Add(new CssSpecifiedDeclaration("margin-top", verticalMargin));
                defaultStyleBuilder.Add(new CssSpecifiedDeclaration("margin-bottom", verticalMargin));
            }
            else
            {
                // Default margins as specified by the HTML standard.
                CssValue margin = new CssLengthValue(40, CssUnit.Px);
                defaultStyleBuilder.Add(new CssSpecifiedDeclaration("margin-left", margin));
                defaultStyleBuilder.Add(new CssSpecifiedDeclaration("margin-right", margin));
            }

            mDefaultDeclarations = defaultStyleBuilder.GetDeclarations();
        }

        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            cssDeclarations.AddOrReplace(mDefaultDeclarations);
        }

        private readonly CssDeclarationCollection mDefaultDeclarations;
    }
}
