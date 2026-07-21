// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/11/2013 by Ivan Lyagin

using System;
using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a collection of <see cref="IFieldUpdateListener"/> instances.
    /// </summary>
    internal class FieldUpdateListenerCollection
    {
        /// <summary>
        /// Adds the specified field update listener to the collection.
        /// </summary>
        internal void Add(IFieldUpdateListener listener)
        {
            if (IsEmpty)
                mListeners = new List<IFieldUpdateListener>(); // Create on the first demand.

            mListeners.Add(listener);
        }

        /// <summary>
        /// Removes the specified field update listener from the collection.
        /// </summary>
        internal void Remove(IFieldUpdateListener listener)
        {
            if (IsEmpty)
                return;

            mListeners.Remove(listener);

            if (mListeners.Count == 0)
                mListeners = null;
        }

        /// <summary>
        /// Notifies all of the collected field update listeners about that the specified field
        /// is about to be updated.
        /// </summary>
        internal FieldUpdateListenerResponse NotifyUpdating(Field field, Field parentField)
        {
            if (IsEmpty)
                return null;

            FieldUpdateListenerResponse listenerResponse = null;

            foreach (IFieldUpdateListener listener in mListeners)
            {
                listenerResponse = listener.NotifyUpdating(field, parentField);

                switch (listenerResponse.PreferredStageStrategy)
                {
                    case FieldUpdatePreferredStageStrategy.MainLoop:
                        // Continue iterating.
                        break;
                    case FieldUpdatePreferredStageStrategy.Skip:
                        // Return immediately.
                        return listenerResponse;
                    default:
                        throw new InvalidOperationException();
                }

                field = listenerResponse.Field;
            }

            return listenerResponse;
        }

        /// <summary>
        /// Notifies all of the collected field update listeners about that the specified field
        /// has been updated.
        /// </summary>
        internal void NotifyUpdated(FieldUpdateContext context, FieldUpdateStrategy strategy)
        {
            if (IsEmpty)
                return;

            foreach (IFieldUpdateListener listener in mListeners)
                listener.NotifyUpdated(context, strategy);
        }

        /// <summary>
        /// Notifies all of the collected field update listeners about that the specified field
        /// is updating.
        /// </summary>
        internal bool NotifyUpdate(FieldUpdateContext context)
        {
            if (IsEmpty)
                return false;

            foreach (IFieldUpdateListener listener in mListeners)
            {
                if (listener.Update(context))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether this collection does not contain any items.
        /// </summary>
        internal bool IsEmpty
        {
            get { return (mListeners == null); }
        }

        private List<IFieldUpdateListener> mListeners;
    }
}
