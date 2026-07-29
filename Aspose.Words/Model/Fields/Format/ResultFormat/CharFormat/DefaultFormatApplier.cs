// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/05/2016 by Edward Voronov

using Aspose.Words.Markup;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Applies default format to a field result.
    /// </summary>
    /// <remarks>
    /// Used for fields without any general formats.
    /// </remarks>
    internal class DefaultFormatApplier : IFieldResultFormatApplier
    {
        internal DefaultFormatApplier(Inline formattingSource)
            : this(formattingSource.RunPr, new[] { FontAttr.Bidi, FontAttr.Hidden })
        {
        }

        protected DefaultFormatApplier(RunPr formattingSourceRunPr, int[] ignoredAttributes)
        {
            mFormattingSourceRunPr = formattingSourceRunPr;
            mIgnoredAttributes = ignoredAttributes;
        }

        void IFieldResultFormatApplier.ApplyFormat(NodeRange result)
        {
            ApplyFormatCore(result);
        }

        protected virtual void ApplyFormatCore(NodeRange result)
        {
            foreach (Node node in result)
            {
                switch (node.NodeType)
                {
                    case NodeType.Run:
                    case NodeType.Shape:
                    case NodeType.SpecialChar:
                        IInline inline = (IInline)node;
                        inline.RunPr_IInline = AdjustRunPr(inline.RunPr_IInline, node);
                        break;
                    case NodeType.StructuredDocumentTag:
                        StructuredDocumentTag sdt = (StructuredDocumentTag)node;
                        sdt.ContentsRunPr = AdjustRunPr(sdt.ContentsRunPr, node);
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }
        }

        protected virtual RunPr AdjustRunPr(RunPr original, Node owner)
        {
            RunPr sourceClone = mFormattingSourceRunPr.Clone();
            original.MirrorTo(sourceClone, mIgnoredAttributes);
            return sourceClone;
        }

        private readonly RunPr mFormattingSourceRunPr;
        private readonly int[] mIgnoredAttributes;
    }
}
