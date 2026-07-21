// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/11/2013 by Alexey Butalov

using System.Collections.Generic;
using Aspose.Words.RW.Html.Css;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Reader.CommonBorder
{
    /// <summary>
    /// Helps to translate HTML common borders (container borders) into AW paragraph's or table's borders.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Common borders are the borders around a few paragraphs. Common paragraph borders representation differs fundamentally
    /// in AW and HTML. They are represented as container CSS borders in HTML and as paragraph borders in AW.
    /// This class helps to find the common paragraph borders saved in HTML and to translate them into AW paragraph borders
    /// during HTML import.
    /// </para>
    /// <para>
    /// Note that there is an alternative method of representing common borders - by using <see cref="HtmlBlock"/> nodes,
    /// which is close to what MS Word does. When we use that alternative method we disable this resolver.
    /// </para>
    /// </remarks>
    internal class HtmlCommonBorderResolver
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="disabled">
        /// Indicates whether the common border resolver is disabled. If it is <c>true</c> then common borders don't apply
        /// to paragraph's or table's borders and we use another method of representing common borders in the document model.
        /// </param>
        internal HtmlCommonBorderResolver(bool disabled)
        {
            mDisabled = disabled;
        }

        /// <summary>
        /// Starts the container element processing.
        /// </summary>
        /// <param name="declarations">container element CSS declarations.</param>
        internal void StartContainer(CssDeclarationCollection declarations)
        {
            CommonBorderContainer parentContainer = mContainers.Top();
            BordersInfo borders = new BordersInfo(declarations);
            mContainers.Push(new CommonBorderContainer(parentContainer, borders));
        }

        /// <summary>
        /// Ends the container element processing. Applies common border to the inner container's paragraphs if needed.
        /// </summary>
        internal void EndContainer()
        {
            Debug.Assert(mContainers.Count > 0);
            CommonBorderContainer container = mContainers.Pop();
            if (!mDisabled)
                container.ApplyCommonBorderIfNeeded();
        }

        /// <summary>
        /// Processes and collects container's inner paragraph which doesn't have its own border.
        /// </summary>
        /// <param name="paragraph">Model's paragraph.</param>
        internal void CollectParagraphWithoutBorder(Paragraph paragraph)
        {
            CollectParagraphWithBorder(paragraph, CssDeclarationCollection.Empty);
        }

        /// <summary>
        /// Processes and collects container's inner paragraph which can have its own border.
        /// </summary>
        /// <param name="paragraph">Model's paragraph.</param>
        /// <param name="borderDeclarations">Paragraph's border CSS declarations.</param>
        internal void CollectParagraphWithBorder(Paragraph paragraph, CssDeclarationCollection borderDeclarations)
        {
            if (mContainers.Count == 0)
                return;

            CommonBorderContainer container = mContainers.Peek();
            container.Add(paragraph, new BordersInfo(borderDeclarations));
        }

        internal void CollectTable(Table table, CssDeclarationCollection borderDeclarations)
        {
            if (mContainers.Count == 0)
                return;

            CommonBorderContainer container = mContainers.Peek();
            container.Add(table, new BordersInfo(borderDeclarations));
        }

        /// <summary>
        /// A stack of currently opened common border containers.
        /// </summary>
        private readonly Stack<CommonBorderContainer> mContainers = new Stack<CommonBorderContainer>();

        /// <summary>
        /// Indicates whether the common border resolver is disabled.
        /// </summary>
        /// <remarks>
        /// In order to simplify outer code we've introduced an "idle" mode for this module. When the module is disabled,
        /// it doesn't do anything useful but the outer code still uses it normally. In this way we avoid writing a lot of
        /// conditional expression in the calling code and keep it simpler.
        /// </remarks>
        private readonly bool mDisabled;
    }
}
