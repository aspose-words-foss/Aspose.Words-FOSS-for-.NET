// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/03/2012 by Daria

using System;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Enumerates the parameter properties.
    /// </summary>
    [Flags]
    internal enum ParameterPeculiarities
    {
        /// <summary>
        /// Specifies that parameter value doesn't have specific flags.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that the parameter is a radix-10 number: in additional to digits
        /// can contain only negative sign, currency and/or fraction chars.
        /// </summary>
        Number = 1,

        /// <summary>
        /// Indicates that the parameter is a fractional number.
        /// </summary>
        Fractional = 2,

        /// <summary>
        /// Indicates that the parameter value is a negative number.
        /// </summary>
        Negative = 4,

        /// <summary>
        /// Indicates that a currency sign appears at the beginning of parameter.
        /// </summary>
        CurrencyAtStart = 8,

        /// <summary>
        /// Indicates that a currency sign appears at the end of parameter.
        /// </summary>
        CurrencyAtEnd = 16,

        /// <summary>
        /// Indicates that the parameter is preceded by a space.
        /// </summary>
        SpaceBefore = 32,

        /// <summary>
        /// Indicates that the parameter is followed by a space.
        /// </summary>
        SpaceAfter = 64,

        /// <summary>
        /// Indicates that the parameter contains one of the operators supported by Word.
        /// </summary>
        ContainsOperator = 128
    }
}
