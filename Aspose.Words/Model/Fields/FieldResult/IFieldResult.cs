// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2009 by Dmitry Vorobyev

using Aspose.JavaAttributes;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a value used to set field result. The value may be represented by a constant or a
    /// formatted content. Field result may be either calculated (thus being a constant) or taken from
    /// a field argument (thus being a plain text or formatted nodes, depending on what we currently need).
    /// </summary>
    internal interface IFieldResult
    {
        /// <summary>
        /// Returns a constant that represents a result of field evaluation.
        /// </summary>
        [JavaThrows(true)]
        Constant GetFieldResultValue();

        /// <summary>
        /// Returns a node range representation of field result.
        /// </summary>
        NodeRange GetFieldResultRange();
    }
}
