// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/01/2020 by Ivan Lyagin

using System.Diagnostics.CodeAnalysis;

namespace Aspose
{
    /// <summary>
    /// Represents a Boolean value that can be undefined.
    /// </summary>
    [SuppressMessage(
        "Microsoft.Design", 
        "CA1028:EnumStorageShouldBeInt32",
        Justification = "The enum was moved from Aspose.Zip where backing by sbyte can make sense to reduce memory consumption.")]
    public enum NullableBool : sbyte
    {
        /// <summary>
        /// Specifies an undefined value.
        /// </summary>
        NotDefined = -1,

        /// <summary>
        /// Specifies the value of false.
        /// </summary>
        False = 0,

        /// <summary>
        /// Specifies the value of true.
        /// </summary>
        True = 1
    }
}
