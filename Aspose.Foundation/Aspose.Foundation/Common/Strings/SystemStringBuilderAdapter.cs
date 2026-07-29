// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2020 by Edward Voronov

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Aspose.Common
{
    /// <summary>Adapts <see cref="StringBuilder"/> to <see cref="IStringBuilder"/>.</summary>
    [DebuggerDisplay("{Value}")]
    public class SystemStringBuilderAdapter : IStringBuilder
    {
        public SystemStringBuilderAdapter()
            : this(new StringBuilder())
        {
        }

        public SystemStringBuilderAdapter(int capacity)
            : this(new StringBuilder(capacity))
        {
        }

        public SystemStringBuilderAdapter(StringBuilder value)
        {
            Value = value;
        }

        public int Length
        {
            get { return Value.Length; }
            set { Value.Length = value; }
        }

        IChar IStringBuilder.this[int index]
        {
            get { return GetChar(index); }
            set { SetChar(index,(SystemCharAdapter)value); }
        }

        public SystemCharAdapter GetChar(int index)
        {
            return SystemCharAdapter.Create(Value[index]);
        }

        public void SetChar(int index, SystemCharAdapter value)
        {
            Value[index] = value.Value;
        }

        IString IStringBuilder.ToIString()
        {
            return SystemStringAdapter.Create(Value.ToString());
        }

        IStringBuilder IStringBuilder.Append(IChar c)
        {
            return Append((SystemCharAdapter)c);
        }

        public SystemStringBuilderAdapter Append(SystemCharAdapter c)
        {
            Value.Append(c.Value);
            return this;
        }

        IStringBuilder IStringBuilder.Append(char c, IChar source)
        {
            return Append(c);
        }

        public SystemStringBuilderAdapter Append(char c)
        {
            Value.Append(c);
            return this;
        }

        IStringBuilder IStringBuilder.Append(char c, int count, IChar source)
        {
            return Append(c, count);
        }

        public SystemStringBuilderAdapter Append(char c, int count)
        {
            Value.Append(c, count);
            return this;
        }

        IStringBuilder IStringBuilder.Append(IString s)
        {
            return Append((SystemStringAdapter)s);
        }

        public SystemStringBuilderAdapter Append(SystemStringAdapter s)
        {
            Value.Append(s.Value);
            return this;
        }

        IStringBuilder IStringBuilder.Append(string s, IChar source)
        {
            return Append(s);
        }

        public SystemStringBuilderAdapter Append(string s)
        {
            Value.Append(s);
            return this;
        }

        IStringBuilder IStringBuilder.Insert(int index, char c, IChar source)
        {
            return Insert(index, c);
        }

        public SystemStringBuilderAdapter Insert(int index, char c)
        {
            Value.Insert(index, c);
            return this;
        }

        IStringBuilder IStringBuilder.Insert(int index, string s, IChar source)
        {
            return Insert(index, s);
        }

        public SystemStringBuilderAdapter Insert(int index, string s)
        {
            Value.Insert(index, s);
            return this;
        }

        IStringBuilder IStringBuilder.Insert(int index, string s, int count, IList<IChar> sources)
        {
            return Insert(index, s, count);
        }

        public SystemStringBuilderAdapter Insert(int index, string s, int count)
        {
            Value.Insert(index, s, count);
            return this;
        }

        IStringBuilder IStringBuilder.Replace(string oldValue, string newValue)
        {
            return ReplaceInternal(oldValue, newValue);
        }

        public SystemStringBuilderAdapter ReplaceInternal(string oldValue, string newValue)
        {
            Value.Replace(oldValue, newValue);
            return this;
        }

        IStringBuilder IStringBuilder.Replace(int index, char c, IChar source)
        {
            return Replace(index, c);
        }

        public SystemStringBuilderAdapter Replace(int index, char c)
        {
            Value[index] = c;
            return this;
        }

        IStringBuilder IStringBuilder.Remove(int startIndex, int length)
        {
            return RemoveInternal(startIndex, length);
        }

        public SystemStringBuilderAdapter RemoveInternal(int startIndex, int length)
        {
            Value.Remove(startIndex, length);
            return this;
        }

        public StringBuilder Value { get; }
    }
}
