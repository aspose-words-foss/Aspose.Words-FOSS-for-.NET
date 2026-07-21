// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/05/2024 by Ilya Navrotskiy

using System;
using System.Collections;
using System.Collections.Generic;

namespace Aspose.Collections.Generic
{
    /// <summary>
    /// Implements <see cref="IEnumerator"/> for <see cref="StringListGeneric{TValue}"/> class.
    /// </summary>
    public class StringListEnumerator<TValue> : IEnumerator<KeyValuePair<string, TValue>>
    {
        /// <summary>
        /// Initializes new instance of <see cref="StringListEnumerator{TValue}"/> class.
        /// </summary>
        public StringListEnumerator(StringListGeneric<TValue> list)
        {
            mList = list;
            mCurIndex = -1;
        }

        #region IEnumerator implementation

        public bool MoveNext()
        {
            if (++mCurIndex >= mList.Count)
                return false;

            mCurItem = new KeyValuePair<string, TValue>(mList.GetKey(mCurIndex), mList.GetByIndex(mCurIndex));
            return true;
        }

        public void Reset()
        {
            mCurIndex = -1;
        }

        void IDisposable.Dispose()
        {
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public KeyValuePair<string, TValue> Current
        {
            get { return mCurItem; }
        }

        #endregion

        private readonly StringListGeneric<TValue> mList;
        private int mCurIndex;
        private KeyValuePair<string, TValue> mCurItem;
    }
}
