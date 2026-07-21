// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/03/2014 by Alexey Butalov

using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Represents HTML 'colgroup' element.
    /// </summary>
    internal class HtmlColGroup
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal HtmlColGroup()
        {
            mCssBorders = CssBoxBorders.CreateEmpty();
            mDeclarations = CssDeclarationCollection.Empty;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="declarations">Element CSS declarations.</param>
        internal HtmlColGroup(CssDeclarationCollection declarations)
        {
            Debug.Assert(declarations != null);
            mDeclarations = declarations;
            mCssBorders = CssBoxBorders.CreateBorders(declarations, false);
        }

        /// <summary>
        /// CSS borders declared in the element.
        /// </summary>
        internal CssBoxBorders CssBorders
        {
            get { return mCssBorders; }
        }

        /// <summary>
        /// Gets CSS declarations of the element.
        /// </summary>
        internal CssDeclarationCollection Declarations
        {
            get { return mDeclarations; }
        }

        private readonly CssBoxBorders mCssBorders;
        private readonly CssDeclarationCollection mDeclarations;
    }
}
