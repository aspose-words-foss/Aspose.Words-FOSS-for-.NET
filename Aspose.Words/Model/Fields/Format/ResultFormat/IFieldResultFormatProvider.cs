// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/12/2011 by Dmitry Vorobyev

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// When implemented, provides necessary functionality related to field result formatting.
    /// </summary>
    internal interface IFieldResultFormatProvider
    {
        /// <summary>
        /// Returns the inline node that is taken as a formatting pattern for the field result.
        /// </summary>
        /// <returns></returns>
        Inline GetSourceNode();

        /// <summary>
        /// Returns an object that applies result format to the field result.
        /// Returns null if no result format is specified.
        /// </summary>
        /// <returns></returns>
        [JavaThrows(true)]
        IFieldResultFormatApplier GetFormatApplier();
    }
}
