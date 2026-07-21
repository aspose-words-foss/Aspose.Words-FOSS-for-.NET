// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/10/2022 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements fields updating tracking state.
    /// </summary>
    internal class FieldUpdateProgressContext
    {
        internal FieldUpdateProgressContext(IFieldUpdatingProgressCallback callback, object owner)
        {
            Owner = owner;
            mCallback = callback;
        }

        internal object Owner { get; }

        internal void NotifyInternal()
        {
            if(!mHasChanges)
                return;

            mHasChanges = false;

            NotifyCore(false);
        }

        internal void OnFieldToBeUpdated()
        {
            mTotal++;
            mHasChanges = true;
        }

        internal void OnFieldUpdated()
        {
            mUpdated++;
            mHasChanges = true;
        }

        internal void OnUpdateCompleted()
        {
            Debug.Assert(mTotal == mUpdated, "mTotal != mUpdated");

            NotifyCore(true);
        }

        private void NotifyCore(bool completed)
        {
            if (mCallback == null)
                return;

            mCallback.Notify(new FieldUpdatingProgressArgs(mTotal, mUpdated, completed));
        }

        private int mTotal;
        private int mUpdated;
        private bool mHasChanges;

        private readonly IFieldUpdatingProgressCallback mCallback;
    }
}
