// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/03/2015 by Edward Voronov

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// A field result represented by a node range.
    /// </summary>
    internal class NodeRangeFieldResult : IFieldResult
    {
        internal NodeRangeFieldResult(NodeRange nodeRange)
        {
            mNodeRange = nodeRange;
        }

        Constant IFieldResult.GetFieldResultValue()
        {
            return new StringConstant(NodeTextCollector.GetText(mNodeRange, true));
        }

        NodeRange IFieldResult.GetFieldResultRange()
        {
            return mNodeRange;
        }

        private readonly NodeRange mNodeRange;
    }
}
