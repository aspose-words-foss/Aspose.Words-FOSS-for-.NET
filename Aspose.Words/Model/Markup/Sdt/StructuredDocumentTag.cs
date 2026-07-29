// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/06/2010 by Denis Darkin

using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Drawing;
using Aspose.JavaAttributes;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Notes;
using Aspose.Words.Revisions;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.Tables;
using Aspose.Words.Validation;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Represents a structured document tag (SDT or content control) in a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-content-control-sdt/">Structured Document Tags or Content Control</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>Structured document tags (SDTs) allow to embed customer-defined semantics as well as its
    /// behavior and appearance into a document.</para>
    ///
    /// <para>In this version Aspose.Words provides a number of public methods and properties to
    /// manipulate the behavior and content of <see cref="StructuredDocumentTag"/>.
    /// Mapping of SDT nodes to custom XML packages within a document can be performed with using
    /// the <see cref="XmlMapping"/> property.</para>
    ///
    /// <para><see cref="StructuredDocumentTag"/> can occur in a document in the following places:</para>
    /// <list type="bullet">
    /// <item>Block-level - Among paragraphs and tables, as a child of a <see cref="Body"/>, <see cref="HeaderFooter"/>,
    /// <see cref="Comment"/>, <see cref="Footnote"/> or a <see cref="Aspose.Words.Drawing.Shape"/> node.</item>
    /// <item>Row-level - Among rows in a table, as a child of a <see cref="Aspose.Words.Tables.Table"/> node.</item>
    /// <item>Cell-level - Among cells in a table row, as a child of a <see cref="Aspose.Words.Tables.Row"/> node.</item>
    /// <item>Inline-level - Among inline content inside, as a child of a <see cref="Paragraph"/>.</item>
    /// <item>Nested inside another <see cref="StructuredDocumentTag"/>.</item>
    /// </list>
    ///
    /// </remarks>
    [JavaGenericArguments("CompositeNode<Node>")]
    public class StructuredDocumentTag : CompositeNode, IMarkupNode, ITrackableNode, IRunAttrSource, IStructuredDocumentTag
    {
        /// <summary>
        /// Initializes a new instance of the <b>Structured document tag</b> class.
        /// </summary>
        /// <remarks>
        /// <para>The following types of SDT can be created:</para>
        /// <list type="bullet">
        /// <item><see cref="Markup.SdtType.Checkbox"/></item>
        /// <item><see cref="Markup.SdtType.DropDownList"/></item>
        /// <item><see cref="Markup.SdtType.ComboBox"/></item>
        /// <item><see cref="Markup.SdtType.Date"/></item>
        /// <item><see cref="Markup.SdtType.BuildingBlockGallery"/></item>
        /// <item><see cref="Markup.SdtType.Group"/></item>
        /// <item><see cref="Markup.SdtType.Picture"/></item>
        /// <item><see cref="Markup.SdtType.RichText"/></item>
        /// <item><see cref="Markup.SdtType.PlainText"/></item>
        /// </list>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        /// <param name="type">Type of SDT node.</param>
        /// <param name="level">Level of SDT node within the document.</param>
        public StructuredDocumentTag(DocumentBase doc, SdtType type, MarkupLevel level) : this(doc, level)
        {
            ControlProperties = Create(type, level);
            CreatePlaceholderIfNeeded();
            SdtContentHelper.InsertDefaultContent(this, false);
            mId = doc.SdtIdManager.GetUniqueId();
        }

        /// <summary>
        /// internal AW ctor.
        /// </summary>
        /// <remarks>
        /// By default this ctor does not define things like placeholder, id, control properties etc.
        /// This is the responsibility of the caller, so you should not use this ctor unless you know what you're doing.
        /// </remarks>
        internal StructuredDocumentTag(DocumentBase doc, MarkupLevel level) : base(doc)
        {
            mLevel = level;
            mControlProperties = new SdtEmpty();
            mContentsRunPr = new RunPr();
            mEndCharRunPr = new RunPr();
            mXmlMapping = new XmlMapping(this);
        }

        /// <summary>
        /// Sets the symbol used to represent the checked state of a check box content control.
        /// </summary>
        /// <param name="characterCode">The character code for the specified symbol.</param>
        /// <param name="fontName">The name of the font that contains the symbol.</param>
        /// <remarks>
        /// <para>Accessing this method will only work for <see cref="Markup.SdtType.Checkbox"/> SDT types.</para>
        /// <para>For all other SDT types exception will occur.</para>
        /// </remarks>
        public void SetCheckedSymbol(int characterCode, string fontName)
        {
            SetSymbol(characterCode, fontName, true);
        }

        /// <summary>
        /// Sets the symbol used to represent the unchecked state of a check box content control.
        /// </summary>
        /// <param name="characterCode">The character code for the specified symbol.</param>
        /// <param name="fontName">The name of the font that contains the symbol.</param>
        /// <remarks>
        /// <para>Accessing this method will only work for <see cref="Markup.SdtType.Checkbox"/> SDT types.</para>
        /// <para>For all other SDT types exception will occur.</para>
        /// </remarks>
        public void SetUncheckedSymbol(int characterCode, string fontName)
        {
            SetSymbol(characterCode, fontName, false);
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            StructuredDocumentTag lhs = (StructuredDocumentTag)base.Clone(isCloneChildren, cloningListener);

            lhs.mControlProperties = mControlProperties.Clone();

            // AM. Temporarily make link to parent SDT in simplest way.
            // Later we need to think how it can be refactored.
            if (lhs.mControlProperties is SdtDropDownList || lhs.mControlProperties is SdtComboBox)
                ((SdtDropDownListBase)lhs.mControlProperties).ListItems.SetParentSdt(lhs);

            lhs.mContentsRunPr = mContentsRunPr.Clone();
            lhs.mEndCharRunPr = mEndCharRunPr.Clone();

            if (mPlaceholder != null) // we reuse blocks, not clone.
                lhs.mPlaceholder = mPlaceholder;

            if (mId != UnusedSdtId) // happens during testing
                lhs.mId = lhs.Document.SdtIdManager.AddUniqueId(mId);

            lhs.mXmlMapping = mXmlMapping.Clone(lhs);

            lhs.mContentsFont = null; // created on demand

            return lhs;
        }

        /// <summary>
        /// Removes just this SDT node itself, but keeps the content of it inside the document tree.
        /// </summary>
        public void RemoveSelfOnly()
        {
            RemoveSelfOnly(true);
        }

        /// <summary>
        /// Removes just this SDT node itself, but keeps the content of it inside the document tree.
        /// </summary>
        internal void RemoveSelfOnly(bool isUpdateDataBoundContent)
        {
            // WORDSNET-13527 Update data bound content before removing this sdt.
            if (isUpdateDataBoundContent)
                XmlMapping.UpdateContent();

            NodeUtil.ResetDisplacedAnnotationReferences(this, true);

            CoreRemoveSelfOnly();
        }

        /// <include file='..\..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// Calls <see cref="DocumentVisitor.VisitStructuredDocumentTagStart"/>, then calls <see cref="Node.Accept"/> for all
        /// child nodes of the smart tag and calls <see cref="DocumentVisitor.VisitStructuredDocumentTagEnd"/> at the end.
        /// </remarks>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }

        /// <summary>
        /// Clears contents of this structured document tag and displays a placeholder if it is defined.
        /// </summary>
        /// <remarks>
        /// <para>It is not possible to clear contents of a structured document tag if it has revisions.</para>
        /// <para>If this structured document tag is mapped to custom XML (with using the <see cref="XmlMapping"/>
        /// property), the referenced XML node is cleared.</para>
        /// </remarks>
        public void Clear()
        {
            using (new SuspendMappedCustomXmlUpdateDocument(Document))
                SdtContentHelper.Clear(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the start of the StructuredDocumentTag.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitStructuredDocumentTagStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the StructuredDocumentTag.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitStructuredDocumentTagEnd(this);
        }

        /// <summary>
        /// Allows to insert inline-level, cell-level, row-level or block-level nodes depending on
        /// the value of the <see cref="Level"/> property.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            return NodeUtil.CanInsertIntoMarkupNode(this, newChild);
        }

        /// <summary>
        /// Update this SDT id to a unique value within document.
        /// Used in <see cref="DocumentPostLoader"/> to maintain validity of the model.
        /// </summary>
        internal void UpdateId()
        {
            if (mId == UnusedSdtId)
                mId = Document.SdtIdManager.GetUniqueId();
        }

        /// <summary>
        /// Internal setter that also validates uniqueness of this id across all SDTs in the parent document.
        /// </summary>
        internal void SetId(int id)
        {
            mId = Document.SdtIdManager.AddUniqueId(id);
        }

        internal void SetIdExplicitly(int id)
        {
            mId = id;
        }

        /// <summary>
        /// Finds placeholder BuildingBlock in the glossary of the document and stores reference to it for use in <see cref="Placeholder"/>.
        /// Creates placeholder if needed.
        /// </summary>
        /// <remarks>
        /// If forcePlaceholderLookup set to true, then we will find and attach a new placeholder
        /// even if mPlaceholder is not null which is used during import.
        /// </remarks>
        internal void UpdatePlaceholderReference(bool forcePlaceholderLookup)
        {
            // Find building block corresponding to this placeholder, if not found create a new one.
            if ((forcePlaceholderLookup || (mPlaceholder == null)) && StringUtil.HasChars(PlaceholderName))
                mPlaceholder = Document.SdtPlaceholderManager.FindPlaceholder(this, true);

            Document.SdtPlaceholderManager.IncrementReference(this);
        }

        /// <summary>
        /// Used  when we don't want to search for BuildingBlock.
        /// This happens during read operation when we don't yet have access to fully constructed glossary
        /// and thus can not use <see cref="PlaceholderName"/>
        /// </summary>
        internal void SetPlaceholderNameCore(string value)
        {
            if (StringUtil.HasChars(value))
                mPlaceholderName = value;
        }

        /// <summary>
        /// Sets the symbol used to represent the checked/unchecked state of a check box content control.
        /// </summary>
        private void SetSymbol(int characterCode, string fontName, bool isCheckedSymbol)
        {
            ValidateUsage(SdtType == SdtType.Checkbox,
                (isCheckedSymbol) ? ErrorCheckboxSetCheckedSymbol : ErrorCheckboxSetUncheckedSymbol);

            SdtCheckBox sdtCheckbox = (SdtCheckBox)ControlProperties;

            if (isCheckedSymbol)
                sdtCheckbox.CheckedStateInfo = new SdtCheckBoxStateInfo(fontName, characterCode);
            else
                sdtCheckbox.UncheckedStateInfo = new SdtCheckBoxStateInfo(fontName, characterCode);

            UpdateCheckboxContent(isCheckedSymbol);
        }

        /// <summary>
        /// Updates the checkbox content.
        /// </summary>
        private void UpdateCheckboxContent(bool isChecked)
        {
            SdtContentHelper.UpdateCheckboxContent(this);

            if ((XmlMapping == null) || XmlMapping.IsEmpty)
                return;

            string stringValue = XmlMapping.GetValue();

            // WORDSNET-20775 XmlMapping can be not null and not empty, but without related custom xml.
            // In this case we should skip it - impossible to update the non existing xml.
            if (stringValue == null)
                return;

            // Update data bound XML if value changed.
            bool boundValue = ((stringValue == "1") || (stringValue == "true"));

            if (boundValue != isChecked)
                XmlMapping.SetValue(isChecked ? "true" : "false");
        }

        /// <summary>
        /// Validates the usage.
        /// </summary>
        private void ValidateUsage(bool validCondition, string errorMessage)
        {
            AssertNonEmptyControlProperties();

            if (!validCondition)
                throw new InvalidOperationException(errorMessage);
        }

        /// <summary>
        /// Indicates that we support update of given SdtType.
        /// </summary>
        internal bool IsUpdateable
        {
            get
            {
                switch (SdtType)
                {
                    case SdtType.PlainText:
                    case SdtType.RichText:
                    case SdtType.Date:
                    case SdtType.DropDownList:
                    case SdtType.ComboBox:
                    case SdtType.Picture:
                    case SdtType.RepeatingSection:
                    case SdtType.Checkbox:
                        return true;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Returns true when the given SDT has to be ignored while updating.
        /// </summary>
        internal bool CanBeUpdated
        {
            get
            {
                // WORDSNET-20475 Do not update data bound SDT in AlternateContent i.e outside of Document.
                Node doc = GetAncestor(NodeType.Document);
                if (doc == null)
                    return false;

                return Document.NodeType != NodeType.GlossaryDocument;
            }
        }

        /// <summary>
        /// Returns sdtContent displayed text, hidden text and text marked for deletion are ignored.
        /// </summary>
        internal string VisibleText
        {
            get
            {
                StringBuilder visibleText = new StringBuilder();

                NodeCollection sdtContent = GetChildNodes(NodeType.Run, true);
                foreach (Run run in sdtContent)
                {
                    // Do not count hidden text.
                    if (InlineHelper.FetchAttr(run, FontAttr.Hidden) == AttrBoolEx.True)
                        continue;

                    // WORDSNET-24644 Do not count text marked for deletion.
                    if(run.IsDeleteRevision)
                        continue;

                    visibleText.Append(run.Text);
                }

                return visibleText.ToString();
            }
        }

        /// <summary>
        /// Sets the level at which this <b>SDT</b> occurs in the document tree.
        /// </summary>
        internal void SetLevel(MarkupLevel value)
        {
            StructuredDocumentTag sdtToCheck = new StructuredDocumentTag(Document, value);
            if ((ParentNode != null) && !ParentNode.CanInsert(sdtToCheck))
                throw new InvalidOperationException("Cannot change markup level.");

            NodeCollection nodes= GetChildNodes(NodeType.StructuredDocumentTag, true);

            foreach (StructuredDocumentTag sdt in nodes)
                CheckAndSet(value, sdtToCheck, sdt);

            // The same for this node.
            CheckAndSet(value, sdtToCheck, this);
        }

        private static void CheckAndSet(MarkupLevel value, StructuredDocumentTag sdtToCheck, StructuredDocumentTag sdt)
        {
            foreach (Node node in sdt.GetChildNodes(NodeType.Any, false))
            {
                if ((node.NodeType != NodeType.StructuredDocumentTag) && !sdtToCheck.CanInsert(node))
                    throw new InvalidOperationException("Cannot change markup level.");
            }

            sdt.mLevel = value;
        }

        /// <summary>
        /// Inserts default contents and sets the <see cref="IsShowingPlaceholderText"/> flag if the SDT is empty.
        /// </summary>
        internal void InsertDefaultContentIfEmpty()
        {
            if (!XmlMapping.IsEmpty)
                return;

            // Implemented for Block and Inline SDTs only now.
            Debug.Assert(Level == MarkupLevel.Inline || Level == MarkupLevel.Block);

            CompositeNode compositeChild = FirstNonMarkupCompositeDescendant;
            if (compositeChild != null)
            {
                // Exit if the SDT contains a non-empty paragraph or a table.
                if ((compositeChild != LastNonMarkupCompositeDescendant) ||
                    (compositeChild.FirstNonAnnotationChild != null) ||
                    (compositeChild.NodeType != NodeType.Paragraph))
                    return;
            }
            else
            {
                // Exit if the SDT contains inlines.
                if (HasNonMarkupNonAnnotationDescendant)
                    return;
            }

            IsShowingPlaceholderText = true;
            if ((Placeholder != null) || (SdtType == SdtType.Picture) || (SdtType == SdtType.Checkbox))
            {
                if (compositeChild != null)
                {
                    // Keep annotations.
                    InsertBefore(compositeChild.FirstChild, null, compositeChild);
                    compositeChild.Remove();
                }

                SdtContentHelper.InsertDefaultContent(this, true);
            }
            else
            {
                SdtContentHelper.InsertContentForEmptySdt(
                    (compositeChild != null) ? compositeChild : this,
                    ContentsRunPr,
                    (Level == MarkupLevel.Block) && (compositeChild == null),
                    EndCharacterRunPr);
            }
        }

        /// <summary>
        /// Checks that given range is valid to place ranged SDT.
        /// </summary>
        internal static bool IsRangeValid(Node rangeStart, Node rangeEnd)
        {
            // Make severe restrictions for range location for a while. Later we can weaken it if needed.

            if (rangeStart.ParentNode.NodeType != NodeType.Body)
                return false;

            if (rangeEnd.ParentNode.NodeType != NodeType.Body)
                return false;

            return true;
        }

        /// <summary>
        /// Converts all (includes <paramref name="node"/>) outer SDTs to ranged STDs.
        /// </summary>
        internal static void ConvertOuterSdtsToRanges(Node node)
        {
            List<StructuredDocumentTag> outerSdts = new List<StructuredDocumentTag>();

            // Collect all STD ancestors in topmost-first order.
            Node curNode = node;
            while (curNode != null)
            {
                if(curNode.NodeType == NodeType.StructuredDocumentTag)
                    outerSdts.Insert(0, (StructuredDocumentTag)curNode);

                curNode = curNode.ParentNode;
            }

            // And convert them all to ranged SDTs.
            foreach(StructuredDocumentTag sdt in outerSdts)
                sdt.ConvertToRange(sdt.LastChild, true);
        }

        /// <summary>
        /// Helper method to convert the SDT to ranged SDT.
        /// </summary>
        /// <remarks>
        /// This proposed to be public API but at first stage we just handle round-trip.
        /// </remarks>
        internal StructuredDocumentTagRangeStart ConvertToRange(Node refRangeEnd, bool isAfter)
        {
            StructuredDocumentTagRangeStart rangeStart = new StructuredDocumentTagRangeStart(Document, this);

            // Insert range right after original SDT.
            ParentNode.InsertAfter(rangeStart, this);

            // Move out content of original SDT.
            Node refNode = rangeStart;
            while (GetChildNodes(NodeType.Any, false).Count > 0)
                refNode = refNode.InsertNext(FirstChild);

            // Now original SDT can be removed from document tree.
            Remove();

            StructuredDocumentTagRangeEnd rangeEnd = new StructuredDocumentTagRangeEnd(rangeStart.Document, rangeStart.Id);
            refRangeEnd.ParentNode.Insert(rangeEnd, refRangeEnd, isAfter);

            return rangeStart;
        }

        /// <summary>
        /// Converts current inline SDT to block SDT.
        /// Moves content before and after STD to individual paragraphs if any.
        /// </summary>
        internal void ConvertToBlock()
        {
            Debug.Assert(Level == MarkupLevel.Inline);

            Paragraph parentParagraph = (Paragraph)GetAncestor(NodeType.Paragraph);

            // Check if any content before SDT.
            if (!IsFirstOrBookmarksBefore(this))
            {
                // Extract part before SDT to separate paragraph.
                Paragraph before = new Paragraph(parentParagraph.Document);
                while (!IsFirstChild)
                    before.AppendChild(parentParagraph.FirstChild);

                parentParagraph.InsertPrevious(before);
            }

            // Check if any content after SDT.
            if (!IsLastOrBookmarksAfter(this))
            {
                // Extract part after SDT to separate paragraph.
                Paragraph after = (Paragraph)parentParagraph.Clone(false);
                while (!IsLastChild)
                    after.AppendChild(NextSibling);

                parentParagraph.InsertNext(after);
            }

            // Here we can have only bookmarks before SDT, move them.
            while (!IsFirstChild)
                parentParagraph.InsertPrevious(parentParagraph.FirstChild);

            // Here we can have only bookmarks after SDT, move them.
            Node nodeAfter = parentParagraph;
            while (!IsLastChild)
                nodeAfter = parentParagraph.ParentNode.InsertAfter(NextSibling, nodeAfter);

            // Change SDT level, we have to remove SDT to do this.
            parentParagraph.InsertAfter(FirstChild, null, null);
            Remove();
            SetLevel(MarkupLevel.Block);

            parentParagraph.InsertPrevious(this);
            AppendChild(parentParagraph);
        }

        /// <summary>
        /// Checks that given SDT is a first child or only bookmarks are before.
        /// </summary>
        private static bool IsFirstOrBookmarksBefore(StructuredDocumentTag sdt)
        {
            if (sdt.IsFirstChild)
                return true;

            Node node = sdt.PreviousSibling;
            while (node != null)
            {
                if (!NodeUtil.IsBookmarkNode(node))
                    return false;

                node = node.PreviousSibling;
            }

            return true;
        }

        /// <summary>
        /// Checks that given SDT is a last child or only bookmarks are after.
        /// </summary>
        private static bool IsLastOrBookmarksAfter(StructuredDocumentTag sdt)
        {
            if (sdt.IsLastChild)
                return true;

            Node node = sdt.NextSibling;
            while (node != null)
            {
                if (!NodeUtil.IsBookmarkNode(node))
                    return false;

                node = node.NextSibling;
            }

            return true;
        }

        bool IStructuredDocumentTag.IsMultiSection
        {
            get {return false; }
        }

        Node IStructuredDocumentTag.Node
        {
            get { return this; }
        }

        /// <summary>
        /// Returns true if this SdtType can be created through public API, false otherwise.
        /// </summary>
        /// <remarks>
        /// DD: This method can be useful for public API, RK to review and include if needed.
        /// </remarks>
        private static bool IsTypeAllowedForCreation(SdtType type)
        {
            switch (type)
            {
                case SdtType.Checkbox:
                case SdtType.DropDownList:
                case SdtType.ComboBox:
                case SdtType.Date:
                case SdtType.BuildingBlockGallery:
                case SdtType.Group:
                case SdtType.Picture:
                case SdtType.RichText:
                case SdtType.PlainText:
                case SdtType.RepeatingSection:
                case SdtType.RepeatingSectionItem:
                case SdtType.Citation:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if this node type can be created at this markup level as per MS Word implementation.
        /// </summary>
        private static bool IsMarkupLevelAllowedForCreation(SdtType type, MarkupLevel level)
        {
            bool result;
            switch (level)
            {
                case MarkupLevel.Inline:
                    result =
                        (type != SdtType.RepeatingSection) &&
                        (type != SdtType.RepeatingSectionItem);
                    break;
                case MarkupLevel.Block:
                case MarkupLevel.Cell:
                    result = true;
                    break;
                case MarkupLevel.Row:
                    result =
                        (type == SdtType.RichText) ||
                        (type == SdtType.RepeatingSection) ||
                        (type == SdtType.RepeatingSectionItem) ||
                        (type == SdtType.Group);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("level");
            }

            return result;
        }

        /// <summary>
        /// If Sdt needs placeholder then request a placeholder (new or reused) from manager.
        /// </summary>
        private void CreatePlaceholderIfNeeded()
        {
            BuildingBlock placeholder = Document.SdtPlaceholderManager.FetchPlaceholderByType(SdtType);
            if (placeholder != null) // if SDT actually needs placeholder.
            {
                mPlaceholder = placeholder;
                mPlaceholderName = placeholder.Name;

                if (mLevel != MarkupLevel.Row) // for row-level SDT Word does not show placeholder.
                    mIsShowingPlcHdr = true;
            }
        }

        /// <summary>
        /// Used in all user exposed properties to verify existence of custom-property bag.
        /// </summary>
        private void AssertNonEmptyControlProperties()
        {
            if (ControlProperties == null)
                throw new InvalidOperationException(ErrorPleaseReport);
        }

        /// <summary>
        /// Creates control specific pr bags out of range allowed by <see cref="IsTypeAllowedForCreation"/>
        /// </summary>
        private SdtControlProperties Create(SdtType type, MarkupLevel level)
        {
            if (!IsTypeAllowedForCreation(type))
                throw new ArgumentException("Creation of such SdtType is not allowed.");

            if (!IsMarkupLevelAllowedForCreation(type, level))
                throw new ArgumentException("Can not create such SdtType at this level.");

            switch (type)
            {
                case SdtType.Checkbox:
                    return new SdtCheckBox();
                case SdtType.DropDownList:
                case SdtType.ComboBox:
                {
                    SdtDropDownListBase controlProperties = (type == SdtType.DropDownList)
                        ? (SdtDropDownListBase)new SdtDropDownList()
                        : new SdtComboBox();

                    controlProperties.ListItems.SetParentSdt(this);
                    return controlProperties;
                }
                case SdtType.Date:
                    return new SdtDate();
                case SdtType.BuildingBlockGallery:
                    return new SdtBuildingBlockGallery();
                case SdtType.Group:
                    return new SdtGroup();
                case SdtType.Picture:
                    return new SdtPicture();
                case SdtType.RichText:
                    return new SdtText(true);
                case SdtType.PlainText:
                    return new SdtText(false);
                case SdtType.RepeatingSection:
                    return new SdtRepeatingSection();
                case SdtType.RepeatingSectionItem:
                    return new SdtRepeatingSectionItem();
                case SdtType.Citation:
                    return new SdtCitation();
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        /// <summary>
        /// Returns either style itself if style is character or linked character style. Otherwise returns null.
        /// </summary>
        private static Style GetCharacterStyle(Style style)
        {
            if (style.Type == StyleType.Character)
                return style;

            Style linkedStyle = style.Styles.GetByIstd(style.LinkedIstd, false);

            if ((linkedStyle != null) && (linkedStyle.Type == StyleType.Character))
                return linkedStyle;

            return null;
        }

        /// <summary>
        /// Returns <see cref="NodeType.StructuredDocumentTag"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.StructuredDocumentTag; }
        }

        /// <summary>
        /// Gets the <see cref="BuildingBlock"/> containing placeholder text which should be displayed when this SDT run contents are empty,
        /// the associated mapped XML element is empty as specified via the <see cref="XmlMapping"/> element
        /// or the <see cref="IsShowingPlaceholderText"/> element is <c>true</c>.
        /// </summary>
        /// <remarks>Can be <c>null</c>, meaning that the placeholder is not applicable for this Sdt.</remarks>
        public BuildingBlock Placeholder
        {
            get { return mPlaceholder; }
        }

        /// <summary>
        /// <para>Gets or sets Name of the <see cref="BuildingBlock"/> containing placeholder text.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">Throw if BuildingBlock with this name <see cref="BuildingBlock.Name"/> is not present in <see cref="Document.GlossaryDocument"/>.</exception>
        public string PlaceholderName
        {
            get { return mPlaceholderName; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetPlaceholderNameCore(value);
                BuildingBlock attachedBlock = Document.SdtPlaceholderManager.FindPlaceholder(this, false);
                if (attachedBlock != null) // found placeholder in the glossary
                {
                    mPlaceholder = attachedBlock;

                    // populate content from custom placeholder to default text inside Sdt.
                    if (IsShowingPlaceholderText || XmlMapping.IsEmpty || !XmlMapping.IsValid)
                    {
                        // clean the contents and populate with new content from placeholder.
                        RemoveAllChildren();
                        SdtContentHelper.InsertDefaultContent(this, false);
                    }
                }
                else
                    throw new InvalidOperationException("BuildingBlock with such Name does not exist in the document glossary.");
            }
        }

        /// <summary>
        /// Gets the level at which this <b>SDT</b> occurs in the document tree.
        /// </summary>
        public MarkupLevel Level
        {
            get { return mLevel; }
        }

        /// <summary>
        /// Gets type of this <b>Structured document tag</b>.
        /// </summary>
        public SdtType SdtType
        {
            get
            {
                Debug.Assert(ControlProperties != null);
                return ControlProperties.Type;
            }
        }

        /// <summary>
        /// <para>Specifies a unique read-only persistent numerical Id for this <b>SDT</b>.</para>
        /// </summary>
        /// <remarks>
        /// <para> Id attribute shall follow these rules:
        /// <list type="bullet">
        /// <item>The document shall retain SDT ids only if the whole document is cloned <see cref="Document.Clone()"/>.</item>
        /// <item>During <see cref="DocumentBase.ImportNode(Aspose.Words.Node,bool)"/>
        /// Id shall be retained if import does not cause conflicts with other SDT Ids in
        /// the target document.</item>
        /// <item>
        /// If multiple SDT nodes specify the same decimal number value for the Id attribute,
        /// then the first SDT in the document shall maintain this original Id,
        /// and all subsequent SDT nodes shall have new identifiers assigned to them when the document is loaded.
        /// </item>
        /// <item>During standalone SDT <see cref="StructuredDocumentTag.Clone"/> operation new unique ID will be generated for the cloned SDT node.</item>
        /// <item>
        /// If Id is not specified in the source document, then the SDT node shall have a new unique identifier assigned
        /// to it when the document is loaded.
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        public int Id
        {
            get
            {
                Debug.Assert(mId != UnusedSdtId);
                return mId;
            }
        }

        /// <summary>
        /// When set to <c>true</c>, this property will prohibit a user from deleting this <b>SDT</b>.
        /// </summary>
        public bool LockContentControl
        {
            get { return mLockContentControl; }
            set { mLockContentControl = value; }
        }

        /// <summary>
        /// When set to <c>true</c>, this property will prohibit a user from editing the contents of this <b>SDT</b>.
        /// </summary>
        public bool LockContents
        {
            get { return mLockContents; }
            set { mLockContents = value; }
        }

        /// <summary>
        /// <para>
        /// Specifies whether the content of this <b>SDT</b> shall be interpreted to contain placeholder text
        /// (as opposed to regular text contents within the SDT).
        /// </para>
        /// <para>
        /// if set to <c>true</c>, this state shall be resumed (showing placeholder text) upon opening this document.
        /// </para>
        /// </summary>
        public bool IsShowingPlaceholderText
        {
            get { return mIsShowingPlcHdr; }
            set { mIsShowingPlcHdr = value; }
        }

        /// <summary>
        /// Specifies a tag associated with the current SDT node.
        /// Can not be <c>null</c>.
        /// </summary>
        /// <remarks> A tag is an arbitrary string which applications can associate with SDT
        ///  in order to identify it without providing a visible friendly name.</remarks>
        public string Tag
        {
            get { return mTag; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mTag = value;
            }
        }

        /// <summary>
        /// Font formatting that will be applied to text entered into <b>SDT</b>.
        /// </summary>
        public Font ContentsFont
        {
            get
            {
                if (mContentsFont == null)
                    mContentsFont = new Font(this, Document);

                return mContentsFont;
            }
        }

        /// <summary>
        /// Font formatting that will be applied to the last character of text entered into <b>SDT</b>.
        /// </summary>
        public Font EndCharacterFont
        {
            get
            {
                if (mEndCharacterFont == null)
                    mEndCharacterFont = new Font(EndCharacterRunPr, Document);

                return mEndCharacterFont;
            }
        }

        /// <summary>
        /// Specifies whether this <b>SDT</b> shall be removed from the WordProcessingML document when its contents
        /// are modified.
        /// </summary>
        public bool IsTemporary
        {
            get { return mIsTemporary; }
            set { mIsTemporary = value; }
        }

        /// <summary>
        /// Specifies the friendly name associated with this <b>SDT</b>.
        /// Can not be <c>null</c>.
        /// </summary>
        public string Title
        {
            get { return mTitle; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mTitle = value;
            }
        }

        /// <summary>
        /// Gets <see cref="SdtListItemCollection"/> associated with this <b>SDT</b>.
        /// </summary>
        /// <remarks>
        /// <para>Accessing this property will only work for <see cref="Markup.SdtType.ComboBox"/> or <see cref="Markup.SdtType.DropDownList"/>
        /// SDT types.
        /// </para>
        /// <para>For all other SDT types exception will occur.</para>
        /// </remarks>
        public SdtListItemCollection ListItems
        {
            get
            {
                ValidateUsage((SdtType == SdtType.ComboBox) || (SdtType == SdtType.DropDownList), ErrorListItems);
                return ((SdtDropDownListBase)ControlProperties).ListItems;
            }
        }

        /// <summary>
        /// Gets/Sets current state of the Checkbox <b>SDT</b>.
        /// Default value for this property is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>Accessing this property will only work for <see cref="Markup.SdtType.Checkbox"/>
        /// SDT types.
        /// </para>
        /// <para>For all other SDT types exception will occur.</para>
        /// </remarks>
        public bool Checked
        {
            get
            {
                ValidateUsage(SdtType == SdtType.Checkbox, ErrorCheckboxChecked);
                return ((SdtCheckBox)ControlProperties).Checked;
            }
            set
            {
                ValidateUsage(SdtType == SdtType.Checkbox, ErrorCheckboxChecked);
                ((SdtCheckBox)ControlProperties).Checked = value;

                UpdateCheckboxContent(value);
            }
        }

        /// <summary>
        /// Gets/sets the appearance of a structured document tag.
        /// </summary>
        public SdtAppearance Appearance
        {
            get { return mAppearance; }
            set
            {
                mAppearance = value;

                SdtContentUpdater.UpdateNonBoundDataContent(this);
            }
        }

        /// <summary>
        /// Sets appearance without updating SDT content.
        /// </summary>
        internal void SetAppearanceInternal(SdtAppearance appearance)
        {
            mAppearance = appearance;
        }

        /// <summary>
        /// Gets current state of the content of the Checkbox <b>SDT</b>.
        /// </summary>
        /// <remarks>
        /// <para>Accessing this property will only work for <see cref="Markup.SdtType.Checkbox"/> SDT types.</para>
        /// <para>For all other SDT types exception will occur.</para>
        /// </remarks>
        internal bool ContentChecked
        {
            get
            {
                ValidateUsage(SdtType == SdtType.Checkbox, ErrorCheckboxContentChecked);

                Run firstRun = GetChild(NodeType.Run, 0, true) as Run;
                if (firstRun != null)
                {
                    string characterText = firstRun.Text;
                    if (characterText.Length == 1)
                    {
                        int characterCode = characterText[0];
                        SdtCheckBox checkBox = (SdtCheckBox)ControlProperties;

                        if (characterCode == checkBox.CheckedStateInfo.CharacterCode)
                            return true;

                        if (characterCode == checkBox.UncheckedStateInfo.CharacterCode)
                            return false;
                    }
                }

                // AM. I think it'd better do not throw here.
                return false;
            }
        }

        /// <summary>
        /// Allows to set/get the language format for the date displayed in this <b>SDT</b>.
        /// </summary>
        /// <remarks>
        /// <para>Accessing this property will only work for <see cref="Markup.SdtType.Date"/> SDT type.
        /// </para>
        /// <para>For all other SDT types exception will occur.</para>
        /// </remarks>
        public int DateDisplayLocale
        {
            get
            {
                ValidateUsage(SdtType == SdtType.Date, ErrorDateDisplayLocale);
                return ((SdtDate)ControlProperties).Lid;
            }
            set
            {
                ValidateUsage(SdtType == SdtType.Date, ErrorDateDisplayLocale);
                ((SdtDate)ControlProperties).Lid = value;

                SdtContentUpdater.UpdateNonBoundDataContent(this);
            }
        }

        /// <summary>
        /// String that represents the format in which dates are displayed.
        /// </summary>
        /// <remarks>
        /// <para>Can not be <c>null</c>.</para>
        /// <para>The dates for English (U.S.) is "mm/dd/yyyy"</para>
        /// <para>Accessing this property will only work for <see cref="Markup.SdtType.Date"/> SDT type.</para>
        /// <para>For all other SDT types exception will occur.</para>
        /// </remarks>
        public string DateDisplayFormat
        {
            get
            {
                ValidateUsage(SdtType == SdtType.Date, ErrorDateDisplayFormat);
                return ((SdtDate)ControlProperties).DateFormat;
            }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                ValidateUsage(SdtType == SdtType.Date, ErrorDateDisplayFormat);
                ((SdtDate)ControlProperties).DateFormat = value;

                SdtContentUpdater.UpdateNonBoundDataContent(this);
            }
        }

        /// <summary>
        /// Specifies the full date and time last entered into this <b>SDT</b>.
        /// </summary>
        /// <remarks>
        /// <para>Accessing this property will only work for <see cref="Markup.SdtType.Date"/> SDT type.
        /// </para>
        /// <para>For all other SDT types exception will occur.</para>
        /// </remarks>
        public DateTime FullDate
        {
            get
            {
                ValidateUsage(SdtType == SdtType.Date, ErrorFullDate);
                return ((SdtDate)ControlProperties).FullDate;
            }
            set
            {
                ValidateUsage(SdtType == SdtType.Date, ErrorFullDate);
                ((SdtDate)ControlProperties).FullDate = value;

                SdtContentUpdater.UpdateNonBoundDataContent(this);
            }
        }

        /// <summary>
        /// Gets/sets format in which the date for a date SDT is stored when the <b>SDT</b> is bound to an XML node in the document's data store.
        /// Default value is <see cref="SdtDateStorageFormat.DateTime"/>
        /// </summary>
        /// <remarks>
        /// <para>Accessing this property will only work for <see cref="Markup.SdtType.Date"/> SDT type.
        /// </para>
        /// <para>For all other SDT types exception will occur.</para>
        /// </remarks>
        public SdtDateStorageFormat DateStorageFormat
        {
            get
            {
                ValidateUsage(SdtType == SdtType.Date, ErrorDateStorageFormat);
                return ((SdtDate)ControlProperties).StoreMappedDataAs;
            }
            set
            {
                ValidateUsage(SdtType == SdtType.Date, ErrorDateStorageFormat);
                ((SdtDate)ControlProperties).StoreMappedDataAs = value;

                SdtContentUpdater.UpdateNonBoundDataContent(this);
            }
        }

        /// <summary>
        /// Specifies the type of calendar for this <b>SDT</b>.
        /// Default is <see cref="SdtCalendarType.Default"/>
        /// </summary>
        /// <remarks>
        /// <para>Accessing this property will only work for <see cref="Markup.SdtType.Date"/> SDT type.
        /// </para>
        /// <para>For all other SDT types exception will occur.</para>
        /// </remarks>
        public SdtCalendarType CalendarType
        {
            get
            {
                ValidateUsage(SdtType == SdtType.Date, ErrorCalendarType);
                return ((SdtDate)ControlProperties).CalendarType;
            }
            set
            {
                ValidateUsage(SdtType == SdtType.Date, ErrorCalendarType);
                ((SdtDate)ControlProperties).CalendarType = value;

                SdtContentUpdater.UpdateNonBoundDataContent(this);
            }
        }

        /// <summary>
        /// Specifies type of building block for this <b>SDT</b>.
        /// Can not be <c>null</c>.
        /// </summary>
        /// <remarks>
        /// <para>Accessing this property will only work for <see cref="Markup.SdtType.BuildingBlockGallery"/> and
        /// <see cref="Markup.SdtType.DocPartObj"/> SDT types. It is read-only for <b>SDT</b> of the document part type.
        /// </para>
        /// <para>For all other SDT types exception will occur.</para>
        /// </remarks>
        public string BuildingBlockGallery
        {
            get
            {
                // WORDSNET-15972 Provide ability to obtain document part gallery filter from "docPartObj" SDT.
                ValidateUsage(IsSdtDocPart, ErrorBuildingBlockType);
                return ((SdtDocPart)ControlProperties).BuildingBlockType;
            }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                ValidateUsage(SdtType == SdtType.BuildingBlockGallery, ErrorBuildingBlockType);
                ((SdtDocPart)ControlProperties).BuildingBlockType = value;
            }
        }

        /// <summary>
        /// Specifies category of building block for this <b>SDT</b> node.
        /// Can not be <c>null</c>.
        /// </summary>
        /// <remarks>
        /// <para>Accessing this property will only work for <see cref="Markup.SdtType.BuildingBlockGallery"/> and
        /// <see cref="Markup.SdtType.DocPartObj"/> SDT types. It is read-only for <b>SDT</b> of the document part type.
        /// </para>
        /// <para>For all other SDT types exception will occur.</para>
        /// </remarks>
        public string BuildingBlockCategory
        {
            get
            {
                ValidateUsage(IsSdtDocPart, ErrorBuildingBlockCategory);
                return ((SdtDocPart)ControlProperties).BuildingBlockCategory;
            }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                ValidateUsage(SdtType == SdtType.BuildingBlockGallery, ErrorBuildingBlockCategory);
                ((SdtDocPart)ControlProperties).BuildingBlockCategory = value;
            }
        }

        /// <summary>
        /// Specifies whether this <b>SDT</b> allows multiple lines of text.
        /// </summary>
        /// <remarks>
        /// <para>Accessing this property will only work for <see cref="Markup.SdtType.RichText"/> and <see cref="Markup.SdtType.PlainText"/> SDT type.
        /// </para>
        /// <para>For all other SDT types exception will occur.</para>
        /// </remarks>
        public bool Multiline
        {
            get
            {
                ValidateUsage((SdtType == SdtType.RichText) || (SdtType == SdtType.PlainText), ErrorMultiline);
                return ((SdtText)ControlProperties).IsMultiline;
            }
            set
            {
                ValidateUsage((SdtType == SdtType.RichText) || (SdtType == SdtType.PlainText), ErrorMultiline);
                ((SdtText)ControlProperties).IsMultiline = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the structured document tag.
        /// </summary>
        public System.Drawing.Color Color
        {
            get
            {
                return (BaseColor != null) ? BaseColor.ToNativeColor(): (DrColor.Empty).ToNativeColor();
            }
            set
            {
                BaseColor = DrColor.FromNativeColor(value);
            }
        }

        /// <summary>
        /// Gets or sets the Style of the structured document tag.
        /// </summary>
        /// <remarks>
        /// Only <see cref="StyleType.Character" /> style or <see cref="StyleType.Paragraph" /> style with linked character style can be set.
        /// </remarks>
        /// <dev>
        /// It's ridiculous that Word uses character styles internally but exposes
        /// linked paragraph styles in GUI and VBA. Anyway, we do the same.
        /// </dev>
        public Style Style
        {
            get
            {
                Style style = Document.Styles.FetchByIstd(mContentsRunPr.Istd, StyleIndex.DefaultParagraphFont);
                if (style.Istd == StyleIndex.DefaultParagraphFont)
                    return style;

                Style linkedStyle = style.Styles.GetByIstd(style.LinkedIstd, false);

                if (linkedStyle != null && linkedStyle.Type == StyleType.Paragraph)
                    return linkedStyle;

                if (linkedStyle == null && style.Type == StyleType.Character)
                    return style;

                return null;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value.Document != Document)
                    throw new ArgumentException("This style belongs to a different document.");

                Style characterStyle = GetCharacterStyle(value);
                if (characterStyle == null)
                    throw new InvalidOperationException(
                        "Cannot apply this style to the SDT: Style should be a character or a linked one.");

                int styleIstd = characterStyle.Istd;

                // Setting DefaultParagraphFont in Word removes the info about the Style.
                if (characterStyle.Istd == StyleIndex.DefaultParagraphFont)
                    styleIstd = StyleIndex.Nil;

                mContentsRunPr.Istd = styleIstd;
            }
        }

        /// <summary>
        /// Gets or sets the name of the style applied to the structured document tag.
        /// </summary>
        public string StyleName
        {
            get { return Style.Name; }
            set { Style = Document.Styles.FetchByName(value); }
        }

        /// <summary>
        /// Gets/sets the color on which to base the visual elements of a structured document tag.
        /// </summary>
        internal DrColor BaseColor
        {
            get { return mBaseColor; }
            set { mBaseColor = value; }
        }

        /// <summary>
        /// Gets/sets the theme color on which to base the visual elements of a structured document tag.
        /// </summary>
        internal string ThemeColor
        {
            get { return mThemeColor; }
            set { mThemeColor = value; }
        }

        /// <summary>
        /// Gets/sets the shade value applied to the supplied <see cref="ThemeColor"/>.
        /// </summary>
        internal string ThemeShade
        {
            get { return mThemeShade; }
            set { mThemeShade = value; }
        }

        /// <summary>
        /// Gets/sets the tint value applied to the supplied <see cref="ThemeColor"/>.
        /// </summary>
        internal string ThemeTint
        {
            get { return mThemeTint; }
            set { mThemeTint = value; }
        }

        /// <summary>
        /// Specifies that the parent structured document tag is a repeated section.
        /// </summary>
        internal bool IsRepeatingSection
        {
            get { return (SdtType == SdtType.RepeatingSection); }
        }

        /// <summary>
        /// Specifies that the parent structured document tag is a repeated section item.
        /// </summary>
        internal bool IsRepeatingSectionItem
        {
            get { return (SdtType == SdtType.RepeatingSectionItem); }
        }

        /// <summary>
        /// Specifies a relationship between the structured document tag and an Office Web Extension (webExtension):
        /// if the structured document tag was created by, and/or is bound to, at least one webExtension.
        /// </summary>
        /// <dev>
        /// §2.5.1.12 webExtensionCreated and §2.5.1.13 webExtensionLinked of [MS-DOCX].
        /// </dev>
        internal SdtWebExtensionRelationship WebExtensionRelationship
        {
            get { return mWebExtensionRelationship; }
            set { mWebExtensionRelationship = value; }
        }

        /// <summary>
        /// Specifies the label identifier associated with the current structured document tag.
        /// </summary>
        /// <remarks>
        /// The contents of the structured document tag resolved by a Label unique identifier
        /// shall be used as the label content.</remarks>
        internal int Label
        {
            get { return mLabel; }
            set
            {
                mLabel = value;
                mIsLabelDefined = true;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if <see cref="Label"/> is defined for this <b>SDT</b>.
        /// </summary>
        internal bool IsLabelDefined
        {
            get { return mIsLabelDefined; }
        }

        /// <summary>
        /// Specifies the position of the current structured document tag in the navigation (tab) order used in the document.
        /// </summary>
        /// <remarks>
        /// <para>TabIndex shall is analogous to the tabIndex attribute in HTML.</para>
        /// <para>Objects that support tab index shall be navigated by consumers in the following order:
        /// <list type="bullet">
        /// <item><description>
        /// Objects for which the XML specifies a non-zero TabIndex value are navigated first.
        /// Navigation proceeds with the element with the lowest resolved value of tabIndex to the element with the highest
        /// resolved value of tabIndex.
        /// </description></item>
        /// <item><description>
        /// Objects that specify identical resolved values of TabIndex is navigated in the lexical order in which
        /// the elements appear in the underlying WordprocessingML.
        /// </description></item>
        /// <item><description>
        /// Objects for which the XML does not specify an index or objects for which the XML specifies a resolved
        /// TabIndex value of 0 are navigated last. These objects are navigated in the lexical order in which they
        /// appear in the underlying WordprocessingML.
        /// </description></item>
        /// </list>
        /// </para>
        /// </remarks>
        internal int TabIndex
        {
            get { return mTabIndex; }
            set { mTabIndex = value; }
        }

        /// <summary>
        /// Run Properties For Structured Document Tag Contents. Specifies the set of run properties which shall be applied
        /// to the text entered into the parent structured document tag in replacement of placeholder text.
        /// </summary>
        internal RunPr ContentsRunPr
        {
            get { return mContentsRunPr; }
            set { mContentsRunPr = value; }
        }

        /// <summary>
        /// This element specifies the set of run properties which shall be applied to the character present to delimit the
        /// end of the structured document tag's contents. When these properties are applied, they shall be applied in
        /// addition to the run properties specified for the entire structured document tag via the <see cref="ContentsRunPr"/>
        /// element stored in the tag's main property container.
        /// </summary>
        internal RunPr EndCharacterRunPr
        {
            get { return mEndCharRunPr; }
            set { mEndCharRunPr = value; }
        }

        /// <summary>
        /// Gets an object that represents the mapping of this structured document tag to XML data
        /// in a custom XML part of the current document.
        /// </summary>
        /// <remarks>
        /// You can use the <see cref="Markup.XmlMapping.SetMapping(CustomXmlPart,string,string)"/> method of this object to map
        /// a structured document tag to XML data.
        /// </remarks>
        /// <dev>
        /// If this element is present and the parent Sdt is not of a rich text type, then the current
        /// value of the Sdt shall be determined by finding the XML element (if any) which is
        /// determined by the attributes on this element.
        /// See Iso29500, chapter 1, 17.5.2.6 dataBinding (XML Mapping).
        /// If DataBinding information does not result in an XML element, then the
        /// application can use any algorithm desired to find the closest available match. If this information does result in an
        /// XML element, then the contents of that element shall be used to replace the current run content within the
        /// document.
        /// </dev>
        public XmlMapping XmlMapping
        {
            get { return mXmlMapping; }
        }

        /// <summary>
        /// Gets a string that represents the XML contained within the node in the <see cref="SaveFormat.FlatOpc"/> format.
        /// </summary>
        public string WordOpenXML
        {
            get
            {
                OpcDocumentFragmentWriter writer = new OpcDocumentFragmentWriter();
                return writer.SaveToString(this);
            }
        }

        /// <summary>
        /// Gets a string that represents the XML contained within the node in the <see cref="SaveFormat.FlatOpc"/> format.
        ///
        /// Unlike the <see cref="WordOpenXML"/> property, this method generates a stripped-down document that excludes any non-content-related parts.
        /// </summary>
        /// <dev>
        /// This is experimental API to understand uncertain needs of important customer.
        /// Later we could try rework it using Node.ToString() pattern.
        /// </dev>
        public string WordOpenXMLMinimal
        {
            get
            {
                OpcDocumentFragmentWriter writer = new OpcDocumentFragmentWriter(true);
                return writer.SaveToString(this);
            }
        }

        /// <summary>
        /// Encapsulates all differences between different type of SDT-based content controls.
        /// </summary>
        internal SdtControlProperties ControlProperties
        {
            get { return mControlProperties; }
            set { mControlProperties = value; }
        }

        /// <summary>
        /// Returns <c>true</c>, if content control was modified and should be updated.
        /// </summary>
        internal bool NeedToUpdateContent
        {
            get { return ((SdtType == SdtType.Date) && ((SdtDate)ControlProperties).NeedToUpdateContent); }
        }

        /// <summary>
        /// Returns <c>true</c>, if this SDT uses extensions defined in [MS-DOCX] Word Extensions to the Office Open XML
        /// (.docx) File Format.
        /// </summary>
        internal bool IsDocxExtension
        {
            get
            {
                return
                    (SdtType == SdtType.Checkbox) ||
                    (SdtType == SdtType.EntityPicker) ||
                    IsRepeatingSection ||
                    IsRepeatingSectionItem ||
                    (Appearance != SdtAppearance.Default) ||
                    (WebExtensionRelationship != SdtWebExtensionRelationship.None) ||
                    (BaseColor != null) ||
                    (ThemeColor != null) ||
                    XmlMapping.IsDocx15Extension;
            }
        }

        /// <summary>
        /// Adds an empty para to SDT content if needed to make document valid from MSW point of view.
        /// </summary>
        internal void EnsureCorrectLastChild()
        {
            Node lastChild = LastNonAnnotationChild;

            // WORDSNET-18147 A rare case when a SDT content's last child is a specific table.
            if ((lastChild != null) && (lastChild.NodeType == NodeType.Table))
            {
                Table table = (Table)lastChild;

                if ((table.Rows.Count > 0) && (table.LastRow.Cells.Count == 1) &&
                    (table.LastRow.FirstCell.CellPr.IsMergedToPrevious))
                {
                    AppendChild(new Paragraph(Document));
                }
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this SDT contains any non-markup (<see cref="NodeUtil.IsMarkupNode(Words.NodeType)"/>)
        /// non-annotation (<see cref="NodeUtil.IsCrossStructureAnnotation(Node)"/>) descendant.
        /// </summary>
        private bool HasNonMarkupNonAnnotationDescendant
        {
            get
            {
                Node node = NextPreOrder(this);
                while ((node != null) &&
                    (NodeUtil.IsMarkupNode(node) || NodeUtil.IsCrossStructureAnnotation(node)))
                    node = node.NextPreOrder(this);

                return node != null;
            }
        }

        MarkupLevel IMarkupNode.Level_IMarkupNode
        {
            get { return Level; }
        }

        EditRevision ITrackableNode.InsertRevision
        {
            get { return ContentsRunPr.InsertRevision; }
            set { ContentsRunPr.InsertRevision = value; }
        }

        EditRevision ITrackableNode.DeleteRevision
        {
            get { return ContentsRunPr.DeleteRevision; }
            set { ContentsRunPr.DeleteRevision = value; }
        }

        MoveRevision IMoveTrackableNode.MoveFromRevision
        {
            get { return ContentsRunPr.MoveFromRevision; }
            set { ContentsRunPr.MoveFromRevision = value; }
        }

        MoveRevision IMoveTrackableNode.MoveToRevision
        {
            get { return ContentsRunPr.MoveToRevision; }
            set { ContentsRunPr.MoveToRevision = value; }
        }

        void IMoveTrackableNode.RemoveMoveRevisions()
        {
            ContentsRunPr.Remove(RevisionAttr.MoveFromRevision);
            ContentsRunPr.Remove(RevisionAttr.MoveToRevision);
        }

        /// <summary>
        /// Indicates that current SDT is the document part type or the document part gallery type.
        /// </summary>
        internal bool IsSdtDocPart
        {
            get { return (SdtType == SdtType.BuildingBlockGallery) || (SdtType == SdtType.DocPartObj); }
        }

        /// <summary>
        /// Returns value of GetText() method without breaks.
        /// </summary>
        internal string ContentValue
        {
            get { return GetText().Trim(gBreaks); }
        }

        object IRunAttrSource.GetDirectRunAttr(int key)
        {
            return mContentsRunPr[key];
        }

        object IRunAttrSource.GetDirectRunAttr(int key, RevisionsView revisionsView)
        {
            return mContentsRunPr.GetDirectAttr(key, revisionsView);
        }

        object IRunAttrSource.FetchInheritedRunAttr(int key)
        {
            return mContentsRunPr.FetchInheritedAttr(key);
        }

        void IRunAttrSource.SetRunAttr(int key, object value)
        {
            mContentsRunPr.SetAttr(key, value);

            // WORDSNET-15602 Sets attribute for both RunPr collections - SDT and SDT Content.
            if (SdtType == SdtType.Checkbox)
            {
                SetCheckboxRunAttr(key, value);
            }
        }

        void IRunAttrSource.RemoveRunAttr(int key)
        {
            mContentsRunPr.Remove(key);
        }

        void IRunAttrSource.ClearRunAttrs()
        {
            mContentsRunPr.Clear();
        }

        private void SetCheckboxRunAttr(int key, object value)
        {
            Run curRun = (Run)GetChild(NodeType.Run, 0, true);
            if (curRun == null)
            {
                SdtContentHelper.InsertDefaultContent(this, false);
                curRun = (Run)GetChild(NodeType.Run, 0, true);
            }
            curRun.RunPr.SetAttr(key, value);
            SdtCheckBox checkBox = (SdtCheckBox)ControlProperties;
            curRun.RunPr.Name = checkBox.Checked ?
                checkBox.CheckedStateInfo.FontName :
                checkBox.UncheckedStateInfo.FontName;
        }

        private string mPlaceholderName;

        private MarkupLevel mLevel;
        private SdtControlProperties mControlProperties;
        private XmlMapping mXmlMapping;

        /// <summary>
        /// See Iso29500, chapter 1, 17.5.2.19 label (Structured Document Tag Label)
        /// </summary>
        private int mLabel;
        private bool mIsLabelDefined;

        /// <summary>
        /// See Iso29500, chapter 1, 17.5.2.18 id (Unique ID)
        /// </summary>
        private int mId = UnusedSdtId;

        /// <summary>
        /// See Iso29500, chapter 1, 17.5.2.1 alias (Friendly Name)
        /// </summary>
        private string mTitle = "";

        /// <summary>
        /// See Iso29500, chapter 1, 17.5.2.39 showingPlcHdr (Current Contents Are Placeholder Text)
        /// </summary>
        private bool mIsShowingPlcHdr;

        /// <summary>
        /// See Iso29500, chapter 1, 17.5.2.41 tabIndex (Structured Document Tag Navigation Order Index)
        /// </summary>
        private int mTabIndex;

        /// <summary>
        /// See Iso29500, chapter 1, 17.5.2.42 tag (Programmatic Tag)
        /// </summary>
        private string mTag = "";

        /// <summary>
        /// See Iso29500, chapter 1, 17.5.2.43 temporary (Remove Structured Document Tag When Contents Are Edited)
        /// </summary>
        private bool mIsTemporary;

        /// <summary>
        /// See Iso29500, chapter 1, 17.5.2.27 rPr (Run Properties For Structured Document Tag Contents)
        /// </summary>
        private RunPr mContentsRunPr;

        /// <summary>
        ///  See Iso29500, chapter 1, 17.5.2.28 rPr (Structured Document Tag End Character Run Properties)
        /// </summary>
        private RunPr mEndCharRunPr;

        /// <summary>
        ///  See Iso29500, chapter 1, 17.5.2.25 placeholder (Structured Document Tag Placeholder Text)
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private BuildingBlock mPlaceholder;

        private bool mLockContentControl;

        private bool mLockContents;

        private Font mContentsFont;

        private Font mEndCharacterFont;

        private DrColor mBaseColor;
        private string mThemeColor;
        private string mThemeShade;
        private string mThemeTint;

        private SdtAppearance mAppearance;
        private SdtWebExtensionRelationship mWebExtensionRelationship;

        internal const string CoverPageBuildingBlockGallery = "Cover Pages";

        /// <summary>
        /// By default we will use this as indicator that Id has not been set for SDT Id <see cref="Id"/>.
        /// </summary>
        /// <remarks>There </remarks>
        private const int UnusedSdtId = -1;

        private const string SDTType = " SDT type.";
        private const string SDTTypes = " SDT types.";
        private const string AccessibleFor = " is only accessible for ";
        private const string ErrorDate = AccessibleFor + "Date" + SDTType;
        private const string ErrorCheckbox = AccessibleFor + "Checkbox" + SDTType;
        private const string ErrorBuildingBlock = AccessibleFor + "BuildingBlockGallery" + SDTType;

        private const string ErrorFullDate = "FullDate" + ErrorDate;
        private const string ErrorCalendarType = "CalendarType" + ErrorDate;
        private const string ErrorDateDisplayLocale = "DateDisplayLocale" + ErrorDate;
        private const string ErrorDateDisplayFormat = "DateDisplayFormat" + ErrorDate;
        private const string ErrorDateStorageFormat = "DateStorageFormat" + ErrorDate;
        private const string ErrorBuildingBlockType = "BuildingBlockType" + ErrorBuildingBlock;
        private const string ErrorBuildingBlockCategory = "BuildingBlockCategory" + ErrorBuildingBlock;
        private const string ErrorCheckboxChecked = "Checked" + ErrorCheckbox;
        private const string ErrorCheckboxContentChecked = "ContentChecked" + ErrorCheckbox;
        private const string ErrorCheckboxSetCheckedSymbol = "SetCheckedSymbol" + ErrorCheckbox;
        private const string ErrorCheckboxSetUncheckedSymbol = "SetUncheckedSymbol" + ErrorCheckbox;

        private const string ErrorListItems = "ListItems" + AccessibleFor + "ComboBox or DropDownList" + SDTTypes;
        private const string ErrorMultiline = "Multiline" + AccessibleFor + "Richtext and Plaintext" + SDTTypes;

        private const string ErrorPleaseReport = "Please report exception.";

#if DEBUG
        public override string ToString()
        {
            return String.Format("{0} <{1}:{2}>", base.ToString(), Level, Tag);
        }
#endif

        private static readonly char[] gBreaks = new char[]
            { ControlChar.CellChar, ControlChar.ParagraphBreakChar, ControlChar.SectionBreakChar };
    }
}
