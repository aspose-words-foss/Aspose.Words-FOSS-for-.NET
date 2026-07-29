// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2013 by Victor Chebotok

using Aspose.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlAElementDef : HtmlElementDef
    {
        internal HtmlAElementDef(bool applyFormattingAsMsWord)
        {
            mDefaultColor = applyFormattingAsMsWord
                ? DrColor.Blue
                : DefaultColorHtml;
        }

        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.

            // If the href attribute is not present, the <a> tag is not a hyperlink.
            if (element.GetAttributeValue("href") == null)
                return;

            // WORDSNET-27579 These declarations may or may not be overridable depending on the element's position
            // in the HTML tree. They may be removed at a later stage where the element's position is known.
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("color", CssHashValue.FromColor(mDefaultColor)));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("text-decoration",
                                                                     new CssTextDecorationPropertyValue(CssValue.Underline)));
        }

        internal static readonly DrColor DefaultColorHtml = new DrColor(0, 0, 0xEE);

        private readonly DrColor mDefaultColor;
    }
}
