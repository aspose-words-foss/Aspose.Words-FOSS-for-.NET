// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/02/2009 by Roman Korchagin
using System;
using System.Diagnostics.CodeAnalysis;
using Aspose.JavaAttributes;

namespace Aspose.Words.BuildingBlocks
{
    /// <summary>
    /// Represents a glossary document entry such as a Building Block, AutoText or an AutoCorrect entry.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="BuildingBlock"/> can contain only <see cref="Aspose.Words.Section"/> nodes.</p>
    /// <p><see cref="BuildingBlock"/> can only be a child of <see cref="GlossaryDocument"/>.</p>
    /// 
    /// <para>You can create new building blocks and insert them into a glossary document.
    /// You can modify or delete existing building blocks. You can copy or move building blocks
    /// between documents. You can insert content of a building block into a document.</para>
    /// 
    /// <para>Corresponds to the <b>docPart</b>, <b>docPartPr</b> and <b>docPartBody</b> elements in OOXML.</para>
    /// 
    /// <seealso cref="GlossaryDocument"/>
    /// </remarks>
    //public-internal
    [JavaGenericArguments("CompositeNode<Node>")]
    public class BuildingBlock : CompositeNode
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <remarks>
        /// <p>When <see cref="BuildingBlock"/> is created, it belongs to the specified glossary document, 
        /// but is not yet part of the glossary document and <see cref="Node.ParentNode"/> is <c>null</c>.</p>
        /// <p>To append <see cref="BuildingBlock"/> to a <see cref="GlossaryDocument"/> use 
        /// <see cref="CompositeNode.AppendChild{T}(T)"/>.</p>
        /// </remarks>
        /// <param name="glossaryDoc">The owner document.</param>
        public BuildingBlock(GlossaryDocument glossaryDoc) : base(glossaryDoc)
        {
        }

        /// <summary>
        /// Returns the <see cref="Aspose.Words.NodeType.BuildingBlock"/> value.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.BuildingBlock; }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// <para>Calls <see cref="DocumentVisitor.VisitBuildingBlockStart"/>, then calls 
        /// <see cref="Node.Accept"/> for all child nodes of this building block, then calls 
        /// <see cref="DocumentVisitor.VisitBuildingBlockEnd"/>.</para>
        /// </remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="BuildingBlock.NotVisited"]/*'/>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }

