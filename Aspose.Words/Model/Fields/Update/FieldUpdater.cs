// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/06/2010 by Dmitry Vorobyev

using System;
using System.Collections.Generic;
using System.Globalization;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Updates single or multiple fields in a single session. Represents a root node of a field update tree.
    /// See <see cref="FieldUpdateContextContainer"/> remarks for details.
    /// </summary>
    /// <remarks>
    /// Fields are updated from the bottom to the top traversing the field update tree.
    /// </remarks>
    internal sealed class FieldUpdater : FieldUpdateContextContainer
    {
        /// <summary>
        /// Creates a <see cref="FieldUpdater"/> instance for a field update session which is considered not to be
        /// initiated by layout.
        /// </summary>
        private FieldUpdater(Document document)
            : this(document, (FieldDisplayContext)null, null)
        {
        }

        private FieldUpdater(Document document, FieldDisplayContext displayContext)
            : this(document, displayContext, null)
        {
        }

        internal FieldUpdater(Document document, BookmarkCache bookmarkCache, IFieldCleaner fieldCleaner)
            : this (document, (FieldDisplayContext)null, fieldCleaner)
        {
            BookmarkCache = bookmarkCache;
            mInternalCreatedBookmarkCache = false;
        }

        private FieldUpdater(Document document, bool updateBeforeSave, bool updateDirtyFields)
            : this (document)
        {
            mIsUpdateBeforeSave = updateBeforeSave;
            mIsUpdateDirtyFields = updateDirtyFields;
        }

        private FieldUpdater(Document document, FieldDisplayContext displayContext, IFieldCleaner fieldCleaner)
        {
            CurrentStage = FieldUpdateStage.MainLoop;
            Document = document;
            BookmarkCache = new BookmarkCache(Document);
            mFieldCleaner = (fieldCleaner != null) ? fieldCleaner : new DefaultFieldCleaner();
            mInternalCreatedBookmarkCache = true;
            DisplayContext = displayContext;
            IsUpdateInitiatedByLayout = (displayContext != null);
            Listeners = new FieldUpdateListenerCollection();
            mSession = displayContext != null && displayContext.FieldUpdateSession != null
                           ? displayContext.FieldUpdateSession
                           : new FieldUpdateSession(document);
            DataProviders = new FieldUpdateDataProvider(this, mSession.DataProviderCollection);
        }

        internal void EnsureFieldUpdateProgressContext()
        {
            if (Document.FieldUpdateProgressContext == null)
            {
                Document.FieldUpdateProgressContext = new FieldUpdateProgressContext(
                    Document.FieldOptions.FieldUpdatingProgressCallback,
                    this);
            }
        }

        private void ReleaseFieldUpdateProgressContext()
        {
            if (Document.FieldUpdateProgressContext.Owner != this)
                return;

            Document.FieldUpdateProgressContext.OnUpdateCompleted();
            Document.FieldUpdateProgressContext = null;
        }

        /// <summary>
        /// Updates the given field and its child fields if any in a single session. The given field is considered
        /// to be topmost by the updater.
        /// </summary>
        internal static void UpdateField(Field field)
        {
            UpdateFieldFullCycle(field, null, NullableBool.NotDefined);
        }

        private static FieldUpdateResults UpdateFieldFullCycle(
            Field field,
            FieldDisplayContext displayContext,
            NullableBool isTopmostField)
        {
            // The field should not be updated already.
            Debug.Assert(!field.IsUpdating);

            FieldUpdater updater = new FieldUpdater(field.FetchDocument(), displayContext);
            updater.EnsureFieldUpdateProgressContext();
            using (FieldUpdateContext context = updater.AddTopmostFieldToUpdate(field, isTopmostField))
            {
                if (context == null)
                {
                    updater.ReleaseFieldUpdateProgressContext();
                    return FieldUpdateResults.None;
                }

                updater.UpdateFieldsByStages(FieldUpdateStage.MainLoop, FieldUpdateStage.DeferredUpdateLayout);
                updater.FinalizeUpdateFields();

                return context.UpdateResults;
            }
        }

        /// <summary>
        /// Updates fields contained in the given node.
        /// </summary>
        internal static void UpdateFields(Node node)
        {
            FieldUpdater updater = new FieldUpdater(node.FetchDocument());
            updater.EnsureFieldUpdateProgressContext();
            UpdateFields(node, updater);
        }

        /// <summary>
        /// Updates fields contained in the given node range.
        /// </summary>
        internal static void UpdateFields(NodeRange nodeRange)
        {
            IList<Field> fieldColl = FieldExtractor.ExtractToCollection(nodeRange);
            foreach (Field field in fieldColl)
                UpdateField(field);
        }

        /// <summary>
        /// Updates given fields.
        /// </summary>
        internal static void UpdateFields(IList<Field> fields)
        {
            if (fields.Count == 0)
                return;

            FieldUpdater updater = new FieldUpdater(fields[0].FetchDocument());
            updater.EnsureFieldUpdateProgressContext();
            foreach (Field field in fields)
                updater.AddTopmostFieldToUpdate(field, NullableBool.NotDefined);

            UpdateFields(updater);
        }

        internal static void UpdateFieldsBeforeSave(Node node)
        {
            FieldUpdater updater = new FieldUpdater(node.FetchDocument(), true, false);
            updater.EnsureFieldUpdateProgressContext();
            UpdateFields(node, updater);
        }

        internal static void UpdateDirtyFields(Node node)
        {
            FieldUpdater updater = new FieldUpdater(node.FetchDocument(), false, true);
            updater.EnsureFieldUpdateProgressContext();
            UpdateFields(node, updater);
        }

        private static void UpdateFields(Node node, FieldUpdater updater)
        {
            updater.AddTopmostFieldsToUpdate(node);
            UpdateFields(updater);
        }

        private static void UpdateFields(FieldUpdater updater)
        {
            updater.UpdateFieldsByStages(FieldUpdateStage.MainLoop, FieldUpdateStage.DeferredUpdateLayout);
            updater.FinalizeUpdateFields();
        }

        /// <summary>
        /// Updates fields contained in the given nodes. Allows calling multiple times because does not update deferred fields.
        /// </summary>
        internal void UpdateFieldsRepeatedly(IList<Node> nodes)
        {
            AddTopmostFieldsToUpdate(nodes);
            UpdateFieldsByStages(FieldUpdateStage.MainLoop, FieldUpdateStage.MainLoop);
            RemoveDeferredFields();
        }

        /// <summary>
        /// Updates deferred fields and finalizes field update. Should be called after last call of
        /// <see cref="UpdateFieldsRepeatedly"/> is done.
        /// </summary>
        internal void UpdateFieldsRepeatedlyFinalize()
        {
            UpdateFieldsByStages(FieldUpdateStage.DeferredUpdateRef, FieldUpdateStage.DeferredUpdateLayout);
            FinalizeUpdateFields();
        }

        internal void RemoveTopmostFieldContexts(IList<Node> nodes)
        {
            RemoveTopmostFieldHelper helper = new RemoveTopmostFieldHelper();
            helper.Extract(nodes);
#if JAVA
            IEnumerator<FieldUpdateContext> contextEnumerator = GetEnumerator();
#else
            using (IEnumerator<FieldUpdateContext> contextEnumerator = GetEnumerator())
#endif
            {
                while (helper.FieldStarts.Count != 0 && contextEnumerator.MoveNext())
                {
                    FieldUpdateContext context = contextEnumerator.Current;
                    int fieldStartIndex = helper.FieldStarts.IndexOf(context.Field.Start);
                    if (fieldStartIndex == -1)
                        continue;

                    RemoveChild(context);
                    helper.FieldStarts.RemoveAt(fieldStartIndex);
                }
            }
        }

        private void AddTopmostFieldsToUpdate(Node node)
        {
            NullableBool isTopmost = node is DocumentBase ? NullableBool.True : NullableBool.NotDefined;
            AddTopmostFieldHelper helper = new AddTopmostFieldHelper(this, isTopmost);
            helper.Extract(node);
        }

        private void AddTopmostFieldsToUpdate(IList<Node> nodes)
        {
            AddTopmostFieldHelper helper = new AddTopmostFieldHelper(this, NullableBool.NotDefined);
            helper.Extract(nodes);
        }

        private FieldUpdateContext AddTopmostFieldToUpdate(Field field, NullableBool isTopmostField)
        {
            BeginInsertFieldToUpdate();
            FieldUpdateContext context = FieldUpdateContext.AppendTopmostContext(field, this, isTopmostField);
            EndInsertFieldToUpdate(context);

            return context;
        }

        private void BeginInsertFieldToUpdate()
        {
            IsFieldToUpdateInserting = true;
        }

        private void EndInsertFieldToUpdate(FieldUpdateContext context)
        {
            IsFieldToUpdateInserting = false;
            if (context == null)
                return;

            // FOSS
        }

        /// <summary>
        /// Updates collected fields by stages from the specified range.
        /// </summary>
        private void UpdateFieldsByStages(FieldUpdateStage startStage, FieldUpdateStage endStage)
        {
            for (int stage = (int)startStage; stage <= (int)endStage; stage++)
            {
                CurrentStage = (FieldUpdateStage)stage;
                ExternalActions.Perform(CurrentStage);
                UpdateFieldsOnCurrentStage(this);
            }
        }

        /// <summary>
        /// Tries to update fields (which should be updated on the current stage) corresponding to child contexts
        /// of the specified container. Returns <c>false</c> if update of any field in the list is skipped. Returns
        /// <c>true</c> if all of the fields in the list are updated or their updates are deferred.
        /// </summary>
        private bool UpdateFieldsOnCurrentStage(FieldUpdateContextContainer container)
        {
            if (!container.HasChildren)
                return true;

            bool result;
            do
            {

                result = true;

                foreach (FieldUpdateContext context in container)
                {
                    // If the field's update is skipped or the field is updated already, move to the next one.
                    if (context.IsUpdateSkipped || context.IsUpdatePerformed)
                        continue;

                    // Update child fields first and the field itself then.
                    UpdateChildFieldsOnCurrentStage(context);
                    if (!UpdateFieldOnCurrentStage(context))
                        result = false;
                }
            }
            while (!FinalizeUpdateFieldsOnCurrentStage(container));

            return result;
        }

        /// <summary>
        /// Checks whether fields' update on the current stage can be finalized for the specified container.
        /// Returns <c>true</c> if the check is succeeded. Returns <c>false</c> to indicate, that the update
        /// should be continued.
        /// </summary>
        private bool FinalizeUpdateFieldsOnCurrentStage(FieldUpdateContextContainer container)
        {
            // The purpose of this method is to ensure that parent field code cache (if any) was not invalidated
            // upon child fields' update. This typically happens when a child field not enclosed in double quotes
            // is removed while updating. If the parent field code cache was invalidated, we have to reparse it
            // and perform child fields' update one more time. Note, that fields which are already updated will
            // not be updated once more because of FieldUpdateContext.IsUpdatePerformed check.
            //
            // If the specified container is the updater, then it does not correspond to any field in a document.
            if (container == this)
                return true;

            // If the container is context and its field has valid field code cache, then child fields' update
            // can be finalized.
            FieldUpdateContext context = (FieldUpdateContext)container;
            if (context.Field.HasFieldCodeCache)
                return true;

            // The parent field code chache is invalid. Reparse it and make child fields' update be continued.
            context.Field.ParseFieldCode();
            return false;
        }

        private void UpdateChildFieldsOnCurrentStage(FieldUpdateContext context)
        {
            if (context.AreAllChildFieldsUpdated)
                return;

            bool isUpdateDeferred = GetFieldUpdateStrategyByAncestor(context) != FieldUpdateStrategy.Update;

            // Update child fields by stages, if it is required.
            //
            // Note, that if the field's update is deferred, then it does not make any sense to update its child fields
            // by stages, because only immediate child fields will be updated in this case regardless of any conditions.
            // See GetUpdateStrategyOnCurrentStage for details.
            //
            // This also guarantees that Field.DefineChildFieldUpdateConditions is called only when it is really needed.
            if (!isUpdateDeferred && context.Field.SupportsConditionalUpdate)
            {
                context.ChildUpdateStage = FieldChildUpdateStage.Permanent;
                UpdateFieldsOnCurrentStage(context);
                context.ChildUpdateStage = FieldChildUpdateStage.Conditional;
            }

            if (!UpdateFieldsOnCurrentStage(context))
                return;

            // If the field's update is deferred, then its child fields' update is deferred either and hence we should not
            // consider them as updated in this case.
            if (!isUpdateDeferred)
                context.NotifyAllChildFieldsUpdated();
        }

        /// <summary>
        /// Returns a field update startegy of the specified field accordingly to ancestor fields conditional updates of its child fields.
        /// </summary>
        private static FieldUpdateStrategy GetFieldUpdateStrategyByAncestor(FieldUpdateContext context)
        {
            for (FieldUpdateContext childContext = context, parentContext = context.ParentContext;
                 parentContext != null;
                 childContext = parentContext, parentContext = parentContext.ParentContext)
            {
                if (parentContext.Field.SupportsConditionalUpdate)
                {
                    parentContext.Field.EnsureFieldCodeCache();
                    FieldUpdateStrategy strategy = parentContext.Field.GetChildFieldsUpdateStrategyInArgument(childContext.ParentArgument);
                    if (strategy != FieldUpdateStrategy.Update)
                    {
                        return strategy;
                    }
                }
            }

            return FieldUpdateStrategy.Update;
        }

        /// <summary>
        /// Tries to update the specified field on the current stage. Returns <c>true</c> if the field's update is performed or
        /// deferred. Returns <c>false</c> if its update is skipped.
        /// </summary>
        private bool UpdateFieldOnCurrentStage(FieldUpdateContext context)
        {
            FieldUpdater updater = context.Updater;
            FieldUpdateStrategy strategy = GetUpdateStrategyOnCurrentStage(context);

            // SPEED: Cache strategy to use during parent context update strategy calculating.
            context.FieldUpdateStrategy = strategy;

            try
            {
                switch (strategy)
                {
                    case FieldUpdateStrategy.Update:
                        UpdateField(context);
                        break;
                    case FieldUpdateStrategy.Skip:
                        return false;
                    case FieldUpdateStrategy.Defer:
                    case FieldUpdateStrategy.Reject:
                        context.NotifyUpdateProgressCurrentFieldIsUpdated();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            finally
            {
                updater.Listeners.NotifyUpdated(context, strategy);
            }

            return true;
        }

        private FieldUpdateStrategy GetUpdateStrategyOnCurrentStage(FieldUpdateContext context)
        {
            // The order of the following checks is very important. Do not change it, unless you really understand what you
            // are doing.

            if (FieldUtil.GetEffectiveFieldLocked(context.Field))
                return FieldUpdateStrategy.Defer;

            if (mIsUpdateDirtyFields && !context.Field.IsDirty)
                return FieldUpdateStrategy.Defer;

            FieldUpdateStrategy strategyByAncestor = GetFieldUpdateStrategyByAncestor(context);

            // 0. The field's update can be rejected by its ancestor field.
            if (!context.RequiresImmediateUpdate && (strategyByAncestor == FieldUpdateStrategy.Reject || strategyByAncestor == FieldUpdateStrategy.Skip))
                return strategyByAncestor;

            // 1. If the field update is deferred to a later stage, skip it. This also handles those fields, which should be
            //    updated immediately but are deferred by the field update listener at the same time.
            //
            // We can encounter a field which update is deferred to an earlier stage. This can happen in the following cases:
            //   - the field's parent supports conditional update and this field is updated on conditional stage.
            //   - a child field of this field was deferred to this stage but the field itself is deferred to an earlier stage.
            // We should allow update of such fields either.
            if (context.StageToDefer > CurrentStage)
                return FieldUpdateStrategy.Skip;

            // 2. If the field requires an immediate update, do it right away. If the field contains any child fields, which
            // should be updated on later stages, then they are not to be updated at this point.
            //
            // Theoretically we may deal with something like { MERGEFIELD "person{ REF num }" }. This means that the field
            // which should be updated immediately contains a field which should be updated on a later stage. This conflict
            // can not be resolved easily, so let's just ignore this case at the moment. It seems to have a little significance
            // in practice anyway.
            if (context.RequiresImmediateUpdate)
                return FieldUpdateStrategy.Update;

            // 3. If a common field should be updated externally by the layout, skip it, since the layout will update it anyway.
            if (context.IsUpdatedByLayoutExternally)
                return FieldUpdateStrategy.Skip;

            // 4. Skip update, if any child fields update skipped (WORDSNET-12155).
            if (IsAnyChildFieldsUpdateSkipped(context))
                return FieldUpdateStrategy.Skip;

            // 5. The field's update can be deferred by its ancestor field.
            if (strategyByAncestor == FieldUpdateStrategy.Defer)
                return FieldUpdateStrategy.Defer;

            // 6. Common fields should be updated after all of its children.
            if (!context.AreAllChildFieldsUpdated)
                return FieldUpdateStrategy.Skip;

            return FieldUpdateStrategy.Update;
        }

        /// <summary>
        /// Indicates whether updating of any child field of specified context update is skipped.
        /// </summary>
        private static bool IsAnyChildFieldsUpdateSkipped(FieldUpdateContext context)
        {
            // WORDSNET-14390 return if all child fields are updated.
            if (context.AreAllChildFieldsUpdated)
                return false;

            foreach (FieldUpdateContext childContext in context)
            {
                if (childContext.FieldUpdateStrategy == FieldUpdateStrategy.Skip)
                    return true;

                if (IsAnyChildFieldsUpdateSkipped(childContext))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the field either through layout or in a common way.
        /// </summary>
        private void UpdateField(FieldUpdateContext context)
        {
            // FOSS

            // Request the field's update through layout ...
            if (NeedRequestUpdateFieldByLayout(context) && DisplayContext.UpdateField(context.Field, context.IsTopmostField))
            {
                ReflowLayoutOrDefer(context.Field);
                return;
            }

            // ... if it is rejected, update the field in a common way.
            FieldUpdateResults results = UpdateFieldCore(context);
            NotifyLayoutFieldResultChanged(context, results);
        }

        private void NotifyLayoutFieldResultChanged(FieldUpdateContext context, FieldUpdateResults updateResults)
        {
            // FOSS
        }

        private bool NeedRequestUpdateFieldByLayout(FieldUpdateContext context)
        {
            // No display context - can not request an update through layout.
            if (DisplayContext == null)
                return false;

            // If the update session is initiated by layout, then the topmost fields are updated by layout from the beginning,
            // so do not request their update through layout one more time.
            return (!IsUpdateInitiatedByLayout || context.HasParentContext || context.IsInFieldResult || FieldUtil.RequiresUpdateFieldsInResult(context.Field.Type));
        }

        /// <summary>
        /// Updates the specified field in a common way.
        /// </summary>
        private static FieldUpdateResults UpdateFieldCore(FieldUpdateContext context)
        {
            if (context.Updater.Document.FieldUpdateProgressContext != null)
                context.Updater.Document.FieldUpdateProgressContext.NotifyInternal();

            if (context.Field.IsRemoved)
            {
                context.NotifyUpdateProgressCurrentFieldIsUpdated();

                return FieldUpdateResults.None;
            }

            FieldUpdater updater = context.Updater;

            if (!updater.NeedSkipFieldUpdate(context))
            {
                context.Field.StoreOldResultNodesIfNeeded();

                RaiseFieldUpdatingEvent(context.Field, updater);

                if (updater.Listeners.NotifyUpdate(context) || UpdateFieldCultureAware(context.Field))
                    updater.ProcessFieldResultChange(context, true);

                RaiseFieldUpdatedEvent(context.Field, updater);

                context.Field.RemoveStoredOldResultNodes();
            }

            context.NotifyUpdateProgressCurrentFieldIsUpdated();

            // If the field requires fields in its result to be updated, update them regardless of whether
            // the field itself was updated or not.
            if (InsertFieldResultFieldsIfNeeded(context))
                updater.ProcessFieldResultChange(context, context.Field.IsInHeaderFooter);

            EndUpdateField(context, updater);

            return context.UpdateResults;
        }

        private bool NeedSkipFieldUpdate(FieldUpdateContext context)
        {
            return NeedSkipFieldUpdateInHeaderFooter(context) || NeedSkipFieldUpdateBeforeSave(context);
        }

        private bool NeedSkipFieldUpdateInHeaderFooter(FieldUpdateContext context)
        {
            // Some fields located in header/footer can not be updated if the update was not initiated by layout.
            // Skip them here.
            if (context.Field.IsInHeaderFooter &&
                !IsUpdateInitiatedByLayout &&
                !FieldUtil.IsUpdatedWithoutLayoutInHeaderFooter(context.Field.Type))
            {
                return true;
            }

            // WORDSNET-5457 Do not update fields that MS Word does not update.
            return (IsUpdateInitiatedByLayout && !IsUpdatedWithLayoutInHeaderFooter(context));
        }

        private static bool IsUpdatedWithLayoutInHeaderFooter(FieldUpdateContext context)
        {
            if (FieldUtil.IsUpdatedWithLayoutInHeaderFooter(context.Field.Type))
                return true;

            if (context.HasChildren)
            {
                foreach (FieldUpdateContext childContext in context)
                {
                    if (IsUpdatedWithLayoutInHeaderFooter(childContext))
                        return true;
                }
            }

            return false;
        }

        private bool NeedSkipFieldUpdateBeforeSave(FieldUpdateContext context)
        {
            if (!mIsUpdateBeforeSave)
                return false;

            return !FieldsAutoUpdateUtils.IsFieldUpdatedBeforeSave(context);
        }

        /// <summary>
        /// Updates the specified field considering that it may require using of a culture, other than the current
        /// thread's one. Returns a value indicating whether the field's result was changed during the update.
        /// </summary>
        private static bool UpdateFieldCultureAware(Field field)
        {
            FieldOptions options = field.FetchDocument().FieldOptions;

            string fieldUpdateCultureName = GetFieldUpdateCultureName(options, field);
            if (fieldUpdateCultureName == null)
                return UpdateFieldUsingCurrentCulture(field);

            CultureInfo culture = null;
            if (options.FieldUpdateCultureProvider != null)
                culture = options.FieldUpdateCultureProvider.GetCulture(fieldUpdateCultureName, field);

            if (culture == null)
                culture = SystemPal.GetCulture(fieldUpdateCultureName);

            CultureInfo currentCulture = SystemPal.GetCurrentCulture();
            try
            {
                SystemPal.SetCulture(culture);
                return UpdateFieldUsingCurrentCulture(field);
            }
            finally
            {
                SystemPal.SetCulture(currentCulture);
            }
        }

        /// <summary>
        /// Returns culture name that should be used while updating of the specified field. Returns <c>null</c> to
        /// indicate that the current thread's culture should be used.
        /// </summary>
        private static string GetFieldUpdateCultureName(FieldOptions options, Field field)
        {
            if (options.FieldUpdateCultureSource == FieldUpdateCultureSource.CurrentThread)
                return null;

            // It looks like MS Word uses field's language for explicit date formatting only, except of date/time fields.
            // WORDSNET-6333 Use field's language for cardinal and ordinal number formatting
            if (IgnoreFieldLocale(field))
                return null;

            // WORDSNET-4241 Use the culture specified in the first character of the field code.
            int languageId = field.LocaleId;
            if (languageId == RunPr.ProcessOrUserDefaultLanguageId)
                return null;

            // RK I convert LCID to culture name here at this level because such conversion is not easily available
            // in Java at the framework level.
            return LocaleConverter.LocaleToDocxTag(languageId);
        }

        private static bool IgnoreFieldLocale(Field field)
        {
            if (field.FieldCodeCache.HasSwitch("\\@"))
                return false;

            if (FieldUtil.AlwaysConsidersLocale(field.Type))
                return false;

            GeneralFormat numericFormat = field.Format.GeneralFormats.GetNumericFormat();
            switch (numericFormat)
            {
                   case GeneralFormat.CardText:
                   case GeneralFormat.OrdText:
                   case GeneralFormat.Ordinal:
                   case GeneralFormat.DollarText:
                       return false;
                   default:
                       return true;
            }
        }

        /// <summary>
        /// Performs actual update of the specified field's result using the current thread's culture.
        /// Returns a value indicating whether the field's result was changed during the update.
        /// </summary>
        private static bool UpdateFieldUsingCurrentCulture(Field field)
        {
            // WORDSNET-10517, WORDSNET-21236 MS Word keeps bookmark nodes within field result during field update.
            FieldResultBookmarsPreserver bookmarksPreserver = new FieldResultBookmarsPreserver(field);

            TocEntriesBookmarkDeferredRemover bookmarkDeferredRemover = field.Updater.mTocEntriesBookmarkDeferredRemover;
            bookmarkDeferredRemover.FreezeBookmarks(field);

            FieldUpdateAction action = field.FieldCodeCache.HasParseError
                ? new FieldUpdateActionInsertErrorMessage(field, field.FieldCodeCache.ParseErrorMessage)
                : field.UpdateCore();

            // If the field result's removal is required, we should not postpone it, since the field result
            // affects the parent field's code and hence its result either. Moreover, it does not make any
            // sense to update the field result in this case.
            //
            // Other cleanup actions can be safely postponed to reduce parent field code reparsings.
            //
            if ((field.UpdateContext.CleanupActions & FieldUpdateCleanupActions.RemoveFieldResult) != 0)
            {
                // If the field does not have any result, do nothing.
                if (!field.HasSeparator || (field.Separator.NextSibling == field.End))
                    return false;

                field.RemoveFieldResult();
                return true;
            }

            if (action == null)
                return false;

            action.Perform();

            // WORDSNET-10517, WORDSNET-21236 MS Word keeps bookmark nodes within field result during field update.
            bookmarksPreserver.Restore();

            // WORDSNET-13586 Restore bookmarks in field result used by PAGEREF fields in TOC entries which update is deferred.
            bookmarkDeferredRemover.RestoreFrozenBookmarks(field);

            field.IsDirty = false;

            return true;
        }

        private static void RaiseFieldUpdatingEvent(Field field, FieldUpdater updater)
        {
            IFieldUpdatingCallback callback = updater.Document.FieldOptions.FieldUpdatingCallback;
            if (callback == null)
                return;

            callback.FieldUpdating(field);
        }

        private static void RaiseFieldUpdatedEvent(Field field, FieldUpdater updater)
        {
            IFieldUpdatingCallback callback = updater.Document.FieldOptions.FieldUpdatingCallback;
            if (callback == null)
                return;

            callback.FieldUpdated(field);
        }

        private void ProcessFieldResultChange(FieldUpdateContext context, bool notifyFieldResultChanged)
        {
            ReflowLayoutOrDefer(context.Field);

            // Notify the context.
            if (notifyFieldResultChanged)
                context.NotifyFieldAreaChanged(FieldArea.Result);

            if (context.HasParentContext)
            {
                // If the field is nested to another field's code, then its result affects string representation of
                // the corresponding field argument. Invalidate it, it to be re-read on the next demand if any.
                if (!FieldUtil.IsFieldResultInvisible(context.Field))
                    context.ParentArgument.InvalidateText();

                context.ParentContext.NotifyChildFieldUpdated(context);
            }
        }

        private void ReflowLayoutOrDefer(Field field)
        {
            // FOSS
        }

        private static bool RequiresImmediateReflowLayout(Field field)
        {
            if (field.Type != FieldType.FieldIndex)
                return false;

            NodeRange result = field.GetFieldResultRange();
            foreach (Node node in result)
            {
                if (node.NodeType == NodeType.Section)
                    return true;
            }

            return false;
        }

        internal static bool InsertFieldResultFieldsIfNeeded(FieldUpdateContext sourceContext)
        {
            if (!FieldUtil.RequiresUpdateFieldsInResult(sourceContext.Field.Type))
                return false;

            InsertFieldResultFieldHelper helper = new InsertFieldResultFieldHelper(sourceContext);
            helper.Extract(sourceContext.Field.GetFieldResultRange());
            return true;
        }

        private FieldUpdateContext InsertFieldResultField(FieldUpdateContext sourceContext, Field field)
        {
            BeginInsertFieldToUpdate();
            FieldUpdateContext context = sourceContext.InsertFieldResultContextAfter(field);
            EndInsertFieldToUpdate(context);

            return context;
        }

        /// <summary>
        /// Performs finalization steps of the specified field's update process.
        /// </summary>
        private static void EndUpdateField(FieldUpdateContext context, FieldUpdater updater)
        {
            // Notify the context.
            context.NotifyUpdatePerformed();

            PerformCleanup(context, updater);

            // If the context is topmost, then everything is done at this point and we can safely remove all of
            // the references to the context and its children, so GC could eat them.
            if (!context.HasParentContext)
            {
                ReleaseContextReferences(context);
                RemoveChild(context);
            }
        }

        private static void ReleaseContextReferences(FieldUpdateContext context)
        {
            if (context.HasChildren)
            {
                foreach (FieldUpdateContext childContext in context)
                    ReleaseContextReferences(childContext);
            }

            if (!context.IsUpdateSkipped)
                context.Field.EndUpdate();
        }

        private static void PerformCleanup(FieldUpdateContext context, FieldUpdater fieldUpdater)
        {
            // Retrieve paragraph before replace field with its result.
            Paragraph containingParagraph = context.Field.Start.ParentParagraph == context.Field.End.ParentParagraph
                ? context.Field.Start.ParentParagraph
                : null;

            bool removeFieldCodePerformed = PerformRemoveFieldCode(context, fieldUpdater);

            if (NeedRemoveEmptyParagraph(containingParagraph, context))
            {
                if (fieldUpdater.mFieldCleaner.RemoveEmptyParagraph(containingParagraph))
                    context.InvalidateParentContextFieldCode();
            }

            if (removeFieldCodePerformed)
                context.NotifyCleanupPerformed();
        }

        private static bool NeedRemoveEmptyParagraph(Paragraph paragraph, FieldUpdateContext context)
        {
            if (paragraph == null)
                return false;

            if ((context.CleanupActions & FieldUpdateCleanupActions.RemoveContainingParagraphIfEmpty) == 0)
                return false;

            if (!IsCleanableParagraph(paragraph, context))
                return false;

            if (!WordUtil.ParagraphCanBeRemoved(paragraph))
                return false;

            return true;
        }

        private static bool IsCleanableParagraph(Paragraph paragraph, FieldUpdateContext context)
        {
            bool punctuationMarksAllowed = (context.CleanupActions & FieldUpdateCleanupActions.RemoveContainingParagraphWithPunctuationMarks) != 0;
            for (Node childNode = paragraph.FirstChild; childNode != null; childNode = childNode.NextSibling)
            {
                if (NodeUtil.IsWhitespace(childNode))
                    continue;

                if (!punctuationMarksAllowed)
                    return false;

                if (childNode.NodeType != NodeType.Run)
                    return false;

                string childText = childNode.GetText();
                foreach (char c in childText)
                {
                    if (char.IsWhiteSpace(c))
                        continue;

                    if (StringUtil.IsPunctuationMark(c))
                        continue;

                    return false;
                }
            }

            return true;
        }

        private static bool PerformRemoveFieldCode(FieldUpdateContext context, FieldUpdater fieldUpdater)
        {
            if (context.IsTopmostCleanableContext)
                return PerformPostponedRemoveFieldCode(context, fieldUpdater);

            // WORDSNET-12486 Perform field code removing only for field with empty result. Otherwise we need invalidate parent field code.
            // But it makes mail merge extremely slow.
            if ((context.Field.Result == string.Empty) && RemoveFieldCode(context, fieldUpdater))
                return true;

            return false;
        }

        private static bool PerformPostponedRemoveFieldCode(FieldUpdateContext context, FieldUpdater fieldUpdater)
        {
            if (RemoveFieldCode(context, fieldUpdater))
                return true;

            if (context.HasChildren)
            {
                foreach (FieldUpdateContext childContext in context)
                    PerformPostponedRemoveFieldCode(childContext, fieldUpdater);
            }

            return false;
        }

        private static bool RemoveFieldCode(FieldUpdateContext context, FieldUpdater fieldUpdater)
        {
            if ((context.CleanupActions & FieldUpdateCleanupActions.RemoveFieldCode) != 0)
            {
                return fieldUpdater.mFieldCleaner.RemoveFieldCode(context);
            }

            return false;
        }

        /// <summary>
        /// Notifies the updater about that the specified field updated by it is replaced with its result
        /// or completely removed.
        /// </summary>
        internal void NotifyFieldReplaced(Field field, bool isFieldResultRemoved)
        {
            // Notify the context.
            FieldUpdateContext context = field.UpdateContext;
            context.NotifyFieldAreaChanged(FieldArea.Code);

            if (isFieldResultRemoved)
                ProcessFieldResultChange(context, true);

            // If the field could not be replaced safely (see FieldUpdateContext.IsSafeReplacingPossible for details),
            // then its replacing invalidates its parent field's code (if any).
            if (context.HasParentContext && !context.IsSafeReplacingPossible)
                context.ParentContext.Field.InvalidateFieldCodeCache();
        }

        /// <summary>
        /// Returns display range of the field. This method is called by layout whenever it needs to know visible field result.
        /// </summary>
        /// <returns>An object containing display range and its meaning or null if the existing field range
        /// should be displayed.</returns>
        internal static FieldDisplayResult GetFieldDisplayResult(
            Field field,
            FieldDisplayContext displayContext,
            NullableBool isTopmostField)
        {
            FieldDisplayResult result = GetFieldDisplayResultCore(field, displayContext, isTopmostField);

            // Forcibly call EndFieldUpdate if it was not called inside the invocation above.
            // This can happen if the fake result was used or the field does not have a separator.
            if (field.IsUpdating)
                EndUpdateField(field.UpdateContext, field.Updater);

            return result;
        }

        private static FieldDisplayResult GetFieldDisplayResultCore(
            Field field,
            FieldDisplayContext displayContext,
            NullableBool isTopmostField)
        {
            Debug.Assert(displayContext != null);

            // WORDSNET-18421 This handles situation when a FWR field contains a FCL field.
            FieldUpdateResults updateResults = UpdateField(field, displayContext, isTopmostField);

            return GetFieldDisplayResultCore(field, updateResults);
        }

        private static FieldDisplayResult GetFieldDisplayResultCore(Field field, FieldUpdateResults updateResults)
        {
            NodeRange fakeResult = field.GetFakeResult();
            if (fakeResult != null)
                return new FieldDisplayResult(fakeResult, FieldDisplayRangeType.FakeResult);

            // WORDSNET-6779 Ignore field without separator that somehow reached this point.
            if (!field.HasSeparator)
                return null;

            // If there was no actual update, return null to signalize no nodes were changed.
            if (updateResults == FieldUpdateResults.None)
                return null;

            if (FieldUtil.IsFieldResultInvisible(field))
                return null;

            if (field.IsRemoved || ((updateResults & FieldUpdateResults.FieldCodeChanged) != 0))
                return new FieldDisplayResult(field.GetFieldRange(), FieldDisplayRangeType.WholeField);

            return new FieldDisplayResult(field.GetFieldResultRange(), FieldDisplayRangeType.FieldResult);
        }

        /// <summary>
        /// Updates the specified field using the given display context. Updates the field in a full cycle if it is
        /// not being updated already. Otherwise, updates the field in a common way.
        /// </summary>
        private static FieldUpdateResults UpdateField(Field field, FieldDisplayContext displayContext, NullableBool isTopmostField)
        {
            // Ensure display context, it can not be null at this point.
            Debug.Assert(displayContext != null);

            // Only fields located in header/footer can be updated in a full cycle at this point.
            Debug.Assert(field.IsUpdating || field.IsInHeaderFooter);

            if (!field.IsUpdating)
                return UpdateFieldFullCycle(field, displayContext, isTopmostField);

            // If the field is being updated already, then the same display context should be passed during
            // its roundtrip. Ensure this.
            Debug.Assert(field.Updater.DisplayContext == displayContext);

            return UpdateFieldCore(field.UpdateContext);
        }

        /// <summary>
        /// Performs finalization steps for a field update session.
        /// </summary>
        private void FinalizeUpdateFields()
        {
            mTocEntriesBookmarkDeferredRemover.RemoveFrozenBookmarks();
            mFieldCleaner.FinalizeCleanup();
            RemoveDeferredFields();
            ClearBookmarkCache();
            ReflowLayoutIfNeeded();
            NotifyUpdateProgressChildrenFieldsAreUpdated();
            ReleaseFieldUpdateProgressContext();
        }

        /// <summary>
        /// Removes fields which removal was previously deferred.
        /// </summary>
        private void RemoveDeferredFields()
        {
            if ((mDeferredFieldsToRemove == null) || (mDeferredFieldsToRemove.Count == 0))
                return;

            foreach (Field field in mDeferredFieldsToRemove)
                field.Remove();

            mDeferredFieldsToRemove.Clear();
        }

        /// <summary>
        /// Clears the bookmark cache.
        /// </summary>
        private void ClearBookmarkCache()
        {
            if (!mInternalCreatedBookmarkCache)
                return;

            BookmarkCache.Clear();
        }

        /// <summary>
        /// Reflows document layout if it is present and this action was requested previously.
        /// </summary>
        private void ReflowLayoutIfNeeded()
        {
            if (!mNeedReflowLayout)
                return;

            ReflowLayout();
        }

        internal void ReflowLayout()
        {
            // FOSS

            mNeedReflowLayout = false;
        }

        /// <summary>
        /// Requests an external action to perform immediately.
        /// </summary>
        internal void RequestExternalAction(IExternalAction action)
        {
            RequestExternalAction(action, CurrentStage);
        }

        /// <summary>
        /// Requests an external action to perform on the specified field update stage.
        /// </summary>
        internal void RequestExternalAction(IExternalAction action, FieldUpdateStage stage)
        {
            if (ExternalActions.Add(stage, action) && (stage == CurrentStage))
                action.Perform();
        }

        /// <summary>
        /// Defers the specified field's removal until all of the fields being updated in this session
        /// are completely updated.
        /// </summary>
        internal void DeferRemove(Field field)
        {
            // Deferred removal is available only for fields which results are invisible.
            // Otherwise, it would corrupt the layout.
            Debug.Assert(FieldUtil.IsFieldResultInvisible(field));

            // Create on the first demand.
            if (mDeferredFieldsToRemove == null)
                mDeferredFieldsToRemove = new List<Field>();

            mDeferredFieldsToRemove.Add(field);
        }

        /// <summary>
        /// Gets a bookmark for the bookmark start from the cache.
        /// </summary>
        internal Bookmark GetCachedBookmark(string bookmarkName)
        {
            return BookmarkCache[bookmarkName];
        }

        /// <summary>
        /// Removes fields of the specified types contained within the given node.
        /// </summary>
        internal static void RemoveFields(Node node, params FieldType[] fieldTypes)
        {
            if ((fieldTypes == null) || (fieldTypes.Length == 0))
                return;

            // We should build an update tree before fields' removal, FieldReplacer to be capable to determine
            // whether a particular field should be escaped while its removal.
            FieldUpdater updater = new FieldUpdater(node.FetchDocument());
            updater.AddTopmostFieldsToUpdate(node);
            RemoveFields(updater, fieldTypes);
        }

        private static void RemoveFields(FieldUpdateContextContainer container, FieldType[] fieldTypes)
        {
            foreach (FieldUpdateContext context in container)
            {
                if (Array.IndexOf(fieldTypes, context.Field.Type) != -1)
                {
                    // Remove the field. Do not process child contexts since their fields are to be removed either.
                    context.Field.Remove();
                }
                else
                {
                    // Process child contexts.
                    RemoveFields(context, fieldTypes);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this update session is initiated by layout.
        /// </summary>
        internal bool IsUpdateInitiatedByLayout { get; }

        /// <summary>
        /// Gets display context defined for this session.
        /// </summary>
        internal FieldDisplayContext DisplayContext { get; private set; }

        /// <summary>
        /// Gets the current update stage.
        /// </summary>
        internal FieldUpdateStage CurrentStage
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether some field is being inserted to the field update tree at the moment.
        /// </summary>
        internal bool IsFieldToUpdateInserting { get; private set; }

        /// <summary>
        /// Gets a collection of <see cref="IFieldUpdateListener"/> objects attached to this instance.
        /// </summary>
        internal FieldUpdateListenerCollection Listeners { get; }

        /// <summary>
        /// Gets a collection of <see cref="IFieldUpdateDataProvider"/> objects attached to this instance.
        /// </summary>
        internal FieldUpdateDataProvider DataProviders { get; }

        internal BookmarkCache BookmarkCache { get; }

        internal Document Document { get; }

        private ExternalActionCollection ExternalActions
        {
            get { return mSession.ExternalActions; }
        }

        internal HiddenAttributeCache HiddenAttributeCache
        {
            get { return mSession.HiddenAttributeCache; }
        }

        internal ImportedStylesIstdsCollector ImportedStylesIstdsCollector
        {
            get { return mSession.ImportedStylesIstdsCollector; }
        }

        internal AutoTextEntryExtractor AutoTextEntryExtractor
        {
            get { return mSession.AutoTextEntryExtractor; }
        }

        internal void FreezeTocEntryBookmark(string bookmarkName)
        {
            mTocEntriesBookmarkDeferredRemover.RegisterBookmark(bookmarkName);
        }

        /// <summary>
        /// Helps to add a range of topmost fields to update them by the updater.
        /// </summary>
        private class AddTopmostFieldHelper : FieldExtractor
        {
            internal AddTopmostFieldHelper(FieldUpdater updater, NullableBool isTopmostField)
            {
                mUpdater = updater;
                mIsTopmostField = isTopmostField;
            }

            protected override void OnFieldExtracted()
            {
                if (!IsInField)
                    mUpdater.AddTopmostFieldToUpdate(CurrentField, mIsTopmostField);
            }

            private readonly FieldUpdater mUpdater;
            private readonly NullableBool mIsTopmostField;
        }

        /// <summary>
        /// Helps to remove a range of topmost fields to prevent update them by the updater.
        /// </summary>
        private class RemoveTopmostFieldHelper : FieldExtractor
        {
            internal RemoveTopmostFieldHelper()
            {
                FieldStarts = new List<FieldStart>();
            }

            protected override void OnFieldExtracted()
            {
                if (!IsInField)
                    FieldStarts.Add(CurrentField.Start);
            }

            public List<FieldStart> FieldStarts { get; }
        }

        /// <summary>
        /// Helps to add topmost fields contained in another field's result to update them by the updater.
        /// </summary>
        private class InsertFieldResultFieldHelper : FieldExtractor
        {
            internal InsertFieldResultFieldHelper(FieldUpdateContext sourceContext)
            {
                mSourceContext = sourceContext;
            }

            protected override void OnFieldExtracted()
            {
                if (IsInField)
                    return;

                FieldUpdateContext newContext = mSourceContext.Updater.InsertFieldResultField(mSourceContext, CurrentField);
                if (newContext != null)
                    mSourceContext = newContext;
            }

            private FieldUpdateContext mSourceContext;
        }

        private readonly IFieldCleaner mFieldCleaner;
        private readonly bool mInternalCreatedBookmarkCache;
        private readonly TocEntriesBookmarkDeferredRemover mTocEntriesBookmarkDeferredRemover = new TocEntriesBookmarkDeferredRemover();

        private readonly bool mIsUpdateBeforeSave;
        private List<Field> mDeferredFieldsToRemove;
        private readonly FieldUpdateSession mSession;
        private bool mNeedReflowLayout;
        private readonly bool mIsUpdateDirtyFields;

#if DEBUG
        public override string ToString()
        {
            return string.Format("Updater [Stage: {0}]", CurrentStage);
        }
#endif
    }
}
