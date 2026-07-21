// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/05/2017 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides special formatting for <see cref="FieldMergeField"/> field result.
    /// </summary>
    internal class NoProofingFormatProvider : IFieldResultFormatProvider
    {
        private NoProofingFormatProvider()
        {
        }

        public Inline GetSourceNode()
        {
            return null;
        }

        public IFieldResultFormatApplier GetFormatApplier()
        {
            return NoProofingFormatApplier.Instance;
        }

        internal static readonly NoProofingFormatProvider Instance = new NoProofingFormatProvider();
    }
}