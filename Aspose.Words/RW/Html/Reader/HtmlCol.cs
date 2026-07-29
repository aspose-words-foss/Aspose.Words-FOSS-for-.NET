// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/03/2014 by Alexey Butalov

using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Represents HTML 'col' element.
    /// </summary>
    internal class HtmlCol
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="declarations">Element CSS declarations.</param>
        /// <param name="colGroup">HTML col group.</param>
        internal HtmlCol(CssDeclarationCollection declarations, HtmlColGroup colGroup)
        {
            Debug.Assert(declarations != null);
            Debug.Assert(colGroup != null);
            mDeclarations = declarations;
            mColGroup = colGroup;
            mCssBorders = CssBoxBorders.CreateBorders(declarations, false);
        }

        /// <summary>
        /// Gets CSS declarations of the element.
        /// </summary>
        internal CssDeclarationCollection Declarations
        {
            get { return mDeclarations; }
        }

        /// <summary>
        /// CSS borders declared in the element.
        /// </summary>
        internal CssBoxBorders CssBorders
        {
            get { return mCssBorders; }
        }

        /// <summary>
        /// HTML col group of the element.
        /// </summary>
        internal HtmlColGroup ColGroup
        {
            get { return mColGroup; }
        }

        private readonly CssBoxBorders mCssBorders;
        private readonly CssDeclarationCollection mDeclarations;
        private readonly HtmlColGroup mColGroup;
    }
}
