// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/11/2017 by Alexey Butalov

using System.Collections;
using System.Collections.Generic;
using Aspose.JavaAttributes;

namespace Aspose
{
    /// <summary>
    /// If you want to use <see cref="IEnumerator{T}"/> methods <see cref="IEnumerator{T}.MoveNext"/> and <see cref="IEnumerator{T}.Current"/>
    /// in your C# code directly, then you need to wrap your enumerator instance into an object of this class to achieve autoportability to Java.
    /// </summary>
    [JavaManual("Manual porting by design.")]
    public class EnumeratorWrapperPalGeneric<T>
    {
        public EnumeratorWrapperPalGeneric(IEnumerator<T> wrappedEnumerator)
        {
            mWrappedEnumerator = wrappedEnumerator;
        }

        public bool MoveNext()
        {
            return mWrappedEnumerator.MoveNext();
        }

        public void Reset()
        {
            mWrappedEnumerator.Reset();
        }

        public T Current
        {
            get { return mWrappedEnumerator.Current; }
        }

        [CodePorting.Translator.Cs2Cpp.CppSkipEntity("C++ doesn't support untyped collection interfaces")]
        protected IEnumerator WrappedEnumerator
        {
            get { return mWrappedEnumerator; }
        }

        private readonly IEnumerator<T> mWrappedEnumerator;
    }
}
