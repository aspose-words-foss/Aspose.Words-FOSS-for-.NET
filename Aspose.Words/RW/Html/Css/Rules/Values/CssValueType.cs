// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// These are major categories of CSS value types.
    /// </summary>
    internal enum CssValueType
    {
        /// <summary>
        /// The property value is an identifier such as 'xx-large' (no quotes). Stored as a string.
        /// This is a default value used for any property value that we don't recognize.
        /// </summary>
        Identifier,
        /// <summary>
        /// The property value is a text string. 
        /// </summary>
        String,
        /// <summary>
        /// The property value is a hash string (a continuous text prefixed by the hash symbol '#').
        /// </summary>
        /// <remarks>
        /// The string is not necessarily a hex number. It's up to the calling code to correctly interpret the value.
        /// </remarks>
        Hash,
        /// <summary>
        /// The property value is a number without units. Stored as double.
        /// </summary>
        Number,
        /// <summary>
        /// The property value is an absolute length with units. Stored as double. 
        /// Examine unit type to find out what units the value is in.
        /// </summary>
        Length,
        /// <summary>
        /// The property value is a number of percent. Stored as double.
        /// </summary>
        Percentage,
        /// <summary>
        /// The property value is a URI value (Uniform Resource Identifiers, see [RFC3986], which includes URLs, URNs, etc).
        /// </summary>
        Uri,
        /// <summary>
        /// , char
        /// </summary>
        Comma,
        /// <summary>
        /// U+002F SOLIDUS (/)
        /// </summary>
        Solidus,
        /// <summary>
        /// The property value is a function such as counter(cnt1).
        /// </summary>
        Function,
        /// <summary>
        /// The property value is a degree such as 90deg. Stored as double.
        /// </summary>
        Degree
    }
}
