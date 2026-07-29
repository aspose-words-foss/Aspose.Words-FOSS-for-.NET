// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/08/2005 by Roman Korchagin

using System;
using System.Diagnostics;
using System.Text;
using Aspose.JavaAttributes;

namespace Aspose
{
    /// <summary>
    /// Array related utility functions.
    /// </summary>
    [DebuggerStepThrough]
    public static class ArrayUtil
    {
        /// <summary>
        /// Compares two arrays.
        /// </summary>
        public static bool IsArrayEqual(char[] array1, char[] array2)
        {
            if (array1 == array2)
                return true;

            if (array1 == null || array2 == null)
                return false;

            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; ++i)
            {
                if (array1[i] != array2[i])
                    return false;
            }

            return true;
        }

        public static bool IsArrayEqual(byte[] array1, byte[] array2)
        {
            if (array1 == array2)
                return true;

            if (array1 == null || array2 == null)
                return false;

            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; ++i)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }

        public static bool IsArrayEqual(short[] array1, short[] array2)
        {
            if (array1 == array2)
                return true;

            if (array1 == null || array2 == null)
                return false;

            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; ++i)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }

        public static bool IsArrayEqual(int[] array1, int[] array2)
        {
            if (array1 == array2)
                return true;

            if (array1 == null || array2 == null)
                return false;

            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; ++i)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }

        public static bool IsArrayEqual(long[] array1, long[] array2)
        {
            if (array1 == array2)
                return true;

            if (array1 == null || array2 == null)
                return false;

            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; ++i)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }

        public static bool IsArrayEqual(float[] array1, float[] array2)
        {
            if (array1 == array2)
                return true;

            if (array1 == null || array2 == null)
                return false;

            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; ++i)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }

        public static bool IsArrayEqual(double[] array1, double[] array2)
        {
            if (array1 == array2)
                return true;

            if (array1 == null || array2 == null)
                return false;

            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; ++i)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }

        [JavaDelete]
        public static bool IsArrayEqual(object[] array1, object[] array2)
        {
            if (array1 == array2)
                return true;

            if (array1 == null || array2 == null)
                return false;

            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; ++i)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }

        public static bool IsArrayEqual<T>(T[] array1, T[] array2)
        {
            if (array1 == array2)
                return true;

            if (array1 == null || array2 == null)
                return false;

            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; ++i)
            {
                if (!object.Equals(array1[i], array2[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Dumps byte array into a string for debugging.
        /// </summary>
        public static string DumpArray(byte[] data)
        {
            return data == null ? "Null" : DumpArray(data, 0, data.Length);
        }

        public static string DumpArray(byte[] data, int start, int count)
        {
            if (data == null)
                return "Null";

            StringBuilder builder = new StringBuilder();
            while (count > 0)
            {
                builder.AppendFormat("{0:X2} ", data[start]);
                start++;
                count--;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Dumps integer array into a string for debugging.
        /// </summary>
        public static string DumpArray(int[] data, int start, int count)
        {
            if (data == null)
                return "Null";

            StringBuilder builder = new StringBuilder();
            for (int i = start; (i < data.Length) && (count > 0); i++, count--)
                builder.AppendFormat("{0:X8} ", data[i]);

            return builder.ToString();
        }

        public static string DumpArray(float[] data)
        {
            StringBuilder builder = new StringBuilder(data.Length * 10);
            builder.Append('[');
            bool first = true;
            foreach (float f in data)
            {
                if (!first)
                    builder.Append(", ");
                else
                    first = false;

                builder.Append(f);
            }

            builder.Append(']');
            return builder.ToString();
        }

        /// <summary>
        /// This code was copied and modified from Rotor.
        /// Performs a binary search without boxing elements of the array.
        /// </summary>
        /// <param name="array">Array to search in.</param>
        /// <param name="index">Start index.</param>
        /// <param name="length">Length of search area.</param>
        /// <param name="value">Value to search for.</param>
        /// <returns>Index of the found value or negated index of possible insertion.</returns>
        public static int BinarySearch(int[] array, int index, int length, int value)
        {
            Debug.Assert(array != null);
            Debug.Assert((index >= 0) && (length >= 0));
            Debug.Assert(index + length - 1 <= array.Length);

            int lo = index;
            int hi = index + length - 1;

            while (lo <= hi)
            {
                int median = (lo + hi) >> 1;
                int v = array[median];

                if (v == value)
                    return median;

                if (v < value)
                    lo = median + 1;
                else
                    hi = median - 1;
            }

            return ~lo;
        }

        /// <summary>
        /// This code was copied and modified from Rotor.
        /// Performs a binary search without boxing elements of the array. If duplicate elements are encountered then the leftmost is returned.
        /// Please refer to https://en.wikipedia.org/wiki/Binary_search_algorithm#Procedure_for_finding_the_leftmost_element for details.
        /// </summary>
        /// <param name="array">Array to search in.</param>
        /// <param name="index">Start index.</param>
        /// <param name="length">Length of search area.</param>
        /// <param name="value">Value to search for.</param>
        /// <returns>Index of the found value or negated index of possible insertion.</returns>
        public static int BinarySearchLeftmost(int[] array, int index, int length, int value)
        {
            Debug.Assert(array != null);
            Debug.Assert((index >= 0) && (length >= 0));
            Debug.Assert(index + length - 1 <= array.Length);

            int lo = index;
            int hi = index + length;

            while (lo < hi)
            {
                int median = (lo + hi) >> 1;
                int v = array[median];

                if (v < value)
                    lo = median + 1;
                else
                    hi = median;
            }

            return (lo < array.Length && array[lo] == value) ? lo : ~lo;
        }

        /// <summary>
        /// A copy of int[] BinarySearch() - see above.
        /// </summary>
        public static int BinarySearch(short[] array, int index, int length, short value)
        {
            Debug.Assert(array != null);
            Debug.Assert((index >= 0) && (length >= 0));
            Debug.Assert(index + length - 1 <= array.Length);

            int lo = index;
            int hi = index + length - 1;

            while (lo <= hi)
            {
                int median = (lo + hi) >> 1;
                short v = array[median];

                if (v == value)
                    return median;

                if (v < value)
                    lo = median + 1;
                else
                    hi = median - 1;
            }

            return ~lo;
        }

        /// <summary>
        /// A copy of int[] BinarySearch() - see above.
        /// </summary>
        public static int BinarySearch(long[] array, int index, int length, long value)
        {
            Debug.Assert(array != null);
            Debug.Assert((index >= 0) && (length >= 0));
            Debug.Assert(index + length - 1 <= array.Length);

            int lo = index;
            int hi = index + length - 1;

            while (lo <= hi)
            {
                int median = (lo + hi) >> 1;
                long v = array[median];

                if (v == value)
                    return median;

                if (v < value)
                    lo = median + 1;
                else
                    hi = median - 1;
            }

            return ~lo;
        }

        /// <summary>
        /// A copy of int[] BinarySearch() - see above.
        /// </summary>
        public static int BinarySearch(byte[] array, int index, int length, byte value)
        {
            Debug.Assert(array != null);
            Debug.Assert((index >= 0) && (length >= 0));
            Debug.Assert(index + length - 1 <= array.Length);

            int lo = index;
            int hi = index + length - 1;

            while (lo <= hi)
            {
                int median = (lo + hi) >> 1;
                byte v = array[median];

                if (v == value)
                    return median;

                if (v < value)
                    lo = median + 1;
                else
                    hi = median - 1;
            }

            return ~lo;
        }

        /// <summary>
        /// A copy of int[] BinarySearch() - see above.
        /// </summary>
        public static int BinarySearch(float[] array, int index, int length, float value)
        {
            Debug.Assert(array != null);
            Debug.Assert((index >= 0) && (length >= 0));
            Debug.Assert(index + length - 1 <= array.Length);

            int lo = index;
            int hi = index + length - 1;

            while (lo <= hi)
            {
                int median = (lo + hi) >> 1;
                float v = array[median];

                if (v == value)
                    return median;

                if (v < value)
                    lo = median + 1;
                else
                    hi = median - 1;
            }

            return ~lo;
        }

        /// <summary>
        /// A copy of int[] BinarySearch() - see above.
        /// </summary>
        public static int BinarySearch(double[] array, int index, int length, double value)
        {
            Debug.Assert(array != null);
            Debug.Assert((index >= 0) && (length >= 0));
            Debug.Assert(index + length - 1 <= array.Length);

            int lo = index;
            int hi = index + length - 1;

            while (lo <= hi)
            {
                int median = (lo + hi) >> 1;
                double v = array[median];

                if (v == value)
                    return median;

                if (v < value)
                    lo = median + 1;
                else
                    hi = median - 1;
            }

            return ~lo;
        }

        /// <summary>
        /// A copy of int[] BinarySearch() - see above.
        /// </summary>
        public static int BinarySearch(char[] array, int index, int length, char value)
        {
            Debug.Assert(array != null);
            Debug.Assert((index >= 0) && (length >= 0));
            Debug.Assert(index + length - 1 <= array.Length);

            int lo = index;
            int hi = index + length - 1;

            while (lo <= hi)
            {
                int median = (lo + hi) >> 1;
                char v = array[median];

                if (v == value)
                    return median;

                if (v < value)
                    lo = median + 1;
                else
                    hi = median - 1;
            }

            return ~lo;
        }

        public static int BinarySearch(string[] array, string value)
        {
            return BinarySearch(array, 0, array.Length, value);
        }

        public static int BinarySearch(string[] array, int index, int length, string value)
        {
            Debug.Assert(array != null);
            Debug.Assert((index >= 0) && (length >= 0));
            Debug.Assert(index + length - 1 <= array.Length);

            int lo = index;
            int hi = index + length - 1;

            while (lo <= hi)
            {
                int median = (lo + hi) / 2;
                string v = array[median];

                int compare = StringUtil.CompareStringSort(v, value);

                if (compare == 0)
                    return median;

                if (compare < 0)
                    lo = median + 1;
                else
                    hi = median - 1;
            }

            return ~lo;
        }

        /// <summary>
        /// Analogue of C memset.
        /// </summary>
        public static void SetBytes(byte[] array, byte value, int offset, int length)
        {
            for (int i = 0; i < length; i++)
                array[i + offset] = value;
        }

        /// <summary>
        /// Copies safely one array into another.
        /// </summary>
        /// <remarks>
        /// Checks that the passed parameters are within the allowed bounds.
        /// </remarks>
        public static int Copy(byte[] srcArray, int srcIndex, int length, byte[] dstArray, int dstIndex)
        {
            if (length < 1)
                return 0;

            if ((srcIndex < 0) || (srcIndex >= srcArray.Length))
                return 0;

            if ((dstIndex < 0) || (dstIndex >= dstArray.Length))
                return 0;

            // Ensure index will be in range of the source array.
            int maxLength = Math.Min(length, srcArray.Length - srcIndex);
            // Ensure index will be in range of the destination array.
            maxLength = Math.Min(maxLength, dstArray.Length - dstIndex);

            for (int i = 0; i < maxLength; i++)
                dstArray[dstIndex + i] = srcArray[srcIndex + i];

            return maxLength;
        }

        /// <summary>
        /// Analogue of C memcmp.
        /// </summary>
        public static bool CompareBytes(byte[] array1, byte[] array2, int length)
        {
            if (array1.Length < length || array2.Length < length)
                return false;

            for (int i = 0; i < length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }

        public static void CheckArraysEqual(char[] array1, char[] array2)
        {
            int minLength = Math.Min(array1.Length, array2.Length);
            for (int i = 0; i < minLength; ++i)
            {
                if (array1[i] != array2[i])
                    throw new InvalidOperationException(string.Format(ExceptionArrayValues,
                        i, array1[i], array2[i]));
            }

            if (array1.Length != array2.Length)
                throw new InvalidOperationException(string.Format(ExceptionArrayLengths, array1.Length, array2.Length));
        }

        public static void CheckArraysEqual(byte[] array1, byte[] array2)
        {
            int minLength = Math.Min(array1.Length, array2.Length);
            for (int i = 0; i < minLength; ++i)
            {
                if (array1[i] != array2[i])
                    throw new InvalidOperationException(string.Format(ExceptionArrayValues,
                        i, array1[i], array2[i]));
            }

            if (array1.Length != array2.Length)
                throw new InvalidOperationException(string.Format(ExceptionArrayLengths, array1.Length, array2.Length));
        }

        public static void CheckArraysEqual(short[] array1, short[] array2)
        {
            int minLength = Math.Min(array1.Length, array2.Length);
            for (int i = 0; i < minLength; ++i)
            {
                if (array1[i] != array2[i])
                    throw new InvalidOperationException(string.Format(ExceptionArrayValues,
                        i, array1[i], array2[i]));
            }

            if (array1.Length != array2.Length)
                throw new InvalidOperationException(string.Format(ExceptionArrayLengths, array1.Length, array2.Length));
        }

        public static void CheckArraysEqual(int[] array1, int[] array2)
        {
            int minLength = Math.Min(array1.Length, array2.Length);
            for (int i = 0; i < minLength; ++i)
            {
                if (array1[i] != array2[i])
                    throw new InvalidOperationException(string.Format(ExceptionArrayValues,
                        i, array1[i], array2[i]));
            }

            if (array1.Length != array2.Length)
                throw new InvalidOperationException(string.Format(ExceptionArrayLengths, array1.Length, array2.Length));
        }

        public static void CheckArraysEqual(float[] array1, float[] array2, float delta)
        {
            int minLength = Math.Min(array1.Length, array2.Length);
            for (int i = 0; i < minLength; ++i)
            {
                if ((array1[i] - array2[i]) > delta)
                    throw new InvalidOperationException(string.Format("Array values are different at position {0}: {1} vs. {2} with delta {3}.",
                        i, array1[i], array2[i], delta));
            }

            if (array1.Length != array2.Length)
                throw new InvalidOperationException(string.Format(ExceptionArrayLengths, array1.Length, array2.Length));
        }

        public static void PrintArrayDiff(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
                throw new InvalidOperationException(string.Format(ExceptionArrayLengths, array1.Length, array2.Length));

            for (int i = 0; i < array1.Length; ++i)
            {
                if (array1[i] != array2[i])
                    Debug.WriteLine(string.Format("Array values are different at position 0x{0:X2}, 0x{1:X2} 0x{2:X2}.", i, array1[i], array2[i]));
            }
        }

        /// <summary>
        /// Creates a new byte array and copies first elements of the source array to it. Number of copied elements is
        /// min(source array length, destination array length).
        /// </summary>
        /// <param name="srcArray">Source array.</param>
        /// <param name="length">New array length.</param>
        /// <returns>New array.</returns>
        public static byte[] ResizeArray(byte[] srcArray, int length)
        {
            byte[] dstArray = new byte[length];

            if (length > srcArray.Length)
                length = srcArray.Length;
            Array.Copy(srcArray, dstArray, length);
            return dstArray;
        }

        /// <summary>
        /// Returns true if the byte array is not null and not empty.
        /// </summary>
        public static bool HasData(byte[] byteArray)
        {
            return (byteArray != null) && (byteArray.Length > 0);
        }

        /// <summary>
        /// Returns true if the string array is not null and not empty.
        /// </summary>
        public static bool HasData(string[] stringArray)
        {
            return (stringArray != null) && (stringArray.Length > 0);
        }

        /// <summary>
        /// Merges string array to string.
        /// </summary>
        public static string StringArrayToString(string[] stringArray)
        {
            return HasData(stringArray) ? string.Concat(stringArray) : string.Empty;
        }

        /// <summary>
        /// Behave exactly as ArrayList.Contains(). Full duplicate of above method.
        /// </summary>
        public static bool Contains(string[] array, string value)
        {
            if (value == null)
            {
                foreach (string item in array)
                {
                    if (item == null)
                        return true;
                }
            }
            else
            {
                foreach (string item in array)
                {
                    if (value.Equals(item, StringComparison.Ordinal))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Behave exactly as ArrayList.Contains().
        /// </summary>
        public static bool Contains(byte[] array, byte value)
        {
            foreach (byte item in array)
            {
                if (value == item)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Behave exactly as ArrayList.Contains().
        /// </summary>
        public static bool Contains(int[] array, int value)
        {
            foreach (int item in array)
            {
                if (value == item)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if source array contains target array.
        /// </summary>
        public static bool Contains(byte[] source, byte[] target)
        {
            if (source == null || target == null)
                return false;

            byte first = target[0];
            int srcLength = source.Length;
            int targetLength = target.Length;

            if (targetLength > srcLength)
                return false;

            for (int i = 0; i < srcLength; i++)
            {
                /* Look for first character. */
                if (source[i] != first)
                    continue;

                /* Found first character, now look at the rest of v2 */
                int j = i + 1;
                int end = j + targetLength - 1;
                if (end > srcLength)
                    return false;

                for (int k = 1; j < end; j++, k++)
                {
                    if (source[j] != target[k])
                        return false;
                }

                /* Found whole string. */
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a position of subarray in a source array.
        /// </summary>
        public static int IndexOf(byte[] source, byte[] subarray)
        {
            return IndexOf(source, subarray, 0);
        }

        /// <summary>
        /// Returns a position of subarray in a source array.
        /// </summary>
        public static int IndexOf(byte[] source, byte[] subarray, int sourceStartIndex)
        {
            return IndexOf(source, subarray, sourceStartIndex, source.Length);
        }

        /// <summary>
        /// Returns a position of subarray in a source array.
        /// </summary>
        public static int IndexOf(byte[] source, byte[] subarray, int sourceStartIndex, int sourceEndIndex)
        {
            int endIndex = Math.Min(source.Length - subarray.Length + 1, sourceEndIndex);
            for (int i = sourceStartIndex; i < endIndex; i++)
            {
                if (source[i] == subarray[0])
                {
                    bool found = true;

                    for (int j = 1; j < subarray.Length; j++)
                    {
                        if (source[i + j] != subarray[j])
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Added for debug
        /// </summary>
        public static string ArrayToString(byte[] data, int start, int count)
        {
            StringBuilder builder = new StringBuilder();
            while (count > 0)
            {
                char c = (char)data[start];
                uint codePoint = (uint)data[start];
                if (codePoint <= 0x9F && (codePoint >> 5 == 0))
                    c = '.';
                else if (c > 127)
                    c = (char)(data[start] + 1040 - 128);//Cyrillic 'A' so the output will be as in WinHex
                builder.Append(c);
                start++;
                count--;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Added for debug in Java
        /// </summary>
        public static string ArrayToStringCombined(byte[] data, int start, int count)
        {
            StringBuilder builder = new StringBuilder();
            count -= start;
            int len = 16;
            int i = start;
            while (i < count)
            {
                if (i + len > count)
                    len = count - i;

                builder.Append(DumpArray(data, i, len)).
                        Append(" | ").
                        Append(ArrayToString(data, i, len)).
                        Append('\n');

                i += 16;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Compares the specified char with each member of array of chars until the first successful comparison.
        /// </summary>
        public static bool FindCharInArray(char[] chars, char sample)
        {
            foreach (char c in chars)
            {
                if (sample == c)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Fills byte array in natural order. It's different from <see ref="Guid.ToByteArray"/>.
        /// </summary>
        public static byte[] GuidToByteArray(Guid guid)
        {
            byte[] guidBytes = guid.ToByteArray();

            Array.Reverse(guidBytes, 0, 4);
            Array.Reverse(guidBytes, 4, 2);
            Array.Reverse(guidBytes, 6, 2);

            return guidBytes;
        }

        /// <summary>
        /// Writes 32-bit unsigned integer represented as BigEndian sequence of 4 bytes to byte array at the specified offset.
        /// </summary>
        /// <param name="value">The 32-bit unsigned integer value to write to byte array.</param>
        /// <param name="bytes">The one-dimensional array of bytes./></param>
        /// <param name="offset">A 32-bit integer that represents the index in the byte array at which copying begins.</param>
        public static void WriteUInt32ToByteArray(int value, byte[] bytes, int offset)
        {
            for (int i = 0; i < 4; i++)
            {
                bytes[offset + i] = (byte)value;
                value >>= 8;
            }
        }

        /// <summary>
        /// Returns the index of the first occurrence of a <paramref name="value"/> in an <paramref name="array"/>.
        /// Uses a case-insensitive search.
        /// </summary>
        public static int IndexOfIgnoreCase(string[] array, string value)
        {
            for (int i = 0; i < array.Length; i++)
                if (StringUtil.EqualsIgnoreCase(array[i], value))
                    return i;

            return -1;
        }

        /// <summary>
        /// Concatenates two arrays of type T and returns an array with the combined elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the arrays.</typeparam>
        /// <param name="first">The first array.</param>
        /// <param name="second">The second array.</param>
        /// <returns>An array containing the elements of both input arrays in the order they were passed.</returns>
        public static T[] Concat<T>(T[] first, T[] second, T[] result)
        {
            if (first == null)
                return second;

            if (second == null)
                return first;

            if (result.Length == first.Length + second.Length)
            {
                first.CopyTo(result, 0);
                second.CopyTo(result, first.Length);

                return result;
            }

            return null;
        }

        public static int[] Concat(int[] first, int[] second)
        {
            if (first == null)
                return second;

            if (second == null)
                return first;

            int[] result = new int[first.Length + second.Length];

            first.CopyTo(result, 0);
            second.CopyTo(result, first.Length);

            return result;
        }

        public static int ConcatLength<T>(T[] first, T[] second)
        {
            if (second == null && first == null)
                return 0;

            if (first == null)
                return second.Length;

            if (second == null)
                return first.Length;

            return first.Length + second.Length;
        }

        public static readonly byte[] EmptyByteArray = { };
        public static readonly string[] EmptyStringArray = { };
        public static readonly object[] EmptyObjectArray = { };

        private const string ExceptionArrayLengths = "Array lengths are different: {0} and {1}.";
        private const string ExceptionArrayValues = "Array values are different at position 0x{0:X2}, 0x{1:X2}, 0x{2:X2}.";
    }
}
