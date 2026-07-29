// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/01/2004 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Aspose.Bidi;
using Aspose.JavaAttributes;
using Aspose.Words.Fields.Expressions;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a Microsoft Word document field.
    /// <para>To learn more, visit the <ms><see href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</see></ms><java><a href="https://docs.aspose.com/words/java/working-with-fields/">Working with Fields</a></java> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>A field in a Word document is a complex structure consisting of multiple nodes that include field start,
    /// field code, field separator, field result and field end. Fields can be nested, contain rich content and span
    /// multiple paragraphs or sections in a document. The <see cref="Field"/> class is a "facade" object that provides
    /// properties and methods that allow to work with a field as a single object. </para>
    ///
    /// <para>The <see cref="Start"/>, <see cref="Separator"/> and <see cref="End"/> properties point to the
    /// field start, separator and end nodes of the field respectively.</para>
    ///
    /// <para>The content between the field start and separator is the field code. The content between the
    /// field separator and field end is the field result. The field code typically consists of one or more
    /// <see cref="Run"/> objects that specify instructions. The processing application is expected to execute
    /// the field code to calculate the field result.</para>
    ///
    /// <para>The process of calculating field results is called the field update. Aspose.Words can update field
    /// results of most of the field types in exactly the same way as Microsoft Word does it. Most notably,
    /// Aspose.Words can calculate results of even the most complex formula fields. To calculate the field
    /// result of a single field use the <see cref="Update()"/> method. To update fields in the whole document
    /// use <see cref="Aspose.Words.Document.UpdateFields"/>.</para>
    ///
    /// <para>You can get the plain text version of the field code using the <see cref="GetFieldCode(bool)"/> method.
    /// You can get and set the plain text version of the field result using the <see cref="Result"/> property.
    /// Both the field code and field result can contain complex content, such as nested fields, paragraphs, shapes,
    /// tables and in this case you might want to work with the field nodes directly if you need more control.</para>
    ///
    /// <para>You do not create instances of the <see cref="Field"/> class directly.
    /// To create a new field use the <see cref="DocumentBuilder.InsertField(string)"/> method.</para>
    ///
    /// </remarks>
    public class Field
    {
        protected Field()
        {
        }

        internal Field(FieldBundle bundle)
        {
            mBundle = bundle;
        }

        /// <summary>
        /// Initializes the object. Two stage construction because makes it simpler to implement a field factory.
        /// </summary>
        internal virtual void Initialize(FieldStart start, FieldSeparator separator, FieldEnd end)
        {
            mBundle = new FieldBundle(start, separator, end);
        }

        /// <summary>
        /// Parses field code and caches it. Inheritors may add parsing logic if more complex properties should be parsed.
        /// </summary>
        internal virtual void ParseFieldCode()
        {
            if (IsUpdating)
                mUpdateContext.NotifyFieldCodeParsing();

            mFieldCodeCache = new FieldCode(this);

            if (IsUpdating)
                mUpdateContext.NotifyFieldCodeParsed();
        }

        /// <summary>
        /// Clears the current field code cache making it to refresh on the next demand if any.
        /// </summary>
        internal void InvalidateFieldCodeCache()
        {
            mFieldCodeCache = null;
        }

        [JavaThrows(true)]
        internal virtual void NotifyChildFieldUpdated(IFieldArgument argument)
        {
        }

        /// <summary>
        /// Ensures a field code cache for this field.
        /// </summary>
        internal void EnsureFieldCodeCache()
        {
            if (!HasFieldCodeCache)
                ParseFieldCode();
        }

        /// <summary>
        /// Returns text between field start and field separator (or field end if there is no separator).
        /// Both field code and field result of child fields are included.
        /// </summary>
        public string GetFieldCode()
        {
            return GetFieldCode(true);
        }

        /// <summary>
        /// Returns text between field start and field separator (or field end if there is no separator).
        /// </summary>
        /// <param name="includeChildFieldCodes">
        /// <c>true</c> if child field codes should be included.
        /// </param>
        public string GetFieldCode(bool includeChildFieldCodes)
        {
            return mBundle.GetFieldCode(includeChildFieldCodes);
        }

        /// <summary>
        /// Detects field type from field code, unlike <see cref="Type"/> which detects it from a field start.
        /// </summary>
        /// <returns></returns>
        internal FieldType GetFieldTypeFromCode()
        {
            return FieldUtil.GetFieldType(FieldCodeCache.FieldType);
        }

        internal void SetFieldCode(string fieldCode)
        {
            RemoveFieldCode();

            DocumentBuilder builder = new DocumentBuilder(FetchDocument());
            builder.MoveTo(FieldCodeEnd);
            builder.Write(fieldCode);
        }

        internal void SetFieldType(FieldType fieldType)
        {
            mBundle.FieldType = fieldType;
        }

        internal void DetermineFieldType()
        {
            mBundle.DetermineFieldType();
        }

        /// <summary>
        /// Removes the field from the document. Returns a node right after the field. If the field's end is the last child
        /// of its parent node, returns its parent paragraph. If the field is already removed, returns <c>null</c>.
        /// </summary>
        public virtual Node Remove()
        {
            return FieldReplacer.Remove(this);
        }

        /// <summary>
        /// Removes field code.
        /// </summary>
        internal void RemoveFieldCode()
        {
            NodeRemover.Remove(Start, false, FieldCodeEnd, false);
        }

        /// <summary>
        /// Removes field result.
        /// </summary>
        internal void RemoveFieldResult()
        {
            if (!HasSeparator)
                return;

            NodeRemover.Remove(Separator, false, End, false, FieldUtil.GetFieldResultRemovalNodeJoinMode(this));
        }

        /// <summary>
        /// Performs the field update. Throws if the field is being updated already.
        /// </summary>
        public void Update()
        {
            Update(false);
        }

        /// <summary>
        /// Performs a field update. Throws if the field is being updated already.
        /// </summary>
        /// <param name="ignoreMergeFormat">
        /// If <c>true</c> then direct field result formatting is abandoned, regardless of the MERGEFORMAT switch, otherwise normal update is performed.
        /// </param>
        public void Update(bool ignoreMergeFormat)
        {
            if (IsUpdating)
                throw new InvalidOperationException("The field is being updated already.");

            bool mergeFormat = false;
            if (ignoreMergeFormat)
            {
                mergeFormat = Format.GeneralFormats.HasFormat(GeneralFormat.MergeFormat);
                Format.GeneralFormats.AddOrRemove(GeneralFormat.MergeFormat, false);
            }

            // This update is performed with the topmost context.
            FieldUpdater.UpdateField(this);

            if (ignoreMergeFormat)
                Format.GeneralFormats.AddOrRemove(GeneralFormat.MergeFormat, mergeFormat);
        }

        /// <summary>
        /// Performs the field unlink.
        /// </summary>
        /// <remarks>
        /// <para>Replaces the field with its most recent result.</para>
        /// <para>Some fields, such as XE (Index Entry) fields and SEQ (Sequence) fields, cannot be unlinked.</para>
        /// </remarks>
        /// <returns>
        /// <c>true</c> if the field has been unlinked, otherwise <c>false</c>.
        /// </returns>
        public bool Unlink()
        {
            if (!IsUnlinkable)
                return false;

            FieldUnlinker.UnlinkField(this);
            return true;
        }

        /// <summary>
        /// Initiates the field's update.
        /// </summary>
        internal void BeginUpdate(FieldUpdateContext context)
        {
            Debug.Assert(!IsUpdating);

            mUpdateContext = context;
            ParseFieldCode();
        }

        /// <summary>
        /// Finalizes the field's update.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void EndUpdate()
        {
            Debug.Assert(IsUpdating);

            mUpdateContext = null;
        }

        /// <summary>
        /// When overridden in a derived class, returns a value indicating how a child field contained
        /// within the specified argument of this field should be updated at the moment.
        /// </summary>
        [JavaThrows(true)]
        internal virtual FieldUpdateStrategy GetChildFieldsUpdateStrategyInArgument(IFieldArgument argument)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the field's update stage.
        /// </summary>
        [JavaThrows(true)]
        internal virtual FieldUpdateStage GetUpdateStage()
        {
            return FieldUtil.RequiresLayoutDocumentOnUpdate(Type)
                ? FieldUpdateStage.DeferredUpdateLayout
                : FieldUpdateStage.MainLoop;
        }

        /// <summary>
        /// Returns an update action that specifies how to update the field (set result, remove etc) and also
        /// holds all necessary calculated data. To be overridden by specific field types.
        /// </summary>
        /// <remarks>
        /// Any override of this method should return <c>null</c> only if the corresponding field result is not
        /// changed during its update. If the field's result is not changed by some update action (but changed
        /// by the overridden code itself), then you should return <see cref="FieldUpdateActionDoNothing"/> to
        /// indicate this. Otherwise, layout engine will not be able to handle the change properly.
        /// </remarks>
        [JavaThrows(true)]
        internal virtual FieldUpdateAction UpdateCore()
        {
            Constant value = Updater.DataProviders.GetValue(this);
            return value != null
                ? new FieldUpdateActionApplyResult(this, value)
                : null;
        }

        internal void UnlinkCore()
        {
            if (!IsUnlinkable)
                return;

            FieldReplacer.ReplaceWithResult(this, new UnlinkFieldReplaceListener(this), NodeJoinMode.JoinToNextSibling);
        }

        private bool IsUnlinkable
        {
            get
            {
                switch (Type)
                {
                    case FieldType.FieldIndexEntry:
                    case FieldType.FieldTOCEntry:
                    case FieldType.FieldRefDoc:
                    case FieldType.FieldEquation:
                    case FieldType.FieldSymbol:
                    case FieldType.FieldBarcode:
                    case FieldType.FieldFormCheckBox:
                    case FieldType.FieldTOAEntry:
                    case FieldType.FieldPrivate:
                    case FieldType.FieldAdvance:
                    case FieldType.FieldBidiOutline:
                        return false;

                    default:
                        return !IsRemoved;
                }
            }
        }

        [JavaThrows(true)]
        internal virtual void BeforeUnlink()
        {
            // Do nothing by default.
        }

        /// <summary>
        /// Returns a displayable result for some fields that actually have no separator (such as SYMBOL,
        /// MACROBUTTON etc).
        /// </summary>
        [JavaThrows(true)]
        internal virtual NodeRange GetFakeResult()
        {
            return null;
        }

        /// <summary>
        /// Returns a node range occupying the whole field.
        /// </summary>
        internal NodeRange GetFieldRange()
        {
            return IsRemoved
                ? NodeRange.Void
                : new NodeRange(Start, End);
        }

        /// <summary>
        /// Returns a node range representing field code.
        /// </summary>
        internal NodeRange GetFieldCodeRange()
        {
            return IsRemoved
                ? NodeRange.Void
                : new NodeRange(Start, false, FieldCodeEnd, false);
        }

        /// <summary>
        /// Returns a node range representing field result.
        /// </summary>
        internal NodeRange GetFieldResultRange()
        {
            return IsRemoved || !HasSeparator
                ? NodeRange.Void
                : new NodeRange(Separator, false, End, false);
        }

        /// <summary>
        /// Returns object implementing <see cref="IBidiParagraphLevelOverride"/> interface to use in field result
        /// forming routines when <see cref="FieldOptions.IsBidiTextSupportedOnUpdate"/> is set to true.
        /// </summary>
        internal virtual IBidiParagraphLevelOverride GetBidiParagraphLevelOverride()
        {
            bool resolvedBidi = GetBidiEmbeddingLevel();
            return ConstantBidiParagraphLevelOverride.GetInstance(resolvedBidi);
        }

        private bool GetBidiEmbeddingLevel()
        {
            // BIDI embedding level of most of the fields depends on field start's RTL attribute.
            // Code snippet for BIDI resolving below is copied from the last version of FieldUpdateCleanupActionMergeTextField
            // class, which is removed for now. However, this code seems to be error prone in case of AttrBoolEx.Same.
            // But there's no more suitable options, so leave as it is.
            // WORDSNET-8416 Some fields use the parent paragraph's BIDI attribute in this case.
            switch (Type)
            {
                case FieldType.FieldPage:
                case FieldType.FieldNumPages:
                    return IsInHeaderFooter
                        ? GetPageAndNumPagesBidiEmbeddingLevelSource()
                        : Start.ParentParagraph.ParagraphFormat.Bidi;
                case FieldType.FieldPageRef:
                    return Start.ParentParagraph.ParagraphFormat.Bidi;
                default:
                    return (bool)Start.RunPr.Bidi.ResolveFetchInheritedRunAttrWithNull(Start, FontAttr.Bidi);
            }
        }

        private bool GetPageAndNumPagesBidiEmbeddingLevelSource()
        {
            Debug.Assert(IsInHeaderFooter);
            Debug.Assert(Type == FieldType.FieldPage || Type == FieldType.FieldNumPages);

            if (Start.Font.Bidi)
                return true;

            if (!Format.GeneralFormats.HasFormat(GeneralFormat.MergeFormat))
                return false;

            Inline inline = FirstOrDefaultInlineNode(OldResultNodes);
            return inline != null
                ? inline.Font.Bidi
                : Start.ParentParagraph.ParagraphFormat.Bidi;
        }

        private static Inline FirstOrDefaultInlineNode(IEnumerable<Node> nodes)
        {
            if (nodes == null)
                return null;

            foreach (Node node in nodes)
            {
                Inline inline = node as Inline;
                if (inline != null)
                    return inline;
            }

            return null;
        }

        /// <summary>
        /// Makes sure the field has a separator and otherwise either throws or inserts it.
        /// </summary>
        internal void EnsureSeparator(bool force)
        {
            if (HasSeparator)
                return;

            if (force || (FieldUtil.GetSeparatorPresence(Type) != FieldSeparatorPresence.Never))
            {
                // The separator is missed for some reason, insert it.
                FieldSeparator separator = new FieldSeparator(End.Document, new RunPr(), Start.FieldType);
                End.InsertPrevious(separator);

                mBundle.Separator = separator;
                End.SetHasSeparator(true);

                if (IsUpdating)
                    UpdateContext.NotifyFieldAreaChanged(FieldArea.Code);
            }
            else
            {
                // This field cannot have a result, throw.
                throw new InvalidOperationException("Cannot set result of a field that does not have a separator.");
            }
        }

        [JavaThrows(true)]
        internal virtual Section GetPageNumberFormatSection()
        {
            return null;
        }

        /// <summary>
        /// Returns the default date/time formatting string used when the custom date/time format is omitted.
        /// </summary>
        internal virtual string GetDefaultDateTimeFormat()
        {
            return null;
        }

        /// <summary>
        /// Returns the main (not glossary) owner document.
        /// </summary>
        internal Document FetchDocument()
        {
            return Start.FetchDocument();
        }

        /// <summary>
        /// Stores the old result nodes into the list.
        /// </summary>
        internal void StoreOldResultNodesIfNeeded()
        {
            mOldResultNodes = NeedStoreOldResultNodes()
                ? FieldOldResultNodeCollection.Create(this)
                : null;

            if (mOldResultNodes != null)
            {
                if (Separator != null && Separator.ParentParagraph != null)
                    OldResultStartParagraph = (Paragraph)Separator.ParentParagraph.Clone(false);

                if (End != null && End.ParentParagraph != null)
                    OldResultEndParagraph = (Paragraph)End.ParentParagraph.Clone(false);
            }
        }

        /// <summary>
        /// Returns the value, indicating whether the old field result must be stored during a field result update
        /// (for example, to compare the old field result with the new one).
        /// </summary>
        protected virtual bool NeedStoreOldResultNodes()
        {
            if (FieldUtil.GetSeparatorPresence(Type) == FieldSeparatorPresence.Never)
                return false;

            if (FieldCodeCache.HasParseError)
                return false;

            // At the moment there is only one case, when storing of the field result is necessary.
            // It is the format merging. But may be it would be useful in some other scenarios.
            return Format.GeneralFormats.HasFormat(GeneralFormat.MergeFormat);
        }

        /// <summary>
        /// Frees the list, containing the old result nodes.
        /// </summary>
        internal void RemoveStoredOldResultNodes()
        {
            mOldResultNodes = null;
        }

        /// <summary>
        /// Returns previously collected old result nodes. It is guaranteed, that all of the nodes
        /// do not belong to a document.
        /// </summary>
        internal IEnumerable<Node> OldResultNodes
        {
            get
            {
                Debug.Assert(mOldResultNodes != null);
                return mOldResultNodes.Range;
            }
        }

        /// <summary>
        /// Returns first paragraph of old result.
        /// It is guaranteed, that node does not belong to a document.
        /// </summary>
        internal Paragraph OldResultStartParagraph { get; private set; }

        /// <summary>
        /// Returns last paragraph of old result.
        /// It is guaranteed, that node does not belong to a document.
        /// </summary>
        internal Paragraph OldResultEndParagraph { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the field supports conditional child fields' update or not.
        /// </summary>
        /// <dev>
        /// This is important to provide this info not through <see cref="FieldUtil"/> method, but through the field object
        /// because of MERGEFIELD surrogates.
        /// </dev>
        internal virtual bool SupportsConditionalUpdate
        {
            get { return false; }
        }

        /// <summary>
        /// Gets whether this field is located in a header/footer.
        /// </summary>
        internal bool IsInHeaderFooter
        {
            get { return Start.GetAncestor(NodeType.HeaderFooter) != null; }
        }

        /// <summary>
        /// Gets the node that represents the start of the field.
        /// </summary>
        public FieldStart Start
        {
            [CppConstMethod]
            get { return mBundle.Start; }
        }

        /// <summary>
        /// Gets the node that represents the field separator. Can be <c>null</c>.
        /// </summary>
        public FieldSeparator Separator
        {
            get
            {
                if (mBundle.HasSeparator == End.HasSeparator)
                    return mBundle.Separator;

                mBundle.Separator = End.HasSeparator
                    ? FieldBundle.GetFieldBundleNoSeparatorCheck(Start).Separator
                    : null;

                return mBundle.Separator;
            }
        }

        /// <summary>
        /// Gets the node that represents the field end.
        /// </summary>
        public FieldEnd End
        {
            [CppConstMethod]
            get { return mBundle.End; }
        }

#if CPLUSPLUS
        // For backward compatibility

        public FieldStart FieldStart
        {
            [CppConstMethod]
            get { return mBundle.Start; }
        }

        public FieldEnd FieldEnd
        {
            [CppConstMethod]
            get { return mBundle.End; }
        }
#endif

        /// <summary>
        /// Gets the Microsoft Word field type.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Public API, as designed.")]
        public virtual FieldType Type
        {
            get { return Start.FieldType; }
        }

        /// <summary>
        /// Gets or sets text that is between the field separator and field end.
        /// </summary>
        public string Result
        {
            get { return GetResult(false); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");

                EnsureSeparator(false);
                StoreOldResultNodesIfNeeded();

                FieldResultApplier resultApplier = BuildResultApplier(value);
                resultApplier.ApplyResult();

                RemoveStoredOldResultNodes();
            }
        }

        /// <remarks>
        /// The method has to be "private protected", but it requires C# 7.2 or greater.
        /// </remarks>
        internal virtual FieldResultApplier BuildResultApplier(string result)
        {
            return new TextResultApplier(this, result);
        }

        /// <summary>
        /// Gets the text that represents the displayed field result.
        /// </summary>
        /// <remarks>
        /// The <see cref="Words.Document.UpdateListLabels"/> method must be called to obtain correct value for the
        /// <see cref="FieldListNum"/>, <see cref="FieldAutoNum"/>, <see cref="FieldAutoNumOut"/> and <see cref="FieldAutoNumLgl"/> fields.
        /// </remarks>
        public string DisplayResult
        {
            get
            {
                NodeRange fakeResult = GetFakeResult();

                if (fakeResult != null)
                    return NodeTextCollector.GetText(fakeResult, true);

                return GetResult(true);
            }
        }

        internal string GetResult(bool isFieldResultMode)
        {
            return HasSeparator
                ? NodeTextCollector.GetText(Separator, false, End, false, isFieldResultMode)
                : string.Empty;
        }

        internal bool HasResult
        {
            get
            {
                NodeRange resultRange = GetFieldResultRange();

                if (resultRange.IsVoid)
                    return false;

                foreach (Node node in resultRange)
                {
                    if (resultRange.Start.Node == node || resultRange.End.Node == node)
                        continue;

                    if (NodeUtil.GetLength(node) > 0)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets whether the field is locked (should not recalculate its result).
        /// </summary>
        public bool IsLocked
        {
            get { return mBundle.IsLocked; }
            set { mBundle.IsLocked = value; }
        }

        /// <summary>
        /// Gets or sets whether the current result of the field is no longer correct (stale) due to other modifications made to the document.
        /// </summary>
        public bool IsDirty
        {
            get { return mBundle.IsDirty; }
            set { mBundle.IsDirty = value; }
        }

        /// <summary>
        /// Returns either separator or field end node.
        /// </summary>
        internal FieldChar FieldCodeEnd
        {
            get { return mBundle.FieldCodeEnd; }
        }

        /// <summary>
        /// Gets a <see cref="FieldCode"/> cache for this field.
        /// </summary>
        internal FieldCode FieldCodeCache
        {
            get
            {
                EnsureFieldCodeCache();

                return mFieldCodeCache;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the field has field code cache.
        /// </summary>
        internal bool HasFieldCodeCache
        {
            get { return mFieldCodeCache != null; }
        }

        /// <summary>
        /// Gets a <see cref="FieldFormat"/> object that provides typed access to field's formatting.
        /// </summary>
        public FieldFormat Format
        {
            get { return mFieldFormatCache ?? (mFieldFormatCache = new FieldFormat(this)); }
        }

        /// <summary>
        /// Gets or sets the LCID of the field.
        /// </summary>
        /// <seealso cref="FieldUpdateCultureSource.FieldCode"/>
        public int LocaleId
        {
            get
            {
                return FieldCodeCache.Bidi
                    ? FieldCodeCache.LanguageIdBi
                    : FieldCodeCache.LanguageId;
            }

            set
            {
                Language language = (Language)value;
                if ((language == Language.LanguageNotSet) || !LocaleClassifier.IsDefined(language))
                    throw new ArgumentException("Not a valid LCID: " + value);

                if (LocaleClassifier.IsArabic(value) || LocaleClassifier.IsHebrew(value))
                {
                    FieldCodeCache.LanguageIdBi = value;
                    FieldCodeCache.Bidi = true;
                }
                else
                {
                    FieldCodeCache.LanguageId = value;
                    FieldCodeCache.Bidi = false;
                }
            }
        }

        /// <summary>
        /// Gets whether this field has a separator.
        /// </summary>
        internal bool HasSeparator
        {
            get { return Separator != null; }
        }

        /// <summary>
        /// Gets whether this field is removed from the model.
        /// </summary>
        internal bool IsRemoved
        {
            get
            {
                if (Start.ParentNode == null)
                    return true;

                if (End.ParentNode == null)
                    return true;

                // The field is considered as not removed if there is common start and end ancestor
                // (even if the common ancestor itself is removed), i.e. we can traverse from start to end.
                return Start.GetTopmostAncestor() != End.GetTopmostAncestor();
            }
        }

        internal FieldUpdater Updater
        {
            get { return mUpdateContext.Updater; }
        }

        internal FieldUpdateContext UpdateContext
        {
            get { return mUpdateContext; }
        }

        internal bool IsUpdating
        {
            get { return UpdateContext != null; }
        }

        internal DocumentBase Document
        {
            get { return Start.Document; }
        }

        /// <summary>
        /// Returns a value indicating whether the field is topmost field in the DOM itself
        /// or a part of outer topmost field result (recursively).
        /// </summary>
        internal bool IsTopmostField()
        {
            return IsTopmostField(End);
        }

        internal virtual INodeModifier GetResultModifier()
        {
            return null;
        }

        private static bool IsTopmostField(FieldEnd fieldEnd)
        {
            DocumentPosition currentPosition = DocumentPosition.CreatePositionAfter(fieldEnd);
            FieldCharCounter fieldCharCounter = new FieldCharCounter();

            while (true)
            {
                Node lastNode = currentPosition.Node;
                do
                {
                    if (!currentPosition.Move(null, true, true, true, false, false))
                        return true;
                }
                while (currentPosition.Node == lastNode);

                Node currentNode = currentPosition.Node;
                if (FieldCharCounter.IsValidFieldChar(currentNode))
                {
                    switch (currentNode.NodeType)
                    {
                        case NodeType.FieldSeparator:
                            if (!fieldCharCounter.IsInFieldCode)
                                return false;

                            break;
                        case NodeType.FieldEnd:
                            if (!fieldCharCounter.IsInField)
                            {
                                FieldEnd parentFieldEnd = ((FieldEnd)currentNode);
                                return parentFieldEnd.HasSeparator && IsTopmostField(parentFieldEnd);
                            }

                            break;
                        default:
                            break;
                    }
                }

                fieldCharCounter.VisitNode(currentNode);
            }
        }

        private FieldBundle mBundle;
        private FieldCode mFieldCodeCache;
        private FieldFormat mFieldFormatCache;
        [CppWeakPtr]
        private FieldUpdateContext mUpdateContext;
        private FieldOldResultNodeCollection mOldResultNodes;

#if DEBUG
        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}-{3}", Type.ToString(), GetHashCode(), Start.GetNodeId(), End.GetNodeId());
        }
#endif
    }
}
