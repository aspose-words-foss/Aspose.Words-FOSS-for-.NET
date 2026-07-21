// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/06/2010 by Dmitry Vorobyev

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// When implemented, listens to field update events.
    /// </summary>
    internal interface IFieldUpdateListener
    {
        /// <summary>
        /// Called when a field is about to be updated. Also controls whether it should happen.
        /// </summary>
        /// <returns>A value indicating what to do with the field.</returns>
        [JavaThrows(true)]
        FieldUpdateListenerResponse NotifyUpdating(Field field, Field parentField);

        /// <summary>
        /// Called when a field has been updated with specified strategy.
        /// </summary>
        [JavaThrows(true)]
        void NotifyUpdated(FieldUpdateContext context, FieldUpdateStrategy strategy);

        /// <summary>
        /// Called when a field is updating.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if the field update was performed by listener and the field should not be updated in common way.
        /// </returns>
        [JavaThrows(true)]
        bool Update(FieldUpdateContext context);
    }
}
