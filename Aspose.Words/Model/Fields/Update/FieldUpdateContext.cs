// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/08/2009 by Dmitry Vorobyev

using System;
using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Passes data corresponding to a single field among objects in the field evaluation module. Represents a branch or
    /// leaf node of a field update tree. See <see cref="FieldUpdateContextContainer"/> remarks for details.
    /// </summary>
    internal sealed class FieldUpdateContext : FieldUpdateContextContainer, IDisposable
    {
        /// <summary>
        /// Creates and appends the topmost context to the field update tree of the specified updater.
        /// Returns the created instance or <c>null</c> if the context creating was rejected by some update listener.
        /// </summary>
        internal static FieldUpdateContext AppendTopmostContext(
            Field field,
            FieldUpdater updater,
            NullableBool isTopmostField)
        {
            FieldUpdateContext context = InsertContext(field, false, updater, null, true, isTopmostField);
            if (context != null)
                context.IsSafeReplacingPossible = true;

            return context;
        }

        public void Dispose()
        {
            CloseResources();
        }

        private void CloseResources()
        {
            if (mOldResultRemover != null)
            {
                mOldResultRemover.Dispose();
                mOldResultRemover = null;
            }
        }

        /// <summary>
        /// Creates and inserts a child context to the specified parent container before or after the given
        /// reference child context.
        /// Returns the created instance or <c>null</c> if the context creating was rejected by some update listener.
        /// </summary>
        private static FieldUpdateContext InsertContext(
            Field field,
            bool isInFieldResult,
            FieldUpdateContextContainer parent,
            FieldUpdateContext refChild,
            bool isAfter,
            NullableBool isTopmostField)
        {
            FieldUpdater updater = GetUpdater(parent);
            Field parentField = IsContext(parent)
                ? ((FieldUpdateContext)parent).Field
                : null;
            FieldUpdateListenerResponse listenerResponse = updater.Listeners.NotifyUpdating(field, parentField);

            if (listenerResponse != null)
            {
                field = listenerResponse.Field;
            }
            else
            {
                listenerResponse = FieldUpdateListenerResponse.Default;
            }

            FieldUpdateContext context = new FieldUpdateContext(
                field,
                isInFieldResult,
                listenerResponse,
                parentField != null ? NullableBool.False : isTopmostField);

            InsertChild(parent, refChild, context, isAfter);
            context.RegisterCleanupActions(listenerResponse.BaseCleanupActions);

            if (!context.IsUpdateSkipped)
            {
                // The following call fills the field update tree downward recursively.
                context.Field.BeginUpdate(context);

                if (updater.Document.FieldUpdateProgressContext != null)
                    updater.Document.FieldUpdateProgressContext.OnFieldToBeUpdated();
            }

            if (FieldUtil.GetEffectiveFieldLocked(field))
                FieldUpdater.InsertFieldResultFieldsIfNeeded(context);

            return context;
        }

        /// <summary>
        /// Returns an updater for the specified container.
        /// </summary>
        private static FieldUpdater GetUpdater(FieldUpdateContextContainer container)
        {
            while (IsContext(container))
                container = container.Parent;

            return (FieldUpdater)container;
        }

        /// <summary>
        /// Returns a value indicating whether the specified container is context.
        /// </summary>
        private static bool IsContext(FieldUpdateContextContainer container)
        {
            return (container.Parent != null);
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        private FieldUpdateContext(
            Field field,
            bool isInFieldResult,
            FieldUpdateListenerResponse listenerResponse,
            NullableBool isTopmostField)
        {
            CleanupActions = FieldUpdateCleanupActions.None;
            UpdateResults = FieldUpdateResults.None;

            Field = field;
            IsInFieldResult = isInFieldResult;
            RequiresImmediateUpdate = listenerResponse.RequiresImmediateUpdate;
            mIsTopmostField = isTopmostField;
            IsAutoUpdatableBeforeSave = NullableBool.NotDefined;

            switch (listenerResponse.PreferredStageStrategy)
            {
                case FieldUpdatePreferredStageStrategy.MainLoop:
                    StageToDefer = FieldUpdateStage.MainLoop;
                    break;
                case FieldUpdatePreferredStageStrategy.Skip:
                    // WORDSNET-9610 We should keep a context for a field which update is rejected by some listener in field update tree.
                    // Otherwise, collisions may take place in case when a nested mail merge region captures an outer field either.
                    // The example:
                    //
                    // [ IF [ MERGEFIELD parentField ] = 1 [ MERGEFIELD TableStart:ChildTable ]
                    // [ MERGEFIELD childField ]
                    // [ MERGEFIELD TableEnd:ChildTable ]" ]
                    //
                    // In this case IF's field code is invalidated when parentField is merged. And if we did not keep a context
                    // for childField in a tree, then childField would be recognized as newly added field (see NotifyChildFieldEncountered)
                    // and merged in the context of parent data source, which is wrong.
                    //
                    // So instantiate the context but remember that an update of the corresponding field should be skipped.
                    IsUpdateSkipped = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Notifies the context about that the corresponding field's code is about to be parsed.
        /// </summary>
        internal void NotifyFieldCodeParsing()
        {
            // We should recalculate aggregate data, so reset it here.
            ResetAggregateData();

            // Deactivate children if any. Deactivation means, that child contexts are unbound from the corresponding
            // parent field arguments. They are not removed from the parent context not to lose child fields' update
            // states. They are to be activated (i.e. bound to some parent field arguments) while parent field code
            // parsing. Inactive child contexts will be removed from the parent context only after the parent field
            // code is completely parsed.
            if (HasChildren)
            {
                foreach (FieldUpdateContext childContext in this)
                {
                    childContext.mParentArgument = null;
                    childContext.IsSafeReplacingPossible = childContext.IsInFieldResult;
                }
            }
        }

        /// <summary>
        /// Restores initial state of this context's aggregate data (i.e. when there is no child context detected).
        /// </summary>
        private void ResetAggregateData()
        {
            mHasAnyValueProvidedByLayout = FieldUtil.IsComputedByLayout(Field.Type);
        }

        /// <summary>
        /// Notifies the context about that the corresponding field's direct child field has been encountered
        /// while field's code parsing.
        /// </summary>
        internal void NotifyChildFieldEncountered(FieldStart start, FieldSeparator separator, FieldEnd end)
        {
            Debug.Assert(start != null);
            Debug.Assert(end != null);
            Debug.Assert(end.HasSeparator == (separator != null));

            if (!TryActivateInactiveChildContext(start))
                TryActivateNewChildContext(start, separator, end);
        }

        /// <summary>
        /// Searches for the corresponding inactive child context and activates it if found. Returns a value indicating
        /// whether the context was found and activated.
        /// </summary>
        private bool TryActivateInactiveChildContext(FieldStart start)
        {
            // If the context does not have any children or it is being inserted to a field update tree, it definitely
            // does not contain inactive children.
            if (!HasChildren || Updater.IsFieldToUpdateInserting)
                return false;

            ChildContextActivationStage stage = mLastActivatedChildContext == null
                ? ChildContextActivationStage.Search
                : ChildContextActivationStage.InitSearch;

            foreach (FieldUpdateContext childContext in this)
            {
                switch (stage)
                {
                    case ChildContextActivationStage.InitSearch:
                    {
                        // We should not consider inactive child contexts before the last encountered one,
                        // since child contexts' order is important.
                        if (childContext == mLastActivatedChildContext)
                            stage = ChildContextActivationStage.Search;
                        break;
                    }
                    case ChildContextActivationStage.Search:
                    {
                        if (childContext.Field.Start == start)
                        {
                            // Corresponding inactive child context is found. Activate it.
                            ActivateChildContext(childContext);

                            // If the field requires fields in its result to be updated, search and activate
                            // their contexts either.
                            if (!FieldUtil.RequiresUpdateFieldsInResult(childContext.Field.Type))
                                return true;

                            stage = ChildContextActivationStage.ActivateDependants;
                        }
                        break;
                    }
                    case ChildContextActivationStage.ActivateDependants:
                    {
                        // Quit on the first independent child context encountered.
                        if (!childContext.IsInFieldResult)
                            return true;

                        ActivateChildContext(childContext);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // If we have stopped while activating dependent child contexts, then the activation succeeded.
            // Otherwise, corresponding inactive child context was not found.
            return stage == ChildContextActivationStage.ActivateDependants;
        }

        /// <summary>
        /// Tries to create and activate a new child context. Returns a value indicating whether the context
        /// was created and activated.
        /// </summary>
        private void TryActivateNewChildContext(FieldStart start, FieldSeparator separator, FieldEnd end)
        {
            Field childField = FieldFactory.CreateField(start, separator, end);

            // If we have encountered any child context we should add new child context after the last encountered one.
            // Otherwise, we should prepend new child context, since this context can contain inactive children.
            bool isAfter = mLastActivatedChildContext != null;

            FieldUpdateContext childContext = InsertContext(childField, false, this, mLastActivatedChildContext, isAfter, mIsTopmostField);
            if (childContext == null)
                return;

            ActivateChildContext(childContext);
        }

        /// <summary>
        /// Activates the specified child context (i.e. binds it to the pending parent field argument).
        /// </summary>
        private void ActivateChildContext(FieldUpdateContext childContext)
        {
            childContext.mParentArgument = gPendingArgumentPlaceholder;
            if (IsInDoubleQuotes)
                childContext.IsSafeReplacingPossible = true;

            // Make this context consider the specified child context's aggregate data as well.
            AggregateChildContextData(childContext);

            // Remember the last activated child context so the next one could be inserted after it.
            mLastActivatedChildContext = childContext;
        }

        private void AggregateChildContextData(FieldUpdateContext childContext)
        {
            if (childContext.mHasAnyValueProvidedByLayout)
                mHasAnyValueProvidedByLayout = true;
        }

        /// <summary>
        /// Notifies the context about that the field argument has been created while field code parsing.
        /// </summary>
        internal void NotifyArgumentAdded(FieldArgument argument)
        {
            if (!HasChildren)
                return;

            // Replace pending argument placeholders with the real argument for all child contexts.
            foreach (FieldUpdateContext childContext in this)
            {
                if (childContext.mParentArgument == gPendingArgumentPlaceholder)
                    childContext.mParentArgument = argument;
            }
        }

        /// <summary>
        /// Notifies the context about that the corresponding field's code has been parsed.
        /// </summary>
        internal void NotifyFieldCodeParsed()
        {
            // Reset the last activated child context.
            mLastActivatedChildContext = null;

            if (HasChildren)
            {
                foreach (FieldUpdateContext childContext in this)
                {
                    // At this point we should replace all pending argument placeholders with real arguments. If this condition
                    // is not met, then we must deal with a field, which does not provide the field's type through its code and
                    // hence its first argument is recognized as field type by the field code parser. So we have to capture
                    // the whole field in this case to be able to invalidate its cached field type when needed.
                    if (childContext.mParentArgument == gPendingArgumentPlaceholder)
                        childContext.mParentArgument = new UnknownFieldArgument(Field);

                    // If the context was populated by children before the field code parse then it can contain child contexts
                    // for fields, which were removed from the parent field code (i.e. inactive children). Remove them here.
                    if (childContext.mParentArgument == null)
                        RemoveChild(childContext);
                }
            }

            // If the context is being added to a field update tree, its aggregate data is calculated "automatically".
            // In other cases we should manually recalculate aggregate data for all ancestor contexts.
            if (!Updater.IsFieldToUpdateInserting)
            {
                for (FieldUpdateContext parentContext = ParentContext;
                     parentContext != null;
                     parentContext = parentContext.ParentContext)
                {
                    parentContext.RecalcAggregateData();
                }
            }
        }

        private void RecalcAggregateData()
        {
            ResetAggregateData();

            if (HasChildren)
            {
                foreach (FieldUpdateContext childContext in this)
                    AggregateChildContextData(childContext);
            }
        }

        /// <summary>
        /// Creates and inserts a context for the specified field contained in another field's result to a field update
        /// tree after this context.
        /// Returns the created instance or <c>null</c> if the context creating was rejected by some update listener.
        /// </summary>
        /// <remarks>
        /// Contexts corresponding to fields which are located in other fields' results are inserted as siblings to these
        /// fields' contexts because of the way they affect parent fields' codes. Look at an example. Field A contains
        /// field B in its field code. Field B contains field C in its result. When field C is updated it does not affect
        /// field B's field code. But it affects field A's field code in the same way as updating of field B would.
        /// That's why it is simpler and more convenient to consider contexts of field B and field C as siblings rather
        /// than a parent and a child.
        /// </remarks>
        internal FieldUpdateContext InsertFieldResultContextAfter(Field field)
        {
            FieldUpdateContext context = InsertContext(field, true, Parent, this, true, mIsTopmostField);
            if (context == null)
                return null;

            context.mParentArgument = mParentArgument;
            context.IsSafeReplacingPossible = true; // This is because the field is contained in another field's result.

            // Aggregate new context's data to all ancestor contexts.
            for (FieldUpdateContext childContext = context, parentContext = context.ParentContext;
                 parentContext != null;
                 childContext = parentContext, parentContext = parentContext.ParentContext)
            {
                parentContext.AggregateChildContextData(childContext);
            }

            return context;
        }

        /// <summary>
        /// Notifies the context about that all of the corresponding field's child fields which should be updated
        /// according to the field's update logic have been updated.
        /// </summary>
        internal void NotifyAllChildFieldsUpdated()
        {
            // It is important to calculate the field update stage after all of its child fields are updated,
            // since their results can affect the calculation.
            FieldUpdateStage stage = Field.GetUpdateStage();
            if (StageToDefer < stage)
                StageToDefer = stage;

            AreAllChildFieldsUpdated = true;

            NotifyUpdateProgressChildrenFieldsAreUpdated();
        }

        internal void NotifyChildFieldUpdated(FieldUpdateContext childContext)
        {
            Field.NotifyChildFieldUpdated(childContext.ParentArgument);
        }

        /// <summary>
        /// Notifies the context about that the corresponding field has been updated.
        /// </summary>
        internal void NotifyUpdatePerformed()
        {
            IsUpdatePerformed = true;
        }

        /// <summary>
        /// Notifies the context about that the corresponding field's area has been changed.
        /// </summary>
        internal void NotifyFieldAreaChanged(FieldArea area)
        {
            switch (area)
            {
                case FieldArea.Code:
                    UpdateResults |= FieldUpdateResults.FieldCodeChanged;
                    break;
                case FieldArea.Result:
                    UpdateResults |= FieldUpdateResults.FieldResultChanged;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("area");
            }

            // Ancestor fields' codes are changed regardless of what area is changed for a descendant field.
            for (FieldUpdateContext parentContext = ParentContext;
                 parentContext != null;
                 parentContext = parentContext.ParentContext)
            {
                parentContext.UpdateResults |= FieldUpdateResults.FieldCodeChanged;
            }
        }

        /// <summary>
        /// Registers cleanup actions for the context to be performed when appropriate.
        /// </summary>
        /// <remarks>
        /// Note, that we allow to register cleanup actions for the context, for which this is requested,
        /// regardless of its <see cref="IsCleanable"/> property value. This implements the principle:
        /// if the cleanup is requested then it should be performed as the caller knows, what it is doing.
        /// </remarks>
        internal void RegisterCleanupActions(FieldUpdateCleanupActions actions)
        {
            CleanupActions |= actions;

            if (((actions & FieldUpdateCleanupActions.RemoveContainingFieldCode) == 0) || !HasParentContext)
            {
                // If the cleanup actions do not affect parent field code, simply apply them as is.
                return;
            }

            CleanupActions |= (actions & FieldUpdateCleanupActions.RemoveFieldResult);

            // Rearrange cleanup actions to apply them to the topmost cleanable context.
            actions ^= FieldUpdateCleanupActions.RemoveContainingFieldCode;
            actions |= FieldUpdateCleanupActions.RemoveFieldCode;

            FieldUpdateContext topmostCleanableContext = GetTopmostCleanableContextOrSelf();

            bool applyRemoveFieldResultActionToTopmostContext = ApplyRemoveFieldResultActionToTopmostContext(actions, topmostCleanableContext.CleanupActions);

            topmostCleanableContext.CleanupActions |= actions;
            // WORDSNET-11549 Apply RemoveFieldResult action only if requested by all nested cleanable contexts.
            if (!applyRemoveFieldResultActionToTopmostContext)
                topmostCleanableContext.CleanupActions &= ~FieldUpdateCleanupActions.RemoveFieldResult;

            // WORDSNET-23511 Apply RemoveContainingParagraphIfEmpty action to all parent contexts.
            BubbleRemoveContainingParagraphIfEmptyAction(actions);
        }

        private void BubbleRemoveContainingParagraphIfEmptyAction(FieldUpdateCleanupActions actions)
        {
            if ((actions & FieldUpdateCleanupActions.RemoveContainingParagraphIfEmpty) == 0)
                return;

            actions &=
                FieldUpdateCleanupActions.RemoveFieldCode |
                FieldUpdateCleanupActions.RemoveContainingParagraphIfEmpty |
                FieldUpdateCleanupActions.RemoveContainingParagraphWithPunctuationMarks;

            if (actions == FieldUpdateCleanupActions.None)
                return;

            foreach (FieldUpdateContext context in GetAncestralCleanableContexts())
                context.CleanupActions |= actions;
        }

        private IEnumerable<FieldUpdateContext> GetAncestralCleanableContexts()
        {
            List<FieldUpdateContext> result = new List<FieldUpdateContext>();

            FieldUpdateContext parentContext = ParentContext;

            while ((parentContext != null) && parentContext.IsCleanable)
            {
                result.Add(parentContext);
                parentContext = parentContext.ParentContext;
            }

            return result;
        }

        private static bool ApplyRemoveFieldResultActionToTopmostContext(FieldUpdateCleanupActions actions, FieldUpdateCleanupActions topmostContextActions)
        {
            if ((actions & FieldUpdateCleanupActions.RemoveFieldResult) == 0)
                return false;

            return (topmostContextActions & FieldUpdateCleanupActions.RemoveFieldResult) == FieldUpdateCleanupActions.RemoveFieldResult ||
                   topmostContextActions == FieldUpdateCleanupActions.None;
        }

        internal void InvalidateParentContextFieldCode()
        {
            if (!HasParentContext)
                return;

            ParentContext.InvalidateFieldCode();
        }

        internal void InvalidateFieldCode()
        {
            Field.InvalidateFieldCodeCache();
            Field.ParseFieldCode();

            InvalidateParentContextFieldCode();
        }

        private FieldUpdateContext GetTopmostCleanableContextOrSelf()
        {
            FieldUpdateContext topmostCleanableContext = this;

            while ((topmostCleanableContext.ParentContext != null) && topmostCleanableContext.ParentContext.IsCleanable)
                topmostCleanableContext = topmostCleanableContext.ParentContext;

            return topmostCleanableContext;
        }

        /// <summary>
        /// Notifies the context about that all of the registered cleanup actions have been performed.
        /// </summary>
        internal void NotifyCleanupPerformed()
        {
            // Theoretically a context subtree can contain multiple contexts considered as topmost cleanup
            // contexts for different descendant contexts and hence the cleanup can be requested multiple
            // times for a single context. We need to ensure, that cleanup is performed only once for every
            // particular context, so we need to reset cleanup actions after the cleanup is performed.
            CleanupActions = FieldUpdateCleanupActions.None;
        }

        internal IDisposable RemoveOldResultSafe()
        {
            if (mOldResultRemover == null)
            {
                mOldResultRemover = new NodeRemover(Field.GetFieldResultRange(),
                    FieldUtil.GetFieldOldResultRemovalNodeJoinMode(Field), false);
                mOldResultRemover.RemoveCore();
            }

            return new FieldOldResultNodesCombiner(this);
        }

        internal void JoinOldResultNodes()
        {
            if (mOldResultRemover != null)
                mOldResultRemover.JoinNodes(FieldUtil.GetFieldResultRemovalNodeJoinMode(Field));
            CloseResources();
        }

        /// <summary>
        /// Gets the field being updated.
        /// </summary>
        internal Field Field
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the corresponding field's update should be skipped.
        /// </summary>
        internal bool IsUpdateSkipped
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the corresponding field is contained within another field's result.
        /// </summary>
        internal bool IsInFieldResult { get; }

        /// <summary>
        /// Gets a value indicating whether the corresponding field should be updated on the stage where it is
        /// supposed to regardless of any conditions.
        /// </summary>
        internal bool RequiresImmediateUpdate { get; }

        /// <summary>
        /// Gets a field update stage where the corresponding field should be updated.
        /// </summary>
        internal FieldUpdateStage StageToDefer { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the corresponding field is topmost field.
        /// Non topmost context always refers to non topmost field.
        /// But the topmost context may refer to non topmost field in some cases (i.e. if it's updated directly).
        /// </summary>
        /// <seealso cref="Aspose.Words.Fields.Field.IsTopmostField()"/>
        internal bool IsTopmostField
        {
            get
            {
                if (HasParentContext)
                    return false;

                if (mIsTopmostField == NullableBool.NotDefined)
                {
                    mIsTopmostField = Field.IsTopmostField()
                        ? NullableBool.True
                        : NullableBool.False;
                }

                return mIsTopmostField == NullableBool.True;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this context has parent context. This is <c>false</c> only for
        /// the topmost contexts.
        /// </summary>
        internal bool HasParentContext
        {
            get { return Parent != null && IsContext(Parent); }
        }

        /// <summary>
        /// Gets the parent context.
        /// </summary>
        internal FieldUpdateContext ParentContext
        {
            get { return HasParentContext ? (FieldUpdateContext)Parent : null; }
        }

        internal FieldUpdateContext TopmostParentContextOrSelf
        {
            get { return HasParentContext ? ParentContext.TopmostParentContextOrSelf : this; }
        }

        /// <summary>
        /// Gets a value indicating whether this context is the topmost cleanable context when looking upward
        /// starting from the current context.
        /// </summary>
        /// <remarks>
        /// A cleanable context is a context to which cleanup actions can be applied.
        /// </remarks>
        internal bool IsTopmostCleanableContext
        {
            get { return ((this == GetTopmostCleanableContextOrSelf()) && IsCleanable); }
        }

        /// <summary>
        /// Gets a value indicating whether this context is cleanable.
        /// </summary>
        /// <remarks>
        /// A cleanable context is a context to which cleanup actions can be applied.
        /// </remarks>
        private bool IsCleanable
        {
            get
            {
                // Cleanup actions were forcibly set for the context, so its cleanability is overridden.
                if (CleanupActions != FieldUpdateCleanupActions.None)
                    return true;

                // Fields without separator are not cleanable. MS Word does it in this way.
                if (FieldUtil.GetSeparatorPresence(Field.Type) == FieldSeparatorPresence.Never)
                    return false;

                // WORDSNET-27036 MS Word does not cleanup hyperlinks.
                if (Field.Type == FieldType.FieldHyperlink)
                    return false;

                // We forcibly skip cleanup for fields in header/footer containing child FCL fields at the same time.
                // There is no way to cleanup such fields in AW at the moment.
                // It is possible to implement it, but it is quite cumbersome, so let's wait for a client request.
                return (RequiresImmediateUpdate || !IsUpdatedByLayoutExternally);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the corresponding field should be updated by layout externally,
        /// i.e. layout should initiate a distinct update session to update the field.
        /// </summary>
        internal bool IsUpdatedByLayoutExternally
        {
            get { return (mHasAnyValueProvidedByLayout && Field.IsInHeaderFooter && !Updater.IsUpdateInitiatedByLayout); }
        }

        internal FieldUpdater Updater
        {
            get { return GetUpdater(Parent); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current field token encountered while the corresponding field's code
        /// parsing is located in double quotes.
        /// </summary>
        internal bool IsInDoubleQuotes { get; set; }

        /// <summary>
        /// Gets a parent field's argument to which the corresponding field is related.
        /// </summary>
        internal IFieldArgument ParentArgument
        {
            get { return (IFieldArgument)mParentArgument; }
        }

        internal FieldUpdateResults UpdateResults { get; private set; }

        /// <summary>
        /// Gets cleanup actions registered for the context.
        /// </summary>
        internal FieldUpdateCleanupActions CleanupActions { get; private set; }

        /// <summary>
        /// Gets or sets child fields' update stage. This is used only for contexts which corresponding fields
        /// support conditional update.
        /// </summary>
        internal FieldChildUpdateStage ChildUpdateStage { get; set; }

        internal FieldUpdateStrategy FieldUpdateStrategy
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the corresponding field can be replaced with its result or completely removed
        /// without any harm to the parent field's code (if any). This is true, if the field is enclosed in double quotes
        /// or contained within another field's result.
        /// </summary>
        internal bool IsSafeReplacingPossible { get; private set; }

        /// <summary>
        /// Gets a value indicating whether all of the corresponding field's child fields which should be updated
        /// according to the field's update logic were updated.
        /// </summary>
        internal bool AreAllChildFieldsUpdated
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the corresponding field was updated.
        /// </summary>
        internal bool IsUpdatePerformed
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get;
            private set;
        }

        internal NullableBool IsAutoUpdatableBeforeSave { get; set; }

        internal void NotifyUpdateProgressCurrentFieldIsUpdated()
        {
            if (mIsUpdateProgressNotified)
                return;

            if (!Field.IsUpdating)
                return;

            if (Updater.Document.FieldUpdateProgressContext == null)
                return;

            Updater.Document.FieldUpdateProgressContext.OnFieldUpdated();
            Updater.Document.FieldUpdateProgressContext.NotifyInternal();

            mIsUpdateProgressNotified = true;
        }

        /// <summary>
        /// Represents a field argument of a field that does not provide its type through its field code.
        /// </summary>
        private class UnknownFieldArgument : IFieldArgument
        {
            internal UnknownFieldArgument(Field field)
            {
                mField = field;
            }

            void IFieldArgument.InvalidateText()
            {
                // If the field does not provide its type through its field code then the field's argument text is contained
                // in FieldCode.FieldType. So we have to invalidate the whole field code cache to make it refresh on request.
                // Inefficient, but it seems like we have no other options. Moreover, the case is quite rare.
                mField.InvalidateFieldCodeCache();
            }

            private readonly Field mField;
        }

        private sealed class FieldOldResultNodesCombiner : IDisposable
        {
            private readonly FieldUpdateContext mFieldUpdateContext;

            public FieldOldResultNodesCombiner(FieldUpdateContext fieldUpdateContext)
            {
                mFieldUpdateContext = fieldUpdateContext;
            }

            public void Dispose()
            {
                mFieldUpdateContext.JoinOldResultNodes();
            }
        }

        /// <summary>
        /// Specifies a stage of an inactive child context activation process.
        /// </summary>
        private enum ChildContextActivationStage
        {
            /// <summary>
            /// Specifies that an inactive child context search is about to be started.
            /// </summary>
            InitSearch,
            /// <summary>
            /// Specifies that an inactive child context is being searched for.
            /// </summary>
            Search,
            /// <summary>
            /// Specifies that dependent inactive child contexts are being activated.
            /// </summary>
            ActivateDependants
        }

        private bool mHasAnyValueProvidedByLayout;
        private bool mIsUpdateProgressNotified;
        private NodeRemover mOldResultRemover;
        private FieldUpdateContext mLastActivatedChildContext;
        private object mParentArgument;
        private NullableBool mIsTopmostField;

        private static readonly object gPendingArgumentPlaceholder = new object();

#if DEBUG
        internal override void dd()
        {
            if (IsUpdateSkipped)
                return;

            base.dd();
        }

        public override string ToString()
        {
            string fieldCode = Field.GetFieldCode().Trim().Replace(ControlChar.ParagraphBreak, @"\r");
            return string.Format("{0} [Strategy: {1}; ChildFieldsUpdated: {2}; Skipped: {3}; Performed: {4}]",
                StringUtil.Ellipsisize(fieldCode, 25),
                FieldUpdateStrategy,
                AreAllChildFieldsUpdated,
                IsUpdateSkipped,
                IsUpdatePerformed);
        }
#endif
    }
}
