// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/08/2005 by Roman Korchagin

namespace Aspose.Words.Properties
{
    /// <summary>
    /// Specifies data type of a document property.
    /// </summary>
    /// <seealso cref="DocumentProperty"/>
    /// <seealso cref="DocumentProperty.Type"/>
    public enum PropertyType
    {
        /// <summary>
        /// The property is a boolean value.
        /// </summary>
        Boolean,

        /// <summary>
        /// The property is a date time value.
        /// </summary>
        DateTime,

        /// <summary>
        /// The property is a floating number.
        /// </summary>
        Double,

        /// <summary>
        /// The property is an integer number.
        /// </summary>
        Number,

        /// <summary>
        /// The property is a string value.
        /// </summary>
        String,

        /// <summary>
        /// The property is an array of strings.
        /// </summary>
        StringArray,

        /// <summary>
        /// The property is an array of objects.
        /// </summary>
        ObjectArray,

        /// <summary>
        /// The property is an array of bytes.
        /// </summary>
        ByteArray,

        /// <summary>
        /// The property is some other type.
        /// </summary>
        Other
    }
}
