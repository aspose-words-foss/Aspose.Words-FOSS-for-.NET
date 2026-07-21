// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2020 by Edward Voronov

using System.Collections.Generic;

namespace Aspose.Common
{
    /// <summary>Represents text as a sequence of <see cref="IChar"/> objects.</summary>
    public interface IString : IEnumerable<IChar>
    {
        /// <summary>Gets the number of characters in the current <see cref="IString" /> object.</summary>
        int Length { get; }

        /// <summary>Gets the <see cref="IChar" /> object at a specified position in the current <see cref="IString" /> object.</summary>
        IChar this[int index] { get; }

        /// <summary>Reports the zero-based index of the first occurrence of the specified string in this instance.</summary>
        int IndexOf(string value);

        /// <summary>Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.</summary>
        IString Replace(string oldValue, string newValue);

        /// <summary>Returns a new string in which a specified number of characters in the current instance beginning at a specified position have been deleted.</summary>
        IString Remove(int startIndex, int count);

        /// <summary>Returns a copy of this string converted to uppercase.</summary>
        IString ToUpper();

        /// <summary>Returns a copy of this string converted to lowercase.</summary>
        IString ToLower();

        /// <summary>Converts the value of this instance to its equivalent string representation.</summary>
        string ToSystemString();
    }
}
