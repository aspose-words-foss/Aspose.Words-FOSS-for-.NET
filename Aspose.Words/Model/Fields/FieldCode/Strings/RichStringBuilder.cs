// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/17/2017 by Edward Voronov

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    [DebuggerDisplay("{ToRichString()}")]
    internal class RichStringBuilder : IStringBuilder
    {
        internal RichStringBuilder()
            : this(0)
        {
        }

        internal RichStringBuilder(RichString s)
            : this()
        {
            AppendInternal(s);
        }

        internal RichStringBuilder(int capacity)
        {
            mChars = new List<IChar>(capacity);
        }

        public int Length
        {
            get { return mChars.Count; }
            set
            {
                if (Length == value)
                    return;

                if (Length > value)
                {
                    mChars.RemoveRange(value, mChars.Count - value);
                }
                else
                {
                    while (Length != value)
                        AppendInternal(RichChar.NullChar);
                }
            }
        }

        IChar IStringBuilder.this[int index]
        {
            get { return GetInternal(index); }
            set { SetInternal(index, (RichChar)value); }
        }

        internal RichChar GetInternal(int index)
        {
            return (RichChar)mChars[index];
        }

        internal void SetInternal(int index, RichChar value)
        {
            mChars[index] = value;
        }

        IString IStringBuilder.ToIString()
        {
            return ToRichString();
        }

        IStringBuilder IStringBuilder.Append(IChar c)
        {
            return AppendInternal((RichChar)c);
        }

        internal RichStringBuilder AppendInternal(RichChar c)
        {
            mChars.Add(c);
            return this;
        }

        IStringBuilder IStringBuilder.Append(char c, IChar source)
        {
            return AppendInternal(c, (RichChar)source);
        }

        internal RichStringBuilder AppendInternal(char c, RichChar source)
        {
            return Append(c, GetSourceRunPrOrDefault(source));
        }

        IStringBuilder IStringBuilder.Append(char c, int count, IChar source)
        {
            return AppendInternal(c, count, (RichChar)source);
        }

        internal RichStringBuilder AppendInternal(char c, int count, RichChar source)
        {
            RunPr runPr = GetSourceRunPrOrDefault(source);

            for (int i = 0; i < count; i++)
                Append(c, runPr);

            return this;
        }

        IStringBuilder IStringBuilder.Append(IString s)
        {
            return AppendInternal((RichString)s);
        }

        internal RichStringBuilder AppendInternal(RichString s)
        {
            foreach (IChar c in s)
                AppendInternal((RichChar)c);

            return this;
        }

        IStringBuilder IStringBuilder.Append(string s, IChar source)
        {
            return AppendInternal(s, (RichChar)source);
        }

        internal RichStringBuilder AppendInternal(string s, RichChar source)
        {
            return Append(s, GetSourceRunPrOrDefault(source));
        }

        IStringBuilder IStringBuilder.Insert(int index, char c, IChar source)
        {
            return InsertInternal(index, c, (RichChar)source);
        }

        internal RichStringBuilder InsertInternal(int index, char c, RichChar source)
        {
            mChars.Insert(index, new RichChar(c, GetSourceRunPrOrDefault(source)));
            return this;
        }

        IStringBuilder IStringBuilder.Insert(int index, string s, IChar source)
        {
            return InsertInternal(index, s, (RichChar)source);
        }

        internal RichStringBuilder InsertInternal(int index, string s, RichChar source)
        {
            RunPr runPr = GetSourceRunPrOrDefault(source);

            foreach (char c in s)
                mChars.Insert(index++, new RichChar(c, runPr));

            return this;
        }

        IStringBuilder IStringBuilder.Insert(int index, string s, int count, IList<IChar> sources)
        {
            return InsertInternal(index, s, count, sources);
        }

        internal RichStringBuilder InsertInternal(int index, string s, int count, IList<IChar> sources)
        {
            for (int j = 0; j < count; j++)
            {
                RichChar source = (sources != null) && (j < sources.Count)
                    ? (RichChar)sources[j]
                    : null;

                RunPr runPr = GetSourceRunPrOrDefault(source);

                foreach (char c in s)
                {
                    mChars.Insert(index++, new RichChar(c, runPr));
                }
            }

            return this;
        }

        IStringBuilder IStringBuilder.Replace(string oldValue, string newValue)
        {
            return ReplaceInternal(oldValue, newValue);
        }

        internal RichStringBuilder ReplaceInternal(string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(oldValue))
                return this;

            int index = 0;
            while (true)
            {
                string systemString = ToRichString().ToSystemString();
                index = systemString.IndexOf(oldValue, index, StringComparison.Ordinal);
                if (index == -1)
                    return this;

                RichChar source = GetInternal(index);
                RemoveInternal(index, oldValue.Length);
                InsertInternal(index, newValue, source);

                index += newValue.Length;
            }
        }

        IStringBuilder IStringBuilder.Replace(int index, char c, IChar source)
        {
            return ReplaceInternal(index, c, (RichChar)source);
        }

        internal RichStringBuilder ReplaceInternal(int index, char c, RichChar source)
        {
            mChars[index] = new RichChar(c, GetSourceRunPrOrDefault(source));
            return this;
        }

        IStringBuilder IStringBuilder.Remove(int startIndex, int length)
        {
            return RemoveInternal(startIndex, length);
        }

        internal RichStringBuilder RemoveInternal(int startIndex, int length)
        {
            mChars.RemoveRange(startIndex, length);
            return this;
        }

        internal void Clear()
        {
            mChars.Clear();
        }

        internal RichStringBuilder Append(char c, RunPr runPr)
        {
            return AppendInternal(new RichChar(c, runPr));
        }

        internal RichStringBuilder Append(string text, RunPr runPr)
        {
            foreach (char c in text)
                Append(c, runPr);

            return this;
        }

        internal RichString ToRichString()
        {
            return new RichString(mChars);
        }

        private static RunPr GetSourceRunPrOrDefault(RichChar source)
        {
            return source != null
                ? source.RunPr
                : new RunPr();
        }

        private readonly List<IChar> mChars;
    }
}
