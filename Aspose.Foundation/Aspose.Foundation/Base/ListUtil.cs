// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2017 by Alexey Butalov

using System.Collections.Generic;

namespace Aspose
{
    /// <summary>
    /// Provides utility methods to work with <see cref="List{T}"/> class.
    /// </summary>
    public static class ListUtil
    {
        /// <summary>
        /// Increases the capacity of the specified list, if necessary, to ensure that it can hold at least 
        /// the number of elements specified by the minimum capacity argument.
        /// </summary>
        public static void EnsureCapacity<T>(List<T> list, int capacity)
        {
            ListUtilPal.EnsureCapacity(list, capacity);
        }

        public static void AddRange<T>(List<T> destination, List<T> source)
        {
            destination.AddRange(source);
        }

        public static void AddRange<TD, TS>(List<TD> destination, IEnumerable<TS> source)
            where TS : TD
        {
            foreach (TS item in source)
                destination.Add(item);
        }

        /// <summary>
        /// Checks whether values in the lists are equals.
        /// </summary>
        public static bool CheckAreEquals<T>(IList<T> arg1, IList<T> arg2)
        {
            if (arg1.Count != arg2.Count)
                return false;

            bool valuesAreEquals = true;
            for (int i = 0; i < arg1.Count; i++)
            {
                valuesAreEquals &= object.Equals(arg1[i], arg2[i]);
                if (!valuesAreEquals)
                    break;
            }

            return valuesAreEquals;
        }

        /// <summary>
        /// Returns an List containing the elements of the specified collection in reverse order.
        /// </summary>
        public static List<T> Reverse<T>(IEnumerable<T> collection)
        {
            List<T> list = new List<T>(collection);
            list.Reverse();
            return list;
        }

        public static List<T> Repeat<T>(T value, int count)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < count; i++)
                list.Add(value);
            return list;
        }

        public static void Sort(List<string> strings)
        {
            ListUtilPal.Sort(strings);
        }
    }
}