        /// <summary>
        /// Accepts a visitor for visiting the start of the BuildingBlock.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitBuildingBlockStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the BuildingBlock.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitBuildingBlockEnd(this);
        }
        
        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            BuildingBlock lhs = (BuildingBlock)base.Clone(isCloneChildren, cloningListener);

            // Facade objects are reset to null.
            lhs.mSectionsCache = null;

            return lhs;
        }

        /// <summary>
        /// Can only add <see cref="Section"/> nodes.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            return (newChild.NodeType == NodeType.Section);
        }

        internal void SetNameSafe(string name)
        {
            if (StringUtil.HasChars(name))
                mName = name;
        }

        internal void SetCategorySafe(string category)
        {
            if (StringUtil.HasChars(category))
                mCategory = category;
        }

        /// <summary>
        /// Returns a collection that represents all sections in the building block.
        /// </summary>
        public SectionCollection Sections
        {
            get
            {
                if (mSectionsCache == null)
                    mSectionsCache = new SectionCollection(this);
                return mSectionsCache;
            }
        }

        /// <summary>
        /// Gets the first section in the building block.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if there are no sections.
        /// </remarks>
        public Section FirstSection
        {
            get { return (Section)GetChild(NodeType.Section, 0, false); }
        }

        /// <summary>
        /// Gets the last section in the building block.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if there are no sections.
        /// </remarks>
        public Section LastSection
        {
            get { return (Section)GetChild(NodeType.Section, -1, false); }
        }

        /// <summary>
        /// Gets or sets the name of this building block.
        /// </summary>
        /// <remarks>
        /// <para>The name may contain any string content, usually a friendly identifier.
        /// Multiple building blocks can have the same name.</para>
        /// 
        /// <para>Cannot be <c>null</c> and cannot be an empty string.</para>
        /// 
        /// <para>Corresponds to the <b>docPartPr.name</b> element in OOXML.</para>
        /// 
        /// <seealso cref="Gallery"/>
        /// <seealso cref="Category"/>
        /// </remarks>
        public string Name
        {
            get { return mName; }
            set
            {
                ArgumentUtil.CheckHasChars(value, "name");
                mName = value;
            }
        }

        /// <summary>
        /// When true, specifies that this is a built-in entry.
        /// </summary>
        internal bool Decorated
        {
            get { return mDecorated; }
            set { mDecorated = value; }
        }

        /// <summary>
        /// Gets or sets an identifier (a 128-bit GUID) that uniquely identifies this building block.
        /// </summary>
        /// <remarks>
        /// <para>Can be used by an application to uniquely reference a building block regardless of
        /// different naming due to localization.</para>
        /// 
        /// <para>Corresponds to the <b>docPartPr.guid</b> element in OOXML.</para>
        /// </remarks>
        public Guid Guid
        {
            get { return mGuid; }
            set { mGuid = value; }
        }


        /// <summary>
        /// Gets or sets the description associated with this building block.
        /// </summary>
        /// <remarks>
        /// <para>The description may contain any string content, usually additional information.</para>
        /// 
        /// <para>Cannot be <c>null</c>, but can be an empty string.</para>
        /// 
        /// <para>Corresponds to the <b>docPartPr.description</b> element in OOXML.</para>
        /// </remarks>
        public string Description
        {
            get { return mDescription; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "description");
                mDescription = value;
            }
        }

        /// <summary>
        /// Specifies the first-level categorization for the building block for the purposes of 
        /// classification or user interface sorting.
        /// </summary>
        /// <remarks>
        /// <para>Building blocks in Microsoft Word user interface are arranged 
        /// into Galleries. Each <see cref="Gallery"/> can have multiple Categories. Each block within
        /// a <see cref="Category"/> has a <see cref="Name"/>.</para>
        /// 
        /// <para>Corresponds to the <b>docPartPr.category.gallery</b> element in OOXML.</para>
        /// 
        /// <seealso cref="Category"/>
        /// <seealso cref="Name"/>
        /// </remarks>
        public BuildingBlockGallery Gallery
        {
            get { return mGallery; }
            set { mGallery = value; }
        }

        /// <summary>
        /// Specifies the second-level categorization for the building block.
        /// </summary>
        /// <remarks>
        /// <para>Building blocks in Microsoft Word user interface are arranged 
        /// into Galleries. Each <see cref="Gallery"/> can have multiple Categories. Each block within
        /// a <see cref="Category"/> has a <see cref="Name"/>.</para>
        /// 
        /// <para>Cannot be <c>null</c> and cannot be an empty string.</para>
        /// 
        /// <para>Corresponds to the <b>docPartPr.category.name</b> element in OOXML.</para>
        /// 
        /// <seealso cref="Gallery"/>
        /// <seealso cref="Name"/>
        /// </remarks>
        public string Category
        {
            get { return mCategory; }
            set
            {
                ArgumentUtil.CheckHasChars(value, "category");
                mCategory = value;
            }
        }

        /// <summary>
        /// Specifies the behavior that shall be applied when the contents of the building block
        /// is inserted into the main document.
        /// </summary>
        public BuildingBlockBehavior Behavior
        {
            get { return mBehavior; }
            set { mBehavior = value; }
        }

        /// <summary>
        /// The SPEC says it is a paragraph style identifier, but it does not look so.
        /// We just preserve it as a string.
        /// </summary>
        internal string Style
        {
            get { return mStyle; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "style");
                mStyle = value;
            }
        }

        /// <summary>
        /// Specifies the building block type.
        /// </summary>
        /// <remarks>
        /// <para>The building block type can influence the visibility and behavior of the 
        /// building block in Microsoft Word.</para>
        /// 
        /// <para>Corresponds to the <b>docPartPr.types</b> element in OOXML.</para>
        /// </remarks>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Public API, as designed.")]
        public BuildingBlockType Type
        {
            get { return mType; }
            set { mType = value; }
        }

        private string mName = "(Empty Name)";
        private bool mDecorated;
        private Guid mGuid = Guid.Empty;    // Must be initialized for Java to work.
        private string mDescription = "";
        private BuildingBlockGallery mGallery = BuildingBlockGallery.Default;
        private string mCategory = "(Empty Category)";
        private BuildingBlockBehavior mBehavior = BuildingBlockBehavior.Default;
        private string mStyle = "";
        private BuildingBlockType mType = BuildingBlockType.Default;

        /// <summary>
        /// Facade, created on demand.
        /// </summary>
        private SectionCollection mSectionsCache;
    }
}
