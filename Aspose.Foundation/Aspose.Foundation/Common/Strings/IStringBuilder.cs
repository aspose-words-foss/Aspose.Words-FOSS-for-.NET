// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2020 by Edward Voronov

using System.Collections.Generic;

namespace Aspose.Common
{
    /// <summary>Represents a mutable <see cref="IString"/> string of <see cref="IChar"/> characters.</summary>
    public interface IStringBuilder
    {
        /// <summary>Gets or sets the length of the current <see cref="IStringBuilder" /> object.</summary>
        int Length { get; set; }

        /// <summary>Gets or sets the character at the specified character position in this instance.</summary>
        IChar this[int index] { get; set; }

        /// <summary>Converts the value of this instance to a <see cref="IString" />.</summary>
        IString ToIString();

        /// <summary>Appends the string representation of a specified character to this instance.</summary>
        IStringBuilder Append(IChar c);

        /// <summary>Appends a copy of the specified string to this instance.</summary>
        IStringBuilder Append(IString s);

        /// <summary>Appends the string representation of a specified character to this instance.</summary>
        IStringBuilder Append(char c, IChar source);

        IStringBuilder Append(char c, int count, IChar source);

        /// <summary>Appends a copy of the specified string to this instance.</summary>
        IStringBuilder Append(string s, IChar source);

        /// <summary>Inserts the string representation of a specified character into this instance at the specified character position.</summary>
        IStringBuilder Insert(int index, char c, IChar source);

        /// <summary>Inserts a string into this instance at the specified character position.</summary>
        IStringBuilder Insert(int index, string s, IChar source);

        /// <summary>Inserts one or more copies of a specified string into this instance at the specified character position.</summary>
        IStringBuilder Insert(int index, string s, int count, IList<IChar> sources);

        /// <summary>Replaces all occurrences of a specified string in this instance with another specified string.</summary>
        IStringBuilder Replace(string oldValue, string newValue);

        /// <summary>Replaces the character at the specified character position in this instance.</summary>
        IStringBuilder Replace(int index, char c, IChar source);

        /// <summary>Removes the specified range of characters from this instance.</summary>
        IStringBuilder Remove(int startIndex, int length);
    }
}
