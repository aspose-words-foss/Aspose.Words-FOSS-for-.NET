// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/02/2009 by Roman Korchagin

using Aspose.JavaAttributes;
using Aspose.Words.Fonts;
using Aspose.Words.Themes;

namespace Aspose.Words.BuildingBlocks
{
    /// <summary>
    /// Represents the root element for a glossary document within a Word document.
    /// A glossary document is a storage for AutoText, AutoCorrect entries and Building Blocks.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>Some documents, usually templates, can contain AutoText, AutoCorrect entries
    /// and/or Building Blocks (also known as <i>glossary document entries</i>, <i>document parts</i>
    /// or <i>building blocks</i>).</para>
    ///
    /// <para>To access building blocks, you need to load a document into a <see cref="Aspose.Words.Document"/>
    /// object. Building blocks will be available via the <see cref="Aspose.Words.Document.GlossaryDocument"/> property.</para>
    ///
    /// <para><see cref="GlossaryDocument"/> can contain any number of <see cref="BuildingBlock"/> objects.
    /// Each <see cref="BuildingBlock"/> represents one document part.</para>
    ///
    /// <para>Corresponds to the <b>glossaryDocument</b> and <b>docParts</b> elements in OOXML.</para>
    ///
    /// <seealso cref="Aspose.Words.Document"/>
    /// <seealso cref="Aspose.Words.Document.GlossaryDocument"/>
    /// <seealso cref="BuildingBlock"/>
    /// </remarks>
    [JavaGenericArguments("DocumentBase<BuildingBlock>")]
    public class GlossaryDocument : DocumentBase
    {
        /// <summary>
        /// Returns the <see cref="Aspose.Words.NodeType.GlossaryDocument"/> value.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.GlossaryDocument; }
        }

        /// <summary>
        /// Helps to resolve fonts in this document.
        /// </summary>
        internal override DocumentFontProvider FontProvider
        {
            get { return mMainDocument != null ? mMainDocument.FontProvider : null; }
        }

        /// <summary>
        /// Returns null as glossary document does not have its own theme.
        /// </summary>
        internal override Theme GetThemeInternal()
        {
            return null;
        }

        internal override void SuspendTrackRevisions(SuspendedRevisionTypes revisionTypes)
        {
            // Do not track changes in glossary.
        }

        internal override void ResumeTrackRevisions(SuspendedRevisionTypes revisionTypes)
        {
            // Do not track changes in glossary.
        }

        internal override bool IsTrackRevisionsEnabled
        {
            // Do not track changes in glossary.
            get { return false; }
        }

        internal override bool IsMoveRevisionsTracked
        {
            get { return false; }
        }

        /// <summary>
        /// Returns a typed collection that represents all building blocks in the glossary document.
        /// </summary>
        public BuildingBlockCollection BuildingBlocks
        {
            get
            {
                if (mBuildingBlocksCache == null)
                    mBuildingBlocksCache = new BuildingBlockCollection(this);
                return mBuildingBlocksCache;
            }
        }

        /// <summary>
        /// Gets the first building block in the glossary document.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if there are no building blocks available.
        /// </remarks>
        public BuildingBlock FirstBuildingBlock
        {
            get { return (BuildingBlock)GetChild(NodeType.BuildingBlock, 0, false); }
        }

        /// <summary>
        /// Gets the last building block in the glossary document.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if there are no building blocks available.
        /// </remarks>
        public BuildingBlock LastBuildingBlock
        {
            get { return (BuildingBlock)GetChild(NodeType.BuildingBlock, -1, false); }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// <para>Calls <see cref="DocumentVisitor.VisitGlossaryDocumentStart"/>, then calls <see cref="Node.Accept"/>
        /// for all child nodes of this node and then calls <see cref="DocumentVisitor.VisitGlossaryDocumentEnd"/>
        /// at the end.</para>
        /// </remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="GlossaryDocument.NotVisited"]/*'/>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }

        /// <summary>
        /// Accepts a visitor for visiting the start of the Glossary document.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitGlossaryDocumentStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the Glossary document.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitGlossaryDocumentEnd(this);
        }

        /// <summary>
        /// Can only add building blocks.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            return (newChild.NodeType == NodeType.BuildingBlock);
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            GlossaryDocument lhs = (GlossaryDocument)base.Clone(isCloneChildren, cloningListener);

            // Facade objects are reset to null.
            lhs.mBuildingBlocksCache = null;
            lhs.mMainDocument = null;
            return lhs;
        }

        /// <summary>
        /// Finds a building block using the specified gallery, category and name.
        /// </summary>
        /// <remarks>
        /// <para>This is a convenience method that iterates over all building blocks
        /// in this collection and returns the first building block that matches
        /// the specified gallery, category and name.</para>
        ///
        /// <para>Microsoft Word organizes building blocks into galleries. The galleries
        /// are predefined using the <see cref="BuildingBlockGallery"/> enum.
        /// Within each gallery, building blocks can be organized into one or more categories.
        /// The category name is a string. Each building block has a name. A building block
        /// name is not guaranteed to be unique.</para>
        /// </remarks>
        /// <param name="gallery">The gallery criteria.</param>
        /// <param name="category">The category criteria. Can be <c>null</c>, in which case it will not be used for comparison.</param>
        /// <param name="name">The building block name criteria.</param>
        /// <returns>The matching building block or <c>null</c> if a match was not found.</returns>
        public BuildingBlock GetBuildingBlock(BuildingBlockGallery gallery, string category, string name)
        {
            foreach (BuildingBlock block in this)
            {
                if ((block.Gallery == gallery) && ((category == null) || (block.Category == category)) && (block.Name == name))
                    return block;
            }
            return null;
        }

        internal Document MainDocument
        {
            get { return mMainDocument; }
            set { mMainDocument = value; }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private Document mMainDocument;
        private BuildingBlockCollection mBuildingBlocksCache;
    }
}
