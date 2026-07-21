// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Markup;
using Aspose.Words.Tables;

namespace Aspose.Words
{
    /// <summary>
    /// Base class for elements that contain block-level nodes <see cref="Paragraph"/> and <see cref="Table"/>.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/logical-levels-of-nodes-in-a-document/">Logical Levels of Nodes in a Document</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Text of a Word document is said to consist of several stories.
    /// The main text is stored in the main text story represented by <see cref="Body"/>,
    /// each header and footer is stored in a separate story represented by <see cref="HeaderFooter"/>.</p>
    /// </remarks>
    [JavaGenericArguments("CompositeNode<Node>")]
    public abstract class Story : CompositeNode, IStory
    {
        internal Story(DocumentBase doc, StoryType storyType) : base(doc)
        {
            StoryType = storyType;
        }

        /// <summary>
        /// Gets the type of this story.
        /// </summary>
        public StoryType StoryType { get; }

        /// <summary>
        /// Gets the first paragraph in the story.
        /// </summary>
        public Paragraph FirstParagraph
        {
            get { return (Paragraph)GetChild(NodeType.Paragraph, 0, false); }
        }

        /// <summary>
        /// Gets the last paragraph in the story.
        /// </summary>
        public Paragraph LastParagraph
        {
            get { return (Paragraph)GetChild(NodeType.Paragraph, -1, false); }
        }

        /// <summary>
        /// Gets a collection of paragraphs that are immediate children of the story.
        /// </summary>
        public ParagraphCollection Paragraphs
        {
            get { return mParagraphs ?? (mParagraphs = new ParagraphCollection(this)); }
        }

        /// <summary>
        /// Gets a collection of tables that are immediate children of the story.
        /// </summary>
        public TableCollection Tables
        {
            get { return mTables ?? (mTables = new TableCollection(this)); }
        }

        /// <summary>
        /// Gets a collection of shapes that are immediate children of the story.
        /// </summary>
        internal ShapeCollection Shapes
        {
            get { return mShapes ?? (mShapes = new ShapeCollection(this)); }
        }

        /// <summary>
        /// Gets a collection of structured document tags that are children of the story.
        /// </summary>
        internal StructuredDocumentTagCollection StructuredDocumentTags
        {
            get
            {
                return mStructuredDocumentTags ?? (mStructuredDocumentTags = new StructuredDocumentTagCollection(this));
            }
        }

        /// <summary>
        /// Returns true if the section contains only a single empty paragraph with a section break.
        /// </summary>
        internal bool IsSectionBreak
        {
            get { return HasOneChildOnly && (LastChild.GetText() == ControlChar.SectionBreak); }
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            Story lhs = (Story)base.Clone(isCloneChildren, cloningListener);
            lhs.mParagraphs = null;
            lhs.mTables = null;
            lhs.mShapes = null;
            return lhs;
        }

        /// <summary>
        /// Deletes all shapes from the text of this story.
        /// </summary>
        public void DeleteShapes()
        {
            NodeCollection groupShapes = GetChildNodes(NodeType.GroupShape, true);
            groupShapes.Clear();

            NodeCollection shapes = GetChildNodes(NodeType.Shape, true);
            shapes.Clear();
        }

        /// <summary>
        /// Can insert only block level elements into the story.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            return NodeUtil.IsBlockLevelNode(newChild);
        }

        /// <summary>
        /// A shortcut method that creates a <see cref="Paragraph"/> object with optional text and appends it to the end of this object.
        /// </summary>
        /// <param name="text">The text for the paragraph. Can be <c>null</c> or empty string.</param>
        /// <returns>The newly created and appended paragraph.</returns>
        public Paragraph AppendParagraph(string text)
        {
            Paragraph para = new Paragraph(Document);
            AppendChild(para);

            if (StringUtil.HasChars(text))
                para.AppendChild(new Run(Document, text));

            return para;
        }

        private ParagraphCollection mParagraphs;
        private TableCollection mTables;
        private ShapeCollection mShapes;
        private StructuredDocumentTagCollection mStructuredDocumentTags;
    }
}
