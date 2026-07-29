// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/11/2013 by Ivan Lyagin

using System.Collections.Generic;


namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a collection of <see cref="IExternalAction"/> instances distributed by field update stages.
    /// </summary>
    internal class ExternalActionCollection
    {
        /// <summary>
        /// Appends the specified <see cref="IExternalAction"/> instance for the given field update stage
        /// to the collection. Returns a value indicating whether the external action was added. Its adding
        /// can be rejected by the collection if an instance of <see cref="IExternalAction"/> with the same
        /// <see cref="IExternalAction.Id"/> has been already added to the collection for the same field
        /// update stage.
        /// </summary>
        internal bool Add(FieldUpdateStage stage, IExternalAction action)
        {
            List<IExternalAction> actions = GetActions(stage);
            if (actions == null)
            {
                // Create on the first demand.
                actions = new List<IExternalAction>();
                SetActions(stage, actions);
            }
            else
            {
                foreach (IExternalAction actionToTest in actions)
                {
                    // Do not allow adding of an external action with the same id.
                    if (action.Id == actionToTest.Id)
                        return false;
                }
            }

            actions.Add(action);
            return true;
        }

        /// <summary>
        /// Performs all external actions for the specified field update stage in the order of their appearance.
        /// </summary>
        internal void Perform(FieldUpdateStage stage)
        {
            IList<IExternalAction> actions = GetActions(stage);
            if (actions == null)
                return;

            foreach (IExternalAction action in actions)
                action.Perform();
        }

        private List<IExternalAction> GetActions(FieldUpdateStage stage)
        {
            return (List<IExternalAction>)mActionsByStages[(int)stage];
        }

        private void SetActions(FieldUpdateStage stage, List<IExternalAction> externalActions)
        {
            mActionsByStages[(int)stage] = externalActions;
        }

        private readonly object[] mActionsByStages = new object[gFieldUpdateStageCount];

#if !CPLUSPLUS
        private static readonly int gFieldUpdateStageCount =
            EnumUtilPal.GetEffectiveArrayLength(FieldUpdateStage.DeferredUpdateLayout.GetType(), 3);
#else
        private static readonly int gFieldUpdateStageCount = 3;
#endif
    }
}
