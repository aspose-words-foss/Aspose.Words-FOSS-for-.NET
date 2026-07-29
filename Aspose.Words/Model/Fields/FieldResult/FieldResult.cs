// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2009 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// A field result represented by a constant.
    /// </summary>
    internal class FieldResult : IFieldResult
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="value"></param>
        internal FieldResult(Constant value)
        {
            mValue = value;
        }

        Constant IFieldResult.GetFieldResultValue()
        {
           return mValue;
        }

        NodeRange IFieldResult.GetFieldResultRange()
        {
            return null;
        }

        private readonly Constant mValue;
    }
}
