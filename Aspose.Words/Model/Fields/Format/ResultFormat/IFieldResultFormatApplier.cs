// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/03/2010 by Dmitry Vorobyev

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// When implemented, applies formatting to a field result.
    /// </summary>
    internal interface IFieldResultFormatApplier
    {
        /// <summary>
        /// Applies result formatting to the specified field result.
        /// </summary>
        /// <param name="result"></param>
        [JavaThrows(true)]
        void ApplyFormat(NodeRange result);
    }
}
