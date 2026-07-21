// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/05/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Base class for all CSS pseudo-class selectors.
    /// </summary>
    internal abstract class CssPseudoClassSelector : CssSimpleSelector
    {
        internal override CssSelectorSpecificity Specificity
        {
            get { return gSpecificity; }
        }

        private static readonly CssSelectorSpecificity gSpecificity = new CssSelectorSpecificity(0, 1, 0);
    }
}