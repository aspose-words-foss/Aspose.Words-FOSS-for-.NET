// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Loading;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a single section in a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-sections/">Working with Sections</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="Section"/> can have one <see cref="Aspose.Words.Body"/> and maximum one <see cref="HeaderFooter"/>
    /// of each <see cref="HeaderFooterType"/>. <see cref="Aspose.Words.Body"/> and <see cref="HeaderFooter"/> nodes
    /// can be in any order inside <see cref="Section"/>.</p>
    ///
    /// <p>A minimal valid section needs to have <see cref="Aspose.Words.Body"/> with one <see cref="Paragraph"/>.</p>
    ///
    /// <p>Each section has its own set of properties that specify page size, orientation, margins etc.</p>
    /// <p>You can create a copy of a section using <see cref="Node.Clone(bool)"/>. The copy can be inserted into
    /// the same or different document.</p>
    /// <p>To add, insert or remove a whole section including section break and
    /// section properties use methods of the <see cref="Document.Sections"/> object.</p>
    /// <p>To copy and insert just content of the section excluding the section break
    /// and section properties use <see cref="AppendContent"/> and <see cref="PrependContent"/> methods.</p>
    /// </remarks>
    [JavaGenericArguments("CompositeNode<Node>")]
    public sealed class Section : CompositeNode, ISectionAttrSource
    {
        /// <summary>
        /// Initializes a new instance of the Section class.
        /// </summary>
        /// <remarks>
        /// <p>When the section is created, it belongs to the specified document, but is not
        /// yet part of the document and <see cref="Node.ParentNode"/> is <c>null</c>.</p>
        /// <p>To include <see cref="Section"/> into a document use <see cref="CompositeNode.InsertAfter{T}(T, Node)"/> and
        /// <see cref="CompositeNode.InsertBefore{T}(T, Node)"/> methods of the <see cref="Document"/> OR
        /// <see cref="NodeCollection.Add"/> and <see cref="NodeCollection.Insert"/> methods of the <see cref="Document.Sections"/> property.</p>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        public Section(DocumentBase doc) : this(doc, new SectPr())
        {
        }

        internal Section(DocumentBase doc, SectPr sectPr) : base(doc)
        {
            mSectPr = sectPr;
        }

        /// <summary>
        /// Returns <see cref="NodeType.Section"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.Section; }
        }

        /// <summary>
        /// Returns the <see cref="Aspose.Words.Body"/> child node of the section.
        /// </summary>
        /// <remarks>
        /// <p><see cref="Aspose.Words.Body"/> contains main text of the section.</p>
        /// <p>Returns <c>null</c> if the section does not have a <see cref="Aspose.Words.Body"/> node among its children.</p>
        /// </remarks>
        public Body Body
        {
            get { return (Body)GetChild(NodeType.Body, 0, false); }
        }

        /// <summary>
        /// Provides access to the headers and footers nodes of the section.
        /// </summary>
        public HeaderFooterCollection HeadersFooters
        {
            get
            {
                if (mHeadersFooters == null)
                    mHeadersFooters = new HeaderFooterCollection(this);
                return mHeadersFooters;
            }
        }

        /// <summary>
        /// Returns an object that represents page setup and section properties.
        /// </summary>
        public PageSetup PageSetup
        {
            get
            {
                if (mPageSetup == null)
                    mPageSetup = new PageSetup(this, Document.DocPr, Document.Styles, new FirstSectionAttrSource(this));
                return mPageSetup;
            }
        }

        /// <summary>
        /// True if the section is protected for forms. When a section is protected for forms,
        /// users can select and modify text only in form fields in Microsoft Word.
        /// </summary>
        public bool ProtectedForForms
        {
            get { return !PageSetup.Unlocked; }
            set { PageSetup.Unlocked = !value; }
        }

        /// <summary>
        /// Gets and sets section properties.
        /// </summary>
        ///<remarks> We don't normally set SectPr, since in Section's lifecycle it is provided upon construction.</remarks>
        internal SectPr SectPr
        {
            get { return mSectPr; }
            set
            {
                Debug.Assert(value != null);
                mSectPr = value;
            }
        }

        /// <summary>
        /// Creates a duplicate of this section.
        /// </summary>
        /// <dev>Kept to remain compatible with the old API.</dev>
        public Section Clone()
        {
            return (Section)Clone(true);
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            Section lhs = (Section)base.Clone(isCloneChildren, cloningListener);
            SectPr sectPr = mSectPr.Clone();
            lhs.mSectPr = sectPr;

            //Don't clone these because created on demand.
            lhs.mPageSetup = null;
            lhs.mHeadersFooters = null;

            return lhs;
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// Calls <see cref="DocumentVisitor.VisitSectionStart"/>, then calls <see cref="Node.Accept"/> for all child nodes of the section
        /// and calls <see cref="DocumentVisitor.VisitSectionEnd"/> at the end.
        /// </remarks>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }

        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitSectionStart(this);
        }

        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitSectionEnd(this);
        }

        /// <summary>
        /// Inserts a copy of content of the source section at the beginning of this section.
        /// </summary>
        /// <remarks>
        /// <p>Only content of <see cref="Body"/> of the source section is copied, page setup,
        /// headers and footers are not copied.</p>
        /// <p>The nodes are automatically imported if the source section belongs to a different document.</p>
        /// <p>No new section is created in the destination document.</p>
        /// </remarks>
        /// <param name="sourceSection">The section to copy content from.</param>
        public void PrependContent(Section sourceSection)
        {
            InsertContent(sourceSection, false);
        }

        /// <summary>
        /// Inserts a copy of content of the source section at the end of this section.
        /// </summary>
        /// <remarks>
        /// <p>Only content of <see cref="Body"/> of the source section is copied, page setup,
        /// headers and footers are not copied.</p>
        /// <p>The nodes are automatically imported if the source section belongs to a different document.</p>
        /// <p>No new section is created in the destination document.</p>
        /// </remarks>
        /// <param name="sourceSection">The section to copy content from.</param>
        public void AppendContent(Section sourceSection)
        {
            InsertContent(sourceSection, true);
        }

        /// <summary>
        /// Clears the section.
        /// </summary>
        /// <remarks>
        /// <p>The text of <see cref="Body"/> is cleared, only one empty paragraph is left that represents the section break.</p>
        /// <p>The text of all headers and footers is cleared, but <see cref="HeaderFooter"/> objects themselves are not removed.</p>
        /// </remarks>
        public void ClearContent()
        {
            ClearHeadersFooters();
            Body.RemoveAllChildren();
            Body.EnsureMinimum();
        }

        /// <summary>
        /// Clears the headers and footers of this section.
        /// </summary>
        /// <remarks>
        /// <p>The text of all headers and footers is cleared, but <see cref="HeaderFooter"/> objects themselves are not removed.</p>
        /// <p>This makes headers and footers of this section linked to headers and footers of the previous section.</p>
        /// </remarks>
        public void ClearHeadersFooters()
        {
            ClearHeadersFooters(false);
        }

        /// <summary>
        /// Clears the headers and footers of this section.
        /// </summary>
        /// <remarks>
        /// <p>The text of all headers and footers is cleared, but <see cref="HeaderFooter"/> objects themselves are not removed.</p>
        /// <p>This makes headers and footers of this section linked to headers and footers of the previous section.</p>
        /// </remarks>
        /// <param name="preserveWatermarks">True if the watermarks shall not be removed.</param>
        public void ClearHeadersFooters(bool preserveWatermarks)
        {
            for (Node child = FirstChild; child != null; child = child.NextSibling)
            {
                if (child.NodeType != NodeType.HeaderFooter)
                    continue;

                HeaderFooter headerFooter = (HeaderFooter)child;

                List<Shape> shapes = preserveWatermarks
                    ? headerFooter.GetChildNodes(NodeType.Shape, true).ToList<Shape>()
                    : EmptyNodeCollection.CreateEmpty().ToList<Shape>();

                headerFooter.RemoveAllChildren();

                foreach (Shape shape in shapes)
                {
                    if (shape.CanBeTextWatermark || shape.CanBeImageWatermark)
                    {
                        headerFooter.AppendChild(new Paragraph(Document));
                        headerFooter.FirstParagraph.AppendChild(shape);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes all shapes (drawing objects) from the headers and footers of this section.
        /// </summary>
        public void DeleteHeaderFooterShapes()
        {
            for (Node child = FirstChild; child != null; child = child.NextSibling)
            {
                if (child.NodeType == NodeType.HeaderFooter)
                    ((HeaderFooter)child).DeleteShapes();
            }
        }

        /// <summary>
        /// Ensures that the section has <see cref="Body"/> with one <see cref="Paragraph"/>.
        /// </summary>
        public void EnsureMinimum()
        {
            Body body = this.Body;
            if (body == null)
                body = (Body)AppendChild(new Body(Document));

            body.EnsureMinimum();
        }

        /// <summary>
        /// Sets <see cref="PaperSize"/> value according to the specified <see cref="EditingLanguage"/>
        /// https://en.wikipedia.org/wiki/Paper_size
        /// </summary>
        internal void SetPaperSizeByLanguage(EditingLanguage editingLanguage)
        {

            switch (editingLanguage)
            {
                case EditingLanguage.EnglishCanada:
                case EditingLanguage.EnglishPhilippines:
                case EditingLanguage.EnglishUS:
                case EditingLanguage.SpanishMexico:
                case EditingLanguage.SpanishCostaRica:
                case EditingLanguage.SpanishColombia:
                case EditingLanguage.SpanishVenezuela:
                case EditingLanguage.SpanishChile:
                    PageSetup.PaperSize = PaperSize.Letter;
                    break;
                default:
                    PageSetup.PaperSize = PaperSize.A4;
                    break;
            }
        }

        internal Story GetStory(StoryType storyType)
        {
            for (Node node = FirstChild; node != null; node = node.NextSibling)
            {
                if (((Story)node).StoryType == storyType)
                    return (Story)node;
            }
            return null;
        }

        /// <summary>
        /// Can only insert one body and one only one header footer of each type.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            switch (newChild.NodeType)
            {
                case NodeType.Body:
                case NodeType.HeaderFooter:
                {
                    StoryType newStoryType = ((Story)newChild).StoryType;
                    return (GetStory(newStoryType) == null);
                }
                default:
                    return false;
            }
        }

        internal void AddWatermark(Shape shape, bool createHeaders)
        {
            AddWatermarkIntoHeader(shape, HeaderFooterType.HeaderFirst, createHeaders);
            AddWatermarkIntoHeader(shape, HeaderFooterType.HeaderEven, createHeaders);
            AddWatermarkIntoHeader(shape, HeaderFooterType.HeaderPrimary, createHeaders);
        }

        private void AddWatermarkIntoHeader(Shape shape, HeaderFooterType headerType, bool createHeader)
        {
            HeaderFooter header = HeadersFooters[headerType];

            if (header == null)
            {
                if (!createHeader)
                    return;

                header = new HeaderFooter(Document, headerType);
                HeadersFooters.Add(header);
            }

            // Word inserts a watermark to the first paragraph.
            if (header.Paragraphs.Count == 0)
                header.AppendChild(new Paragraph(Document));

            Shape shapeToInsert = (Shape)shape.Clone(true);
            // WORDSNET-23231 Update shape ids to avoid duplicates.
            shapeToInsert.Id = shapeToInsert.Document.GetNextShapeId();
            shapeToInsert.Name = string.Format(
                "{0}{1}",
                shapeToInsert.HasImage ? Watermark.ImageNamePrefix : Watermark.TextNamePrefix,
                shapeToInsert.Id);

            header.FirstParagraph.AppendChild(shapeToInsert);
        }

        internal bool RemoveWatermark()
        {
            bool watermarkRemoved = false;
            foreach (HeaderFooter headerFooter in HeadersFooters)
            {
                if (!headerFooter.IsHeader)
                    continue;

                // Word only defines the first watermark shape as the watermark in the header,
                // but removes all watermark shapes.
                foreach (Shape shape in headerFooter.Shapes)
                {
                    if (shape.IsWatermark)
                    {
                        shape.Remove();
                        watermarkRemoved = true;
                    }
                }
            }

            return watermarkRemoved;
        }

        /// <dev>Copies and appends or prepends body of the source section into this section.</dev>
        private void InsertContent(Section sourceSection, bool isAfter)
        {
            if (sourceSection == null)
                throw new ArgumentNullException("sourceSection");

            Body srcBody = sourceSection.Body;
            if (srcBody == null)
                return;

            Body dstBody = Body;
            //Ensure we have a body to copy into.
            if (dstBody == null)
                dstBody = (Body)AppendChild(new Body(Document));

            //This makes a copy and ensures we can insert the nodes into this document.
            srcBody = (Body)Document.ImportNode(srcBody, true);

            Node refNode = (isAfter) ? dstBody.LastParagraph : null;

            // WORDSNET-22257 Reference node should be immediate child, update if needed.
            while ((refNode != null) && (refNode.ParentNode != dstBody))
                refNode = refNode.ParentNode;

            dstBody.InsertAfter(srcBody.FirstChild, null, refNode);
        }

        object ISectionAttrSource.GetDirectSectionAttr(int key)
        {
            return mSectPr.GetDirectAttr(key);
        }
        object ISectionAttrSource.GetDirectSectionAttr(int key, RevisionsView revisionsView)
        {
            return mSectPr.GetDirectAttr(key, revisionsView);
        }

        object ISectionAttrSource.FetchInheritedSectionAttr(int key)
        {
            // This attribute is only applicable document-wide, get it from docpr.
            if (key == SectAttr.EndnoteLocation)
                return Document.DocPr.FootnotePr.FetchAttr(key);
            else
                return Aspose.Words.SectPr.FetchDefaultAttr(key);
        }

        object ISectionAttrSource.FetchSectionAttr(int key)
        {
            object value = ((ISectionAttrSource)this).GetDirectSectionAttr(key);
            return (value != null) ? value : ((ISectionAttrSource)this).FetchInheritedSectionAttr(key);
        }

        object ISectionAttrSource.FetchSectionAttr(int key, RevisionsView revisionsView)
        {
            object value = ((ISectionAttrSource)this).GetDirectSectionAttr(key, revisionsView);
            return (value != null) ? value : ((ISectionAttrSource)this).FetchInheritedSectionAttr(key);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        void ISectionAttrSource.SetSectionAttr(int key, object value)
        {
            // andrnosk: WORDSNET-9138 We have to clear PaperCode upon setting PageWidth or PageHeight.
            // The PaperCode attribute specifies the producing application’s unique identifier value for current paper size.
            // If Height/Width was changed this code has no meaning.
            if ((key == SectAttr.PageWidth) || (key == SectAttr.PageHeight))
                mSectPr.Remove(SectAttr.PaperCode);

            if (key == SectAttr.EndnoteLocation)
                Document.DocPr.FootnotePr.SetAttr(key, value);
            else
                mSectPr.SetAttr(key, value);
        }

        void ISectionAttrSource.SetSectionAttr(int key, object value, RevisionsView revisionsView)
        {
            // See appropriate comment above.
            if ((key == SectAttr.PageWidth) || (key == SectAttr.PageHeight))
                mSectPr.Remove(SectAttr.PaperCode, revisionsView);

            if (key == SectAttr.EndnoteLocation)
                Document.DocPr.FootnotePr.SetAttr(key, value, revisionsView);
            else
                mSectPr.SetAttr(key, value, revisionsView);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        void ISectionAttrSource.ClearSectionAttrs()
        {
            mSectPr.Clear();
        }

        /// <summary>
        /// Returns true if this section is the first section of the document.
        /// </summary>
        internal bool IsFirstSection
        {
            get { return (ParentNode.FirstChild == this); }
        }

        /// <summary>
        /// Returns true if this section is the last section of the document.
        /// </summary>
        internal bool IsLastSection
        {
            get { return (ParentNode.LastChild == this); }
        }

        /// <summary>
        /// Returns true if this section is the only section of the document.
        /// </summary>
        internal bool IsSingleSection
        {
            get { return (ParentNode.FirstChild == ParentNode.LastChild); }
        }

        /// <summary>
        /// This class implements the <see cref="ISectionAttrSource"/> interface that allows getting attributes of
        /// the first section of the document. It is needed for the <see cref="Words.PageSetup"/> class to calculate
        /// margins and page content size with taking gutter into account.
        /// </summary>
        /// <remarks>
        /// Note that if the first section cannot be found, attribute of the current section will be returned.
        /// </remarks>
        private class FirstSectionAttrSource : ISectionAttrSource
        {
            internal FirstSectionAttrSource(Section section)
            {
                mSection = section;
            }

            object ISectionAttrSource.GetDirectSectionAttr(int key)
            {
                return FirstOrCurrentSection.GetDirectSectionAttr(key);
            }

            object ISectionAttrSource.GetDirectSectionAttr(int key, RevisionsView revisionsView)
            {
                return FirstOrCurrentSection.GetDirectSectionAttr(key, revisionsView);
            }

            object ISectionAttrSource.FetchInheritedSectionAttr(int key)
            {
                return FirstOrCurrentSection.FetchInheritedSectionAttr(key);
            }

            object ISectionAttrSource.FetchSectionAttr(int key)
            {
                return FirstOrCurrentSection.FetchSectionAttr(key);
            }

            object ISectionAttrSource.FetchSectionAttr(int key, RevisionsView revisionsView)
            {
                return FirstOrCurrentSection.FetchSectionAttr(key, revisionsView);
            }

            void ISectionAttrSource.SetSectionAttr(int key, object value)
            {
                // Do nothing: not intended for change.
            }

            void ISectionAttrSource.SetSectionAttr(int key, object value, RevisionsView revisionsView)
            {
                // Do nothing: not intended for change.
            }

            void ISectionAttrSource.ClearSectionAttrs()
            {
                // Do nothing: not intended for change.
            }

            /// <summary>
            /// Returns the first section if it can be achieved. Otherwise returns the current section.
            /// </summary>
            private ISectionAttrSource FirstOrCurrentSection
            {
                get
                {
                    Document document = mSection.Document as Document;
                    return ((document != null) && (document.FirstSection != null)) ? document.FirstSection : mSection;
                }
            }

            [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
            private readonly Section mSection;
        }

        private SectPr mSectPr;
        private PageSetup mPageSetup;
        private HeaderFooterCollection mHeadersFooters;

#if DEBUG
        public override string ToString()
        {
            return string.Format("{0} {1}", base.ToString(), SectPr.SectionStart);
        }
#elif JAVA
        public override string ToString()
        {
            // WORDSJAVA-2613 return implementation of toString method
            return string.Format("{0} {1}", base.ToString(), SectPr.SectionStart.ToString());
        }
#endif
    }
}
