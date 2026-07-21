// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlMenuElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.Block));

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

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("-aw-padding-start", new CssLengthValue(40, CssUnit.Px)));
        }
    }
}
