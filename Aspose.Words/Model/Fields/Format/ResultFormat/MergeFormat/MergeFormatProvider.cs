// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/12/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides the MERGEFORMAT functionality.
    /// </summary>
    internal class MergeFormatProvider : IFieldResultFormatProvider
    {
        internal MergeFormatProvider(Field field)
        {
            mField = field;
        }

        public Inline GetSourceNode()
        {
            foreach (Node node in mField.GetFieldResultRange())
            {
                if (node.NodeType == NodeType.Run)
                {
                    // WORDSNET-4384 Skip merge field opening bracket as it might be in different
                    // formatting from what the user wants (especially when using embedded fonts).
                    // WORDSNET-4384 Cancelled, as this is the way MS Word does it.
                    return (Inline)node;
                }
            }

            // Maybe we should traverse it from the very start of the paragraph?
            // E.g. we could consider only runs. This could handle only some weird cases where two
            // fields go adjascent and field end of the first field has improper formatting.
            Inline prevInline = mField.Start.PreviousSibling as Inline;
            if (prevInline != null)
                return prevInline;

            // The field has no result or no runs, there is no previous node. Lets return properties of the field end.
            return mField.End;
        }

        public IFieldResultFormatApplier GetFormatApplier()
        {
            if (FieldUtil.GetSeparatorPresence(mField.Type) != FieldSeparatorPresence.Never)
            {
                mField.EnsureSeparator(true);

                // WORDSNET-11595 The INCLUDEPICTURE field applies MERGEFORMAT switch in own way.
                if (FieldUtil.IsImageFieldResult(mField.Type))
                    return new ImageResultFieldMergeFormatApplier(mField.OldResultNodes);

                return new MergeFormatApplier(mField.OldResultNodes, mField.OldResultStartParagraph, mField.OldResultEndParagraph);
            }
            else
            {
                return null;
            }
        }

        private readonly Field mField;
    }
}
