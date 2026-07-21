// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/12/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to store information about a pseudo-element in <see cref="DocumentFormatter"/>.
    /// Stores intermediate values used for performance optimization.
    /// </summary>
    internal class PseudoElementInfo : HtmlElementInfo
    {
        internal PseudoElementInfo(
            IHtmlElementProvider element,
            CssDeclarationCollection declarations,
            HtmlElementPart part,
            PseudoElementContent content)
            : base(element, declarations)
        {
            Debug.Assert(part != HtmlElementPart.Element);
            Debug.Assert(content != null);

            mPart = part;
            mGeneratedContent = content;
        }

        internal override CssDeclarationCollection BeforePseudoElementDeclarations
        {
            get { return CssDeclarationCollection.Empty; }
        }

        internal override CssDeclarationCollection AfterPseudoElementDeclarations
        {
            get { return CssDeclarationCollection.Empty; }
        }

        internal override HtmlElementPart Part
        {
            get { return mPart; }
        }

        internal override PseudoElementContent GeneratedContent
        {
            get { return mGeneratedContent; }
        }

        private readonly HtmlElementPart mPart;

        private readonly PseudoElementContent mGeneratedContent;
    }
}
