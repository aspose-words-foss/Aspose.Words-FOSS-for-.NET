// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/05/2025 by Ilya Navrotskiy

using System.Collections.Generic;
using System.Text;

namespace Aspose.Charset
{
    /// <summary>
    /// Represents a helper class that allows to work with text ranges in <see cref="CharsetDetector"/>.
    /// </summary>
    public class TextRange
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TextRange"/> class.
        /// </summary>
        /// <param name="start">The text at the very beginning of the range.</param>
        /// <param name="end">The text at the very end of the range.</param>
        /// <param name="requiredTextsInside">The array of text snippets that are required between start and end of the range.</param>
        public TextRange(string start, string end, string[] requiredTextsInside = null)
        {
            mStartText = Encoding.UTF8.GetBytes(start);
            mEndText = Encoding.UTF8.GetBytes(end);
            if (requiredTextsInside != null)
            {
                foreach (string requiredText in requiredTextsInside)
                    mRequiredTextsInside.Add(Encoding.UTF8.GetBytes(requiredText));
            }

            StartIndex = -1;
        }

        /// <summary>
        /// Finds range in a specified input array.
        /// </summary>
        /// <remarks>
        /// Sets appropriate values to the corresponding <see cref="StartIndex"/> and <see cref="EndIndex"/>, if found.
        /// Note, <see cref="EndIndex"/> may be not found and then it sets to -1. In this case you can continue search
        /// it in another buffer using <see cref="FindEnd"/>.
        /// </remarks>
        public bool Find(byte[] input, int startIndex)
        {
            EndIndex = -1;
            StartIndex = IndexOfIgnoreAsciiCase(input, mStartText, startIndex);
            if (StartIndex == -1)
                return false;

            foreach (byte[] requiredTextInside in mRequiredTextsInside)
            {
                int requiredTextIndex = IndexOfIgnoreAsciiCase(input, requiredTextInside, StartIndex + mStartText.Length);
                if (requiredTextIndex == -1)
                    return false;

                int endIndex = IndexOfIgnoreAsciiCase(input, mEndText, StartIndex + mStartText.Length, requiredTextIndex);
                if (endIndex != -1)
                    return false;
            }

            EndIndex = IndexOfIgnoreAsciiCase(input, mEndText, StartIndex + mStartText.Length);

            return true;
        }

        /// <summary>
        /// Finds end text for the range in a specified input array.
        /// </summary>
        public bool FindEnd(byte[] input, int startIndex)
        {
            EndIndex = IndexOfIgnoreAsciiCase(input, mEndText, startIndex);
            return EndIndex != -1;
        }

        /// <summary>
        /// Returns true, if <see cref="StartIndex"/> of this range is less, than start index of the other specified range.
        /// </summary>
        public bool IsBefore(TextRange other)
        {
            if (other == null || other.StartIndex == -1)
                return StartIndex != -1;

            if (StartIndex == -1)
                return false;

            return StartIndex < other.StartIndex;
        }

        /// <summary>
        /// Returns a position of subarray in a source array.
        /// Ignores case for ASCII letters (A-Z, a-z) on comparison.
        /// </summary>
        private static int IndexOfIgnoreAsciiCase(byte[] source, byte[] subArray, int sourceStartIndex)
        {
            return IndexOfIgnoreAsciiCase(source, subArray, sourceStartIndex, source.Length);
        }

        /// <summary>
        /// Returns a position of subarray in a source array.
        /// Ignores case for ASCII letters (A-Z, a-z) on comparison.
        /// </summary>
        private static int IndexOfIgnoreAsciiCase(byte[] source, byte[] subArray, int sourceStartIndex, int sourceEndIndex)
        {
            int endIndex = System.Math.Min(source.Length - subArray.Length + 1, sourceEndIndex);
            for (int i = sourceStartIndex; i < endIndex; i++)
            {
                int j = 0;
                while ((j < subArray.Length) && AreEqualIgnoreAsciiCase(source[i + j], subArray[j]))
                    j++;

                if (j == subArray.Length)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns true, if two bytes are equal.
        /// Ignores case for ASCII letters (A-Z, a-z) on comparison.
        /// </summary>
        private static bool AreEqualIgnoreAsciiCase(byte b1, byte b2)
        {
            if (b1 == b2)
                return true;

            if (b1 >= 'A' && b1 <= 'Z')
                return (b1 + 32) == b2;

            if (b1 >= 'a' && b1 <= 'z')
                return (b1 - 32) == b2;

            return false;
        }

#if DEBUG

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return string.Format(
                "{0}:{1} [{2} ... {3}]",
                StartIndex, EndIndex,
                Encoding.UTF8.GetString(mStartText),
                Encoding.UTF8.GetString(mEndText));
        }
#endif

        /// <summary>
        /// Gets an integer value representing start index of the range in a byte array after <see cref="Find"/> is called.
        /// </summary>
        public int StartIndex { get; private set; }

        /// <summary>
        /// Gets an integer value representing end index of the range in a byte array after <see cref="Find"/>,
        /// or <see cref="FindEnd"/> is called.
        /// </summary>
        public int EndIndex { get; private set; }

        /// <summary>
        /// Gets an integer value representing next index just after end text of the range in a byte array.
        /// </summary>
        public int NextIndex
        {
            get { return EndIndex == -1 ? 0 : EndIndex + mEndText.Length; }
        }

        private readonly byte[] mStartText;
        private readonly byte[] mEndText;
        private readonly List<byte[]> mRequiredTextsInside = new List<byte[]>();
    }
}
