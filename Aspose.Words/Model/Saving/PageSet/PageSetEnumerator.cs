// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2020 by Konstantin Kornilov

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Saving
{
    internal abstract class PageSetEnumerator : IEnumerable<int>, IEnumerator<int>
    {
        public IEnumerator<int> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }

        public abstract bool MoveNext();

        public abstract void Reset();

        public abstract int Current { get; }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
