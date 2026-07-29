// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Contains a node range used to display the result of the field. Required by layout engine.
    /// </summary>
    internal class FieldDisplayResult
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="resultType"></param>
        internal FieldDisplayResult(NodeRange range, FieldDisplayRangeType resultType)
        {
            Range = range;
            RangeType = resultType;
        }

        /// <summary>
        /// Gets a node range containing nodes to display.
        /// </summary>
        internal NodeRange Range { get; }

        /// <summary>
        /// Gets the meaning of the display range.
        /// </summary>
        internal FieldDisplayRangeType RangeType { get; }
    }
}
