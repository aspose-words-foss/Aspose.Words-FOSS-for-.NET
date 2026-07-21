// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/02/2014 by Alexey Butalov

using Aspose.Words.RW.Html.Css;
using Aspose.Words.RW.Html.Parser;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Represents a table caption during import of an HTML table.
    /// </summary>
    internal class HtmlTableCaption
    {
        /// <summary>
        /// Creates a caption from a 'caption' node.
        /// </summary>
        internal HtmlTableCaption(HtmlElementNode node, CssDeclarationCollection captionDeclarations)
        {
            Debug.Assert(node != null);
            Debug.Assert(captionDeclarations != null);
            mNode = node;
            CssDeclaration captionSideDeclaration = captionDeclarations["caption-side"];
            mIsTopCaption = (captionSideDeclaration == null) || (!captionSideDeclaration.Value.Equals(CssValue.Bottom));
        }

        /// <summary>
        /// Returns the HTML 'caption' node that this caption represents.
        /// </summary>
        internal HtmlElementNode Node
        {
            get { return mNode; }
        }

        internal bool IsTopCaption
        {
            get { return mIsTopCaption; }
        }

        private readonly HtmlElementNode mNode;
        private readonly bool mIsTopCaption;
    }
}
