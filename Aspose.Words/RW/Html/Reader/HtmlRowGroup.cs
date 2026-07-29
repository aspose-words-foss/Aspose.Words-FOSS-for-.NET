// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/03/2014 by Alexey Butalov

using Aspose.Words.RW.Html.Css;
using Aspose.Words.RW.Html.Parser;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Represents HTML row group element ('thead', 'tbody' or 'tfoot').
    /// </summary>
    internal class HtmlRowGroup
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="declarations">Element CSS declarations.</param>
        internal HtmlRowGroup(HtmlElementNode node, CssDeclarationCollection declarations)
        {
            Debug.Assert(node != null);
            Debug.Assert(declarations != null);
            mNode = node;
            mCssBorders = CssBoxBorders.CreateBorders(declarations, false);
            mDeclarations = declarations;
        }

        internal HtmlElementNode Node
        {
            get { return mNode; }
        }

        /// <summary>
        /// CSS borders declared in the element.
        /// </summary>
        internal CssBoxBorders CssBorders
        {
            get { return mCssBorders; }
        }

        internal bool IsThead
        {
            get { return mNode.Name == "thead"; }
        }

        /// <summary>
        /// CSS declarations of the table row group.
        /// </summary>
        public CssDeclarationCollection Declarations
        {
            get { return mDeclarations; }
        }

        private readonly CssBoxBorders mCssBorders;
        private readonly HtmlElementNode mNode;
        private readonly CssDeclarationCollection mDeclarations;
    }
}
