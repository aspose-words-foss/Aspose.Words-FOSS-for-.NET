// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/05/2017 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Applies <see cref="FontAttr.NoProofing"/> attribute to a field result.
    /// </summary>
    internal class NoProofingFormatApplier : IFieldResultFormatApplier
    {
        private NoProofingFormatApplier()
        {
        }

        public void ApplyFormat(NodeRange result)
        {
            foreach (Node node in result)
            {
                Inline inline = node as Inline;
                if (inline == null)
                    continue;

                inline.Font.NoProofing = true;
            }
        }

        internal static readonly NoProofingFormatApplier Instance = new NoProofingFormatApplier();
    }
}