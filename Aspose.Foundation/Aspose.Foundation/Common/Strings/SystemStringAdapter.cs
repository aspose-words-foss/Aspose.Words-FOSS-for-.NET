// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2020 by Edward Voronov

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aspose.Common
{
    /// <summary>Adapts <see cref="System.String"/> to <see cref="IString"/>.</summary>
    [DebuggerDisplay("{Value}")]
    public class SystemStringAdapter : IString
    {
        private SystemStringAdapter(string value)
        {
            Value = value;
            mChars = new SystemCharAdapter[Value.Length];
        }

        public static SystemStringAdapter Create(string value)
        {
            return value == string.Empty
                ? Empty
                : new SystemStringAdapter(value);
        }

        public static readonly SystemStringAdapter Empty = new SystemStringAdapter(string.Empty);

        public int Length
        {
            get { return Value.Length; }
        }

        IChar IString.this[int index]
        {
            get { return GetInternal(index); }
        }

        public SystemCharAdapter GetInternal(int index)
        {
            return EnsureChar(index);
        }

        public int IndexOf(string value)
        {
            return Value.IndexOf(value, StringComparison.Ordinal);
        }

        IString IString.Replace(string oldValue, string newValue)
        {
            return ReplaceInternal(oldValue, newValue);
        }

        public SystemStringAdapter ReplaceInternal(string oldValue, string newValue)
        {
            return Create(Value.Replace(oldValue, newValue));
        }

        IString IString.Remove(int startIndex, int count)
        {
            return RemoveInternal(startIndex, count);
        }

        public SystemStringAdapter RemoveInternal(int startIndex, int count)
        {
            return Create(Value.Remove(startIndex, count));
        }

        IString IString.ToUpper()
        {
            return ToUpperInternal();
        }

        public SystemStringAdapter ToUpperInternal()
        {
            return Create(Value.ToUpper());
        }

        IString IString.ToLower()
        {
            return ToLowerInternal();
        }

        public SystemStringAdapter ToLowerInternal()
        {
            return Create(Value.ToLower());
        }

        public string ToSystemString()
        {
            return Value;
        }

        IEnumerator<IChar> IEnumerable<IChar>.GetEnumerator()
        {
#if JAVA
            return EnsureChars().GetEnumerator();
#else
            return ((IEnumerable<IChar>)EnsureChars()).GetEnumerator();
#endif
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<IChar>)this).GetEnumerator();
        }

        public string Value { get; }

        private SystemCharAdapter EnsureChar(int index)
        {
            return mChars[index] ?? (mChars[index] = SystemCharAdapter.Create(Value[index]));
        }

        private IChar[] EnsureChars()
        {
            for (int i = 0; i < mChars.Length; i++)
                EnsureChar(i);

            return mChars;
        }

        private readonly SystemCharAdapter[] mChars;
    }
}
