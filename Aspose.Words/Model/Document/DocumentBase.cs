// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/02/2009 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Drawing;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Fonts;
using Aspose.Words.Formatting.Intern;
using Aspose.Words.Lists;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.Notes;
using Aspose.Words.Settings;
using Aspose.Words.Themes;

namespace Aspose.Words
{
    /// <summary>
    /// Provides the abstract base class for a main document and a glossary document of a Word document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>Aspose.Words represents a Word document as a tree of nodes. <see cref="DocumentBase"/> is a
    /// root node of the tree that contains all other nodes of the document.</para>
    ///
    /// <para><see cref="DocumentBase"/> also stores document-wide information such as <see cref="Styles"/> and
    /// <see cref="Lists"/> that the tree nodes might refer to.</para>
    ///
    /// <seealso cref="Aspose.Words.Document"/>
    /// <seealso cref="Aspose.Words.DocumentBase"/>
    /// </remarks>
    [JavaGenericParameter("T extends Node"), JavaGenericArguments("CompositeNode<T>")]
    public abstract class DocumentBase : CompositeNode
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        protected DocumentBase()
        {
            mNullNode = new NullNode(this);

            // We just need a start to make new shapes unique ids.
            // This number is well above MS Word native shape numbers so its safe.
            mNextShapeId = 100000;

            // According to "Office Drawing Extensions to Office Open XML Structure" textbox 'id' is unsignedShort type value,
            // that is why we use half of unsignedShort as a start unique id (ushort.MaxValue / 2).
            // WORDSNET-22298 Exclude 0x7FFF value from the id's range. In the Word 2019 64 bit this value breaks document layout.
            mNextDmlTextBoxId = 32768;

            mFontInfos = new FontInfoCollection();

            mStyles = new StyleCollection(this);
            mLists = new ListCollection(this);
            mHtmlBlockCollection = new HtmlBlockCollection();
            mPeople = new PersonCollectionInternal();
            mDocPr = new DocPr();
            mVariables = new VariableCollection();
            mFootnoteSeparators = new FootnoteSeparatorCollection();
            // FOSS

            mNodeChangingCallbackChain = new NodeChangingCallbackChain();
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {

            // Calling the base clones all child nodes if necessary.
            // We don't need to import child nodes because all styles, lists etc are cloned too
            // and existing ids will work fine.
            DocumentBase lhs = (DocumentBase)base.Clone(isCloneChildren, cloningListener);

            lhs.mNullNode = new NullNode(lhs);
            lhs.mNodeChangingCallbackChain = new NodeChangingCallbackChain();

#if CPLUSPLUS
            lhs.mHangingNodesCollection = new CodePorting.Translator.Cs2Cpp.MemoryManagement.ObjectsBag();
#endif
            // DD: There is one special effect that happens to this.mSdtIdManager:
            // When we clone the base nodes, in the first line of this function,
            // we perform adding newly cloned SDT nodes and they will get new
            // Ids because lhs.mSdtIdManager is shallow cloned and still references
            // this.mSdtIdManager.
            // We will reset Ids back to their original numbers for lhs by using RestoreSdtIds below,
            // but the source mSdtManager will store references of newIDs during document life cycle.
            // We could add special code to remove these newIDs, but I don't think Clone is done
            // often enough to clutter mSdtIdManager too much with those phantom IDs.
            lhs.mSdtIdManager = null;

            // will have to create new placeholder and look and attach placeholders to SDTs if needed.
            lhs.mSdtPlaceholderManager = null;

            // Copy only objects that can be changed during the document lifetime.
            lhs.mFontInfos = mFontInfos.Clone();
            lhs.mStyles = mStyles.Clone(lhs);
            lhs.mLists = mLists.Clone(lhs, cloningListener);
            lhs.mDocPr = mDocPr.Clone();
            lhs.mVariables = mVariables.Clone();
            lhs.mFootnoteSeparators = mFootnoteSeparators.Clone(lhs, cloningListener);
            lhs.mHtmlBlockCollection = mHtmlBlockCollection.Clone();
            lhs.mPeople = mPeople.Clone();
            // FOSS

            if (mBackgroundShape != null)
                lhs.mBackgroundShape = (Shape)lhs.ImportNode(mBackgroundShape, true, cloningListener);

            if (isCloneChildren)
                RestoreSdtIds(lhs);

            if (InternManager != null)
                lhs.InternManager = null;
            // WORDSNET-28264 We shall reset mTemporaryDoc. It is not safe to use it in a cloned copy.
            lhs.mTemporaryDoc = null;

            return lhs;
        }

        /// <summary>
        /// Resets document state as before loading data from a source.
        /// </summary>
        protected void ResetState()
        {
            mStyles = new StyleCollection(this);
            RemoveAllChildren();
        }

        /// <summary>
        /// Cloning of DocumentBase instances needs to keep Ids of Sdt elements <see cref="StructuredDocumentTag"/>> during clone.
        /// This is because cloning the whole documentBase does not interfere with Id uniqueness, so we need to retain Ids since OOXML
        /// spec considers them persistent. See more in <see cref="StructuredDocumentTag.Id"/>
        /// </summary>
        /// <remarks>
        /// RK: This looks a bit ugly to override IDs after cloning. Is there a way to make it nicer?
        /// DD: It is not easy with present cloning technique. If cloning was a a visitor-style operation, then we could set mSdtIdManager to null
        /// before SDT nodes are appended to the cloned DocumentBase and this would have fixed the issue.
        /// </remarks>
        private void RestoreSdtIds(DocumentBase lhs)
        {
            NodeCollection sdts = GetChildNodes(NodeType.StructuredDocumentTag, true);
            if (sdts.Count != 0)
            {
                NodeCollection dstSdts = lhs.GetChildNodes(NodeType.StructuredDocumentTag, true);
                Debug.Assert(sdts.Count == dstSdts.Count);
                for (int sdtIdx = 0; sdtIdx < dstSdts.Count; sdtIdx++)
                    ((StructuredDocumentTag)dstSdts[sdtIdx]).SetId(((StructuredDocumentTag)sdts[sdtIdx]).Id);
            }
        }

        /// <overloads>Imports a node from another document to the current document.</overloads>
        /// <summary>
        /// <p>Imports a node from another document to the current document.</p>
        /// </summary>
        /// <remarks>
        /// <p>This method uses the <see cref="ImportFormatMode.UseDestinationStyles"/> option to resolve formatting.</p>
        ///
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.ImportNodeCommon"]/*'/>
        ///
        /// <seealso cref="NodeImporter"/>
        /// </remarks>
        /// <param name="srcNode">The node being imported.</param>
        /// <param name="isImportChildren"><c>true</c> to import all child nodes recursively; otherwise, <c>false</c>.</param>
        /// <returns>The cloned node that belongs to the current document.</returns>
        public Node ImportNode(Node srcNode, bool isImportChildren)
        {
            return ImportNode(srcNode, isImportChildren, ImportFormatMode.UseDestinationStyles, null);
        }

        /// <summary>
        /// <p>Imports a node from another document to the current document.</p>
        /// </summary>
        /// <remarks>
        /// <p>This method uses the <see cref="ImportFormatMode.UseDestinationStyles"/> option to resolve formatting.</p>
        ///
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.ImportNodeCommon"]/*'/>
        ///
        /// <seealso cref="NodeImporter"/>
        /// </remarks>
        /// <param name="srcNode">The node being imported.</param>
        /// <param name="isImportChildren"><c>true</c> to import all child nodes recursively; otherwise, <c>false</c>.</param>
        /// <param name="cloningListener">The listener to the node cloning events.</param>
        /// <returns>The cloned node that belongs to the current document.</returns>
        internal Node ImportNode(Node srcNode, bool isImportChildren, INodeCloningListener cloningListener)
        {
            return ImportNode(srcNode, isImportChildren, ImportFormatMode.UseDestinationStyles, cloningListener);
        }

        /// <summary>
        /// <p>Imports a node from another document to the current document with an option to control formatting.</p>
        /// </summary>
        /// <remarks>
        /// <p>This overload is useful to control how styles and list formatting are imported.</p>
        ///
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.ImportNodeCommon"]/*'/>
        ///
        /// <seealso cref="ImportFormatMode"/>
        /// <seealso cref="NodeImporter"/>
        /// </remarks>
        /// <param name="srcNode">The node to imported.</param>
        /// <param name="isImportChildren"><c>true</c> to import all child nodes recursively; otherwise, <c>false</c>.</param>
        /// <param name="importFormatMode">Specifies how to merge style formatting that clashes.</param>
        /// <returns>The cloned, imported node. The node belongs to the destination document, but has no parent.</returns>
        public Node ImportNode(Node srcNode, bool isImportChildren, ImportFormatMode importFormatMode)
        {
            return ImportNode(srcNode, isImportChildren, importFormatMode, null);
        }

        /// <summary>
        /// <p>Imports a node from another document to the current document with an option to control formatting.</p>
        /// </summary>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.ImportNodeCommon"]/*'/>
        ///
        /// <seealso cref="ImportFormatMode"/>
        /// <seealso cref="NodeImporter"/>
        /// </remarks>
        /// <param name="srcNode">The node to imported.</param>
        /// <param name="isImportChildren"><c>true</c> to import all child nodes recursively; otherwise, <c>false</c>.</param>
        /// <param name="importFormatMode">Specifies how to merge style formatting that clashes.</param>
        /// <param name="cloningListener">The listener to the node cloning events.</param>
        /// <returns>The cloned, imported node. The node belongs to the destination document, but has no parent.</returns>
        private Node ImportNode(
            Node srcNode,
            bool isImportChildren,
            ImportFormatMode importFormatMode,
            INodeCloningListener cloningListener)
        {
            NodeImporter importer = new NodeImporter(srcNode.Document, this, importFormatMode);
            return importer.ImportNode(srcNode, isImportChildren, cloningListener);
        }

        /// <summary>
        /// Called before any insert or remove node in the tree takes place.
        /// Marks the document as modified.
        /// Returns an argument object for the node changed event if there are any listeners registered for this event.
        /// </summary>
        internal NodeChangingArgs InternalEvent(Node node, Node oldParent, Node newParent, NodeChangingAction action)
        {
            // Indicate the document was modified by the user.
            mTreeChangeCount++;

            return !mNodeChangingCallbackChain.IsEmpty
                ? new NodeChangingArgs(node, oldParent, newParent, action)
                : null;
        }

        /// <summary>
        /// Marks document as modified.
        /// </summary>
        /// <remarks>
        /// This forces auxiliary collections rebuilt.
        /// </remarks>
        internal void MarkChanged()
        {
            mTreeChangeCount++;
        }

        [JavaConvertCheckedExceptions]
        internal void BeforeEvent(NodeChangingArgs args)
        {
            if (mNodeChangingCallbackChain.IsEmpty)
                return;

            switch (args.Action)
            {
                case NodeChangingAction.Insert:
                    mNodeChangingCallbackChain.NodeInserting(args);
                    break;
                case NodeChangingAction.Remove:
                    mNodeChangingCallbackChain.NodeRemoving(args);
                    break;
                default:
                    throw new InvalidOperationException("Unknown node changed action.");
            }
        }

        [JavaConvertCheckedExceptions]
        internal void AfterEvent(NodeChangingArgs args)
        {
            if (mNodeChangingCallbackChain.IsEmpty)
                return;

            switch (args.Action)
            {
                case NodeChangingAction.Insert:
                    mNodeChangingCallbackChain.NodeInserted(args);
                    break;
                case NodeChangingAction.Remove:
                    mNodeChangingCallbackChain.NodeRemoved(args);
                    break;
                default:
                    throw new InvalidOperationException("Unknown node changed action.");
            }
        }

        /// <summary>
        /// Adds the specified <see cref="INodeChangingCallback"/> instance to the end of the invocation chain.
        /// </summary>
        internal void AddInternalNodeChangingCallback(INodeChangingCallback callback)
        {
            mNodeChangingCallbackChain.AddInternalCallback(callback);
        }

        /// <summary>
        /// Removes the specified <see cref="INodeChangingCallback"/> instance from the invocation chain
        /// if the callback is contained in it. Returns <c>true</c> if the callback was removed.
        /// </summary>
        internal bool RemoveInternalNodeChangingCallback(INodeChangingCallback callback)
        {
            return mNodeChangingCallbackChain.RemoveInternalCallback(callback);
        }

        internal abstract void SuspendTrackRevisions(SuspendedRevisionTypes revisionTypes);
        internal abstract void ResumeTrackRevisions(SuspendedRevisionTypes revisionTypes);

        internal void SuspendMappedCustomXmlUpdate()
        {
            mSuspendMappedCustomXmlUpdate++;
        }

        internal void ResumeMappedCustomXmlUpdate()
        {
            mSuspendMappedCustomXmlUpdate--;
        }

        /// <summary>
        /// Indicates whether mapped CustomXML runtime update is enabled.
        /// </summary>
        internal bool IsMappedCustomXmlUpdateSuspended
        {
            get { return mSuspendMappedCustomXmlUpdate > 0; }
        }

        internal abstract bool IsTrackRevisionsEnabled
        {
            get;
        }

        /// <summary>
        /// Indicates whether node movements are tracked as move revisions. If it is <c>false</c>, the movements are
        /// represented as <see cref="RevisionType.Deletion"/> and <see cref="RevisionType.Insertion"/> revisions.
        /// </summary>
        internal abstract bool IsMoveRevisionsTracked
        {
            get;
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        public override DocumentBase Document
        {
            [System.Diagnostics.DebuggerStepThrough]
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return this; }
        }

        /// <summary>
        /// Called when a node is inserted or removed in the document.
        /// </summary>
        public INodeChangingCallback NodeChangingCallback
        {
            get { return mNodeChangingCallbackChain.UserCallback; }
            set { mNodeChangingCallbackChain.UserCallback = value; }
        }

        /// <summary>
        /// Allows to control how external resources are loaded.
        /// </summary>
        public IResourceLoadingCallback ResourceLoadingCallback
        {
            get { return mResourceLoadingCallback; }
            set { mResourceLoadingCallback = value; }
        }

        /// <summary>
        /// Incremented each time a node is inserted or removed. Used as a version to invalidate live node collections.
        /// </summary>
        internal int TreeChangeCount
        {
            get { return mTreeChangeCount; }
        }

        internal Node NullNode
        {
            get { return mNullNode; }
        }
#if CPLUSPLUS // alexnosk: Workaround for hanging nodes. Must be removed when CsToCppPorter.MemoryManagement.BindLifetime is used where required.
        /// <summary>
        /// Collection of nodes created fro this document but not added into it.
        /// Allows to keep nodes in memory while document is alive. Required to avoid NullReferenceExceptions in C++.
        /// </summary>
        internal CodePorting.Translator.Cs2Cpp.MemoryManagement.ObjectsBag HangingNodesCollection
        {
            get { return mHangingNodesCollection; }
        }

        internal void RemoveHangingNode(Node node)
        {
            HangingNodesCollection.Remove(node);
        }

        internal void AddHangingNode(Node node)
        {
            HangingNodesCollection.Add(node);
        }
#endif
        /// <summary>
        /// Provides access to properties of fonts used in this document.
        /// </summary>
        /// <remarks>
        /// <para>This collection of font definitions is loaded as is from the document.
        /// Font definitions might be optional, missing or incomplete in some documents.</para>
        ///
        /// <para>Do not rely on this collection to ascertain that a particular font is used in the document.
        /// You should only use this collection to get information about fonts that might be used in the document.</para>
        ///
        /// <seealso cref="FontInfoCollection"/>
        /// <seealso cref="FontInfo"/>
        /// </remarks>
        public FontInfoCollection FontInfos
        {
            get { return mFontInfos; }
        }

        /// <summary>
        /// Returns a collection of styles defined in the document.
        /// </summary>
        /// <remarks>
        /// <p>For more information see the description of the <see cref="StyleCollection"/> class.</p>
        ///
        /// <seealso cref="StyleCollection"/>
        /// <seealso cref="Style"/>
        /// </remarks>
        public StyleCollection Styles
        {
            get { return mStyles; }
        }

        /// <summary>
        /// Provides access to the list formatting used in the document.
        /// </summary>
        /// <remarks>
        /// <p>For more information see the description of the <see cref="ListCollection"/> class.</p>
        ///
        /// <seealso cref="ListCollection"/>
        /// <seealso cref="List"/>
        /// <seealso cref="ListFormat"/>
        /// </remarks>
        public ListCollection Lists
        {
            get { return mLists; }
        }

        /// <summary>
        /// Returns a collection of paragraph group properties in the document.
        /// </summary>
        internal HtmlBlockCollection HtmlBlockCollection
        {
            get { return mHtmlBlockCollection; }
        }

        /// <summary>
        /// Returns a collection of objects that represent contact information for authors of comments and revisions
        /// in the document.
        /// </summary>
        internal PersonCollectionInternal People
        {
            get { return mPeople; }
        }

        /// <summary>
        /// Called during various document processing procedures when an issue is detected that might result
        /// in data or formatting fidelity loss.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public IWarningCallback WarningCallback
        {
            get { return mWarningCallback; }
            set { mWarningCallback = value; }
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        internal void Warn(WarningType type, WarningSource source, string description)
        {
            if (WarningCallback != null)
                WarningCallback.Warning(new WarningInfo(type, source, description));
        }

        internal int GetNextShapeId()
        {
            // WORDSNET-14012 Pre-increment have to be used to avoid duplicate shape identifiers.
            return ++mNextShapeId;
        }

        /// <summary>
        /// Get next identifier of the shape.
        /// This overload for enable testing purpose.
        /// </summary>
        /// <param name="shape">Shape which requested new identifier.</param>
        /// <returns>Identifier for shape.</returns>
        internal virtual int GetNextShapeId(ShapeBase shape)
        {
            return GetNextShapeId();
        }

        internal void SetNextShapeId(int value)
        {
            mNextShapeId = value;
        }

        /// <summary>
        /// Return new value for shape identifier in case when linked shapes placed
        /// in different identifier range.
        /// </summary>
        /// <param name="currentId">First shape identifier in the linked shape sequence.</param>
        /// <param name="lnkItemsCount">Count of the inked shapes.</param>
        /// <returns>Mapped identifier.</returns>
        internal virtual int MapShapeToRange(int currentId, int lnkItemsCount)
        {
            int lastChainId = currentId + lnkItemsCount;

            // Textboxes from one linked chain have to be placed in one shape identifier range.
            if (!HaveSameIdMap(currentId, lastChainId))
            {
                currentId = GetNearestMapId(currentId);
                SetNextShapeId(currentId);
                currentId = GetNextShapeId();
            }

            return currentId;
        }

        /// <summary>
        /// Returns true if both given identifiers belong to the same drawing idmap.
        /// </summary>
        /// <remarks>
        /// AM. Frankly we should analyze idmaps deeper because more than one range can be allocated to drawing but
        /// lets postpone for a while.
        /// </remarks>
        internal static bool HaveSameIdMap(int id1, int id2)
        {
            int spidmap1 = id1 / MapDivider;
            int spidmap2 = id2 / MapDivider;

            return (spidmap1 == spidmap2);
        }

        /// <summary>
        /// Get nearest from right side initial value for a new range of the identifiers.
        /// </summary>
        /// <param name="id">current identifier.</param>
        /// <returns>Nearest beginning of the identifiers range.</returns>
        internal static int GetNearestMapId(int id)
        {
            return unchecked(((id / MapDivider) + (((id % MapDivider) > 0) ? 1 : 0)) * MapDivider);
        }

        internal int GetNextDmlTextBoxId()
        {
            return mNextDmlTextBoxId++;
        }

        internal void ResetNextAnnotationId()
        {
            mNextAnnotationId = 0;
        }

        internal int GetNextAnnotationId()
        {
            return mNextAnnotationId++;
        }

        /// <summary>
        /// Updates NextAnnotationId to max identifier of existing comments.
        /// </summary>
        internal void UpdateAnnotationId()
        {
            foreach (Comment comment in GetChildNodes(NodeType.Comment, true))
                mNextAnnotationId = System.Math.Max(mNextAnnotationId, comment.Id + 1);
        }

        internal DocPr DocPr
        {
            get { return mDocPr; }
        }

        /// <summary>
        /// Returns the collection of variables added to a document or template.
        /// </summary>
        internal VariableCollection Variables
        {
            get { return mVariables; }
        }

        /// <summary>
        /// Provides access to the footnote/endnote separators defined in the document.
        /// </summary>
        public FootnoteSeparatorCollection FootnoteSeparators
        {
            get { return mFootnoteSeparators; }
        }

        /// <summary>
        /// Gets or sets the background shape of the document. Can be <c>null</c>.
        /// </summary>
        /// <remarks>
        /// <para>Microsoft Word allows only a shape that has its <see cref="ShapeBase.ShapeType"/> property equal
        /// to <see cref="ShapeType.Rectangle"/> to be used as a background shape for a document.</para>
        ///
        /// <para>Microsoft Word supports only the fill properties of a background shape. All other properties
        /// are ignored.</para>
        ///
        /// <para>Setting this property to a non-null value will also set the <see cref="ViewOptions.DisplayBackgroundShape"/> to <c>true</c>.</para>
        ///
        /// <seealso cref="ViewOptions.DisplayBackgroundShape"/>
        /// <seealso cref="PageColor"/>
        /// </remarks>
        public Shape BackgroundShape
        {
            get { return mBackgroundShape; }
            set
            {
                if (value != null)
                {
                    if (value.Document != this)
                        throw new ArgumentException("The shape was created from a different document.");

                    if (value.ParentNode != null)
                        throw new ArgumentException("The shape is a child of another node.");

                    if (value.ShapeType != ShapeType.Rectangle)
                        throw new ArgumentException("Only a rectangle shape can be set as a document background.");
                }

                mBackgroundShape = value;
                mDocPr.ViewOptions.DisplayBackgroundShape = (mBackgroundShape != null);
            }
        }

        /// <summary>
        /// Use this during document loading.
        /// </summary>
        internal void SetBackgroundShapeSafe(Shape shape)
        {
            mBackgroundShape = shape;
        }

        /// <summary>
        /// Creates a temporary document, for rendering text of Dml txShape and for Math.
        /// </summary>
        internal Document CreateTempDocument()
        {
            // Clone source document without children, we will use this document as temporary document,
            // for layout text in shape. Do not create new document here to keep using styles and themes from source one.
            Document tmpDoc = Clone(false) as Document; // Removed redundant 'this' for C++.
            if (tmpDoc == null)
                tmpDoc = new Document(DocumentCtorMode.BlankDocumentNode);

            tmpDoc.EnsureMinimum();

            // Fonts have to be added to RequestedFontsCache.
            tmpDoc.FontProvider.RequestedFontsCache = FontProvider.RequestedFontsCache;

#if CPLUSPLUS
            AddHangingNode(tmpDoc);
#endif
            return tmpDoc;
        }

        /// <summary>
        /// Gets a temporary document. Used to render text for various DML objects.
        /// </summary>
        /// <param name="clear">Whether to remove all content.</param>
        /// <returns>The cached temporary document. Creates a new one if it doesn't exist.</returns>
        internal Document GetTemporaryDoc(bool clear)
        {
            if (mTemporaryDoc == null)
                mTemporaryDoc = CreateTempDocument();

            if (clear)
            {
                mTemporaryDoc.RemoveAllChildren();
                mTemporaryDoc.EnsureMinimum();
            }

            return mTemporaryDoc;
        }

        /// <summary>
        /// Gets or sets the page color of the document. This property is a simpler version of <see cref="BackgroundShape"/>.
        /// </summary>
        /// <remarks>
        /// <para>This property provides a simple way to specify a solid page color for the document.
        /// Setting this property creates and sets an appropriate <see cref="BackgroundShape"/>.</para>
        ///
        /// <para>If the page color is not set (e.g. there is no background shape in the document) returns
        /// <ms><see cref="Color.Empty"/></ms><java>a zero color</java><cpp><see cref="Color.Empty"/></cpp>.</para>
        ///
        /// <seealso cref="BackgroundShape"/>
        /// </remarks>
        public Color PageColor
        {
            get { return (BackgroundShape != null) ? BackgroundShape.FillColor : Color.Empty; }
            set
            {
                // WORDSNET-20068 Color.Empty should remove BackgroundShape.
                if (value.IsEmpty)
                {
                    BackgroundShape = null;
                    return;
                }

                BackgroundShape = new Shape(this, ShapeType.Rectangle);
                BackgroundShape.FillColor = value;

                // WORDSNET-9995 The background shape should not have a visible border.
                BackgroundShape.Stroke.On = false;
            }
        }

        /// <summary>
        /// Handles all placeholder-specific tasks for Sdts in this document.
        /// </summary>
        internal SdtPlaceholderManager SdtPlaceholderManager
        {
            get
            {
                if (mSdtPlaceholderManager == null)
                    mSdtPlaceholderManager = new SdtPlaceholderManager(this);

                return mSdtPlaceholderManager;
            }
        }

        /// <summary>
        /// Handles all id-related tasks for Sdts in this document.
        /// </summary>
        internal UniqueIdManager SdtIdManager
        {
            get
            {
                if (mSdtIdManager == null)
                    mSdtIdManager = new UniqueIdManager();

                return mSdtIdManager;
            }
        }

        /// <summary>
        /// False for normal documents.
        /// Can be true for documents that were loaded from clipboard data.
        /// When true, indicated that the last section (paragraph) break only exists for the purpose
        /// to make the document valid, but it was not in the user's selection.
        /// </summary>
        internal bool IsIgnoreLastSectionBreak
        {
            get { return mIsIgnoreLastSectionBreak; }
            set { mIsIgnoreLastSectionBreak = value; }
        }

        /// <summary>
        /// A cache for calculated image transparency flag.
        /// The type of a key is <see cref="int"/> (image bytes hash code), the type of a value is <see cref="bool"/>.
        /// </summary>
        internal Dictionary<int, bool> ImageTransparencyFlagCache
        {
            get
            {
                if (mImageTransparencyFlagCache == null)
                    mImageTransparencyFlagCache = new Dictionary<int, bool>();

                return mImageTransparencyFlagCache;
            }
        }

        /// <summary>
        /// Helps to resolve fonts in this document.
        /// </summary>
        internal abstract DocumentFontProvider FontProvider { get; }

        /// <summary>
        /// If the document implementation has a theme, it should return it. Can return null.
        /// </summary>
        /// <dev>
        /// Renamed to GetThemeInternal() to resolve name conflict with Document.Theme property on Java.
        /// </dev>
        internal abstract Theme GetThemeInternal();

        internal InternManager InternManager;

        //Tree fields
        /// <summary>
        /// A special proxy node that serves as a parent for nodes that are not part of a document tree yet.
        /// </summary>
        private Node mNullNode;
#if CPLUSPLUS // alexnosk: Workaround for hanging nodes. Must be removed when CsToCppPorter.MemoryManagement.BindLifetime is used where required.
        /// <summary>
        /// In the code sometimes dummy nodes are created, which are not added to the document,
        /// such nodes are "disposed" in C++ code and cause NullReferenceException if try to use their children.
        /// To prevent this add all newly created nodes into collection to keep them while document is alive.
        /// </summary>
        private CodePorting.Translator.Cs2Cpp.MemoryManagement.ObjectsBag mHangingNodesCollection = new CodePorting.Translator.Cs2Cpp.MemoryManagement.ObjectsBag();
#endif
        private NodeChangingCallbackChain mNodeChangingCallbackChain;
        private int mTreeChangeCount;

        /// <summary>
        /// The identifier that will be assigned to next generated shape. Do not use this number directly when
        /// exporting, it is just a unique number, not necessarily valid from MS Word point of view.
        /// </summary>
        private int mNextShapeId;

        /// <summary>
        /// The identifier that will be assigned to next generated linked Dml textbox. Do not use this number directly when
        /// exporting, it is just a unique number, not necessarily valid from MS Word point of view.
        /// </summary>
        private int mNextDmlTextBoxId;

        /// <summary>
        /// The identifier that will be assigned to a next created comment/editableRange.
        /// Do not use this directly when exporting, it is just a unique number valid within the model.
        /// </summary>
        private int mNextAnnotationId;

        /// <summary>
        /// Handles uniqueness of Ids across SDTs in the document descendants. This applies both to Document and GlossaryDocument, as they both can host SDTs.
        /// </summary>
        private UniqueIdManager mSdtIdManager;

        /// <summary>
        /// Handles placeholders for SDTs in the document descendants. This applies both to Document and GlossaryDocument, as they both can host SDTs.
        /// </summary>
        private SdtPlaceholderManager mSdtPlaceholderManager;

        private bool mIsIgnoreLastSectionBreak;

        // Word document fields
        private FontInfoCollection mFontInfos;
        private StyleCollection mStyles;
        private ListCollection mLists;
        private HtmlBlockCollection mHtmlBlockCollection;
        private PersonCollectionInternal mPeople;
        private DocPr mDocPr;
        private VariableCollection mVariables;
        private FootnoteSeparatorCollection mFootnoteSeparators;
        /// <summary>
        /// Can be null.
        /// </summary>
        private Shape mBackgroundShape;
        private IResourceLoadingCallback mResourceLoadingCallback;
        private IWarningCallback mWarningCallback;
        private Document mTemporaryDoc;
        protected Dictionary<int, bool> mImageTransparencyFlagCache;

#if DEBUG
        /// <summary>
        /// For rendering's tests of building wrapping polygon only
        /// (see comments in Aspose.Words.Tests.Rendering.Shapes.TestWrapping).
        /// </summary>
        internal bool IsTestModeForTextWrapping
        {
            get { return mIsTestModeForTextWrapping; }
            set { mIsTestModeForTextWrapping = value; }
        }

        internal bool IsTestModeForBoundingBox
        {
            get { return mIsTestModeForBoundingBox; }
            set { mIsTestModeForBoundingBox = value; }
        }

        /// <summary>
        /// For WordArt rendering tests.
        /// </summary>
        internal bool IsTestModeForWordArt
        {
            get { return mIsTestModeForWordArt; }
            set { mIsTestModeForWordArt = value; }
        }

        private bool mIsTestModeForTextWrapping = false;
        private bool mIsTestModeForBoundingBox = false;
        private bool mIsTestModeForWordArt = false;
#endif

        private int mSuspendMappedCustomXmlUpdate;

        /// <summary>
        /// Value divides shape identifiers to different ranges.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MapDivider = 1024;
    }
}
