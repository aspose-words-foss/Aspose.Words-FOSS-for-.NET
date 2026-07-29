// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/03/2010 by Dmitry Vorobyev

using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Applies MERGEFORMAT to a field result.
    /// </summary>
    /// <remarks>
    /// MERGEFORMAT applies run and paragraph formatting taken from the "old" result to the "new" result. It splits
    /// the results to tokens, such as words, whitespace, and separators, and attempts to apply formatting on
    /// token to tokem basis. Given we have both results represented by a jagged range of nodes, correct merge
    /// of formatting requires a pretty sophisticated algorithm.
    /// </remarks>
    internal class MergeFormatApplier : IFieldResultFormatApplier
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal MergeFormatApplier(IEnumerable<Node> oldResultNodes,
            Paragraph oldStartParagraph, Paragraph oldEndParagraph)
        {
            mOldResultNodes = oldResultNodes;
            mOldStartParagraph = oldStartParagraph;
            mOldEndParagraph = oldEndParagraph;
        }

        void IFieldResultFormatApplier.ApplyFormat(NodeRange result)
        {
            OldResultEnumerator oldResultEnumerator = new OldResultEnumerator(mOldResultNodes, mOldStartParagraph);

            Paragraph newResultStartParagraph = (Paragraph)result.Start.Node.GetAncestor(NodeType.Paragraph);
            Paragraph newResultEndParagraph = (Paragraph)result.End.Node.GetAncestor(NodeType.Paragraph);

            NewResultEnumerator.MergeFormat(
                result,
                newResultStartParagraph,
                newResultEndParagraph,
                oldResultEnumerator,
                mOldEndParagraph);
        }

        /// <summary>
        /// An collection containing runs and paragraphs that comprised old result.
        /// </summary>
        private readonly IEnumerable<Node> mOldResultNodes;
        /// <summary>
        /// The paragraph that contained field's start.
        /// </summary>
        private readonly Paragraph mOldStartParagraph;
        /// <summary>
        /// The paragraph that contained field's end.
        /// </summary>
        private readonly Paragraph mOldEndParagraph;
    }
}
