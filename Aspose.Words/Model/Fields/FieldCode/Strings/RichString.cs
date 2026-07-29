// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/03/2014 by Alexey Noskov

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>Represents collection of <see cref="RichChar"/> instances.</summary>
    [DebuggerDisplay("{ToSystemString()}")]
    internal class RichString : IString
    {
        /// <summary>Creates <see cref="RichString"/> from the specified string.</summary>
        internal static RichString CreateFromString(string srcString, RunPr runPr)
        {
            if (srcString == null)
                return null;

            RichStringBuilder builder = new RichStringBuilder(srcString.Length);
            builder.Append(srcString, runPr);
            return builder.ToRichString();
        }

        /// <summary>Creates <see cref="RichString"/> from the specified string.</summary>
        internal static RichString CreateFromString(string srcString)
        {
            return CreateFromString(srcString, new RunPr());
        }

        internal static readonly RichString Empty = new RichString();

        internal RichString(List<IChar> chars)
        {
            mChars = chars.ToArray();
        }

        internal RichString(params RichChar[] chars)
        {
            mChars = new RichChar[chars.Length];
            chars.CopyTo(mChars, 0);
        }

        IChar IString.this[int index]
        {
            get { return GetInternal(index); }
        }

        internal RichChar GetInternal(int index)
        {
            return (RichChar)mChars[index];
        }

        public int IndexOf(string value)
        {
            return ToSystemString().IndexOf(value, StringComparison.Ordinal);
        }

        public int IndexOf(string value, int startIndex)
        {
            return ToSystemString().IndexOf(value, startIndex, StringComparison.Ordinal);
        }

        internal RichString Substring(int startIndex, int length)
        {
            List<IChar> chars = new List<IChar>(mChars).GetRange(startIndex, length);
            return new RichString(chars);
        }

        IString IString.Replace(string oldValue, string newValue)
        {
            return ReplaceInternal(oldValue, newValue);
        }

        internal RichString ReplaceInternal(string oldValue, string newValue)
        {
            RichStringBuilder builder = new RichStringBuilder(this);
            return builder
                .ReplaceInternal(oldValue, newValue)
                .ToRichString();
        }

        IString IString.Remove(int startIndex, int count)
        {
            return RemoveInternal(startIndex, count);
        }

        internal RichString RemoveInternal(int startIndex, int count)
        {
            RichStringBuilder builder = new RichStringBuilder(this);
            return builder
                .RemoveInternal(startIndex, count)
                .ToRichString();
        }

        IString IString.ToUpper()
        {
            return ToUpperInternal();
        }

        internal RichString ToUpperInternal()
        {
            RichStringBuilder builder = new RichStringBuilder(mChars.Length);

            foreach (IChar @char in mChars)
                builder.AppendInternal((RichChar)@char.ToUpper());

            return builder.ToRichString();
        }

        IString IString.ToLower()
        {
            return ToLowerInternal();
        }

        internal RichString ToLowerInternal()
        {
            RichStringBuilder builder = new RichStringBuilder(mChars.Length);

            foreach (IChar @char in mChars)
                builder.AppendInternal((RichChar)@char.ToLower());

            return builder.ToRichString();
        }

        public string ToSystemString()
        {
            if (mSystemString == null)
            {
                StringBuilder builder = new StringBuilder(mChars.Length);

                foreach (IChar c in mChars)
                    builder.Append(c.ToSystemChar());

                mSystemString = builder.ToString();
            }

            return mSystemString;
        }

        /// <summary>Removes all leading and trailing white-space characters from the current <see cref="RichString" /> object.</summary>
        internal RichString Trim()
        {
            if (Length == 0)
                return this;

            int firstNonWhiteSpaceChar = -1;
            int lastNonWhiteSpaceChar = -1;

            for (int i = 0; i < Length; i++)
            {
                bool isWhiteSpace = char.IsWhiteSpace(GetInternal(i).Character);
                if (!isWhiteSpace)
                {
                    lastNonWhiteSpaceChar = i;
                    if (firstNonWhiteSpaceChar == -1)
                        firstNonWhiteSpaceChar = i;
                }
            }

            if (firstNonWhiteSpaceChar == -1)
                return Empty;

            RichStringBuilder builder = new RichStringBuilder();

            for (int i = firstNonWhiteSpaceChar; i <= lastNonWhiteSpaceChar; i++)
                builder.AppendInternal(GetInternal(i));

            return builder.ToRichString();
        }

        internal RichString TrimFormat(params int[] attrs)
        {
            RichStringBuilder builder = new RichStringBuilder();

            foreach (IChar ichar in this)
            {
                RichChar @char = (RichChar)ichar;
                RunPr format = @char.RunPr.Clone();
                foreach (int attr in attrs)
                    format.Remove(attr);
                builder.Append(@char.Character, format);
            }

            return builder.ToRichString();
        }

        internal RichStringChunk[] ToChunks()
        {
            if (mChunks == null)
            {
                mChunks = new List<RichStringChunk>();

                StringBuilder builder = new StringBuilder();
                RunPr runPr = null;
                foreach (IChar ichar in mChars)
                {
                    RichChar @char = (RichChar)ichar;
                    if ((runPr != null) && !AttrCollection.Equals(runPr, @char.RunPr, null))
                    {
                        mChunks.Add(new RichStringChunk(builder.ToString(), runPr));
                        builder.Length = 0;
                    }

                    runPr = @char.RunPr;
                    builder.Append(@char.Character);
                }

                mChunks.Add(new RichStringChunk(builder.ToString(), runPr));
            }

            return mChunks.ToArray();
        }

        public int Length
        {
            get { return mChars.Length; }
        }

        IEnumerator<IChar> IEnumerable<IChar>.GetEnumerator()
        {
#if JAVA
            return mChars.GetEnumerator();
#else
            return ((IEnumerable<IChar>)mChars).GetEnumerator();
#endif
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<IChar>)this).GetEnumerator();
        }

        [CodePorting.Translator.Cs2Cpp.CppGenerateBeginEndMethods]
        private readonly IChar[] mChars;
        private string mSystemString;
        private List<RichStringChunk> mChunks;
    }
}
