// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/12/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    internal class FieldUpdateListenerResponse
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldUpdateListenerResponse(
            Field field,
            FieldUpdatePreferredStageStrategy preferredStageStrategy,
            bool requiresImmediateUpdate,
            FieldUpdateCleanupActions baseCleanupActions)
        {
            Field = field;
            PreferredStageStrategy = preferredStageStrategy;
            RequiresImmediateUpdate = requiresImmediateUpdate;
            BaseCleanupActions = baseCleanupActions;
        }

        /// <summary>
        /// Gets a field to update.
        /// </summary>
        internal Field Field { get; }

        /// <summary>
        /// Gets a value specifying how to update the corresponding field.
        /// </summary>
        internal FieldUpdatePreferredStageStrategy PreferredStageStrategy { get; }

        /// <summary>
        /// Gets a value indicating whether the corresponding field should be updated on the stage where it is
        /// supposed to regardless of any conditions.
        /// </summary>
        internal bool RequiresImmediateUpdate { get; }

        /// <summary>
        /// Gets base cleanup actions to be applied to the corresponding field.
        /// </summary>
        internal FieldUpdateCleanupActions BaseCleanupActions { get; }

        /// <summary>
        /// An instance of the class representing "no response" situation. Note, that its <see cref="Field"/> value is null.
        /// </summary>
        internal static readonly FieldUpdateListenerResponse Default = new FieldUpdateListenerResponse(
            null,
            FieldUpdatePreferredStageStrategy.MainLoop,
            false,
            FieldUpdateCleanupActions.None);
    }
}
