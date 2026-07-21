// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2023 by Edward Voronov

using System;
using System.Collections.Generic;
using Aspose.JavaAttributes;

namespace Aspose.Common
{
    public sealed class CompositeDisposable<T> : IDisposable
        where T : class, IDisposable
    {
        public CompositeDisposable(IEnumerable<T> items)
        {
            mItems = new List<T>(items);
        }

        [JavaThrows(true)]
        void IDisposable.Dispose()
        {
            foreach (T item in mItems)
            {
                if (item != null)
                    item.Dispose();
            }
        }

        private readonly List<T> mItems;
    }
}
