// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/12/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides the CHARFORMAT functionality.
    /// </summary>
    internal class CharFormatProvider : IFieldResultFormatProvider
    {
        internal CharFormatProvider(Field field)
        {
            mField = field;
        }

        public Inline GetSourceNode()
        {
            bool allowHidden = mField.Start.IsHiddenOrDeleted;
            foreach (Node node in mField.GetFieldCodeRange())
            {
                if (node.NodeType == NodeType.Run)
                {
                    Run run = (Run)node;

                    if (run.IsHiddenOrDeleted && !allowHidden)
                        continue;

                    // Leading spacing is not taken into account.
                    if (!StringUtil.ContainsOnlySpaces(run.Text))
                        return run;
                }
            }

            // I have never seen fields without field code but should it happen, let's take formatting from the separator.
            return mField.Separator;
        }

        public IFieldResultFormatApplier GetFormatApplier()
        {
            Inline sourceNode = GetSourceNode();

            if (sourceNode == null)
                return DoNothingFieldResultFormatApplier.Instance;

            // WORDSNET-13464 The CHARFORMAT formatting is similar to default formatting, but is not equal to.
            return mField.Format.GeneralFormats.HasFormat(GeneralFormat.CharFormat)
                ? new CharFormatApplier(sourceNode)
                : new DefaultFormatApplier(sourceNode);
        }

        private readonly Field mField;

        private class DoNothingFieldResultFormatApplier : IFieldResultFormatApplier
        {
            private DoNothingFieldResultFormatApplier()
            {
            }

            void IFieldResultFormatApplier.ApplyFormat(NodeRange result)
            {
                // Do nothing.
            }

            internal static readonly IFieldResultFormatApplier Instance = new DoNothingFieldResultFormatApplier();
        }
    }
}
