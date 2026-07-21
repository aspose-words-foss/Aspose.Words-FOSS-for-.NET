// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/05/2024 by Edward Voronov

using System;
using Aspose.Words.Markup;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Applies CHARFORMAT to a field result.
    /// </summary>
    internal class CharFormatApplier : DefaultFormatApplier
    {
        internal CharFormatApplier(Inline formattingSource)
            : base(formattingSource.GetExpandedRunPr(RunPrExpandFlags.Normal), new[] { FontAttr.Bidi })
        {
        }

        protected override void ApplyFormatCore(NodeRange result)
        {
            base.ApplyFormatCore(result);

            Paragraph start = NodeUtil.GetAncestorOrSelf(result.Start.Node, NodeType.Paragraph) as Paragraph;
            if (start == null)
                return;

            Paragraph end = NodeUtil.GetAncestorOrSelf(result.End.Node, NodeType.Paragraph) as Paragraph;
            if (start == end)
                return;

            foreach (Node node in result)
            {
                if (node.NodeType != NodeType.Paragraph)
                    continue;

                Paragraph paragraph = (Paragraph)node;
                Paragraph previousParagraph = (Paragraph)paragraph.PreviousSiblingOfType(NodeType.Paragraph);
                if (previousParagraph != null)
                {
                    previousParagraph.ParagraphBreakRunPr = AdjustParagraphBreakRunPr(
                        previousParagraph.ParagraphBreakRunPr,
                        previousParagraph);
                }
            }
        }

        protected override RunPr AdjustRunPr(RunPr original, Node owner)
        {
            RunPr result = base.AdjustRunPr(original, owner);

            // Since the source formatting contains expanded values we may not need some of them
            // if the result node inherits the same values. Remove duplicated ones.
            RunPr inheritedAttrs = GetInheritedRunPrAttrs(owner);
            result.RemoveEquals(inheritedAttrs);

            return result;
        }

        private static RunPr GetInheritedRunPrAttrs(Node source)
        {
            const RunPrExpandFlags inheritedFormattingFlag = RunPrExpandFlags.NoDirectFormatting;

            RunPr inheritedAttrs;

            switch (source.NodeType)
            {
                case NodeType.Paragraph:
                    Paragraph paragraph = (Paragraph)source;
                    inheritedAttrs = paragraph.GetExpandedParagraphBreakRunPr(inheritedFormattingFlag);
                    break;
                case NodeType.Run:
                case NodeType.Shape:
                case NodeType.SpecialChar:
                    IInline inline = (IInline)source;
                    inheritedAttrs = inline.GetExpandedRunPr_IInline(inheritedFormattingFlag);
                    break;
                case NodeType.StructuredDocumentTag:
                    StructuredDocumentTag sdt = (StructuredDocumentTag)source;
                    inheritedAttrs = new RunPr();
                    InlineHelper.ExpandRunPr(
                        sdt.Document,
                        (Paragraph)sdt.GetAncestor(NodeType.Paragraph),
                        sdt.ContentsRunPr,
                        inheritedAttrs,
                        inheritedFormattingFlag);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // The style identifier itself is not an inherited attribute.
            inheritedAttrs.Remove(FontAttr.Istd);

            return inheritedAttrs;
        }

        private RunPr AdjustParagraphBreakRunPr(RunPr original, Node owner)
        {
            RunPr result = AdjustRunPr(original, owner);
            original.MirrorTo(result, FontAttr.RsidR, FontAttr.RsidRPr);
            return result;
        }
    }
}
