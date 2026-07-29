// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Specifies a constant type.
    /// </summary>
    internal enum ConstantType
    {
        /// <summary>
        /// The constant has no value.
        /// </summary>
        Null,
        /// <summary>
        /// The constant has an integer numeric value.
        /// </summary>
        Int32,
        /// <summary>
        /// The constant has a floating point numeric value.
        /// </summary>
        Double,
        /// <summary>
        /// The constant has a date/time value.
        /// </summary>
        DateTime,
        /// <summary>
        /// The constant has a boolean value.
        /// </summary>
        Boolean,
        /// <summary>
        /// The constant has a text value.
        /// </summary>
        String,
        /// <summary>
        /// The constant has a value that consists of other constants.
        /// </summary>
        Aggregate,
        /// <summary>
        /// The constant represents an erroneous evaluation result.
        /// </summary>
        Error,
        /// <summary>
        /// The constant represents a reference to cell, which does not exist.
        /// </summary>
        NullCellReference
    }
}
