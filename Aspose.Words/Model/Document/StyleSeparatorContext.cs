// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/11/2017 by Dmitry Sokolov

namespace Aspose.Words
{
    internal class StyleSeparatorContext
    {
        public Paragraph StyleSeparatorParagraph
        {
            get { return mStyleSeparatorParagraph; }
            set { mStyleSeparatorParagraph = value; }
        }
        public Paragraph LineBreakParagraph
        {
            get { return mLineBreakParagraph; }
            set { mLineBreakParagraph = value; }
        }

        public CompositeNode LineBreakParentNode
        {
            get { return mLineBreakParentNode; }
            set { mLineBreakParentNode = value; }
        }

        public Node LineBreakNextSibling
        {
            get { return mLineBreakNextSibling; }
            set { mLineBreakNextSibling = value; }
        }

        public Node LineBreakPrevSibling
        {
            get { return mLineBreakPrevSibling; }
            set { mLineBreakPrevSibling = value; }
        }

        public bool IsRepeatedInsertExpected
        {
            get { return mIsRepeatedInsertExpected; }
            set { mIsRepeatedInsertExpected = value; }
        }

        private Paragraph mStyleSeparatorParagraph;
        private Paragraph mLineBreakParagraph;
        private CompositeNode mLineBreakParentNode;
        private Node mLineBreakNextSibling;
        private Node mLineBreakPrevSibling;
        private bool mIsRepeatedInsertExpected;
    }
}
