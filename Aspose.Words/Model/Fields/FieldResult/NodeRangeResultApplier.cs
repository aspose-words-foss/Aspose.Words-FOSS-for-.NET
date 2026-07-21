// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/01/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Applies a field result represented by a node range.
    /// </summary>
    internal class NodeRangeResultApplier : FieldResultApplier
    {
        internal NodeRangeResultApplier(Field field, NodeRange result) : base(field)
        {
            mResult = result;
        }

        protected override void ApplyResultCore()
        {
            // WORDSNET-6625 If a NodeRange is empty or void need just to exit because NodeCopier doesn't validate
            // input parameters and empty NodeRange leads to NullReferenceException.
            if (mResult == null)
                return;

            if (mResult.IsVoid || mResult.IsEmpty)
                return;

            FieldTokenDecoderOptions decoderOptions = FieldTokenDecoderOptions.None;
            decoderOptions = FieldTokenDecoderOptionsUtil.WithTrimDoubleQuotes(decoderOptions, !FieldUtil.PreserveDoubleQuotesInResult(Field.Type));
            decoderOptions = FieldTokenDecoderOptionsUtil.WithEscapeChars(decoderOptions, !FieldUtil.PreserveEscapingBackslashesInResult(Field.Type));
            CompositeModifier modifier = new CompositeModifier(

                new FieldTokenDecoderNodeModifier(mResult, decoderOptions),
                new CommentIdsRegeneratorNodeModifier(),
                Field.GetResultModifier());

            // WORDSNET-15968 There is a contract between Field Engine and Layout: field updating should not affect nodes outside field results to make layout model sync with DOM.
            // Layout considers field end parent paragraph as not a part of field result, even if it holds result nodes.
            // NodeCopier.CopyWithoutFields splits field end paragraph and moves field end into new one. This breaks the contract.
            Paragraph endParagraph = PreserveFieldEndParentParagraph();

            // Copy nodes from the source to the destination, decoding nodes.
            const NodeCopierOptions options = NodeCopierOptions.UseSourceStartAncestorPr |
                                              NodeCopierOptions.SkipCrossStructureAnnotations |
                                              NodeCopierOptions.CloneNode |
                                              NodeCopierOptions.ProlongRangeStartSectionHeadersFooters;
            NodeCopier.CopyWithoutFields(mResult, Field.End, modifier, null, false, options);

            RestoreFieldEndParentParagraph(endParagraph);
        }

        private Paragraph PreserveFieldEndParentParagraph()
        {
            if (Field.Start.ParentNode == Field.End.ParentNode)
                return null;

            if (Field.End.ParentNode.NodeType != NodeType.Paragraph)
                return null;

            Paragraph endParagraph = (Paragraph)Field.End.ParentNode;
            Paragraph endParagraphClone = (Paragraph)endParagraph.Clone(false);

            endParagraphClone.InsertAfter(endParagraph.FirstChild, null, null);
            endParagraph.InsertPrevious(endParagraphClone);

            return endParagraph;
        }

        private void RestoreFieldEndParentParagraph(Paragraph endParagraph)
        {
            if (endParagraph == null)
                return;

            Paragraph endParagraphClone = (Paragraph)Field.End.ParentNode;

            endParagraph.InsertAfter(endParagraphClone.FirstChild, null, null);
            endParagraph.ParaPr = endParagraphClone.ParaPr;
            endParagraph.ParagraphBreakRunPr = endParagraphClone.ParagraphBreakRunPr;

            endParagraphClone.Remove();
        }

        private readonly NodeRange mResult;
    }
}
