// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/10/2019 by Edward Voronov

using System;
using System.Collections;
using System.Collections.Generic;

namespace Aspose.Collections
{
    public sealed class EmptyEnumerator<T> : IEnumerator<T>
    {
        private EmptyEnumerator()
        {
        }

        void IDisposable.Dispose()
        {
            // Do nothing
        }

        bool IEnumerator.MoveNext()
        {
            return false;
        }

        void IEnumerator.Reset()
        {
            // Do nothing
        }

        T IEnumerator<T>.Current
        {
            get { throw new InvalidOperationException(); }
        }

        [JavaAttributes.JavaDelete]
        object IEnumerator.Current
        {
            get { throw new InvalidOperationException(); }
        }

        // We can't create a static singleton instance with generic parameters in Java
#if JAVA
        private static readonly IEnumerator<object> gInstance = new EmptyEnumerator<object>();
#else
        private static readonly IEnumerator<T> gInstance = new EmptyEnumerator<T>();
#endif

        public static IEnumerator<T> GetInstance()
        {
#if JAVA
            return (IEnumerator<T>)gInstance;
#else
            return gInstance;
#endif
        }
    }
}
