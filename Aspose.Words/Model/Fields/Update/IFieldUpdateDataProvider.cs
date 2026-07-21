// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/06/2010 by Dmitry Vorobyev

using Aspose.JavaAttributes;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// When implemented, provides a field value or other data for a field that is unable to update itself. For example,
    /// merge fields need to take their value from a mail merge region.
    /// </summary>
    internal interface IFieldUpdateDataProvider
    {
        /// <summary>
        /// Returns a field value (represented by a constant) for the specified field or null if impossible.
        /// </summary>
        [JavaThrows(true)]
        Constant GetValue(Field field);

        /// <summary>
        /// Returns an arbitrary data for the specified field or null if impossible.
        /// </summary>
        object GetData(Field field);
    }
}

