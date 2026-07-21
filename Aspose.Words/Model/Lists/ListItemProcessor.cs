// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/05/2019 by Alexander Zhiltsov

using System;
using Aspose.Words.Drawing;
using Aspose.Words.Notes;

namespace Aspose.Words.Lists
{
    /// <summary>
    /// Base for classes that need to go through all list items/paragraphs in numeration order by numeration areas
    /// (Body, HeaderFooter, Footnote, Endnote, Shape, Comment).
    /// For more information please see description of the <see cref="ListLabelUpdater"/> class.
    /// </summary>
    internal abstract class ListItemProcessor : DocumentVisitor
    {
        public sealed override VisitorAction VisitBodyStart(Body body)
        {
            OnNumberingAreaStart(ListNumberingArea.Body);
            return VisitorAction.Continue;
        }

        public sealed override VisitorAction VisitBodyEnd(Body body)
        {
            OnNumberingAreaEnd();
            return VisitorAction.Continue;
        }

        public sealed override VisitorAction VisitHeaderFooterStart(HeaderFooter headerFooter)
        {
            OnNumberingAreaStart(ListNumberingArea.HeaderFooter);
            return VisitorAction.Continue;
        }

        public sealed override VisitorAction VisitHeaderFooterEnd(HeaderFooter headerFooter)
        {
            OnNumberingAreaEnd();
            return VisitorAction.Continue;
        }

        public sealed override VisitorAction VisitFootnoteStart(Footnote footnote)
        {
            switch (footnote.FootnoteType)
            {
                case FootnoteType.Footnote:
                    OnNumberingAreaStart(ListNumberingArea.Footnote);
                    break;
                case FootnoteType.Endnote:
                    OnNumberingAreaStart(ListNumberingArea.Endnote);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return VisitorAction.Continue;
        }

        public sealed override VisitorAction VisitFootnoteEnd(Footnote footnote)
        {
            OnNumberingAreaEnd();
            return VisitorAction.Continue;
        }

        public sealed override VisitorAction VisitCommentStart(Comment comment)
        {
            OnNumberingAreaStart(ListNumberingArea.Comment);
            return VisitorAction.Continue;
        }

        public sealed override VisitorAction VisitCommentEnd(Comment comment)
        {
            OnNumberingAreaEnd();
            return VisitorAction.Continue;
        }

        public sealed override VisitorAction VisitShapeStart(Shape shape)
        {
            OnNumberingAreaStart(ListNumberingArea.Shape);
            return VisitorAction.Continue;
        }

        public sealed override VisitorAction VisitShapeEnd(Shape shape)
        {
            OnNumberingAreaEnd();
            return VisitorAction.Continue;
        }

        /// <summary>
        /// The method is executed when a numbering area is started.
        /// </summary>
        protected abstract void OnNumberingAreaStart(ListNumberingArea area);

        /// <summary>
        /// The method is executed when a numbering area is ended.
        /// </summary>
        protected abstract void OnNumberingAreaEnd();

        /// <summary>
        /// Count of items in <see cref="ListNumberingArea"/>.
        /// </summary>
        protected static readonly int NumberOfAreas = 
            EnumUtilPal.GetEffectiveArrayLength(ListNumberingArea.Body.GetType(), 6);

        /// <summary>
        /// Designates an area of continuous list numbering.
        /// </summary>
        internal enum ListNumberingArea
        {
            Body = 0,
            HeaderFooter = 1,
            Footnote = 2,
            Endnote = 3,
            Shape = 4,
            Comment = 5,
        }
    }
}
