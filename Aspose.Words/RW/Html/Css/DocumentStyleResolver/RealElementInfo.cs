// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/12/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to store information about a real HTML element (not a pseudo-element) in <see cref="DocumentFormatter"/>.
    /// Stores intermediate values used for performance optimization.
    /// </summary>
    internal class RealElementInfo : HtmlElementInfo
    {
        internal RealElementInfo(
            IHtmlElementProvider element,
            CssDeclarationCollection declarations,
            CssDeclarationCollection beforePseudoElementDeclarations,
            CssDeclarationCollection afterPseudoElementDeclarations)
            : base(element, declarations)
        {
            Debug.Assert(beforePseudoElementDeclarations != null);
            beforePseudoElementDeclarations.DebugCheckAllComputed();
            Debug.Assert(afterPseudoElementDeclarations != null);
            afterPseudoElementDeclarations.DebugCheckAllComputed();

            mBeforePseudoElementDeclarations = beforePseudoElementDeclarations;
            mAfterPseudoElementDeclarations = afterPseudoElementDeclarations;
        }

        internal override HtmlElementPart Part
        {
            get { return HtmlElementPart.Element; }
        }

        internal override PseudoElementContent GeneratedContent
        {
            get { return null; }
        }

        internal override CssDeclarationCollection BeforePseudoElementDeclarations
        {
            get { return mBeforePseudoElementDeclarations; }
        }

        internal override CssDeclarationCollection AfterPseudoElementDeclarations
        {
            get { return mAfterPseudoElementDeclarations; }
        }

        private readonly CssDeclarationCollection mBeforePseudoElementDeclarations;

        private readonly CssDeclarationCollection mAfterPseudoElementDeclarations;
    }
}
